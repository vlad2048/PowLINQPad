<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
</Query>

using static PowLINQPad.RxControls.Utils.CtrlBlocks;

void Main()
{
	var simpleVar = Var.Make<int?>(null).D(D);
	var simpleUi = Ctrls.MkInt(simpleVar, new CtrlOpt("Number", null, null)).D(D);
	
	
	
	vert(
		horz(
			simpleUi,
			new Button("Inc", _ => simpleVar.V++)
		),
		simpleVar.Select(e => $"val: {e}").ToSpan(D)
	).Dump("Simple Var");
	
	
	var twowayVar = Var.MakeBnd<int?>(null).D(D);
	var twowayUi = Ctrls.MkInt(twowayVar, new CtrlOpt("Number", null, null)).D(D);
	vert(
		horz(
			twowayUi,
			new Button("Inc", _ => twowayVar.V++)
		),
		twowayVar.Select(e => $"val: {e}").ToSpan(D),
		twowayVar.WhenOuter.Select(e => $"outer: {e}").ToSpan(D),
		twowayVar.WhenInner.Select(e => $"inner: {e}").ToSpan(D)
	).Dump("TwoWay Var");
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
