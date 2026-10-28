using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Secrets;

public class DecryptedSecretValueTextDto : SecretValueBaseDto
{
    public SecretTextTypes Type { get; set; }
    public string DecryptedText { get; set; }
}
