// See https://aka.ms/new-console-template for more information
using Encryptarium.Client;

Console.WriteLine("Hello, World!");
var client = new EncryptariumClient("http://localhost:50090");
//await client.RegisterAsync("Admin", "12345_TT");
await client.AuthorizationAsync("Admin", "Admin-12345");
//client.
//await client.GetSecretAsync();