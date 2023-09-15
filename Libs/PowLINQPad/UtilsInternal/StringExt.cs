namespace PowLINQPad.UtilsInternal;

static class StringExt
{
    public static string[] Chop(this string s, char sep) => s.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
