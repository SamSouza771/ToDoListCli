using Spectre.Console;
using System.Text.Json;
using System.IO; 

namespace ToDoListCli;

class Program
{
    static void Main(string[] args)
    {
        // Verifica se a pasta e o arquivo existem e cria caso não exista
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

        List<tarefa> tarefas = new();
        try{
        string conteudo = File.ReadAllText(pathtsk);        
        //Console.Write(conteudo);
        tarefas = JsonSerializer.Deserialize<List<tarefa>>(conteudo);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }

        Menu(tarefas, pathtsk);
    }
    static void Menu(List<tarefa> tarefas, string path){
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choice:")
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑ ↓ para navegar)[/]")
                .AddChoices(new[]{
                    "Add Task", "List Tasks", "Check Tasks","Delete Tasks", "Exit"
                })
        );

        switch (choice){
            case "Add Task":
                AddTask(tarefas, path);
            break;
            case "List Tasks":
                ListTasks(tarefas, path);
            break;
            case "Check Tasks":
                var chkselected = SelectTasks(tarefas);
                CheckTasks(chkselected, tarefas, path);
            break;
            case "Delete Tasks":
                var delselected = SelectTasks(tarefas);
                DeleteTasks(delselected, tarefas, path);
            break;
            case "Exit":
                Environment.Exit(0);
            break;
        }
    }
    
    static void AddTask(List<tarefa> tarefas, string path) {
        string titulo = AnsiConsole.Ask<string>("Type the task title: ");
        int novoId = tarefas.Any() ? tarefas.Max(t => t.Id) + 1 : 1;

        tarefa novaTarefa = new() {
            Id = novoId,
            Name = titulo,
            Status = false
        };

        tarefas.Add(novaTarefa);
        SaveTask(tarefas, path);
        ListTasks(tarefas, path);
    }
    
    static void SaveTask(List<tarefa> tarefas, string path){
        string json = JsonSerializer.Serialize(tarefas, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(path, json);
    }

    static void ListTasks(List<tarefa> tarefas, string path){
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Tarefa");
        table.AddColumn("Status");

        foreach (var t in tarefas){
            string status = t.Status ? "[green]Feita[/]" : "[red]Pendente[/]";
            table.AddRow(t.Id.ToString(), t.Name, status);
        }
        Console.Clear();
        AnsiConsole.Write(table);
        SaveTask(tarefas, path);
        Menu(tarefas, path);
    }

    static List<string> SelectTasks(List<tarefa> tarefas){
        var selected = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Quais tarefas deseja marcar como feito? ")
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑ ↓ para navegar)[/]")
                .InstructionsText("[grey](Pressione Espaço para marcar e Enter para confirmar)[/]")
                .AddChoices(tarefas.Select(t => $"{t.Id} - {t.Name}"))
        );

        return selected;
    }

    static void CheckTasks(List<string> selected, List<tarefa> tarefas, string path){
        foreach(var item in selected){
            var partes = item.Split(" - ");
            int id = int.Parse(partes[0]);

            var tarefaEncontrada =tarefas.Find(t => t.Id == id);
            tarefaEncontrada.Status = true;
        }
        ListTasks(tarefas, path);
    }

    static void DeleteTasks(List<string> selected, List<tarefa> tarefas, string path){
        foreach(var item in selected){
            var partes = item.Split(" - ");
            int id = int.Parse(partes[0]);

            var tarefaEncontrada =tarefas.Find(t => t.Id == id);
            tarefas.Remove(tarefaEncontrada);
        }
        ListTasks(tarefas, path);
    }
    
}
