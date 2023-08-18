using System.Text.Encodings.Web;
using System.Text.Json;
using PowBasics.Geom.Serializers;

namespace PowLINQPad.UtilsInternal.Json_;

static class Jsoners
{
    private static readonly JsonSerializerOptions jsonOpt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
		Converters =
		{
			new PtSerializer(),
			new RSerializer(),
			new SzSerializer(),
			new VecPtSerializer(),
			new VecRSerializer(),
		}
    };

    public static readonly Jsoner Common = new(jsonOpt);
}
