using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionShape : IInspectable, IUnknown, IDisposable
{
	Vector2 CenterPoint { get; }

	void SetCenterPoint(Vector2 value);
}
