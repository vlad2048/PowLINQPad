namespace PowLINQPad.Editing._Base;

public class EditAttribute : Attribute
{
	public string? Text { get; }

	public EditAttribute(string? text = null)
	{
		Text = text;
	}
}