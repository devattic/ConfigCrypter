using Microsoft.Extensions.Configuration.Json;
using System;
using System.Security.Cryptography;

namespace DevAttic.ConfigCrypter.ConfigProviders.Json
{
    /// <summary>
    ///  JSON configuration provider that uses the underlying crypter to decrypt the given keys.
    /// </summary>
    public class EncryptedJsonConfigProvider : JsonConfigurationProvider
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
        /// Loads the JSON configuration file and decrypts all configured keys with the given crypter.
        /// </summary>
        public override void Load()
        {
            base.Load();

            using (var crypter = _jsonConfigSource.CrypterFactory(_jsonConfigSource))
            {
                foreach (var key in _jsonConfigSource.KeysToDecrypt)
                {
                    if (Data.TryGetValue(key, out var encryptedValue))
                    {
                        try
                        {
                            Data[key] = crypter.DecryptString(encryptedValue);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Decryption failed for field: {key}", ex);
                        }
                    }
                }
            }
        }
    }
}