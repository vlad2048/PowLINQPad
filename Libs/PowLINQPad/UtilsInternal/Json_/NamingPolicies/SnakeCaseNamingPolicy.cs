using System.Text.Json;

namespace PowLINQPad.UtilsInternal.Json_.NamingPolicies;

sealed class SnakeCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name) => name.ToSnakeCase();
}


file static class Utils
{
	public static string ToSnakeCase(this string str) => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
}