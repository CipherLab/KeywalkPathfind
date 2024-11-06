namespace KeyboardPathAnalysis
{
    // Mobile keyboard weight strategy
    public class MobileKeyboardStrategy : IKeyboardWeightStrategy
    {
        public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
        {
            // Mobile keyboards have different ergonomics - thumb reach patterns
            throw new NotImplementedException();
        }

        public double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
        {
            throw new NotImplementedException();
        }
    }
}