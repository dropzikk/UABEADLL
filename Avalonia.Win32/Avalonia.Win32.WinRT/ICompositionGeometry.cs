using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionGeometry : IInspectable, IUnknown, IDisposable
{
	float TrimEnd { get; }

	float TrimOffset { get; }

	float TrimStart { get; }

	void SetTrimEnd(float value);

	void SetTrimOffset(float value);

	void SetTrimStart(float value);
}
