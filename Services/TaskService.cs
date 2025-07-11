using System.Text.Json;
using ToDoListCli.Models;

namespace ToDoListCli.Services;

public class TaskService
{
    private readonly string filePath;
    public List<TaskItem> Tasks { get; private set; }

    public TaskService(string filePath)
    {
        this.filePath = filePath;

        var json = File.ReadAllText(filePath);
        Tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
    }

    public void Add(TaskItem task)
    {
        Tasks.Add(task);
        Save();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void DeleteById(int id)
    {
        var task = Tasks.Find(t => t.Id == id);
        if (task != null)
        {
            Tasks.Remove(task);
            Save();
        }
    }

    public void MarkAsDone(int id)
    {
        var task = Tasks.Find(t => t.Id == id);
        if (task != null)
        {
            task.MarkAsDone();
            Save();
        }
    }
}
