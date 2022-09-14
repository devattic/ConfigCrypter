using System.Collections.Generic;
using DevAttic.ConfigCrypter.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                .ConfigureAppConfiguration((hostingContext, cfg) =>
                {
                    cfg.AddEncryptedAppSettings(hostingContext.HostingEnvironment, crypter =>
                    {
                        crypter.ReloadOnChange = true;
                        crypter.CertificatePath = "cert.pfx";
                        crypter.KeysToDecrypt = new List<string> { "Nested:KeyToEncrypt" };
                    });
                });
    }
}
