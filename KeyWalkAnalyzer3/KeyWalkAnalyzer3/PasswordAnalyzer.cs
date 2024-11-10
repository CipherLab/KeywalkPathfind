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

    private char GetNextCharInRow(char currentChar, bool goingRight)
    {
        var pos = _keyboard.GetKeyPosition(currentChar);
        if (pos == null) return currentChar;

        // Get the row from QWERTY layout
        var row = pos.Row switch
        {
            0 => "1234567890-=",
            1 => "qwertyuiop[]",
            2 => "asdfghjkl;'",
            3 => "zxcvbnm,./",
            _ => ""
        };

        int currentIndex = row.IndexOf(currentChar);
        if (currentIndex == -1) return currentChar;

        if (goingRight)
        {
            // Move right, wrap around to start if at end
            return currentIndex < row.Length - 1 ? row[currentIndex + 1] : row[0];
        }
        else
        {
            // Move left, wrap around to end if at start
            return currentIndex > 0 ? row[currentIndex - 1] : row[row.Length - 1];
        }
    }

    private char GetNextCharInColumn(char currentChar, bool goingDown)
    {
        var pos = _keyboard.GetKeyPosition(currentChar);
        if (pos == null) return currentChar;

        // Get the column from QWERTY layout
        var column = pos.Col switch
        {
            0 => "1234567890-=",
            1 => "qwertyuiop[]",
            2 => "asdfghjkl;'",
            3 => "zxcvbnm,./",
            _ => ""
        };

        int currentIndex = column.IndexOf(currentChar);
        if (currentIndex == -1) return currentChar;

        if (goingDown)
        {
            // Move down, wrap around to start if at end
            return currentIndex < column.Length - 1 ? column[currentIndex + 1] : column[0];
        }
        else
        {
            // Move up, wrap around to end if at start
            return currentIndex > 0 ? column[currentIndex - 1] : column[column.Length - 1];
        }
    }

    public string GeneratePasswordFromPattern(string fingerprint, char startChar, int outputLength)
    {
        var sb = new StringBuilder();
        sb.Append(startChar);
        var currentChar = startChar;

        // Split the fingerprint into individual steps using ASCII characters
        var steps = Regex.Split(fingerprint, @"(?=[→←↑↓►◄▲▼◘])")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        var tempSteps = steps.Skip(1).ToList();//.Skip(1);
        while (steps.Count < outputLength)
            steps.AddRange(tempSteps);

        foreach (var step in steps)
        {
            if (step.Contains("(")) continue; // Skip encoded references

            try
            {
                switch (step)
                {
                    case "↓":
                    case "▼":
                        currentChar = GetNextCharInColumn(currentChar, true);
                        if (step == "▼")
                            sb.Append(currentChar);
                        break;

                    case "↑":
                    case "▲":
                        currentChar = GetNextCharInColumn(currentChar, false);
                        if (step == "▲")
                            sb.Append(currentChar);
                        break;

                    case "←":
                    case "◄":
                        currentChar = GetNextCharInRow(currentChar, false);
                        if (step == "◄")
                            sb.Append(currentChar);
                        break;

                    case "→":
                    case "►":
                        currentChar = GetNextCharInRow(currentChar, true);
                        if (step == "►")
                            sb.Append(currentChar);
                        break;

                    case "◘":
                        sb.Append(currentChar);
                        break;
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
            var basePattern = GeneratePasswordFromPattern(command, startChar, targetLength);

            if (basePattern.Length == 0)
                continue;

            // Calculate how many times we need to repeat the pattern
            int repetitions = (targetLength + basePattern.Length - 1) / basePattern.Length;
            var password = string.Concat(Enumerable.Repeat(basePattern, repetitions));

            // Trim to the target length
            password = password.Substring(0, targetLength);

            passwords.Add(password);
        }

        return passwords;
    }
}
