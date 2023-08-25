<Query Kind="Program">
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>PowLINQPad.RxControls.Structs</Namespace>
  <Namespace>PowLINQPad.RxControls.Utils</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

#load "..\PowLINQPad-Samples\libs\base"

global using Obs = System.Reactive.Linq.Observable;
global using PowRxVar;
global using static PowLINQPad.RxUI;
global using static PowLINQPad.RxControls.Utils.CtrlBlocks;
global using IDisp = System.IDisposable;


void Main()
{
	var rxVar = Var.MakeBnd(15).D(D);
	rxVar.WhenInner.Log();
	rxVar.WhenOuter.Log();
	
	//rxVar.V = 123;
	
	var ctrl = C.MkIntSlider(rxVar, opt("var"), bnd(0, 40, 5), e => $"{e}").D(D);
	show(ctrl, rxVar, 10, 25);
}


public static class C
{
	public static (Control, IDisp) MkIntSlider(
			IRwVar<int> rxVar,
			CtrlOpt opt,
			RngIntBounds bounds,
			Func<int, string> fmt
		) => Mk(d =>
		{

			return
				mk(CtrlSize.Single,
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
								.Var2Ctrl(rxVar, (ctrl, val) =>
								{
									L($"Ctrl.Value <- rxVar ({val})");
									ctrl.Value = val;
								}).D(d)
								.Ctrl2Var(rxVar, (ctrl, set) =>
								{
									L($"rxVar <- Ctrl.Value ({ctrl.Value})");
									set(ctrl.Value);
								}).D(d),
							rxVar.Select(fmt).ToSpan(d)
						)
					))
				);
		});


	private static (C, IDisp) Mk<C>(Func<IRoDispBase, C> fun)
	{
		var d = new Disp();
		return (fun(d), d);
	}
	
	private static (C, IDisp) Var2Ctrl<C, T>(
		this C ctrl,
		IRwVar<T> rxVar,
		Action<C, T> setCtrl
	) where C : Control =>
		rxVar
			.When()
			.Subscribe(v => setCtrl(ctrl, v))
			.WithObj(ctrl);

	private static (C, IDisp) Ctrl2Var<C, T>(
		this C ctrl,
		IRwVar<T> rxVar,
		Action<C, Action<T>> setVar
	) where C : Control =>
		ctrl.WhenCtrlChanged()
			.Subscribe(_ => setVar(ctrl, rxVar.Set))
			.WithObj(ctrl);


	private static IObservable<T> When<T>(this IRwVar<T> rxVar) => rxVar switch
	{
		IFullRwBndVar<T> rxBndVar => rxBndVar.WhenOuter.Prepend(rxVar.V),
		_ => rxVar
	};

	private static void Set<T>(this IRwVar<T> rxVar, T val)
	{
		switch (rxVar)
		{
			case IFullRwBndVar<T> rxBndVar:
				rxBndVar.SetInner(val);
				break;
				
			default:
				rxVar.V = val;
				break;
		}
	}
}























