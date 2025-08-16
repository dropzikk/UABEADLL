using System.Linq;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
public class TabControl : SelectingItemsControl, IContentPresenterHost
{
	private object? _selectedContent;

	private IDataTemplate? _selectedContentTemplate;

	private CompositeDisposable? _selectedItemSubscriptions;

	public static readonly StyledProperty<Dock> TabStripPlacementProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty;

	public static readonly DirectProperty<TabControl, object?> SelectedContentProperty;

	public static readonly DirectProperty<TabControl, IDataTemplate?> SelectedContentTemplateProperty;

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	public Dock TabStripPlacement
	{
		get
		{
			return GetValue(TabStripPlacementProperty);
		}
		set
		{
			SetValue(TabStripPlacementProperty, value);
		}
	}

	public IDataTemplate? ContentTemplate
	{
		get
		{
			return GetValue(ContentTemplateProperty);
		}
		set
		{
			SetValue(ContentTemplateProperty, value);
		}
	}

	public object? SelectedContent
	{
		get
		{
			return _selectedContent;
		}
		internal set
		{
			SetAndRaise(SelectedContentProperty, ref _selectedContent, value);
		}
	}

	public IDataTemplate? SelectedContentTemplate
	{
		get
		{
			return _selectedContentTemplate;
		}
		internal set
		{
			SetAndRaise(SelectedContentTemplateProperty, ref _selectedContentTemplate, value);
		}
	}

	internal ItemsPresenter? ItemsPresenterPart { get; private set; }

	internal ContentPresenter? ContentPart { get; private set; }

	IAvaloniaList<ILogical> IContentPresenterHost.LogicalChildren => base.LogicalChildren;

	static TabControl()
	{
		TabStripPlacementProperty = AvaloniaProperty.Register<TabControl, Dock>("TabStripPlacement", Dock.Top);
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<TabControl>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<TabControl>();
		ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner<TabControl>();
		SelectedContentProperty = AvaloniaProperty.RegisterDirect("SelectedContent", (TabControl o) => o.SelectedContent);
		SelectedContentTemplateProperty = AvaloniaProperty.RegisterDirect("SelectedContentTemplate", (TabControl o) => o.SelectedContentTemplate);
		DefaultPanel = new FuncTemplate<Panel>(() => new WrapPanel());
		SelectingItemsControl.SelectionModeProperty.OverrideDefaultValue<TabControl>(SelectionMode.AlwaysSelected);
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<TabControl>(DefaultPanel);
		Layoutable.AffectsMeasure<TabControl>(new AvaloniaProperty[1] { TabStripPlacementProperty });
		SelectingItemsControl.SelectedItemProperty.Changed.AddClassHandler(delegate(TabControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.UpdateSelectedContent();
		});
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabControl>(AutomationControlType.Tab);
	}

	bool IContentPresenterHost.RegisterContentPresenter(ContentPresenter presenter)
	{
		return RegisterContentPresenter(presenter);
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new TabItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<TabItem>(item, out recycleKey);
	}

	protected internal override void PrepareContainerForItemOverride(Control element, object? item, int index)
	{
		base.PrepareContainerForItemOverride(element, item, index);
		if (index == base.SelectedIndex)
		{
			UpdateSelectedContent(element);
		}
	}

	protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
	{
		base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
		int selectedIndex = base.SelectedIndex;
		if (selectedIndex == oldIndex || selectedIndex == newIndex)
		{
			UpdateSelectedContent();
		}
	}

	protected internal override void ClearContainerForItemOverride(Control element)
	{
		base.ClearContainerForItemOverride(element);
		UpdateSelectedContent();
	}

	private void UpdateSelectedContent(Control? container = null)
	{
		_selectedItemSubscriptions?.Dispose();
		_selectedItemSubscriptions = null;
		if (base.SelectedIndex == -1)
		{
			IDataTemplate selectedContent = (SelectedContentTemplate = null);
			SelectedContent = selectedContent;
			return;
		}
		if (container == null)
		{
			container = ContainerFromIndex(base.SelectedIndex);
		}
		if (container != null)
		{
			_selectedItemSubscriptions = new CompositeDisposable(container.GetObservable(ContentControl.ContentProperty).Subscribe(delegate(object v)
			{
				SelectedContent = v;
			}), container.GetObservable(ContentControl.ContentTemplateProperty).Subscribe(delegate(IDataTemplate v)
			{
				SelectedContentTemplate = v ?? ContentTemplate;
			}));
		}
	}

	protected virtual bool RegisterContentPresenter(ContentPresenter presenter)
	{
		if (presenter.Name == "PART_SelectedContentHost")
		{
			ContentPart = presenter;
			return true;
		}
		return false;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		ItemsPresenterPart = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");
		ItemsPresenterPart?.ApplyTemplate();
		Panel panel = ItemsPresenterPart?.Panel;
		if (panel != null)
		{
			if (!panel.IsSet(KeyboardNavigation.TabNavigationProperty))
			{
				panel.SetCurrentValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Once);
			}
			KeyboardNavigation.SetTabOnceActiveElement(panel, KeyboardNavigation.GetTabOnceActiveElement(this));
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (e.NavigationMethod == NavigationMethod.Directional)
		{
			e.Handled = UpdateSelectionFromEventSource(e.Source);
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
		{
			e.Handled = UpdateSelectionFromEventSource(e.Source);
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left && e.Pointer.Type != 0)
		{
			Control container = GetContainerFromEventSource(e.Source);
			if (container != null && container.GetVisualsAt(e.GetPosition(container)).Any((Visual c) => container == c || container.IsVisualAncestorOf(c)))
			{
				e.Handled = UpdateSelectionFromEventSource(e.Source);
			}
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TabStripPlacementProperty)
		{
			RefreshContainers();
		}
		else if (change.Property == ContentTemplateProperty)
		{
			IDataTemplate newValue = change.GetNewValue<IDataTemplate>();
			if (SelectedContentTemplate != newValue)
			{
				Control control = ContainerFromIndex(base.SelectedIndex);
				if (control != null && control.GetValue(ContentControl.ContentTemplateProperty) == null)
				{
					SelectedContentTemplate = newValue;
				}
			}
		}
		else if (change.Property == KeyboardNavigation.TabOnceActiveElementProperty)
		{
			Panel panel = ItemsPresenterPart?.Panel;
			if (panel != null)
			{
				KeyboardNavigation.SetTabOnceActiveElement(panel, change.GetNewValue<IInputElement>());
			}
		}
	}
}
