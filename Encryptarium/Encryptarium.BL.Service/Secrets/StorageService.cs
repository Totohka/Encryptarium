using Encryptarium.Core.Service.Mapper;
using Encryptarium.Core.Service.Roles;
using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Storages;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Encryptarium.BL.Service.Secrets
{
    public interface IStorageService
    {
        IAsyncEnumerable<StorageDto> GetStoragesAsync(int userId);
        Task CreateStorageAsync(StorageModel storageModel, int userId);
        Task CreateStorageAsync(StorageModel storageModel, int roleId, int userId);
        Task UpdateStorageAsync(Guid storageId, StorageModel storageModel, int userId);
        Task DeleteStorageAsync(Guid storageId, int userId);
    }

    public class StorageService : IStorageService
    {
        private readonly IXBaseRepository _xBaseRepository;
        private readonly IBaseRepository<Guid> _baseUidRepository;
        private readonly IBaseRepository<int> _baseIntRepository;
        private readonly IRoleCoreService _roleCoreService;
        public StorageService(IXBaseRepository xBaseRepository, IBaseRepository<Guid> baseUidRepository, IBaseRepository<int> baseIntRepository, IRoleCoreService roleCoreService) 
        {
            _xBaseRepository = xBaseRepository;
            _baseUidRepository = baseUidRepository;
            _baseIntRepository = baseIntRepository;
            _roleCoreService = roleCoreService;
        }

        public async Task CreateStorageAsync(StorageModel storageModel, int userId)
        {
            if (storageModel.StorageType == StorageTypes.Private)
            {
                var privateRole = await _baseIntRepository.GetAll<eRole>()
                    .FirstOrDefaultAsync(r => r.UserRoles.Select(ur => ur.UserId).Contains(userId) && r.RoleType == RoleType.Private);

                var privateStorage = await _baseUidRepository.GetAll<eStorage>()
                    .FirstOrDefaultAsync(s => s.StorageRoles.Select(sr => sr.RoleId).Contains(privateRole.Id) && s.StorageType == StorageTypes.Private);

                if (privateStorage == null)
                {
                    await _baseUidRepository.CreateAsync(new eStorage
                    {
                        Name = storageModel.Name,
                        Description = storageModel.Description,
                        StorageType = StorageTypes.Private,
                        StorageRoles = new HashSet<xStorageRole> { new xStorageRole { RoleId = privateRole.Id } }
                    });
                }
            }
            else if (storageModel.StorageType == StorageTypes.Roles)
            {
                var rolesByUser = await _baseIntRepository.GetAll<eRole>()
                    .Include(r => r.FunctionalRulesStorage)
                    .Where(r => r.UserRoles.Any(ur => ur.UserId == userId) && r.RoleType == RoleType.Role)
                    .ToListAsync();

                bool accessCreate = rolesByUser
                    .Select(r => r.FunctionalRulesStorage.Select(r => r.Id))
                    .All(rules => rules.Contains((int)FunctionalRulesStorage.Add));

                if (accessCreate)
                {
                    var storage = storageModel.MapToEntity();
                    storage.StorageRoles.Add(new xStorageRole { RoleId = rolesByUser[0].Id });

                    await _baseUidRepository.CreateAsync(storage);
                }
            }
            else
            {
                return;
            }
        }

        public async Task CreateStorageAsync(StorageModel storageModel, int roleId, int userId)
        {
            if (storageModel.StorageType == StorageTypes.Roles)
            {
                var storage = storageModel.MapToEntity();
                storage.StorageRoles.Add(new xStorageRole { RoleId = roleId });

                await _baseUidRepository.CreateAsync(storage);
            }
        }

        public async Task DeleteStorageAsync(Guid storageId, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForStorageAsync(userId, storageId, FunctionalRulesStorage.Delete);
            
            if (access)
            {
                var storage = await _baseUidRepository.GetAsync<eStorage>(storageId);
                if (storage.StorageType == StorageTypes.Private)
                {
                    return;
                }

                await _baseUidRepository.DeleteAsync<eStorage>(storageId);
            }
        }

        public async IAsyncEnumerable<StorageDto> GetStoragesAsync(int userId)
        {
            var roleIds = _xBaseRepository.GetAll<xUserRole>()
                .Where(ur => ur.UserId == userId && ur.Role.FunctionalRulesStorage.Select(r => r.Id).Contains((int)FunctionalRulesStorage.View))
                .Select(ur => ur.RoleId);

            var storageIds = _xBaseRepository.GetAll<xStorageRole>()
                .Where(sr => roleIds.Contains(sr.RoleId))
                .Select(sr => sr.StorageId);

            var storages = await _baseUidRepository.GetAllAsync<eStorage>(s => storageIds.Contains(s.Id));

            foreach (var storage in storages)
            {
                yield return storage.MapToDto();
            }
        }

        public async Task UpdateStorageAsync(Guid storageId, StorageModel storageModel, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForStorageAsync(userId, storageId, FunctionalRulesStorage.Update);

            if (access)
            {
                var storage = await _baseUidRepository.GetAsync<eStorage>(storageId);

                storage.Description = storageModel.Description;
                storage.Name = storageModel.Name;
            }
        }
    }
}
