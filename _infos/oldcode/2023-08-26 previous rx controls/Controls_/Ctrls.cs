using System.Reactive.Linq;
using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.Controls_.Structs;
using PowLINQPad.Controls_.Utils;
using PowLINQPad.Structs;
using PowLINQPad.UtilsUI;
using static PowLINQPad.Controls_.Utils.CtrlRxStatic;

namespace PowLINQPad.Controls_;


public static class Ctrls
{
    public static (Control[], IDisp) MkTabber<E>(
        IRwVar<E> rxVar,
        Func<E, IRoVar<bool>, Control> disp
    ) where E : struct, Enum =>
        Mk(d =>
            Enum.GetValues<E>()
                .SelectToArray(e =>
                {
                    var isOn = rxVar.SelectVar(f => f.Equals(e));
                    return disp(e, isOn)
                        .Ctrl2Var(rxVar, (_, set) => set(e)).D(d);
                })
        );



    public static (Control, IDisp) MkInt(
        IRwVar<int?> rxVar,
        CtrlOpt opt
    ) =>
        Mk(d =>
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
        IRwVar<BoolOpt> rxVar,
        CtrlOpt opt
    ) =>
        Mk(d =>
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
        return
            Mk(d =>
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
        IRwVar<TxtSearch> rxVar,
        CtrlOpt opt,
        bool allowRegex
    ) =>
        Mk(d =>
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


    public static (Control, IDisp) MkIntSlider(
        IRwVar<int> rxVar,
        CtrlOpt opt,
        RngIntBounds bounds,
        Func<int, string> fmt
    ) =>
        Mk(d =>
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
                            .Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val).D(d)
                            .Ctrl2Var(rxVar, (ctrl, set) => set(ctrl.Value)).D(d),
                        rxVar.Select(fmt).ToSpan(d)
                    )
                ))
            )
        );







    public static (Control, IDisp) MkRngInt(
        IRwVar<RngInt> rxVar,
        CtrlOpt opt,
        RngIntBounds bounds
    ) =>
        Mk(d =>
            mk(CtrlSize.Double,
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
                            .Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Min.RngIntMinToUI(bounds)).D(d)
                            .Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Min = ctrl.Value.RngIntMinFromUI(bounds) })).D(d),
                        rxVar.Select(e => e.Min.RngIntToUILabel()).ToSpan(d)
                    ),
                    horzMid(
                        new RangeControl(
                                bounds.Min,
                                bounds.Max,
                                bounds.Max
                            )
                            .SetStep(bounds.Step)
                            .CssRangeInt()
                            .Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Max.RngIntMaxToUI(bounds)).D(d)
                            .Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Max = ctrl.Value.RngIntMaxFromUI(bounds) })).D(d),
                        rxVar.Select(e => e.Max.RngIntToUILabel()).ToSpan(d)
                    )
                ))
            )
        );

    private static int RngIntMinToUI(this int? v, RngIntBounds bounds) => v ?? bounds.Min;
    private static int RngIntMaxToUI(this int? v, RngIntBounds bounds) => v ?? bounds.Max;
    private static string RngIntToUILabel(this int? v) => v.HasValue ? $"{v}" : "_";
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
        return
            Mk(d =>
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
                            rxVar.Select(e => e.Min.RngTimeToUILabel()).ToSpan(d)
                        ),
                        horzMid(
                            new RangeControl(
                                    0,
                                    ticks.Length - 1,
                                    ticks.Length - 1
                                ).CssRangeTime()
                                .Var2Ctrl(rxVar, (ctrl, val) => ctrl.Value = val.Max.RngTimeMaxToUI(ticks)).D(d)
                                .Ctrl2Var(rxVar, (ctrl, set) => set(rxVar.V with { Max = ctrl.Value.RngTimeMaxFromUI(ticks) })).D(d),
                            rxVar.Select(e => e.Max.RngTimeToUILabel()).ToSpan(d)
                        )
                    ))
                )
            );
    }

    private static int RngTimeMinToUI(this DateTime? v, DateTime[] ticks) => v == null ? 0 : ticks.IndexOf(v.Value);
    private static int RngTimeMaxToUI(this DateTime? v, DateTime[] ticks) => v == null ? ticks.Length - 1 : ticks.IndexOf(v.Value);
    private static string RngTimeToUILabel(this DateTime? v) => v.HasValue ? $"{v:yyyy-MM-dd}" : "_";
    private static DateTime? RngTimeMinFromUI(this int v, DateTime[] ticks) => v == 0 ? null : ticks[v];
    private static DateTime? RngTimeMaxFromUI(this int v, DateTime[] ticks) => v == ticks.Length - 1 ? null : ticks[v];





    public static (Control, IDisp) MkEnumSingle<E>(
        IRwVar<E> rxVar,
        CtrlOpt opt
    ) where E : struct, Enum
    {
        var vals = Enum.GetValues<E>();
        return
            Mk(d =>
                mk(
                    CtrlSize.Single,
                    mkKey(opt.KeyWidth, opt.Title),
                    mkVal(opt.ValWidth,
                        new SelectBox(
                                SelectBoxKind.DropDown,
                                vals.SelectToArray(e => $"{e}")
                            )
                            .Var2Ctrl(rxVar, (ctrl, val) => ctrl.SelectedIndex = vals.IndexOf(val)).D(d)
                            .Ctrl2Var(rxVar, (ctrl, set) => set(vals[ctrl.SelectedIndex])).D(d)
                    )
                )
            );
    }



    public static (Control, IDisp) MkEnumMultiple<E>(
        IRwVar<E[]> rxVar,
        CtrlOpt opt
    ) where E : struct, Enum
    {
        var vals = Enum.GetValues<E>();
        return
            Mk(d =>
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
}