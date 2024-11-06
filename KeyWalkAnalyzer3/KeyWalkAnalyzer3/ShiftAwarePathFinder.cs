using System;
using System.Collections.Generic;

namespace KeyboardPathAnalysis
{

    public class ShiftAwarePathFinder
    {
        private readonly IKeyboardWeightStrategy weightStrategy;
        private readonly Dictionary<char, EnhancedKeyPosition> keyboardLayout;

        public ShiftAwarePathFinder(IKeyboardWeightStrategy strategy)
        {
            this.weightStrategy = strategy;
            this.keyboardLayout = InitializeKeyboardLayout();
        }

        public List<PathStep> FindPath(string sequence)
        {
            var path = new List<PathStep>();
            ShiftState currentShiftState = ShiftState.NoShift;

            for (int i = 0; i < sequence.Length - 1; i++)
            {
                // Determine if next character needs shift
                bool nextNeedsShift = char.IsUpper(sequence[i + 1]) ||
                                    IsShiftCharacter(sequence[i + 1]);

                // Determine optimal shift key if needed
                if (nextNeedsShift)
                {
                    currentShiftState = DetermineOptimalShiftKey(
                        keyboardLayout[sequence[i]],
                        keyboardLayout[sequence[i + 1]]
                    );
                }

                var steps = CalculateSteps(
                    keyboardLayout[sequence[i]],
                    keyboardLayout[sequence[i + 1]],
                    currentShiftState
                );

                path.AddRange(steps);

                // Reset shift state after character
                if (!nextNeedsShift)
                {
                    currentShiftState = ShiftState.NoShift;
                }
            }

            return path;
        }

        private ShiftState DetermineOptimalShiftKey(
            EnhancedKeyPosition current,
            EnhancedKeyPosition next)
        {
            // If next key is on the right side, prefer left shift
            if (next.PreferredHand == Hand.Right)
                return ShiftState.LeftShift;

            // If next key is on the left side, prefer right shift
            if (next.PreferredHand == Hand.Left)
                return ShiftState.RightShift;

            // For middle keys, choose based on current position
            return current.PreferredHand == Hand.Left ?
                ShiftState.RightShift : ShiftState.LeftShift;
        }

        private List<PathStep> CalculateSteps(
            EnhancedKeyPosition from,
            EnhancedKeyPosition to,
            ShiftState shiftState)
        {
            var steps = new List<PathStep>();

            // Add shift press if needed
            if (shiftState != ShiftState.NoShift && to.RequiresShift)
            {
                steps.Add(new PathStep("shift_down", isPress: true)
                {
                    Hand = shiftState == ShiftState.LeftShift ? Hand.Left : Hand.Right
                });
            }

            // Calculate movement
            double movementCost = weightStrategy.CalculateMovementCost(from, to, shiftState);
            steps.Add(new PathStep("move", isPress: false) { Cost = movementCost });

            // Add key press
            double pressCost = weightStrategy.CalculateKeyPressCost(to, shiftState);
            steps.Add(new PathStep("press", isPress: true) { Cost = pressCost });

            // Release shift if used
            if (shiftState != ShiftState.NoShift && to.RequiresShift)
            {
                steps.Add(new PathStep("shift_up", isPress: true));
            }

            return steps;
        }

        private bool IsShiftCharacter(char c)
        {
            return "~!@#$%^&*()_+{}|:\"<>?".Contains(c);
        }

        private Dictionary<char, EnhancedKeyPosition> InitializeKeyboardLayout()
        {
            // Implementation would initialize full keyboard layout
            // with enhanced position information
            throw new NotImplementedException();
        }
    }
}