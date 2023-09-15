using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LINQPad;
using LINQPad.Controls;

namespace PowLINQPad.Utils.Ctrls_;

public static class CtrlsEvtExt
{
    public static IObservable<Unit> WhenClicked(this Control ctrl) => Obs.FromEventPattern(e => ctrl.Click += e, e => ctrl.Click -= e).ToUnit();

    public static C WhenClickedAction<C>(this C ctrl, IRoDispBase d, Action action) where C : Control
    {
        ctrl.WhenClicked().Subscribe(_ => action()).D(d);
        return ctrl;
    }



    public static IObservable<Unit> WhenClickedOutside(this Control ctrl, IRoDispBase d)
    {
        Util.HtmlHead.AddScript("""
			function onClickOutside(eltId) {
				const elt = document.getElementById(eltId);
				if (!elt) { external.log(`elt not found: ${eltId}`); return; }
				//external.log(`elt found: ${eltId}`);
				
				const listener = event => {
					const contains = elt.contains(event.target);
					if (!contains) {
						//external.log('Click Outside Detected');
						elt.dispatchEvent(new CustomEvent('clickOutside'));
					}
					//external.log(`evt  contains:${contains}`);
				}
				
				document.addEventListener('click', listener);
				//external.log('finished');
			}
		""");
        Util.InvokeScript(false, "onClickOutside", ctrl.HtmlElement.ID);

        return ctrl.WhenEvent("clickOutside", d);
    }

    public static C WhenClickedOutsideAction<C>(this C ctrl, IRoDispBase d, Action action) where C : Control
    {
        ctrl.WhenClickedOutside(d).Subscribe(_ => action()).D(d);
        return ctrl;
    }





    public static IObservable<Unit> WhenEvent(this Control ctrl, string eventName, IRoDispBase d)
    {
        ISubject<Unit> when = new Subject<Unit>().D(d);

        void Handler(object? sender, EventArgs args) => when.OnNext(Unit.Default);

        ctrl.HtmlElement.AddEventListener(eventName, Handler);
        Disposable.Create(() => ctrl.HtmlElement.RemoveEventListener(eventName, Handler));

        return when.AsObservable();
    }

    public static IObservable<T> WhenPropsEvent<T>(this Control ctrl, string eventName, IRoDispBase d, Func<IDictionary<string, string>, T> fun, params string[] props)
    {
        ISubject<T> when = new Subject<T>().D(d);

        void Handler(object? sender, PropertyEventArgs args) => when.OnNext(fun(args.Properties));

        ctrl.HtmlElement.AddEventListener(eventName, props, Handler);
        Disposable.Create(() => ctrl.HtmlElement.RemoveEventListener(eventName, props, Handler));

        return when.AsObservable();
    }


    private sealed record Attempt(long Index, bool Success);

    public static IObservable<Unit> WhenMounted(this Control ctrl) =>
        Obs.Interval(TimeSpan.FromMilliseconds(0))
            .Take(100)
            .Select(i => new Attempt(i, IsElementPresent(ctrl.HtmlElement.ID)))
            .TakeUntil(e => e.Success)
            .Do(e =>
            {
                if (e is { Index: 99, Success: false })
                {
                    $"Failed to detect mount for control: '{ctrl.HtmlElement.ID}'".Dump();
                }
            })
            .Where(e => e.Success)
            .Take(1)
            .ToUnit();


    private static bool IsElementPresent(string id)
    {
        var obj = Util.InvokeScript(true, "eval", $$"""
				!!document.getElementById('{{id}}');
			""");
        return obj is string s && bool.Parse(s);
    }
}
