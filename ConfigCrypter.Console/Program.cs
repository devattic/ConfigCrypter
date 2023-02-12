using CommandLine;
using ConfigCrypter.Console.Options;
using DevAttic.ConfigCrypter;
using DevAttic.ConfigCrypter.CertificateLoaders;
using DevAttic.ConfigCrypter.ConfigCrypters.Json;
using DevAttic.ConfigCrypter.Crypters;

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
                    crypter.EncryptKeyInFile(opts.ConfigFile, opts.Key, opts.Separator);
                })
                .WithParsed<DecryptOptions>(opts =>
                {
                    var crypter = CreateCrypter(opts);
                    crypter.DecryptKeyInFile(opts.ConfigFile, opts.Key, opts.Separator);
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
                certLoader = new StoreCertificateLoader(options.CertSubjectName);
            }

            var configCrypter = new JsonConfigCrypter(new RSACrypter(certLoader));

            var fileCrypter = new ConfigFileCrypter(configCrypter, new ConfigFileCrypterOptions()
            {
                ReplaceCurrentConfig = options.Replace
            });

            return fileCrypter;
        }
    }
}