using PowBasics.CollectionsExt;

namespace PowLINQPad.Editing.Layout_;

sealed record CtrlGroup(
	EditCtx[] Ctrls
);

sealed record Tab(int Index, string Name);

sealed record TabGroup(
	Tab Tab,
	CtrlGroup[] CtrlGroups
);

sealed record Layout(
	TabGroup[] Tabs
);


static class LayoutLogic
{
	public static Layout Compute(EditCtx[] ctrls)
	{
		var tabs = GetTabs(ctrls);
		var nfos = Resolve(ctrls, tabs);
		
		var layout = new Layout(
			nfos
				.GroupBy(e => e.Tab)
				.SelectToArray(g => MakeGroup(g.Key, g.ToArray()))
		);
		return layout;
	}


	private sealed record CtrlNfo(Tab Tab, int Index, EditCtx Ctx);
	
	
	// GetTabs
	// =======
	private static Tab[] GetTabs(EditCtx[] ctrls)
	{
		//var tabs = ctrls.Select((e, i) => (e.Pos.TabName, i)).Where(t => t.TabName != null).Distinct().SelectToArray(t => new Tab(t.i, t.TabName!));
		var tabNames = ctrls.Select(e => e.Pos.TabName).Where(e => e != null).Distinct().SelectToArray(e => e!);
		if (tabNames.Length == 0) tabNames = new[] { "Default" };
		var tabs = tabNames.SelectToArray((e, i) => new Tab(i, e));
		return tabs;
	}
	
	
	// Resolve
	// =======
	private static CtrlNfo[] Resolve(EditCtx[] ctrls, Tab[] tabs)
	{
		Tab GetTab(EditCtx ctrl) => ctrl.Pos.TabName switch
		{
			null => tabs[0],
			not null => tabs.Single(e => e.Name == ctrl.Pos.TabName)
		};
		var slots = new int[tabs.Length];
		for (var i = 0; i < tabs.Length; i++)
		{
			var tabCtrls = ctrls
				.Where(ctrl => GetTab(ctrl) == tabs[i])
				.Where(ctrl => ctrl.Pos.Index.HasValue)
				.ToArray();
			var maxSlot = -1;
			if (tabCtrls.Length > 0)
				maxSlot = tabCtrls.Max(e => e.Pos.Index!.Value);
			slots[i] = maxSlot + 1;
		}

		return (
			from ctrl in ctrls
			let tab = GetTab(ctrl)
			let index = ctrl.Pos.Index ?? slots[tab.Index]++
			select new CtrlNfo(tab, index, ctrl)
		).ToArray();
	}
	
	
	// MakeGroup
	// =========
	private static TabGroup MakeGroup(Tab tab, CtrlNfo[] ctrls) => new(
		tab,
		ctrls
			.GroupBy(e => e.Index)
			.OrderBy(g => g.Key)
			.SelectToArray(g => new CtrlGroup(g.SelectToArray(e => e.Ctx)))
	);
}