using System.Reactive;

namespace PowLINQPad.Interactions.Paging;

public sealed record PagerState(
	IRwVar<int> PageIndex,
	IRoVar<int> PageCount,
	Action DecPage,
	Action IncPage,
	int PageSize
)
{
	public IRoVar<bool> CanDecPage => Var.Expr(() => PageIndex.V > 0);
	public IRoVar<bool> CanIncPage => Var.Expr(() => PageIndex.V < PageCount.V - 1);
	public IObservable<Unit> WhenChanged => Obs.Merge(PageIndex, PageCount).ToUnit();
}