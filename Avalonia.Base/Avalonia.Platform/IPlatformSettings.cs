using System;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[NotClientImplementable]
public interface IPlatformSettings
{
	TimeSpan HoldWaitDuration { get; }

	PlatformHotkeyConfiguration HotkeyConfiguration { get; }

	event EventHandler<PlatformColorValues>? ColorValuesChanged;

	Size GetTapSize(PointerType type);

	Size GetDoubleTapSize(PointerType type);

	TimeSpan GetDoubleTapTime(PointerType type);

	PlatformColorValues GetColorValues();
}
