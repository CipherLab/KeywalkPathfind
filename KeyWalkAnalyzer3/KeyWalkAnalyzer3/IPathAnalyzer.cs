using System.Collections.Generic;

namespace KeyWalkAnalyzer3
{
    public interface IPathAnalyzer
    {
        List<PathStep> GenerateKeyPath(string password);
        string EncodePath(List<PathStep> path);
        double CalculateSimilarity(string fingerprint1, string fingerprint2);
    }
}
