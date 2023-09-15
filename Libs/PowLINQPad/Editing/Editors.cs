using LINQPad.Controls;
using System.Reactive.Linq;
using System.Reflection;
using LINQPad;
using PowBasics.CollectionsExt;
using PowLINQPad.Editing._Base;
using PowLINQPad.Editing.Controls_;
using PowLINQPad.Editing.Controls_.Slider_;
using PowLINQPad.Editing.Layout_;
using PowLINQPad.Structs;
using PowMaybeErr;
using PowLINQPad.UtilsInternal;

namespace PowLINQPad.Editing;


sealed record EditCtx(
	EditAttribute EditAttr,
	Pos Pos,
	PropertyInfo PropNfo,
	object RxVar,
	IRoDispBase D,
	IReadOnlyDictionary<string, object> ExtraParams,
	string[] TabNames
)
{
	public Control Build() => EditAttr.Build(this);
}


public abstract class EditAttribute : Attribute
{
	public string? Text { get; }

	protected EditAttribute(string? text = null)
	{
		Text = text;
	}

	internal abstract Control Build(EditCtx ctx);
}


// ****************
// * MultipleEnum *
// ****************
public class MultipleEnumEditAttribute : EditAttribute
{
	public MultipleEnumEditAttribute(string? text = null) : base(text)
	{
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from propType in ctx.PropNfo.GetElementType()
		from ctrl in this.CallBuildGen(nameof(BuildGen), propType, ctx.RxVar, ctx.PropNfo.Name, ctx.D)
		select ctrl
	).Ensure();

	private Control BuildGen<E>(IFullRwBndVar<E[]> rxVar, string propName, IRoDispBase d) where E : struct, Enum =>
		new CtrlDropdownButton(
				Text ?? propName,
				Enum.GetNames<E>(),
				true
			)
			.ControlWith(rxVar.Convert(Enum2Index, Index2Enum<E>, d), d);

	private static int[] Enum2Index<E>(E[] v) where E : struct, Enum => Enum.GetValues<E>().Select((e, i) => (e, i)).Where(t => v.Contains(t.e)).SelectToArray(t => t.i);
	private static E[] Index2Enum<E>(int[] v) where E : struct, Enum => v.SelectToArray(e => Enum.GetValues<E>()[e]);
}


// **************
// * SingleEnum *
// **************
public class SingleEnumEdit : EditAttribute
{
	public SingleEnumEdit(string? text = null) : base(text)
	{
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from ctrl in this.CallBuildGen(nameof(BuildGen), ctx.PropNfo.PropertyType, ctx.RxVar, ctx.PropNfo.Name, ctx.D)
		select ctrl
	).Ensure();

	private Control BuildGen<E>(IFullRwBndVar<E> rxVar, string propName, IRoDispBase d) where E : struct, Enum =>
		new CtrlDropdownButton(
				Text ?? propName,
				Enum.GetNames<E>(),
				false
			)
			.ControlWith(rxVar.Convert(Enum2Index, Index2Enum<E>, d), d);

	private static int[] Enum2Index<E>(E v) where E : struct, Enum => new[] { Enum.GetValues<E>().IndexOf(v) };
	private static E Index2Enum<E>(int[] v) where E : struct, Enum => v.Select(e => Enum.GetValues<E>()[e]).First();
}


// ***********
// * IntList *
// ***********
public class IntListEdit : EditAttribute
{
	public IntListEdit(string? text = null) : base(text)
	{
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from rxVarTyped in ctx.RxVar.ToTypedVar<int[]>()
		select BuildGen(rxVarTyped, ctx.PropNfo.Name, ctx.D)
	).Ensure();

	private Control BuildGen(IFullRwBndVar<int[]> rxVar, string propName, IRoDispBase d) =>
		new CtrlTextBox(Text ?? propName)
			.ControlWith(rxVar.Convert(Ints2Str, Str2Ints, d), d);

	private static string Ints2Str(int[] v) => v.JoinText(",");

	private static int[] Str2Ints(string v) => v
		.Chop(',')
		.Where(e => int.TryParse(e, out _))
		.SelectToArray(int.Parse);
}


