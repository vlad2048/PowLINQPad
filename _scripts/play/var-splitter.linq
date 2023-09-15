<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowLINQPad.Editing.Utils</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var rxVar = Var.MakeBnd(new Rec("Vlad", new[] { 1, 2, 3 })).D(D);

	var rxName = (IFullRwBndVar<string>)rxVar.SplitProp(typeof(Rec).GetProperty("Name")!, D);
	var rxVals = (IFullRwBndVar<int[]>)rxVar.SplitProp(typeof(Rec).GetProperty("Vals")!, D);

	rxVar.Log();
	rxVar.WhenInner.Log();
	rxVar.WhenOuter.Log();
	rxName.Log();
	rxName.WhenInner.Log();
	rxName.WhenOuter.Log();
	rxVals.Log();
	rxVals.WhenInner.Log();
	rxVals.WhenOuter.Log();

	Thread.Sleep(1000);

	rxName.SetInner("Erik");
	//rxVals.SetInner(new[] { 4, 5, 6 });
}

record Rec(
	string Name,
	int[] Vals
);


public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
