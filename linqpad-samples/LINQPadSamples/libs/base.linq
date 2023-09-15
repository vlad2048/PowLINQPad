<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.RxControls.Utils</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

void Main()
{
}


public static CtrlOpt opt(string title) => new(title, null, null);
public static RngIntBounds bnd(int min, int max, int step) => new(min, max, step);

public static (Control, IDisposable) demo<T>(
	string title,
	Func<IRwVar<T>, (Control, IDisposable)> ctrlFun,
	T valInit,
	T valA,
	T valB
)
{
	var d = new Disp();
	var ui =
		new FieldSet(title,
			demoSimple(ctrlFun, valInit, valA, valB).D(d),
			demoBound(ctrlFun, valInit, valA, valB).D(d)
		).inlineVert();
	return (ui, d);
}

private static (Control, IDisposable) demoSimple<T>(
	Func<IRwVar<T>, (Control, IDisposable)> ctrlFun,
	T valInit,
	T valA,
	T valB
)
{
	var d = new Disp();
	var rxVar = Var.Make(valInit).D(d);
	var ctrl = ctrlFun(rxVar).D(d);
	var evtDc = new EvtDC();
	rxVar.Subscribe(v => evtDc.Log($"When <- {v}")).D(d);

	return (
		new FieldSet("Simple Var",
			new Div(
				new Div(
					ctrl,
					new Div(
						new Button($"{valA}", _ => rxVar.V = valA),
						new Button($"{valB}", _ => rxVar.V = valB)
					).horz()
				).vert(),
				rxVar.Select(e => $"val:{e}").ToDC(D),
				evtDc.DC
			).horz()
		).inline(),
		d
	);
}

private static (Control, IDisposable) demoBound<T>(
	Func<IRwVar<T>, (Control, IDisposable)> ctrlFun,
	T valInit,
	T valA,
	T valB
)
{
	var d = new Disp();
	var rxVar = Var.MakeBnd(valInit).D(d);
	var ctrl = ctrlFun(rxVar).D(d);
	var evtDc = new EvtDC();
	rxVar.WhenInner.Subscribe(v => evtDc.Log($"WhenInner <- {v}")).D(d);
	rxVar.WhenOuter.Subscribe(v => evtDc.Log($"WhenOuter <- {v}")).D(d);

	return (
		new FieldSet("Bound Var",
			new Div(
				new Div(
					ctrl,
					new Div(
						new Button($"{valA}", _ => rxVar.V = valA),
						new Button($"{valB}", _ => rxVar.V = valB)
					).horz()
				).vert(),
				rxVar.Select(e => $"val:{e}").ToDC(D),
				evtDc.DC
			).horz()
		).inline(),
		d
	);
}

public static void L(string s) => $"[{DateTime.Now:HH:mm:ss}] {s}".Dump();
public static class DbgExt
{
	public static void Log<T>(this IObservable<T> obs, [CallerArgumentExpression(nameof(obs))] string? obsExpr = null) => obs.Subscribe(v => $"[{DateTime.Now:HH:mm:ss}]-[{obsExpr}] <- {v}".Dump()).D(RxUI.D);
}


private sealed class EvtDC
{
	private const int MaxLength = 5;
	private readonly Queue<string> msgs = new();
	
	public DumpContainer DC { get; } = new();
	
	public EvtDC()
	{
		for (var i = 0; i < MaxLength; i++)
			msgs.Enqueue(string.Empty);
		DC.UpdateContent(msgs.Reverse());
	}
	
	public void Log(string msg)
	{
		msg = $"[{Timestamp}] {msg}";
		msgs.Enqueue(msg);
		while (msgs.Count > MaxLength)
			msgs.Dequeue();
		DC.UpdateContent(msgs.Reverse());
	}
	
	
	private static string Timestamp => $"{DateTime.Now:HH:mm:ss}";
}


static class DemoCssExt
{
	public static Control horz(this Control c) => c.Css("""
		display: flex;
		align-items: center;
		white-space: nowrap;
		column-gap: 10px;
	""");
	public static Control vert(this Control c) => c.Css("""
		display: flex;
		flex-direction: column;
		align-items: flex-start;
		row-gap: 10px;
	""");
	public static Control inline(this Control c) => c.Css("""
		display: inline;
	""");
	public static Control inlineVert(this Control c) => c.Css("""
		display: inline-flex;
		flex-direction: column;
		align-items: flex-start;
		row-gap: 10px;
	""");
}


public static Disp D => RxUI.D;

void OnStart()
{
	RxUI.Start();
	Util.HtmlHead.AddStyles("""
		body {
			font-family: Consolas;
		}
	""");
}
