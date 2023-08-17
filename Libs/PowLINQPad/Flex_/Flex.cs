using LINQPad;
using LINQPad.Controls;
using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;
using PowLINQPad.Flex_.StructsInternal;
using PowLINQPad.Flex_.Utils;

namespace PowLINQPad.Flex_;

public static class Flex
{
	internal static void Init() => Util.HtmlHead.AddStyles("""
		* {
			box-sizing: border-box;
		}
		html, body, #final {
			height: 100%;
			padding: 0;
			margin: 0;
		}
		#final>div {
			height: 100%;
		}
		html, body, #final {
			height: 100%;
			padding: 0;
			margin: 0;
		}
		#final>div {
			height: 100%;
		}
	""");



	public static Div Horz(IDim dimX, IDim dimY, params Control[] kids) =>
		new Div(kids).FlexH_(dimX, dimY);

	private static C FlexH_<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			false,
			null
		));



	public static Div Vert(IDim dimX, IDim dimY, params Control[] kids) =>
		new Div(kids).FlexV_(dimX, dimY);

	private static C FlexV_<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			false,
			null
		));



	public static Div Scroll(IDim dimX, IDim dimY, params Control[] kids) =>
		new Div(kids).Scroll_(dimX, dimY);

	private static C Scroll_<C>(this C ctrl, IDim dimX, IDim dimY) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			true,
			null
		));



	public static Div OverlayH(IDim dimX, IDim dimY, OverlayPos anchor, params Control[] kids) =>
		new Div(kids).OverlayH_(dimX, dimY, anchor);

	private static C OverlayH_<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			false,
			anchor
		));



	public static Div OverlayV(IDim dimX, IDim dimY, OverlayPos anchor, params Control[] kids) =>
		new Div(kids).OverlayV_(dimX, dimY, anchor);

	private static C OverlayV_<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			false,
			anchor
		));



	public static Div OverlayScrollH(IDim dimX, IDim dimY, OverlayPos anchor, params Control[] kids) =>
		new Div(kids).OverlayScrollH_(dimX, dimY, anchor);

	private static C OverlayScrollH_<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Horz,
			dimX, dimY,
			true,
			anchor
		));



	public static Div OverlayScrollV(IDim dimX, IDim dimY, OverlayPos anchor, params Control[] kids) =>
		new Div(kids).OverlayScrollV_(dimX, dimY, anchor);

	private static C OverlayScrollV_<C>(this C ctrl, IDim dimX, IDim dimY, OverlayPos anchor) where C : Control =>
		ctrl.SetFlex(new FlexNfo(
			Dir.Vert,
			dimX, dimY,
			true,
			anchor
		));
}