using LINQPad.Controls;
using LINQPad;

namespace PowLINQPad.Controls_;

public class FillDiv : Control
{
	public FillDiv(params Control[] kids) : base("div", kids)
	{
		Util.HtmlHead.AddScript("""
			function fixFill(eltId) {
				let elt = document.getElementById(eltId);
				if (!!elt) {
					const ofsTop = elt.offsetTop;
					const winHeight = window.innerHeight;
					const filHeight = Math.max(0, winHeight - ofsTop - 10);
					//external.log(`ofsTop=${ofsTop}  winHeight=${winHeight}  filHeight=${filHeight}  elt.style.height=${elt.style.height}  elt.style.overflowY=${elt.style.overflowY}`);
					elt.style.height = `${filHeight}px`;
					elt.style.overflowY = 'auto';
				}
			}
		""");
	}

	protected override void OnRendering(EventArgs e)
	{
		base.OnRendering(e);

		Obs.Interval(TimeSpan.FromMilliseconds(1000))
			.Subscribe(_ =>
			{
				Util.InvokeScript(false, "fixFill", HtmlElement.ID);
			});
	}
}