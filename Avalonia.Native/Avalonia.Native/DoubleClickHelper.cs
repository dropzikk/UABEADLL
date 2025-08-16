using Avalonia.Input;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class DoubleClickHelper
{
	private int _clickCount;

	private Rect _lastClickRect;

	private ulong _lastClickTime;

	public bool IsDoubleClick(ulong timestamp, Point p)
	{
		IPlatformSettings? service = AvaloniaLocator.Current.GetService<IPlatformSettings>();
		double num = service?.GetDoubleTapTime(PointerType.Mouse).TotalMilliseconds ?? 500.0;
		Size size = service?.GetDoubleTapSize(PointerType.Mouse) ?? new Size(4.0, 4.0);
		if (!_lastClickRect.Contains(p) || (double)(timestamp - _lastClickTime) > num)
		{
			_clickCount = 0;
		}
		_clickCount++;
		_lastClickTime = timestamp;
		_lastClickRect = new Rect(p, default(Size)).Inflate(new Thickness(size.Width / 2.0, size.Height / 2.0));
		return _clickCount == 2;
	}
}
