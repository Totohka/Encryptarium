using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.Model.Entities;

public class rSecretText : BaseSecretType
{
    public byte[] Value { get; set; }
    public SecretTextTypes Type { get; set; }
}
