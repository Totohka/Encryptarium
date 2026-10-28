using Encryptarium.Domain.DTO.Auth;
using Encryptarium.Domain.DTO.Secrets;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Encryptarium.Client
{
    public class EncryptariumClient
    {
        static HttpClient? httpClient;
        private TokenDto? token;

        public EncryptariumClient(string baseAddress)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            };
            httpClient = new HttpClient(socketsHandler);
            httpClient.BaseAddress = new Uri(baseAddress);
        }

        public async Task RegisterAsync(string userName, string password)
        {
            try
            {
                var register = new RegisterModel()
                {
                    UserName = userName,
                    Password = password
                };

                var json = JsonSerializer.Serialize(register);
                var content = new StringContent(json);
                var response = await httpClient.PostAsync("api/v1/Auth/register", content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }
        }

        public async Task AuthorizationAsync(string userName, string password)
        {
            try
            {
                var login = new LoginModel
                {
                    UserName = userName,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login);
                var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));
                var response = await httpClient.PostAsync("api/v1/Auth/login", content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                token = JsonSerializer.Deserialize<TokenDto>(result);
                SetHeaderAuth();
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateSecretAsync()
        {
            var tokenStatus = CheckToken();

            //if (tokenStatus == TokenStatuses.Incorrect || tokenStatus == TokenStatuses.Empty)
            //{
            //    return new Secret { Status = VaultOperationStatuses.Unauthorize };
            //}

            //if (tokenStatus == TokenStatuses.Expired && !(await RefreshToken().ConfigureAwait(false)))
            //{
            //    return new Secret { Status = VaultOperationStatuses.Unauthorize };
            //}


        }

        public async Task<Secret> GetSecretAsync(Guid uid)
        {
            try
            {
                var tokenStatus = CheckToken();

                if (tokenStatus == TokenStatuses.Incorrect || tokenStatus == TokenStatuses.Empty)
                {
                    return new Secret { Status = VaultOperationStatuses.Unauthorize };
                }

                if (tokenStatus == TokenStatuses.Expired && !(await RefreshToken().ConfigureAwait(false)))
                {
                    return new Secret { Status = VaultOperationStatuses.Unauthorize };
                }

                var response = await httpClient.GetAsync($"api/v1/Secret/{uid}").ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var secretDto = JsonSerializer.Deserialize<SecretDto>(result);

                return new Secret
                {
                    SecretDto = secretDto,
                    Status = VaultOperationStatuses.Success
                };
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue && (int)ex.StatusCode.Value >= 500)
                {
                    return new Secret
                    {
                        Status = VaultOperationStatuses.ErrorVault
                    };
                }
                return new Secret
                {
                    Status = VaultOperationStatuses.Error
                };
            }
            catch
            {
                throw;
            }
        }

        private void SetHeaderAuth()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        private async Task<bool> RefreshToken()
        {
            try
            {
                var json = JsonSerializer.Serialize(token);
                var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));
                var response = await httpClient.PostAsync("api/v1/Auth/refresh", content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                token = JsonSerializer.Deserialize<TokenDto>(result);
                SetHeaderAuth();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private TokenStatuses CheckToken()
        {
            if (token == null)
            {
                return TokenStatuses.Empty;
            }

            if (string.IsNullOrWhiteSpace(token.AccessToken) || string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                return TokenStatuses.Incorrect;
            }

            if (token.ExpiresIn < DateTime.UtcNow)
            {
                return TokenStatuses.Expired;
            }

            return TokenStatuses.Ok;    
        }
    }
}
