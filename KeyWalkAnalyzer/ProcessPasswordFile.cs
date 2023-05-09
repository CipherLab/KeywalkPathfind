using System.Collections.Concurrent;
using KeyWalkAnalyzer;

public class ProcessPasswordFile
{
    private char[,] table;
    private Helpers helper;
    private HashSet<string> passwordHashSet = new HashSet<string>();
    private BloomFilter bloomFilter { get; set; }
    public KeywalkRepo KeywalkRepo { get; }

    private SemaphoreSlim processFilesSemaphore = new SemaphoreSlim(1, 1);

    public ProcessPasswordFile(KeywalkRepo keywalkRepo)
    {
        helper = new Helpers();

        table = helper.GetSimplifiedTable();
        helper = new Helpers();
        KeywalkRepo = keywalkRepo;
    }

    private ConcurrentDictionary<string, Tuple<string, int>> foundCommandList = new ConcurrentDictionary<string, Tuple<string, int>>();
    private static ConcurrentDictionary<string, Tuple<string, int>> masterCommandList = new ConcurrentDictionary<string, Tuple<string, int>>();

    public async Task ProcessFile(string fileIn)
    {
        foundCommandList = new ConcurrentDictionary<string, Tuple<string, int>>();
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
        await ProcessPasswords(passwords);

        await processFilesSemaphore.WaitAsync();
        try
        {
            await KeywalkRepo.SaveToDatabaseAsync(foundCommandList);//.Where(x => x.Value.Item2 >= 1));
            await KeywalkRepo.SaveFileFlag(fileIn);
            CachePasswords(passwords);
            Console.WriteLine($"Cached Passwords: {passwordHashSet.Count()}");
        }
        finally
        {
            processFilesSemaphore.Release();
        }
    }

    private List<string> ReadPasswordsFromFile(FileInfo file)
    {
        return File.ReadAllLines(file.FullName).ToList();
    }

    private void CachePasswords(List<string> passwords)
    {
        Console.WriteLine($"Caching {passwords.Count()} Passwords...");
        int passwordHashSetCount = passwordHashSet.Count();
        foreach (var p in passwords)
        {
            passwordHashSet.Add(p);
        }

        Console.WriteLine($"Done. Added {passwordHashSet.Count() - passwordHashSetCount}");
    }

    private string GetFlagFilePath(FileInfo file)
    {
        string keywalkPath = Path.Combine(file.Directory.Parent.FullName, "keywalks");
        return Path.Combine(keywalkPath, $"flag_{file.Name}");
    }

    private async Task ProcessPasswords(List<string> passwords)
    {
        int maxConcurrency = Environment.ProcessorCount;
        var tasks = new List<Task>();
        int completedCount = 0;
        double percentComplete = 0;
        if (passwords.Count() < maxConcurrency)
            maxConcurrency = passwords.Count();

        using (SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency))
        {
            foreach (var p in passwords.Where(p => p.Length >= 4))
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        ProcessPassword(p);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                    // Increment the completed task count
                }));
                completedCount++;
                UpdateProgress(passwords.Count, completedCount, ref percentComplete);
            }

            await Task.WhenAll(tasks);
        }
    }

    private void ProcessPassword(string password)
    {
        var simplifiedPassword = helper.SimplifyPassword(password).Trim();

        var pathfinder = new PathFinder(table);
        var foundPath = pathfinder.FindPath(password, simplifiedPassword);
        var reducedPath = pathfinder.ReduceCommands(foundPath);
        string commandAsString = pathfinder.CommandsToString(reducedPath);

        // Use the original password if it's not equal to the simplified password.
        string passwordToUse = password != simplifiedPassword ? password : simplifiedPassword;

        if (string.IsNullOrEmpty(passwordToUse) || string.IsNullOrEmpty(commandAsString))
            return;

        // Increment the observed count in the foundCommandList and masterCommandList.
        IncrementCommandListCount(foundCommandList, passwordToUse, commandAsString);
        IncrementCommandListCount(masterCommandList, passwordToUse, commandAsString);
    }

    private void IncrementCommandListCount(ConcurrentDictionary<string, Tuple<string, int>> commandList, string password, string commandAsString)
    {
        commandList.AddOrUpdate(password,
            key => Tuple.Create(commandAsString, 1),
            (key, oldValue) => Tuple.Create(commandAsString, oldValue.Item2 + 1)
        );
    }

    private void UpdateProgress(int passwordCount, int completedCount, ref double percentComplete)
    {
        double newPercentComplete = (double)completedCount / passwordCount * 100;
        if (newPercentComplete - percentComplete >= 10)
        {
            percentComplete = newPercentComplete;
            Console.WriteLine($"Progress: {percentComplete:F}%");
        }
    }
}