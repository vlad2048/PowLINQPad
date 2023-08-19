using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI;

public static class CssClsExt
{
	public static C AddCls<C>(this C ctrl, string? cls) where C : Control
	{
		ctrl.CssClass = ctrl.CssClass.AddCls(cls);
		return ctrl;
	}

	public static C DelCls<C>(this C ctrl, string? cls) where C : Control
	{
		ctrl.CssClass = ctrl.CssClass.DelCls(cls);
		return ctrl;
	}



	private static string? AddCls(this string? str, string? cls) => cls
		switch
		{
			null => str,
			not null => str switch
			{
				null => cls,
				not null => str.AddClsNotNull(cls)
			}
		};
	private static string? DelCls(this string? str, string? cls) => cls
		switch
		{
			null => str,
			not null => str switch
			{
				null => null,
				not null => str.DelClsNotNull(cls)
			}
		};

	private static string AddClsNotNull(this string str, string cls)
	{
		var parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return parts.Contains(cls) switch
		{
			true => str,
			false => parts.Append(cls).JoinText(" ")
		};
	}
	private static string DelClsNotNull(this string str, string cls)
	{
		var parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return parts.Contains(cls) switch
		{
			true => parts.Where(e => e != cls).JoinText(" "),
			false => str
		};
	}
}