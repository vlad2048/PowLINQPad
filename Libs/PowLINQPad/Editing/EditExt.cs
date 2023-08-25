using System.Reactive.Linq;
using System.Reflection;
using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.Editing._Base;
using PowLINQPad.Editing.Editors_;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Editing;



public static class EditExt
{
	public static Control MakeEditor<T>(this IFullRwBndVar<T> rxVar, IRoDispBase d)
	{
		var ctrls = (
			from prop in typeof(T).GetProperties()
			let attr = Attribute.GetCustomAttributes(prop).FirstOrDefault(attr => attr is EditAttribute)
			where attr != null
			let access = rxVar.BreakdownProp(prop, d)
			select CreateCtrl(prop, (EditAttribute)attr, access, d)
		)
			.ToArray();

		var ui = new Div(ctrls).Css("""
			display: flex;
		""");
		return ui;
	}

	private static Control CreateCtrl(PropertyInfo prop, EditAttribute attrBase, PropAccess access, IRoDispBase d) =>
		attrBase switch
		{
			SingleEnumEditAttribute attr => CallMake(typeof(SingleEnumEditor), prop.PropertyType, access, d, attr.Text ?? prop.Name),
			MultipleEnumEditAttribute attr => CallMake(typeof(MultipleEnumEditor), prop.PropertyType.GetElementType()!, access, d, attr.Text ?? prop.Name),
			_ => throw new ArgumentException("Uknown editor")
		};


	private static Control CallMake(Type editorType, Type makeType, PropAccess access, IRoDispBase d, params object[] extraParams)
	{
		var ps = new List<object>
		{
			access,
			d
		};
		ps.AddRange(extraParams);
		return (Control)editorType.GetMethod("Make")!.MakeGenericMethod(makeType).Invoke(null, ps.ToArray())!;
	}

	

	private static PropAccess BreakdownProp<T>(this IFullRwBndVar<T> rxVar, PropertyInfo prop, IRoDispBase d)
	{
		object Get() => prop.GetValue(rxVar.V)!;
		void Set(object obj)
		{
			var valNext = Utils.ConstructWith(rxVar.V, obj, prop);
			rxVar.SetInner(valNext);
		}
		return new PropAccess(prop, Get, Set);
	}
}


public sealed record PropAccess(
	PropertyInfo PropNfo,
	Func<object> Get,
	Action<object> Set
)
{
	public PropAccess<T> ToTyped<T>() => new(
		PropNfo,
		() => (T)Get(),
		obj => Set(obj!)
	);
}

public sealed record PropAccess<T>(
	PropertyInfo PropNfo,
	Func<T> Get,
	Action<T> Set
);



