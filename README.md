<div align="center">

# DINFO (Directory Info)

**DINFO** is a simple C# console application that provides an overview of files in a directory.
Currently, it can count the number of lines in one or more files passed as arguments. Future updates will add more detailed file and directory information.

</div>

## Features

- Count lines in one or more files.
- Supports multiple file paths as command-line arguments.
- Lightweight and easy to use.

## Usage

1. Clone the repository or download the executable.
2. Open a terminal and navigate to the folder containing the program.
3. Run the program with one or more file paths as arguments:

```bash
dotnet run Program.cs AnotherFile.txt
```

Or, if you have the compiled executable:

```bash
Dinfo Program.cs AnotherFile.txt
```

The program will output the number of lines in each file. For example:

```bash
Lines of Program.cs: 21
```

## Requirements

- .NET 6.0 SDK or later

## Roadmap

Here’s what is planned for **Dinfo**:

- [ ] **Detect file types**  
       Identify the type of each file (e.g., C#, Python, etc.) to give better insight into the directory content.

- [x] **Scan entire directories**  
       Recursively scan an entire directory and display the line count for all files.

- [ ] **File statistics overview**  
       Show the total number of files in a directory and determine the most used programming language.

- [ ] **TUI (Text User Interface)**  
       Create a visually appealing and interactive terminal interface for easier navigation and data visualization.

## License

This project is licensed under the GPL-3.0 License.
