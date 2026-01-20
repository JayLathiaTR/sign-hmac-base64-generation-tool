using SignHmacTutorial.Models;
using SignHmacTutorial.Services;

namespace SignHmacTutorial;

internal static class CliRunner
{
    public static void Run()
    {
        Console.WriteLine("\nSignHmacTutorial CLI mode (use UI by default).\n");

        Console.Write("SecretKey: ");
        string secretKey = Console.ReadLine() ?? string.Empty;

        Console.Write("Product (AIT/AIA): ");
        string productInput = (Console.ReadLine() ?? string.Empty).Trim();

        var product = string.Equals(productInput, "AIT", StringComparison.OrdinalIgnoreCase)
            ? ProductType.AIT
            : ProductType.AIA;

        string rawBody;
        if (product == ProductType.AIT)
        {
            Console.Write("sapSoldToAccount: ");
            string sapSoldToAccount = Console.ReadLine() ?? string.Empty;

            Console.Write("emsId (number): ");
            string emsIdText = Console.ReadLine() ?? string.Empty;
            if (!long.TryParse(emsIdText, out var emsId))
                throw new InvalidOperationException("emsId must be a number.");

            Console.Write("emFirmId: ");
            string emFirmId = Console.ReadLine() ?? string.Empty;

            Console.Write("productDomain: ");
            string productDomain = Console.ReadLine() ?? string.Empty;

            rawBody = PayloadBuilder.BuildAitJson(sapSoldToAccount, emsId, emFirmId, productDomain);
        }
        else
        {
            Console.Write("EmsId (string): ");
            string emsId = Console.ReadLine() ?? string.Empty;

            Console.Write("SapSoldToAccount: ");
            string sapSoldToAccount = Console.ReadLine() ?? string.Empty;

            Console.Write("EmFirmId: ");
            string emFirmId = Console.ReadLine() ?? string.Empty;

            rawBody = PayloadBuilder.BuildAiaJson(emsId, sapSoldToAccount, emFirmId);
        }

        Console.WriteLine("\nrawBody:\n" + rawBody);
        Console.WriteLine("\nHmac-Base64:\n" + HmacSigner.ComputeHmacBase64(secretKey, rawBody));
    }
}
