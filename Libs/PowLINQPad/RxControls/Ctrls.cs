using System.Reactive.Linq;
using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.RxControls.Structs;
using PowLINQPad.RxControls.Utils;
using PowLINQPad.Structs;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.RxControls;


public static class Ctrls
{
	public static (Control, IDisp) MkInt(
		IRwVar<int?> rxVar,
		CtrlOpt opt
	) => Mk(d =>
		mk(
			CtrlSize.Single,
			mkKey(opt.KeyWidth, opt.Title),
			mkVal(opt.ValWidth,
				new TextBox(rxVar.V.IntToUI())
					.OnChange(v => rxVar.V = IntFromUI(v)).D(d)
			)
		)
	);

	private static string IntToUI(this int? v) => $"{v}";
	private static int? IntFromUI(this string v) => int.TryParse(v, out var val) ? val : null;




	public static (Control, IDisp) MkBoolOpt(
		IRwVar<BoolOpt> rxVar,
		CtrlOpt opt
	) => Mk(d =>
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

	private static (Control[], IDisp) MakeRadios<E>(IRwVar<E> rxVar) where E : struct, Enum
	{
		var groupId = MakeGroupId();
		return Mk(d =>
			Enum.GetValues<E>()
				.SelectToArray(e =>
					new RadioButton(groupId, isChecked: rxVar.V.Equals(e))
						.Squeeze()
						.OnSelected(() => rxVar.V = e).D(d)
				)
		);
	}



	public static (Control, IDisp) MkText(
		IRwVar<TxtSearch> rxVar,
		CtrlOpt opt,
		bool allowRegex
	) => Mk(d => 
		mk(
			CtrlSize.Single,
			mkKey(opt.KeyWidth, opt.Title, allowRegex switch
			{
				false => null,
				true => new CheckBox("regex", rxVar.V.UseRegex)
					.OnChange(v => rxVar.V = rxVar.V.SetUseRegex(v)).D(D)
			}),
			mkVal(opt.ValWidth,
				new TextBox(rxVar.V.Text)
					.OnChange(v => rxVar.V = rxVar.V.SetText(v)).D(d)
			)
		)
	);



	public static (Control, IDisp) MkRngInt(
		IRwVar<RngInt> rxVar,
		CtrlOpt opt,
		RngIntBounds bounds
	) => Mk(d => 
		mk(CtrlSize.Double,
			mkKey(opt.KeyWidth, opt.Title),
			mkVal(opt.ValWidth, vert(
				horzMid(
					new RangeControl(
							bounds.Min,
							bounds.Max,
							rxVar.V.Min.RngIntMinToUI(bounds)
						).CssRangeInt()
						.OnChange(v => rxVar.V = rxVar.V with {Min = v.RngIntMinFromUI(bounds)}).D(D),
					rxVar.Select(e => e.Min.RngIntMinToUI(bounds)).ToSpan(d)
				),
				horzMid(
					new RangeControl(
							bounds.Min,
							bounds.Max,
							rxVar.V.Max.RngIntMaxToUI(bounds)
						).CssRangeInt()
						.OnChange(v => rxVar.V = rxVar.V with {Max = v.RngIntMaxFromUI(bounds)}).D(D),
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
		IRwVar<RngTime> rxVar,
		CtrlOpt opt,
		IEnumerable<DateTime> times,
		int maxTicks
	)
	{
		var ticks = RngTimeUtils.MakeTicks(times, maxTicks);
		return Mk(d => 
			mk(CtrlSize.Double,
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth, vert(
					horzMid(
						new RangeControl(
								0,
								ticks.Length - 1,
								rxVar.V.Min.RngTimeMinToUI(ticks)
							).CssRangeTime()
							.OnChange(v => rxVar.V = rxVar.V with {Min = v.RngTimeMinFromUI(ticks)}).D(D),
						rxVar.Select(e => e.Min.RngTimeMinToUI(ticks)).ToSpan(d)
					),
					horzMid(
						new RangeControl(
								0,
								ticks.Length - 1,
								rxVar.V.Max.RngTimeMaxToUI(ticks)
							).CssRangeTime()
							.OnChange(v => rxVar.V = rxVar.V with {Max = v.RngTimeMaxFromUI(ticks)}).D(D),
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
		IRwVar<E[]> rxVar,
		CtrlOpt opt
	) where E : struct, Enum
	{
		var vals = Enum.GetValues<E>();
		return Mk(d => 
			mk(
				CtrlSize.Double,
				mkKey(opt.KeyWidth, opt.Title),
				mkVal(opt.ValWidth,
					new SelectBox(
							SelectBoxKind.MultiSelectListBox,
							vals.SelectToArray(e => $"{e}")
						)
						.InitSelectedIndices(rxVar.V)
						//.CssIf(valHeight.HasValue, $"height: {valHeight}px;")
						.OnChange<E>(vs => rxVar.V = vs).D(d)
				)
			)
		);
	}

	private static SelectBox InitSelectedIndices<E>(this SelectBox ctrl, E[] selVals) where E : struct, Enum
	{
		var vals = Enum.GetValues<E>();
		ctrl.SelectedIndexes = selVals.SelectToArray(e => vals.IndexOf(e));
		return ctrl;
	}


	private static (C, IDisp) Mk<C>(Func<IRoDispBase, C> fun)
	{
		var d = new Disp();
		return (fun(d), d);
	}
}