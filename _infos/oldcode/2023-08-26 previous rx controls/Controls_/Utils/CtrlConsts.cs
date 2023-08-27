namespace PowLINQPad.Controls_.Utils;

enum CtrlSize
{
    Single,
    Double
}

static class CtrlConsts
{
    public const int DefaultValWidth = 300;
    public const int Gap = 5;
    public const int SqueezeWidth = 3;
    public const int RangeIntPix = 30;
    public const int RangeTimePix = 100;
    public static readonly Dictionary<CtrlSize, int> CtrlSizes = new()
    {
        { CtrlSize.Single, 30 },
        { CtrlSize.Double, 65 },
    };
}
