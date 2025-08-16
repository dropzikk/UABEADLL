using System;
using System.Threading.Tasks;
using Avalonia.Input.Raw;

namespace Avalonia.FreeDesktop;

internal interface IX11InputMethodControl : IDisposable
{
	bool IsEnabled { get; }

	event Action<string> Commit;

	event Action<X11InputMethodForwardedKey> ForwardKey;

	void SetWindowActive(bool active);

	ValueTask<bool> HandleEventAsync(RawKeyEventArgs args, int keyVal, int keyCode);

	void UpdateWindowInfo(PixelPoint position, double scaling);
}
