// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;
using KeyWalkAnalyzer2;

KeyboardGrid keyboard = new KeyboardGrid(4, 13);
keyboard.SetQwertyLayout();
PathFinder pathFinder = new PathFinder(keyboard.Grid);

//Console.WriteLine("Enter the input string:");
//string inputString = Console.ReadLine();
string inputString = "qwertYUIOP";
inputString = "qetuo[";
inputString = "123!@#qweQWE";

Console.WriteLine($"\nInput String: {inputString}");



List<PathElement> completePath = new List<PathElement>();

for (int i = 0; i < inputString.Length - 1; i++)
{
    char startChar = inputString[i];
    char endChar = inputString[i + 1];

    char nonShiftStart = startChar;
    char nonShiftEnd = endChar;
    if (startChar.IsShiftedSymbol())
    {
        nonShiftStart = startChar.ConvertToNonShift();
    }
    if (endChar.IsShiftedSymbol())
    {
        nonShiftEnd = endChar.ConvertToNonShift();
    }


    List<Tuple<int, int>> path = pathFinder.AStarPathFinding(nonShiftStart, nonShiftEnd);
    if (path != null)
    {
        List<PathElement> pathElements = keyboard.GeneratePathElements(path, startChar, endChar);
        completePath.AddRange(pathElements); // Add elements to the complete path

    }
    else
    {
        Console.WriteLine($"No path found between {startChar} and {endChar}");
    }
}

Console.WriteLine($"Path To String: {Helpers.PathToString(completePath)}");


foreach (PathElement pathElement in completePath)
{
    Console.WriteLine($"Direction: {pathElement.Direction}, Steps: {pathElement.Steps}, Select: {pathElement.Select}, ShiftDown: {pathElement.ShiftDown}");
}

List<PathElement> reducedPath = Helpers.ReducePath(completePath);

Console.WriteLine("Reduced Path:");
foreach (PathElement pathElement in reducedPath)
{
    Console.WriteLine($"Direction: {pathElement.Direction}, Steps: {pathElement.Steps}, Select: {pathElement.Select}, ShiftDown: {pathElement.ShiftDown}");
}


string outputString = Helpers.ExecuteReducedPath(inputString[0], reducedPath, keyboard);
Console.WriteLine($"Output String: {outputString}");

