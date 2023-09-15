using System.Runtime.CompilerServices;
using LINQPad;

namespace PowLINQPad.Utils;

public static class DbgExt
{
	public static void Log<T>(this IObservable<T> obs, [CallerArgumentExpression(nameof(obs))] string? obsExpr = null) => obs.Subscribe(v => $"[{DateTime.Now:HH:mm:ss}]-[{obsExpr}] <- {v}".Dump()).D(RxUI.D);
}