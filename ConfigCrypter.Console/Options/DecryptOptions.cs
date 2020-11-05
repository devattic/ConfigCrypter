using CommandLine;

namespace ConfigCrypter.Console.Options
{
    [Verb("decrypt", HelpText = "Decrypts a key in the config file.")]
    class DecryptOptions : CommandlineOptions { }
}
