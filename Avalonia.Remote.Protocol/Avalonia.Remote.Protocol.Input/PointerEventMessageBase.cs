namespace Avalonia.Remote.Protocol.Input;

public abstract class PointerEventMessageBase : InputEventMessageBase
{
	public double X { get; set; }

	public double Y { get; set; }
}
