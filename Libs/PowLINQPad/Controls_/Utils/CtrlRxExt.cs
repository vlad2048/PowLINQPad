using System.Reactive;
using LINQPad.Controls;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PowLINQPad.Controls_.Utils;

namespace PowLINQPad.Controls_.Utils;

static class CtrlRxStatic
{
    public static (C, IDisp) Mk<C>(Func<IRoDispBase, C> fun)
    {
        var d = new Disp();
        return (fun(d), d);
    }

}

static class CtrlRxExt
{
    public static (C, IDisp) Var2Ctrl<C, T>(
        this C ctrl,
        IRwVar<T> rxVar,
        Action<C, T> setCtrl
    ) where C : Control =>
        rxVar
            .When()
            .Subscribe(v => setCtrl(ctrl, v))
            .WithObj(ctrl);


    public static (C, IDisp) Ctrl2Var<C, T>(
        this C ctrl,
        IRwVar<T> rxVar,
        Action<C, Action<T>> setVar
    ) where C : Control =>
        ctrl.WhenCtrlChanged()
            .Subscribe(_ => setVar(ctrl, rxVar.Set))
            .WithObj(ctrl);





    public static (T, IDisp) WithObj<T>(this IDisp d, T obj) => (obj, d);

    public static IObservable<Unit> WhenCtrlChanged<C>(this C ctrl) where C : Control =>
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