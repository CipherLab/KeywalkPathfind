namespace KeyboardPathAnalysis
{
    // Gaming keyboard weight strategy - different priorities
    public class GamingKeyboardStrategy : IKeyboardWeightStrategy
    {
        public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
        {
            // Gaming keyboards often prioritize WASD and nearby keys
            // Implementation would focus on different ergonomic factors
            throw new NotImplementedException();
        }

        public double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
        {
            throw new NotImplementedException();
        }
    }
}