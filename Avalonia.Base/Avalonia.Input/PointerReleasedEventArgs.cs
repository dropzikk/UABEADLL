using Avalonia.Metadata;

namespace Avalonia.Input;

public class PointerReleasedEventArgs : PointerEventArgs
{
	public MouseButton InitialPressMouseButton { get; }

	[Unstable("This constructor might be removed in 12.0. For unit testing, consider using IHeadlessWindow mouse methods.")]
	public PointerReleasedEventArgs(object source, IPointer pointer, Visual rootVisual, Point rootVisualPosition, ulong timestamp, PointerPointProperties properties, KeyModifiers modifiers, MouseButton initialPressMouseButton)
		: base(InputElement.PointerReleasedEvent, source, pointer, rootVisual, rootVisualPosition, timestamp, properties, modifiers)
	{
		InitialPressMouseButton = initialPressMouseButton;
	}
}
