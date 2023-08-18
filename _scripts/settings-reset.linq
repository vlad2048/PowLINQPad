<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>PowLINQPad.Settings_</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

class Prefs : ISettings
{
	public int? Val { get; set; } = 43;
}


void Main()
{
	var prefs = Settings.Load<Prefs>();
	var rxVar = prefs.Get(e => e.Val);
	
	var uiVar = Ctrls.MkInt(rxVar, new CtrlOpt("Val", null, null)).D(D);
	
	Util.VerticalRun(
		Util.HorizontalRun(true,
			uiVar,
			rxVar.Select(e => $"val:{e}").ToSpan(D)
		),
		new Button("Reset", _ => prefs.Reset())
	).Dump();
}


public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
