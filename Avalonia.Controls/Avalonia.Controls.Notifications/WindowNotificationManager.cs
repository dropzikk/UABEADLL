using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Notifications;

[TemplatePart("PART_Items", typeof(Panel))]
[PseudoClasses(new string[] { ":topleft", ":topright", ":bottomleft", ":bottomright" })]
public class WindowNotificationManager : TemplatedControl, IManagedNotificationManager, INotificationManager
{
	private IList? _items;

	public static readonly StyledProperty<NotificationPosition> PositionProperty;

	public static readonly StyledProperty<int> MaxItemsProperty;

	public NotificationPosition Position
	{
		get
		{
			return GetValue(PositionProperty);
		}
		set
		{
			SetValue(PositionProperty, value);
		}
	}

	public int MaxItems
	{
		get
		{
			return GetValue(MaxItemsProperty);
		}
		set
		{
			SetValue(MaxItemsProperty, value);
		}
	}

	public WindowNotificationManager(TopLevel? host)
	{
		if (host != null)
		{
			Install(host);
		}
		UpdatePseudoClasses(Position);
	}

	static WindowNotificationManager()
	{
		PositionProperty = AvaloniaProperty.Register<WindowNotificationManager, NotificationPosition>("Position", NotificationPosition.TopRight);
		MaxItemsProperty = AvaloniaProperty.Register<WindowNotificationManager, int>("MaxItems", 5);
		Layoutable.HorizontalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(HorizontalAlignment.Stretch);
		Layoutable.VerticalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(VerticalAlignment.Stretch);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_items = e.NameScope.Find<Panel>("PART_Items")?.Children;
	}

	public void Show(INotification content)
	{
		Show((object)content);
	}

	public async void Show(object content)
	{
		INotification notification = content as INotification;
		NotificationCard notificationControl = new NotificationCard
		{
			Content = content
		};
		notificationControl.NotificationClosed += delegate(object? sender, RoutedEventArgs args)
		{
			notification?.OnClose?.Invoke();
			_items?.Remove(sender);
		};
		notificationControl.PointerPressed += delegate(object? sender, PointerPressedEventArgs args)
		{
			if (notification != null && notification.OnClick != null)
			{
				notification.OnClick();
			}
			(sender as NotificationCard)?.Close();
		};
		_items?.Add(notificationControl);
		if (_items?.OfType<NotificationCard>().Count((NotificationCard i) => !i.IsClosing) > MaxItems)
		{
			_items.OfType<NotificationCard>().First((NotificationCard i) => !i.IsClosing).Close();
		}
		if (notification == null || !(notification.Expiration == TimeSpan.Zero))
		{
			await Task.Delay(notification?.Expiration ?? TimeSpan.FromSeconds(5.0));
			notificationControl.Close();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == PositionProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<NotificationPosition>());
		}
	}

	private void Install(TemplatedControl host)
	{
		AdornerLayer adornerLayer = host.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer;
		if (adornerLayer != null)
		{
			adornerLayer.Children.Add(this);
			AdornerLayer.SetAdornedElement(this, adornerLayer);
		}
	}

	private void UpdatePseudoClasses(NotificationPosition position)
	{
		base.PseudoClasses.Set(":topleft", position == NotificationPosition.TopLeft);
		base.PseudoClasses.Set(":topright", position == NotificationPosition.TopRight);
		base.PseudoClasses.Set(":bottomleft", position == NotificationPosition.BottomLeft);
		base.PseudoClasses.Set(":bottomright", position == NotificationPosition.BottomRight);
	}
}
