namespace PowLINQPad.Structs;

public sealed record RngInt(int? Min, int? Max)
{
	public static readonly RngInt Empty = new(null, null);
	public override string ToString() => $"[{f(Min)} - {f(Max)}]";
	private static string f(int? v) => v.HasValue ? $"{v}" : "_";
}
public sealed record RngIntBounds(int Min, int Max);
