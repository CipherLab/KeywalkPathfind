using Xunit;
using KeyWalkAnalyzer3;
using System.Linq;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3.Tests
{
    public class PathAnalyzerTests
    {


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

    }
}
