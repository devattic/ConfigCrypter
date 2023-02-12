using System.IO;
using DevAttic.ConfigCrypter.ConfigCrypters;

namespace DevAttic.ConfigCrypter
{
    /// <summary>
    /// Configuration crypter that reads the configuration file from the filesystem.
    /// </summary>
    public class ConfigFileCrypter
    {
        private readonly IConfigCrypter _configCrypter;
        private readonly ConfigFileCrypterOptions _options;

        /// <summary>
        /// Creates an instance of the ConfigFileCrypter.
        /// </summary>
        /// <param name="configCrypter">A config crypter instance.</param>
        /// <param name="options">Options used for encrypting and decrypting.</param>
        public ConfigFileCrypter(IConfigCrypter configCrypter, ConfigFileCrypterOptions options)
        {
            _configCrypter = configCrypter;
            _options = options;
        }

        /// <summary>
        /// <para>Decrypts the given key in the config file.</para>
        /// <para> </para>
        /// <para>If the "ReplaceCurrentConfig" setting has been set in the options the file is getting replaced.</para>
        /// <para>If the setting has not been set a new file with the "DecryptedConfigPostfix" appended to the current file name will be created.</para>
        /// </summary>
        /// <param name="filePath">Path of the configuration file.</param>
        /// <param name="configKey">Key to decrypt, passed in a format the underlying config crypter understands.</param>
        /// <param name="separator">Split the key if it has multiple value.</param>
        public void DecryptKeyInFile(string filePath, string configKey, char separator = ';')
        {
            var configContent = File.ReadAllText(filePath);
            var decryptedConfigContent = _configCrypter.DecryptKey(configContent, configKey, separator);

            var targetFilePath = GetDestinationConfigPath(filePath, _options.DecryptedConfigPostfix);
            File.WriteAllText(targetFilePath, decryptedConfigContent);
        }

        /// <summary>
        /// <para>Encrypts the given key in the config file.</para>
        /// <para> </para>
        /// <para>If the "ReplaceCurrentConfig" setting has been set in the options the file is getting replaced.</para>
        /// <para>If the setting has not been set a new file with the "EncryptedConfigPostfix" appended to the current file name will be created.</para>
        /// </summary>
        /// <param name="filePath">Path of the configuration file.</param>
        /// <param name="configKey">Key to encrypt, passed in a format the underlying config crypter understands.</param>
        /// <param name="separator">Split the key if it has multiple value.</param>
        public void EncryptKeyInFile(string filePath, string configKey, char separator = ';')
        {
            var configContent = File.ReadAllText(filePath);
            var encryptedConfigContent = _configCrypter.EncryptKey(configContent, configKey, separator);

            var targetFilePath = GetDestinationConfigPath(filePath, _options.EncryptedConfigPostfix);
            File.WriteAllText(targetFilePath, encryptedConfigContent);
        }

        private string GetDestinationConfigPath(string currentConfigFilePath, string postfix)
        {
            if (_options.ReplaceCurrentConfig)
            {
                return currentConfigFilePath;
            }

            var currentConfigDirectory = Path.GetDirectoryName(currentConfigFilePath);
            var newFilename =
                $"{Path.GetFileNameWithoutExtension(currentConfigFilePath)}{postfix}{Path.GetExtension(currentConfigFilePath)}";
            var targetFile = Path.Combine(currentConfigDirectory, newFilename);

            return targetFile;
        }
    }
}