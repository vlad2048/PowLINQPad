using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using LINQPad;
using LINQPad.Controls;
using PowBasics.StringsExt;
using PowLINQPad.Structs;
using PowLINQPad.UtilsInternal.Json_;

namespace PowLINQPad.Editing.Controls_;

// http://ionden.com/a/plugins/ion.rangeSlider/



public enum SliderType
{
    Single,
    Double,
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
    [JsonIgnore]
    public int? Width { get; set; } = 200;
    [JsonIgnore]
    public bool DumpOpt { get; set; }

    // Basic setup
    public SliderType Type { get; set; } = SliderType.Single;
    public int Min { get; set; } = 0;
    public int Max { get; set; } = 100;
    public int From { get; set; } = 0;      // Double: min init value (Single: init value)
    public int To { get; set; } = 100;  // Double: max init value

    // Advanced setup
    public int Step { get; set; } = 5;
    public bool Keyboard { get; set; } = true;

    // Grid setup
    public bool Grid { get; set; } = false;
    public bool GridMargin { get; set; } = true;
    public int GridNum { get; set; } = 4;
    public bool GridSnap { get; set; } = false;

    // Interval control
    public bool DragInterval { get; set; } = false;
    public int? MinInterval { get; set; }
    public int? MaxInterval { get; set; }

    // Handles control
    public bool FromFixed { get; set; } = false;
    public int FromMin { get; set; } = 0;
    public int FromMax { get; set; } = 100;
    public bool FromShadow { get; set; } = false;
    public bool ToFixed { get; set; } = false;
    public int ToMin { get; set; } = 0;
    public int ToMax { get; set; } = 100;
    public bool ToShadow { get; set; } = false;

    // UI control
    public Skin Skin { get; set; } = Skin.Flat;
    public bool HideMinMax { get; set; } = false;
    public bool HideFromTo { get; set; } = false;
    public bool ForceEdges { get; set; } = false;
    public string ExtraClasses { get; set; } = string.Empty;
    public bool Block { get; set; } = false;

    // Prettify numbers
    public bool PrettifyEnabled { get; set; } = true;
    public string PrettifySeparator { get; set; } = " ";
    public string? Prettify { get; set; }
    public string? Prefix { get; set; }
    public string? Postfix { get; set; }
    public string? MaxPostfix { get; set; }
    public bool DecorateBoth { get; set; } = true;
    public string ValuesSeparator { get; set; } = "—";

    // Data control
    public string InputValuesSeparator { get; set; } = ";";
    public bool Disable { get; set; } = false;

    // Callbacks
    public string? Scope { get; set; }
    [JsonPropertyName("onStart")]
    public string? OnStart { get; set; }
    [JsonPropertyName("onChange")]
    public string? OnChange { get; set; }
    [JsonPropertyName("onFinish")]
    public string? OnFinish { get; set; }
    [JsonPropertyName("onUpdate")]
    public string? OnUpdate { get; set; }


    private SliderOpt() { }
    public static SliderOpt Build(Action<SliderOpt>? optFun)
    {
        var opt = new SliderOpt();
        optFun?.Invoke(opt);
        return opt;
    }
}


public class CtrlSlider : Control
{
    private static int idCnt;
    private readonly int id;
    private readonly SliderOpt opt;
    private readonly ISubject<RngInt> whenChanged = new Subject<RngInt>();

    private string EltRootId => $"ionrange-root-{id}";
    private string EltId => $"ionrange-{id}";
    private string EltCls => $"ionrange-{id}-cls";

    public IObservable<RngInt> WhenChanged => whenChanged.AsObservable();

    public CtrlSlider(Action<SliderOpt>? optFun = null) : base("div")
    {
        id = idCnt++;
        opt = SliderOpt.Build(optFun);
        opt.ExtraClasses += $" {EltCls}";
        HtmlElement.ID = EltRootId;
        HtmlElement.InnerHtml = $"<input type='text' id='{EltId}' name='my_range' value='' style='display:none'/>";

        opt.OnChange = Utils.MkDispatchEvtFun(EltRootId);
        Util.HtmlHead.AddStyles(Utils.MkExtraStyles(EltCls, opt));
    }



    protected override void OnRendering(EventArgs e)
    {
        base.OnRendering(e);

        var optStr = Jsoners.IonSlider.Ser(opt).FixOptStr();
        if (opt.DumpOpt) optStr.Dump();

        void Handler(object? sender, PropertyEventArgs args)
        {
            var val = Utils.ReadValue(EltId, opt.Min, opt.Max);
            whenChanged.OnNext(val);
        }

        Task.Delay(TimeSpan.FromSeconds(0)).ContinueWith(_ =>
        {
            Util.InvokeScript(false, "eval", $"$('#{EltId}').ionRangeSlider({optStr})");
            HtmlElement.AddEventListener("rightClick", Array.Empty<string>(), Handler);
        });
    }



    internal static void Init()
    {
        Util.HtmlHead.AddCssLink("https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.3.1/css/ion.rangeSlider.min.css");
        Util.HtmlHead.AddScriptFromUri("https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js");
        Util.HtmlHead.AddScriptFromUri("https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.3.1/js/ion.rangeSlider.min.js");
    }
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

    public static string FixOptStr(this string str) => str.SplitInLines()
        .Select(e => e.Trim().StartsWith("\"on") switch
        {
            false => e,
            true => e.Replace("\"", "").Replace("\\t", "").Replace("\\r", "").Replace("\\n", "")
        })
        .JoinLines();
}