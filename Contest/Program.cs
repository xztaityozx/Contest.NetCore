using static IO;


return;

# pragma warning disable CS8321
public static class IO
{
    public static string ReadLine() => Console.ReadLine() ?? throw new System.Data.NoNullAllowedException();
    public static string[] Tokens() => ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    public static T Next<T>() where T : IParsable<T> => T.Parse(ReadLine(), null);
    public static (T1, T2) Next<T1, T2>() where T1 : IParsable<T1> where T2 : IParsable<T2>
    {
        var tokens = Tokens();
        return (T1.Parse(tokens[0], null), T2.Parse(tokens[1], null));
    }
    public static (T1, T2, T3) Next<T1, T2, T3>() where T1 : IParsable<T1> where T2 : IParsable<T2> where T3 : IParsable<T3>
    {
        var tokens = Tokens();
        return (T1.Parse(tokens[0], null), T2.Parse(tokens[1], null), T3.Parse(tokens[2], null));
    }
    public static (T1, T2, T3, T4) Next<T1, T2, T3, T4>() where T1 : IParsable<T1> where T2 : IParsable<T2> where T3 : IParsable<T3> where T4 : IParsable<T4>
    {
        var tokens = Tokens();
        return (T1.Parse(tokens[0], null), T2.Parse(tokens[1], null), T3.Parse(tokens[2], null), T4.Parse(tokens[3], null));
    }
}
