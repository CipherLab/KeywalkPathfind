using System;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3;

public class PasswordAnalyzer
{
    private readonly PathAnalyzer pathAnalyzer;
    private readonly Dictionary<string, List<string>> patternGroups;
    private readonly KeyboardLayout keyboard;

    public PasswordAnalyzer()
    {
        pathAnalyzer = new PathAnalyzer();
        patternGroups = new Dictionary<string, List<string>>();
        keyboard = new KeyboardLayout();
    }

    private List<PathStep> _path = new List<PathStep>();
    public void AnalyzePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        _path = pathAnalyzer.GenerateKeyPath(password);
        var fingerprint = pathAnalyzer.EncodePath(_path);

        // Group passwords by similar fingerprints
        var similarGroupKey = patternGroups
            .Where(g => pathAnalyzer.CalculateSimilarity(g.Key, fingerprint) > 0.8)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (similarGroupKey != null)
        {
            patternGroups[similarGroupKey].Add(password);
        }
        else
        {
            patternGroups[fingerprint] = new List<string> { password };
        }
    }

    public Dictionary<string, List<string>> GetPatternGroups()
    {
        return patternGroups;
    }
    public List<PathStep> GetPath()
    {
        return _path;
    }
    public string GeneratePasswordFromStartChar(char startChar)
    {
        // Implement the specific keyboard path pattern generation
        // The pattern is roughly: 3 in a row, down, back 3, 3 in a row
        var currentPos = keyboard.GetKeyPosition(startChar);
        if (currentPos == null)
        {
            throw new ArgumentException($"Invalid start character: {startChar}");
        }

        // Generate password following the pattern
        var password = new List<char> { startChar };

        // First 3 in a row (horizontally)
        var horizontalNeighbors = keyboard.GetHorizontalNeighbors(startChar);
        password.AddRange(horizontalNeighbors.Take(2));

        // Move down
        var downChar = keyboard.GetVerticalNeighbor(password[2], 1);
        password.Add(downChar);

        // Back 3 (horizontally from the down character)
        var backNeighbors = keyboard.GetHorizontalNeighbors(downChar);
        password.AddRange(backNeighbors.Take(2));

        return new string(password.ToArray());
    }
}
