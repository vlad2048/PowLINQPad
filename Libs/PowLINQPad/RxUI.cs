using PowLINQPad.Flex_;
using PowLINQPad.Flex_.Utils;
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
		CssExt.Init();
		//Flex.Init();
		PaletteUtils.Init();
	}

	private static readonly SerialDisp<Disp> serD = new();

}