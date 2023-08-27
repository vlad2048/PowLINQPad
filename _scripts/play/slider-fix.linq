<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowLINQPad.Editing._Base</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>PowLINQPad.UtilsInternal.Json_</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowBasics.StringsExt</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	/*var slider = new CtrlSlider(opt =>
	{
		opt.Skin = Skin.Sharp;
	});
	slider.Dump();
	slider.RxVar.ToDC(D).Dump();*/
	
	Util.HtmlHead.AddStyles("""
		body {
			font-family: Consolas;
		}
	""");
	
	var str = """
		{
		  "min": 0,
		  "max": 100,
		  "from": 0,
		  "to": 100,
		  "step": 5,
		  "skin": "sharp"
		}
	""";
	var json = str.AddHandler("onChange", """
		function (data) {
			external.log('changed');
		}
	""");
	json.Dump();
}


static class JsonExt
{
	public static string AddHandler(this string json, string name, string code)
	{
		var lines = json.SplitInLines().ToList();
		lines[lines.Count - 2] = lines[lines.Count - 2] + ",";
		var add = code.SplitInLines().Select(e => $"      {e}").Prepend($"    \"{name}\":");
		lines.InsertRange(lines.Count - 1, add);
		return lines.JoinLines();
	}
}





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
        HtmlElement.ID = EltRootId;
        HtmlElement.InnerHtml = $"<input type='text' id='{EltId}' name='my_range' value='' style='display:none'/>";
        CssClass = Css.ClsRoot;

        //$"id:{id}  xtra:{ExtraCls}  ID:{EltRootId}  EltId:{EltId}".Dump();

        opt.OnChange = MkDispatchEvtFun(EltRootId);
		Util.HtmlHead.AddStyles(Css.Styles);
    }


	public static string MkDispatchEvtFun(string eltRootId) => $$"""
		function (data) {
			external.log('changed');
		}
	""";

    /*public static string MkDispatchEvtFun(string eltRootId) => $$"""
		function (data) {
			const valMin = data.from;
			const valMax = data.to;
			const elt = document.getElementById('{{eltRootId}}');
			elt.dispatchEvent (new CustomEvent ('rightClick', { detail: [valMin, valMax] }));
		}
	""";*/


    protected override void OnRendering(EventArgs e)
    {
        base.OnRendering(e);

        var optStr = Jsoners.IonSlider.Ser(opt);
		
		optStr.Dump();

        void Handler(object? sender, PropertyEventArgs args)
        {
            var val = Utils.ReadValue(EltId, opt.Min, opt.Max);
			rxVar.SetInner(val);
        }

		bool Success()
		{
			var obj = Util.InvokeScript(true, "eval", $$"""
				$('#{{EltId}}').ionRangeSlider({{optStr}});
				!!document.getElementById('{{EltId}}');
			""");
			obj.Dump();
			return obj != null && obj is string && bool.Parse((string)obj);
		}

		Utils.RetryUntilSuccess(Success)
			.Subscribe(_ =>
			{
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
	public static IObservable<Unit> RetryUntilSuccess(Func<bool> successFun) =>
		Obs.Interval(TimeSpan.FromMilliseconds(0))
			.Take(100)
			.Select(_ => successFun())
			.TakeUntil(e => e)
			.Where(e => e)
			.Take(1)
			.ToUnit();

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


    public static string FixOptStr(this string str) => str.SplitInLines()
        .Select(e => e.IsEventLine() switch
        {
            false => e,
            true => e.Replace("\"", "").Replace("\\t", "").Replace("\\r", "").Replace("\\n", "")
        })
        .JoinLines();

    private static bool IsEventLine(this string s)
    {
		s = s.Trim();
		return
			s.StartsWith("\"on") ||
			s.StartsWith("\"prettify\"");
    }
}











public enum Skin
{
    Flat,
    Big,
    Modern,
    Sharp,
    Round,
    Square,
}

public class SliderOpt
{
    public int Min { get; set; } = 0;
    public int Max { get; set; } = 100;
    public int From { get; set; } = 0;      // Double: min init value (Single: init value)
    public int To { get; set; } = 100;  // Double: max init value
    public int Step { get; set; } = 5;
    public Skin Skin { get; set; } = Skin.Flat;

    // Callbacks
    [JsonIgnore]
    public string? OnChange { get; set; }
	
    //[JsonPropertyName("onUpdate")]
    //public string? OnUpdate { get; set; }


    private SliderOpt() { }
    public static SliderOpt Build(Action<SliderOpt>? optFun)
    {
        var opt = new SliderOpt();
        optFun?.Invoke(opt);
        return opt;
    }
}






static class RxExt
{
	public static IObservable<T> WhenOuterOrInit<T>(this IRwBndVar<T> rxVar) => rxVar.WhenOuter.Prepend(rxVar.V);
}




public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
