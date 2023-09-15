<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.Editing</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.Editing.Controls_</Namespace>
  <Namespace>PowLINQPad.Flex_</Namespace>
  <Namespace>PowLINQPad.Editing.Layout_</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;

const string TimeRangeEditParamName = "AllTimes";

void Main()
{
	var ps = new Dictionary<string, object>
	{
		{ TimeRangeEditParamName, RngTimeSampleData.Data }
	};
	
	var filter = Var.MakeBnd(Rec.Empty).D(D);
	
	filter.MakeEditor(D, ps);
	
	/*Util.VerticalRun(
		ui,
		filter.Select(e => Util.Pivot(e)).ToDC(D),
		Util.HorizontalRun(true,
			new Button("Export", _ => HtmlExporter.OpenInBrowser())
		)
	).Dump();*/
	
	Util.VerticalRun(
		filter.Select(e => Util.Pivot(e)).ToDC(D),
		new Button("Export", _ => HtmlExporter.OpenInBrowser())
	)
		.Dump();
}



enum State
{
	One,
	Two,
	Three,
}

static class Tabs
{
	public const string Main = "Main";
	public const string Flags = "Flags";
}

record Rec(

	[property:Pos(Tabs.Main, 0)]
	[property:SingleEnumEdit]
	State Single,
	
	[property:Pos(Tabs.Main, 1)]
	[property:MultipleEnumEdit]
	State[] Multiple,
	
	[property:Pos(Tabs.Main, 1)]
	[property:IntListEdit]
	int[] Ids,
	
	[property:Pos(Tabs.Flags, 0)]
	[property:IntRangeEdit(min: 30, max: 110, step: 5, width: 200, text: "Price", prefix: "£")]
	RngInt Price,
	
	[property:Pos(Tabs.Flags, 1)]
	[property:TimeRangeEdit(maxTicks: 30, paramName: TimeRangeEditParamName, width: 200, text: "Date")]
	RngTime Date
	
	/*[property:SingleEnumEdit]
	State Single2,
	
	[property:MultipleEnumEdit]
	State[] Multiple2,
	
	[property:IntListEdit]
	int[] Ids2,
	
	[property:IntRangeEdit(min: 30, max: 110, step: 5, width: 200, text: "Price", prefix: "£")]
	RngInt Price2,
	
	[property:TimeRangeEdit(maxTicks: 30, paramName: TimeRangeEditParamName, width: 200, text: "Date")]
	RngTime Date2*/
	
)
{
	public static readonly Rec Empty = new(
		State.Three,
		new[] { State.One },
		new [] { 7, 13, 47 },
		new RngInt(45, 80),
		new RngTime(null, null)
		
		/*State.Three,
		new[] { State.One },
		new [] { 7, 13, 47 },
		new RngInt(45, 80),
		new RngTime(null, null)*/
	);
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
