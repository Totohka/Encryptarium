using System.ComponentModel.DataAnnotations;

namespace Encryptarium.Domain.Model.Entities;

public class BaseEntity<T> where T : struct
{
    [Key]
    public T Id { get; set; }
}
