using Xunit;
using KeyboardPathAnalysis;

namespace KeyWalkAnalyzer3.Tests
{
    public class KeyWeightTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var keyWeight = new KeyWeight
            {
                LanguageFrequency = 0.1,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.5,
                IsHomeRow = true
            };

            // Assert
            Assert.Equal(0.1, keyWeight.LanguageFrequency);
            Assert.Equal(FingerStrength.Index, keyWeight.Finger);
            Assert.Equal(0.5, keyWeight.ReachDifficulty);
            Assert.True(keyWeight.IsHomeRow);
        }

        [Theory]
        [InlineData(0.0, FingerStrength.Pinky, 0.0, false, 1.5)]      // Difficult to reach, low frequency, weak finger
        [InlineData(1.0, FingerStrength.Thumb, 1.0, true, 0.4)]       // High frequency, easy reach, home row
        [InlineData(0.5, FingerStrength.Middle, 0.5, false, 1.0)]  // Moderate values
        public void CalculateWeight_ReturnsCorrectWeight(
            double languageFrequency,
            FingerStrength finger,
            double reachDifficulty,
            bool isHomeRow,
            double expectedWeightApproximation)
        {
            // Arrange
            var keyWeight = new KeyWeight
            {
                LanguageFrequency = languageFrequency,
                Finger = finger,
                ReachDifficulty = reachDifficulty,
                IsHomeRow = isHomeRow
            };

            // Act
            var calculatedWeight = keyWeight.CalculateWeight();

            // Assert
            Assert.InRange(calculatedWeight, expectedWeightApproximation * 0.9, expectedWeightApproximation * 1.1);
        }

        [Fact]
        public void CalculateWeight_HomeRowReducesWeight()
        {
            // Arrange
            var homeRowKey = new KeyWeight
            {
                LanguageFrequency = 0.5,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.0,
                IsHomeRow = true
            };

            var nonHomeRowKey = new KeyWeight
            {
                LanguageFrequency = 0.5,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.0,
                IsHomeRow = false
            };

            // Act
            var homeRowWeight = homeRowKey.CalculateWeight();
            var nonHomeRowWeight = nonHomeRowKey.CalculateWeight();

            // Assert
            Assert.True(homeRowWeight < nonHomeRowWeight);
        }

        [Fact]
        public void CalculateWeight_ReachDifficulty_IncreasesWeight()
        {
            // Arrange
            var easyReachKey = new KeyWeight
            {
                LanguageFrequency = 0.5,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.0,
                IsHomeRow = false
            };

            var hardReachKey = new KeyWeight
            {
                LanguageFrequency = 0.5,
                Finger = FingerStrength.Index,
                ReachDifficulty = 1.0,
                IsHomeRow = false
            };

            // Act
            var easyReachWeight = easyReachKey.CalculateWeight();
            var hardReachWeight = hardReachKey.CalculateWeight();

            // Assert
            Assert.True(hardReachWeight > easyReachWeight);
        }

        [Fact]
        public void CalculateWeight_LanguageFrequency_ReducesWeight()
        {
            // Arrange
            var lowFrequencyKey = new KeyWeight
            {
                LanguageFrequency = 0.1,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.5,
                IsHomeRow = false
            };

            var highFrequencyKey = new KeyWeight
            {
                LanguageFrequency = 0.9,
                Finger = FingerStrength.Index,
                ReachDifficulty = 0.5,
                IsHomeRow = false
            };

            // Act
            var lowFrequencyWeight = lowFrequencyKey.CalculateWeight();
            var highFrequencyWeight = highFrequencyKey.CalculateWeight();

            // Assert
            Assert.True(highFrequencyWeight < lowFrequencyWeight);
        }
    }
}
