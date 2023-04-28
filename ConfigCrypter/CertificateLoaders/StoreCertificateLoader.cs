using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DevAttic.ConfigCrypter.CertificateLoaders
{
    /// <summary>
    /// Loader that loads a certificate from the Windows certificate store.
    /// </summary>
    public class StoreCertificateLoader : ICertificateLoader
    {
        private readonly string _identifier;
        private readonly X509FindType _findType;

        public StoreCertificateLoader(string identifier, X509FindType findType)
        {
            _identifier = identifier;
            _findType = findType;
        }

        /// <summary>
        /// Loads a certificate by subject name from the store.
        /// </summary>
        /// <returns>A X509Certificate2 instance.</returns>
        /// <remarks>The loader looks for the certificate in the own certificates of the local machine store. It uses the specified find type.</remarks>
        public X509Certificate2 LoadCertificate()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(_findType, _identifier, false);
                var cert = certs.Cast<X509Certificate2>().FirstOrDefault();
                store.Close();

                return cert;
            }
        }
    }
}