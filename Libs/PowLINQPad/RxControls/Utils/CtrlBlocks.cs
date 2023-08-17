﻿using LINQPad.Controls;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.RxControls.Utils;

enum CtrlSize
{
	Single,
	Double
}

public static class CtrlBlocks
{
	public static Div horz(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	row;
		column-gap:		{Gap}px;
	""");
	
	public static Div vert(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	column;
		row-gap:		{Gap}px;
	""");


	internal static C Squeeze<C>(this C ctrl) where C : Control => ctrl.Css($"width: {SqueezeWidth}px;");
	internal static RangeControl CssRangeInt(this RangeControl ctrl) => ctrl.Css($"width: calc(100% - {RangeIntPix}px);");
	internal static RangeControl CssRangeTime(this RangeControl ctrl) => ctrl.Css($"width: calc(100% - {RangeTimePix}px);");

	internal static Div mk(CtrlSize sz, Control key, Control val) => new Div(key, val).Css($"""
		display:	flex;
		overflow:	hidden;
		height:		{ctrlSizes[sz]}px;
	""");

	internal static Div mkKey(int? width, string title, Control? under = null)
	{
		var span = new Label($"{title}:");
		var kids = under switch
		{
			null => new Control[] { span },
			not null => new[] { span, under },
		};
		var div = new Div(kids).Css($"""
			display:			flex;
			flex-direction:		column;
			align-items:		flex-end;
			{(width.HasValue ? $"width:	{width}px;" : "")}
		""");
		// {(DbgColors ? $"background-color:	{dbgColKey};" : "")}
		// width:				{width ?? DefaultKeyWidth}px;
		return div;
	}

	internal static Div mkKeyUnder(params Control[] kids) => new Div(kids).Css("""
		display:		flex;
	""");
	
	internal static Control mkVal(int? width, Control kid) => kid.Css($"""
		width:				{width ?? DefaultValWidth}px !important;
	""");
	// {(DbgColors ? $"background-color:	{dbgColVal};" : "")}

	internal static Div horzMid(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	row;
		align-items:	center;
		column-gap:		{Gap}px;
	""");


	internal static string MakeGroupId() => $"group-{grpId++}";

	//private const int DefaultKeyWidth = 70;
	private const int DefaultValWidth = 300;
	private const int Gap = 5;
	private const int SqueezeWidth = 3;
	private const int RangeIntPix = 30;
	private const int RangeTimePix = 100;
	private static readonly Dictionary<CtrlSize, int> ctrlSizes = new()
	{
		{ CtrlSize.Single, 30 },
		{ CtrlSize.Double, 65 },
	};
	private static int grpId;
}