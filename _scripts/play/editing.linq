<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <NuGetReference>PowRxVar</NuGetReference>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowLINQPad.Editing</Namespace>
  <Namespace>PowLINQPad.Editing._Base</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.Editing.Controls_</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var filter = Var.MakeBnd(Rec.Empty).D(D);
	var ui = filter.MakeEditor(D);
	
	Util.HorizontalRun(true,
		Util.VerticalRun(
			ui,
			slider
		),
		filter.ToDC(D)
	).Dump();

	/*Task.Delay(0).ContinueWith(_ =>
	{
		filter.V = new Rec(
			State.Two,
			new[] { State.One },
			new[] { 12, 13, 47 }
		);
	});*/
	
	//var rxVar = Var.MakeBnd(new[] { 12, 13, 47 }).D(D);
	//var rxVarStr = rxVar.Convert(Ints2Str, Str2Ints, D);
	//rxVarStr.WhenOuter.Prepend(rxVarStr.V).Log();
	new Button("Export", _ => HtmlExporter.OpenInBrowser()).Dump();
}

private static string Ints2Str(int[] v) => v.JoinText(",");

private static int[] Str2Ints(string v) => v
	.Chop(',')
	.Where(e => int.TryParse(e, out _))
	.SelectToArray(int.Parse);


public static class LUtils
{
	public static Control ControlWith<C, T>(this C ctrl, IFullRwBndVar<T> rxVar, IRoDispBase d) where C : Control, IBoundCtrl<T>
	{
		rxVar.WhenOuter.Prepend(rxVar.V).PipeToOuter(ctrl.RxVar, d);
		ctrl.RxVar.WhenInner.PipeToInner(rxVar, d);
		return ctrl;
	}
	
	public static string[] Chop(this string s, char sep) => s.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	
	public static IFullRwBndVar<U> Convert<T, U>(this IFullRwBndVar<T> rxVar, Func<T, U> mapNext, Func<U, T> mapPrev, IRoDispBase d)
	{
		var rxCnv = Var.MakeBnd(mapNext(rxVar.V)).D(d);
		rxVar.WhenOuter.Select(mapNext).PipeToOuter(rxCnv, d);
		rxCnv.WhenInner.Select(mapPrev).PipeToInner(rxVar, d);
		return rxCnv;
	}
	private static void PipeToOuter<U>(this IObservable<U> obsProp, IRwBndVar<U> rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetOuter).D(d);
	private static void PipeToInner<U>(this IObservable<U> obsProp, IFullRwBndVar<U> rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetInner).D(d);
}


enum State
{
	One,
	Two,
	Three,
}

record Rec(
	[property:SingleEnumEdit]
	State Single,
	[property:MultipleEnumEdit]
	State[] Multiple,
	[property:IntListEdit]
	int[] ids
)
{
	public static readonly Rec Empty = new(
		State.Three,
		new[] { State.One },
		new [] { 7, 13, 47 }
	);
}




public static Disp D => RxUI.D;
void OnStart()
{
	RxUI.Start();
	
	//Util.HtmlHead.AddStyles("@import url('https://fonts.googleapis.com/css2?family=Inter&display=swap');");
}





