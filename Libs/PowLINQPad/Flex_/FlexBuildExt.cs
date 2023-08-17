using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowBasics.StringsExt;
using PowLINQPad.Flex_.Logic;
using PowLINQPad.Flex_.StructsInternal;
using PowLINQPad.Flex_.Utils;
using PowLINQPad.UtilsInternal.Json_;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Flex_;

public static class FlexBuildExt
{
	public static C Build<C>(this C ctrl, bool dbgColors = false) where C : Control
	{
		var root = BuildTree(ctrl);
		SolveTree(ctrl, root, dbgColors);
		return ctrl;
	}

	
	[Flags]
	private enum KidPos
	{
		First = 1,
		Middle = 2,
		Last = 4
	}

	
	private static void SolveTree(Control rootCtrl, TNod<FlexNfo> rootNode, bool dbgColors)
	{
		void Rec(Control ctrl, TNod<FlexNfo> node, int level, KidPos pos)
		{
			var attrs = FlexSolver.SolveNode(null, node);
			var css = attrs.GetCss()
				.AddLinqPadExtraPaddingIFN(level, pos)
				.AddDbgColorsIFN(dbgColors);
			
			ctrl.Css(css);
			ctrl.ClearFlex();
			
			var ctrlKids = ctrl.GetKids();
			var nodeKids = node.Children;
			for (var i = 0; i < ctrlKids.Length; i++)
			{
				var ctrlKid = ctrlKids[i];
				var nodeKid = nodeKids[i];
				var kidPos = GetKidPos(i, ctrlKids.Length);
				Rec(ctrlKid, nodeKid, level + 1, kidPos);
			}
		}
		
		Rec(rootCtrl, rootNode, 0, KidPos.First);
	}

	
	private static string AddLinqPadExtraPaddingIFN(this string css, int level, KidPos pos)
	{
		if (level != 1) return css;
		var padLeft = 2;
		var padRight = 2;
		var padTop = pos.HasFlag(KidPos.First) ? 2 : 0;
		var padBottom = pos.HasFlag(KidPos.Last) ? 1 : 0;
		return $"""
			{css}
			padding: {padTop}px {padRight}px {padBottom}px {padLeft}px;
		""";
	}

	private static string AddDbgColorsIFN(this string css, bool dbgColors)
	{
		if (!dbgColors) return css;
		var color = PaletteUtils.GetNextColor();
		return $"""
			{css}
			background-color: {color};
		""";
	}
	
	private static KidPos GetKidPos(int idx, int cnt) => cnt switch
	{
		1 => KidPos.First | KidPos.Last,
		> 1 => idx switch
		{
			0 => KidPos.First,
			_ when idx == cnt - 1 => KidPos.Last,
			_ => KidPos.Middle
		},
		_ => throw new ArgumentException()
	};
	
	private static TNod<FlexNfo> BuildTree(Control rootCtrl)
	{
		TNod<FlexNfo> Make(Control ctrl)
		{
			if (!ctrl.HasFlex()) throw new ArgumentException();
			var node = Nod.Make(Jsoners.Common.Deser<FlexNfo>(ctrl.GetFlex()));
			var kids = ctrl.GetKids();
			foreach (var kid in kids)
			{
				var kidNode = Make(kid);
				node.AddChild(kidNode);
			}
			return node;
		}
		return Make(rootCtrl);
	}
	
	private static string GetCss(this ICssAttr[] attrs) => (
		from attr in attrs
		from line in SplitAttrCss($"{attr}")
		select line
	)
		.JoinLines();
		

	private static string[] SplitAttrCss(this string s) => s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).SelectToArray(e => $"{e};");
}