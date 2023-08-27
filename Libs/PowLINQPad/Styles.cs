using LINQPad;

namespace PowLINQPad;

static class Styles
{
	public static void Init() => Util.HtmlHead.AddStyles("""
		@import url('https://fonts.googleapis.com/css2?family=Inter&display=swap');

		:root {
			--colorPageBk:			#1d2125;
			--colorPageFk:			#ffffff;

			--colorButtonBk:		#0066ff;
			--colorButtonHoverBk:	#0052cc;
			--borderCtrl:			1px solid #15181b;
		}
	
		* {
			box-sizing: border-box;
		}


		body {
			background-color:	var(--colorPageBk);
			color:				var(--colorPageFk);
			font-family:		Inter;
			font-size:			14px;
			line-height:		21px;
		}
		button {
			font-family:		inherit;
			color:				inherit;
			background-color:	var(--colorButtonBk);
			border:				var(--borderCtrl);
			border-radius:		6px;
			padding:			6px 8px 6px 8px;
			cursor:				pointer;
		}
		button:hover {
			background-color:	var(--colorButtonHoverBk);
		}
	""");
}