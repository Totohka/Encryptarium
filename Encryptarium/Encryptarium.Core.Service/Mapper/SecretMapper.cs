using Encryptarium.Domain.DTO.Secrets;
using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Core.Service.Mapper
{
    public static class SecretMapper
    {
        public static eSecret MapToEntity(this SecretDto dto)
        {
            return new eSecret
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                SecretType = dto.SecretType
            };
        }

        public static eSecret MapToEntity(this SecretModel model)
        {
            return new eSecret
            {
                Name = model.Name,
                Description = model.Description,
                StorageId = model.StorageId,
                SecretType = model.SecretType
            };
        }

        public static SecretModel MapToModel(this eSecret entity)
        {
            return new SecretModel
            {
                Name = entity.Name,
                Description = entity.Description,
                SecretType = entity.SecretType,
                StorageId = entity.StorageId
            };
        }

        public static SecretDto MapToDto(this eSecret entity)
        {
            return new SecretDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                SecretType = entity.SecretType
            };
        }
    }
}
