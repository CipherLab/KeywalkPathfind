namespace KeyWalkAnalyzer3;

public class PatternDetector
    {
        private readonly KeyboardLayout _layout;

        public PatternDetector(KeyboardLayout layout)
        {
            _layout = layout;
        }

        public List<MovementPattern> FindRepeatingPatterns(string input)
        {
            var patterns = new List<MovementPattern>();

            // Try different pattern lengths, from 1 up to half the input length
            for (int patternLength = 1; patternLength <= input.Length / 2; patternLength++)
            {
                if (input.Length % patternLength != 0) continue;

                var potentialPattern = GetMovementPattern(input.Substring(0, patternLength));
                bool isRepeatingPattern = true;

                // Check if this pattern repeats throughout the entire input
                for (int i = patternLength; i < input.Length; i += patternLength)
                {
                    var nextSegmentPattern = GetMovementPattern(input.Substring(i, patternLength));

                    if (!AreMovementPatternsEquivalent(potentialPattern, nextSegmentPattern))
                    {
                        isRepeatingPattern = false;
                        break;
                    }
                }

                if (isRepeatingPattern)
                {
                    patterns.Add(potentialPattern);
                }
            }

            return patterns.OrderBy(p => p.Movements.Count).ToList();
        }

        private MovementPattern GetMovementPattern(string segment)
        {
            var movements = new List<Movement>();
            char startChar = segment[0];

            for (int i = 0; i < segment.Length - 1; i++)
            {
                var currentPos = _layout.GetKeyPosition(segment[i]);
                var nextPos = _layout.GetKeyPosition(segment[i + 1]);

                if (currentPos == null || nextPos == null)
                    throw new ArgumentException($"Invalid characters in segment: {segment}");

                var movement = DetermineMovement(currentPos, nextPos);
                movements.Add(movement);
            }

            return new MovementPattern(startChar, movements);
        }

        private Movement DetermineMovement(KeyPosition current, KeyPosition next)
        {
            int rowDiff = next.Row - current.Row;
            int colDiff = next.Col - current.Col;

            // Determine if this is a direct movement or needs intermediate steps
            bool isDirect = Math.Abs(rowDiff) <= 1 && Math.Abs(colDiff) <= 1;

            return new Movement
            {
                RowDiff = rowDiff,
                ColDiff = colDiff,
                IsDirect = isDirect
            };
        }

        private bool AreMovementPatternsEquivalent(MovementPattern pattern1, MovementPattern pattern2)
        {
            if (pattern1.Movements.Count != pattern2.Movements.Count)
                return false;

            // Check if the relative movements are the same
            for (int i = 0; i < pattern1.Movements.Count; i++)
            {
                if (!pattern1.Movements[i].Equals(pattern2.Movements[i]))
                    return false;
            }

            return true;
        }

        public string ValidatePattern(MovementPattern pattern, int targetLength)
        {
            var result = new List<char> { pattern.StartChar };
            char currentChar = pattern.StartChar;

            while (result.Count < targetLength)
            {
                foreach (var movement in pattern.Movements)
                {
                    if (result.Count >= targetLength) break;

                    // Use your KeyboardLayout methods to move to the next character
                    if (movement.RowDiff != 0)
                    {
                        currentChar = _layout.GetNextCharInColumn(currentChar, movement.RowDiff > 0);
                    }
                    else if (movement.ColDiff != 0)
                    {
                        currentChar = _layout.GetNextCharInRow(currentChar, movement.ColDiff > 0);
                    }

                    result.Add(currentChar);
                }
            }

            return new string(result.Take(targetLength).ToArray());
        }
    }
