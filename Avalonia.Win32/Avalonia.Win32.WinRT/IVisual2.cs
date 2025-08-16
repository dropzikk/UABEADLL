using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IVisual2 : IInspectable, IUnknown, IDisposable
{
	IVisual ParentForTransform { get; }

	Vector3 RelativeOffsetAdjustment { get; }

	Vector2 RelativeSizeAdjustment { get; }

	void SetParentForTransform(IVisual value);

	void SetRelativeOffsetAdjustment(Vector3 value);

	void SetRelativeSizeAdjustment(Vector2 value);
}
