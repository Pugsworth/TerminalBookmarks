// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Microsoft.CodeAnalysis;
using TerminalBookmarks;

public static class Program
{
    // TODO: Make this user configurable.
    static string GetStorageLocation()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    static void ListBookmarks(BookmarksManager manager)
    {
        var bookmarks = manager.GetAll();
        for (int i = 0; i < bookmarks.Length; i++)
        {
            Console.WriteLine($"\t {i}. {bookmarks[i]}");
        }
    }

    public static void Interactive(BookmarksManager manager)
    {
        ListBookmarks(manager);
        var bookmarks = manager.GetAll();

        Console.Write("Select bookmark to CD to, or Enter/q to cancel > ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input) || input.Trim().ToLower() == "q")
        {
            Console.WriteLine("Canceling...");
            return;
        }

        int value = int.Parse(input);
        var path = bookmarks[value];

        Console.WriteLine($"You selected #{input}, which correlates to \"{path}\"");

        // Console.Write($"pushd \"{path}\"");

        // Clipboard.SetText($"pushd \"{path}\"");

        // Environment.CurrentDirectory = path;

        Console.Write(path);
    }

    public static async Task Main(string[] args)
    {
        // Console.WriteLine("Arguments verbatim: ");
        // Console.WriteLine(string.Join(", ", args));


        var dataLocation = GetStorageLocation();
        var manager = new BookmarksManager(dataLocation);

        var rootCmd = new RootCommand("Bookmarks Manager\n\tManage paths to quickly go to favorite paths.");
        rootCmd.SetHandler(() =>
        {
            // When invoking the program without commands, we should list out the list of bookmarks and prompt to choose one.
            Interactive(manager);
        });

        var addCmd = new Command("add", "Add a path to bookmarks.");
        var addCmdPath = new Argument<string>("path", "Path to add to bookmarks.");
        addCmd.AddArgument(addCmdPath);
        addCmd.SetHandler((path) =>
        {
            if (manager.AddBookmark(path))
            {
                manager.Save();
            }
        }, addCmdPath);
        rootCmd.AddCommand(addCmd);

        var removeCmd = new Command("remove", "Remove a path from bookmarks.");
        var removeCmdPath = new Argument<string>("path", "Remove a path from the bookmarks.");
        removeCmd.AddArgument(removeCmdPath);
        removeCmd.SetHandler((path) =>
        {
            if (manager.RemoveBookmark(path))
            {
                manager.Save();
            }
        }, removeCmdPath);
        rootCmd.AddCommand(removeCmd);

        var listCmd = new Command("list", "Lists all current bookmarks.");
        listCmd.SetHandler(() =>
        {
            var bookmarks = manager.GetAll();
            for (int i = 0; i < bookmarks.Length; i++)
            {
                Console.WriteLine($"\t {i}. {bookmarks[i]}");
            }
        });
        rootCmd.AddCommand(listCmd);

        var getCmd = new Command("get", "Returns the bookmark at index for use with cd/set-location");
        var getCmdArg = new Argument<int>();
        getCmd.AddArgument(getCmdArg);
        getCmd.SetHandler((index) =>
        {
            var bookmarks = manager.GetAll();
            Console.WriteLine(bookmarks[index]);
        }, getCmdArg);
        rootCmd.AddCommand(getCmd);


        await rootCmd.InvokeAsync(args);
    }
}