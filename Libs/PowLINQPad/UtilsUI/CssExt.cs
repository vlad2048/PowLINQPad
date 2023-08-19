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


	public static C Css<C>(this C ctrl, string css) where C : Control =>
		ctrl.AddCls(GetClass(css));


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
					ctrl.DelCls(clsOn).AddCls(clsOff);
					break;
				case true:
					ctrl.DelCls(clsOff).AddCls(clsOn);
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


	
	
	internal static void Init() => clsMap.Clear();



	private static int clsId;
	private static readonly Dictionary<string, int> clsMap = new();
}