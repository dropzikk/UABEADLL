using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Collections;
using Avalonia.Controls.Platform;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class NativeMenu : AvaloniaObject, IEnumerable<NativeMenuItemBase>, IEnumerable, INativeMenuExporterEventsImplBridge
{
	private sealed class NativeMenuInfo
	{
		public bool ChangingIsExported { get; set; }

		public ITopLevelNativeMenuExporter? Exporter { get; }

		public NativeMenuInfo(TopLevel target)
		{
			NativeMenuInfo nativeMenuInfo = this;
			Exporter = target.PlatformImpl?.TryGetFeature<ITopLevelNativeMenuExporter>();
			if (Exporter != null)
			{
				Exporter.OnIsNativeMenuExportedChanged += delegate
				{
					SetIsNativeMenuExported(target, nativeMenuInfo.Exporter.IsNativeMenuExported);
				};
			}
		}
	}

	private readonly AvaloniaList<NativeMenuItemBase> _items = new AvaloniaList<NativeMenuItemBase>
	{
		ResetBehavior = ResetBehavior.Remove
	};

	private NativeMenuItem? _parent;

	public static readonly DirectProperty<NativeMenu, NativeMenuItem?> ParentProperty;

	public static readonly AttachedProperty<bool> IsNativeMenuExportedProperty;

	private static readonly AttachedProperty<NativeMenuInfo?> s_nativeMenuInfoProperty;

	public static readonly AttachedProperty<NativeMenu?> MenuProperty;

	[Content]
	public IList<NativeMenuItemBase> Items => _items;

	public NativeMenuItem? Parent
	{
		get
		{
			return _parent;
		}
		internal set
		{
			SetAndRaise(ParentProperty, ref _parent, value);
		}
	}

	public event EventHandler<EventArgs>? NeedsUpdate;

	public event EventHandler<EventArgs>? Opening;

	public event EventHandler<EventArgs>? Closed;

	public NativeMenu()
	{
		_items.Validate = Validator;
		_items.CollectionChanged += ItemsChanged;
	}

	void INativeMenuExporterEventsImplBridge.RaiseNeedsUpdate()
	{
		this.NeedsUpdate?.Invoke(this, EventArgs.Empty);
	}

	void INativeMenuExporterEventsImplBridge.RaiseOpening()
	{
		this.Opening?.Invoke(this, EventArgs.Empty);
	}

	void INativeMenuExporterEventsImplBridge.RaiseClosed()
	{
		this.Closed?.Invoke(this, EventArgs.Empty);
	}

	private void Validator(NativeMenuItemBase obj)
	{
		if (obj.Parent != null)
		{
			throw new InvalidOperationException("NativeMenuItem already has a parent");
		}
	}

	private void ItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			foreach (NativeMenuItemBase oldItem in e.OldItems)
			{
				oldItem.Parent = null;
			}
		}
		if (e.NewItems == null)
		{
			return;
		}
		foreach (NativeMenuItemBase newItem in e.NewItems)
		{
			newItem.Parent = this;
		}
	}

	public void Add(NativeMenuItemBase item)
	{
		_items.Add(item);
	}

	public IEnumerator<NativeMenuItemBase> GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static bool GetIsNativeMenuExported(TopLevel tl)
	{
		return tl.GetValue(IsNativeMenuExportedProperty);
	}

	private static NativeMenuInfo GetInfo(TopLevel target)
	{
		NativeMenuInfo nativeMenuInfo = target.GetValue(s_nativeMenuInfoProperty);
		if (nativeMenuInfo == null)
		{
			target.SetValue(s_nativeMenuInfoProperty, nativeMenuInfo = new NativeMenuInfo(target));
			SetIsNativeMenuExported(target, nativeMenuInfo.Exporter?.IsNativeMenuExported ?? false);
		}
		return nativeMenuInfo;
	}

	private static void SetIsNativeMenuExported(TopLevel tl, bool value)
	{
		GetInfo(tl).ChangingIsExported = true;
		tl.SetValue(IsNativeMenuExportedProperty, value);
	}

	public static void SetMenu(AvaloniaObject o, NativeMenu? menu)
	{
		o.SetValue(MenuProperty, menu);
	}

	public static NativeMenu? GetMenu(AvaloniaObject o)
	{
		return o.GetValue(MenuProperty);
	}

	static NativeMenu()
	{
		ParentProperty = AvaloniaProperty.RegisterDirect("Parent", (NativeMenu o) => o.Parent);
		IsNativeMenuExportedProperty = AvaloniaProperty.RegisterAttached<NativeMenu, TopLevel, bool>("IsNativeMenuExported", defaultValue: false);
		s_nativeMenuInfoProperty = AvaloniaProperty.RegisterAttached<NativeMenu, TopLevel, NativeMenuInfo>("___NativeMenuInfo");
		MenuProperty = AvaloniaProperty.RegisterAttached<NativeMenu, AvaloniaObject, NativeMenu>("Menu");
		IsNativeMenuExportedProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<bool> args)
		{
			NativeMenuInfo info = GetInfo((TopLevel)args.Sender);
			if (!info.ChangingIsExported)
			{
				throw new InvalidOperationException("IsNativeMenuExported property is read-only");
			}
			info.ChangingIsExported = false;
		});
		MenuProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<NativeMenu> args)
		{
			if (args.Sender is TopLevel target)
			{
				GetInfo(target).Exporter?.SetNativeMenu(args.NewValue.GetValueOrDefault());
			}
			else if (args.Sender is INativeMenuExporterProvider nativeMenuExporterProvider)
			{
				nativeMenuExporterProvider.NativeMenuExporter?.SetNativeMenu(args.NewValue.GetValueOrDefault());
			}
		});
	}
}
