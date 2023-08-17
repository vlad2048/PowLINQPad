using System.Text.Json.Serialization;

namespace PowLINQPad.Flex_.Structs;

[JsonDerivedType(typeof(FixDim), typeDiscriminator: "Fix")]
[JsonDerivedType(typeof(AutoDim), typeDiscriminator: "Auto")]
[JsonDerivedType(typeof(FillDim), typeDiscriminator: "Fill")]
public interface IDim { }

public sealed record FixDim(int Val) : IDim;
public sealed record AutoDim : IDim;
public sealed record FillDim : IDim;

public static class Dim
{
	// @formatter:off
	public static			IDim Fix(int val)	=> new FixDim(val);
	public static readonly	IDim Auto			= new AutoDim();
	public static readonly	IDim Fill			= new FillDim();
	// @formatter:on
}