namespace PowLINQPad.Flex_.StructsInternal;

interface ICssFlex { }

sealed record FixCssFlex(int Val) : ICssFlex;
sealed record AutoCssFlex : ICssFlex;
sealed record FillCssFlex : ICssFlex;

static class CssFlex
{
	// @formatter:off
	public static			ICssFlex Fix(int val)	=> new FixCssFlex(val);
	public static readonly	ICssFlex Auto			= new AutoCssFlex();
	public static readonly	ICssFlex Fill			= new FillCssFlex();
	// @formatter:on
}