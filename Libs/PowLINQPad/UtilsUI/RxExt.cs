using System.Reactive.Linq;

namespace PowLINQPad.UtilsUI;

static class RxExt
{
	public static IObservable<T> WhenOuterOrInit<T>(this IRwBndVar<T> rxVar) => rxVar.WhenOuter.Prepend(rxVar.V);
}