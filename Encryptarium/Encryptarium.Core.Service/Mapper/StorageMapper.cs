using Encryptarium.Domain.DTO.Storages;
using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Core.Service.Mapper
{
    public static class StorageMapper
    {
        public static eStorage MapToEntity(this StorageDto dto)
        {
            return new eStorage
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                StorageType = dto.StorageType
            };
        }

        public static eStorage MapToEntity(this StorageModel model)
        {
            return new eStorage
            {
                Name = model.Name,
                Description = model.Description,
                StorageType = model.StorageType
            };
        }

        public static StorageModel MapToModel(this eStorage entity)
        {
            return new StorageModel
            {
                Name = entity.Name,
                Description = entity.Description,
                StorageType = entity.StorageType
            };
        }

        public static StorageDto MapToDto(this eStorage entity)
        {
            return new StorageDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                StorageType = entity.StorageType
            };
        }
    }
}
