using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Storages;

public class StorageModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public StorageTypes StorageType { get; set; }
}
