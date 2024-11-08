using System;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3;


public class PasswordAnalyzer
{
    private readonly PathAnalyzer pathAnalyzer;
    private readonly Dictionary<string, List<string>> patternGroups;

    public PasswordAnalyzer()
    {
        pathAnalyzer = new PathAnalyzer();
        patternGroups = new Dictionary<string, List<string>>();
    }

    public void AnalyzePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var path = pathAnalyzer.GenerateKeyPath(password);
        var fingerprint = pathAnalyzer.EncodePath(path);

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
}