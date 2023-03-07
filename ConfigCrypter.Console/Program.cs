using CommandLine;
using ConfigCrypter.Console.Options;
using DevAttic.ConfigCrypter;
using DevAttic.ConfigCrypter.CertificateLoaders;
using DevAttic.ConfigCrypter.ConfigCrypters.Json;
using DevAttic.ConfigCrypter.Crypters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ConfigCrypter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args)
                .WithParsed<EncryptOptions>(opts =>
                {
                    var crypter = CreateCrypter(opts);
                    crypter.EncryptKeyInFile(opts.ConfigFile, opts.Key);
                })
                .WithParsed<DecryptOptions>(opts =>
                {
                    var crypter = CreateCrypter(opts);
                    crypter.DecryptKeyInFile(opts.ConfigFile, opts.Key);
                });
        }

        private static ConfigFileCrypter CreateCrypter(CommandlineOptions options)
        {
            ICertificateLoader certLoader = null;

            if (!string.IsNullOrEmpty(options.CertificatePath))
            {
                certLoader = new FilesystemCertificateLoader(options.CertificatePath, options.CertificatePassword);
            }
            else if (!string.IsNullOrEmpty(options.CertSubjectName))
            {
                certLoader = new StoreCertificateLoader(options.CertSubjectName, X509FindType.FindBySubjectName);

            }
            else if (!string.IsNullOrEmpty(options.CertThumbprintField))
            {
                var certThumbprint = LoadThumbPrintFromFile(options);
                certLoader = new StoreCertificateLoader(certThumbprint, X509FindType.FindByThumbprint);
            }
            else if(!string.IsNullOrEmpty(options.CertThumbprint))
            {
                certLoader = new StoreCertificateLoader(options.CertThumbprint, X509FindType.FindByThumbprint);
            }

            var configCrypter = new JsonConfigCrypter(new RSACrypter(certLoader));

            var fileCrypter = new ConfigFileCrypter(configCrypter, new ConfigFileCrypterOptions()
            {
                ReplaceCurrentConfig = options.Replace
            });

            return fileCrypter;
        }

        private static string LoadThumbPrintFromFile(CommandlineOptions options)
        {
            var configContent = File.ReadAllText(options.ConfigFile);
            return ParseConfig(configContent, options.CertThumbprintField);
        }

        private static string ParseConfig(string json, string configKey)
        {
            var parsedJson = JObject.Parse(json);
            var keyToken = parsedJson.SelectToken(configKey);

            if (keyToken == null || keyToken.Type != JTokenType.String)
            {
                throw new InvalidOperationException($"The key {configKey} could not be found, or is empty");
            }
            var value = keyToken.Value<string>();

            if (String.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"The key {configKey} could not be found, or is empty");
            }
            return value;
        }
    }
}