using Xunit;
using Moq;
using KeyboardPathAnalysis;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3.Tests
{
    public class PasswordAnalyzerTests
    {
        private readonly PasswordAnalyzer _analyzer;

        public PasswordAnalyzerTests()
        {
            _analyzer = new PasswordAnalyzer();
        }

        [Fact]
        public void AnalyzePassword_EmptyPassword_CreatesNoPatternGroups()
        {
            // Arrange & Act
            _analyzer.AnalyzePassword("");

            // Assert
            var groups = _analyzer.GetPatternGroups();
            Assert.Empty(groups);
        }

        [Fact]
        public void AnalyzePassword_SinglePassword_CreatesNewPatternGroup()
        {
            // Arrange & Act
            _analyzer.AnalyzePassword("test123");

            // Assert
            var groups = _analyzer.GetPatternGroups();
            Assert.Single(groups);
            Assert.Contains("test123", groups.First().Value);
        }

        [Fact]
        public void AnalyzePassword_SimilarPasswords_GroupsTogether()
        {
            // Arrange & Act
            _analyzer.AnalyzePassword("qwerty");
            _analyzer.AnalyzePassword("qwerty123");

            // Assert
            var groups = _analyzer.GetPatternGroups();
            Assert.True(groups.Count <= 2); // Should be grouped if similarity > 0.8
            Assert.Contains(groups, g => g.Value.Contains("qwerty") && g.Value.Contains("qwerty123"));
        }

        [Fact]
        public void AnalyzePassword_DissimilarPasswords_CreatesSeparateGroups()
        {
            // Arrange & Act
            _analyzer.AnalyzePassword("abc123");
            _analyzer.AnalyzePassword("xyz789");

            // Assert
            var groups = _analyzer.GetPatternGroups();
            Assert.Equal(2, groups.Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AnalyzePassword_InvalidInput_HandlesGracefully(string password)
        {
            // Arrange & Act
            _analyzer.AnalyzePassword(password);

            // Assert
            var groups = _analyzer.GetPatternGroups();
            Assert.Empty(groups);
        }
    }
}
