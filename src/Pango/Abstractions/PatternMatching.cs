namespace Pango.Abstractions;

public static class PatternMatching
{
    public static U Match<U, T1, T2>(Func<T1, U> match1, Func<T2, U> match2, T1? value1, T2? value2)
    {
        if (value1 is not null)
            return match1(value1);

        if (value2 is not null)
            return match2(value2);

        throw new InvalidOperationException("Match should have least 1 valid value");
    }
    public static void Match<T1, T2>(Action<T1> match1, Action<T2> match2, T1? value1, T2? value2)
    {
        if (value1 is not null)
            match1(value1);

        if (value2 is not null)
            match2(value2);
    }
}