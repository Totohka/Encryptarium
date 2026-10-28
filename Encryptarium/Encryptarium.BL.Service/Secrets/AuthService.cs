using Encryptarium.Core.Service.Roles;
using Encryptarium.Domain.DTO.Auth;
using Encryptarium.Domain.DTO.Storages;
using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Encryptarium.BL.Service.Secrets
{
    public interface IAuthService
    {
        Task<IdentityResult> CreateUserAsync(eUser user, string password);
        Task<bool> IsValidUserAsync(LoginModel model);
        Task<TokenDto> GenerateTokenAsync(string userName);
        Task<TokenDto> RefreshTokenAsync(TokenDto token);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<eUser> _userManager;
        private readonly IStorageService _storageService;
        private readonly IRoleCoreService _roleCoreService;
        public AuthService(UserManager<eUser> userManager, IConfiguration configuration, IStorageService storageService,
            IRoleCoreService roleCoreService) 
        {
            _userManager = userManager;
            _configuration = configuration;
            _storageService = storageService;
            _roleCoreService = roleCoreService;
        }

        public async Task<bool> IsValidUserAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            return (user != null) && (await _userManager.CheckPasswordAsync(user, model.Password));
        }

        public async Task<TokenDto> GenerateTokenAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = GetUserClaims(user);
            var options = GetTokenOptions(claims);
            var token = new JwtSecurityTokenHandler().WriteToken(options);

            DateTime exp = options.Payload.ValidTo;
            var refreshToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenDto()
            {
                AccessToken = token,
                ExpiresIn = exp,
                RefreshToken = refreshToken
            };
        }

        public async Task<IdentityResult> CreateUserAsync(eUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var roleId = await _roleCoreService.CreateNewRoleAsync(
                    new eRole
                    {
                        Name = $"Личная роль {user.UserName}",
                        Description = "Роль используется только для личного контейнера",
                        RoleType = RoleType.Private
                    },
                    [FunctionalRulesSecret.View, FunctionalRulesSecret.Update, FunctionalRulesSecret.Add, FunctionalRulesSecret.Delete],
                    [FunctionalRulesStorage.View, FunctionalRulesStorage.Update, FunctionalRulesStorage.Add, FunctionalRulesStorage.Delete]
                );

                await _roleCoreService.SetRoleForUserAsync(roleId, user.Id);

                await _storageService.CreateStorageAsync(new StorageModel
                {
                    Name = "Личный контейнер",
                    Description = "Контейнер виден только вам.",
                    StorageType = StorageTypes.Private
                }, user.Id);
            }
            return result;
        }

        public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
        {
            var principal = GetClaimsPrincipal(token.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != token.RefreshToken)
                throw new SecurityTokenException("Ошибка обновления токена");

            return await GenerateTokenAsync(user.UserName);
        }

        private List<Claim> GetUserClaims(eUser user)
        {
            return [
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            ];
        }
        private JwtSecurityToken GetTokenOptions(List<Claim> claims)
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var secret = new SymmetricSecurityKey(key);
            SigningCredentials signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var tokenSettings = _configuration.GetSection("tokenSettings");
            var tokenOptions = new JwtSecurityToken(
                            signingCredentials: signingCredentials,
                            issuer: tokenSettings["Issuer"],
                            audience: tokenSettings["Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(20));
            return tokenOptions;
        }

        private ClaimsPrincipal GetClaimsPrincipal(string expiredToken)
        {
            var tokenSettings = _configuration.GetSection("tokenSettings");
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
                ValidIssuer = tokenSettings["Issuer"],
                ValidAudience = tokenSettings["Audience"],
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiredToken,
                               tokenValidationParameters,
                               out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
