// Updated ShiftAwarePathFinder.cs

using System;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3;

public class ShiftAwarePathFinder
{
    private readonly IKeyboardWeightStrategy weightStrategy;
    private readonly Dictionary<char, EnhancedKeyPosition> keyboardLayout;

    public ShiftAwarePathFinder(IKeyboardWeightStrategy strategy)
    {
        weightStrategy = strategy;
        keyboardLayout = InitializeKeyboardLayout();
    }

    public List<PathStep> FindPath(string sequence)
    {
        var path = new List<PathStep>();
        ShiftState currentShiftState = ShiftState.NoShift;

        for (int i = 0; i < sequence.Length; i++)
        {
            char currentChar = sequence[i];
            char lowerChar = char.ToLower(currentChar);
            bool needsShift = char.IsUpper(currentChar) || Utilities.IsShiftCharacter(currentChar);

            // Determine optimal shift key if needed
            if (needsShift)
            {
                EnhancedKeyPosition current = keyboardLayout.ContainsKey(lowerChar) ? keyboardLayout[lowerChar] : null;
                EnhancedKeyPosition next = null;
                if (i + 1 < sequence.Length)
                    next = keyboardLayout.ContainsKey(char.ToLower(sequence[i + 1])) ? keyboardLayout[char.ToLower(sequence[i + 1])] : null;

                if (current != null && next != null)
                {
                    currentShiftState = DetermineOptimalShiftKey(current, next);
                }
            }

            var steps = CalculateSteps(
                keyboardLayout.ContainsKey(lowerChar) ? keyboardLayout[lowerChar] : null,
                needsShift ? keyboardLayout.ContainsKey(lowerChar) ? keyboardLayout[lowerChar] : null : keyboardLayout.ContainsKey(lowerChar) ? keyboardLayout[lowerChar] : null,
                currentShiftState
            );

            // Set the Key for press actions
            foreach (var step in steps)
            {
                if (step.Direction == "press" || step.Direction == "release")
                    step.Key = currentChar;
            }

            path.AddRange(steps);

            // Reset shift state after character
            if (!needsShift)
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
        if (shiftState != ShiftState.NoShift && to != null && to.RequiresShift)
        {
            char shiftKey = shiftState == ShiftState.LeftShift ? 'L' : 'R';
            steps.Add(new PathStep(shiftKey, "shift_down", isPress: true)
            {
                Hand = shiftState == ShiftState.LeftShift ? Hand.Left : Hand.Right
            });
        }

        // Calculate movement
        double movementCost = weightStrategy.CalculateMovementCost(from, to, shiftState);
        steps.Add(new PathStep('\0', "move", isPress: false) { Cost = movementCost });

        // Add key press and release
        double pressCost = weightStrategy.CalculateKeyPressCost(to, shiftState);
        steps.Add(new PathStep(to != null ? to.Key : '\0', "press", isPress: true) { Cost = pressCost });
        steps.Add(new PathStep(to != null ? to.Key : '\0', "release", isPress: false) { Cost = pressCost * 0.5 }); // Added release step

        // Release shift if used
        if (shiftState != ShiftState.NoShift && to != null && to.RequiresShift)
        {
            char shiftKey = shiftState == ShiftState.LeftShift ? 'L' : 'R';
            steps.Add(new PathStep(shiftKey, "shift_up", isPress: true));
        }

        return steps;
    }

    private Dictionary<char, EnhancedKeyPosition> InitializeKeyboardLayout()
    {
        // Implementation would initialize full keyboard layout
        // with enhanced position information
        throw new NotImplementedException();
    }
}