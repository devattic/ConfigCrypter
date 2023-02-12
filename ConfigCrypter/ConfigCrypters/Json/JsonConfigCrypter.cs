using System;
using System.Text;
using DevAttic.ConfigCrypter.Crypters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevAttic.ConfigCrypter.ConfigCrypters.Json
{
    /// <summary>
    /// Config crypter that encrypts and decrypts keys in JSON config files.
    /// </summary>
    public class JsonConfigCrypter : IConfigCrypter
    {
        private readonly ICrypter _crypter;

        /// <summary>
        /// Creates an instance of the JsonConfigCrypter.
        /// </summary>
        /// <param name="crypter">An ICrypter instance.</param>
        public JsonConfigCrypter(ICrypter crypter)
        {
            _crypter = crypter;
        }

        /// <summary>
        /// Decrypts the key in the given content of a config file.
        /// </summary>
        /// <param name="configFileContent">String content of a config file.</param>
        /// <param name="configKeys">Key of the config entry. The key has to be in JSONPath format.</param>
        /// <param name="separator">Split the key if it has multiple value.</param>
        /// <returns>The content of the config file where the key has been decrypted.</returns>
        public string DecryptKey(string configFileContent, string configKeys, char separator = ';')
        {
            var configKeyList = configKeys.Split(separator);

            var parsedJson = JObject.Parse(configFileContent);

            foreach (var configKey in configKeyList)
            {
                if (string.IsNullOrWhiteSpace(configKey)) continue;

                var settingsToken = ParseConfig(parsedJson, configKey);

                var decryptedValue = _crypter.DecryptString(settingsToken.Value<string>());
                settingsToken.Replace(decryptedValue);
            }

            var newConfigContent = parsedJson.ToString(Formatting.Indented);
            return newConfigContent;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Encrypts the key in the given content of a config file.
        /// </summary>
        /// <param name="configFileContent">String content of a config file.</param>
        /// <param name="configKeys">Key of the config entry. The key has to be in JSONPath format.</param>
        /// <param name="separator">Split the key by semicolon(;) if it has multiple value.</param>
        /// <returns>The content of the config file where the key has been encrypted.</returns>
        public string EncryptKey(string configFileContent, string configKeys, char separator = ';')
        {

            var configKeyList = configKeys.Split(separator);

            var parsedJson = JObject.Parse(configFileContent);

            foreach (var configKey in configKeyList)
            {
                if (string.IsNullOrWhiteSpace(configKey)) continue;

                var settingsToken = ParseConfig(parsedJson, configKey);

                var encryptedValue = _crypter.EncryptString(settingsToken.Value<string>());
                settingsToken.Replace(encryptedValue);
            }

            var newConfigContent = parsedJson.ToString(Formatting.Indented);
            return newConfigContent;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _crypter?.Dispose();
            }
        }

        private JToken ParseConfig(JToken json, string configKey)
        {
            var keyToken = json.SelectToken(configKey);

            if (keyToken == null)
            {
                throw new InvalidOperationException($"The key {configKey} could not be found.");
            }

            return keyToken;
        }
    }
}
