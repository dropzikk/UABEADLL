using Avalonia.Metadata;

namespace Avalonia.Input;

public class PointerPressedEventArgs : PointerEventArgs
{
	public int ClickCount { get; }

	[Unstable("This constructor might be removed in 12.0. For unit testing, consider using IHeadlessWindow mouse methods.")]
	public PointerPressedEventArgs(object source, IPointer pointer, Visual rootVisual, Point rootVisualPosition, ulong timestamp, PointerPointProperties properties, KeyModifiers modifiers, int clickCount = 1)
		: base(InputElement.PointerPressedEvent, source, pointer, rootVisual, rootVisualPosition, timestamp, properties, modifiers)
	{
		ClickCount = clickCount;
	}
}
