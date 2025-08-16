using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal static class FramePriorityExtensions
{
	public static FramePriority ToFramePriority(this BindingPriority priority, FrameType type = FrameType.Style)
	{
		return (FramePriority)((int)((priority > BindingPriority.LocalValue) ? priority : (priority + 1)) * 3 + type);
	}

	public static bool IsType(this FramePriority priority, FrameType type)
	{
		return (int)priority % 3 == (int)type;
	}
}
