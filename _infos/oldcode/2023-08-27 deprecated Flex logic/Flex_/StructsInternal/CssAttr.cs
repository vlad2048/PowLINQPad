using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;

namespace PowLINQPad.Flex_.StructsInternal;


interface ICssAttr { }


// @formatter:off
sealed record DisplayCssAttr(Dir Dir) : ICssAttr
{
	public override string ToString() => Dir switch
	{
		Dir.Horz => "display: flex; flex-direction: row;",
		Dir.Vert => "display: flex; flex-direction: column;",
	};
}

sealed record DimCssAttr(Dir Dir, IDim Dim) : ICssAttr
{
	public override string ToString() => (Dir, Dim) switch
	{
		(Dir.Horz, FixDim { Val: var val })	=> $"width: {val}px;",
		(Dir.Vert, FixDim { Val: var val })	=> $"height: {val}px;",
		(Dir.Horz, FitDim)					=> "width: auto;",
		(Dir.Vert, FitDim)					=> "height: auto;",
		(Dir.Horz, FilDim)					=> "width: 100%;",
		(Dir.Vert, FilDim)					=> "height: 100%;",
	};
}

sealed record FlexCssAttr(IDim Dim) : ICssAttr
{
	public override string ToString() => Dim switch
	{
		FixDim { Val: var val }	=> $"flex: 0 0 {val}px;",
		FitDim					=> "flex: 0 0 auto;",
		FilDim					=> "flex: 1 1 auto;",
		_ => throw new ArgumentException()
	};
}

sealed record OverflowCssAttr(CssOverflow Overflow) : ICssAttr
{
	public override string ToString() => Overflow switch
	{
		CssOverflow.None	=> "overflow-y: visible;",
		CssOverflow.Clip	=> "overflow-y: hidden;",
		CssOverflow.Scroll	=> "overflow-y: auto;",
	};
}

sealed record PositionCssAttr(CssPosition Position) : ICssAttr
{
	public override string ToString() => Position switch
	{
		CssPosition.Normal			=> "",
		//CssPosition.Overlay			=> "position: absolute; z-index: var(--zindex-overlays)",
		CssPosition.Overlay			=> "position: absolute; z-index: 2;",
		CssPosition.OverlayParent	=> "position: relative;",
	};
}

sealed record OverlayPosCssAttr(OverlayPos OverlayPos) : ICssAttr
{
	public override string ToString() => OverlayPos switch
	{
		OverlayPos.TopLeft		=> "left: 0; top: 0;",
		OverlayPos.TopRight		=> "right: 0; top: 0;",
		OverlayPos.BottomLeft	=> "left: 0; bottom: 0;",
		OverlayPos.BottomRight	=> "right: 0; bottom: 0;",
	};
}


static class CssAttr
{
	public static ICssAttr Display(Dir dir)						=> new DisplayCssAttr(dir);
	public static ICssAttr Dim(Dir dir, IDim dim)				=> new DimCssAttr(dir, dim);
	public static ICssAttr Flex(IDim flex)						=> new FlexCssAttr(flex);
	public static ICssAttr Overflow(CssOverflow overflow)		=> new OverflowCssAttr(overflow);
	public static ICssAttr Position(CssPosition position)		=> new PositionCssAttr(position);
	public static ICssAttr OverlayPos(OverlayPos overlayPos)	=> new OverlayPosCssAttr(overlayPos);
}
// @formatter:on
