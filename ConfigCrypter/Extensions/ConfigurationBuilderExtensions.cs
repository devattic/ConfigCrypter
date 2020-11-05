using System;
using DevAttic.ConfigCrypter.CertificateLoaders;
using DevAttic.ConfigCrypter.ConfigProviders.Json;
using Microsoft.Extensions.Configuration;

namespace DevAttic.ConfigCrypter.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds a provider to decrypt keys in the appsettings.json file.
        /// </summary>
        /// <param name="builder">A ConfigurationBuilder instance.</param>
        /// <param name="configAction">An action used to configure the configuration source.</param>
        /// <returns>The current ConfigurationBuilder instance.</returns>
        public static IConfigurationBuilder AddEncryptedAppSettings(
            this IConfigurationBuilder builder, Action<EncryptedJsonConfigSource> configAction)

        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var configSource = new EncryptedJsonConfigSource { Path = "appsettings.json" };
            configAction(configSource);

            InitializeCertificateLoader(configSource);

            builder.Add(configSource);
            return builder;
        }

        /// <summary>
        /// Adds a provider to decrypt keys in the given json config file.
        /// </summary>
        /// <param name="builder">A ConfigurationBuilder instance.</param>
        /// <param name="configAction">An action used to configure the configuration source.</param>
        /// <returns>The current ConfigurationBuilder instance.</returns>
        public static IConfigurationBuilder AddEncryptedJsonConfig(
                    this IConfigurationBuilder builder, Action<EncryptedJsonConfigSource> configAction)

        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var configSource = new EncryptedJsonConfigSource();
            configAction(configSource);

            InitializeCertificateLoader(configSource);

            builder.Add(configSource);
            return builder;
        }

        private static void InitializeCertificateLoader(EncryptedJsonConfigSource config)
        {
            if (!string.IsNullOrEmpty(config.CertificatePath))
            {
                config.CertificateLoader = new FilesystemCertificateLoader(config.CertificatePath);
            }
            else if (!string.IsNullOrEmpty(config.CertificateSubjectName))
            {
                config.CertificateLoader = new StoreCertificateLoader(config.CertificateSubjectName);
            }

            if (config.CertificateLoader == null)
            {
                throw new InvalidOperationException(
                    "Either CertificatePath or CertificateSubjectName has to be provided if CertificateLoader has not been set manually.");
            }

            if (string.IsNullOrEmpty(config.Path))
            {
                throw new InvalidOperationException(
                    "The \"Path\" property has to be set to the path of a config file.");
            }
        }
    }
}