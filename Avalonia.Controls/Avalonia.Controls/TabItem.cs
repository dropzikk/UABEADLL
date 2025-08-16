using System;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":pressed", ":selected" })]
public class TabItem : HeaderedContentControl, ISelectable
{
	private Dock? _tabStripPlacement;

	private IDisposable? _ownerSubscriptions;

	public static readonly DirectProperty<TabItem, Dock?> TabStripPlacementProperty;

	public static readonly StyledProperty<bool> IsSelectedProperty;

	public Dock? TabStripPlacement
	{
		get
		{
			return _tabStripPlacement;
		}
		private set
		{
			SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetValue(IsSelectedProperty);
		}
		set
		{
			SetValue(IsSelectedProperty, value);
		}
	}

	static TabItem()
	{
		TabStripPlacementProperty = AvaloniaProperty.RegisterDirect("TabStripPlacement", (TabItem o) => o.TabStripPlacement, null, null);
		IsSelectedProperty = SelectingItemsControl.IsSelectedProperty.AddOwner<TabItem>();
		SelectableMixin.Attach<TabItem>(IsSelectedProperty);
		PressedMixin.Attach<TabItem>();
		InputElement.FocusableProperty.OverrideDefaultValue(typeof(TabItem), defaultValue: true);
		StyledElement.DataContextProperty.Changed.AddClassHandler(delegate(TabItem x, AvaloniaPropertyChangedEventArgs e)
		{
			x.UpdateHeader(e);
		});
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabItem>(AutomationControlType.TabItem);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ListItemAutomationPeer(this);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_ownerSubscriptions?.Dispose();
		_ownerSubscriptions = null;
		TabControl tabControl = this.FindAncestorOfType<TabControl>();
		if (tabControl != null && tabControl.IndexFromContainer(this) != -1)
		{
			SubscribeToOwnerProperties(tabControl);
		}
	}

	protected void SubscribeToOwnerProperties(AvaloniaObject owner)
	{
		_ownerSubscriptions = owner.GetObservable(TabControl.TabStripPlacementProperty).Subscribe(delegate(Dock v)
		{
			TabStripPlacement = v;
		});
	}

	private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
	{
		if (base.Header == null)
		{
			if (obj.NewValue is IHeadered headered)
			{
				if (base.Header != headered.Header)
				{
					base.Header = headered.Header;
				}
			}
			else if (!(obj.NewValue is Control))
			{
				base.Header = obj.NewValue;
			}
		}
		else if (base.Header == obj.OldValue)
		{
			base.Header = obj.NewValue;
		}
	}
}
