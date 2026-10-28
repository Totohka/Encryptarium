using Encryptarium.BL.Service.Secrets;
using Encryptarium.Domain.DTO.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Encryptarium.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SecretController : BaseController
    {
        private readonly ISecretService _secretService;
        public SecretController(ISecretService secretService) 
        {
            _secretService = secretService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSecret(Guid id) => await BaseRequest(async (int userId) =>
        {
            return Ok(await _secretService.GetDecryptedSecretAsync(id, userId));
        });

        [HttpGet("storage/{id}")]
        public async Task<IActionResult> GetSecretsByStorage(Guid id, CancellationToken cancellationToken) => await BaseRequest(async (int userId) =>
        {
            return Ok(await _secretService.GetSecretsByStorageAsync(id, userId, cancellationToken));
        });

        [HttpPost]
        public async Task<IActionResult> CreateSecret(SecretModel model) => await BaseRequest(async (int userId) =>
        {
            await _secretService.CreateSecretAsync(model, userId);
            return Created();
        });

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSecret(Guid id, SecretModel model) => await BaseRequest(async (int userId) =>
        {
            await _secretService.UpdateSecretAsync(id, model, userId);
            return Ok();
        });

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSecret(Guid id) => await BaseRequest(async (int userId) =>
        {
            await _secretService.DeleteSecretAsync(id, userId);
            return Ok();
        });
    }
}
