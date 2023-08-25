using LINQPad.Controls;
using LINQPad;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Editing.Controls_;

public class CtrlTextBox : Control, IDisp
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();
	
	private static int idCnt;
	private readonly IFullRwBndVar<string> rxVal;
	
	public IRwBndVar<string> Value => rxVal.ToRwBndVar();
	
	public CtrlTextBox(string text) : base("div")
	{
		var id = idCnt++;
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
			if (textObj is not string textStr) return;
			rxVal.SetInner(textStr);
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
