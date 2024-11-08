using Xunit;
using Moq;
using KeyboardPathAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace KeyWalkAnalyzer3.Tests
{
    public class PathAnalyzerTests
    {
        private readonly PathAnalyzer _analyzer;

        public PathAnalyzerTests()
        {
            _analyzer = new PathAnalyzer();
        }

        [Fact]
        public void GenerateKeyPath_EmptyPassword_ReturnsEmptyPath()
        {
            // Arrange & Act
            var result = _analyzer.GenerateKeyPath("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateKeyPath_SingleCharacter_ReturnsEmptyPath()
        {
            // Arrange & Act
            var result = _analyzer.GenerateKeyPath("a");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateKeyPath_AdjacentKeys_GeneratesDirectPath()
        {
            // Arrange & Act
            var result = _analyzer.GenerateKeyPath("qw");

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, step => Assert.True(
                step.ToString().Contains("right") ||
                step.ToString().Contains("press")
            ));
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("asdfgh")]
        [InlineData("zxcvbn")]
        public void GenerateKeyPath_CommonPatterns_GeneratesValidPath(string input)
        {
            // Arrange & Act
            var result = _analyzer.GenerateKeyPath(input);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, step => Assert.NotNull(step.ToString()));
        }

        [Fact]
        public void EncodePath_EmptyPath_ReturnsEmptyString()
        {
            // Arrange
            var emptyPath = new List<PathStep>();

            // Act
            var result = _analyzer.EncodePath(emptyPath);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void EncodePath_RepeatedSteps_CompressesEfficiently()
        {
            // Arrange
            var path = new List<PathStep>
            {
                new PathStep('a',"right"),
                new PathStep('a', "right"),
                new PathStep('a', "right")
            };

            // Act
            var result = _analyzer.EncodePath(path);

            // Assert
            Assert.Contains("right", result);
            Assert.Matches(@"right\(\d+,\d+\)", result); // Verify coordinate format
        }

        [Theory]
        [InlineData("test1", "test2", 0.8)] // Similar passwords
        [InlineData("abc123", "xyz789", 0.2)] // Different passwords
        [InlineData("qwe123", "asdqwe", 0.83)] // Common pattern
        public void CalculateSimilarity_VariousInputs_ReturnsExpectedResults(
            string password1, string password2, double expectedMinSimilarity)
        {
            // Arrange
            var fingerprint1 = _analyzer.EncodePath(_analyzer.GenerateKeyPath(password1));
            var fingerprint2 = _analyzer.EncodePath(_analyzer.GenerateKeyPath(password2));

            // Act
            var similarity = _analyzer.CalculateSimilarity(fingerprint1, fingerprint2);

            // Assert
            Assert.True(similarity >= expectedMinSimilarity);
        }

        [Theory]
        [InlineData("", "test")]
        [InlineData("test", "")]
        [InlineData("", "")]
        public void CalculateSimilarity_EmptyInputs_ReturnsZero(string fp1, string fp2)
        {
            // Act
            var similarity = _analyzer.CalculateSimilarity(fp1, fp2);

            // Assert
            Assert.Equal(0, similarity);
        }
    }
}