/*
public static class EditExt
{
	//public static Control MakeEditor<T>(this IFullRwBndVar<T> rxVar, IRoDispBase d)
	//{
	//	var props = 
	//}


	public static object[] Breakdown<T>(this IFullRwBndVar<T> rxVar, IRoDispBase d)
	{
		var t = typeof(T);
		var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		return props.SelectToArray(prop => BreakdownProp(rxVar, prop, d));
	}

	private static object BreakdownProp<T>(this IFullRwBndVar<T> rxVar, PropertyInfo prop, IRoDispBase d)
	{
		var initVal = prop.GetValue(rxVar.V)!;
		var rxPropVar = GenCall.MakeBnd(initVal, d);

		GenCall.PipeToOuter(
			//rxVar.WhenOuter.Select(e => prop.GetValue(e)),
			GenCall.Select(rxVar.WhenOuter, MakeDelegate<T>(prop.PropertyType, prop), prop.PropertyType),
			rxPropVar,
			d
		);
		//GenCall.PipeToInner(
		//	rxVar.WhenOuter.Select(e => prop.GetValue(e)),
		//	rxPropVar,
		//	d
		//);
		return rxPropVar;
	}

	private static Delegate MakeDelegate<T>(Type destType, PropertyInfo prop) =>
		Delegate.CreateDelegate(
			typeof(Func<,>).MakeGenericType(typeof(T), destType),
			prop.GetAccessors()[0]
		);
	

	private sealed record PropNfo(
		Func<object> Get,
		Action<object> Set
	);

	private static PropNfo[] GetProps<T>(IFullRwBndVar<T> rxVar) =>
		typeof(T)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.SelectToArray(p => GetProp(rxVar, p));
	
	private static PropNfo GetProp<T>(IFullRwBndVar<T> rxVar, PropertyInfo prop)
	{
		object Get() => prop.GetValue(rxVar.V)!;
		void Set(object obj)
		{
			var valNext = Utils.ConstructWith(rxVar.V, obj, prop);
			rxVar.SetInner(valNext);
		}
		return new PropNfo(Get, Set);
	}
}


file static class GenCall
{
	private static readonly MethodInfo makeBnd = typeof(Utils).GetMethod("MakeBnd")!;
	private static readonly MethodInfo select = typeof(Obs).GetMethods().First(e => e.Name == "Select");
	private static readonly MethodInfo pipeToOuter = typeof(Utils).GetMethod("PipeToOuter")!;
	private static readonly MethodInfo pipeToInner = typeof(Utils).GetMethod("PipeToInner")!;

	public static object MakeBnd(object val, IRoDispBase d) =>
		makeBnd.MakeGenericMethod(val.GetType()).Invoke(null, new[] { val, d })!;

	public static object Select(object obs, object fun, Type destType) =>
		select.MakeGenericMethod(
				obs.GetType().GenericTypeArguments[0],
				destType
			)
			.Invoke(null, new[] { obs, fun })!;

	public static void PipeToOuter(object obs, object rxVar, IRoDispBase d) =>
		pipeToOuter.MakeGenericMethod(obs.GetType().GenericTypeArguments[0]).Invoke(null, new[] { obs, rxVar, d });

	public static void PipeToInner(object obs, object rxVar, IRoDispBase d) =>
		pipeToInner.MakeGenericMethod(obs.GetType().GenericTypeArguments[0]).Invoke(null, new[] { obs, rxVar, d });
}*/


file static class Utils
{
	public static IFullRwBndVar<T> MakeBnd<T>(T val, IRoDispBase d) => Var.MakeBnd(val).D(d);

	public static void PipeToOuter<T>(this IObservable<T> obs, IFullRwBndVar<T> rxVar, IRoDispBase d) => obs.Subscribe(rxVar.SetOuter).D(d);
	public static void PipeToInner<T>(this IObservable<T> obs, IFullRwBndVar<T> rxVar, IRoDispBase d) => obs.Subscribe(rxVar.SetInner).D(d);

	public static T ConstructWith<T>(T rec, object propVal, PropertyInfo nfo)
	{
		var t = typeof(T);
		var constrs = t.GetConstructors();
		if (constrs.Length != 1) throw new ArgumentException("not 1 constructor exactly");
		var constr = constrs[0];
		var constrParams = constr.GetParameters();
		var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		VerifyParamsMatch(constrParams, props);
		
		var args = new object[props.Length];
		for (var i = 0; i < props.Length; i++)
		{
			var prop = props[i];
			if (prop.Name == nfo.Name && prop.PropertyType == nfo.PropertyType)
				args[i] = propVal;
			else
				args[i] = prop.GetValue(rec)!;
		}
		
		var res = constr.Invoke(args);		
		
		return (T)res;
	}
	
	private static void VerifyParamsMatch(ParameterInfo[] cs, PropertyInfo[] ps)
	{
		var isMatch = cs.Length == ps.Length && cs.Zip(ps).Select(t => (c: t.First, p: t.Second)).All(t => t.c.Name == t.p.Name && t.c.ParameterType == t.p.PropertyType);
		if (!isMatch)
			throw new ArgumentException("constructor params do not match properties");
	}
}