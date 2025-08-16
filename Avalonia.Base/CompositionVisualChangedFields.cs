using System;

[Flags]
internal enum CompositionVisualChangedFields : uint
{
	Root = 1u,
	Parent = 2u,
	Visible = 4u,
	VisibleAnimated = 8u,
	Opacity = 0x10u,
	OpacityAnimated = 0x20u,
	Clip = 0x40u,
	ClipToBounds = 0x80u,
	ClipToBoundsAnimated = 0x100u,
	Offset = 0x200u,
	OffsetAnimated = 0x400u,
	Size = 0x800u,
	SizeAnimated = 0x1000u,
	AnchorPoint = 0x2000u,
	AnchorPointAnimated = 0x4000u,
	CenterPoint = 0x8000u,
	CenterPointAnimated = 0x10000u,
	RotationAngle = 0x20000u,
	RotationAngleAnimated = 0x40000u,
	Orientation = 0x80000u,
	OrientationAnimated = 0x100000u,
	Scale = 0x200000u,
	ScaleAnimated = 0x400000u,
	TransformMatrix = 0x800000u,
	TransformMatrixAnimated = 0x1000000u,
	AdornedVisual = 0x2000000u,
	AdornerIsClipped = 0x4000000u,
	OpacityMaskBrush = 0x8000000u,
	Effect = 0x10000000u,
	RenderOptions = 0x20000000u
}
