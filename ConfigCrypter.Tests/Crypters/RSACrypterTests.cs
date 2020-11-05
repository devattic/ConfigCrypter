using DevAttic.ConfigCrypter.Crypters;
using Xunit;

namespace DevAttic.ConfigCrypter.Tests.Crypters
{
    public class RSACrypterTests
    {
        [Fact]
        public void Constructor_CallsLoadCertificate()
        {
            var certificateLoaderMock = Mocks.CertificateLoader;

            var rsaCrypter = new RSACrypter(certificateLoaderMock.Object);

            certificateLoaderMock.Verify(loader => loader.LoadCertificate());
        }
    }
}
