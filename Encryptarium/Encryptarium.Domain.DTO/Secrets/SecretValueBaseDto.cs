using System.Text.Json.Serialization;

namespace Encryptarium.Domain.DTO.Secrets;

[JsonDerivedType(typeof(DecryptedSecretValueTextDto), typeDiscriminator: "textD")]
[JsonDerivedType(typeof(DecryptedSecretValueUserPassDto), typeDiscriminator: "userPassD")]
[JsonDerivedType(typeof(DecryptedSecretValueFileDto), typeDiscriminator: "fileD")]
[JsonDerivedType(typeof(EncryptedSecretValueTextDto), typeDiscriminator: "textE")]
[JsonDerivedType(typeof(EncryptedSecretValueUserPassDto), typeDiscriminator: "userPassE")]
[JsonDerivedType(typeof(EncryptedSecretValueFileDto), typeDiscriminator: "fileE")] 
public class SecretValueBaseDto
{
}
