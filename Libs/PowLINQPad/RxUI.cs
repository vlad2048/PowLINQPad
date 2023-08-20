using PowLINQPad.Flex_.Utils;
using PowLINQPad.Interactions.ListScrolling;
using PowLINQPad.Settings_;
using PowLINQPad.UtilsUI;

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
		PaletteUtils.Init();
		ListScroller.Init();
	}

	private static readonly SerialDisp<Disp> serD = new();

}