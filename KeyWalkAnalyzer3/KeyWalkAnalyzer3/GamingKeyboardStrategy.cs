namespace KeyboardPathAnalysis
{
    // Gaming keyboard optimized movement strategy
    public class GamingKeyboardStrategy : IKeyboardWeightStrategy
    {
        private readonly HashSet<char> gamingCriticalKeys = new HashSet<char>
        {
            'w', 'a', 's', 'd',
            '1', '2', '3', '4', '5',
            'q', 'e', 'r', 'f', 'g'
        };

        public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
        {
            var fromEnhanced = from as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(from);
            var toEnhanced = to as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(to);

            double cost = CalculateBaseCost(fromEnhanced, toEnhanced);

            // Apply shift key penalties with gaming-specific adjustments
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

            // Gaming keyboards have more responsive key mechanics
            cost *= (4 - (int)enhancedKey.Finger) / 3.0;  // More finger-friendly
            cost *= (1 + enhancedKey.ReachDifficulty * 0.7);  // Reduced reach penalty

            // Significant advantage for gaming-critical keys
            if (gamingCriticalKeys.Contains(char.ToLower(enhancedKey.Key)))
            {
                cost *= 0.6;  // Lower cost for gaming-critical keys
            }

            // Home row and gaming key cluster advantages
            if (enhancedKey.IsHomeRow || IsGamingKeyCluster(enhancedKey))
            {
                cost *= 0.7;
            }

            // Shift penalties with gaming keyboard considerations
            if (shiftState != ShiftState.NoShift)
            {
                bool isShiftHandConstrained =
                    (shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left) ||
                    (shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right);

                if (isShiftHandConstrained)
                {
                    cost *= 1.2;  // Slightly reduced shift penalty
                }
            }

            return cost;
        }

        private bool IsGamingKeyCluster(EnhancedKeyPosition key)
        {
            // Define gaming key clusters based on typical gaming keyboard layouts
            return (key.Row == 1 && key.Col >= 1 && key.Col <= 5) ||  // WASD area
                   (key.Row == 0 && key.Col >= 1 && key.Col <= 5);    // Number row near WASD
        }

        private EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
        {
            // Create a default EnhancedKeyPosition with gaming keyboard assumptions
            return new EnhancedKeyPosition(
                key.Row,
                key.Col,
                key.Key,
                Hand.Right,  // Default to right hand
                FingerStrength.Middle,  // Default to middle finger
                0.4,  // Reduced reach difficulty for gaming keyboards
                false  // Not home row by default
            );
        }

        private double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
        {
            // Physical distance with gaming keyboard spacing
            double distance = Math.Sqrt(
                Math.Pow(from.Row - to.Row, 2) +
                Math.Pow(from.Col - to.Col, 2)
            );

            // Same finger penalty (reduced for gaming keyboards)
            if (from.Finger == to.Finger && from.PreferredHand == to.PreferredHand)
            {
                distance *= 1.2;  // Less penalty compared to standard keyboard
            }

            // Cross-hand movement bonus
            if (from.PreferredHand != to.PreferredHand)
            {
                distance *= 0.7;  // Enhanced cross-hand movement
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

            // Reduced penalty if the shift hand needs to reach
            if (to.PreferredHand == shiftHand)
            {
                cost *= (1 + to.ReachDifficulty);  // Less severe reach penalty
            }

            // Reduced penalty for awkward combinations
            if (from.PreferredHand == shiftHand && to.PreferredHand == shiftHand)
            {
                cost *= 1.5;  // Less severe than standard keyboard
            }

            return cost;
        }
    }
}
