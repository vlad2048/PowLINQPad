using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;
using PowLINQPad.Flex_.StructsInternal;
using PowLINQPad.UtilsInternal.Json_;

namespace PowLINQPad.Flex_.Utils;

static class AttrAccessor
{
	private const string AttrName = "data-flex";

	
	public static bool HasFlex(this Control ctrl) => ctrl.HtmlElement.GetAttribute(AttrName) != null;

	public static string GetFlex(this Control ctrl) => ctrl.HtmlElement.GetAttribute(AttrName);
	
	public static C SetFlex<C>(
		this C ctrl,
		Dir dir,
		Dims dims,
		bool scroll,
		OverlayPos? overlay
	) where C : Control
	{
		var nfo = new FlexNfo(
			dir,
			dims,
			scroll,
			overlay
		);
		ctrl.HtmlElement.SetAttribute(AttrName, Jsoners.Common.Ser(nfo));
		return ctrl;
	}

	public static void ClearFlex(this Control ctrl) => ctrl.HtmlElement.SetAttribute(AttrName, null);


	public static Control[] GetKids(this Control ctrl) =>
		(
			ctrl switch
			{
				Div e => e.Children.ToArray(),
				Span e => e.Children.ToArray(),
				_ => Array.Empty<Control>()
			}
		)
		.WhereToArray(e => e.HasFlex());
}