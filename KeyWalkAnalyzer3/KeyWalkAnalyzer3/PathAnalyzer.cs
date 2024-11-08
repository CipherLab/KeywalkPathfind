// Updated PathAnalyzer.cs

using System.Text;

namespace KeyWalkAnalyzer3;

public class PathAnalyzer
{
    private readonly KeyboardLayout keyboard;
    private readonly AStar pathFinder;

    public PathAnalyzer()
    {
        keyboard = new KeyboardLayout();
        pathFinder = new AStar(keyboard);
    }

    public List<PathStep> GenerateKeyPath(string password)
    {
        if (string.IsNullOrEmpty(password))
            return new List<PathStep>();

        var completePath = new List<PathStep>();

        for (int i = 0; i < password.Length - 1; i++)
        {
            var path = pathFinder.FindPath(password[i], password[i + 1]);

            // Filter out release steps for adjacent keys
            var filteredPath = path.Where(step =>
                step.ToString().Contains("right") ||
                step.ToString().Contains("press")
            ).ToList();

            completePath.AddRange(filteredPath);
        }

        return OptimizePath(completePath);
    }

    // Rest of the code remains the same as in the previous implementation
    private List<PathStep> OptimizePath(List<PathStep> path)
    {
        var optimized = new List<PathStep>();
        if (path.Count == 0) return optimized;

        var currentStep = path[0];
        int count = 1;

        for (int i = 1; i < path.Count; i++)
        {
            if (path[i].ToString() == currentStep.ToString())
            {
                count++;
            }
            else
            {
                if (count > 1)
                {
                    optimized.Add(new PathStep('\0', $"{currentStep.Direction} * {count}", currentStep.IsPress));
                }
                else
                {
                    optimized.Add(currentStep);
                }
                currentStep = path[i];
                count = 1;
            }
        }

        // Add the last group
        if (count > 1)
        {
            optimized.Add(new PathStep('\0', $"{currentStep.Direction} * {count}", currentStep.IsPress));
        }
        else
        {
            optimized.Add(currentStep);
        }

        return RemoveRedundantMoves(optimized);
    }

    private List<PathStep> RemoveRedundantMoves(List<PathStep> path)
    {
        var simplified = new List<PathStep>();
        var redundantPairs = new Dictionary<string, string>
        {
            {"up", "down"},
            {"down", "up"},
            {"left", "right"},
            {"right", "left"}
        };

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (!path[i].IsPress && !path[i + 1].IsPress)
            {
                if (redundantPairs.TryGetValue(path[i].Direction, out string opposite))
                {
                    if (path[i + 1].Direction == opposite)
                    {
                        i++; // Skip both moves
                        continue;
                    }
                }
            }
            simplified.Add(path[i]);
        }

        if (path.Count > 0)
            simplified.Add(path[path.Count - 1]);

        return simplified;
    }

    // Rest of the code remains the same
    public string EncodePath(List<PathStep> path)
    {
        var sb = new StringBuilder();
        var dictionary = new Dictionary<string, int>();
        var currentPhrase = new StringBuilder();
        var lookAheadWindow = 4; // Adjustable window size

        for (int i = 0; i < path.Count; i++)
        {
            currentPhrase.Clear();
            int maxMatch = 0;
            int matchPosition = -1;

            // Look for matches in the dictionary
            for (int j = i; j < Math.Min(i + lookAheadWindow, path.Count); j++)
            {
                currentPhrase.Append(path[j].ToString() + "|");
                string phrase = currentPhrase.ToString();

                if (dictionary.TryGetValue(phrase, out int position))
                {
                    if (phrase.Length > maxMatch)
                    {
                        maxMatch = phrase.Length;
                        matchPosition = position;
                    }
                }
            }

            if (maxMatch > 0)
            {
                // Encode as (position, length)
                sb.Append($"({matchPosition},{maxMatch})");
                i += maxMatch / 2 - 1; // Skip matched steps
            }
            else
            {
                // Add new phrase to dictionary and output literal
                string newPhrase = path[i].ToString() + "|";
                dictionary[newPhrase] = dictionary.Count;
                sb.Append(path[i].ToString());
            }
        }

        return sb.ToString();
    }

    public double CalculateSimilarity(string fingerprint1, string fingerprint2)
    {
        if (string.IsNullOrEmpty(fingerprint1) || string.IsNullOrEmpty(fingerprint2))
            return 0;

        // Using Levenshtein distance for similarity
        int distance = LevenshteinDistance(fingerprint1, fingerprint2);
        int maxLength = Math.Max(fingerprint1.Length, fingerprint2.Length);

        return 1 - (double)distance / maxLength;
    }

    private int LevenshteinDistance(string s1, string s2)
    {
        int[,] distance = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            distance[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++)
            distance[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(
                    distance[i - 1, j] + 1,     // deletion
                    distance[i, j - 1] + 1),    // insertion
                    distance[i - 1, j - 1] + cost); // substitution
            }
        }

        return distance[s1.Length, s2.Length];
    }
}
