<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad.Flex_.Structs</Namespace>
  <Namespace>PowLINQPad.Flex_</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>PowLINQPad</Namespace>
</Query>

void Main() =>
	Flex.Vert(Dim.Fill, Dim.Fill,
		Flex.Horz(Dim.Fill, Dim.Auto,
			new Span("Header Start"),
			new Span("Header End")
		),
		
		Flex.Horz(Dim.Fill, Dim.Fill,
			Flex.Scroll(Dim.Fill, Dim.Fill,
				Enumerable.Range(0, 30).SelectToArray(e => new Span($"Left Item: {e}"))
			),
			Flex.Scroll(Dim.Fill, Dim.Fill,
				Enumerable.Range(0, 60).SelectToArray(e => new Span($"Right Item: {e}"))
			)
		),
		
		Flex.Horz(Dim.Fill, Dim.Auto,
			new Span("Footer")
		)
	)
		.Build(true)
		.Dump();

public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
