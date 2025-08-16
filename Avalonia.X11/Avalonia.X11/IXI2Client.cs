using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Avalonia.X11;

internal interface IXI2Client
{
	bool IsEnabled { get; }

	IInputRoot InputRoot { get; }

	IMouseDevice MouseDevice { get; }

	TouchDevice TouchDevice { get; }

	void ScheduleXI2Input(RawInputEventArgs args);
}
