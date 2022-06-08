using System;
using System.IO.Compression;

if (args.Length == 0)
{
    WriteLine("Hello from Util CLI!");
    Write("Type {|util-cli --help} to see all the commands", ConsoleColor.Green);
}
else
{
    var parsedArgs = ParseArgs(args);

    if (parsedArgs.Help)
    {
        WriteLine("{|--zip:} Zips a file or a directory. Example: \"util-cli --zip ./folder/nestedFolder\"", ConsoleColor.Yellow);
        WriteLine("{|--delete:} Deletes a file or a directory. Example: \"util-cli --delete ./delete.txt\"", ConsoleColor.Yellow);
        WriteLine("{|--create:} Creates a file or a directory. Example: \"util-cli -create ./ \"", ConsoleColor.Yellow);
        WriteLine("{|--help:} To see all the commands", ConsoleColor.Yellow);
    }

    if (parsedArgs.Argument.Command == "--zip")
    {
        if (parsedArgs.Argument.Value == string.Empty)
        {
            WriteError("You must specify a file or directory to zip.");
        }
        else
        {
            Zip(parsedArgs.Argument.Value);
        }
    }

    if (parsedArgs.Argument.Command == "--delete")
    {
        if (parsedArgs.Argument.Value == string.Empty)
        {
            WriteError("You must specify a file or directory to delete.");
        }
        else
        {
            Delete(parsedArgs.Argument.Value);
        }
    }

    if (parsedArgs.Argument.Command == "--create")
    {
        if (parsedArgs.Argument.Value == string.Empty)
        {
            WriteError("You must specify a file or directory to create.");
        }
        else
        {
            Create(parsedArgs.Argument.Value);
        }
    }
}

void Create(string path)
{
    throw new NotImplementedException();
}

void Delete(string path)
{
    throw new NotImplementedException();
}

void Zip(string path)
{
    string fullPath = Path.GetFullPath(path);

    if (File.Exists(fullPath))
    {
        string fileName = Path.GetFileNameWithoutExtension(fullPath);

        string zipName = $"{fileName}.zip";

        using ZipArchive zip = ZipFile.Open(zipName, ZipArchiveMode.Create);
        zip.CreateEntryFromFile(fullPath, Path.GetFileName(fullPath));
        Write($"{{|New zip created with success: {zipName}}}", ConsoleColor.Green);
        return;
    }

    if (Directory.Exists(fullPath))
    {
        string currentDirectory = Path.TrimEndingDirectorySeparator(Environment.CurrentDirectory);

        if (string.Equals(fullPath, currentDirectory, StringComparison.OrdinalIgnoreCase))
        {
            WriteError("You cannot zip the current directory.");
            return;
        }

        var directory = new DirectoryInfo(fullPath)?.Name;

        string zipName = $"{directory}.zip";

        string destination = Path.Combine(currentDirectory, zipName);

        ZipFile.CreateFromDirectory(fullPath, destination);

        Write($"{{|New zip created with success: {zipName}}}", ConsoleColor.Green);
        return;
    }

    WriteError("The current file or directory does not exists.");
}

ParsedArgs ParseArgs(string[] args)
{
    var parsedArgs = new ParsedArgs();
    foreach (var arg in args)
    {
        if (arg == "--help")
        {
            if (args.Length > 1)
            {
                WriteError("To use the --help command, don’t use any other arguments.");
            }
            else
            {
                parsedArgs.Help = true;
                return parsedArgs;
            }
        }

        if (arg == "--zip")
        {
            parsedArgs.Argument.Command = "--zip";
            parsedArgs.Argument.Value = args[1];
            return parsedArgs;
        }

        if (arg == "--delete")
        {
            parsedArgs.Argument.Command = "--delete";
            parsedArgs.Argument.Value = args[1];
            return parsedArgs;
        }

        if (arg == "--create")
        {
            parsedArgs.Argument.Command = "--create";
            parsedArgs.Argument.Value = args[1];
            return parsedArgs;
        }

        WriteError($"Error. Util CLI could not parse the argument {arg}.");
    }

    return parsedArgs;
}

void WriteError(string message)
{
    WriteLine($"{{|{message}}}", ConsoleColor.Red);
}

void WriteLine(string message, ConsoleColor color = ConsoleColor.White) => Write(message, color, true);

void Write(string message, ConsoleColor color = ConsoleColor.White, bool line = false)
{
    var split = message.Split('{', '}');
    foreach (var splitMessage in split)
    {
        if (splitMessage == string.Empty) continue;

        string printMessage = splitMessage;
        var printColor = ConsoleColor.White;

        if (splitMessage.Contains('|'))
        {
            printMessage = splitMessage.Replace("|", "");
            printColor = color;
        }

        Print(printMessage, printColor);
    }

    if (line)
    {
        Console.WriteLine();
    }
}

void Print(string message, ConsoleColor color = ConsoleColor.White)
{
    Console.ForegroundColor = color;
    Console.Write(message);
    Console.ForegroundColor = ConsoleColor.White;
}

record ParsedArgs
{
    public Argument Argument { get; set; } = new Argument();
    public bool Help { get; set; }
}

record Argument
{
    public string Command { get; set; } = "";
    public string Value { get; set; } = "";
}

