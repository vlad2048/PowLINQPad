using LINQPad;
using LINQPad.Controls;

namespace PowLINQPad.Utils.Css_;

public static class FullScreenLayout
{
	public static void Init() => Util.HtmlHead.AddStyles("""
		html {
			height:			100%;
		}
		body {
			height:			100%;
			margin:			0;
		}
		#final {
			height:			100%;
			display:		flex;
			flex-direction:	column;
		}	
		
		.fullscreen-fix {
			flex-shrink:	0;
		}
		.fullscreen-fillscroll {
			overflow-y:		auto;
		}
	""");

	public static C FullScreenFix<C>(this C ctrl) where C : Control => ctrl.AddCls("fullscreen-fix");

	public static C FullScreenFillScroll<C>(this C ctrl) where C : Control => ctrl.AddCls("fullscreen-fillscroll");
}