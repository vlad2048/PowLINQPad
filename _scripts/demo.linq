<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>PowLINQPad.Settings_</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>PowLINQPad.Flex_</Namespace>
  <Namespace>PowLINQPad.Flex_.Structs</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
</Query>

enum Status
{
	First,
	Second,
	Third
}
record Filt(
	int? Id,
	BoolOpt BoolOpt,
	TxtSearch Text,
	RngInt RngInt,
	RngTime RngTime,
	Status[] Enums
)
{
	public static readonly Filt Empty = new(
		null,
		BoolOpt.None,
		TxtSearch.Empty,
		RngInt.Empty,
		RngTime.Empty,
		Array.Empty<Status>()
	);
}
class Sets : ISettings
{
	public Filt F { get; set; } = Filt.Empty;
}

void Main()
{
	var sets = Settings.Load<Sets>();
	
	var rxId = sets.Get(e => e.F.Id).D(D);
	var rxBoolOpt = sets.Get(e => e.F.BoolOpt).D(D);
	var rxText = sets.Get(e => e.F.Text).D(D);
	var rxRngInt = sets.Get(e => e.F.RngInt).D(D);
	var rxRngTime = sets.Get(e => e.F.RngTime).D(D);
	var rxEnums = sets.Get(e => e.F.Enums).D(D);
	
	CtrlOpt o(int? keyWidth, int? valWidth, string title) => new(title, keyWidth, valWidth);
	
	var uiId		= Ctrls.MkInt			(rxId,		o(null, null, "Id"		)).D(D);
	var uiBoolOpt	= Ctrls.MkBoolOpt		(rxBoolOpt,	o(null, null, "BoolOpt"	)).D(D);
	var uiText		= Ctrls.MkText			(rxText,	o(null, null, "Text"	), true).D(D);
	var uiRngInt	= Ctrls.MkRngInt		(rxRngInt,	o(null, null, "RngInt"	), new RngIntBounds(25, 50)).D(D);
	var uiRngTime	= Ctrls.MkRngTime		(rxRngTime,	o(null, null, "RngTime"	), RngTime.SampleTimes, 20).D(D);
	var uiEnums		= Ctrls.MkEnumMultiple	(rxEnums,	o(null, null, "Enums"	)).D(D);

	
	Flex.Vert(Dim.Fill, Dim.Fill,
	
		Flex.Horz(Dim.Fill, Dim.Auto,
			Flex.Vert(Dim.Auto, Dim.Auto,
				uiId,
				uiBoolOpt
			),
			uiText,
			uiRngInt
		),
		Flex.Horz(Dim.Fill, Dim.Auto,
			uiRngTime,
			uiEnums
		),
		
		Flex.Scroll(Dim.Fill, Dim.Fill,
			Enumerable.Range(0, 60).SelectToArray(e => new Span($"Item: {e}"))
		),
		
		Flex.Vert(Dim.Fill, Dim.Fix(200),
			rxId		.Select(e => $"Id: {e}"		).ToSpan(D),
			rxBoolOpt	.Select(e => $"BoolOpt: {e}").ToSpan(D),
			rxText		.Select(e => $"Text: [{e.Fmt()}]").ToSpan(D),
			rxRngInt	.Select(e => $"RngInt: {e}"	).ToSpan(D),
			rxRngTime	.Select(e => $"RngTime: {e}").ToSpan(D),
			rxEnums		.Select(e => $"Enums: {e}"	).ToSpan(D)
		)
	)
		.Build(true)
		.Dump();
}


static class FmtExt
{
	public static string Fmt(this TxtSearch e)
	{
		var sb = new StringBuilder();
		
		sb.Append($"useRegex:{e.UseRegex}");
		
		sb.Append($"  text:'{e.Text}'");
		
		sb.Append("  parts:");
		if (e.Parts == null)
			sb.Append("_");
		else
			sb.Append($"({e.Parts.JoinText(",")})");
			
		sb.Append("  regex:");
		if (e.Regex == null)
			sb.Append("_");
		else
			sb.Append("yes");

		sb.Append($"  isError:{e.IsError}");

		return sb.ToString();
	}
}


public static Disp D => RxUI.D;

void OnStart()
{
	RxUI.Start();
}
