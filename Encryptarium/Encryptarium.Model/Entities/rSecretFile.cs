namespace Encryptarium.Domain.Model.Entities;

public class rSecretFile : BaseSecretType
{
    public string Name { get; set; }
    public byte[] Value { get; set; }
}
