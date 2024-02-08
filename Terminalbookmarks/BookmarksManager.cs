using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TerminalBookmarks;

using BookmarkList = List<string>;

/**
 * Manage the storage and retrieval of the bookmarks for the user.
 */
public class BookmarksManager
{
    /// <summary>
    /// Manages terminal bookmarks via a .term-bookmarks file.
    /// </summary>
    /// <param name="storageLocation">The location to read and write the .term-bookmarks file.</param>
    public BookmarksManager(string storageLocation)
    {
        if (!Directory.Exists(storageLocation))
        {
            throw new DirectoryNotFoundException($"Directory '{storageLocation}' doesn't exist!");
        }

        dataLoc = Path.Join(storageLocation, ".term-bookmarks");
        if (File.Exists(dataLoc))
        {
            BookmarkList data = ReadFromDisk(dataLoc)
                .ContinueWith(task =>
                {
                    try
                    {
                        return Deserialize(task.Result);
                    }
                    catch (Exception e)
                    {
                        // Something went wrong with reading the data. For now, ignore and move on.
                        Console.WriteLine(e);
                        // throw;
                        return [];
                    }
                }).Result;

            bookmarks = data;
        }
        else
        {
            // If the file doesn't exist, then it's dirty and needs to be written to disk (even if empty).
            dirty = true;
        }
    }

    /// <summary>
    /// Get an Array of all bookmarks, ordered by some metric (latest, most used, etc?)
    /// </summary>
    /// <returns>Copy of list of bookmarks as array.</returns>
    public string[] GetAll()
    {
        return bookmarks.ToArray();
    }

    /// <summary>
    /// Add a new bookmark to the manager to be saved.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>True if added, False if already exists.</returns>
    public bool AddBookmark(string item)
    {
        if (bookmarks.Exists(o => o.Equals(item)))
        {
            return false;
        }

        bookmarks.Add(item);
        dirty = true;

        return true;
    }

    // Remove an existing bookmark, if it exists.
    public bool RemoveBookmark(string item)
    {
        var numRemoved = bookmarks.RemoveAll(str => str.Equals(item));
        if (numRemoved > 0) dirty = true;
        return numRemoved > 0;
    }

    /// <summary>
    /// Clears the stored bookmarks.
    /// Doesn't mark dirty and doesn't clear the file.
    /// </summary>
    /// <returns>If successful</returns>
    public bool ClearBookmarks()
    {
        bookmarks.Clear();
        return true;
    }


    //**********************************************
    // Save/Restore
    //**********************************************

    /// <summary>
    /// Saves the current bookmarks to file.
    /// </summary>
    public async void Save()
    {
        var data = Serialize();
        var token = new CancellationToken();
        await WriteToDisk(dataLoc, data, token);
    }

    /// <summary>
    /// Discards the current bookmarks and loads from file.
    /// </summary>
    /// <exception cref="FileNotFoundException">If the file cannot be found, then cancels and throws an exception.</exception>
    public async void Restore()
    {
        bookmarks.Clear();
        var data = await ReadFromDisk(dataLoc);
        bookmarks = Deserialize(data);
    }


    //**********************************************
    // Private
    //**********************************************

    private BookmarkList bookmarks = new List<string>();

    private string dataLoc;

    // Is the bookmarks list out of sync with the disk.
    private bool dirty = false;


    // Serialize the list of bookmarks to be written to disk.
    private string Serialize()
    {
        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
        };

        var outStr = JsonSerializer.Serialize(new
        {
            bookmarks = bookmarks.ToArray()
        }, options);

        return outStr;
    }

    // Deserialize into a list of bookmarks.
    private BookmarkList Deserialize(byte[] data)
    {
        var doc = JsonDocument.Parse(data);
        var bookmarksProp = doc.RootElement.GetProperty("bookmarks");

        BookmarkList bookmarks = new();

        foreach (var element in bookmarksProp.EnumerateArray())
        {
            string item = element.ToString();
            bookmarks.Add(item);
        }

        return bookmarks;
    }

    async Task<bool> WriteToDisk(string path, string data, CancellationToken cancelToken)
    {
        if (!dirty) return true;

        var task = File.WriteAllTextAsync(path, data, cancelToken);
        await task.WaitAsync(cancelToken);

        if (task.IsCompleted)
        {
            return new Boolean();
        }

        return false;
    }

    async Task<byte[]> ReadFromDisk(string path)
    {
        return await File.ReadAllBytesAsync(path);
    }
}