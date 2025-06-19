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
        //Console.Write(path);
        var pathfdr = Path.Combine(path, "ToDoCli");
        var pathtsk = Path.Combine(pathfdr, "task.json");    
        if (!Directory.Exists(pathfdr)){
            //Console.Write("passei");
            Directory.CreateDirectory(pathfdr);
        }
        if (!File.Exists(pathtsk)){
            //Console.Write("passei");
            using (StreamWriter swriter = File.CreateText(pathtsk))
            {
                swriter.WriteLine("[]"); 
            }
        }

        

        
    }
}
