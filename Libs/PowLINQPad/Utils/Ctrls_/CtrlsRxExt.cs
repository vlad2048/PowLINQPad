using LINQPad;
using LINQPad.Controls;

namespace PowLINQPad.Utils.Ctrls_;

public static class CtrlsRxExt
{
    public static Span ToSpan<T>(this IObservable<T> obs, IRoDispBase d)
    {
        var span = new Span();
        obs.Subscribe(v => span.Text = $"{v}").D(d);
        return span;
    }
    public static DumpContainer ToDC<T>(this IObservable<T> obs, IRoDispBase d)
    {
        var dc = new DumpContainer();
        obs.Subscribe(v => dc.UpdateContent(v)).D(d);
        return dc;
    }

    public static C EnableIf<C>(this C ctrl, IObservable<bool> obs, IRoDispBase d) where C : Control
    {
        obs.Subscribe(v => ctrl.Enabled = v).D(d);
        return ctrl;
    }

    public static C DisplayIf<C>(this C ctrl, IObservable<bool> obs, IRoDispBase d) where C : Control
    {
        obs.Subscribe(v => ctrl.Styles["display"] = v ? "flex" : "none").D(d);
        return ctrl;
    }

    public static C VisibleIf<C>(this C ctrl, IObservable<bool> obs, IRoDispBase d) where C : Control
    {
        obs.Subscribe(v => ctrl.Styles["visibility"] = v ? "visible" : "hidden").D(d);
        return ctrl;
    }
}