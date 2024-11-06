namespace KeyboardPathAnalysis
{
    public class WeightedAStar : AStar
    {
        private readonly WeightedKeyboardLayout weightedKeyboard;

        public WeightedAStar(WeightedKeyboardLayout keyboard) : base(keyboard)
        {
            this.weightedKeyboard = keyboard;
        }

        protected override double CalculateCost(char fromKey, char toKey)
        {
            return weightedKeyboard.GetMovementCost(fromKey, toKey);
        }

        public override List<PathStep> FindPath(char startKey, char endKey)
        {
            var path = base.FindPath(startKey, endKey);

            // Annotate path with effort metrics
            double totalEffort = 0;
            foreach (var step in path)
            {
                if (!step.IsPress)
                {
                    totalEffort += weightedKeyboard.GetMovementCost(startKey, endKey);
                }
            }

            // Add effort metadata to path
            path[0].Metadata["TotalEffort"] = totalEffort;

            return path;
        }
    }
}