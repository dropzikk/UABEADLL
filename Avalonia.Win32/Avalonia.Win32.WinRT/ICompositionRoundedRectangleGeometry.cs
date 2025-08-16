using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionRoundedRectangleGeometry : IInspectable, IUnknown, IDisposable
{
	Vector2 CornerRadius { get; }

	Vector2 Offset { get; }

	Vector2 Size { get; }

	void SetCornerRadius(Vector2 value);

	void SetOffset(Vector2 value);

	void SetSize(Vector2 value);
}
