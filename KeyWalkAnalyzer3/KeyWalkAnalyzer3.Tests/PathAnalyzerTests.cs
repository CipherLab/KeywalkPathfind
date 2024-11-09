using Xunit;
using KeyWalkAnalyzer3;
using System.Linq;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3.Tests
{
    public class PathAnalyzerTests
    {
        [Fact]
        public void EncodePath_CanCompressSpacedRepeatedPatterns()
        {
            // Arrange
            var pathAnalyzer = new PathAnalyzer();
            var path = new List<PathStep>
            {
                new PathStep('q', "press", true),  // Initial press (ignored)
                new PathStep(' ', "right", false),
                new PathStep('e', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('t', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('u', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('o', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('[', "press", true)
            };

            // Act
            string encodedPath = pathAnalyzer.EncodePath(path);

            // Assert
            // Should encode as ([right,press]*5) since it's a repeating pattern of right movement followed by press
            Assert.Equal("([right,press]*5)", encodedPath);
        }

        [Fact]
        public void EncodePath_CanCompressRepeatedPatterns()
        {
            // Arrange
            var pathAnalyzer = new PathAnalyzer();
            var path = new List<PathStep>
            {
                new PathStep('a', "press", true),  // Initial press (ignored)
                new PathStep(' ', "right", false),
                new PathStep('a', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('a', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('a', "press", true),
                new PathStep(' ', "right", false),
                new PathStep('a', "press", true)
            };

            // Act
            string encodedPath = pathAnalyzer.EncodePath(path);

            // Assert
            // Should encode as ([right,press]*4) since it's a repeating pattern of right movement followed by press
            Assert.Equal("([right,press]*4)", encodedPath);
        }

        [Fact]
        public void EncodePath_HandlesEmptyPath()
        {
            // Arrange
            var pathAnalyzer = new PathAnalyzer();
            var path = new List<PathStep>();

            // Act
            string encodedPath = pathAnalyzer.EncodePath(path);

            // Assert
            Assert.Equal(string.Empty, encodedPath);
        }

        [Fact]
        public void EncodePath_HandlesSingleStep()
        {
            // Arrange
            var pathAnalyzer = new PathAnalyzer();
            var path = new List<PathStep>
            {
                new PathStep('a', "press", true)
            };

            // Act
            string encodedPath = pathAnalyzer.EncodePath(path);

            // Assert
            // Should return empty string since we ignore the first press
            Assert.Equal(string.Empty, encodedPath);
        }
    }
}
