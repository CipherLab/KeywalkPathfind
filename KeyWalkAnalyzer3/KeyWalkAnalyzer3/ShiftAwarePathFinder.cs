// Updated ShiftAwarePathFinder.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyWalkAnalyzer3;

public class ShiftAwarePathFinder
{
    private readonly IKeyboardWeightStrategy weightStrategy;
    private readonly Dictionary<char, EnhancedKeyPosition> keyboardLayout;

    public ShiftAwarePathFinder(IKeyboardWeightStrategy strategy)
    {
        weightStrategy = strategy;
        keyboardLayout = Utilities.InitializeKeyboardLayout();
    }

    public List<PathStep> FindPath(string sequence)
    {
        if (string.IsNullOrEmpty(sequence))
            return new List<PathStep>();

        var path = new List<PathStep>();
        ShiftState currentShiftState = ShiftState.NoShift;
        bool shiftActive = false;

        for (int i = 0; i < sequence.Length; i++)
        {
            char currentChar = sequence[i];
            char lowerChar = char.ToLower(currentChar);
            bool needsShift = char.IsUpper(currentChar) || Utilities.IsShiftCharacter(currentChar);

            // Retrieve key position
            EnhancedKeyPosition currentKeyPos = keyboardLayout.ContainsKey(lowerChar)
                ? keyboardLayout[lowerChar]
                : null;

            // Determine if shift is required
            if (needsShift && !shiftActive)
            {
                currentShiftState = DetermineOptimalShiftKey(currentKeyPos);

                // Add shift down step
                path.Add(new PathStep(
                    currentChar,  // Use the current character as the key
                    "shift_down",
                    isPress: true
                ));
                shiftActive = true;
            }

            // Calculate steps for the current character
            var steps = CalculateSteps(
                path.Count > 0 ? keyboardLayout[char.ToLower(path.Last().Key)] : null,
                currentKeyPos,
                currentShiftState
            );

            // Set the Key for press actions
            foreach (var step in steps)
            {
                step.Key = currentChar;
            }

            path.AddRange(steps);

            // Release shift if no longer needed
            if (shiftActive && !needsShift)
            {
                path.Add(new PathStep(
                    currentChar,
                    "shift_up",
                    isPress: true
                ));
                shiftActive = false;
                currentShiftState = ShiftState.NoShift;
            }
        }

        return path;
    }

    private ShiftState DetermineOptimalShiftKey(EnhancedKeyPosition keyPos)
    {
        // If key requires shift, choose shift based on hand preference
        return keyPos.PreferredHand == Hand.Left
            ? ShiftState.RightShift
            : ShiftState.LeftShift;
    }

    private List<PathStep> CalculateSteps(
        EnhancedKeyPosition from,
        EnhancedKeyPosition to,
        ShiftState shiftState)
    {
        var steps = new List<PathStep>();

        // Calculate movement
        double movementCost = weightStrategy.CalculateMovementCost(from, to, shiftState);
        steps.Add(new PathStep('\0', "move", isPress: false) { Cost = movementCost });

        // Add key press and release
        double pressCost = weightStrategy.CalculateKeyPressCost(to, shiftState);
        steps.Add(new PathStep(to != null ? to.Key : '\0', "press", isPress: true) { Cost = pressCost });
        steps.Add(new PathStep(to != null ? to.Key : '\0', "release", isPress: false) { Cost = pressCost * 0.5 });

        return steps;
    }


}