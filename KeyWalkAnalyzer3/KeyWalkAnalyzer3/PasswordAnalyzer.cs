using System;
using System.Collections.Generic;

namespace KeyboardPathAnalysis
{

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
            var path = pathAnalyzer.GenerateKeyPath(password);
            var fingerprint = pathAnalyzer.EncodePath(path);

            // Group passwords by similar fingerprints
            var similarGroup = patternGroups
                .FirstOrDefault(g => pathAnalyzer.CalculateSimilarity(g.Key, fingerprint) > 0.8);

            if (!string.IsNullOrEmpty(similarGroup.Key))
            {
                patternGroups[similarGroup.Key].Add(password);
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
}