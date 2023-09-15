using System.Diagnostics;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using LINQPad;

namespace PowLINQPad.Utils;

public static class HtmlExporter
{
	public static void OpenInBrowser()
	{
		var file = $"{Path.GetTempFileName()}.html";
		var html = GetHtml();
		File.WriteAllText(file, html);
		Process.Start(new ProcessStartInfo
		{
			FileName = file,
			UseShellExecute = true,
		});
	}

	public static string GetHtml() =>
		((string)Util.InvokeScript(true, "eval", "document.documentElement.innerHTML"))
			.BeautifyHtml();

	public static string BeautifyHtml(this string html)
	{
		var parser = new HtmlParser();
		var doc = parser.ParseDocument(html);
		using var writer = new StringWriter();
		doc.ToHtml(writer, new PrettyMarkupFormatter
		{
			Indentation = "\t",
			NewLine = "\n"
		});
		var formattedHtml = writer.ToString();
		return formattedHtml;
	}
}