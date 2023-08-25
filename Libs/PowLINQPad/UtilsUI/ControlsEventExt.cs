using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LINQPad.Controls;

namespace PowLINQPad.UtilsUI;

public static class ControlsEventExt
{
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
}