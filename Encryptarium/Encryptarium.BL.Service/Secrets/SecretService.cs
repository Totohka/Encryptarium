using Encryptarium.Core.Service.Encrypt;
using Encryptarium.Core.Service.Mapper;
using Encryptarium.Core.Service.Roles;
using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Encryptarium.BL.Service.Secrets
{
    public interface ISecretService
    {
        Task<SecretDto> GetDecryptedSecretAsync(Guid secretId, int userId);
        Task<SecretDto> GetDecryptedSecretAsync(Guid secretId, SecretTypes secretType, int userId);
        Task<List<SecretDto>> GetSecretsByStorageAsync(Guid storageId, int userId, CancellationToken? cancellationToken);
        Task CreateSecretAsync(SecretModel secret, int userId);
        Task UpdateSecretAsync(Guid secretId, SecretModel secret, int userId);
        Task DeleteSecretAsync(Guid secretId, int userId);
    }

    public class SecretService : ISecretService
    {
        private readonly IBaseRepository<Guid> _baseUidRepository;
        private readonly IBaseRepository<int> _baseIntRepository;
        private readonly IEncryptCoreService _encryptCoreService;
        private readonly IRoleCoreService _roleCoreService;
        public SecretService(IEncryptCoreService encryptCoreService, IBaseRepository<Guid> baseUidRepository, 
            IBaseRepository<int> baseIntRepository, IRoleCoreService roleCoreService)
        {
            _encryptCoreService = encryptCoreService;
            _baseUidRepository = baseUidRepository;
            _baseIntRepository = baseIntRepository;
            _roleCoreService = roleCoreService;
        }

        public async Task CreateSecretAsync(SecretModel secretModel, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForSecretAsync(userId, null, FunctionalRulesSecret.Add, secretModel.StorageId);
            if (access)
            {
                var secret = secretModel.MapToEntity();
                CreateSecretValueAsync(secretModel, secret);

                var privateRole = await _baseIntRepository.GetAll<eRole>()
                    .Where(r => r.UserRoles.Any(ur => ur.UserId == userId) && r.RoleType == RoleType.Private)
                    .FirstOrDefaultAsync();

                secret.SecretRoles.Add(new xSecretRole { RoleId = privateRole.Id });

                await _baseUidRepository.CreateAsync(secret);
            }
        }

        public async Task DeleteSecretAsync(Guid secretId, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForSecretAsync(userId, secretId, FunctionalRulesSecret.Delete);
            if (access)
            {
                await _baseUidRepository.DeleteAsync<eSecret>(secretId);
            }
        }

        public async Task<SecretDto> GetDecryptedSecretAsync(Guid secretId, int userId)
        {
            var secret = await _baseUidRepository.GetAsync<eSecret>(secretId);
            return await GetDecryptedSecretAsync(secretId, secret.SecretType, userId);
        }

        public async Task<SecretDto> GetDecryptedSecretAsync(Guid secretId, SecretTypes secretType, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForSecretAsync(userId, secretId, FunctionalRulesSecret.View);
            if (access)
            {
                return await _encryptCoreService.GetDecryptedSecretAsync(secretId);
            }

            return new SecretDto();
        }

        public async Task<List<SecretDto>> GetSecretsByStorageAsync(Guid storageId, int userId, CancellationToken? cancellationToken)
        {
            var accessForStorage = await _roleCoreService.CheckRoleAccessForStorageAsync(userId, storageId, FunctionalRulesStorage.View);
            if (accessForStorage)
            {
                return await _baseUidRepository.GetAll<eSecret>(s => s.StorageId == storageId)
                    .ToAsyncEnumerable()
                    .Where(async (s, cancellationToken) => await _roleCoreService.CheckRoleAccessForSecretAsync(userId, s.Id, FunctionalRulesSecret.View))
                    .Select(s => s.MapToDto())
                    .ToListAsync();
            }
            return new List<SecretDto> { };
        }

        public async Task UpdateSecretAsync(Guid secretId, SecretModel secretModel, int userId)
        {
            var access = await _roleCoreService.CheckRoleAccessForSecretAsync(userId, secretId, FunctionalRulesSecret.Update);
            if (access)
            {
                var secret = await _baseUidRepository.GetAll<eSecret>()
                    .Include(s => s.SecretText)
                    .Include(s => s.SecretUserPass)
                    .Include(s => s.SecretFile)
                    .FirstOrDefaultAsync(s => s.Id == secretId);

                if (secret == null)
                {
                    return;
                }

                secret.Name = secretModel.Name;
                secret.Description = secretModel.Description;
                secret.StorageId = secretModel.StorageId;
                secret.SecretType = secretModel.SecretType;

                if (secret.SecretText != null)
                    await _baseUidRepository.DeleteAsync<rSecretText>(secret.SecretText.Id);
                if (secret.SecretFile != null)
                    await _baseUidRepository.DeleteAsync<rSecretFile>(secret.SecretFile.Id);
                if (secret.SecretUserPass != null)
                    await _baseUidRepository.DeleteAsync<rSecretUserPass>(secret.SecretUserPass.Id);

                CreateSecretValueAsync(secretModel, secret);
                
                await _baseUidRepository.UpdateAsync(secret);
            }
        }

        private void CreateSecretValueAsync(SecretModel secretModel, eSecret secret)
        {
            switch (secret.SecretType)
            {
                case SecretTypes.Text:
                    if (secretModel.Value is DecryptedSecretValueTextDto dtoText)
                    {
                        secret.SecretText = new rSecretText
                        {
                            Type = dtoText.Type,
                            Value = _encryptCoreService.GetEncryptedSecret(dtoText.DecryptedText, SecretTypes.Text)
                        };
                    }
                    break;
                case SecretTypes.File:
                    if (secretModel.Value is DecryptedSecretValueFileDto dtoFile)
                    {
                        secret.SecretFile = new rSecretFile
                        {
                            Name = dtoFile.Name,
                            Value = _encryptCoreService.GetEncryptedSecret(dtoFile.DecryptedFile, SecretTypes.File)
                        };
                    }
                    break;
                case SecretTypes.UserPass:
                    if (secretModel.Value is DecryptedSecretValueUserPassDto dtoUserPass)
                    {
                        secret.SecretUserPass = new rSecretUserPass
                        {
                            Login = _encryptCoreService.GetEncryptedSecret(dtoUserPass.DecryptedLogin, SecretTypes.File),
                            Password = _encryptCoreService.GetEncryptedSecret(dtoUserPass.DecryptedPassword, SecretTypes.File),
                        };
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
