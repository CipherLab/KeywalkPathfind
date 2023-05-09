# KeyWalk Password Analyzer

KeyWalk Password Analyzer is a password analysis tool that uses A* pathfinding algorithm to analyze passwords. It treats the keyboard as a grid, and the next character in the password string as the target. The program navigates through the "Enter Password" field, generating a path as the keywalk. This readme.md file provides an overview of the program and its usage.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

## Installation

Clone the GitHub repository to your local machine:

```
git clone https://github.com/yourusername/KeyWalk-Password-Analyzer.git
```

Open the project in your preferred C# IDE and build the solution.

## Usage

The main entry point of the program is `Program.cs`. The program accepts command line arguments to process a file containing passwords or generate new passwords based on a given command string, length, and starting point.

There are two main command line options:

1. `process`: Process a file or folder containing password files.
   - `-p`, `--folder-file`: File path or folder path to process. (Required)
2. `generate`: Generate passwords based on a command string, length, and starting point.
   - `-c`, `--command`: Command string to use for password generation. (Required)
   - `-l`, `--length`: Length of the passwords to generate. (Required)
   - `-s`, `--starting-point`: String of characters to use as starting points. (Required)

## Examples

To process a file or folder containing passwords, use the `process` command:

```
dotnet run -- process -p /path/to/passwords.txt
```

To generate new passwords based on a command string, length, and starting point, use the `generate` command:

```
dotnet run -- generate -c "►◄" -l 6 -s "h"
```

The example output for the generated paths to create new passwords is as follows:

```
	◘	hhhhhh
	►	hjkl;'
	►◄	hjhjhj
	◄	hgfdsa
	◄►	hghghg
	►►←◄	hjkhjk
	→→►←←◄	hlhlhl
	←◄→►	hfhfhf
	▲▼	hyhyhy
	→►←◄	hkhkhk
```

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes. Make sure to provide a clear description of your changes and update any relevant documentation.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more information.
