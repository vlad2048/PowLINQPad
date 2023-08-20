using System.Diagnostics;
using AngleSharp.Html.Parser;
using AngleSharp.Html;
using LINQPad;

namespace PowLINQPad.UtilsUI;

public static class HtmlExporter
{
	public static void OpenInBrowser()
	{
		var file = $"{Path.GetTempFileName()}.html";
		SavePage(file);
		Process.Start(new ProcessStartInfo
		{
			FileName = file,
			UseShellExecute = true,
		});
	}

	private static void SavePage(string htmlFile)
	{
		var html = (string)Util.InvokeScript(true, "eval", "document.documentElement.innerHTML");
		html = Beautify(html);
		File.WriteAllText(htmlFile, html);
	}

	private static string Beautify(string html)
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