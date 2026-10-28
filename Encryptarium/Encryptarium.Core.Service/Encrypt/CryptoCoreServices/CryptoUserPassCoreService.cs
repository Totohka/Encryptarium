using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Model.Entities;
using System.Security.Cryptography;

namespace Encryptarium.Core.Service.Encrypt.CryptoCoreServices
{
    public class CryptoUserPassCoreService : CryptoBaseCoreService<DecryptedSecretValueUserPassDto, rSecretUserPass>
    {
        public CryptoUserPassCoreService(IBaseRepository<Guid> baseUidRepository, IMemoryRepository memoryRepository) : base(baseUidRepository, memoryRepository)
        {
        }

        protected override async Task<rSecretUserPass> GetSecretTypeAsync(Guid secretId)
        {
            return await _baseUidRepository.GetAsync<rSecretUserPass>(sup => sup.SecretId == secretId);
        }

        protected override DecryptedSecretValueUserPassDto GetDecryptedSecretValueAsync(rSecretUserPass secretType, byte[] ivSecret, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = ivSecret;

            var encryptedLogin = secretType.Login;
            var encryptedPassword = secretType.Password;

            using var decryptor = aes.CreateDecryptor();
            try
            {
                var msDecrypt = new MemoryStream(encryptedLogin);
                var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                var srDecrypt = new StreamReader(csDecrypt);
                var login = srDecrypt.ReadToEnd();
                msDecrypt.Close();
                csDecrypt.Close();
                srDecrypt.Close();

                msDecrypt = new MemoryStream(encryptedLogin);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);
                var password = srDecrypt.ReadToEnd();
                msDecrypt.Close();
                csDecrypt.Close();
                srDecrypt.Close();

                return MappingDto(secretType, login, password);
            }
            catch (CryptographicException ex)
            {
                return new SecretValueBaseDto() as DecryptedSecretValueUserPassDto;
            }
        }

        protected override DecryptedSecretValueUserPassDto MappingDto(rSecretUserPass secretConcrette, params string[] values)
        {
            return new DecryptedSecretValueUserPassDto
            {
                DecryptedLogin = values[0],
                DecryptedPassword = values[1],
            };
        }
    }
}
