using PowBasics.CollectionsExt;

namespace PowLINQPad.Structs;

public sealed record RngTime(DateTime? Min, DateTime? Max)
{
	public static readonly RngTime Empty = new(null, null);
	public override string ToString() => $"[{f(Min)} - {f(Max)}]";
	private static string f(DateTime? v) => v.HasValue ? $"{v:yyyy-MM-dd HH:mm}" : "_";

    public static DateTime[] SampleTimes => RngTimeSampleData.Data;
}



static class RngTimeUtils
{
	public static DateTime[] MakeTicks(IEnumerable<DateTime> times, int maxTicks) =>
		ComputeTimes(
			times
				.OrderBy(e => e)
				.GroupBy(e => e)
				.SelectToArray(g => new T(
					g.Key,
					g.Count())
				),
			maxTicks
		);

	private static DateTime[] ComputeTimes(T[] arr, int maxTicks) => arr.Length switch
	{
		0 => new[] { DateTime.MinValue },
		1 => new[] { arr[0].Time },
		2 => new[] { arr[0].Time, arr[1].Time },
		_ when maxTicks >= arr.Length => arr.SelectToArray(e => e.Time),
		_ => ComputeSampling(arr, maxTicks)
	};

	private static DateTime[] ComputeSampling(T[] arr, int maxTicks)
	{
		var flat = arr.SelectMany(e => Enumerable.Range(0, e.Cnt).Select(_ => e.Time)).ToArray();
		var list = new List<DateTime>();
		var idxInc = (double)flat.Length / maxTicks;
		for (var i = 0; i < maxTicks - 1; i++)
		{
			var idx = (int)(idxInc * i);
			var elt = flat[idx];
			list.Add(elt);
		}
		list.Add(flat.Last());
		return list.Distinct().ToArray();
	}

	private sealed record T(DateTime Time, int Cnt);
}


/*sealed class RngTimeBounds
{
	public DateTime[] Times { get; }
	private RngTimeBounds(DateTime[] times) => Times = times;

	public static RngTimeBounds Make(IEnumerable<DateTime> allTimes, int maxTicks) => new(
		ComputeTimes(
			allTimes.OrderBy(e => e).GroupBy(e => e).SelectToArray(g => new T(g.Key, g.Count())),
			maxTicks
		)
	);
	
	private static DateTime[] ComputeTimes(T[] arr, int maxTicks) => arr.Length switch
	{
		0 => new[] { DateTime.MinValue },
		1 => new[] { arr[0].Time },
		2 => new[] { arr[0].Time, arr[1].Time },
		_ when maxTicks >= arr.Length => arr.SelectToArray(e => e.Time),
		_ => ComputeSampling(arr, maxTicks)
	};
	
	private static DateTime[] ComputeSampling(T[] arr, int maxTicks)
	{
		var flat = arr.SelectMany(e => Enumerable.Range(0, e.Cnt).Select(_ => e.Time)).ToArray();
		var list = new List<DateTime>();
		var idxInc = (double)flat.Length / maxTicks;
		for (var i = 0; i < maxTicks - 1; i++)
		{
			var idx = (int)(idxInc * i);
			var elt = flat[idx];
			list.Add(elt);
		}
		list.Add(flat.Last());
		return list.Distinct().ToArray();
	}
	
	private sealed record T(DateTime Time, int Cnt);
}*/