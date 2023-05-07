// See https://aka.ms/new-console-template for more information
public class CommandListEqualityComparer : IEqualityComparer<List<Command>>
{
    public bool Equals(List<Command> x, List<Command> y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (x.Count != y.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i].Direction != y[i].Direction || x[i].Take != y[i].Take)
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(List<Command> obj)
    {
        int hash = 17;

        foreach (var command in obj)
        {
            hash = hash * 31 + (command.Direction?.GetHashCode() ?? 0);
            hash = hash * 31 + command.Take.GetHashCode();
        }

        return hash;
    }
}
