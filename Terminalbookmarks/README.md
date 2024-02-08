# About
This C# .Net CLI tool is meant to provide a way to save and restore paths that you might frequent.


# Goals
If you work in a terminal, then you don't *really* have an easy way to "favorite" different paths.
I wanted way to manage a list of paths that I might want to hold on to, in the lowest friction possible.
After I had already started it, I found a couple other projects that does a similar thing. After checking them,
I really just wanted to solve this issue in the way that I felt was right.  

It's a simple json file in the user's directory that the program will save and load. You can move this file
and it should just work as long as the program is pointed to the new location. This makes it rather portable.
(ignore the scripts needed to get it to work :V)


# Installation

Because processes cannot change the working directory of the parent process, there needs to be a little bit of
trickery to get it working well.

1. The program and scripts needs to downloaded into a folder.
2. Setup for different shells:
    - Powershell  
      The user's PowerShell profile script needs to be modified:
        1. Find and edit the profile script.  
           This can be found using `echo $profile`
        2. Add the following at the end of the file.
            ```powershell
            $termmarks_path = "<the path you used>"
            $env:Path = $env:Path+";$termmark_path"
            Import-Module "$termmark_path/ps-termmark.psm1"
            ```   
        3. The `ps-termmark` module won't work unless you unblock the file.  
           This can be done using `Unblock_file <path to ps-termmark.psm1>`

    - Bash
        - TBD

    - CMD
        - TBD

3. Finally, reload your terminal and begin using the commands!

---

# Roadmap

## Config
If a [config](#config-format) exists in the same folder as the executable, you can change a couple things about how it works.
- bookmark file location.
- 

## Config Format
TDB

## Commands

| Done?   | Command | Arguments   | Description                                                 |
|---------|---------|-------------|-------------------------------------------------------------|
| &cross; | add     | \[path] | Add a new bookmark to the list. Uses PWD if missing.        |
| &cross; | list    | None        | List out all of the bookmarks.                              |
| &cross; | remove  | \[path] | Removes a path from the bookmark list. Uses PWD if missing. |
| &cross; | info    | None        | Various info, including the path to the bookmarks file.     |
| &cross; | get     | \<id\>      | The path at index \<id\>.                                   |
| &cross; | purge   | None        | Purges the bookmarks list. Asks for confirmation.           |


# Attributions/Credits
## [NeilMacMullen/jumpfs](https://github.com/NeilMacMullen/jumpfs/tree/main)
- Big thanks to [NeilMacMullen](https://github.com/NeilMacMullen) for their similar project!  
  I discovered that you can indeed cause your terminal to change directories using scripts. These scripts will execute
  the program and pass the return values to the appropriate shell commands. This is necessary because it's impossible
  to actually change the parent's working directory from a child process. Once that child process dies, the
  working directory is restored.