using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

var rootCommand = new RootCommand("Task Tracker CLI - управление задачами");

var dateOption = new Option<DateTime>("--date", () => DateTime.Today, "Дата для отображения задач");
var listCommand = new Command("list", "Показать задачи за дату");
listCommand.AddOption(dateOption);
listCommand.SetHandler(async (date) =>
{
    Console.WriteLine($"Tasks for {date:yyyy-MM-dd}: (not implemented yet)");
}, dateOption);

rootCommand.AddCommand(listCommand);

var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .Build();

await parser.InvokeAsync(args);
