namespace Encryptarium.Domain.DTO.Secrets;

public class DecryptedSecretValueFileDto : SecretValueBaseDto
{
    public string Name { get; set; }
    public string DecryptedFile { get; set; }
}
