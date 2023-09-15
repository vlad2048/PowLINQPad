using LINQPad;
using PowBasics.CollectionsExt;
using PowBasics.StringsExt;

namespace PowLINQPad.Utils.Css_.Utils;

sealed record CssKeyVal(string Key, string Val);

static class CssUtils
{
    public static void Init() => clsMap.Clear();

    public static string? GetClass(string? css)
    {
        if (css == null) return null;
        string MkCls(int i) => $"cls-{i}";

        var id = clsMap.GetOrCreate(css, () =>
        {
            var i = clsId++;
            Util.HtmlHead.AddStyles($$"""
				.{{MkCls(i)}} {
					{{css}}
				}
			""");
            return i;
        });

        return MkCls(id);
    }

    public static CssKeyVal[] ParseCss(string css)
    {
        if (css.Contains('{') || css.Contains('}')) throw new ArgumentException("Cannot apply CSS with braces in LINQPad");
        return (
                from line in css.SplitInLines()
                from subLine in line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                let keyVal = ParseLine(subLine)
                where keyVal != null
                select keyVal
            )
            .ToArray();
    }

    private static CssKeyVal? ParseLine(string line)
    {
        var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2) return null;
        return new CssKeyVal(parts[0], parts[1]);
    }


    private static int clsId;
    private static readonly Dictionary<string, int> clsMap = new();
}