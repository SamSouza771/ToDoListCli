using ToDoListCli.Helpers;
using ToDoListCli.Services;
using ToDoListCli.Ui;

namespace ToDoListCli;

class Program
{
    static void Main(string[] args)
    {
        var path = FileHelper.EnsureStoragePath();
        var service = new TaskService(path);
        var ui = new TaskUi(service);
        ui.Run();
    }
}
