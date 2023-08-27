<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowLINQPad\Libs\PowLINQPad\bin\Debug\net7.0\PowLINQPad.dll</Reference>
  <Namespace>PowLINQPad</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>static PowLINQPad.Flex_.Structs.Dims</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
</Query>

global using IDisp = System.IDisposable;
global using Obs = System.Reactive.Linq.Observable;


void Main()
{
	var btn = new Button("Click").Dump();
	var btnId = btn.HtmlElement.ID.Dump();
	
	
	btn.WhenClickedOutside(D).Subscribe(_ => "OK!".Dump()).D(D);
	
	
	/*Util.HtmlHead.AddScript("""
		function onClickOutside(eltId) {
			const elt = document.getElementById(eltId);
			if (!elt) { external.log(`elt not found: ${eltId}`); return; }
			//external.log(`elt found: ${eltId}`);
			
			const listener = event => {
				const contains = elt.contains(event.target);
				if (!contains) {
					external.log('Click Outside Detected');
				}
				//external.log(`evt  contains:${contains}`);
			}
			
			document.addEventListener('click', listener);
			external.log('finished');
		}
	""");
	Util.InvokeScript(false, "onClickOutside", btnId);*/
}



public static Disp D => RxUI.D;
void OnStart() => RxUI.Start();
