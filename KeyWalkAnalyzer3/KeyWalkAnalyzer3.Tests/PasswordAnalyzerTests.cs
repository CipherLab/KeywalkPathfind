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
              */

    [Theory]
    [InlineData("◘", "hhhhhh")]
    [InlineData("►", "hjkl;'")]
    [InlineData("►◄", "hjhjhj")]
    [InlineData("◄", "hgfdsa")]
    [InlineData("◄►", "hghghg")]
    [InlineData("►►←◄", "hjkhjk")]
    [InlineData("→→►←←◄", "hlhlhl")]
    [InlineData("←◄→►", "hfhfhf")]
    [InlineData("▲▼", "hyhyhy")]
    [InlineData("→►←◄", "hkhkhk")]
    public void Test(string input, string expected)
    {
        // Act
        PathAnalyzer pathAnalyzer = new PathAnalyzer();
        PasswordAnalyzer passwordAnalyzer = new PasswordAnalyzer(_keyboard, pathAnalyzer);
        passwordAnalyzer.AnalyzePasswords(new string[] { input });
        var password = passwordAnalyzer.GeneratePassword(input, expected[0], expected.Length);


        // Assert
        Assert.Equal(expected, password);

    }

}
