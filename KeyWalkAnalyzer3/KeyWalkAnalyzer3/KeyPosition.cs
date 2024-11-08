namespace KeyboardPathAnalysis
{
    public class KeyPosition(int row, int col, char key)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;
        public char Key { get; set; } = key;
    }
}