using System.Reactive.Linq;
using LINQPad.Controls;
using LINQPad;
using PowLINQPad.UtilsUI;
using PowLINQPad.Editing._Base;

namespace PowLINQPad.Editing.Controls_;

public class CtrlTextBox : Control, IBoundCtrl<string>
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();
	
	private static int idCnt;
	private readonly IFullRwBndVar<string> rxVar;
	private readonly Control ctrlInput;
	
	public IRwBndVar<string> RxVar => rxVar.ToRwBndVar();
	
	public CtrlTextBox(string text) : base("div")
	{
		var id = idCnt++;
		HtmlElement.ID = $"textinput-{id}";
		this.SetCls(Css.ClsRoot);
		VisualTree.AddRange(new []
		{
			new Span(text).SetCls(Css.ClsTitle),
			ctrlInput = new Control("input").SetCls(Css.ClsInput)
		});
		Util.HtmlHead.AddStyles(Css.Styles);
		ctrlInput.HtmlElement.SetAttribute("type", "search");

		
		rxVar = Var.MakeBnd(string.Empty).D(d);
	}

	protected override void OnRendering(EventArgs e)
	{
		base.OnRendering(e);

		Task.Delay(0).ContinueWith(_ =>
		{
			ctrlInput.WhenEvent("input", d).Subscribe(_ =>
			{
				var textObj = ctrlInput.HtmlElement.InvokeScript(true, "eval", "targetElement.value");
				if (textObj is not string textStr) return;
				rxVar.SetInner(textStr);
			}).D(d);

			RxVar.WhenOuterOrInit().Subscribe(v => ctrlInput.HtmlElement.InvokeScript(true, "eval", $"targetElement.value = '{v}'")).D(d);
		});
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
			width:						100%;
		}
		.{{ClsInput}} {
			border:						1px solid #15181b;
			border-bottom-right-radius:	6px;
			border-top-right-radius:	6px;
			margin:						0;
			appearance: 				searchfield-cancel-button;
		}
		.{{ClsInput}}:focus {
			outline:					none;
			border:						1px solid #10366E;
		}
	""";
}
