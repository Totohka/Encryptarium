namespace Encryptarium.Domain.DTO.Secrets;

public class DecryptedSecretValueUserPassDto : SecretValueBaseDto
{
    public string DecryptedLogin { get; set; }
    public string DecryptedPassword { get; set; }
}
