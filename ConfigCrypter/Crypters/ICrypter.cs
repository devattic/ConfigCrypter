using System;

namespace DevAttic.ConfigCrypter.Crypters
{
    /// <summary>
    /// A crypter that is used to encrypt and decrypt simple strings.
    /// </summary>
    public interface ICrypter : IDisposable
    {
        /// <summary>
        /// Decrypts the given string.
        /// </summary>
        /// <param name="value">String to decrypt.</param>
        /// <returns>Encrypted string.</returns>
        string DecryptString(string value);

        /// <summary>
        /// Encrypts the given string.
        /// </summary>
        /// <param name="value">String to encrypt.</param>
        /// <returns>Encrypted string.</returns>
        string EncryptString(string value);
    }
}