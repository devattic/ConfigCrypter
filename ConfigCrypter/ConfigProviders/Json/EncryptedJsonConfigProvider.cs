using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Resources;
using System.Text.Json;

namespace DevAttic.ConfigCrypter.ConfigProviders.Json
{
    /// <summary>
    ///  JSON configuration provider that uses the underlying crypter to decrypt the given keys.
    /// </summary>
    public partial class EncryptedJsonConfigProvider : JsonConfigurationProvider
    {
        private readonly EncryptedJsonConfigSource _jsonConfigSource;

        /// <summary>
        /// Creates an instance of the EncryptedJsonConfigProvider.
        /// </summary>
        /// <param name="source">EncryptedJsonConfigSource that is used to configure the provider.</param>
        public EncryptedJsonConfigProvider(EncryptedJsonConfigSource source) : base(source)
        {
            _jsonConfigSource = source;
        }

        /// <summary>
        /// Loads the JSON configuration from stream and decrypts all configured keys with the given crypter.
        /// </summary>
        public override void Load(Stream stream)
        {
            try
            {
                using (var crypter = _jsonConfigSource.CrypterFactory(_jsonConfigSource))
                {
                    Data = EncryptedJsonConfigurationFileParser.Parse(stream, crypter, _jsonConfigSource.KeysToDecrypt);
                }
            }
            catch (JsonException e)
            {
                throw new FormatException($"Error JSONParseError {e.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to decrypt keys", ex);
            }
        }
    }
}