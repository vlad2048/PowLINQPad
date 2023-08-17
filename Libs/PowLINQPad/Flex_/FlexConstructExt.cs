using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowBasics.ColorCode;
using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;
using PowLINQPad.Flex_.StructsInternal;
using PowLINQPad.Flex_.Utils;
using PowLINQPad.UtilsInternal.Json_;

namespace PowLINQPad.Flex_;

public static class FlexConstructExt
{
	public static C FlexH<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			false,
			null
		));

	public static C FlexV<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			false,
			null
		));

	public static C Scroll<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			true,
			null
		));

	public static C OverlayH<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			false,
			anchor
		));

	public static C OverlayV<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			false,
			anchor
		));

	public static C OverlayScrollH<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			true,
			anchor
		));

	public static C OverlayScrollV<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			true,
			anchor
		));
}