using LINQPad;

namespace PowLINQPad;

static class Styles
{
	public static void Init() => Util.HtmlHead.AddStyles("""
		@import url('https://fonts.googleapis.com/css2?family=Inter&display=swap');
	
		* {
			box-sizing: border-box;
		}


		body {
			background-color:	#1d2125;
			font-family:		Inter;
			font-size:			14px;
			line-height:		21px;
			color:				#ffffff;
		}
		button {
			font-family:		inherit;
			color:				inherit;
			background-color:	#0066ff;
			border:				1px solid #15181b;
			border-radius:		6px;
			padding:			6px 8px 6px 8px;
			cursor:				pointer;
		}
		button:hover {
			background-color:	#0052cc;
		}
		button::after {
			content:			"";
			display:			inline-block;
			margin-left:		7px;
			vertical-align:		2px;
			border-top:			4px solid;
			border-left:		4px solid transparent;
			border-right:		4px solid transparent;
			border-bottom:		0;
		}
	""");
}