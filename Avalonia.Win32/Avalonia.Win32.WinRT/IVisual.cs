using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IVisual : IInspectable, IUnknown, IDisposable
{
	Vector2 AnchorPoint { get; }

	CompositionBackfaceVisibility BackfaceVisibility { get; }

	CompositionBorderMode BorderMode { get; }

	Vector3 CenterPoint { get; }

	ICompositionClip Clip { get; }

	CompositionCompositeMode CompositeMode { get; }

	int IsVisible { get; }

	Vector3 Offset { get; }

	float Opacity { get; }

	Quaternion Orientation { get; }

	IContainerVisual Parent { get; }

	float RotationAngle { get; }

	float RotationAngleInDegrees { get; }

	Vector3 RotationAxis { get; }

	Vector3 Scale { get; }

	Vector2 Size { get; }

	Matrix4x4 TransformMatrix { get; }

	void SetAnchorPoint(Vector2 value);

	void SetBackfaceVisibility(CompositionBackfaceVisibility value);

	void SetBorderMode(CompositionBorderMode value);

	void SetCenterPoint(Vector3 value);

	void SetClip(ICompositionClip value);

	void SetCompositeMode(CompositionCompositeMode value);

	void SetIsVisible(int value);

	void SetOffset(Vector3 value);

	void SetOpacity(float value);

	void SetOrientation(Quaternion value);

	void SetRotationAngle(float value);

	void SetRotationAngleInDegrees(float value);

	void SetRotationAxis(Vector3 value);

	void SetScale(Vector3 value);

	void SetSize(Vector2 value);

	void SetTransformMatrix(Matrix4x4 value);
}
