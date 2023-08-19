using System.Reactive;
using System.Reactive.Linq;
using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.RxControls.Structs;
using PowLINQPad.RxControls.Utils;
using PowLINQPad.Structs;
using PowLINQPad.UtilsInternal;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.RxControls;


public static class Ctrls
{
	public static (Control[], IDisp) MkTabber<E>(
		IRwVar<E> rxOutVar,
		Func<E, IRoVar<bool>, Control> disp
	) where E : struct, Enum =>
		rxOutVar.Mk((rxVar, d) => 
			Enum.GetValues<E>()
				.SelectToArray(e =>
				{
					var isOn = rxVar.SelectVar(f => f.Equals(e));
					return disp(e, isOn)
						.Ctrl2Var(rxVar, (ctrl, set) => set(e)).D(d);
				})
		);



	public static (Control, IDisp) MkInt(
		IRwVar<int?> rxOutVar,
		CtrlOpt opt
	) =>
		rxOutVar.Mk((rxVar, d) =>
			mk(
				CtrlSize.Single,
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth,
					new TextBox()
						.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Text = val.IntToUI()).D(d)
						.Ctrl2Var(rxVar, (ctrl, set) => set(ctrl.Text.IntFromUI())).D(d)
				)
			)
		);

	private static string IntToUI(this int? v) => $"{v}";
	private static int? IntFromUI(this string v) => int.TryParse(v, out var val) ? val : null;




	public static (Control, IDisp) MkBoolOpt(
		IRwVar<BoolOpt> rxOutVar,
		CtrlOpt opt
	) => rxOutVar.Mk((rxVar, d) =>
		mk(
			CtrlSize.Single,
			mkKey(opt.KeyWidth, opt.Title),
			mkVal(opt.ValWidth, horz(
				MakeRadios(rxVar).D(d).Append(
						rxVar.Select(e => $" {e}").ToSpan(d)
					)
					.ToArray()
			))
		)
	);

	private static (Control[], IDisp) MakeRadios<E>(IFullRwBndVar<E> rxVar) where E : struct, Enum
	{
		var groupId = MakeGroupId();
		return Mk(d =>
			Enum.GetValues<E>()
				.SelectToArray(e =>
					new RadioButton(groupId, isChecked: rxVar.V.Equals(e))
						.Squeeze()
						.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Checked = val.Equals(e)).D(d)
						.Ctrl2Var(rxVar, (ctrl, set) => { if (ctrl.Checked) set(e); }).D(d)
				)
		);
	}



	public static (Control, IDisp) MkText(
		IRwVar<TxtSearch> rxOutVar,
		CtrlOpt opt,
		bool allowRegex
	) => rxOutVar.Mk((rxVar, d) => 
		mk(
			CtrlSize.Single,
			mkKey(opt.KeyWidth, opt.Title, allowRegex switch
			{
				false => null,
				true => new CheckBox("regex", rxVar.V.UseRegex)
					.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Checked = val.UseRegex).D(d)
					.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V.SetUseRegex(ctrl.Checked))).D(d)
			}),
			mkVal(opt.ValWidth,
				new TextBox(rxVar.V.Text)
					.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Text = val.Text).D(d)
					.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V.SetText(ctrl.Text))).D(d)
			)
		)
	);



	public static (Control, IDisp) MkRngInt(
		IRwVar<RngInt> rxOutVar,
		CtrlOpt opt,
		RngIntBounds bounds
	) => rxOutVar.Mk((rxVar, d) =>
		mk(CtrlSize.Double,
			mkKey(opt.KeyWidth, opt.Title),
			mkVal(opt.ValWidth, vert(
				horzMid(
					new RangeControl(
							bounds.Min,
							bounds.Max,
							bounds.Min
						).CssRangeInt()
						.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Min.RngIntMinToUI(bounds)).D(d)
						.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Min = ctrl.Value.RngIntMinFromUI(bounds) })).D(d),
					rxVar.Select(e => e.Min.RngIntMinToUI(bounds)).ToSpan(d)
				),
				horzMid(
					new RangeControl(
						bounds.Min,
						bounds.Max,
						bounds.Max
					).CssRangeInt()
						.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Max.RngIntMaxToUI(bounds)).D(d)
						.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Max = ctrl.Value.RngIntMaxFromUI(bounds) })).D(d),
					rxVar.Select(e => e.Max.RngIntMaxToUI(bounds)).ToSpan(d)
				)
			))
		)
	);

	private static int RngIntMinToUI(this int? v, RngIntBounds bounds) => v ?? bounds.Min;
	private static int RngIntMaxToUI(this int? v, RngIntBounds bounds) => v ?? bounds.Max;
	private static int? RngIntMinFromUI(this int v, RngIntBounds bounds) => v == bounds.Min ? null : v;
	private static int? RngIntMaxFromUI(this int v, RngIntBounds bounds) => v == bounds.Max ? null : v;




	public static (Control, IDisp) MkRngTime(
		IRwVar<RngTime> rxOutVar,
		CtrlOpt opt,
		IEnumerable<DateTime> times,
		int maxTicks
	)
	{
		var ticks = RngTimeUtils.MakeTicks(times, maxTicks);
		return rxOutVar.Mk((rxVar, d) => 
			mk(CtrlSize.Double,
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth, vert(
					horzMid(
						new RangeControl(
								0,
								ticks.Length - 1,
								0
							).CssRangeTime()
							.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Min.RngTimeMinToUI(ticks)).D(d)
							.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Min = ctrl.Value.RngTimeMinFromUI(ticks) })).D(d),
						rxVar.Select(e => e.Min.RngTimeMinToUI(ticks)).ToSpan(d)
					),
					horzMid(
						new RangeControl(
								0,
								ticks.Length - 1,
								ticks.Length - 1
							).CssRangeTime()
							.Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Max.RngTimeMaxToUI(ticks)).D(d)
							.Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Max = ctrl.Value.RngTimeMaxFromUI(ticks) })).D(d),
						rxVar.Select(e => e.Max.RngTimeMaxToUI(ticks)).ToSpan(d)
					)
				))
			)
		);
	}

	private static int RngTimeMinToUI(this DateTime? v, DateTime[] ticks) => v == null ? 0 : ticks.IndexOf(v.Value);
	private static int RngTimeMaxToUI(this DateTime? v, DateTime[] ticks) => v == null ? ticks.Length - 1 : ticks.IndexOf(v.Value);
	private static DateTime? RngTimeMinFromUI(this int v, DateTime[] ticks) => v == 0 ? null : ticks[v];
	private static DateTime? RngTimeMaxFromUI(this int v, DateTime[] ticks) => v == ticks.Length - 1 ? null : ticks[v];




	public static (Control, IDisp) MkEnumMultiple<E>(
		IRwVar<E[]> rxOutVar,
		CtrlOpt opt
	) where E : struct, Enum
	{
		var vals = Enum.GetValues<E>();
		return rxOutVar.Mk((rxVar, d) => 
			mk(
				CtrlSize.Double,
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth,
					new SelectBox(
							SelectBoxKind.MultiSelectListBox,
							vals.SelectToArray(e => $"{e}")
						)
						.Var2Ctrl(rxVar, (ctrl, val) => ctrl.SelectedIndexes = val.SelectToArray(e => vals.IndexOf(e))).D(d)
						.Ctrl2Var(rxVar, (ctrl, set) => set(ctrl.SelectedIndexes.SelectToArray(idx => vals[idx]))).D(d)
				)
			)
		);
	}






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
}