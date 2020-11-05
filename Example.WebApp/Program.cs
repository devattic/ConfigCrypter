using System.Collections.Generic;
using DevAttic.ConfigCrypter.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Example.WebApp
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
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.AddEncryptedAppSettings(crypter =>
                    {
                        crypter.CertificatePath = "test-certificate.pfx";
                        crypter.KeysToDecrypt = new List<string> { "Nested:KeyToEncrypt" };
                    });
                });
    }
}
