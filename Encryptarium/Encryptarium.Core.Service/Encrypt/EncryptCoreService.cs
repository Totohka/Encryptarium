using Encryptarium.Core.Service.Encrypt.CryptoCoreServices;
using Encryptarium.Core.Service.Mapper;
using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Core.Service.Encrypt
{
    public interface IEncryptCoreService {
        public byte[] GetEncryptedSecret(string value, SecretTypes secretType);
        public Task<SecretDto> GetDecryptedSecretAsync(Guid secretId);
    }

    public class EncryptCoreService : IEncryptCoreService
    {
        private readonly IBaseRepository<Guid> _baseUidRepository;
        private readonly ICryptoBaseCoreService[] _cryptoBaseCoreServices;
        public EncryptCoreService(IBaseRepository<Guid> baseUidRepository, 
            IEnumerable<ICryptoBaseCoreService> cryptoBaseCoreServices)
        {
            _baseUidRepository = baseUidRepository;
            _cryptoBaseCoreServices = cryptoBaseCoreServices.ToArray();
        }

        public byte[] GetEncryptedSecret(string value, SecretTypes secretType)
        {
            return _cryptoBaseCoreServices[(int)secretType].GetEcnryptedValue(value, secretType);
        }

        public async Task<SecretDto> GetDecryptedSecretAsync(Guid secretId)
        {
            var secret = await _baseUidRepository.GetAsync<eSecret>(secretId);
            var secretDto = secret.MapToDto();
            secretDto.Value = await _cryptoBaseCoreServices[(int)secret.SecretType].GetDecryptedSecretAsync(secretId);
            return secretDto;
        }
    }
}
