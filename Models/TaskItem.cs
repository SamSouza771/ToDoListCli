namespace ToDoListCli.Models;

public class TaskItem{
    public int Id { get; }
    public string Name { get; private set; }
    public bool Status { get; private set; }
    public Preference Priority {get; private set;}

    public TaskItem(int id, string name, Preference priority){
        Id = id;
        Name = name;
        Priority = priority;
        Status = false; 
    }

    public void MarkAsDone() => Status = true;
}
public enum Preference{
    high = 3,
    mid = 2,
    low = 1
}
