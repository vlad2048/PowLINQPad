using LINQPad.Controls;
using PowLINQPad.Utils.Css_.Utils;

namespace PowLINQPad.Utils.Css_;

public static class CssApplyExt
{
	public static C Css<C>(this C ctrl, string css) where C : Control
	{
		foreach (var (key, val) in CssUtils.ParseCss(css))
		{
			if (key == "display" && ctrl.Styles[key] == "none") continue;
			ctrl.Styles[key] = val;
		}
		return ctrl;
	}

	public static C fk<C>(this C c, string frontColor) where C : Control => c.Css($"color: {frontColor}");
	public static C bk<C>(this C c, string backColor) where C : Control => c.Css($"background-color: {backColor}");
}