<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowBasics.StringsExt</Namespace>
  <Namespace>LINQPad.Controls.Core</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	//var ctrl = new TextBox("abcdef");
	//ctrl.Dump();
	
	/*Util.HtmlHead.AddStyles("""
		.textinput-root {
			display: flex;
			color: #d7d8db;
		}
		.textinput-title {
			background-color: #2d3239;
			padding: 4px 8px;
			border: 1px solid #15181b;
			border-bottom-left-radius: 6px;
			border-top-left-radius: 6px;
			border-right: none;
		}
		.textinput-input {
			border: 1px solid #15181b;
			border-bottom-right-radius: 6px;
			border-top-right-radius: 6px;
			//appearance: none;
			margin: 0;
			appearance: none;
		}
		.textinput-input:focus {
			//outline: 1px solid red !important;
			outline: none;
			border: 1px solid #10366E;
		}
	""");
	
	Util.RawHtml("""
		<div class='textinput-root'>
			<span class='textinput-title'>Id:</span>
			<input class='textinput-input' value='abcdef'>
		</div>
	""").Dump();*/
	
	
	var ctrl = new CtrlTextBox("Search");
	var dc = ctrl.Value.ToDC(D);
	var btn = new Button("Set", _ => ctrl.Value.V = "def123");
	
	Util.VerticalRun(
		ctrl,
		dc,
		btn
	).Dump();
}



public class CtrlTextBox : Control, IDisp
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();
	
	private static int idCnt;
	private readonly int id;
	private readonly IFullRwBndVar<string> rxVal;
	
	public IRwBndVar<string> Value => rxVal.ToRwBndVar();
	
	public CtrlTextBox(string text) : base("div")
	{
		id = idCnt++;
		HtmlElement.ID = $"textinput-{id}";
		this.SetCls(Css.ClsRoot);
		Control ctrlInput;
		VisualTree.AddRange(new []
		{
			new Span(text).SetCls(Css.ClsTitle),
			ctrlInput = new Control("input").SetCls(Css.ClsInput)
		});
		Util.HtmlHead.AddStyles(Css.Styles);
		
		
		rxVal = Var.MakeBnd(string.Empty).D(d);
		
		ctrlInput.WhenEvent("input", d).Subscribe(_ =>
		{
			var textObj = ctrlInput.HtmlElement.InvokeScript(true, "eval", "targetElement.value");
			if (textObj is not string text) return;
			rxVal.SetInner(text);
		}).D(d);

		rxVal.WhenOuter.Subscribe(v => ctrlInput.HtmlElement.InvokeScript(true, "eval", $"targetElement.value = '{v}'")).D(d);
	}
}


file static class Css
{
	public const string ClsRoot = "textinput-root";
	public const string ClsTitle = "textinput-title";
	public const string ClsInput = "textinput-input";

	public const string Styles = $$"""
		.{{ClsRoot}} {
			display:					flex;
			color:						#d7d8db;
		}
		.{{ClsTitle}} {
			background-color:			#2d3239;
			padding:					4px 8px;
			border:						1px solid #15181b;
			border-bottom-left-radius:	6px;
			border-top-left-radius:		6px;
			border-right:				none;
		}
		.{{ClsInput}} {
			border:						1px solid #15181b;
			border-bottom-right-radius:	6px;
			border-top-right-radius:	6px;
			margin:						0;
			appearance: 				none;
		}
		.{{ClsInput}}:focus {
			outline:					none;
			border:						1px solid #10366E;
		}
	""";
}






public static Disp D => RxUI.D;
void OnStart()
{
	RxUI.Start();
	
	Util.HtmlHead.AddStyles("""
		input {
			font-family:		inherit;
			color:				inherit;
			background-color:	#24282e;
			appearance:			none;
		}
	""");
}








