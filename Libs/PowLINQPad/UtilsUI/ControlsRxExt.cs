using System.Reactive.Disposables;
using LINQPad;
using LINQPad.Controls;
using PowBasics.CollectionsExt;

namespace PowLINQPad.UtilsUI;

public static class ControlsRxExt
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


	//private static (T, IDisposable) WithD<T>(this IDisposable d, T obj) => (obj, d);

	/*private static (T, IDisposable) WithD<T>(this T obj)
	{
		var d = new Disp();
		return (obj, d)
	}*/
	
	/*private static (C, IDisposable) React<C, T>(this C c, IObservable<T> rxV, Action<C, T> action) where C : Control
	{
		var d = new Disp();
		rxV.Subscribe(v => action(c, v)).D(d);
		return (c, d);
	}*/



	
	public static (TextBox, IDisposable) OnChange(this TextBox ctrl, Action<string> action)
	{
		var d = new Disp();
		ctrl.IsMultithreaded = true;
		void Handler(object? sender, EventArgs args) => action(ctrl.Text);
		ctrl.TextInput += Handler;
		Disposable.Create(() => ctrl.TextInput -= Handler).D(d);
		return (ctrl, d);
	}

	public static (CheckBox, IDisposable) OnChange(this CheckBox ctrl, Action<bool> action)
	{
		var d = new Disp();
		ctrl.IsMultithreaded = true;
		void Handler(object? sender, EventArgs args) => action(ctrl.Checked);
		ctrl.Click += Handler;
		Disposable.Create(() => ctrl.Click -= Handler).D(d);
		return (ctrl, d);
	}

	public static (RangeControl, IDisposable) OnChange(this RangeControl ctrl, Action<int> action)
	{
		var d = new Disp();
		ctrl.IsMultithreaded = true;
		void Handler(object? sender, EventArgs args) => action(ctrl.Value);
		ctrl.ValueInput += Handler;
		Disposable.Create(() => ctrl.ValueInput -= Handler).D(d);
		return (ctrl, d);
	}

	public static (SelectBox, IDisposable) OnChange<E>(this SelectBox ctrl, Action<E[]> action) where E : struct, Enum
	{
		var d = new Disp();
		ctrl.IsMultithreaded = true;
		var enumValues = Enum.GetValues<E>();
		void Handler(object? sender, EventArgs args) => action(ctrl.SelectedIndexes.SelectToArray(idx => enumValues[idx]));
		ctrl.SelectionChanged += Handler;
		Disposable.Create(() => ctrl.SelectionChanged -= Handler).D(d);
		return (ctrl, d);
	}

	public static (RadioButton, IDisposable) OnSelected(this RadioButton ctrl, Action action)
	{
		var d = new Disp();
		ctrl.IsMultithreaded = true;
		void Handler(object? sender, EventArgs args) => action();
		ctrl.Click += Handler;
		Disposable.Create(() => ctrl.Click -= Handler).D(d);
		return (ctrl, d);
	}
}