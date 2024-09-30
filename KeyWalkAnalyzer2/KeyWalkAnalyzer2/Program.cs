// See https://aka.ms/new-console-template for more information
using System.Text;
using KeyWalkAnalyzer2;

KeyboardGrid keyboard = new KeyboardGrid(4, 13);
keyboard.SetQwertyLayout();
PathFinder pathFinder = new PathFinder(keyboard.Grid);

//Console.WriteLine("Enter the input string:");
//string inputString = Console.ReadLine();
string inputString = "qwertYUIOP";

Console.WriteLine($"\nInput String: {inputString}");



List<PathElement> completePath = new List<PathElement>();

for (int i = 0; i < inputString.Length - 1; i++)
{
    char startChar = inputString[i];
    char endChar = inputString[i + 1];

    List<Tuple<int, int>> path = pathFinder.AStarPathFinding(startChar.ConvertToNonShift(), endChar.ConvertToNonShift());
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



foreach (PathElement pathElement in completePath)
{
    Console.WriteLine($"Direction: {pathElement.Direction}, Steps: {pathElement.Steps}, Select: {pathElement.Select}, ShiftDown: {pathElement.ShiftDown}");
}

List<PathElement> reducedPath = ReducePath(completePath);

Console.WriteLine("Reduced Path:");
foreach (PathElement pathElement in reducedPath)
{
    Console.WriteLine($"Direction: {pathElement.Direction}, Steps: {pathElement.Steps}, Select: {pathElement.Select}, ShiftDown: {pathElement.ShiftDown}");
}


string outputString = ExecuteReducedPath('q', reducedPath, keyboard);
Console.WriteLine($"Output String: {outputString}");

List<PathElement> ReducePath(List<PathElement> path)
{
    List<PathElement> reducedPath = new List<PathElement>();

    if (path.Count == 0)
    {
        return reducedPath;
    }

    PathElement currentElement = path[0];
    int currentCount = 1;

    for (int i = 1; i < path.Count; i++)
    {
        if (path[i].Direction == currentElement.Direction &&
            path[i].Select == currentElement.Select &&
            path[i].ShiftDown == currentElement.ShiftDown)
        {
            currentCount++;
        }
        else
        {
            currentElement.Steps = currentCount;
            reducedPath.Add(currentElement);
            currentElement = path[i];
            currentCount = 1;
        }
    }

    currentElement.Steps = currentCount;
    reducedPath.Add(currentElement);

    return reducedPath;
}
string ExecuteReducedPath(char startingChar, List<PathElement> reducedPath, KeyboardGrid keyboard)
{
    StringBuilder outputString = new StringBuilder();
    outputString.Append(startingChar); // Add the starting character

    Tuple<int, int> currentPosition = keyboard.FindCharPosition(startingChar.ConvertToNonShift());
    bool shiftIsDown = startingChar.IsShiftedSymbol(); // Track Shift key state

    foreach (PathElement element in reducedPath)
    {
        for (int i = 0; i < element.Steps; i++)
        {
            // Move based on direction
            switch (element.Direction)
            {
                case Direction.Up:
                    currentPosition = Tuple.Create(currentPosition.Item1 - 1, currentPosition.Item2);
                    break;
                case Direction.Down:
                    currentPosition = Tuple.Create(currentPosition.Item1 + 1, currentPosition.Item2);
                    break;
                case Direction.Left:
                    currentPosition = Tuple.Create(currentPosition.Item1, currentPosition.Item2 - 1);
                    break;
                case Direction.Right:
                    currentPosition = Tuple.Create(currentPosition.Item1, currentPosition.Item2 + 1);
                    break;
            }

            // Handle Shift key down
            if (element.ShiftDown && !shiftIsDown)
            {
                shiftIsDown = true;
            }

            // Select character if needed
            if (element.Select)
            {
                char selectedChar = keyboard.Grid[currentPosition.Item1, currentPosition.Item2];
                if (shiftIsDown)
                {
                    // Convert to shifted character if Shift is down
                    // (assuming your keyboard grid handles shifted characters)
                    // You might need a separate method to get the shifted character
                    // based on your keyboard grid implementation.
                    // For now, I'll just assume you have a way to do this.
                    selectedChar = selectedChar.ConvertToShift();
                }
                outputString.Append(selectedChar);

                // Handle Shift key up (if needed)
                // Only release Shift if the next character is not shifted
                if (shiftIsDown && !element.ShiftDown) // Assuming ShiftDown in the element indicates whether the NEXT character needs Shift
                {
                    shiftIsDown = false;
                }
            }
        }
    }

    return outputString.ToString();
}

