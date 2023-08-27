using PowBasics.StringsExt;
using System.Text.Json.Serialization;
using LINQPad;
using PowLINQPad.UtilsInternal.Json_;

namespace PowLINQPad.Editing.Controls_.Slider_;

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
	public string? Label { get; set; }
    [JsonIgnore]
    public int? Width { get; set; } = 400;
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
    [JsonIgnore]
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
    [JsonIgnore]
    public string? OnStart { get; set; }
    [JsonIgnore]
    public string? OnChange { get; set; }
    [JsonIgnore]
    public string? OnFinish { get; set; }
    [JsonIgnore]
    public string? OnUpdate { get; set; }


    private SliderOpt() { }
    public static SliderOpt Build(Action<SliderOpt>? optFun)
    {
        var opt = new SliderOpt();
        optFun?.Invoke(opt);
        return opt;
    }
}



static class SliderOptExt
{
	public static string Serialize(this SliderOpt opt)
	{
		var json = Jsoners.IonSlider.Ser(opt)
			.AddHandler("prettify", opt.Prettify)
			.AddHandler("onStart", opt.OnStart)
			.AddHandler("onChange", opt.OnChange)
			.AddHandler("onFinish", opt.OnFinish)
			.AddHandler("onUpdate", opt.OnUpdate);
		if (opt.DumpOpt) json.Dump();
		return json;
	}

	private static string AddHandler(this string json, string name, string? code)
	{
		if (code == null) return json;
		var lines = json.SplitInLines().ToList();
		lines[^2] += ",";
		var add = code.SplitInLines().Select(e => $"      {e}").Prepend($"    \"{name}\":");
		lines.InsertRange(lines.Count - 1, add);
		return lines.JoinLines();
	}
}