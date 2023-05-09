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

    private Helpers helper = new Helpers();

    public async Task Run(string[] args)
    {
        var keywalkRepo = new KeywalkRepo("Data Source=DESKTOP-0MQNB1C\\SQLEXPRESS;Initial Catalog=Keywalk_Shift;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;");
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a file path or a folder path as a command-line argument.");
            return;
        }
        ProcessPasswordFile processFile = new ProcessPasswordFile(keywalkRepo);
        List<Task> parseTasks = new List<Task>();

        var parseTask = Parser.Default.ParseArguments<CommandLineOptions.ProcessFileOptions, CommandLineOptions.GeneratePasswordsOptions>(args)
        .WithParsed<CommandLineOptions.GeneratePasswordsOptions>(options =>
        {
            string commandString = options.CommandString;
            int length = options.Length;
            string startingPoint = options.StartingPoint;

            List<string> generatedPasswords = helper.GeneratePassword(commandString, length, startingPoint);
        })
      .WithParsed<CommandLineOptions.ProcessFileOptions>(options => parseTasks.Add(ProcessFileOptionsAsync(options, processFile)));

        await Task.WhenAll(parseTasks);
    }

    private async Task ProcessFileOptionsAsync(CommandLineOptions.ProcessFileOptions options, ProcessPasswordFile processFile)
    {
        string inputPath = options.FolderFile;

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
}

public class CommandLineOptions
{
    [Verb("process", HelpText = "Process a file or folder.")]
    public class ProcessFileOptions
    {
        [Option('p', "folder-file", Required = true, HelpText = "File path or folder path to process.")]
        public string FolderFile { get; set; }
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
}