using CommandLine;

namespace ConfigCrypter.Console.Options
{
    [Verb("encrypt", HelpText = "Encrypts a key in the config file.")]
    public class EncryptOptions : CommandlineOptions { }
}
