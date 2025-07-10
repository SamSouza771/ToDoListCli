namespace ToDoListCli.Models;

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
