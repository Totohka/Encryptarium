using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Core.Service.Encrypt.CryptoCoreServices
{
    public class CryptoFileCoreService : CryptoBaseCoreService<DecryptedSecretValueFileDto, rSecretFile>
    {
        public CryptoFileCoreService(IBaseRepository<Guid> baseUidRepository, IMemoryRepository memoryRepository) : base(baseUidRepository, memoryRepository)
        {
        }

        protected override byte[] GetValue(rSecretFile secretConcrette)
        {
            return secretConcrette.Value;
        }

        protected override async Task<rSecretFile> GetSecretTypeAsync(Guid secretId)
        {
            return await _baseUidRepository.GetAsync<rSecretFile>(sf => sf.SecretId == secretId);
        }

        protected override DecryptedSecretValueFileDto MappingDto(rSecretFile secretConcrette, params string[] values)
        {
            return new DecryptedSecretValueFileDto
            {
                DecryptedFile = values[0],
                Name = secretConcrette.Name,
            };
        }
    }
}
