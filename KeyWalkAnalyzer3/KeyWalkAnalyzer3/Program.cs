using KeyWalkAnalyzer3;
using System.Text;
using System.Text.RegularExpressions;

var pathAnalyzer = new PathAnalyzer();
var passwordAnalyzer = new PasswordAnalyzer();

// Test different keyboard walks
string[] keyWalks = new[] {
    "123qwe",  // Diagonal walk down
};

Console.WriteLine("Password Generation from Fingerprints:\n");

foreach (var password in keyWalks)
{
    Console.WriteLine($"Original Password: {password}");

    // Generate path and fingerprint
    var path = pathAnalyzer.GenerateKeyPath(password);
    var fingerprint = pathAnalyzer.EncodePath(path);
    Console.WriteLine($"Fingerprint: {fingerprint}");

    // Generate new passwords from different starting points using the same pattern
    Console.WriteLine("\nGenerated passwords from different starting points:");

    char[] startingChars = new[] { 'q', '1', 'a', 'z' };
    foreach (var startChar in startingChars)
    {
        var generatedPassword = pathAnalyzer.GeneratePasswordFromPattern(fingerprint, startChar);
        Console.WriteLine($"Starting from '{startChar}': {generatedPassword}");
    }

    Console.WriteLine("\n-------------------\n");
}
