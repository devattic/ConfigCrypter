namespace DevAttic.ConfigCrypter
{
    /// <summary>
    /// Options to configure the ConfigFileCrypter.
    /// </summary>
    public class ConfigFileCrypterOptions
    {
        /// <summary>
        /// Name of the postfix that should be appended when a file has been decrypted and "ReplaceCurrentConfig" is set to true.
        /// </summary>
        public string DecryptedConfigPostfix { get; set; } = "_decrypted";
        /// <summary>
        /// Name of the postfix that should be appended when a file has been encrypted and "ReplaceCurrentConfig" is set to true.
        /// </summary>
        public string EncryptedConfigPostfix { get; set; } = "_encrypted";
        /// <summary>
        /// Defines if the original config file should be overriden or a new file should be created.
        /// </summary>
        public bool ReplaceCurrentConfig { get; set; }
    }
}