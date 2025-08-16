using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Threading;

namespace Avalonia.Utilities;

public class WeakEvents
{
	public static readonly WeakEvent<INotifyCollectionChanged, NotifyCollectionChangedEventArgs> CollectionChanged = WeakEvent.Register(delegate(INotifyCollectionChanged c, EventHandler<NotifyCollectionChangedEventArgs> s)
	{
		NotifyCollectionChangedEventHandler handler = delegate(object? _, NotifyCollectionChangedEventArgs e)
		{
			s(c, e);
		};
		c.CollectionChanged += handler;
		return delegate
		{
			c.CollectionChanged -= handler;
		};
	});

	public static readonly WeakEvent<INotifyPropertyChanged, PropertyChangedEventArgs> ThreadSafePropertyChanged = WeakEvent.Register(delegate(INotifyPropertyChanged s, EventHandler<PropertyChangedEventArgs> h)
	{
		bool unsubscribed = false;
		PropertyChangedEventHandler handler2 = delegate(object? _, PropertyChangedEventArgs e)
		{
			if (Dispatcher.UIThread.CheckAccess())
			{
				h(s, e);
			}
			else
			{
				Dispatcher.UIThread.Post(delegate
				{
					if (!unsubscribed)
					{
						h(s, e);
					}
				});
			}
		};
		s.PropertyChanged += handler2;
		return delegate
		{
			unsubscribed = true;
			s.PropertyChanged -= handler2;
		};
	});

	public static readonly WeakEvent<AvaloniaObject, AvaloniaPropertyChangedEventArgs> AvaloniaPropertyChanged = WeakEvent.Register(delegate(AvaloniaObject s, EventHandler<AvaloniaPropertyChangedEventArgs> h)
	{
		EventHandler<AvaloniaPropertyChangedEventArgs> handler3 = delegate(object? _, AvaloniaPropertyChangedEventArgs e)
		{
			h(s, e);
		};
		s.PropertyChanged += handler3;
		return delegate
		{
			s.PropertyChanged -= handler3;
		};
	});

	public static readonly WeakEvent<ICommand, EventArgs> CommandCanExecuteChanged = WeakEvent.Register(delegate(ICommand s, EventHandler h)
	{
		s.CanExecuteChanged += h;
	}, delegate(ICommand s, EventHandler h)
	{
		s.CanExecuteChanged -= h;
	});
}
