using System.Linq;

public static class Utility
{
    public static int count_true_booleans(params bool[] booleans)
    {
        return booleans.Count(is_true => is_true);
    }
}
