using Encryptarium.BL.Service.Secrets;
using Encryptarium.Domain.DTO.Storages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Encryptarium.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StorageController : BaseController
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService) 
        {
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => await BaseRequest(async (int userId) =>
        {
            return Ok(_storageService.GetStoragesAsync(userId));
        });

        [HttpPost]
        public async Task<IActionResult> CreateStorage(StorageModel model) => await BaseRequest(async (int userId) =>
        {
            await _storageService.CreateStorageAsync(model, userId);
            return Created();
        });

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStorage(Guid id, StorageModel model) => await BaseRequest(async (int userId) =>
        {
            await _storageService.UpdateStorageAsync(id, model, userId);    
            return Ok();
        });

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStorage(Guid id) => await BaseRequest(async (int userId) =>
        {
            await _storageService.DeleteStorageAsync(id, userId);
            return Ok();
        });
    }
}
