using System.Reactive.Linq;
using LINQPad;
using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.Editing.Controls_;
using PowLINQPad.Editing.Layout_;
using PowLINQPad.Editing.Utils;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Editing;


public static class EditExt
{
	public static void MakeEditor<T>(
		this IFullRwBndVar<T> rxVar,
		IRoDispBase d,
		IReadOnlyDictionary<string, object>? extraParams = null,
		string[]? tabNames = null
	)
	{
		var contexts = (
			from prop in typeof(T).GetProperties()
			let customAttrs = Attribute.GetCustomAttributes(prop)
			let attr = customAttrs.FirstOrDefault(attr => attr is EditAttribute)
			where attr != null
			let pos = customAttrs.ReadPos()
			let editAttr = (EditAttribute)attr
			let rxProp = rxVar.SplitProp(prop, d)
			select new EditCtx(
				editAttr,
				pos,
				prop,
				rxProp,
				d,
				extraParams ?? new Dictionary<string, object>(),
				tabNames ?? Array.Empty<string>()
			)
		)
			.ToArray();

		Util.HtmlHead.AddStyles(Css.Styles);

		// Compute Layout
		// ==============
		var layout = LayoutLogic.Compute(contexts);

		// Display Tabs
		// ============
		var tabIndex = Var.Make(0).D(d);
		if (layout.Tabs.Length > 1)
		{
			var tabBtns = layout.Tabs.SelectToArray((tab, i) => new Span(tab.Tab.Name).SetClsFlag(Css.ClsTabBtn, i == tabIndex.V).WhenClickedAction(d, () => tabIndex.V = i));
			new Div(tabBtns.OfType<Control>()).SetCls(Css.ClsTabBtnWrapper).Dump();
			tabIndex.Subscribe(t =>
			{
				for (var i = 0; i < tabBtns.Length; i++)
					tabBtns[i].SetClsFlag(Css.ClsTabBtn, i == t);
			}).D(d);
		}

		// Display Layout
		// ==============
		foreach (var tab in layout.Tabs)
		{
			var divs = tab.CtrlGroups.SelectToArray(grp =>
				{
					if (grp.Ctrls.Length == 1)
						return grp.Ctrls[0].Build();
					else
						return new Div(
							grp.Ctrls.SelectToArray(ctrl => ctrl.Build())
						).SetCls(Css.ClsGroupCtrl);
				}
			);
			var tabDiv = new Div(divs.OfType<Control>())
				.SetCls(Css.ClsGroupTab)
				.DisplayIf(tabIndex.Select(i => i == tab.Tab.Index), d);
			tabDiv.Dump();
		}
	}
}


file static class Css
{
	// @formatter:off
	public const string ClsTabBtnWrapper	= "editor-tab-btn-wrapper";
	public const string ClsTabBtn			= "editor-tab-btn";
	public const string ClsGroupTab			= "editor-group-tab";
	public const string ClsGroupCtrl		= "editor-group-ctrl";
	// @formatter:on

	public static readonly string Styles = $$"""
		.{{ClsTabBtnWrapper}} {
			display:			flex;
			column-gap:			10px;
			margin-bottom:		5px;
		}
		.{{ClsTabBtn}} {
			padding:			3px 20px;
			cursor:				pointer;
			border-radius:		6px;
		}
		.{{ClsTabBtn.Flag(true)}} {
			background-color:	#453d73;
		}
		.{{ClsGroupTab}} {
			display:			flex;
			align-items:		flex-start;
			column-gap:			30px;
		}
		.{{ClsGroupCtrl}} {
			display:			flex;
			flex-direction:		column;
		}
	""";
}