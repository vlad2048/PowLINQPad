<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <NuGetReference>PowRxVar</NuGetReference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.Editing</Namespace>
  <Namespace>PowLINQPad.Editing.Editors_</Namespace>
  <Namespace>PowLINQPad.Editing._Base</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var filter = Var.MakeBnd(Rec.Empty).D(D);
	var ui = filter.MakeEditor(D);
	
	Util.HorizontalRun(true,
		ui,
		filter.ToDC(D)
	).Dump();

	/*var prop = typeof(Rec).GetProperty("State");
	var eltType = prop.PropertyType.GetElementType();
	
	var filter = Var.MakeBnd(new Rec(new[] { State.One})).D(D);

	var access = new PropAccess(prop, () => filter.V.State, v => filter.V = filter.V with { State = (State[])v });

	var methGen = typeof(MultipleEnumEditor).GetMethod("Make");
	var meth = methGen.MakeGenericMethod(eltType);
	
	var ctrl = meth.Invoke(null, new object[] { access, D, "Label" });
	ctrl.Dump();*/
}



enum State
{
	One,
	Two,
	Three,
}

record Rec(
	[property:SingleEnumEdit]
	State Single,
	[property:MultipleEnumEdit]
	State[] Multiple
)
{
	public static readonly Rec Empty = new(
		State.Three,
		new[] { State.One }
	);
}




public static Disp D => RxUI.D;
void OnStart()
{
	RxUI.Start();
	
	//Util.HtmlHead.AddStyles("@import url('https://fonts.googleapis.com/css2?family=Inter&display=swap');");
}





