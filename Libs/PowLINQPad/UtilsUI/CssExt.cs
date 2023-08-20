using LINQPad;
using LINQPad.Controls;

namespace PowLINQPad.UtilsUI;


public static class CssVar
{
	public static string Make(string name, string val)
	{
		Util.HtmlHead.AddStyles($$"""
			:root {
				--{{name}}: {{val}};
			}
		""");
		return $"var(--{name})";
	}
}


public static class CssExt
{
	public static C Css<C>(this C ctrl, string css) where C : Control
	{
		foreach (var (key, val) in CssUtils.ParseCss(css))
			ctrl.Styles[key] = val;
		return ctrl;
	}


	
	
	public static (C, IDisposable) CssCls<C>(this (C, IDisposable) ctrlDisp, string css) where C : Control
	{
		ctrlDisp.Item1.CssCls(css);
		return ctrlDisp;
	}


	public static C CssCls<C>(this C ctrl, string css) where C : Control =>
		ctrl.AddCls(CssUtils.GetClass(css));


	public static (C, IDisp) CssClsIf<C>(this C ctrl, IRoVar<bool> isOn, string? cssOn, string? cssOff) where C : Control
	{
		var d = new Disp();

		var clsOn = CssUtils.GetClass(cssOn);
		var clsOff = CssUtils.GetClass(cssOff);

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
}