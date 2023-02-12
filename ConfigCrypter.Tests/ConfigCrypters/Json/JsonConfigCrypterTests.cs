using DevAttic.ConfigCrypter.ConfigCrypters.Json;
using Newtonsoft.Json;
using Xunit;

namespace DevAttic.ConfigCrypter.Tests.ConfigCrypters.Json
{
    public class JsonConfigCrypterTests
    {
        [Fact]
        public void EncryptKey_WithValidJson_CallsEncryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt" });

            var encryptedJson = jsonCrypter.EncryptKey(json, "Key");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            crypterMock.Verify(crypter => crypter.EncryptString("ValueToEncrypt"));

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt_encrypted", parsedJson.Key);
        }

        [Fact]
        public void EncryptKeys_WithValidJson_CallsEncryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt", AdditionalKey = "AdditionalKeyToEncrypt" });

            var encryptedJson = jsonCrypter.EncryptKey(json, "Key;AdditionalKey");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt_encrypted", parsedJson.Key);
            Assert.Equal("AdditionalKeyToEncrypt_encrypted", parsedJson.AdditionalKey);
        }

        [Theory]
        [InlineData('|')]
        [InlineData(',')]
        public void EncryptKeys_WithDifferentSeparator_WithValidJson_CallsEncryptStringOnCrypter(char separator)
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt", AdditionalKey = "AdditionalKeyToEncrypt" });

            var encryptedJson = jsonCrypter.EncryptKey(json, $"Key{separator}AdditionalKey", separator);
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt_encrypted", parsedJson.Key);
            Assert.Equal("AdditionalKeyToEncrypt_encrypted", parsedJson.AdditionalKey);
        }

        [Fact]
        public void EncryptKeys_WithEmptyFieldAndWithValidJson_CallsEncryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt" });

            var encryptedJson = jsonCrypter.EncryptKey(json, "Key;;");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt_encrypted", parsedJson.Key);
        }

        [Fact]
        public void DecryptKey_WithValidJson_CallsDecryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new { Key = "ValueToEncrypt_encrypted" });

            var decryptedJson = jsonCrypter.DecryptKey(json, "Key");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(decryptedJson);

            crypterMock.Verify(crypter => crypter.DecryptString("ValueToEncrypt_encrypted"));
            Assert.Equal("ValueToEncrypt", parsedJson.Key);
        }

        [Fact]
        public void DecryptKeys_WithValidJson_CallsEncryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt_encrypted", AdditionalKey = "AdditionalKeyToEncrypt_encrypted" });

            var encryptedJson = jsonCrypter.DecryptKey(json, "Key;AdditionalKey");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt", parsedJson.Key);
            Assert.Equal("AdditionalKeyToEncrypt", parsedJson.AdditionalKey);
        }

        [Theory]
        [InlineData('|')]
        [InlineData(',')]
        public void DecryptKey_WithDifferentSeparator_WithValidJson_CallsEncryptStringOnCrypter(char separator)
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt_encrypted", AdditionalKey = "AdditionalKeyToEncrypt_encrypted" });

            var encryptedJson = jsonCrypter.DecryptKey(json, $"Key{separator}AdditionalKey", separator);
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt", parsedJson.Key);
            Assert.Equal("AdditionalKeyToEncrypt", parsedJson.AdditionalKey);
        }

        [Fact]
        public void DecryptKeys_WithEmptyFieldAndWithValidJson_CallsEncryptStringOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);
            var json = JsonConvert.SerializeObject(new TestAppSettings() { Key = "ValueToEncrypt_encrypted" });

            var encryptedJson = jsonCrypter.DecryptKey(json, "Key;;");
            var parsedJson = JsonConvert.DeserializeObject<TestAppSettings>(encryptedJson);

            // Additionally we test if the test crypter does its job.
            Assert.Equal("ValueToEncrypt", parsedJson.Key);
        }

        [Fact]
        public void Dispose_CallsDisposeOnCrypter()
        {
            var crypterMock = Mocks.Crypter;
            var jsonCrypter = new JsonConfigCrypter(crypterMock.Object);

            jsonCrypter.Dispose();

            crypterMock.Verify(crypter => crypter.Dispose());
        }
    }
}