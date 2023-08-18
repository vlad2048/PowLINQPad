using LINQPad;
using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI;

public static class CssExt
{
	/*public static C CssIf<C>(this C ctrl, bool condition, string css) where C : Control => condition switch
	{
		false => ctrl,
		true => ctrl.Css(css)
	};*/
	
	public static (C, IDisposable) Css<C>(this (C, IDisposable) ctrlDisp, string css) where C : Control
	{
		ctrlDisp.Item1.Css(css);
		return ctrlDisp;
	}

	public static C Css<C>(this C ctrl, string css) where C : Control
	{
		ctrl.CssClass.AddCls(GetClass(css));
		return ctrl;
	}

	public static (C, IDisp) CssIf<C>(this C ctrl, IRoVar<bool> isOn, string? cssOn, string? cssOff) where C : Control
	{
		var d = new Disp();

		var clsOn = GetClass(cssOn);
		var clsOff = GetClass(cssOff);

		isOn.Subscribe(v =>
		{
			switch (v)
			{
				case false:
					ctrl.CssClass = ctrl.CssClass.DelCls(clsOn).AddCls(clsOff);
					break;
				case true:
					ctrl.CssClass = ctrl.CssClass.DelCls(clsOff).AddCls(clsOn);
					break;
			}
		}).D(d);

		return (ctrl, d);
	}


	private static string? GetClass(string? css)
	{
		if (css == null) return null;
		string MkCls(int i) => $"cls-{i}";

		var id = clsMap.GetOrCreate(css, () =>
		{
			var i = clsId++;
			Util.HtmlHead.AddStyles($$"""
				.{{MkCls(i)}} {
					{{css}}
				}
			""");
			return i;
		});

		return MkCls(id);
	}


	private static string AddCls(this string str, string? cls) => cls
		switch
		{
			null => str,
			not null => str switch
			{
				null => cls,
				not null => str.AddClsNotNull(cls)
			}
		};
	private static string DelCls(this string str, string? cls) => cls
		switch
		{
			null => str,
			not null => str switch
			{
				null => cls,
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

	internal static void Init() => clsMap.Clear();



	private static int clsId;
	private static readonly Dictionary<string, int> clsMap = new();
}