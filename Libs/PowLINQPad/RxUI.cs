using PowLINQPad.Editing.Controls_;
using PowLINQPad.Interactions.ListScrolling;
using PowLINQPad.Settings_;
using PowLINQPad.Utils.Css_;
using PowLINQPad.Utils.Css_.Utils;

namespace PowLINQPad;

public static class RxUI
{
	public static Disp D => serD.Value!;

	public static void Start()
	{
		serD.Value = null;
		serD.Value = new Disp();
		Settings.Init();
		CssVars.Init();
		CssUtils.Init();
		ListScroller.Init();
		CtrlSlider.Init();
		Styles.Init();
	}

	private static readonly SerialDisp<Disp> serD = new();
}