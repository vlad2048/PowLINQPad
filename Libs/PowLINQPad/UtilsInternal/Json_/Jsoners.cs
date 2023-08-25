using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using PowBasics.Geom.Serializers;
using PowLINQPad.UtilsInternal.Json_.NamingPolicies;

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

    private static readonly JsonSerializerOptions ionSliderJsonOpt = new()
    {
	    PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
	    WriteIndented = true,
	    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	    Converters =
	    {
		    new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
	    }
    };

    public static readonly Jsoner Common = new(jsonOpt);
    public static readonly Jsoner IonSlider = new(ionSliderJsonOpt);
}
