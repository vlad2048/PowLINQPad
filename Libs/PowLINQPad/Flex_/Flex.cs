using LINQPad;
using LINQPad.Controls;
using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;
using PowLINQPad.Flex_.StructsInternal;
using PowLINQPad.Flex_.Utils;

namespace PowLINQPad.Flex_;

public static class Flex
{
	public static Div Horz(Dims dims, params Control[] kids) => new Div(kids).SetFlex(Dir.Horz, dims, false, null);
	public static Div Vert(Dims dims, params Control[] kids) => new Div(kids).SetFlex(Dir.Vert, dims, false, null);
	public static Div Scroll(Dims dims, params Control[] kids) => new Div(kids).SetFlex(Dir.Vert, dims, true, null);
	public static Div HorzOverlay(Dims dims, OverlayPos anchor, params Control[] kids) => new Div(kids).SetFlex(Dir.Horz, dims, false, anchor);
	public static Div VertOverlay(Dims dims, OverlayPos anchor, params Control[] kids) => new Div(kids).SetFlex(Dir.Vert, dims, false, anchor);
	public static Div HorzOverlayScroll(Dims dims, OverlayPos anchor, params Control[] kids) => new Div(kids).SetFlex(Dir.Horz, dims, true, anchor);
	public static Div VertOverlayScroll(Dims dims, OverlayPos anchor, params Control[] kids) => new Div(kids).SetFlex(Dir.Vert, dims, true, anchor);
}