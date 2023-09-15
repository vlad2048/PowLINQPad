using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;

namespace PowLINQPad.Flex_.StructsInternal;

sealed record FlexNfo(
	Dir Dir,
	Dims Dims,
	bool Scroll,
	OverlayPos? Overlay
);