<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.Editing.Controls_</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	Util.HorizontalRun(true,
		new CtrlDropdownButton("Button", new[] { "One", "Two", "Three" }, true),
		new Span("Btn")
	)
	.Dump();
	
	HtmlExporter.OpenInBrowser();
	
	/*Util.HtmlHead.AddStyles("""
		.btn::after {
			content: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADJSURBVDhPpZPLDcIwEEQXRAOIC3eO0Es6gU7oifTAEQkK4TPPwtF6YyKjjPQSe72ziX8Lq4v4XuxSz+wmruKdehNaiZN4CJI9xI6CnKrWohfRGLkIcgtRtcWcoUjxJ/y2T3iGfi3GdJJYMD9nEjfi7GK0ifkid5E24fANeDAwyNu3Yx5e61zAk42/zNAt9Zit2VNg0C/iS2yFN9COi4gHb1LcRor4PsRtxDPo34NE7uhItx5lckZHOYuqU5eJseLLwyIEEW+4zmYftaWN5Q68zjIAAAAASUVORK5CYII=')
			
		}
	""");
	
	var btn = new Button("Button");
	btn.CssClass = "btn";
	//btn.Styles["padding-right"] = "50px";
	
	btn.Dump();*/
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
