using Encryptarium;
using Encryptarium.BL.Service.Secrets;
using Encryptarium.Configuration;
using Encryptarium.Core.Service.Encrypt;
using Encryptarium.Core.Service.Encrypt.CryptoCoreServices;
using Encryptarium.Core.Service.Roles;
using Encryptarium.Domain.DAL;
using Encryptarium.Domain.DAL.Repositories;
using Encryptarium.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var tokenSettings = configuration.GetSection("tokenSettings");
var secretKey = Environment.GetEnvironmentVariable("SECRET");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<EncryptariumContext>(builder => builder.UseSqlite(configuration.GetConnectionString("MainDb")));
builder.Services.AddAuthentication()
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenSettings["Issuer"],
            ValidAudience = tokenSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddIdentity<eUser, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<EncryptariumContext>()
  .AddDefaultTokenProviders();

builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT token in the format 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT" 
    };
    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
    };
    options.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddScoped<IBaseRepository<int>, BaseRepository<int>>();
builder.Services.AddScoped<IBaseRepository<Guid>, BaseRepository<Guid>>();
builder.Services.AddScoped<IXBaseRepository, XBaseRepository>();
builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();

builder.Services.AddScoped<IRoleCoreService, RoleCoreService>();
builder.Services.AddTransient<IEncryptCoreService, EncryptCoreService>();

builder.Services.AddTransient<ICryptoBaseCoreService, CryptoTextCoreService>();
builder.Services.AddTransient<ICryptoBaseCoreService, CryptoFileCoreService>();
builder.Services.AddTransient<ICryptoBaseCoreService, CryptoUserPassCoreService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecretService, SecretService>();
builder.Services.AddScoped<IStorageService, StorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/swagger/v1/swagger.json");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.SetDefaultUser();
app.SetMasterKey();

app.Run();
