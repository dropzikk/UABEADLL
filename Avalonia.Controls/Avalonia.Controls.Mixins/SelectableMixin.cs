using System;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Controls.Mixins;

public static class SelectableMixin
{
	public static void Attach<TControl>(AvaloniaProperty<bool> isSelected) where TControl : Control
	{
		if ((object)isSelected == null)
		{
			throw new ArgumentNullException("isSelected");
		}
		isSelected.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<bool> x)
		{
			if (x.Sender is TControl val)
			{
				PseudolassesExtensions.Set(val.Classes, ":selected", x.NewValue.GetValueOrDefault());
				val.RaiseEvent(new RoutedEventArgs
				{
					RoutedEvent = SelectingItemsControl.IsSelectedChangedEvent
				});
			}
		});
	}
}
