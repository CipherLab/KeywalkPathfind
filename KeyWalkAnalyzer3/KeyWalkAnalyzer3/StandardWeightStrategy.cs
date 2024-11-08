namespace KeyboardPathAnalysis
{
    // Standard English frequency and ergonomic weighting
    public class StandardWeightStrategy : IKeyboardWeightStrategy
    {
        private readonly Dictionary<char, double> languageFrequencies;

        public StandardWeightStrategy()
        {
            languageFrequencies = new Dictionary<char, double>
            {
                {'e', 0.111607}, {'a', 0.084966}, {'r', 0.075809},
                // ... other frequencies ...
            };
        }

        public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
        {
            var fromEnhanced = from as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(from);
            var toEnhanced = to as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(to);

            double cost = CalculateBaseCost(fromEnhanced, toEnhanced);

            // Apply shift key penalties
            if (shiftState != ShiftState.NoShift)
            {
                cost = ApplyShiftPenalties(cost, fromEnhanced, toEnhanced, shiftState);
            }

            return cost;
        }

        public double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
        {
            var enhancedKey = key as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(key);
            double cost = 1.0;

            // Base difficulty from finger strength and reach
            cost *= (5 - (int)enhancedKey.Finger) / 4.0;
            cost *= (1 + enhancedKey.ReachDifficulty);

            // Home row advantage
            if (enhancedKey.IsHomeRow) cost *= 0.8;

            // Shift penalties
            if (shiftState != ShiftState.NoShift)
            {
                bool isShiftHandConstrained =
                    (shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left) ||
                    (shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right);

                if (isShiftHandConstrained)
                {
                    cost *= 1.5; // Significant penalty for using shifted hand
                }
            }

            return cost;
        }

        private EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
        {
            // Create a default EnhancedKeyPosition with minimal assumptions
            return new EnhancedKeyPosition(
                key.Row,
                key.Col,
                key.Key,
                Hand.Right,  // Default to right hand
                FingerStrength.Middle,  // Default to middle finger
                0.5,  // Moderate reach difficulty
                false  // Not home row by default
            );
        }

        private double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
        {
            // Physical distance
            double distance = Math.Sqrt(
                Math.Pow(from.Row - to.Row, 2) +
                Math.Pow(from.Col - to.Col, 2)
            );

            // Same finger penalty
            if (from.Finger == to.Finger && from.PreferredHand == to.PreferredHand)
            {
                distance *= 1.5;
            }

            // Cross-hand movement bonus
            if (from.PreferredHand != to.PreferredHand)
            {
                distance *= 0.8;
            }

            return distance;
        }

        private double ApplyShiftPenalties(double baseCost,
            EnhancedKeyPosition from,
            EnhancedKeyPosition to,
            ShiftState shiftState)
        {
            double cost = baseCost;

            // Determine which hand is holding shift
            Hand shiftHand = shiftState == ShiftState.LeftShift ? Hand.Left : Hand.Right;

            // Significant penalty if the shift hand needs to reach far
            if (to.PreferredHand == shiftHand)
            {
                cost *= (1 + to.ReachDifficulty * 1.5);
            }

            // Extra penalty for awkward combinations
            if (from.PreferredHand == shiftHand && to.PreferredHand == shiftHand)
            {
                cost *= 2.0; // Very awkward to use shift and press keys with same hand
            }

            return cost;
        }
    }
}
