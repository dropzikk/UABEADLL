using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnPlatformSettings : IUnknown, IDisposable
{
	AvnPlatformThemeVariant PlatformTheme { get; }

	uint AccentColor { get; }

	void RegisterColorsChange(IAvnActionCallback callback);
}
