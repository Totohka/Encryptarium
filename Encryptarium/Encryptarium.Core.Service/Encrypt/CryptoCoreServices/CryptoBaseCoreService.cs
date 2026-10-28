using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Encryptarium.Core.Service.Encrypt.CryptoCoreServices
{
    public interface ICryptoBaseCoreService
    {
        public Task<SecretValueBaseDto> GetDecryptedSecretAsync(Guid secretId);
        public byte[] GetEcnryptedValue(string value, SecretTypes secretType);
    }

    public abstract class CryptoBaseCoreService<T, Y> : ICryptoBaseCoreService
        where T : SecretValueBaseDto
        where Y : BaseSecretType
    {
        protected readonly IBaseRepository<Guid> _baseUidRepository;
        private readonly IMemoryRepository _memoryRepository;

        protected CryptoBaseCoreService(IBaseRepository<Guid> baseUidRepository, IMemoryRepository memoryRepository)
        {
            _baseUidRepository = baseUidRepository;
            _memoryRepository = memoryRepository;
        }

        public virtual byte[] GetEcnryptedValue(string value, SecretTypes secretType)
        {
            using var aes = Aes.Create();
            aes.Key = GetKey();
            aes.IV = GetIV();
            byte[] encryptedData;

            using (var encryptor = aes.CreateEncryptor())
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(value);
                }

                encryptedData = msEncrypt.ToArray();
            }
            return encryptedData;
        }

        public virtual async Task<SecretValueBaseDto> GetDecryptedSecretAsync(Guid secretId)
        {
            var secretType = await GetSecretTypeAsync(secretId);
            return GetDecryptedSecretValueAsync(secretType, GetIV(), GetKey());
        }

        protected virtual T GetDecryptedSecretValueAsync(Y secretType, byte[] ivSecret, byte[] key)
        {
            if (secretType is Y secretConcrette)
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = ivSecret;
                var encryptedValue = GetValue(secretConcrette);
                using var decryptor = aes.CreateDecryptor();
                using var msDecrypt = new MemoryStream(encryptedValue);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                try
                {
                    var value = srDecrypt.ReadToEnd();

                    return MappingDto(secretConcrette, value);
                }
                catch (CryptographicException ex)
                {
                    return new SecretValueBaseDto() as T;
                }
            }
            else
            {
                return new SecretValueBaseDto() as T;
            }
        }

        protected virtual byte[] GetValue(Y secretConcrette)
        {
            throw new NotImplementedException();
        }

        protected abstract T MappingDto(Y secretConcrette, params string[] values);
        protected abstract Task<Y> GetSecretTypeAsync(Guid secretId);

        private byte[] GetIV()
        {
            var ivSecret = _memoryRepository.GetAsync(nameof(MemoryKeys.IVector));
            return MD5.HashData(Encoding.UTF8.GetBytes(ivSecret));
        }

        private byte[] GetKey()
        {
            var key = _memoryRepository.GetAsync(nameof(MemoryKeys.PrivateKeyAES));
            return SHA256.HashData(Encoding.UTF8.GetBytes(key));
        }
    }
}
