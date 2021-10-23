# DevAttic ConfigCrypter
![DevAttic ConfigCrypter Logo](devattic_configcrypter-normal256.png "DevAttic ConfigCrypter Logo")

## What is DevAttic ConfigCrypter
The DevAttic ConfigCrypter makes it easy for you to encrypt and decrypt config files. It consists of two parts. A command line utility that lets you encrypt keys in your JSON configuration files and a library that decrypts them on the fly in your .NET Standard applications.

## WARNING
Encrypted configuration files will not make your server infrastructure unhackable. Usually the certificate to decrypt is hosted on the same server as your web application. This means an attacker could decrypt your config if your server is not secure and the attacker gains access.

Additional security could be achieved by:
- Storing your certficate in the windows certificate store (supported by ConfigCrypter) and restricting access to it.
- Protect your certificate with a password that is embedded in your source code (currently not supported, but could be easily implemented).

Also these methods would not be perfectly safe. In fact it only makes things harder for an attacker.

Other possibilities would be using Environment variables or the Secret Manager tool, but in these variants the settings are completely unencrypted. For production scenarios Microsoft recommends using the Azure Key Vault, but also this is not perfectly safe and forces you to use Azure.

It is definitely required to secure your server infrastructure, as this is the only way to protect your connection strings and other sensitive configuration values.

## Advantages
- Lets you encrypt only certain keys in your config, so the rest of the config is still readable.
- Access the encrypted values the same way you are used to in your .NET Core applications.
- Lets you share config files or even check them in in your VCS without the need to remove sensitive information.

## Command line tool usage
To use DevAttic ConfigCrypter you will first need to create a self signed X509 certificate that is being used for the encryption and decryption. An easy way to do this is being described in this guide: [Link](https://www.claudiobernasconi.ch/2016/04/17/creating-a-self-signed-x509-certificate-using-openssl-on-windows/)

In fact you can follow every guide as long as the result is a certificate in .pfx format containing a public and private key.

If you now have your certificate you need to decide what keys you want to encrypt. Lets assume we have a JSON file that looks like this:

```json
{
   "Nested": {
      "KeyToEncrypt": "This will be encrypted"
   }
}
```

We want to encrypt the value with the key `Nested.KeyToEncrypt`. Notice the separation of the keys with a dot. How the key is interprated and what kind of syntax is used to define your key is up to the IConfigCrypter implementation.

Currently only JSON is supported and the `JsonConfigCrypter` is using the JSONPath Syntax to define your keys ([Link](https://goessner.net/articles/JsonPath/)). Altough JSONPath usually needs a $ to define the root of the object you can leave it out here.

To install the crypter [command line utility](https://www.nuget.org/packages/DevAttic.ConfigCrypter.Console/) just execute `dotnet tool install -g DevAttic.ConfigCrypter.Console`. After that you can use it with the command `config-crypter` from your command line.

To encrypt our key from above we simple execute:
`config-crypter encrypt -p c:\path\to\cert.pfx -f c:\path\to\config.json -k "Nested.KeyToEncrypt"`.
After that a new files named `config_encrypted.json` should be created at the same folder as your original config file. This file is now the same as the original one except for the fact that the value for the passed key has been encrypted.

If you want to prevent the creation of a new file you can simply pass --replace (-r) as additional paramter to the command and the original file will be replaced.

To decrypt the file again you can simply execute:
`config-crypter decrypt -p c:\path\to\cert.pfx -f c:\path\to\config_encrypted.json -k "Nested.KeyToEncrypt"`

## Command line arguments
The following command line arguments can be passed for the encrypt and decrypt command.
```
-p, --path       (Group: CertLocation) Path of the certificate.
-n, --name       (Group: CertLocation) The subject name of the certificate (CN). This can only be used in Windows environments.
-k, --key        Required. The key to encrypt in the config file.
-f, --file       Required. The path to the config file.
-r, --replace    (Default: false) Replaces the original file if passed as parameter.
--format         (Default: Json) The format of the config file.
--help           Display this help screen.
--version        Display version information.
```

## .NET Core integration
Install the nuget package [DevAttic.ConfigCrypter](https://www.nuget.org/packages/DevAttic.ConfigCrypter/)

To use your encrypted configuration file in your .NET Core applications, DevAttic ConfigCrypter provides convenient extension methods to register the needed configuration providers.

There are two extension methods that can be used to integrate encrypted configuration files.

- `AddEncryptedAppSettings`: Adds a configuration provider that decrypts certain keys of the appsettings.json file.
- `AddEncryptedJsonConfig`: Adds a configuration provider  that decrypts certain keys of a given JSON configuration file.

## ASP.NET Core example
If you encrypted a key in the appsettings.json file of your ASP.NET Core web application you can use the following to enable the decryption.

The easiest way to enable decryption is to modify the `CreateHostBuilder` function in your `Program.cs` file.

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .ConfigureAppConfiguration(cfg =>
        {
            cfg.AddEncryptedAppSettings(crypter =>
            {
                crypter.CertificatePath = "cert.pfx";
                crypter.KeysToDecrypt = new List<string> { "Nested:KeyToEncrypt" };
            });
        });
```

As you can see enabling decryption is as simple as adding a line of code. You just need to specify the path to the certificate and the keys that should be decrypted.

If you want to also use the environment specific appsettings files (appsettings.Development.json, appsettings.Production.json) please use the overloaded method to configure as shown below:

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .ConfigureAppConfiguration((hostingContext, cfg) =>
        {
            cfg.AddEncryptedAppSettings(hostingContext.HostingEnvironment, crypter =>
            {
                crypter.CertificatePath = "cert.pfx";
                crypter.KeysToDecrypt = new List<string> { "Nested:KeyToEncrypt" };
            });
        });
```
Notice how to `HostingEnvironment` property of the `hostingContext` variable is used.

Notice that nested keys have to be separated with colons in this case, because this is the default way to access nested configuration properties in .NET Core ([Link](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#configuration-keys-and-values)).

## How to generate a certificate
Certificates can be generated by using Openssl.
An example certificate is already in the project and the encrypted string in the example appsettings files has been encrypted with it.
To generate a certificate you could use the following commands:

`openssl genrsa 2048 > private.key`

`openssl req -new -x509 -nodes -sha1 -days 365 -key private.key > public.cer`

`openssl pkcs12 -export -in public.cer -inkey private.key -out cert.pfx`
