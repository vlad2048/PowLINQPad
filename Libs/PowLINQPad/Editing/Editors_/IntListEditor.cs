using LINQPad.Controls;
using PowLINQPad.Editing._Base;
using PowLINQPad.Editing.Controls_;

namespace PowLINQPad.Editing.Editors_;

public class IntListEditAttribute : EditAttribute
{
	public IntListEditAttribute(string? text = null) : base(text) { }
}

public static class IntListEditor
{
	public static Control Make(PropAccess accessObj, IRoDispBase d, string text)
	{
		var access = accessObj.ToTyped<int[]>();
		var ctrl = new CtrlTextBox(text).D(d);

	}
}