using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Secrets;

public class EncryptedSecretValueTextDto : SecretValueBaseDto
{
    public SecretTextTypes Type { get; set; }
    public byte[] EncryptedText { get; set; }
}
