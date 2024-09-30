namespace TestProject2;
    using Xunit;

    public class HelpersTests
    {
        [Fact]
        public void SimplifyPassword_ShouldSimplifyCorrectly()
        {
            // Arrange
            var helpers = new Helpers();
            string input = "P@ssw0rd!";
            string expected = "p2ssw0rd1";

            // Act
            string simplified = helpers.SimplifyPassword(input);

            // Assert
            Assert.Equal(expected, simplified);
        }
    }

public class PathFinderTests
{
    [Fact]
    public void FindPath_ShouldReturnCorrectCommands_ForSimplePassword()
    {
        // Arrange
        var helpers = new Helpers();
        var table = helpers.GetSimplifiedTable();
        var pathFinder = new PathFinder(table);
        string password = "asdf";
        string simplifiedPassword = helpers.SimplifyPassword(password);

        // Act
        var commands = pathFinder.FindPath(password, simplifiedPassword);

        // Assert
        // You can assert the commands sequence here
        Assert.NotNull(commands);
        Assert.Equal(3, commands.Count);
        // Add more assertions as needed
    }
}


public class PatternReductionTests
{
    [Fact]
    public void ReduceCommands_ShouldReturnSmallestRepeatingPattern()
    {
        // Arrange
        var helpers = new Helpers();
        var table = helpers.GetSimplifiedTable();
        var pathFinder = new PathFinder(table);
        var kmp = new KMP();
        string inputCommands = "→←→←→←";

        // Act
        string reducedPattern = kmp.GetSmallestRepeatingPattern(inputCommands);

        // Assert
        Assert.Equal("→←", reducedPattern);
    }
}

public class ComplexPasswordTests
{
    [Fact]
    public void FindPath_ShouldReturnCorrectCommands_ForComplexPassword()
    {
        // Arrange
        var helpers = new Helpers();
        var table = helpers.GetSimplifiedTable();
        var pathFinder = new PathFinder(table);
        string password = "QQQqqq!!!111";
        string simplifiedPassword = helpers.SimplifyPassword(password);

        // Act
        var commands = pathFinder.FindPath(password, simplifiedPassword);
        var reducedPattern = pathFinder.ReduceCommands(commands);
        string patternString = pathFinder.ReduceCommandsToString(reducedPattern);

        // Expected pattern representation
        string expectedPattern = "Shift+Q*3,ShiftOff+Q*3,Shift+1*3,ShiftOff1*3";

        // Assert
        Assert.Equal(expectedPattern, patternString);
    }
}
