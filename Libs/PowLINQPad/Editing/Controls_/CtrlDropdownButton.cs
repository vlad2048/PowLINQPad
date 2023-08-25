using LINQPad.Controls;
using LINQPad;
using PowBasics.CollectionsExt;
using System.Reactive;
using System.Reactive.Linq;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Editing.Controls_;

public class CtrlDropdownButton : Control, IDisp
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();
    private readonly IFullRwBndVar<int[]> rxVal;

    public IRwBndVar<int[]> Value => rxVal.ToRwBndVar();

    public CtrlDropdownButton(string text, string[] choices, bool multiple) : base("div")
    {
        var ctrlBtn = new Button(text);
        Span[] ctrlSpans = choices.SelectToArray(e => new Span(e).AddCls("dropdown-span dropdown-span-off"));
        var ctrlSpanWrapper = new Div(ctrlSpans.OfType<Control>()).AddCls("dropdown-span-wrapper");
        VisualTree.AddRange(new Control[]
        {
            ctrlBtn,
            ctrlSpanWrapper
        });
        CssClass = "dropdown-root";
        Util.HtmlHead.AddStyles(DropdownUtils.Styles);

        rxVal = Var.MakeBnd(multiple ? Array.Empty<int>() : new[] { 0 }).D(d);
        var isOn = Var.Make(false).D(d);

        ctrlBtn.WhenClicked().Subscribe(_ => isOn.V = !isOn.V).D(d);

        choices
            .Select((_, idx) => ctrlSpans[idx].WhenClicked().Select(_ => idx))
            .Merge()
            .Subscribe(e => rxVal.Toggle(e, multiple)).D(d);

        isOn.Subscribe(v =>
        {
            if (v)
            {
                ctrlBtn.AddCls("dropdown-on");
                ctrlSpanWrapper.Styles["display"] = "flex";
            }
            else
            {
                ctrlBtn.DelCls("dropdown-on");
                ctrlSpanWrapper.Styles["display"] = "none";
            }
        }).D(d);

        rxVal.Subscribe(v =>
        {
            for (var i = 0; i < choices.Length; i++)
            {
                if (v.Contains(i))
                {
                    ctrlSpans[i].AddCls("dropdown-span-on");
                    ctrlSpans[i].DelCls("dropdown-span-off");
                }
                else
                {
                    ctrlSpans[i].DelCls("dropdown-span-on");
                    ctrlSpans[i].AddCls("dropdown-span-off");
                }
            }
        }).D(d);
    }
}







file static class DropdownUtils
{
    public static void Toggle(this IFullRwBndVar<int[]> rxVar, int val, bool multiple)
    {
        var valPrev = rxVar.V;
        var valNext = multiple switch
        {
            true => valPrev.Contains(val) switch
            {
                false => valPrev.Append(val).ToArray(),
                true => valPrev.WhereToArray(e => !e.Equals(val))
            },
            false => valPrev.Contains(val) switch
            {
                false => new[] { val },
                true => valPrev
            }
        };
        rxVar.SetInner(valNext);
    }

    public const string Styles = """
		.dropdown-root {
			position:			relative;
		}
		
		.dropdown-on {
			background-color:	#003d99 !important;
		}
		
		.dropdown-span-wrapper {
			position:			absolute;
			left:				0;
			top:				30px;
			
			background-color:	#1e2227;
			display:			flex;
			flex-direction:		column;
			row-gap:			1px;
			border:				1px solid #15181b;
			border-radius:		6px;
			padding:			4px;
		}
	
		.dropdown-span {
			padding:			5px 48px 3px 16px;
			cursor:				pointer;
		}
		.dropdown-span-off {
		}
		.dropdown-span-off:hover {
			background-color:	#282d34;
			border-radius:		6px;
		}
		.dropdown-span-on {
			background-color:	#0066ff;
			border-radius:		6px;
		}
	""";
}



static class SkinningRxExt
{
    public static IObservable<Unit> WhenClicked(this Control ctrl) => Obs.FromEventPattern(e => ctrl.Click += e, e => ctrl.Click -= e).ToUnit();
}
