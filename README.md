<div align="center">

# DINFO (Directory Info)

**Dinfo** is a C# tool that provides detailed information about a directory.
It uses **Spectre.Console** to create a clean and colorful TUI, showing files, lines, size, permissions, and file type statistics.

</div>

## 🚀 Features

- Counts the number of **files** and **directories**.
- Counts the number of **lines of code** in files.
- Shows the **total size** of the directory.
- Displays **permissions** of the directory.
- Lists all **file extensions** and shows the **most used extension**.
- Supports **recursive** processing of subdirectories (`-r`).
- **Verbose mode** (`-v`) for more detailed information.
- Elegant text-based interface with borders, tables, and colors via **Spectre.Console**.

---

## 💻 Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/SCSDC-co/DINFO.git
    ```

2. Build with .NET:

    ```bash
    dotnet build # you need to be in src/tui for running this command
    ```

3. Run the tool:

    ```bash
    dotnet run -- [options] [directory] # same for this one
    ```

---

## ❓ Help Page

### 📖 Usage

dotnet dinfo.tui.dll <targetdirectory> [options]

### 📝 Description

Display information about the specified directory and its contents.

### 🔑 Parameters

targetdirectory The Directory to be analyzed. Default: Current directory.

### ⚙️ Options

-r|--recursive Recursively list all files and directories. Default: "False".
-v|--verbose Enable verbose output. Default: "False".
-i|--ignore-gitignore Ignore .gitignore files. Default: "False".
-h|--help Shows help text.
--version Shows version information.

**Example:**

```bash
# Count lines in the current directory
dinfo

# Count lines recursively in all subdirectories
dinfo -r C:\Projects\MyFolder

# Count lines with verbose output
dinfo -v C:\Projects\MyFolder
```

---

## 🖼️ Example Output (verbose)

![Example Output](.github/assets/example-output.png)

---

## ⚡ Technologies

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Spectre.Console](https://spectreconsole.net/) - for TUI and colored output
- [CliFx](https://github.com/Tyrrrz/CliFx) - for command-line parsing

---

## 📝 Notes

- Dinfo is designed to be **lightweight, fast, and simple**.
- Ideal for developers who want to **count lines of code** or get a quick overview of a directory.

---

## 🤝 Contributing

Contributions are welcome! Open a **pull request** or create an **issue** for suggestions or bug reports.

---

## 📜 License

This project is licensed under the **GNU GPL v3.0**. See the [LICENSE](LICENSE) file for details.
