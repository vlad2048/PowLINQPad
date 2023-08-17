using System.Text.Encodings.Web;
using System.Text.Json;

namespace PowLINQPad.UtilsInternal.Json_;

static class Jsoners
{
    private static readonly JsonSerializerOptions jsonOpt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    public static readonly Jsoner Common = new(jsonOpt);
}
