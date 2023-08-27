namespace PowLINQPad.Editing.Layout_;

sealed record Pos(
	string? TabName,
	int? Index
)
{
	public static readonly Pos Empty = new(null, null);
}

public class PosAttribute : Attribute
{
	internal Pos Pos { get; }

	public PosAttribute(string? tabName, int index) => Pos = new Pos(tabName, index);
}


static class PosExt
{
	public static Pos ReadPos(this Attribute[] attrs)
	{
		var attr = attrs.FirstOrDefault(e => e is PosAttribute);
		return attr switch
		{
			null => Pos.Empty,
			not null => ((PosAttribute)attr).Pos
		};
	}
}