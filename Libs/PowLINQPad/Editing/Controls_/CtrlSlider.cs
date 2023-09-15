using LINQPad;
using LINQPad.Controls;
using PowLINQPad.Editing._Base;
using PowLINQPad.Editing.Controls_.Slider_;
using PowLINQPad.Structs;
using PowLINQPad.Utils.Ctrls_;
using PowLINQPad.UtilsInternal;

namespace PowLINQPad.Editing.Controls_;



// http://ionden.com/a/plugins/ion.rangeSlider/
public class CtrlSlider : Control, IBoundCtrl<RngInt>
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

    private static int idCnt;
    private readonly int id;
    private readonly SliderOpt opt;
    private readonly IFullRwBndVar<RngInt> rxVar;

    private string EltRootId => $"ionrange-root-{id}";
    private string EltId => $"ionrange-{id}";
    private string ExtraCls => $"ionrange-{id}-extra-cls";

    public IRwBndVar<RngInt> RxVar => rxVar.ToRwBndVar();

    public CtrlSlider(Action<SliderOpt>? optFun = null) : base("div")
    {
	    rxVar = Var.MakeBnd(RngInt.Empty).D(d);

		id = idCnt++;
        opt = SliderOpt.Build(optFun);
        opt.ExtraClasses += $" {ExtraCls}";
        HtmlElement.ID = EltRootId;
        HtmlElement.InnerHtml = $"<span class='{Css.ClsLabel}'>{opt.Label}</span><input type='text' id='{EltId}' name='my_range' value='' style='display:none'/>";
        CssClass = Css.ClsRoot;

        opt.OnChange = Utils.MkDispatchEvtFun(EltRootId);
        Util.HtmlHead.AddStyles(Utils.MkExtraStyles(ExtraCls, opt));
		Util.HtmlHead.AddStyles(Css.Styles);
    }



    protected override void OnRendering(EventArgs e)
    {
        base.OnRendering(e);

        void Handler(object? sender, PropertyEventArgs args)
        {
            var val = Utils.ReadValue(EltId, opt.Min, opt.Max);
			rxVar.SetInner(val);
        }

		this.WhenMounted()
			.Subscribe(_ =>
			{
				var optStr = opt.Serialize();
				Util.InvokeScript(true, "eval", $"$('#{EltId}').ionRangeSlider({optStr})");

				HtmlElement.AddEventListener("rightClick", Array.Empty<string>(), Handler);

				RxVar.WhenOuterOrInit().Subscribe(v =>
				{
					var minIdx = v.Min.HasValue switch
					{
						true => v.Min.Value,
						false => opt.Min,
					};
					var maxIdx = v.Max.HasValue switch
					{
						true => v.Max.Value,
						false => opt.Max,
					};
					Util.InvokeScript(false, "eval", $$"""$('#{{EltId}}').data('ionRangeSlider').update({from:{{minIdx}}, to:{{maxIdx}}})""");
				}).D(d);
			}).D(d);
    }


    internal static void Init()
    {
        Util.HtmlHead.AddCssLink("https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.3.1/css/ion.rangeSlider.min.css");
        Util.HtmlHead.AddScriptFromUri("https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js");
        Util.HtmlHead.AddScriptFromUri("https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.3.1/js/ion.rangeSlider.min.js");
    }
}


file static class Css
{
	public const string ClsRoot = "slider-root";
	public const string ClsLabel = "slider-label";

	public const string Styles = $$"""
		.{{ClsRoot}} {
			padding:			26px 12px 0px 12px;
			background-color:	#2d3239;
			position:			relative;
			border:				1px solid #15181b;
			border-radius:		6px;
		}
		.{{ClsLabel}} {
			position:			absolute;
			left:				50%;
			top:				5px;
			transform:			translateX(-50%);
		}
	""";
}





file static class Utils
{
    public static RngInt ReadValue(string eltId, int bndMin, int bndMax)
    {
        var valMinObj = Util.InvokeScript(true, "eval", $"$('#{eltId}').data('from')");
        var valMaxObj = Util.InvokeScript(true, "eval", $"$('#{eltId}').data('to')");
        if (valMinObj is not string valMinStr || valMaxObj is not string valMaxStr) throw new ArgumentException("Failed to read value from IonSlider");
        if (!int.TryParse(valMinStr, out var min) || !int.TryParse(valMaxStr, out var max)) throw new ArgumentException("Failed to read value from IonSlider (expected ints)");
        return new RngInt(
            min == bndMin ? null : min,
            max == bndMax ? null : max
        );
    }

    public static string MkDispatchEvtFun(string eltRootId) => $$"""
		function (data) {
			const valMin = data.from;
			const valMax = data.to;
			const elt = document.getElementById('{{eltRootId}}');
			elt.dispatchEvent (new CustomEvent ('rightClick', { detail: [valMin, valMax] }));
		}
	""";

    public static string MkExtraStyles(string eltCls, SliderOpt opt) => $$"""
		.{{eltCls}} {
			{{(opt.Width.HasValue ? $"width: {opt.Width}px" : "")}}
		}
	""";
}