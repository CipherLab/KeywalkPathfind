namespace KeyboardPathAnalysis
{
    public class KeyPosition
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public char Key { get; set; }

        public KeyPosition(int row, int col, char key)
        {
            Row = row;
            Col = col;
            Key = key;
        }
    }
}