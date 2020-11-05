using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DevAttic.ConfigCrypter.CertificateLoaders;
using DevAttic.ConfigCrypter.Crypters;
using Moq;

namespace DevAttic.ConfigCrypter.Tests
{
    public static class Mocks
    {
        public static Mock<ICertificateLoader> CertificateLoader
        {
            get
            {
                var certLoaderMock = new Mock<ICertificateLoader>();
                certLoaderMock.Setup(loader => loader.LoadCertificate()).Returns(() =>
                {
                    using var certStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevAttic.ConfigCrypter.Tests.test-certificate.pfx");
                    using var ms = new MemoryStream();
                    certStream!.CopyTo(ms);

                    return new X509Certificate2(ms.ToArray());
                });

                return certLoaderMock;
            }
        }

        public static Mock<ICrypter> Crypter
        {
            get
            {
                var crypterMock = new Mock<ICrypter>();
                crypterMock.Setup(crypter => crypter.EncryptString(It.IsAny<string>()))
                    .Returns<string>(input => $"{input}_encrypted");
                crypterMock.Setup(crypter => crypter.DecryptString(It.IsAny<string>()))
                    .Returns<string>(input =>
                    {
                        var encryptedIndex = input.LastIndexOf("_encrypted", StringComparison.Ordinal);

                        return encryptedIndex > -1
                            ? input.Substring(0, encryptedIndex)
                            : input.Substring(0, input.Length);
                    });

                return crypterMock;
            }
        }
    }
}
