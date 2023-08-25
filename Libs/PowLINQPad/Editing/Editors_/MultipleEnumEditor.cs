using LINQPad.Controls;
using PowBasics.CollectionsExt;
using PowLINQPad.Editing._Base;
using PowLINQPad.Editing.Controls_;

namespace PowLINQPad.Editing.Editors_;


public class SingleEnumEditAttribute : EditAttribute
{
	public SingleEnumEditAttribute(string? text = null) : base(text) { }
}
public class MultipleEnumEditAttribute : EditAttribute
{
	public MultipleEnumEditAttribute(string? text = null) : base(text) { }
}



public static class SingleEnumEditor
{
	public static Control Make<E>(PropAccess pObj, IRoDispBase d, string text) where E : struct, Enum
	{
		var p = pObj.ToTyped<E>();

		var ctrl = new CtrlDropdownButton(text, Enum.GetNames<E>(), false).D(d);

		ctrl.Value.V = new[] { p.Get() }.ToIdx();
		ctrl.Value.WhenInner.Subscribe(v => p.Set(v.FromIdx<E>()[0])).D(d);

		return ctrl;
	}
}

public static class MultipleEnumEditor
{
	public static Control Make<E>(PropAccess pObj, IRoDispBase d, string text) where E : struct, Enum
	{
		var p = pObj.ToTyped<E[]>();

		var ctrl = new CtrlDropdownButton(text, Enum.GetNames<E>(), true).D(d);

		ctrl.Value.V = p.Get().ToIdx();
		ctrl.Value.WhenInner.Subscribe(v => p.Set(v.FromIdx<E>())).D(d);

		return ctrl;
	}
}


file static class EnumEditorExt
{
	public static int[] ToIdx<E>(this E[] v) where E : struct, Enum
	{
		var vals = Enum.GetValues<E>();
		return v.SelectToArray(e => vals.IndexOf(e));
	}

	public static E[] FromIdx<E>(this int[] v) where E : struct, Enum
	{
		var vals = Enum.GetValues<E>();
		return v.SelectToArray(e => vals[e]);
	}
}