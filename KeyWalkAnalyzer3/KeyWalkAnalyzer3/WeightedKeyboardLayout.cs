namespace KeyWalkAnalyzer3;

public class WeightedKeyboardLayout : KeyboardLayout
{
    private Dictionary<char, KeyWeight> keyWeights;

    public WeightedKeyboardLayout() : base()
    {
        InitializeWeights();
    }

    private void InitializeWeights()
    {
        keyWeights = new Dictionary<char, KeyWeight>();

        // English letter frequencies (normalized)
        var frequencies = new Dictionary<char, double>
        {
            {'e', 0.111607}, {'a', 0.084966}, {'r', 0.075809},
            {'i', 0.075448}, {'o', 0.071635}, {'t', 0.069509},
            {'n', 0.066544}, {'s', 0.057351}, {'l', 0.054893},
            {'c', 0.045388}, {'u', 0.036308}, {'d', 0.033844},
            {'p', 0.031671}, {'m', 0.030129}, {'h', 0.030034},
            {'g', 0.024705}, {'b', 0.020720}, {'f', 0.018121},
            {'y', 0.017779}, {'w', 0.012899}, {'k', 0.011016},
            {'v', 0.010074}, {'x', 0.002902}, {'z', 0.002722},
            {'j', 0.001965}, {'q', 0.001962}
        };

        // Define finger assignments and reach difficulties
        var homePositions = new Dictionary<char, (FingerStrength finger, double reach, bool home)>
        {
            // Left hand
            {'q', (FingerStrength.Pinky, 0.8, false)},
            {'a', (FingerStrength.Pinky, 0.4, true)},
            {'z', (FingerStrength.Pinky, 0.9, false)},

            {'w', (FingerStrength.Ring, 0.7, false)},
            {'s', (FingerStrength.Ring, 0.3, true)},
            {'x', (FingerStrength.Ring, 0.8, false)},

            {'e', (FingerStrength.Middle, 0.6, false)},
            {'d', (FingerStrength.Middle, 0.2, true)},
            {'c', (FingerStrength.Middle, 0.7, false)},

            {'r', (FingerStrength.Index, 0.5, false)},
            {'f', (FingerStrength.Index, 0.1, true)},
            {'v', (FingerStrength.Index, 0.6, false)},
            
            // Right hand
            {'y', (FingerStrength.Index, 0.5, false)},
            {'h', (FingerStrength.Index, 0.1, true)},
            {'n', (FingerStrength.Index, 0.6, false)},

            {'u', (FingerStrength.Middle, 0.6, false)},
            {'j', (FingerStrength.Middle, 0.2, true)},
            {'m', (FingerStrength.Middle, 0.7, false)},

            {'i', (FingerStrength.Ring, 0.7, false)},
            {'k', (FingerStrength.Ring, 0.3, true)},

            {'o', (FingerStrength.Pinky, 0.8, false)},
            {'l', (FingerStrength.Pinky, 0.4, true)},
            {'p', (FingerStrength.Pinky, 0.9, false)}
        };

        foreach (var key in homePositions.Keys)
        {
            var (finger, reach, home) = homePositions[key];
            keyWeights[key] = new KeyWeight
            {
                LanguageFrequency = frequencies.GetValueOrDefault(key, 0.001),
                Finger = finger,
                ReachDifficulty = reach,
                IsHomeRow = home
            };
        }
    }

    public double GetMovementCost(char fromKey, char toKey)
    {
        var fromWeight = keyWeights.GetValueOrDefault(fromKey, new KeyWeight());
        var toWeight = keyWeights.GetValueOrDefault(toKey, new KeyWeight());

        // Base cost is physical distance
        var fromPos = GetKeyPosition(fromKey);
        var toPos = GetKeyPosition(toKey);
        double distance = Math.Sqrt(
            Math.Pow(fromPos.Row - toPos.Row, 2) +
            Math.Pow(fromPos.Col - toPos.Col, 2)
        );

        // Modify cost based on weights
        double cost = distance;

        // Add weight for target key difficulty
        cost *= toWeight.CalculateWeight();

        // If using same finger, increase cost
        if (fromWeight.Finger == toWeight.Finger)
        {
            cost *= 1.5;
        }

        // Crossing hands is generally easier than stretching one hand
        bool crossingHands = fromPos.Col < 5 && toPos.Col >= 5 ||
                           fromPos.Col >= 5 && toPos.Col < 5;
        if (crossingHands)
        {
            cost *= 0.8;
        }

        return cost;
    }
}