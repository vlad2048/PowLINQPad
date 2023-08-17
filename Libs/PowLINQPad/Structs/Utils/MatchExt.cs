namespace PowLINQPad.Structs.Utils;

public static class MatchExt
{
	public static bool Matches(this TxtSearch f, string v) => f.UseRegex switch
	{
		false => f.Parts!.Length == 0 || f.Parts.Any(part => v.Contains(part, StringComparison.InvariantCultureIgnoreCase)),
		true => f.Regex switch
		{
			not null => f.Regex.IsMatch(v),
			null => true
		}
	};
	
	public static bool Matches(this BoolOpt f, bool v) => f switch
	{
		BoolOpt.None => true,
		BoolOpt.True => v,
		BoolOpt.False => !v,
		_ => throw new ArgumentException()
	};
	
	public static bool Matches(this int? f, int v) => f switch
	{
		null => true,
		not null => f.Value == v
	};
	
	public static bool Matches(this RngInt f, int v) =>
		(!f.Min.HasValue || f.Min.Value <= v) &&
		(!f.Max.HasValue || f.Max.Value >= v);
	
	public static bool Matches(this RngInt f, int? v) => v switch
	{
		null =>
			!f.Min.HasValue && !f.Max.HasValue,
		not null =>
			(!f.Min.HasValue || f.Min.Value <= v) &&
			(!f.Max.HasValue || f.Max.Value >= v)
	};
	
	public static bool Matches(this RngTime f, DateTime v) =>
		(!f.Min.HasValue || f.Min.Value <= v) &&
		(!f.Max.HasValue || f.Max.Value >= v);
	
	public static bool Matches<E>(this E[] f, E v) where E : struct, Enum => f.Contains(v);
}