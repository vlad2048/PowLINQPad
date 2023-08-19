namespace PowLINQPad.Interactions.Paging;

public static class PagerMaker
{
	public static (IRoVar<T[]>, IDisp) MakePager<T>(this IRoVar<T[]> items, out PagerState state, int pageSize)
	{
		var d = new Disp();
		var pageIndex = Var.MakeBnd(0).D(d);
		var pageCount = items.SelectVar(arr => CalcPageCount(arr.Length, pageSize));

		state = new PagerState(
			pageIndex,
			pageCount,
			() =>
			{
				if (pageIndex.V <= 0) return;
				pageIndex.SetInner(pageIndex.V - 1);
			},
			() =>
			{
				if (pageIndex.V >= pageCount.V - 1) return;
				pageIndex.SetInner(pageIndex.V + 1);
			},
			pageSize
		);

		var itemsFiltered = Var.Expr(() => items.V.Skip(pageIndex.V * pageSize).Take(pageSize).ToArray());

		return (itemsFiltered, d);
	}


	private static int CalcPageCount(int total, int pageSize) => total switch
	{
		0 => 1,
		_ => Math.Max(1, (total - 1) / pageSize + 1)
	};
}