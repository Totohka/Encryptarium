namespace Encryptarium.Domain.DTO.Secrets
{
    public class EncryptedSecretValueFileDto : SecretValueBaseDto
    {
        public string Name { get; set; }
        public byte[] DecryptedFile { get; set; }
    }
}
