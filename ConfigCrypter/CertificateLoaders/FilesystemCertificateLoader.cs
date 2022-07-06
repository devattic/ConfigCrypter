using System.Security.Cryptography.X509Certificates;

namespace DevAttic.ConfigCrypter.CertificateLoaders
{
    /// <summary>
    /// Loader that loads a certificate from the filesystem.
    /// </summary>
    public class FilesystemCertificateLoader : ICertificateLoader
    {
        private readonly string _certificatePath;
        private readonly string _certificatePassword;

        /// <summary>
        /// Creates an instance of the certificate loader.
        /// </summary>
        /// <param name="certificatePath">Fully qualified path to the certificate (.pfx file).</param>
        /// <param name="certificatePassword">Password of the certificate, if available.</param>
        public FilesystemCertificateLoader(string certificatePath, string certificatePassword = null)
        {
            _certificatePath = certificatePath;
            _certificatePassword = certificatePassword;
        }

        /// <summary>
        /// Loads a certificate from the given location on the filesystem.
        /// </summary>
        /// <returns>A X509Certificate2 instance.</returns>
        public X509Certificate2 LoadCertificate()
        {
            return string.IsNullOrEmpty(_certificatePassword) ?
                new X509Certificate2(_certificatePath) :
                new X509Certificate2(_certificatePath, _certificatePassword);
        }
    }
}