using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Core.Service.Encrypt.CryptoCoreServices
{
    public class CryptoTextCoreService : CryptoBaseCoreService<DecryptedSecretValueTextDto, rSecretText>
    {
        public CryptoTextCoreService(IBaseRepository<Guid> baseUidRepository, IMemoryRepository memoryRepository)
            : base(baseUidRepository, memoryRepository)
        {
        }

        protected override async Task<rSecretText> GetSecretTypeAsync(Guid secretId)
        {
            return await _baseUidRepository.GetAsync<rSecretText>(st => st.SecretId == secretId);
        }

        protected override DecryptedSecretValueTextDto MappingDto(rSecretText secretConcrette, params string[] values)
        {
            return new DecryptedSecretValueTextDto
            {
                DecryptedText = values[0],
                Type = secretConcrette.Type
            };
        }

        protected override byte[] GetValue(rSecretText secretConcrette)
        {
            return secretConcrette.Value;
        }
    }
}
