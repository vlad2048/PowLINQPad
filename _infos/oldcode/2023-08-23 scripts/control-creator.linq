<Query Kind="Program">
  <NuGetReference>PowLINQPad</NuGetReference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>PowLINQPad</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var idx = Var.Make(0).D(D);


	CCtrl.MkIntSlider(idx, new CtrlOpt("Id", null, 300), new RngIntBounds(0, 100, 5), e => $"{e}").D(D).Dump();
	
	idx.Subscribe(e => $"idx <- {e}".Dump()).D(D);
}



public static class CCtrl
{
	public static (Control, IDisp) MkIntSlider(
		//IRwVar<int> rxOutVar,
		IRwVar<int> rxVar,
		CtrlOpt opt,
		RngIntBounds bounds,
		Func<int, string> fmt
	) =>
		Mk(d =>
			mk(
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth, vert(
					horzMid(
						new RangeControl(
								bounds.Min,
								bounds.Max,
								bounds.Min
							)
							.SetStep(bounds.Step)
							.CssRangeInt()
							.Var2CtrlSimple(rxVar, (ctrl, val) => ctrl.Value = val).D(d)
							.Ctrl2VarSimple(rxVar, (ctrl, set) => set(rxVar.V = ctrl.Value)).D(d),
						rxVar.Select(fmt).ToSpan(d)
					)
				))
			)
		);
		
		
		
		/*rxOutVar.Mk((rxVar, d) =>
			mk(
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth, vert(
					horzMid(
						new RangeControl(
								bounds.Min,
								bounds.Max,
								bounds.Min
							)
							.SetStep(bounds.Step)
							.CssRangeInt()
							.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val).D(d)
							.Ctrl2Var(rxVar, (ctrl, set) =>
							{
								$"abc: {ctrl.Value}".Dump();
								set(rxVar.V = ctrl.Value);
							}).D(d),
						rxVar.Select(fmt).ToSpan(d)
					)
				))
			)
		);*/
	
	private static (C, IDisp) Var2CtrlSimple<C, T>(
		this C ctrl,
		IRoVar<T> rxVar,
		Action<C, T> setCtrl
	) where C : Control =>
		rxVar
			//.Prepend(rxVar.V)
			.Subscribe(v => setCtrl(ctrl, v))
			.WithObj(ctrl);
	private static (C, IDisp) Ctrl2VarSimple<C, T>(
		this C ctrl,
		IRwVar<T> rxVar,
		Action<C, Action<T>> setVar
	) where C : Control =>
		ctrl.WhenCtrlChanged()
			.Subscribe(_ => setVar(ctrl, e => rxVar.V = e))
			.WithObj(ctrl);
	
	
	private static (C, IDisp) Var2Ctrl<C, T>(
		this C ctrl,
		IFullRwBndVar<T> rxVar,
		Action<C, T> setCtrl
	) where C : Control =>
		rxVar.WhenOuter
			.Prepend(rxVar.V)
			.Subscribe(v => setCtrl(ctrl, v))
			.WithObj(ctrl);

	private static (C, IDisp) Ctrl2Var<C, T>(
		this C ctrl,
		IFullRwBndVar<T> rxVar,
		Action<C, Action<T>> setVar
	) where C : Control =>
		ctrl.WhenCtrlChanged()
			.Subscribe(_ => setVar(ctrl, rxVar.SetInner))
			.WithObj(ctrl);
	
	
	private static (T, IDisposable) WithObj<T>(this IDisposable d, T obj) => (obj, d);
	
	
	private static IObservable<Unit> WhenCtrlChanged<C>(this C ctrl) where C : Control =>
	(
		ctrl switch
		{
			TextBox c => Obs.FromEventPattern(e => c.TextInput += e, e => c.TextInput -= e),
			RangeControl c => Obs.FromEventPattern(e => c.ValueInput += e, e => c.ValueInput -= e),
			SelectBox c => Obs.FromEventPattern(e => c.SelectionChanged += e, e => c.SelectionChanged -= e),
			//CheckBox c => Obs.FromEventPattern(e => c.Click += e, e => c.Click -= e),
			Control c => Obs.FromEventPattern(e => c.Click += e, e => c.Click -= e),
			_ => throw new ArgumentException($"Cannot detect change for {ctrl.GetType().Name}")
		}).ToUnit();
	
	
	private static RangeControl SetStep(this RangeControl ctrl, int step)
	{
		ctrl.HtmlElement.InvokeScript(false, "eval", $"targetElement.step = {step}");
		return ctrl;
	}
	
	
	private static Div mkKey(int? width, string title, Control? under = null)
	{
		var span = new Label($"{title}:");
		var kids = under switch
		{
			null => new Control[] { span },
			not null => new[] { span, under },
		};
		var div = new Div(kids).Css($"""
			display:			flex;
			flex-direction:		column;
			align-items:		flex-end;
			{(width.HasValue ? $"width:	{width}px;" : "")}
		""");
		// {(DbgColors ? $"background-color:	{dbgColKey};" : "")}
		// width:				{width ?? DefaultKeyWidth}px;
		return div;
	}
	private static Control mkVal(int? width, Control kid) => kid.Css($"""
		width:				{width ?? 300}px !important;
	""");
	private static Div vert(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	column;
		row-gap:		5px;
	""");
	private static Div horzMid(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	row;
		align-items:	center;
		column-gap:		5px;
	""");
	private static RangeControl CssRangeInt(this RangeControl ctrl) => ctrl.Css($"width: calc(100% - 30px);");
	
	
	private static Div mk(Control key, Control val) => new Div(key, val).Css($"""
		display:	flex;
		overflow:	hidden;
		height:		30px;
	""");
	
	private static (C, IDisp) Mk<C, T>(this IRwVar<T> rxOutVar, Func<IFullRwBndVar<T>, IRoDispBase, C> fun)
	{
		var d = new Disp();
		var rxVar = rxOutVar.ToBnd().D(d);
		return (fun(rxVar, d), d);
	}
	
	private static (C, IDisp) Mk<C>(Func<IRoDispBase, C> fun)
	{
		var d = new Disp();
		return (fun(d), d);
	}
	
	private static (IFullRwBndVar<T>, IDisp) ToBnd<T>(this IRwVar<T> rxVar) => rxVar switch
	{
		IFullRwBndVar<T> bndVar => (bndVar, Disposable.Empty),
		_ => rxVar.ToBndMake()
	};

	private static (IFullRwBndVar<T>, IDisp) ToBndMake<T>(this IRwVar<T> rxVar)
	{
		var d = new Disp();
		var rxBndVar = Var.MakeBnd(rxVar.V).D(d);
		rxVar.Subscribe(rxBndVar.SetOuter).D(d);
		rxBndVar.WhenInner.Subscribe(v => rxVar.V = v).D(d);
		return (rxBndVar, d);
	}
}






public static Disp D => RxUI.D;

void OnStart() => RxUI.Start();
