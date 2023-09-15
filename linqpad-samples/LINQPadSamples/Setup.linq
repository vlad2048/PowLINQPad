<Query Kind="Program">
  <NuGetReference>PowLINQPad</NuGetReference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
</Query>

// optional aliases
global using Obs = System.Reactive.Linq.Observable;
global using IDisp = System.IDisposable;
using static PowLINQPad.Flex_.Structs.Dims;


void Main()
{
	
}


public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
