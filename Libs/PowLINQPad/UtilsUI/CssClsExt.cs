using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI;

public static class CssClsExt
{
	public static C AddClsIf<C>(this C ctrl, string cls, bool condition) where C : Control => condition switch
	{
		false => ctrl,
		true => ctrl.AddCls(cls)
	};
	public static C AddCls<C>(this C ctrl, params string?[] clss) where C : Control
	{
		foreach (var cls in clss)
			ctrl.AddClsInternal(cls);
		return ctrl;
	}
	public static C DelCls<C>(this C ctrl, params string?[] clss) where C : Control
	{
		foreach (var cls in clss)
			ctrl.DelClsInternal(cls);
		return ctrl;
	}

	public static C SetCls<C>(this C ctrl, params string?[] clss) where C : Control
	{
		ctrl.CssClass = clss.Where(e => e != null).JoinText(" ");
		return ctrl;
	}

	public static string Flag(this string cls, bool on) => on switch
	{
		false => $"{cls}-off",
		true => $"{cls}-on"
	};
	public static C SetClsFlag<C>(this C ctrl, string cls, bool on) where C : Control => ctrl.SetCls(cls, cls.Flag(on));





	private static C AddClsInternal<C>(this C ctrl, string? cls) where C : Control
	{
		ctrl.CssClass = ctrl.CssClass.AddCls(cls);
		return ctrl;
	}

	private static C DelClsInternal<C>(this C ctrl, string? cls) where C : Control
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