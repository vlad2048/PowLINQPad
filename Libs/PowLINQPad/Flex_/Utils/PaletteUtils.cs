using PowBasics.CollectionsExt;
using PowBasics.ColorCode;

namespace PowLINQPad.Flex_.Utils;

static class PaletteUtils
{
	private static string[] palette = null!;
	private static int paletteIdx;
	
	public static string GetNextColor()
	{
		var col = palette[paletteIdx];
		paletteIdx++;
		if (paletteIdx >= palette.Length)
			paletteIdx = palette.Length - 1;
		return col;
	}

	internal static void Init()
	{
		palette =
			ColorUtils.MakePalette(8, 23235)
				.SelectToArray(e => $"#{e.R:X2}{e.G:X2}{e.B:X2}");
		paletteIdx = 0;
	}
}