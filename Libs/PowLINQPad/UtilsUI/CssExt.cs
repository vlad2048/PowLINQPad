using LINQPad;
using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI;

public static class CssExt
{
	public static C CssIf<C>(this C ctrl, bool condition, string css) where C : Control => condition switch
	{
		false => ctrl,
		true => ctrl.Css(css)
	};
	
	public static (C, IDisposable) Css<C>(this (C, IDisposable) ctrlDisp, string css) where C : Control
	{
		ctrlDisp.Item1.Css(css);
		return ctrlDisp;
	}

	public static C Css<C>(this C ctrl, string css) where C : Control
	{
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
		ctrl.CssClass += $" {MkCls(id)}";
		return ctrl;
	}




	internal static void Init() => clsMap.Clear();



	private static int clsId;
	private static readonly Dictionary<string, int> clsMap = new();
}