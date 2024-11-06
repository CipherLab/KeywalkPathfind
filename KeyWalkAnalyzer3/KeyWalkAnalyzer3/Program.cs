// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;
using KeyboardPathAnalysis;


var pathAnalyzer = new PathAnalyzer();
var passwordAnalyzer = new PasswordAnalyzer();

// Test password
string password = "Hello123";
Console.WriteLine($"Original password: {password}");

// Generate path and fingerprint
var path = pathAnalyzer.GenerateKeyPath(password);
var fingerprint = pathAnalyzer.EncodePath(path);
Console.WriteLine($"Generated fingerprint: {fingerprint}");

// Store in analyzer for pattern matching
passwordAnalyzer.AnalyzePassword(password);

// Get pattern groups to see similar patterns
var patternGroups = passwordAnalyzer.GetPatternGroups();

Console.WriteLine("\nPattern Groups:");
foreach (var group in patternGroups)
{
    Console.WriteLine($"Pattern: {group.Key}");
    Console.WriteLine($"Passwords: {string.Join(", ", group.Value)}");
}

// You can add more test passwords to see pattern matching
passwordAnalyzer.AnalyzePassword("Hello456");
passwordAnalyzer.AnalyzePassword("HelloABC");

// Print updated groups
Console.WriteLine("\nUpdated Pattern Groups:");
patternGroups = passwordAnalyzer.GetPatternGroups();
foreach (var group in patternGroups)
{
    Console.WriteLine($"Pattern: {group.Key}");
    Console.WriteLine($"Passwords: {string.Join(", ", group.Value)}");
}