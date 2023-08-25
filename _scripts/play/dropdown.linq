<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.Controls_.Dropdown_</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var btn = new DropdownButton("Choice", new [] { "One", "Two", "Three", "Four" }, false).D(D);
	var val = btn.V.Select(e => e.JoinText()).ToDC(D);
	Util.VerticalRun(
		btn,
		val
	).Dump();
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
