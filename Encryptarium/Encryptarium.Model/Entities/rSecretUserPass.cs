namespace Encryptarium.Domain.Model.Entities;

public class rSecretUserPass : BaseSecretType
{
    public byte[] Login { get; set; }
    public byte[] Password { get; set; }
}
