<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.Editing.Controls_</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.Editing.Controls_.Slider_</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	new CtrlSlider(opt =>
	{
		opt.Width = 400;
		opt.Type = SliderType.Double;
		opt.Skin = Skin.Sharp;
		
		opt.Step = 5;
		opt.Min = opt.FromMin = opt.ToMin = 0;
		opt.Max = opt.FromMax = opt.ToMax = 60;
		(opt.From, opt.To) = (5, 40);
		
		opt.Grid = true;
		opt.GridNum = 4;
		
	}).Dump();
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
