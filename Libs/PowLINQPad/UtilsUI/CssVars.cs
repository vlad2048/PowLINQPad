using System.Runtime.CompilerServices;
using LINQPad;

namespace PowLINQPad.UtilsUI;

public static class CssVars
{

	public static string v(this string val, [CallerArgumentExpression(nameof(val))] string? valExpr = null) => Get(val, valExpr);




	private static readonly Dictionary<string, string> varMap = new();
	private static readonly HashSet<string> nonVarSet = new();
	

	internal static void Init()
	{
		varMap.Clear();
		nonVarSet.Clear();
	}



	private static string Get(string val, [CallerArgumentExpression(nameof(val))] string? valExpr = null)
	{
		var varName = GetValName(valExpr);
		if (varName == null || nonVarSet.Contains(varName)) return val;

		if (varMap.TryGetValue(varName, out var existingVal))
		{
			if (val != existingVal)
			{
				varMap.Remove(varName);
				nonVarSet.Add(varName);
				return val;
			}
		}
		else
		{
			SetJS(varName, val);
			varMap[varName] = val;
		}
		return $"var(--{varName})";
	}

	
	private static void SetJS(string name, string val) =>
		Util.HtmlHead.AddStyles($$"""
			:root {
				--{{name}}: {{val}};
			}
		""");

	private static string? GetValName(string? expr)
	{
		if (string.IsNullOrEmpty(expr)) return null;
		var c = expr[0];
		if (c is '$' or '@' or '"') return null;
		if (expr.Contains(' ')) return null;
		return expr
			.Replace(".", "");
	}
}