<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.RxControls.Slider_</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var dc = new DumpContainer();
	
	var slider = new Slider(opt =>
	{
		opt.Width = 400;
		//opt.DumpOpt = true;
		opt.Type = SliderType.Double;
		opt.Skin = Skin.Sharp;
		opt.Step = 5;
		opt.From = 55;
		opt.To = 90;
		
		opt.Min = 30;
		opt.Max = 120;
		opt.FromMin = 30;
		opt.FromMax = 120;
		opt.ToMin = 30;
		opt.ToMax = 120;
		
		opt.Prefix = "£";
		opt.DecorateBoth = false;
		opt.MaxPostfix = "+";
	});
	
	slider.WhenChanged.Subscribe(e => dc.UpdateContent($"{e}")).D(D);
	
	Util.VerticalRun(
		slider,
		dc
	).Dump();
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
