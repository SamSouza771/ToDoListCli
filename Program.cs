using Spectre.Console;
using System.Text.Json; 

namespace ToDoListCli;

class Program
{
    static void Main(string[] args)
    {
        // Check and create folder/file if needed
        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var pathfdr = Path.Combine(path, "ToDoCli");
        var pathtsk = Path.Combine(pathfdr, "task.json");    

        if (!Directory.Exists(pathfdr)){
            Directory.CreateDirectory(pathfdr);
        }

        if (!File.Exists(pathtsk)){
            using (StreamWriter swriter = File.CreateText(pathtsk))
            {
                swriter.WriteLine("[]"); 
            }
        }

        List<TaskItem> tasks = new();
        try{
            string content = File.ReadAllText(pathtsk);        
            tasks = JsonSerializer.Deserialize<List<TaskItem>>(content);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }

        Menu(tasks, pathtsk);
    }

    static void Menu(List<TaskItem> tasks, string path){
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choice:")
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑ ↓ to navigate)[/]")
                .AddChoices(new[]{
                    "Add Task", "List Tasks", "Check Tasks", "Delete Tasks", "Exit"
                })
        );

        switch (choice){
            case "Add Task":
                AddTask(tasks, path);
                break;
            case "List Tasks":
                ListTasks(tasks, path);
                break;
            case "Check Tasks":
                var checkSelected = SelectTasks(tasks);
                CheckTasks(checkSelected, tasks, path);
                break;
            case "Delete Tasks":
                var deleteSelected = SelectTasks(tasks);
                DeleteTasks(deleteSelected, tasks, path);
                break;
            case "Exit":
                Environment.Exit(0);
                break;
        }
    }

    static void AddTask(List<TaskItem> tasks, string path) {
        string title = AnsiConsole.Ask<string>("Type the task title: ");
        int newId = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
        var priority = AnsiConsole.Prompt(
            new SelectionPrompt<Preference>()
                .Title("Choice the priority for that task:")
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑ ↓ to navigate)[/]")
                .AddChoices(Enum.GetValues(typeof(Preference))
                    .Cast<Preference>()
                    .ToList()
                )
        );
        
        TaskItem newTask = new() {
            Id = newId,
            Name = title,
            Priority = priority,
            Status = false
        };

        tasks.Add(newTask);
        SaveTask(tasks, path);
        ListTasks(tasks, path);
    }

    static void SaveTask(List<TaskItem> tasks, string path){
        string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(path, json);
    }

    static void ListTasks(List<TaskItem> tasks, string path){
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Task");
        table.AddColumn("Priority");
        table.AddColumn("Status");

        foreach (var t in tasks){
            string status = t.Status ? "[green]Done[/]" : "[red]Pending[/]";
            table.AddRow(t.Id.ToString(), t.Name, t.Priority.ToString() , status);
        }
        Console.Clear();
        AnsiConsole.Write(table);
        Menu(tasks, path);
    }

    static List<string> SelectTasks(List<TaskItem> tasks){
        var selected = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Which tasks do you want to mark as done? ")
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑ ↓ to navigate)[/]")
                .InstructionsText("[grey](Press Space to select and Enter to confirm)[/]")
                .AddChoices(tasks.Select(t => $"{t.Id} - {t.Name}"))
        );

        return selected;
    }

    static void CheckTasks(List<string> selected, List<TaskItem> tasks, string path){
        foreach(var item in selected){
            var parts = item.Split(" - ");
            int id = int.Parse(parts[0]);

            var foundTask = tasks.Find(t => t.Id == id);
            if (foundTask != null)
                foundTask.Status = true;
        }
        ListTasks(tasks, path);
    }

    static void DeleteTasks(List<string> selected, List<TaskItem> tasks, string path){
        foreach(var item in selected){
            var parts = item.Split(" - ");
            int id = int.Parse(parts[0]);

            var foundTask = tasks.Find(t => t.Id == id);
            if (foundTask != null)
                tasks.Remove(foundTask);
        }
        ListTasks(tasks, path);
    }
}

public class TaskItem{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Status { get; set; }
    public Preference Priority {get; set;}
}
public enum Preference{
    high = 3,
    mid = 2,
    low = 1
}

