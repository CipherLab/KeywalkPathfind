using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace KeyWalkAnalyzer3;

class Program
{
    [Verb("process", HelpText = "Process a file or folder containing passwords")]
    public class ProcessOptions
    {
        [Option('p', "folder-file", Required = true,
            HelpText = "File path or folder path to process")]
        public string FolderFile { get; set; }
    }
    [Verb("generate", HelpText = "Generate passwords based on a command string")]
    public class GenerateOptions
    {
        [Option('c', "command", Required = true,
            HelpText = "Command string to use for password generation")]
        public string Command { get; set; }

        [Option('l', "length", Required = false,
            HelpText = "Length of the passwords to generate")]
        public int? Length { get; set; }

        [Option('s', "starting-point", Required = true,
            HelpText = "String of characters to use as starting points")]
        public string StartingPoint { get; set; }
    }
    static async Task<int> Main(string[] args)
    {
        if (IsDebug())
        {
            // Default parameters for debugging
            //args = new string[] { "process", "--folder-file", "path/to/default/folder" };
            // Or for generate command
            /*
                ◘	hhhhhh
                ►	hjkl;'
                ►◄	hjhjhj
                ◄	hgfdsa
                ◄►	hghghg
                ►►←◄	hjkh
                →→►←←◄	hlhlhl
                ←◄→►	hfhfhf
                ▲▼	hyhyhy
                →►←◄	hkhkhk
            */
            args = new string[] { "generate", "--command", "→►", "--length", "6", "--starting-point", "s" };
        }

        return await Parser.Default.ParseArguments<ProcessOptions, GenerateOptions>(args)
            .MapResult(
                (ProcessOptions opts) => ProcessPasswords(opts.FolderFile),
                (GenerateOptions opts) => GeneratePasswords(opts.Command, opts.Length, opts.StartingPoint),
                errs => Task.FromResult(1));
    }

    static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    static Task<int> ProcessPasswords(string folderFile)
    {
        var pathAnalyzer = new PathAnalyzer();
        var passwordAnalyzer = new PasswordAnalyzer(new KeyboardLayout(), pathAnalyzer);

        try
        {
            if (Directory.Exists(folderFile))
            {
                var passwordFiles = Directory.GetFiles(folderFile, "*.txt");
                foreach (var file in passwordFiles)
                {
                    ProcessPasswordFile(file, passwordAnalyzer);
                }
            }
            else if (File.Exists(folderFile))
            {
                ProcessPasswordFile(folderFile, passwordAnalyzer);
            }
            else
            {
                Console.Error.WriteLine($"Path not found: {folderFile}");
                return Task.FromResult(1);
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return Task.FromResult(1);
        }
    }

    static Task<int> GeneratePasswords(string command, int? length, string startingPoints)
    {
        try
        {
            var keyboard = new KeyboardLayout();
            var pathAnalyzer = new PathAnalyzer();
            var passwordAnalyzer = new PasswordAnalyzer(keyboard, pathAnalyzer);

            Console.WriteLine($"Generating passwords with command: {command}, target length: {length ?? command.Length}");

            var passwords = passwordAnalyzer.GeneratePasswords(command, startingPoints, length);

            foreach (var password in passwords)
            {
                Console.WriteLine(password);
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return Task.FromResult(1);
        }
    }
    static void ProcessPasswordFile(string filePath, PasswordAnalyzer passwordAnalyzer)
    {
        try
        {
            var passwords = File.ReadAllLines(filePath);
            Console.WriteLine($"Processing file: {filePath}");
            foreach (var password in passwords)
            {
                 passwordAnalyzer.AnalyzePassword(password);
                var pwd = passwordAnalyzer.GetSmallestPath();
                Console.WriteLine("Pwd:"+pwd);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing file {filePath}: {ex.Message}");
        }
    }
}