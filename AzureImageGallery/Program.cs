using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
//using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleImageGallery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureAppConfiguration(( context, config) =>
            {
                if(!context.HostingEnvironment.IsDevelopment())
                { 
                    var builtConfig = config.Build();
                    var vaultName = builtConfig["ConnectionStrings:VaultName"];
                    if( !string.IsNullOrEmpty(vaultName))
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));
                        config.AddAzureKeyVault(vaultName, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                }
                //var keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
                //{
                //    var credential = new DefaultAzureCredential(true);
                //    var token = credential.GetToken(
                //        new Azure.Core.TokenRequestContext(
                //            new[] { "https://vault.azure.net/.default" }));
                //    return token.Token;
                //});

                //config.AddAzureKeyVault(vaultName, keyVaultClient, new DefaultKeyVaultSecretManager());
            });
    }
}
