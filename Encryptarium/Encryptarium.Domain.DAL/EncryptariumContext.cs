using Encryptarium.Domain.Enum;
using Encryptarium.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Data;
using System.Net.Sockets;

namespace Encryptarium.Domain.DAL
{
    public class EncryptariumContext : IdentityDbContext<eUser, IdentityRole<int>, int>
    {
        public EncryptariumContext(DbContextOptions<EncryptariumContext> dbContextOptions) : base(dbContextOptions) 
        {
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        public DbSet<cFunctionalRuleSecret> cFunctionalRuleSecrets { get; set; }
        public DbSet<cFunctionalRuleStorage> cFunctionalRuleStorages { get; set; }
        public DbSet<eRole> eRoles { get; set; }
        public DbSet<eSecret> eSecrets { get; set; }
        public DbSet<eStorage> eStorages { get; set; }
        public DbSet<rSecretFile> rSecretFiles { get; set; }
        public DbSet<rSecretText> rSecretTexts { get; set; }
        public DbSet<rSecretUserPass> rSecretUserPass { get; set; }
        public DbSet<xSecretRole> xSecretRoles { get; set; }
        public DbSet<xStorageRole> xStorageRoles { get; set; }
        public DbSet<xUserRole> xUserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<xSecretRole>(xSecretRoleConfigure);
            modelBuilder.Entity<xStorageRole>(xStorageRoleConfigure);
            modelBuilder.Entity<xUserRole>(xUserRoleConfigure);
            modelBuilder.Entity<cFunctionalRuleSecret>(cFunctionalRuleSecretConfigure);
            modelBuilder.Entity<cFunctionalRuleStorage>(cFunctionalRuleStorageConfigure);
            modelBuilder.Entity<IdentityRole<int>>(IdentityRoleConfigure);
        }
        public void xSecretRoleConfigure(EntityTypeBuilder<xSecretRole> builder)
        {
            builder.HasKey(p => new { p.SecretId, p.RoleId });
        }
        public void xStorageRoleConfigure(EntityTypeBuilder<xStorageRole> builder)
        {
            builder.HasKey(p => new { p.StorageId, p.RoleId });
        }
        public void xUserRoleConfigure(EntityTypeBuilder<xUserRole> builder)
        {
            builder.HasKey(p => new { p.UserId, p.RoleId });
        }
        public void cFunctionalRuleSecretConfigure(EntityTypeBuilder<cFunctionalRuleSecret> builder)
        {
            builder.HasData(
                [
                    new cFunctionalRuleSecret{
                        Id = (int)FunctionalRulesSecret.View,
                        Name = nameof(FunctionalRulesSecret.View),
                        Description = $"Функциональное правило {FunctionalRulesSecret.View}",
                    },
                    new cFunctionalRuleSecret{
                        Id = (int)FunctionalRulesSecret.Update,
                        Name = nameof(FunctionalRulesSecret.Update),
                        Description = $"Функциональное правило {FunctionalRulesSecret.Update}",
                    },
                    new cFunctionalRuleSecret{
                        Id = (int)FunctionalRulesSecret.Add,
                        Name = nameof(FunctionalRulesSecret.Add),
                        Description = $"Функциональное правило {FunctionalRulesSecret.Add}",
                    },
                    new cFunctionalRuleSecret{
                        Id = (int)FunctionalRulesSecret.Delete,
                        Name = nameof(FunctionalRulesSecret.Delete),
                        Description = $"Функциональное правило {FunctionalRulesSecret.Delete}",
                    },
                ]
            );
        }
        public void cFunctionalRuleStorageConfigure(EntityTypeBuilder<cFunctionalRuleStorage> builder)
        {
            builder.HasData(
                [
                    new cFunctionalRuleStorage{
                        Id = (int)FunctionalRulesStorage.View,
                        Name = nameof(FunctionalRulesStorage.View),
                        Description = $"Функциональное правило {FunctionalRulesStorage.View}",
                    },
                    new cFunctionalRuleStorage{
                        Id = (int)FunctionalRulesStorage.Update,
                        Name = nameof(FunctionalRulesStorage.Update),
                        Description = $"Функциональное правило {FunctionalRulesStorage.Update}",
                    },
                    new cFunctionalRuleStorage{
                        Id = (int)FunctionalRulesStorage.Add,
                        Name = nameof(FunctionalRulesStorage.Add),
                        Description = $"Функциональное правило {FunctionalRulesStorage.Add}",
                    },
                    new cFunctionalRuleStorage{
                        Id = (int)FunctionalRulesStorage.Delete,
                        Name = nameof(FunctionalRulesStorage.Delete),
                        Description = $"Функциональное правило {FunctionalRulesStorage.Delete}",
                    },
                ]
            );
        }
        public void IdentityRoleConfigure(EntityTypeBuilder<IdentityRole<int>> builder)
        {
            builder.HasData(
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                }
            );
        }
    }
}
