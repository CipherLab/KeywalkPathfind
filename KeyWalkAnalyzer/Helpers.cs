using Newtonsoft.Json;
using System.Data.HashFunction.MurmurHash;
using System.Security.Cryptography;
using System.Text;

public class Helpers
{
    private readonly Dictionary<char, char> characterMap = new()
    {
        {'!', '1'}, {'@', '2'}, {'#', '3'}, {'$', '4'}, {'%', '5'},
        {'^', '6'}, {'&', '7'}, {'*', '8'}, {'(', '9'}, {')', '0'},
        {'_', '-'}, {'+', '='}, {'{', '['}, {'}', ']'}, {':', ';'},
        {'<', ','}, {'>', '.'}, {'?', '/'}, {'"', '\''}, {'\\', '\\'}
    };

    private readonly Dictionary<char, char> characterMapShift = new()
    {
        {'!', '1'}, {'@', '2'}, {'#', '3'}, {'$', '4'}, {'%', '5'},
        {'^', '6'}, {'&', '7'}, {'*', '8'}, {'(', '9'}, {')', '0'},
        {'_', '-'}, {'+', '='}, {'{', '['}, {'}', ']'}, {':', ';'},
        {'<', ','}, {'>', '.'}, {'?', '/'}, {'"', '\''}, {'\\', '\\'}
    };

    public string SimplifyPassword(string password)
    {
        StringBuilder simplified = new StringBuilder(password.Length);

        foreach (char c in password)
        {
            char lowerChar = char.ToLower(c);
            if (characterMap.TryGetValue(lowerChar, out char replacement))
            {
                simplified.Append(replacement);
            }
            else
            {
                simplified.Append(lowerChar);
            }
        }

        return simplified.ToString();
    }

    public List<string> GeneratePassword(string commandString, int length, string startingPoint)
    {
        PathFinder pathfinder = new(table: GetSimplifiedTable());

        var commands = pathfinder.StringToCommands(commandString);

        List<string> generatedPasswords = new List<string>();

        foreach (char startChar in startingPoint)
        {
            TableTraversal tableTraversal = new TableTraversal(table: GetSimplifiedTable());
            var generatedPassword = tableTraversal.BuildString(startChar, commands.ToArray(), length);

            //Console.WriteLine($"Rebuilt string: {generatedPassword}");
            if (!string.IsNullOrEmpty(generatedPassword))
                generatedPasswords.Add(generatedPassword);
        }

        return generatedPasswords;
    }

    public bool IsShiftCharacter(char input)
    {
        if (char.IsLetter(input))
        {
            return char.IsUpper(input);
        }
        else if (char.IsDigit(input))
        {
            return false;
        }
        else
        {
            if (characterMap.TryGetValue(input, out char opposite))
            {
                return false;
            }
            else if (characterMapShift.TryGetValue(input, out char opposite2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public char GetOppositeShiftCharacter(char input)
    {
        if (char.IsLetter(input))
        {
            return char.IsUpper(input) ? char.ToLower(input) : char.ToUpper(input);
        }
        else
        {
            if (characterMap.TryGetValue(input, out char opposite))
            {
                return opposite;
            }
            else if (characterMapShift.TryGetValue(input, out char opposite2))
            {
                return opposite2;
            }
            else
            {
                // Look up the lowercase equivalent of the input character
                char lowerInput = char.ToLower(input);
                if (characterMap.TryGetValue(lowerInput, out opposite))
                {
                    return opposite;
                }
            }
        }

        return input;
    }

    private string ComputeMurmurHash3(string input)
    {
        var murmurHash = MurmurHash3Factory.Instance.Create();
        var hashValue = murmurHash.ComputeHash(Encoding.UTF8.GetBytes(input));
        string base64String = hashValue.AsBase64String();

        // Sanitize the base64 string for use as a filename
        string sanitizedString = base64String
            .Replace('/', '_') // Replace '/' with '_'
            .Replace('+', '-'); // Replace '+' with '-'

        return sanitizedString;
    }

    private void SaveToFile(string keywalkPath, string flagFile, List<Command> commands, int counts, string key)
    {
        Console.WriteLine($"Saving: {commands.Count()} commands");
        foreach (var item in commands)
        {
            ///Console.WriteLine($"Pattern Length: {item.Value.Item1.Count()}, Occurrences: {item.Value.Item2}");

            // Serialize the pattern to a JSON string
            string json = JsonConvert.SerializeObject(commands, Formatting.Indented);

            // Check if a file with the hash alone exists
            string existingFilePath = $"*_{key}.json";

            if (!Directory.Exists(keywalkPath))
                Directory.CreateDirectory(keywalkPath);

            var existingFiles = Directory.GetFiles(keywalkPath, existingFilePath);

            if (existingFiles.Length > 0)
            {
                // If a file exists, parse out the "Occurrences" value
                string existingFile = existingFiles[0];
                int existingOccurrences = int.Parse(Path.GetFileNameWithoutExtension(existingFile).Split('_')[0]);

                // Increment the occurrences count and rename the file
                int newOccurrences = existingOccurrences + counts;
                string newFilePath = $"{newOccurrences}_{key}.json";
                File.Move(existingFile, Path.Combine(keywalkPath, newFilePath));
            }
            else
            {
                // If no file exists, create a new file with the hash and occurrences count
                string filePath = $"{counts}_{key}.json";
                File.WriteAllText(Path.Combine(keywalkPath, filePath), json);
            }

            // Create a flag file for the processed filename
            File.Create(flagFile).Close();
        }
    }

    public char[,] GetSimplifiedTable()
    {
        return new char[,]
      {
            {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-','=',' '},
            {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[',']', '\\'},
            {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'',' ',' '},
            {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', ' ', ' ', ' '}
      };
    }

    public char[,] GetShiftTable()
    {
        return new char[,]
      {
            {'!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_','+',' '},
            {'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{','}', '|'},
            {'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '"',' ',' '},
            {'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?', ' ', ' ', ' '}
      };
    }
}