using System.Collections.Concurrent;
using KeyWalkAnalyzer;

public class ProcessFileClass
{
    private char[,] table;
    private Helpers helper;
    private HashSet<string> passwordHashSet = new HashSet<string>();
    private BloomFilter bloomFilter { get; set; }
    public KeywalkRepo KeywalkRepo { get; }

    private SemaphoreSlim processFilesSemaphore = new SemaphoreSlim(1, 1);

    public ProcessFileClass(KeywalkRepo keywalkRepo)
    {
        table = new char[,]
        {
            {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-','=',' '},
            {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[',']', '\\'},
            {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'',' ',' '},
            {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', ' ', ' ', ' '}
        };

        helper = new Helpers();
        KeywalkRepo = keywalkRepo;
    }

    public async Task ProcessFile(string fileIn)
    {
        FileInfo file = new FileInfo(fileIn);
        List<string> passwords = ReadPasswordsFromFile(file);
        var existingFile = KeywalkRepo.GetCachedFileProcessed(fileIn);

        if (existingFile != null && existingFile.IsProcessed)
        {
            CachePasswords(passwords);
            return;
        }

        string flagFile = GetFlagFilePath(file);

        if (File.Exists(flagFile))
        {
            Console.WriteLine($"File {file.Name} already processed");
            return;
        }

        ConcurrentDictionary<string, Tuple<List<Command>, int>> duplicateCommandLists = new ConcurrentDictionary<string, Tuple<List<Command>, int>>();

        await ProcessPasswords(passwords, duplicateCommandLists);

        // Do something with duplicateCommandLists if needed
    }

    private List<string> ReadPasswordsFromFile(FileInfo file)
    {
        return File.ReadAllLines(file.FullName).ToList();
    }

    private void CachePasswords(List<string> passwords)
    {
        Console.WriteLine("Caching Passwords...");
        foreach (var p in passwords)
        {
            passwordHashSet.Add(p);
        }
        Console.WriteLine("Done.");
    }

    private string GetFlagFilePath(FileInfo file)
    {
        string keywalkPath = Path.Combine(file.Directory.Parent.FullName, "keywalks");
        return Path.Combine(keywalkPath, $"flag_{file.Name}");
    }

    public List<string> GeneratePasswords(string commandString, int length, string startingPoint)
    {
        var pathfinder = new PathFinder(table);
        var commands = pathfinder.StringToCommands(commandString);
        List<string> generatedPasswords = new List<string>();

        foreach (char startChar in startingPoint)
        {
            TableTraversal tableTraversal = new TableTraversal(table);
            var generatedPassword = tableTraversal.BuildString(startChar, commands.ToArray(), length);
            Console.WriteLine($"Rebuilt string: {generatedPassword}");
            generatedPasswords.Add(generatedPassword);
        }

        return generatedPasswords;
    }

    private async Task ProcessPasswords(List<string> passwords, ConcurrentDictionary<string, Tuple<List<Command>, int>> duplicateCommandLists)
    {
        int maxConcurrency = Environment.ProcessorCount;
        var tasks = new List<Task>();
        SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency);
        int completedCount = 0;
        double percentComplete = 0;

        foreach (var p in passwords.Where(p => p.Length >= 4))
        {
            if (passwordHashSet.Contains(p))
                continue;

            await semaphore.WaitAsync();
            var task = ProcessPassword(p, semaphore, duplicateCommandLists);
            tasks.Add(task);
            UpdateProgress(passwords.Count, ref completedCount, ref percentComplete);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ProcessPassword(string password, SemaphoreSlim semaphore, ConcurrentDictionary<string, Tuple<List<Command>, int>> duplicateCommandLists)
    {
        await Task.Run(() =>
        {
            var simplifiedPassword = helper.SimplifyPassword(password).Trim();

            var pathfinder = new PathFinder(table);
            var foundPath = pathfinder.FindPath(password, simplifiedPassword);
            List<Command> reducedCommands = foundPath.ToList();
            string reducedString = pathfinder.ReducedToString(reducedCommands, false);

            reducedCommands = pathfinder.StringToCommands(reducedString);
            duplicateCommandLists.AddOrUpdate(reducedString,
    key => Tuple.Create(reducedCommands, 1),
    (key, oldValue) => Tuple.Create(reducedCommands, oldValue.Item2 + 1)
    );
            semaphore.Release();
            // Increment the completed task count
        });
    }

    private void UpdateProgress(int passwordCount, ref int completedCount, ref double percentComplete)
    {
        double newPercentComplete = (double)completedCount / passwordCount * 100;
        if (newPercentComplete - percentComplete >= 10)
        {
            percentComplete = newPercentComplete;
            Console.WriteLine($"Progress: {percentComplete:F}%");
        }
    }
}