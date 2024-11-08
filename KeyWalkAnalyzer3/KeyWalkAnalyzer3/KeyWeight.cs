namespace KeyboardPathAnalysis
{
    public class KeyWeight
    {
        public double LanguageFrequency { get; set; }  // Based on English letter frequency
        public FingerStrength Finger { get; set; }     // Which finger typically hits this key
        public double ReachDifficulty { get; set; }    // How far from home position (0-1)
        public bool IsHomeRow { get; set; }            // Whether it's on the home row

        public double CalculateWeight()
        {
            double baseWeight = 1.0;

            // Harder to reach keys should have higher weights
            baseWeight *= (1 + ReachDifficulty);

            // Frequently used letters should have lower weights
            baseWeight *= (1 - LanguageFrequency);

            // Stronger fingers make keys easier to press
            baseWeight *= (1.0 + (int)Finger + 0.1) / 5.1; // Reversed to match enum values

            // Home row keys are easier to access
            if (IsHomeRow) baseWeight *= 0.8;

            return baseWeight;
        }
    }
}