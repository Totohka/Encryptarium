using Encryptarium.Domain.DAL;
using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Encryptarium.Configuration
{
    public static class WebAppExtensions
    {
        public static void SetDefaultUser(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<EncryptariumContext>();
                    db.Database.Migrate();
                    DefaultUser.SeedDefaultUserAsync(services).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                    throw;
                }
            }
        }

        public static void SetMasterKey(this WebApplication app)
        {
            var useFlesh = Environment.GetEnvironmentVariable("AES_UseFlesh");
            if (bool.TryParse(useFlesh, out bool result) && result)
            {
                return;
            }

            var iv = Environment.GetEnvironmentVariable("AES_IV");
            var key = Environment.GetEnvironmentVariable("AES_Key");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var memoryRepository = scope.ServiceProvider.GetRequiredService<IMemoryRepository>();
                    memoryRepository.CreateOrUpdate(new KeyValuePair<string, string>(nameof(MemoryKeys.IVector), iv!));
                    memoryRepository.CreateOrUpdate(new KeyValuePair<string, string>(nameof(MemoryKeys.PrivateKeyAES), key!));
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Have a problem with set a master key");
                    throw;
                }
            }
        }
    }
}