// *************
// * TxtSearch *
// *************
public class TxtSearchEdit : EditAttribute
{
	public TxtSearchEdit(string? text = null) : base(text)
	{
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from rxVarTyped in ctx.RxVar.ToTypedVar<TxtSearch>()
		select BuildGen(rxVarTyped, ctx.PropNfo.Name, ctx.D)
	).Ensure();

	private Control BuildGen(IFullRwBndVar<TxtSearch> rxVar, string propName, IRoDispBase d) =>
		new CtrlTextBox(Text ?? propName)
			.ControlWith(rxVar.Convert(Search2Str, Str2Search, d), d);

	private static string Search2Str(TxtSearch v) => v.Text;

	private static TxtSearch Str2Search(string v) => new(v);
}


// ************
// * IntRange *
// ************
public class IntRangeEdit : EditAttribute
{
	public int Min { get; }
	public int Max { get; }
	public int Step { get; }
	public int Width { get; }
	public Skin Skin { get; }
	public string? Prefix { get; }

	public IntRangeEdit(
		int min,
		int max,
		int step = 1,
		int width = 400,
		Skin skin = Skin.Sharp,
		string? prefix = null,
		string? text = null
	) : base(text)
	{
		Min = min;
		Max = max;
		Step = step;
		Width = width;
		Skin = skin;
		Prefix = prefix;
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from rxVarTyped in ctx.RxVar.ToTypedVar<RngInt>()
		select BuildGen(rxVarTyped, ctx.PropNfo.Name, ctx.D)
	).Ensure();

	private Control BuildGen(IFullRwBndVar<RngInt> rxVar, string propName, IRoDispBase d) =>
		new CtrlSlider(opt =>
			{
				opt.Label = Text ?? propName;
				opt.Width = Width;

				opt.Type = SliderType.Double;
				opt.Skin = Skin;
				opt.Min = opt.FromMin = opt.ToMin = Min;
				opt.Max = opt.FromMax = opt.ToMax = Max;
				opt.Step = Step;
				opt.Prefix = Prefix;
				opt.Grid = true;
			})
			.ControlWith(rxVar, d);
}


// *************
// * TimeRange *
// *************
public class TimeRangeEdit : EditAttribute
{
	private static int idCnt;

	public int MaxTicks { get; }
	public string ParamName { get; }
	public int Width { get; }
	public Skin Skin { get; }

	public TimeRangeEdit(
		int maxTicks,
		string paramName,
		int width = 400,
		Skin skin = Skin.Sharp,
		string? text = null
	) : base(text)
	{
		MaxTicks = maxTicks;
		ParamName = paramName;
		Width = width;
		Skin = skin;
	}

	internal override Control Build(EditCtx ctx) =>
	(
		from rxVarTyped in ctx.RxVar.ToTypedVar<RngTime>()
		from allTimes in ctx.ExtraParams.ReadExtraParam<DateTime[]>(ParamName)
		select BuildGen(rxVarTyped, ctx.PropNfo.Name, ctx.D, RngTimeUtils.MakeTicks(allTimes, MaxTicks))
	).Ensure();

	private Control BuildGen(IFullRwBndVar<RngTime> rxVar, string propName, IRoDispBase d, DateTime[] ticks)
	{
		var fmtArrName = $"timeStrings{idCnt++}";
		var arrStr = ticks.Select((e, i) =>
		{
			if (i == 0 || i == ticks.Length - 1)
				return "'_'";
			else
				return $"'{e:yyyy-MM-dd}'";
		}).JoinText(",");
		Util.InvokeScript(false, "eval", $"var {fmtArrName} = [{arrStr}];");

		return new CtrlSlider(opt =>
			{
				opt.Label = Text ?? propName;
				opt.Width = Width;

				opt.Type = SliderType.Double;
				opt.Skin = Skin;
				opt.Min = opt.FromMin = opt.ToMin = 0;
				opt.Max = opt.FromMax = opt.ToMax = ticks.Length - 1;
				opt.Step = 1;
				opt.PrettifyEnabled = true;
				opt.Prettify = $$"""function fmt(n) { return {{fmtArrName}}[n]; }""";
			})
			.ControlWith(rxVar.Convert(e => Time2Int(e, ticks), e => Int2Time(e, ticks), d), d);
	}

