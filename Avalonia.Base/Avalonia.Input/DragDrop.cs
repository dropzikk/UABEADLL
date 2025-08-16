using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace Avalonia.Input;

public static class DragDrop
{
	public static readonly RoutedEvent<DragEventArgs> DragEnterEvent = RoutedEvent.Register<DragEventArgs>("DragEnter", RoutingStrategies.Bubble, typeof(DragDrop));

	public static readonly RoutedEvent<DragEventArgs> DragLeaveEvent = RoutedEvent.Register<DragEventArgs>("DragLeave", RoutingStrategies.Bubble, typeof(DragDrop));

	public static readonly RoutedEvent<DragEventArgs> DragOverEvent = RoutedEvent.Register<DragEventArgs>("DragOver", RoutingStrategies.Bubble, typeof(DragDrop));

	public static readonly RoutedEvent<DragEventArgs> DropEvent = RoutedEvent.Register<DragEventArgs>("Drop", RoutingStrategies.Bubble, typeof(DragDrop));

	public static readonly AttachedProperty<bool> AllowDropProperty = AvaloniaProperty.RegisterAttached<Interactive, bool>("AllowDrop", typeof(DragDrop), defaultValue: false, inherits: true);

	public static bool GetAllowDrop(Interactive interactive)
	{
		return interactive.GetValue(AllowDropProperty);
	}

	public static void SetAllowDrop(Interactive interactive, bool value)
	{
		interactive.SetValue(AllowDropProperty, value);
	}

	public static Task<DragDropEffects> DoDragDrop(PointerEventArgs triggerEvent, IDataObject data, DragDropEffects allowedEffects)
	{
		return AvaloniaLocator.Current.GetService<IPlatformDragSource>()?.DoDragDrop(triggerEvent, data, allowedEffects) ?? Task.FromResult(DragDropEffects.None);
	}
}
