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

            // Frequently used letters should have lower weights as they're more predictable
            baseWeight *= (1 - LanguageFrequency);

            // Stronger fingers make keys easier to press
            baseWeight *= (5 - (int)Finger) / 5.0;

            // Home row keys are easier to access
            if (IsHomeRow) baseWeight *= 0.8;

            return baseWeight;
        }
    }
}