	private static RngInt Time2Int(RngTime v, DateTime[] ticks) => new(
		v.Min.HasValue switch
		{
			true => ticks.IndexOf(v.Min.Value),
			false => null,
		},
		v.Max.HasValue switch
		{
			true => ticks.IndexOf(v.Max.Value),
			false => null,
		}
	);

	private static RngTime Int2Time(RngInt v, DateTime[] ticks) => new(
		v.Min.HasValue switch
		{
			true => ticks[v.Min.Value],
			false => null
		},
		v.Max.HasValue switch
		{
			true => ticks[v.Max.Value],
			false => null
		}
	);
}







file static class EditorReflectionExt
{
	public static MaybeErr<T> ReadExtraParam<T>(this IReadOnlyDictionary<string, object> extraParams, string paramName) =>
		extraParams.TryGetValue(paramName, out var paramObj) switch
		{
			false => MayErr.None<T>($"Expected an extra param named '{paramName}' but could not find it"),
			true => paramObj switch
			{
				T paramVal => MayErr.Some(paramVal),
				_ => MayErr.None<T>($"Extra param named '{paramName}' is expected to have a type: '{typeof(T).Name}' but instead has a type of '{paramObj?.GetType().Name}'"),
			}
		};

	public static MaybeErr<IFullRwBndVar<T>> ToTypedVar<T>(this object rxVar) => rxVar switch
	{
		IFullRwBndVar<T> rxVarTyped => MayErr.Some(rxVarTyped),
		_ => MayErr.None<IFullRwBndVar<T>>($"Expected an 'IFullRwBndVar<{typeof(T).Name}>' but got an '{rxVar.GetType().Name}' instead")
	};

	public static MaybeErr<Type> GetElementType(this PropertyInfo propNfo) => propNfo.PropertyType.GetElementType().ToMaybeErr($"Expected an array for the '{propNfo.Name}' field, but got a '{propNfo.PropertyType.Name}' instead");

	public static MaybeErr<Control> CallBuildGen<T>(
		this T self,
		string methName,
		Type propType,
		object rxVar,
		string propName,
		IRoDispBase d
	)
	{
		var meth = typeof(T).GetMethod(methName, BindingFlags.NonPublic | BindingFlags.Instance);
		if (meth == null) return MayErr.None<Control>($"Failed to find method '{methName}' in '{typeof(T).Name}'");

		if (meth.GetGenericArguments().Length != 1) return MayErr.None<Control>($"Expected method '{methName}' in '{typeof(T).Name}' to have exactly one generic argument (got {meth.GetGenericArguments().Length} instead)");
		var methGen = meth.MakeGenericMethod(propType);

		var ctrlObj = methGen.Invoke(self, new[] { rxVar, propName, d });
		if (ctrlObj == null) return MayErr.None<Control>($"Method '{methName}' in '{typeof(T).Name}' returned null");

		if (ctrlObj is not Control ctrl) return MayErr.None<Control>($"Method '{methName}' in '{typeof(T).Name}' returned a '{ctrlObj.GetType().Name}' instead of a Control");

		return MayErr.Some(ctrl);
	}
}



file static class EditorUtils
{
	public static IFullRwBndVar<U> Convert<T, U>(this IFullRwBndVar<T> rxVar, Func<T, U> mapNext, Func<U, T> mapPrev, IRoDispBase d)
	{
		var rxCnv = Var.MakeBnd(mapNext(rxVar.V)).D(d);
		rxVar.WhenOuter.Select(mapNext).PipeToOuter(rxCnv, d);
		rxCnv.WhenInner.Select(mapPrev).PipeToInner(rxVar, d);
		return rxCnv;
	}

	public static Control ControlWith<C, T>(this C ctrl, IFullRwBndVar<T> rxVar, IRoDispBase d) where C : Control, IBoundCtrl<T>
	{
		rxVar.WhenOuterOrInit().PipeToOuter(ctrl.RxVar, d);
		ctrl.RxVar.WhenInner.PipeToInner(rxVar, d);
		return ctrl;
	}

	private static void PipeToOuter<U>(this IObservable<U> obsProp, IRwBndVar<U> rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetOuter).D(d);
	private static void PipeToInner<U>(this IObservable<U> obsProp, IFullRwBndVar<U> rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetInner).D(d);
}
