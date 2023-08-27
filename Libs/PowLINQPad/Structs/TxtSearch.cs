using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Structs;


public class TxtSearch
{
	public string Text { get; }

	[JsonIgnore]
	public string[]? Parts { get; }

	public TxtSearch(string text)
	{
		Text = text;
		Parts = Text.Chop(' ');
	}

	public static readonly TxtSearch Empty = new(string.Empty);
}


/*
public class TxtSearch
{
	public bool UseRegex { get; }
	public string Text { get; }

	[JsonIgnore]
	public string[]? Parts { get; }

	[JsonIgnore]
	public Regex? Regex { get; }

	[JsonIgnore]
	public bool IsError => UseRegex && Regex == null;

	public TxtSearch(bool useRegex, string text)
	{
		UseRegex = useRegex;
		Text = text;
		Parts = UseRegex switch
		{
			false => Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
			true => null
		};
		Regex = UseRegex switch
		{
			false => null,
			true => TxtSearchUtils.MkRegex(Text)
		};
	}

	public static readonly TxtSearch Empty = new(false, string.Empty);
}


static class TxtSearchExt
{
	public static TxtSearch SetUseRegex(this TxtSearch t, bool v) => new(v, t.Text);
	public static TxtSearch SetText(this TxtSearch t, string s) => new(t.UseRegex, s);
}


file static class TxtSearchUtils
{
	public static Regex? MkRegex(string str)
	{
		try
		{
			var regex = new Regex(str, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			return regex;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
*/