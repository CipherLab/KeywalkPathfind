using System.Text;
using System.Text.RegularExpressions;

namespace KeyWalkAnalyzer3;
public class PasswordAnalyzer
{
    private readonly PathAnalyzer _pathAnalyzer;
    private readonly KeyboardLayout _keyboard;
    private List<PathStep> _path;
    private string _smallestPath;

    public PasswordAnalyzer(
        KeyboardLayout keyboard,
        PathAnalyzer pathAnalyzer)
    {
        _keyboard = keyboard;
        _pathAnalyzer = pathAnalyzer;
        _path = new List<PathStep>();
    }

    public void AnalyzePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return;

        _path = _pathAnalyzer.GenerateKeyPath(password);
        var joined = String.Join("", _path);

        _smallestPath = _pathAnalyzer.EncodePath(_path);
    }

    public string GetSmallestPath() => _smallestPath;

    public record PasswordAnalysis(
        List<PathStep> Path);

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
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, true);
                        break;

                    case "▼":
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, true);
                        sb.Append(currentChar);
                        break;

                    case "↑":
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, false);
                        break;

                    case "▲":
                        currentChar = _keyboard.GetNextCharInColumn(currentChar, false);
                        sb.Append(currentChar);
                        break;

                    case "←":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, false);
                        break;

                    case "◄":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, false);
                        sb.Append(currentChar);
                        break;

                    case "→":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, true);
                        break;

                    case "►":
                        currentChar = _keyboard.GetNextCharInRow(currentChar, true);
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
