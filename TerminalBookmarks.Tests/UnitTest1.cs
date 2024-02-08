namespace TerminalBookmarks.Tests;

public class UnitTest1
{
    private BookmarksManager manager;
    public UnitTest1()
    {
        var path = Environment.CurrentDirectory;
        manager = new(path);
    }
    
    [Fact]
    public void TestEmpty()
    {
        var all = manager.GetAll();
        Assert.Empty(all);
    }

    [Fact]
    public void TestAddition()
    {
        manager.AddBookmark("/test");
        var all = manager.GetAll();
        Assert.Equal(all, ["/test"]);
    }
}