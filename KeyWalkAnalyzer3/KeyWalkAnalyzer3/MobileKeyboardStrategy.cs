namespace KeyboardPathAnalysis
{
    // Mobile keyboard optimized movement strategy
    public class MobileKeyboardStrategy : IKeyboardWeightStrategy
    {
        private readonly HashSet<char> frequentMobileKeys = new HashSet<char>
        {
            'e', 'a', 'r', 'i', 'o', 'n',
            ' ', // Space bar is critical in mobile typing
            '.' // Punctuation is frequent
        };

        public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
        {
            var fromEnhanced = from as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(from);
            var toEnhanced = to as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(to);

            double cost = CalculateBaseCost(fromEnhanced, toEnhanced);

            // Apply mobile-specific shift penalties
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

            // Mobile keyboard thumb-based typing dynamics
            cost *= (4 - (int)enhancedKey.Finger) / 3.5;  // Thumb-centric weighting
            cost *= (1 + enhancedKey.ReachDifficulty * 1.2);  // Higher reach difficulty

            // Frequent mobile typing keys get slight advantage
            if (frequentMobileKeys.Contains(char.ToLower(enhancedKey.Key)))
            {
                cost *= 0.8;  // Slight cost reduction for common keys
            }

            // Thumb-friendly zones
            if (IsThumbFriendlyZone(enhancedKey))
            {
                cost *= 0.7;  // Lower cost in thumb-friendly areas
            }

            // Shift penalties with mobile keyboard considerations
            if (shiftState != ShiftState.NoShift)
            {
                bool isShiftHandConstrained =
                    (shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left) ||
                    (shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right);

                if (isShiftHandConstrained)
                {
                    cost *= 1.3;  // Moderate shift penalty
                }
            }

            return cost;
        }

        private bool IsThumbFriendlyZone(EnhancedKeyPosition key)
        {
            // Define thumb-friendly zones typical in mobile keyboards
            // Assumes a layout where bottom rows are more thumb-accessible
            return (key.Row >= 2 && key.Row <= 3) &&  // Bottom rows
                   (key.Col >= 2 && key.Col <= 8);    // Central key area
        }

        private EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
        {
            // Create a default EnhancedKeyPosition with mobile keyboard assumptions
            return new EnhancedKeyPosition(
                key.Row,
                key.Col,
                key.Key,
                Hand.Right,  // Default to right hand (thumb-typing assumption)
                FingerStrength.Thumb,  // Thumb as primary typing finger
                0.6,  // Higher reach difficulty for mobile keyboards
                false  // Not home row by default
            );
        }

        private double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
        {
            // Physical distance with mobile keyboard spacing
            double distance = Math.Sqrt(
                Math.Pow(from.Row - to.Row, 2) +
                Math.Pow(from.Col - to.Col, 2)
            );

            // Same thumb/finger penalty (higher for mobile)
            if (from.Finger == to.Finger && from.PreferredHand == to.PreferredHand)
            {
                distance *= 1.7;  // Higher penalty for same thumb/finger movement
            }

            // Cross-thumb movement consideration
            if (from.PreferredHand != to.PreferredHand)
            {
                distance *= 0.9;  // Slight cross-thumb movement bonus
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

            // Moderate penalty if the shift thumb needs to reach
            if (to.PreferredHand == shiftHand)
            {
                cost *= (1 + to.ReachDifficulty * 1.2);  // Moderate reach penalty
            }

            // Penalty for awkward thumb combinations
            if (from.PreferredHand == shiftHand && to.PreferredHand == shiftHand)
            {
                cost *= 1.8;  // Higher penalty for same-hand thumb movements
            }

            return cost;
        }
    }
}
