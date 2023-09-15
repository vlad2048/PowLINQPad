<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
</Query>

void Main()
{
	new Span("vlad1").Css($"color: {Cols.ColGreen.v()}").Dump();
	new Span("vlad2").Css($"color: {Cols.ColRed.v()}").Dump();

	new Button("Edit", _ => PowLINQPad.UtilsUI.HtmlExporter.OpenInBrowser()).Dump();
}


static class Cols
{
	public static readonly string ColGreen = "green";
	public static readonly string ColRed = "red";
}


public static Disp D => RxUI.D;

void OnStart() => RxUI.Start();
