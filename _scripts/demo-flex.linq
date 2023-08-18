<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad.Flex_.Structs</Namespace>
  <Namespace>PowLINQPad.Flex_</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>PowLINQPad</Namespace>
</Query>

using static PowLINQPad.Flex_.Structs.Dims;

void Main() =>
	Flex.Vert(Fil,
		Flex.Horz(FilFix(65),
			new Span("Header Start"),
			new Span("Header End")
		),
		
		Flex.Horz(Fil,
			Flex.Scroll(Fil,
				Enumerable.Range(0, 30).SelectToArray(e => new Span($"Left Item: {e}"))
			),
			Flex.Scroll(Fil,
				Enumerable.Range(0, 60).SelectToArray(e => new Span($"Right Item: {e}"))
			)
		),
		
		Flex.Horz(FilFit,
			new Span("Footer")
		)
	)
		.Build(true)
		.Dump();

public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
