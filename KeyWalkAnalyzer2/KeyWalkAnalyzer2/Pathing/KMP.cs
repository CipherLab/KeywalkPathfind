public class KMP
{
    public int[] BuildLPSArray(string pattern)
    {
        int[] lps = new int[pattern.Length];
        int len = 0;
        lps[0] = 0;
        int i = 1;

        while (i < pattern.Length)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    len = lps[len - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }

        return lps;
    }

    public string GetSmallestRepeatingPattern(string input)
    {
        for (int patternLength = 1; patternLength <= input.Length / 2; patternLength++)
        {
            string pattern = input.Substring(0, patternLength);
            int nextIndex = patternLength;

            while (nextIndex < input.Length)
            {
                if (input.Substring(nextIndex).StartsWith(pattern))
                {
                    nextIndex += patternLength;
                }
                else
                {
                    break;
                }
            }

            if (nextIndex >= input.Length - patternLength + 1)
            {
                return pattern;
            }
        }

        return input; // If no repeating pattern is found, return the original string.
    }

    public string FindSmallestRepeatingPattern(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string[] parts = input.Split(',');
        string joined = string.Join("", parts);
        int[] lps = BuildLPSArray(joined);

        int patternLength = joined.Length - lps.Last();
        if (patternLength < joined.Length && joined.Length % patternLength == 0)
        {
            return joined.Substring(0, patternLength);
        }
        else
        {
            return input;
        }
    }
}