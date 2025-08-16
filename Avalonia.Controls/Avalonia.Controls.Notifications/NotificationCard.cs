using System;
using System.Linq;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Controls.Notifications;

[PseudoClasses(new string[] { ":error", ":information", ":success", ":warning" })]
public class NotificationCard : ContentControl
{
	private bool _isClosing;

	public static readonly DirectProperty<NotificationCard, bool> IsClosingProperty;

	public static readonly StyledProperty<bool> IsClosedProperty;

	public static readonly RoutedEvent<RoutedEventArgs> NotificationClosedEvent;

	public static readonly AttachedProperty<bool> CloseOnClickProperty;

	public bool IsClosing
	{
		get
		{
			return _isClosing;
		}
		private set
		{
			SetAndRaise(IsClosingProperty, ref _isClosing, value);
		}
	}

	public bool IsClosed
	{
		get
		{
			return GetValue(IsClosedProperty);
		}
		set
		{
			SetValue(IsClosedProperty, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? NotificationClosed
	{
		add
		{
			AddHandler(NotificationClosedEvent, value);
		}
		remove
		{
			RemoveHandler(NotificationClosedEvent, value);
		}
	}

	static NotificationCard()
	{
		IsClosingProperty = AvaloniaProperty.RegisterDirect("IsClosing", (NotificationCard o) => o.IsClosing, null, unsetValue: false);
		IsClosedProperty = AvaloniaProperty.Register<NotificationCard, bool>("IsClosed", defaultValue: false);
		NotificationClosedEvent = RoutedEvent.Register<NotificationCard, RoutedEventArgs>("NotificationClosed", RoutingStrategies.Bubble);
		CloseOnClickProperty = AvaloniaProperty.RegisterAttached<NotificationCard, Button, bool>("CloseOnClick", defaultValue: false);
		CloseOnClickProperty.Changed.AddClassHandler<Button>(OnCloseOnClickPropertyChanged);
	}

	public NotificationCard()
	{
		this.GetObservable(IsClosedProperty).Subscribe(delegate
		{
			if (IsClosing || IsClosed)
			{
				RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
			}
		});
		this.GetObservable(ContentControl.ContentProperty).Subscribe(delegate(object x)
		{
			if (x is Notification notification)
			{
				switch (notification.Type)
				{
				case NotificationType.Error:
					base.PseudoClasses.Add(":error");
					break;
				case NotificationType.Information:
					base.PseudoClasses.Add(":information");
					break;
				case NotificationType.Success:
					base.PseudoClasses.Add(":success");
					break;
				case NotificationType.Warning:
					base.PseudoClasses.Add(":warning");
					break;
				}
			}
		});
	}

	public static bool GetCloseOnClick(Button obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		return obj.GetValue(CloseOnClickProperty);
	}

	public static void SetCloseOnClick(Button obj, bool value)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		obj.SetValue(CloseOnClickProperty, value);
	}

	private static void OnCloseOnClickPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
	{
		Button button = (Button)d;
		if ((bool)e.NewValue)
		{
			button.Click += Button_Click;
		}
		else
		{
			button.Click -= Button_Click;
		}
	}

	private static void Button_Click(object? sender, RoutedEventArgs e)
	{
		((sender as ILogical)?.GetLogicalAncestors().OfType<NotificationCard>().FirstOrDefault())?.Close();
	}

	public void Close()
	{
		if (!IsClosing)
		{
			IsClosing = true;
		}
	}
}
