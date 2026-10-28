using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Encryptarium.Core.Service.Roles
{
    /// <summary>
    /// Сервис для работы с ролями
    /// </summary>
    public interface IRoleCoreService
    {
        /// <summary>
        /// Проверяет правила доступа к секрету
        /// </summary>
        /// <param name="userId">Id юзера</param>
        /// <param name="secretId">Uid секрета</param>
        /// <param name="ruleSecret">Правило доступа к секрету</param>
        /// <returns></returns>
        public Task<bool> CheckRoleAccessForSecretAsync(int userId, Guid? secretId, FunctionalRulesSecret ruleSecret, Guid? storageId = null);

        /// <summary>
        /// Проверяет правила доступа к контейнеру
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storageId"></param>
        /// <param name="ruleStorage"></param>
        /// <returns></returns>
        public Task<bool> CheckRoleAccessForStorageAsync(int userId, Guid storageId, FunctionalRulesStorage ruleStorage);

        /// <summary>
        /// Создать новую роль
        /// </summary>
        /// <param name="role"></param>
        /// <param name="secretRules"></param>
        /// <param name="storageRules"></param>
        /// <returns></returns>
        public Task<int> CreateNewRoleAsync(eRole role, HashSet<FunctionalRulesSecret> secretRules, HashSet<FunctionalRulesStorage> storageRules);

        /// <summary>
        /// Обновить роль
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="secretRules"></param>
        /// <param name="storageRules"></param>
        /// <returns></returns>
        public Task UpdateRoleAsync(int roleId, HashSet<FunctionalRulesSecret> secretRules, HashSet<FunctionalRulesStorage> storageRules);

        /// <summary>
        /// Присвоить роль пользователю
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task SetRoleForUserAsync(int roleId, int userId);

        /// <summary>
        /// Присвоить роль хранилищу
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task SetRoleForStorageAsync(int roleId, Guid storageId);

        /// <summary>
        /// Присвоить роль секрету
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task SetRoleForSecretAsync(int roleId, Guid secretId);

        /// <summary>
        /// Получить все роли пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<eRole>> GetRolesByUserAsync(int userId);
    }

    /// <inheritdoc cref="IRoleCoreService"/>
    public class RoleCoreService : IRoleCoreService
    {
        private readonly IBaseRepository<Guid> _baseUidRepository;
        private readonly IBaseRepository<int> _baseIntRepository;
        private readonly IXBaseRepository _xBaseRepository;

        public RoleCoreService(IBaseRepository<Guid> baseUidRepository, IBaseRepository<int> baseIntRepository, IXBaseRepository xBaseRepository)
        {
            _baseUidRepository = baseUidRepository;
            _baseIntRepository = baseIntRepository;
            _xBaseRepository = xBaseRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckRoleAccessForSecretAsync(int userId, Guid? secretId, FunctionalRulesSecret ruleSecret, Guid? storageId = null)
        {
            if (ruleSecret == FunctionalRulesSecret.Add)
            {
                return await GetAccessCreateSecretAsync(userId, storageId);
            }

            var query = _baseIntRepository.GetAll<eRole>();

            var rolesForUser = query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesStorage)
                .Include(role => role.FunctionalRulesSecret)
                .Where(role => role.UserRoles.Any(ur => ur.UserId == userId));

            if (!secretId.HasValue)
            {
                return false;
            }

            var secret = await _baseUidRepository.GetAsync<eSecret>(secretId.Value);
            Guid storageUid = secret.StorageId;

            if (!await GetAccessViewStorageAsync(rolesForUser, storageUid))
            {
                return false;
            }

            var rolesForSecret = await query
                .Include(role => role.SecretRoles)
                .Include(role => role.FunctionalRulesSecret)
                .Where(role => role.SecretRoles.Any(sr => sr.SecretId == secretId))
                .ToListAsync();

            var commonRulesForSecret = rolesForSecret.Intersect(rolesForUser).SelectMany(role => role.FunctionalRulesSecret);

            return commonRulesForSecret.Any(ru => ru.Id == (int)ruleSecret); 
        }

        private async Task<bool> GetAccessCreateSecretAsync(int userId, Guid? storageId)
        {
            if (!storageId.HasValue)
            {
                return false;
            }

            var query = _baseIntRepository.GetAll<eRole>();
            var rolesForUser = query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesStorage)
                .Include(role => role.FunctionalRulesSecret)
                .Where(role => role.UserRoles.Any(ur => ur.UserId == userId));

            if (!await GetAccessViewStorageAsync(rolesForUser, storageId.Value))
            {
                return false;
            }

            var rolesForStorage = await query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesSecret)
                .Where(role => role.StorageRoles.Any(sr => sr.StorageId == storageId))
                .ToListAsync();

            var commonRulesForStorage = rolesForStorage.Intersect(rolesForUser).SelectMany(role => role.FunctionalRulesSecret);

            if (!commonRulesForStorage.Any(r => r.Id == (int)FunctionalRulesSecret.Add))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> GetAccessViewStorageAsync(IQueryable<eRole> rolesForUser, Guid storageId)
        {
            var query = _baseIntRepository.GetAll<eRole>();

            var rolesForStorage = await query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesStorage)
                .Where(role => role.StorageRoles.Any(sr => sr.StorageId == storageId))
                .ToListAsync();

            var rolesForUserList = await rolesForUser.ToListAsync();

            var commonRulesForStorage = rolesForStorage.Intersect(rolesForUserList).SelectMany(role => role.FunctionalRulesStorage);
            // У юзера должен быть доступ хотя бы на чтение контейнера, чтобы делать что-то с секретом
            if (!commonRulesForStorage.Any(r => r.Id == (int)FunctionalRulesStorage.View))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> CheckRoleAccessForStorageAsync(int userId, Guid storageId, FunctionalRulesStorage ruleStorage)
        {
            var query = _baseIntRepository.GetAll<eRole>();
            var rolesForStorage = await query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesStorage)
                .Where(role => role.RoleType == RoleType.Private || (role.RoleType == RoleType.Role && role.StorageRoles.Any(sr => sr.StorageId == storageId)))
                .ToListAsync();

            var rolesForUser = await query
                .Include(role => role.StorageRoles)
                .Include(role => role.FunctionalRulesStorage)
                .Where(role => role.RoleType == RoleType.Private || (role.RoleType == RoleType.Role && role.UserRoles.Any(ur => ur.UserId == userId)))
                .ToListAsync();

            var commonRulesForSecret = rolesForStorage.Intersect(rolesForUser).SelectMany(role => role.FunctionalRulesStorage);

            return commonRulesForSecret.Any(ru => ru.Id == (int)ruleStorage);
        }

        /// <inheritdoc />
        public async Task<int> CreateNewRoleAsync(eRole role, HashSet<FunctionalRulesSecret> secretRuleIds, HashSet<FunctionalRulesStorage> storageRuleIds)
        {
            var secretRules = await _baseIntRepository.GetAllAsync<cFunctionalRuleSecret>();
            var storageRules = await _baseIntRepository.GetAllAsync<cFunctionalRuleStorage>();

            role.FunctionalRulesSecret = secretRules.Where(r => secretRuleIds.Cast<int>().Contains(r.Id)).ToHashSet();
            role.FunctionalRulesStorage = storageRules.Where(r => secretRuleIds.Cast<int>().Contains(r.Id)).ToHashSet();
            
            return await _baseIntRepository.CreateAsync(role);
        }

        /// <inheritdoc />
        public async Task SetRoleForSecretAsync(int roleId, Guid secretId)
        {
            var entity = await _xBaseRepository.GetAll<xSecretRole>().FirstOrDefaultAsync(ur => ur.RoleId == roleId && ur.SecretId == secretId);
            if (entity == null)
            {
                await _xBaseRepository.CreateAsync(new xSecretRole { RoleId = roleId, SecretId = secretId});
            }
        }

        /// <inheritdoc />
        public async Task SetRoleForStorageAsync(int roleId, Guid storageId)
        {
            var entity = await _xBaseRepository.GetAll<xStorageRole>().FirstOrDefaultAsync(ur => ur.RoleId == roleId && ur.StorageId == storageId);
            if (entity == null)
            {
                await _xBaseRepository.CreateAsync(new xStorageRole { RoleId = roleId, StorageId = storageId });
            }
        }

        /// <inheritdoc />
        public async Task SetRoleForUserAsync(int roleId, int userId)
        {
            var entity = await _xBaseRepository.GetAll<xUserRole>().FirstOrDefaultAsync(ur => ur.RoleId == roleId && ur.UserId == userId);
            if (entity == null)
            {
                await _xBaseRepository.CreateAsync(new xUserRole { RoleId = roleId, UserId = userId });
            }
        }

        /// <inheritdoc />
        public async Task UpdateRoleAsync(int roleId, HashSet<FunctionalRulesSecret> secretRuleIds, HashSet<FunctionalRulesStorage> storageRuleIds)
        {
            var role = await _baseIntRepository.GetAsync<eRole>(roleId);
            var secretRules = await _baseIntRepository.GetAllAsync<cFunctionalRuleSecret>();
            var storageRules = await _baseIntRepository.GetAllAsync<cFunctionalRuleStorage>();

            role.FunctionalRulesSecret = secretRules.Where(r => secretRuleIds.Cast<int>().Contains(r.Id)).ToHashSet();
            role.FunctionalRulesStorage = storageRules.Where(r => secretRuleIds.Cast<int>().Contains(r.Id)).ToHashSet();

            await _baseIntRepository.UpdateAsync(role);
        }

        public async Task<List<eRole>> GetRolesByUserAsync(int userId)
        {
            return await _baseIntRepository.GetAll<eRole>()
                .Where(r => r.UserRoles.Select(ur => ur.UserId).Contains(userId))
                .ToListAsync();
        }
    }
}
