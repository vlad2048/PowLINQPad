<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.Flex_</Namespace>
  <Namespace>PowBasics.StringsExt</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;

const string Filename = @"D:\tmp\fill\index.html";

void Main()
{
	var arr = Enumerable.Range(0, 50).Select(e => $"line {e}");
	var text = arr.JoinLines();
	
	Util.HtmlHead.AddStyles(Styles);
	//MkDiv("header").SetCls("header").Dump();
	//MkDiv(text).SetCls("main").Dump();
	//HtmlExporter.OpenInBrowser();
	//Dbg();
	
	var rnd = new Random((int)DateTime.Now.Ticks);
	
	var content = Var.Make(Array.Empty<string>()).D(D);
	
	new Div(
		new Button("Change", _ => content.V = Enumerable.Range(0, rnd.Next(10, 30)).SelectToArray(e =>
			$"item {e}"
		)),
		new Button("Browser", _ => HtmlExporter.OpenInBrowser())
	).SetCls("header").Dump();
	
	new Div(
		content.ToDC(D)
	).SetCls("main").Dump();
	
	MkDiv("Footer").SetCls("footer").Dump();
}


const string Styles = """
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
	
	.main {
		overflow-y:		auto;
	}




	.header {
		background-color:	#333;
	}
	.footer {
		background-color:	purple;
		height:				60px;
		flex-shrink:		0;
	}
	
	.main {
		background-color:	dodgerblue;
	}
""";

static void Dbg()
{
	var html = ((string)Util.InvokeScript(true, "eval", "document.body.innerHTML")).BeautifyHtml();
	//html.Dump();
	File.WriteAllText(Filename, html);
}

static Div MkDiv(string text)
{
	var div = new Div();
	div.HtmlElement.InnerText = text;
	return div;
}


static class Ext
{
	public static DumpContainer ToDC<T>(this IObservable<T> obs, IRoDispBase d)
	{
		var dc = new DumpContainer();
		obs.Subscribe(v => dc.UpdateContent(v)).D(d);
		return dc;
	}
}


public static Disp D => RxUI.D;
void OnStart()
{
	RxUI.Start();
	Util.HtmlHead.AddStyles("""
		body {
			font-family: Consolas;
		}
	""");
}
