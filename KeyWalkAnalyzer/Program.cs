using System.Collections.Generic;
using CommandLine.Text;
using CommandLine;
using KeyWalkAnalyzer;
using Newtonsoft.Json;

using CommandLine;

public class Program
{
    public static async Task Main(string[] args)
    {
        Program program = new Program();
        await program.Run(args);
    }

    public async Task Run(string[] args)
    {
        /*
        string tempFolderPath = Path.GetTempPath();
        string tempFileName = "keywalk_bloomfilter.txt";
        string tempFilePath = Path.Combine(tempFolderPath, tempFileName);
        if (!File.Exists(tempFilePath))
        {
            bloomFilter = new BloomFilter(551509767, 0.001F, tempFilePath);
            bloomFilter.Save();
        }
        else
        {
            bloomFilter = new BloomFilter(tempFilePath);
        }
        */
        var keywalkRepo = new KeywalkRepo("Data Source=DESKTOP-0MQNB1C\\SQLEXPRESS;Initial Catalog=Keywalk2;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;");
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a file path or a folder path as a command-line argument.");
            return;
        }
        ProcessFileClass processFile = new ProcessFileClass(keywalkRepo);

        Parser.Default.ParseArguments<CommandLineOptions>(args)
         .WithParsed<CommandLineOptions>(async options =>
         {
             if (options.ProcessFileOptions != null)
             {
                 string inputPath = options.ProcessFileOptions.InputPath;

                 if (File.Exists(inputPath) && Path.GetExtension(inputPath).ToLower() == ".txt")
                 {
                     await processFile.ProcessFile(inputPath);
                 }
                 else if (Directory.Exists(inputPath))
                 {
                     var txtFiles = Directory.GetFiles(inputPath, "*.txt");
                     int idx = 0;
                     foreach (string txtFile in txtFiles)
                     {
                         Console.WriteLine($"Processing {txtFile} {idx++} of {txtFiles.Length}");
                         await processFile.ProcessFile(txtFile);
                     }
                 }
                 else
                 {
                     Console.WriteLine($"The specified path \"{inputPath}\" is not a valid file or folder.");
                 }
             }
             else if (options.GeneratePasswordsOptions != null)
             {
                 string commandString = options.GeneratePasswordsOptions.CommandString;
                 int length = options.GeneratePasswordsOptions.Length;
                 string startingPoint = options.GeneratePasswordsOptions.StartingPoint;

                 List<string> generatedPasswords = processFile.GeneratePasswords(commandString, length, startingPoint);

                 // Do something with the generatedPasswords, e.g., save them to a file or process them
             }
         });
    }
}

[Verb("process", HelpText = "Process a file or folder.")]
public class ProcessFileOptions
{
    [Value(0, Required = true, HelpText = "File path or folder path to process.")]
    public string InputPath { get; set; }
}

[Verb("generate", HelpText = "Generate passwords based on command string, length, and starting point.")]
public class GeneratePasswordsOptions
{
    [Option('c', "command", Required = true, HelpText = "Command string to use for password generation.")]
    public string CommandString { get; set; }

    [Option('l', "length", Required = true, HelpText = "Length of the passwords to generate.")]
    public int Length { get; set; }

    [Option('s', "starting-point", Required = true, HelpText = "String of characters to use as starting points.")]
    public string StartingPoint { get; set; }
}

public class CommandLineOptions
{
    public ProcessFileOptions ProcessFileOptions { get; set; }

    public GeneratePasswordsOptions GeneratePasswordsOptions { get; set; }
}