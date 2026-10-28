using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Storages
{
    public class StorageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StorageTypes StorageType { get; set; }
    }
}
