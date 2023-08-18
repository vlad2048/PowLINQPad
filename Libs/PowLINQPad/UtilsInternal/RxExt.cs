using System.Reactive.Disposables;

namespace PowLINQPad.UtilsInternal;

static class RxExt
{
	public static (IFullRwBndVar<T>, IDisp) ToBnd<T>(this IRwVar<T> rxVar) => rxVar switch
	{
		IFullRwBndVar<T> bndVar => (bndVar, Disposable.Empty),
		_ => rxVar.ToBndMake()
	};

	private static (IFullRwBndVar<T>, IDisp) ToBndMake<T>(this IRwVar<T> rxVar)
	{
		var d = new Disp();
		var rxBndVar = Var.MakeBnd(rxVar.V).D(d);
		rxVar.Subscribe(rxBndVar.SetOuter).D(d);
		rxBndVar.WhenInner.Subscribe(v => rxVar.V = v).D(d);
		return (rxBndVar, d);
	}
}