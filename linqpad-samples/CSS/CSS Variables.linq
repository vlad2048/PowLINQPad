<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

void Main()
{
	new Span("vlad")
		.Css($"color: {Cols.MyCol}")
		.Dump();
}

static class Cols
{
	public static readonly string MyCol = CssVar.Make("mycol", "green");
}