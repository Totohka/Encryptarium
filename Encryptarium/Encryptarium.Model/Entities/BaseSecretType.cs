using System.ComponentModel.DataAnnotations.Schema;

namespace Encryptarium.Domain.Model.Entities;

public class BaseSecretType : BaseEntity<Guid>
{
    [ForeignKey("Secret")]
    public Guid SecretId { get; set; }
    public eSecret Secret { get; set; }
}
