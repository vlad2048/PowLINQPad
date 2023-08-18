using System.Text.Json.Serialization;

namespace PowLINQPad.Flex_.Structs;

[JsonDerivedType(typeof(FixDim), typeDiscriminator: "Fix")]
[JsonDerivedType(typeof(FitDim), typeDiscriminator: "Fit")]
[JsonDerivedType(typeof(FilDim), typeDiscriminator: "Fil")]
public interface IDim { }

public sealed record FixDim(int Val) : IDim;
public sealed record FitDim : IDim;
public sealed record FilDim : IDim;

public static class Dim
{
	// @formatter:off
	public static			IDim Fix(int val)	=> new FixDim(val);
	public static readonly	IDim Fit			= new FitDim();
	public static readonly	IDim Fil			= new FilDim();
	// @formatter:on
}


public sealed record Dims(IDim X, IDim Y)
{
	// @formatter:off
	public static Dims			Fix(int x, int y)	=> new(Dim.Fix(x), Dim.Fix(y));
	public static Dims			FixFit(int x)		=> new(Dim.Fix(x), Dim.Fit);
	public static Dims			FixFil(int x)		=> new(Dim.Fix(x), Dim.Fil);

	public static Dims			FitFix(int y)		=> new(Dim.Fit, Dim.Fix(y));
	public static readonly Dims	Fit					= new(Dim.Fit, Dim.Fit);
	public static readonly Dims	FitFil				= new(Dim.Fit, Dim.Fil);

	public static Dims			FilFix(int y)		=> new(Dim.Fil, Dim.Fix(y));
	public static readonly Dims	FilFit				= new(Dim.Fil, Dim.Fit);
	public static readonly Dims	Fil					= new(Dim.Fil, Dim.Fil);
	// @formatter:on
}
