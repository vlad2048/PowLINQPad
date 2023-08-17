using PowBasics.Geom;
using PowLINQPad.Flex_.Structs;

namespace PowLINQPad.Flex_.StructsInternal;

sealed record FlexNfo(
	Dir Dir,
	IDim DimX,
	IDim DimY,
	bool Scroll,
	OverlayPos? Overlay
);