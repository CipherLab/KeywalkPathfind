using System.Text;
using System.Text.RegularExpressions;

namespace KeyWalkAnalyzer3;
public class PasswordAnalyzer
{
    private readonly PathAnalyzer _pathAnalyzer;
    private readonly KeyboardLayout _keyboard;
    private readonly Dictionary<string, List<string>> _patternGroups;
    private List<PathStep> _path;

    public PasswordAnalyzer(
        KeyboardLayout keyboard,
        PathAnalyzer pathAnalyzer)
    {
        _keyboard = keyboard;
        _pathAnalyzer = pathAnalyzer;
        _patternGroups = new Dictionary<string, List<string>>();
        _path = new List<PathStep>();
    }

    public void AnalyzePasswords(IEnumerable<string> passwords)
    {
        foreach (var password in passwords.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            AnalyzePassword(password);
        }
    }

    public void AnalyzePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return;

        _path = _pathAnalyzer.GenerateKeyPath(password);
        var fingerprint = _pathAnalyzer.EncodePath(_path);

        var similarGroupKey = _patternGroups
            .Where(g => _pathAnalyzer.CalculateSimilarity(g.Key, fingerprint) > 0.8)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (similarGroupKey != null)
            _patternGroups[similarGroupKey].Add(password);
        else
            _patternGroups[fingerprint] = new List<string> { password };
    }

    public record PasswordAnalysis(
        Dictionary<string, List<string>> PatternGroups,
        List<PathStep> Path);

    public PasswordAnalysis GetAnalysis() => new(
        _patternGroups,
        _path);

    public string GeneratePasswordFromPattern(string fingerprint, char startChar, int outputLength)
    {
        var sb = new StringBuilder();
        sb.Append(startChar);
        var currentChar = startChar;

        // Split the fingerprint into individual steps using ASCII characters
        var steps = Regex.Split(fingerprint, @"(?=[→←↑↓►◄▲▼◘])")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        var tempSteps = steps.ToList();

        // Ensure enough steps for the desired output length
        while (steps.Count(x =>
            x == "►" ||
            x == "◄" ||
            x == "▲" ||
            x == "▼" ||
            x == "◘") < outputLength + 1)
            steps.AddRange(tempSteps);

        foreach (var step in steps)
        {
            try
            {
                switch (step)
                {
                    case "↓":
                    case "▼":
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, true);
                        if (step == "▼")
                            sb.Append(currentChar);
                        break;

                    case "↑":
                    case "▲":
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, false);
                        if (step == "▲")
                            sb.Append(currentChar);
                        break;

                    case "←":
                    case "◄":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, false);
                        if (step == "◄")
                            sb.Append(currentChar);
                        break;

                    case "→":
                    case "►":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, true);
                        if (step == "►")
                            sb.Append(currentChar);
                        break;

                    case "◘":
                        sb.Append(currentChar);
                        break;

                    default:
                        throw new Exception("Invalid step: " + step);
                }
                if (sb.Length >= outputLength)
                    break;
            }
            catch
            {
                // If we hit a boundary, just continue with current char
                continue;
            }
        }

        return sb.ToString();
    }

    public string GeneratePassword(string command, char startChar, int? length = null)
    {
        int targetLength = length ?? command.Length;
        var password = GeneratePasswords(command, startChar.ToString(), targetLength).First();

        return password;
    }

    public IEnumerable<string> GeneratePasswords(string command, string startingPoints, int? length = null)
    {
        int targetLength = length ?? command.Length;
        var passwords = new List<string>();

        foreach (char startChar in startingPoints)
        {
            // Generate the initial pattern
            var password = GeneratePasswordFromPattern(command, startChar, targetLength);

            if (password.Length == 0)
                continue;

            passwords.Add(password);
        }

        return passwords;
    }
}
