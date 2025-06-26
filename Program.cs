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
        string conteudo = File.ReadAllText(pathtsk);       
        try{
        
        //Console.Write(conteudo);
        tarefas = JsonSerializer.Deserialize<List<tarefa>>(conteudo);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }

        AddTask(tarefas, pathtsk);
        Console.Write(conteudo);
    }
    static void AddTask(List<tarefa> tarefas, string path) {
         
        string titulo = AnsiConsole.Ask<string>("Digite o titúlo: ");
        int novoId = tarefas.Any() ? tarefas.Max(t => t.Id) + 1 : 1;

        tarefa novaTarefa = new() {
            Id = novoId,
            Name = titulo,
            Status = false
        };

        tarefas.Add(novaTarefa);

        string json = JsonSerializer.Serialize(tarefas, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(path, json);
    }
}
