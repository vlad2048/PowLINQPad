﻿using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;
using PowLINQPad.Flex_.StructsInternal;

namespace PowLINQPad.Flex_.Logic;

static class FlexSolver
{
	public static ICssAttr[] SolveNode(Dir? parentDir, TNod<FlexNfo> node)
	{
		var hasAnyOverlayChildren = node.Children.Any(e => e.V.Overlay != null);
		var n = node.V;
		return new []
			{
				SolveDim(Dir.Horz, parentDir == Dir.Horz, n.DimX),
				SolveDim(Dir.Vert, parentDir == Dir.Vert, n.DimY),
				SolveDir(n.Dir),
				SolveScroll(n.Scroll),
				SolveOverlay(n.Overlay, hasAnyOverlayChildren)
			}
			.SelectMany(e => e)
			.ToArray();
	}
	
	
	
	private static ICssAttr[] SolveDir(Dir dir) => A(
		CssAttr.Display(dir)
	);

	
	// @formatter:off
	private static ICssAttr[] SolveDim(Dir dir, bool isDir, IDim dim) => isDir switch
	{
		true => dim switch
		{
			FixDim { Val: var val } =>	A(CssAttr.Flex(CssFlex.Fix(val))),
			AutoDim =>					A(CssAttr.Flex(CssFlex.Auto)),
			FillDim =>					A(CssAttr.Flex(CssFlex.Fill), CssAttr.Dim(dir, Dim.Fill)),
			_ => throw new ArgumentException()
		},
		false => A(CssAttr.Dim(dir, dim)),
	};
	// @formatter:on


	private static ICssAttr[] SolveScroll(bool scroll) => scroll switch
	{
		false => A(CssAttr.Overflow(CssOverflow.Clip)),
		true => A(CssAttr.Overflow(CssOverflow.Scroll)),
	};

	
	// @formatter:off
	private static ICssAttr[] SolveOverlay(OverlayPos? isOverlay, bool hasAnyOverlayChildren) => (isOverlay, hasAnyOverlayChildren) switch
	{
		(null,		false	) => A(),
		(not null,	_		) => A(CssAttr.Position(CssPosition.Overlay), CssAttr.OverlayPos(isOverlay.Value)),
		(null,		true	) => A(CssAttr.Position(CssPosition.OverlayParent))
	};
	// @formatter:on


	private static ICssAttr[] A(params ICssAttr[] arr) => arr;
}