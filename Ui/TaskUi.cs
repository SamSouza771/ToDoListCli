using Spectre.Console;
using ToDoListCli.Models;
using ToDoListCli.Services;

namespace ToDoListCli.Ui;

public class TaskUi {
    private readonly TaskService service;
    public TaskUi(TaskService _service){
        service = _service;
    }
    
    public void Run(){
        while(true){
            Console.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choice:")
                    .AddChoices(new[]{
                        "Add Task", "List Tasks",
                        "Check Tasks", "Delete Tasks", "Exit"
                    })
            );

            switch (choice){
                case "Add Task":
                    AddTask();
                    break;
                case "List Tasks":
                    ListTasks();
                    break;
                case "Check Tasks":
                    var check = SelectTasks();
                    foreach (var id in check)
                        service.MarkAsDone(id);
                    break;
                case "Delete Tasks":
                    var del = SelectTasks();
                    foreach (var id in del)
                        service.DeleteById(id);
                    break;
                case "Exit":
                    Environment.Exit(0);
                    break;
            }
        }
	}
	private void AddTask()
    {
        var title = AnsiConsole.Ask<string>("Task title:").Trim(' ', '[', ']');
        var priority = AnsiConsole.Prompt(
            new SelectionPrompt<Preference>()
                .Title("Priority:")
                .AddChoices(Enum.GetValues(typeof(Preference))
                    .Cast<Preference>()
                )
        );

        var id = service.Tasks.Any() ? service.Tasks.Max(t => t.Id) + 1 : 1;
        service.Add(new TaskItem { Id = id,
            Name = title,
            Priority = priority,
            Status = false });
    }

    private void ListTasks()
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Task");
        table.AddColumn("Priority");
        table.AddColumn("Status");

        foreach (var t in service.Tasks)
        {
            table.AddRow(
                t.Id.ToString(),
                t.Name,
                t.Priority.ToString(),
                t.Status ? "[green]Done[/]" : "[red]Pending[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("[grey]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private List<int> SelectTasks()
    {
        var selected = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select tasks:")
                .AddChoices(service.Tasks.Select(t => $"{t.Id} - {t.Name}"))
        );

        return selected
            .Select(s => int.Parse(s.Split(" - ")[0]))
            .ToList();
    }

    
}
