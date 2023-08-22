using System.Reactive.Linq;
using LINQPad.Controls;
using LINQPad;
using PowLINQPad.Interactions.Paging;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Interactions.ListScrolling;

public static class ListScroller
{
	public static TimeSpan JumpDelay { get; set; } = TimeSpan.FromMilliseconds(10);

	public static (DumpContainer, IDisp) ToJumpDC(this IObservable<Div[]> rxDivs, PagerState pagerState, IObservable<int> jump)
	{
		var d = new Disp();
		var dc = new DumpContainer();

		rxDivs.Subscribe(divs =>
		{
			for (var i = 0; i < divs.Length; i++) divs[i].AddCls($"idx-{i}");
			dc.UpdateContent(divs);
		}).D(d);

		jump
			.Subscribe(idx =>
			{
				var jmpPageIdx = idx / pagerState.PageSize;
				var jmpPageOfs = idx % pagerState.PageSize;
				pagerState.PageIndex.V = jmpPageIdx;
				Observable.Timer(JumpDelay).Subscribe(_ => JumpToIdx(jmpPageOfs));
			}).D(d);

		return (dc, d);
	}


	internal static void Init() => Util.HtmlHead.AddScript("""
		function jump(idx) {
			const elts = document.getElementsByClassName(`idx-${idx}`);
			if (!!elts && elts.length > 0) {
				const elt = elts[0];
				elt.scrollIntoView();
			}
		}
	""");
	private static void JumpToIdx(int idx) => Util.InvokeScript(true, "jump", idx);


	/*
	public static (Div, IDisp) DisplayScrollable(this IRoVar<Div[]> divsVar, IObservable<int> whenScroll)
	{
		var d = new Disp();
		var dc = new DumpContainer();
		var wrapDiv = new Div(dc);
		wrapDiv.CssClass = "dc-height";

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
	*/
}