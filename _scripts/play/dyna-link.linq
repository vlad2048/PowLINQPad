<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


record Filter(
	int Val,
	string Name
)
{
	public static readonly Filter Empty = new(123, "Vlad");
	public static readonly Filter F1 = new(123, "Erik");
	public static readonly Filter F2 = new(456, "Erik");
}


void Main()
{
	var filter = Var.MakeBnd(Filter.Empty).D(D);
	var arr = filter.Split(D);
	var rxVal = (IFullRwBndVar<int>)arr[0];
	var rxName = (IFullRwBndVar<string>)arr[1];
	
	filter.Log();
	filter.WhenOuter.Log();
	filter.WhenInner.Log();
	rxVal.Log();
	rxVal.WhenOuter.Log();
	rxVal.WhenInner.Log();
	rxName.Log();
	rxName.WhenOuter.Log();
	rxName.WhenInner.Log();
	
	Thread.Sleep(2000);
	
	filter.SetOuter(Filter.F1);
	
	Thread.Sleep(2000);
	
	rxName.SetInner("Milou");
}





public static class VarSplitter
{
	public static object[] Split<T>(this IFullRwBndVar<T> rxVar, IRoDispBase d) =>
		typeof(T)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.SelectToArray(propNfo => rxVar.SplitProp(d, propNfo));
			
	
	private static object SplitProp<T>(this IFullRwBndVar<T> rxVar, IRoDispBase d, PropertyInfo propNfo)
	{
		// rxProp = Var.MakeBnd(rxVar.V.[PROP]);
		// =====================================
		var propType = propNfo.PropertyType;
		var rxProp = GenCall.MakeBnd(propNfo.GetValue(rxVar.V)!, d);
		
		// rxVar.WhenOuter.Select(v => v.[PROP]).PipeToOuter(rxProp, d);
		// =============================================================
		object Get(T v) => propNfo.GetValue(v)!;
		var getConv = GenCall.ConvertDelegate<T>(Get, propType);		
		var whenRxVarOuter	= GenCall.Select(rxVar.WhenOuter, getConv, propType);
		GenCall.PipeToOuter(whenRxVarOuter, rxProp, d, propType);
		
		// rxProp.WhenInner.Select(p => rxVar.V with { [PROP] = p }).PipeToInner(rxVar, d);
		// ================================================================================
		var whenRxPropInner	= GenCall.GetWhenInner(rxProp, propType);
		T GetBack(object p) => RecordUtils.ConstructWith(rxVar.V, p, propNfo);
		var getBackConv = GenCall.ConvertBackDelegate(GetBack, propType);
		var whenRxPropInnerMap = GenCall.Select(whenRxPropInner, getBackConv, typeof(T), propType);
		GenCall.PipeToInner(whenRxPropInnerMap, rxVar, d, typeof(T));
		
		return rxProp;
	}
	
}



public static class GenCall
{
	private static readonly MethodInfo makeBnd = typeof(GenCall).GetPrivateMethod("MakeBnd")!;
	private static readonly MethodInfo convertDelegate = typeof(GenCall).GetPrivateMethod("ConvertDelegate")!;
	private static readonly MethodInfo convertBackDelegate = typeof(GenCall).GetPrivateMethod("ConvertBackDelegate")!;
	private static readonly MethodInfo select = typeof(GenCall).GetPrivateMethod("Select")!;
	private static readonly MethodInfo pipeToOuter = typeof(GenCall).GetPrivateMethod("PipeToOuter")!;
	private static readonly MethodInfo pipeToInner = typeof(GenCall).GetPrivateMethod("PipeToInner")!;
	
	private	static	IFullRwBndVar<T>	MakeBnd<T>	(T		val, IRoDispBase d) => Var.MakeBnd(val).D(d);
	public	static	object				MakeBnd		(object	val, IRoDispBase d) => makeBnd.MakeGenericMethod(val.GetType()).CallStatic(val, d);
	
	private	static	Func<T, U>			ConvertDelegate<T, U>		(Func<T, object>	fun) => v => (U)fun(v);
	public	static	object				ConvertDelegate<T>			(Func<T, object>	fun, Type propType) => convertDelegate.MakeGenericMethod(typeof(T), propType).CallStatic(fun);
	
	private	static	Func<U, T>			ConvertBackDelegate<T, U>	(Func<object, T>	fun) => p => fun(p!);
	public	static	object				ConvertBackDelegate<T>		(Func<object, T>	fun, Type propType) => convertBackDelegate.MakeGenericMethod(typeof(T), propType).CallStatic(fun);
	
	private static	IObservable<U>		Select<T, U>			(IObservable<T> varObs,		Func<T, U>		fun) => varObs.Select(fun);
	public	static	object				Select<T>				(IObservable<T> varObs,		object			fun, Type propType) => select.MakeGenericMethod(typeof(T), propType).CallStatic(varObs, fun);
	public	static	object				Select				(object			propObs,	object			fun, Type varType, Type propType) => select.MakeGenericMethod(propType, varType).CallStatic(propObs, fun);

	private	static void	PipeToOuter<U>	(IObservable<U> obsProp, IFullRwBndVar<U>	rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetOuter).D(d);
	public	static void PipeToOuter		(object			obsProp, object				rxProp, IRoDispBase d, Type propType) => pipeToOuter.MakeGenericMethod(propType).CallStatic(obsProp, rxProp, d);
	
	private static void	PipeToInner<U>	(IObservable<U> obsProp, IFullRwBndVar<U>	rxProp, IRoDispBase d) => obsProp.Subscribe(rxProp.SetInner).D(d);
	public	static void PipeToInner		(object			obsProp, object				rxProp, IRoDispBase d, Type propType) => pipeToInner.MakeGenericMethod(propType).CallStatic(obsProp, rxProp, d);
	
	public static object GetWhenInner(object rxProp, Type propType) => typeof(IRoBndVar<>).MakeGenericType(propType).GetProperty("WhenInner")!.GetValue(rxProp)!;
}


public static class ReflectionExt
{
	public static MethodInfo GetPrivateMethod(this Type t, string name) => t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;
	public static object CallStatic(this MethodInfo methodNfo, params object[] parameters) => methodNfo.Invoke(null, parameters)!;
	public static MethodInfo GetGetter(this PropertyInfo propNfo) => propNfo.GetAccessors().First(e => e.Name.StartsWith("get_"));
}



public static class RecordUtils
{
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











public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
