using LINQPad.Controls;
using PowLINQPad.UtilsUI;

namespace PowLINQPad.Controls_.Utils;


public static class CtrlBlocks
{
    public static Div horz(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	row;
		column-gap:		{CtrlConsts.Gap}px;
	""");

    public static Div vert(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	column;
		row-gap:		{CtrlConsts.Gap}px;
	""");


    internal static C Squeeze<C>(this C ctrl) where C : Control => ctrl.Css($"width: {CtrlConsts.SqueezeWidth}px;");
    internal static RangeControl CssRangeInt(this RangeControl ctrl) => ctrl.Css($"width: calc(100% - {CtrlConsts.RangeIntPix}px);");
    internal static RangeControl CssRangeTime(this RangeControl ctrl) => ctrl.Css($"width: calc(100% - {CtrlConsts.RangeTimePix}px);");

    internal static Div mk(CtrlSize sz, Control key, Control val) => new Div(key, val).Css($"""
		display:	flex;
		overflow:	hidden;
		height:		{CtrlConsts.CtrlSizes[sz]}px;
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
		width:				{width ?? CtrlConsts.DefaultValWidth}px !important;
	""");
    // {(DbgColors ? $"background-color:	{dbgColVal};" : "")}

    internal static Div horzMid(params Control[] kids) => new Div(kids).Css($"""
		display:		flex;
		flex-direction:	row;
		align-items:	center;
		column-gap:		{CtrlConsts.Gap}px;
	""");


    internal static string MakeGroupId() => $"group-{grpId++}";

    private static int grpId;
}