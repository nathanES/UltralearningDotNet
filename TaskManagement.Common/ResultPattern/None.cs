namespace TaskManagement.Common.ResultPattern;

public sealed class None
{
    public static readonly None Value = new None();

    private None() { }
}