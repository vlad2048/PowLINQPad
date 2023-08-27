/*
using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI.Css_;

// class='xxx'
public sealed record Cls(string C);

// class='xxx xxx-on'
// class='xxx xxx-off'
public sealed record ClsFlag(string C);

// class='xxx'
// class='xxx-active'
public sealed record ClsActive(string C);


public static class ClsExt
{
	public static C SetCls<C>(this C ctrl, Cls cls) where C : Control
	{
		ctrl.CssClass = cls.C;
		return ctrl;
	}

	public static C SetClsFlag<C>(this C ctrl, Cls cls, bool on) where C : Control => ctrl
		.AddClsStr(cls.C)
		.AddClsStr(on ? $"{cls.C}-on" : $"{cls.C}-off")
		.DelClsStr(on ? $"{cls.C}-off" : $"{cls.C}-on");


	private static C AddClsStr<C>(this C ctrl, string cls) where C : Control
	{
		var ctrlClass = ctrl.CssClass ?? string.Empty;
		var parts = ctrlClass.Chop(' ');
		ctrl.CssClass = parts.Contains(cls) switch
		{
			true => ctrlClass,
			false => parts.Append(cls).JoinText(" ")
		};
		return ctrl;
	}

	private static C DelClsStr<C>(this C ctrl, string cls) where C : Control
	{
		var ctrlClass = ctrl.CssClass ?? string.Empty;
		var parts = ctrlClass.Chop(' ');
		ctrl.CssClass = parts.Contains(cls) switch
		{
			true => parts.Where(e => e != cls).JoinText(" "),
			false => ctrlClass
		};
		return ctrl;
	}
}
*/