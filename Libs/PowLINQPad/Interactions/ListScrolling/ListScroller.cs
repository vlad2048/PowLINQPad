using LINQPad.Controls;
using LINQPad;
using PowBasics.CollectionsExt;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Interactions.ListScrolling;

public static class ListScroller
{
	public static (Div, IDisp) DisplayScrollable(this IRoVar<Div[]> divsVar, IObservable<int> whenScroll)
	{
		var d = new Disp();
		var dc = new DumpContainer();
		var wrapDiv = new Div(dc);

		divsVar.Subscribe(divs => dc.UpdateContent(new Div(divs.DisplayScrollable(whenScroll).D(d)))).D(d);
		
		return (wrapDiv, d);
	}
	
	private static (Control[], IDisp) DisplayScrollable(this IReadOnlyList<Div> divs, IObservable<int> whenScroll)
	{
		var d = new Disp();
		if (divs.Count > 0)
		{
			var anchorSpan = new Span("anchor").Css("display:none");
			divs[0].Children.Insert(0, anchorSpan);
			
			whenScroll.Subscribe(idx =>
			{
				anchorSpan.HtmlElement.InvokeScript(false, "eval", $$"""
					// going from SPAN -> TBODY
					// then get the correct TR child in TBODY
					//const parent = targetElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement;
					const parent = targetElement.parentElement.parentElement;
					if (parent.children.length > 0 && {{idx}} < parent.children.length) {
						const focusIdx = Math.max(0, {{idx}} - 1);
						const elt = parent.children[focusIdx];
						elt.scrollIntoView();
					}
					"""
				);
			}).D(d);
		}
		return (divs.SelectToArray(e => (Control)e), d);
	}
}