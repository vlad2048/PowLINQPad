using LINQPad;

namespace PowLINQPad.Flex_;

static class FlexCssReset
{
	public static void Init() => Util.HtmlHead.AddStyles("""
		* {
			box-sizing: border-box;
		}
		html, body, #final {
			height: 100%;
			padding: 0;
			margin: 0;
		}
		#final>div {
			height: 100%;
		}
		html, body, #final {
			height: 100%;
			padding: 0;
			margin: 0;
		}
		#final>div {
			height: 100%;
		}
	""");
}