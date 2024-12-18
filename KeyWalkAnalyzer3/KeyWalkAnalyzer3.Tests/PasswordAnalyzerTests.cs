using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace KeyWalkAnalyzer3.Tests;

public class PasswordAnalyzerTests
{
    private readonly KeyboardLayout _keyboard;
    private readonly PathAnalyzer _pathAnalyzer;
    private readonly PasswordAnalyzer _analyzer;

    public PasswordAnalyzerTests()
    {
        _keyboard = new KeyboardLayout();
        _pathAnalyzer = new PathAnalyzer();
        _analyzer = new PasswordAnalyzer(_keyboard, _pathAnalyzer);
    }
    /*
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
                  ↓
              */

    [Theory]
    [InlineData("►←◄", "hjg")]
    [InlineData("◘", "hhhhhh")]
    [InlineData("►", "hjkl;'")]
    [InlineData("►◄", "hjhjhj")]
    [InlineData("◄", "hgfdsa")]
    [InlineData("◄►", "hghghg")]
    [InlineData("►", "hj")]
    [InlineData("→→►←←◄", "hlhlhl")]
    [InlineData("←◄→►", "hfhfhf")]
    [InlineData("▲▼", "hyhyhy")]
    [InlineData("→►←◄", "hkhkhk")]
    [InlineData("▼▼↑▲▼▼↑↑►", "1qa1qa2ws2ws3ed3ed")]
    public void TestPatternToPassword(string input, string expected)
    {
        // Act
        PathAnalyzer pathAnalyzer = new PathAnalyzer();
        PasswordAnalyzer passwordAnalyzer = new PasswordAnalyzer(_keyboard, pathAnalyzer);
        passwordAnalyzer.AnalyzePassword(input);
        var password = passwordAnalyzer.GeneratePassword(input, expected[0], expected.Length);


        // Assert
        Assert.Equal(expected, password);

    }

    [Theory]
    [InlineData("hjg", "►←◄")]
    [InlineData("hhhhhh", "◘")]
    [InlineData("hjkl;'", "►")]
    [InlineData("hjhjhj", "►◄")]
    [InlineData("hgfdsa", "◄")]
    [InlineData("hghghg", "◄►")]
    [InlineData("hj", "►")]
    [InlineData("hlhlhl", "→→►←←◄")]
    [InlineData("hfhfhf", "←◄→►")]
    [InlineData("hyhyhy", "▲▼")]
    [InlineData("hkhkhk", "→►←◄")]
    [InlineData("1qa1qa2ws2ws3ed3ed", "▼▼↑▲▼▼↑↑►")]
    [InlineData("1qa!QA2ws@WS3ed#ED", "▼▼↑▲▼▼↑↑►")]
    [InlineData("qazwsx", "▼▼↑↑►▼▼")]
    [InlineData("qaz", "▼")]
    [InlineData("qazwsxedcrfv", "▼▼↑↑►")]

    public void TestPasswordToPattern(string input, string expected)
    {
        // Act
        PathAnalyzer pathAnalyzer = new PathAnalyzer();
        PasswordAnalyzer passwordAnalyzer = new PasswordAnalyzer(_keyboard, pathAnalyzer);
        passwordAnalyzer.AnalyzePassword(input);
        var pattern = passwordAnalyzer.GetSmallestPath();
        // Assert
        Assert.Equal(expected, pattern);


    }

}
