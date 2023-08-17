using System.Text.Json;
using System.Text.Json.Nodes;

namespace PowLINQPad.UtilsInternal.Json_;

class Jsoner
{
    private readonly JsonSerializerOptions jsonOpt;

    public Jsoner(JsonSerializerOptions jsonOpt)
    {
        this.jsonOpt = jsonOpt;
    }

    public string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);

    public T Deser<T>(string str)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<T>(str, jsonOpt);
            return obj switch
            {
                not null => obj,
                null => throw new ArgumentException($"Error deserializing response to {typeof(T).Name}: returned null")
            };
        }
        catch (Exception ex)
        {
	        throw new ArgumentException($"Error deserializing response to {typeof(T).Name} [{ex.GetType().Name}]: {ex.Message}");
        }
    }

    public T Deser<T>(JsonNode node)
    {
        try
        {
            var obj = node.Deserialize<T>(jsonOpt);
            return obj switch
            {
                not null => obj,
                null => throw new ArgumentException($"Error deserializing response to {typeof(T).Name}: returned null")
            };
        }
        catch (Exception ex)
        {
	        throw new ArgumentException($"Error deserializing response to {typeof(T).Name} [{ex.GetType().Name}]: {ex.Message}");
        }
    }

    public void Save<T>(string file, T obj) => File.WriteAllText(file, Ser(obj));

    public T Load<T>(string file) => File.Exists(file) switch
    {
        true => Deser<T>(File.ReadAllText(file)),
        false => throw new ArgumentException($"File not found: '{file}'")
    };

    public T LoadOrCreateDefault<T>(string file, T defaultValue)
    {
        T CreateDefault()
        {
            Save(file, defaultValue);
            return defaultValue;
        }

        if (!File.Exists(file)) return CreateDefault();

        return Load<T>(file);
    }
}


static class JsonerExt
{
    public static void SaveIf<T>(this Jsoner jsoner, bool condition, string file, T obj)
    {
        if (!condition) return;
        jsoner.Save(file, obj);
    }
}