using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevAttic.ConfigCrypter.CertificateLoaders;

namespace DevAttic.ConfigCrypter.Crypters
{
    /// <summary>
    /// RSA based crypter that uses the public and private key of a certificate to encrypt and decrypt strings.
    /// </summary>
    public class RSACrypter : ICrypter
    {
        private readonly ICertificateLoader _certificateLoader;
        private RSA _privateKey;
        private RSA _publicKey;

        /// <summary>
        ///  Creates an instance of the RSACrypter.
        /// </summary>
        /// <param name="certificateLoader">A certificate loader instance.</param>
        public RSACrypter(ICertificateLoader certificateLoader)
        {
            _certificateLoader = certificateLoader;
            InitKeys();
        }

        /// <summary>
        /// Encrypts the given string with the private key of the loaded certificate.
        /// </summary>
        /// <param name="value">String to decrypt.</param>
        /// <returns>Encrypted string.</returns>
        public string DecryptString(string value)
        {
            var decodedBase64 = Convert.FromBase64String(value);
            var decryptedValue = _privateKey.Decrypt(decodedBase64, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decryptedValue);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Decrypts the given string with the public key of the loaded certificate.
        /// </summary>
        /// <param name="value">String to encrypt.</param>
        /// <returns>Encrypted string.</returns>
        public string EncryptString(string value)
        {
            var encryptedValue = _publicKey.Encrypt(Encoding.UTF8.GetBytes(value), RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encryptedValue);
        }

        /// <summary>
        /// Disposes the underlying keys.
        /// </summary>
        /// <param name="disposing">True if called from user code, false if called by finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _privateKey?.Dispose();
                _publicKey?.Dispose();
            }
        }

        private void InitKeys()
        {
            using (var certificate = _certificateLoader.LoadCertificate())
            {
                _privateKey = certificate.GetRSAPrivateKey();
                _publicKey = certificate.GetRSAPublicKey();
            }
        }
    }
}