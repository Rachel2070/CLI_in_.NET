
using System.CommandLine;
using System.Text;

var bundleCommand = new Command("bundle", "bundle code files to a single file");//the bundle command

var languageOption = new Option<string>(new[] { "--language", "--l" }, "Programming languages") { IsRequired = true};

var outputOption = new Option<string>(new[] {"--output","--o"}, "File name");

var noteOption = new Option<bool>(new[] { "--note", "--n" }, "Note the file's original directory al name");

var sortOption = new Option<bool>(new[] { "--sort", "--s" }, "The order of file copy");

var removeEmptyLinesOption = new Option<bool>(new[] { "--remove-empty-lines", "--r" }, "Delet empty lines");

var authorOption = new Option<string>(new[] { "--author", "--a" }, "Write the author name") {IsRequired = false};
//authorOption.SetDefaultValue("");

bundleCommand.AddOption(languageOption);//add option to bundle

bundleCommand.AddOption(outputOption);

bundleCommand.AddOption(noteOption);

bundleCommand.AddOption(sortOption);

bundleCommand.AddOption(removeEmptyLinesOption);

bundleCommand.AddOption(authorOption);

var rootCommand = new RootCommand("root command for file bundle CLI");//root command

rootCommand.AddCommand(bundleCommand);//connect root and bundle

bundleCommand.SetHandler<string, string, bool, bool, bool, string>((language, output, note, sort, removeEmptyLines, author) =>
{
    try
    {
        cli.FileBundler.creatFileByLanguage(language, output, note, sort, removeEmptyLines, author);
    }
    catch (InvalidOperationException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.ExitCode = 1; 
    }
}, languageOption, outputOption, noteOption, sortOption, removeEmptyLinesOption, authorOption);


var createRspCommand = new Command("create-rsp", "Create a responsive file");

createRspCommand.SetHandler(() =>
{
    bool note, sort, removeEmptyLines;
    string author = "";
    
    Console.WriteLine("Enter respons file name:");
    string resName=Console.ReadLine();

    Console.WriteLine("Enter the programming language:");
    string language =Console.ReadLine();
    if (string.IsNullOrEmpty(language))
    {
        while (string.IsNullOrEmpty(language))
        {
            Console.WriteLine("Required - Enter the programming language:");
            language = Console.ReadLine();
        }
    }
    Console.WriteLine("Enter the output file name:");
    string output = Console.ReadLine();
    if (string.IsNullOrEmpty(output))
    {
        while (string.IsNullOrEmpty(output))
        {
            Console.WriteLine("Required - Enter the programming language:");
            output = Console.ReadLine();
        }
    }


    Console.WriteLine("Should the file's original directory name be added as a note? (Enter 'yes' or 'no':");
    string tempNote = Console.ReadLine();
    if (tempNote == "yes")
    {
         note = true;
    }
    else
    {
         note = false;
    }


    Console.WriteLine("Should the order of the copied files be sorted? (Enter 'yes' or 'no':");
    string tempSort = Console.ReadLine();
    if (tempSort == "yes")
    {
         sort = true;
    }
    else
    {
         sort = false;
    }

    Console.WriteLine("Should empty lines be removed? (Enter 'yes' or 'no':");
    string tempRemoveEmptyLines = Console.ReadLine();
    if (tempRemoveEmptyLines == "yes")
    {
         removeEmptyLines = true;
    }
    else
    {
         removeEmptyLines = false;
    }

    Console.WriteLine("Enter the author's name:");
     author = Console.ReadLine();
    if (string.IsNullOrEmpty(author)){
        author = "";
    }
    try
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), resName + ".rsp");

        // Create the file
        using (File.Create(filePath)) { }

        // Create content string
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine($"--language {language}");
        contentBuilder.AppendLine($"--output {output}");
        contentBuilder.AppendLine($"--note {note}");
        contentBuilder.AppendLine($"--sort {sort}");
        contentBuilder.AppendLine($"--remove-empty-lines {removeEmptyLines}");
        if (!string.IsNullOrEmpty(author)){
            contentBuilder.AppendLine($"--author {author}");
        }

        // Write content to the file
        File.WriteAllText(filePath, contentBuilder.ToString());
    }
    catch (System.IO.DirectoryNotFoundException)
    {
        Console.WriteLine("file directory is invalid");
    }
});

rootCommand.AddCommand(createRspCommand);

rootCommand.InvokeAsync(args);


