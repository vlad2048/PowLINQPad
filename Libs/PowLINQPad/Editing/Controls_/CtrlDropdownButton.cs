using LINQPad.Controls;
using LINQPad;
using PowBasics.CollectionsExt;
using System.Reactive.Linq;
using PowLINQPad.UtilsUI;
using PowLINQPad.Editing._Base;

namespace PowLINQPad.Editing.Controls_;

public class CtrlDropdownButton : Control, IBoundCtrl<int[]>
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();
    private readonly bool multiple;
    private readonly IRwVar<bool> isOn;
    private readonly IFullRwBndVar<int[]> rxVar;

    public IRwBndVar<int[]> RxVar => rxVar.ToRwBndVar();

    public CtrlDropdownButton(string text, string[] choices, bool multiple) : base("div")
    {
	    this.multiple = multiple;
        var ctrlBtn = new Button(text);
        var ctrlClear = new Control("img").SetCls(Css.ClsClear);
		ctrlClear.HtmlElement.SetAttribute("src", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABIAAAASCAYAAABWzo5XAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFWSURBVDhPrVQxboRADHSOjo4SPpCOKjzgGj6ARE+ZZ9BTIvEHJGokilyTVFxFlzwApKQgFR0inr1dshByOukYycLe9Qz2euGh73vaQl3XLj+eL9GMzPO8RvoLCCHLsmRIVFWVEBjH8THP82PTXHiu61IYhifDMN45zHzfnwVFMXpFECmK4tW27YnDTXMcZ0KOfKHAQggbSZJ8sLspsDbkKrFZSFWiJ95iqjJdKLvWzn+GNsGFxgHTwcF2Xcd7v+DSv0zTlBERfKzJUKBtWwJXTBiKURT9eRsTp7Isazx1f50HLjQOHJAasY5hGCgIgic+hxMMPtbWUFwhtAeEEC7bGjgTruTMlRxh8LG2xszF+Lj/F3YXvcdx/K2fCXw+7E89BwYuNO4aPzjgivHzApClafom/ZshOZkIxK1koLy7PxEFbODaX2sTe8hRIgA0dvuN7PRjI/oBi5leFNQt5yAAAAAASUVORK5CYII=");
        Span[] ctrlSpans = choices.SelectToArray(e => new Span(e));
        var ctrlSpanWrapper = new Div(ctrlSpans.OfType<Control>());

        VisualTree.Add(ctrlBtn);
		if (multiple)
			VisualTree.Add(ctrlClear);
        VisualTree.Add(ctrlSpanWrapper);

        CssClass = Css.ClsRoot;
        Util.HtmlHead.AddStyles(Css.Styles);

        rxVar = Var.MakeBnd(multiple ? Array.Empty<int>() : new[] { 0 }).D(d);
        isOn = Var.Make(false).D(d);

        ctrlBtn.WhenClicked().Subscribe(_ => isOn.V = !isOn.V).D(d);
        ctrlClear.WhenClicked().Subscribe(_ => rxVar.SetInner(Array.Empty<int>())).D(d);

        choices
            .Select((_, idx) => ctrlSpans[idx].WhenClicked().Select(_ => idx))
            .Merge()
            .Subscribe(e => rxVar.Toggle(e, multiple)).D(d);

        isOn.Subscribe(v =>
        {
	        ctrlBtn.SetClsFlag(Css.ClsBtn, v).AddClsIf(Css.ClsBtnMargin, !multiple);
	        ctrlSpanWrapper.SetClsFlag(Css.ClsSpanWrapper, v);
        }).D(d);

        rxVar.Subscribe(v =>
        {
            for (var i = 0; i < choices.Length; i++)
                ctrlSpans[i].SetClsFlag(Css.ClsSpan, v.Contains(i));

            ctrlClear.Styles["visibility"] = v.Any() ? "visible" : "hidden";
        }).D(d);

        if (!multiple)
        {
	        RxVar.WhenInner.Subscribe(_ => isOn.V = false).D(d);
        }
    }


    protected override void OnRendering(EventArgs e)
    {
	    base.OnRendering(e);

		if (multiple)
		{
			this.WhenMounted()
				.Subscribe(_ =>
				{
					this.WhenClickedOutside(d)
						.Where(_ => isOn.V)
						.Subscribe(_ => isOn.V = false).D(d);
				}).D(d);
		}
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

    
}


file static class Css
{
	// @formatter:off
	public const string		ClsRoot					= "dropdown-root";
	public const string		ClsBtn					= "dropdown-btn";
	public const string		ClsBtnMargin			= "dropdown-btn-margin";
	public const string		ClsClear				= "dropdown-clear";
	public const string		ClsSpanWrapper			= "dropdown-span-wrapper";
	public const string		ClsSpan					= "dropdown-span";
	// @formatter:on

	public static readonly string Styles = $$"""
		.{{ClsRoot}} {
			position:			relative;
			white-space:		nowrap;
			display:			flex;
			align-items:		center;
		}

		.{{ClsBtn}} {
			width: 100%;
		}
		.{{ClsBtnMargin}} {
			margin-right: 23px;
		}
		
		.{{ClsBtn}}::after {
			content:			"";
			display:			inline-block;
			margin-left:		7px;
			vertical-align:		2px;
			border-top:			4px solid;
			border-left:		4px solid transparent;
			border-right:		4px solid transparent;
			border-bottom:		0;
		}
		.{{ClsBtn.Flag(true)}} {
			background-color:	#003d99 !important;
		}

		.{{ClsClear}} {
			padding-left:		5px;
			visibility:			hidden;
			cursor:				pointer;
		}
		
		.{{ClsSpanWrapper}} {
			position:			absolute;
			left:				0;
			top:				30px;
			z-index:			1;
			
			background-color:	#1e2227;
			flex-direction:		column;
			row-gap:			1px;
			border:				1px solid #15181b;
			border-radius:		6px;
			padding:			4px;
		}
		.{{ClsSpanWrapper.Flag(false)}} {
			display: none;
		}
		.{{ClsSpanWrapper.Flag(true)}} {
			display: flex;
		}
		
		.{{ClsSpan}} {
			padding:			5px 48px 3px 16px;
			cursor:				pointer;
		}
		.{{ClsSpan.Flag(false)}}:hover {
			background-color:	#282d34;
			border-radius:		6px;
		}
		.{{ClsSpan.Flag(true)}} {
			background-color:	#0066ff;
			border-radius:		6px;
		}
	""";
}
