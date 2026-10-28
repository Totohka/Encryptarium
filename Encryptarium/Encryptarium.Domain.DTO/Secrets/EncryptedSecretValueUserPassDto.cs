namespace Encryptarium.Domain.DTO.Secrets
{
    public class EncryptedSecretValueUserPassDto : SecretValueBaseDto
    {
        public byte[] EncryptedLogin { get; set; }
        public byte[] EncryptedPassword { get; set; }
    }
}
