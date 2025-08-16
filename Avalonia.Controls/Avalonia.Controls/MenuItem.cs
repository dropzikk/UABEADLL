using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_Popup", typeof(Popup))]
[PseudoClasses(new string[] { ":separator", ":icon", ":open", ":pressed", ":selected" })]
public class MenuItem : HeaderedSelectingItemsControl, IMenuItem, IMenuElement, IInputElement, ILogical, ISelectable, ICommandSource, IClickableControl
{
	private class DependencyResolver : IAvaloniaDependencyResolver
	{
		public static readonly DependencyResolver Instance = new DependencyResolver();

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(IAccessKeyHandler))
			{
				return new MenuItemAccessKeyHandler();
			}
			return AvaloniaLocator.Current.GetService(serviceType);
		}
	}

	public static readonly StyledProperty<ICommand?> CommandProperty;

	public static readonly StyledProperty<KeyGesture?> HotKeyProperty;

	public static readonly StyledProperty<object?> CommandParameterProperty;

	public static readonly StyledProperty<object?> IconProperty;

	public static readonly StyledProperty<KeyGesture?> InputGestureProperty;

	public static readonly StyledProperty<bool> IsSubMenuOpenProperty;

	public static readonly StyledProperty<bool> StaysOpenOnClickProperty;

	public static readonly RoutedEvent<RoutedEventArgs> ClickEvent;

	public static readonly RoutedEvent<RoutedEventArgs> PointerEnteredItemEvent;

	public static readonly RoutedEvent<RoutedEventArgs> PointerExitedItemEvent;

	public static readonly RoutedEvent<RoutedEventArgs> SubmenuOpenedEvent;

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	private bool _commandCanExecute = true;

	private bool _commandBindingError;

	private Popup? _popup;

	private KeyGesture? _hotkey;

	private bool _isEmbeddedInMenu;

	public ICommand? Command
	{
		get
		{
			return GetValue(CommandProperty);
		}
		set
		{
			SetValue(CommandProperty, value);
		}
	}

	public KeyGesture? HotKey
	{
		get
		{
			return GetValue(HotKeyProperty);
		}
		set
		{
			SetValue(HotKeyProperty, value);
		}
	}

	public object? CommandParameter
	{
		get
		{
			return GetValue(CommandParameterProperty);
		}
		set
		{
			SetValue(CommandParameterProperty, value);
		}
	}

	public object? Icon
	{
		get
		{
			return GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	public KeyGesture? InputGesture
	{
		get
		{
			return GetValue(InputGestureProperty);
		}
		set
		{
			SetValue(InputGestureProperty, value);
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetValue(SelectingItemsControl.IsSelectedProperty);
		}
		set
		{
			SetValue(SelectingItemsControl.IsSelectedProperty, value);
		}
	}

	public bool IsSubMenuOpen
	{
		get
		{
			return GetValue(IsSubMenuOpenProperty);
		}
		set
		{
			SetValue(IsSubMenuOpenProperty, value);
		}
	}

	public bool StaysOpenOnClick
	{
		get
		{
			return GetValue(StaysOpenOnClickProperty);
		}
		set
		{
			SetValue(StaysOpenOnClickProperty, value);
		}
	}

	public bool HasSubMenu => !base.Classes.Contains(":empty");

	public bool IsTopLevel => base.Parent is Menu;

	bool IMenuItem.IsPointerOverSubMenu => _popup?.IsPointerOverPopup ?? false;

	IMenuElement? IMenuItem.Parent => base.Parent as IMenuElement;

	protected override bool IsEnabledCore
	{
		get
		{
			if (base.IsEnabledCore)
			{
				return _commandCanExecute;
			}
			return false;
		}
	}

	IMenuItem? IMenuElement.SelectedItem
	{
		get
		{
			int selectedIndex = base.SelectedIndex;
			if (selectedIndex == -1)
			{
				return null;
			}
			return (IMenuItem)ContainerFromIndex(selectedIndex);
		}
		set
		{
			base.SelectedIndex = ((value is Control container) ? IndexFromContainer(container) : (-1));
		}
	}

	IEnumerable<IMenuItem> IMenuElement.SubItems => GetRealizedContainers().OfType<IMenuItem>();

	public event EventHandler<RoutedEventArgs>? Click
	{
		add
		{
			AddHandler(ClickEvent, value);
		}
		remove
		{
			RemoveHandler(ClickEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? PointerEnteredItem
	{
		add
		{
			AddHandler(PointerEnteredItemEvent, value);
		}
		remove
		{
			RemoveHandler(PointerEnteredItemEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? PointerExitedItem
	{
		add
		{
			AddHandler(PointerExitedItemEvent, value);
		}
		remove
		{
			RemoveHandler(PointerExitedItemEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? SubmenuOpened
	{
		add
		{
			AddHandler(SubmenuOpenedEvent, value);
		}
		remove
		{
			RemoveHandler(SubmenuOpenedEvent, value);
		}
	}

	static MenuItem()
	{
		CommandProperty = Button.CommandProperty.AddOwner<MenuItem>(new StyledPropertyMetadata<ICommand>(default(Optional<ICommand>), BindingMode.Default, null, enableDataValidation: true));
		HotKeyProperty = HotKeyManager.HotKeyProperty.AddOwner<MenuItem>();
		CommandParameterProperty = Button.CommandParameterProperty.AddOwner<MenuItem>();
		IconProperty = AvaloniaProperty.Register<MenuItem, object>("Icon");
		InputGestureProperty = AvaloniaProperty.Register<MenuItem, KeyGesture>("InputGesture");
		IsSubMenuOpenProperty = AvaloniaProperty.Register<MenuItem, bool>("IsSubMenuOpen", defaultValue: false);
		StaysOpenOnClickProperty = AvaloniaProperty.Register<MenuItem, bool>("StaysOpenOnClick", defaultValue: false);
		ClickEvent = RoutedEvent.Register<MenuItem, RoutedEventArgs>("Click", RoutingStrategies.Bubble);
		PointerEnteredItemEvent = RoutedEvent.Register<MenuItem, RoutedEventArgs>("PointerEnteredItem", RoutingStrategies.Bubble);
		PointerExitedItemEvent = RoutedEvent.Register<MenuItem, RoutedEventArgs>("PointerExitedItem", RoutingStrategies.Bubble);
		SubmenuOpenedEvent = RoutedEvent.Register<MenuItem, RoutedEventArgs>("SubmenuOpened", RoutingStrategies.Bubble);
		DefaultPanel = new FuncTemplate<Panel>(() => new StackPanel());
		SelectableMixin.Attach<MenuItem>(SelectingItemsControl.IsSelectedProperty);
		PressedMixin.Attach<MenuItem>();
		CommandProperty.Changed.Subscribe(CommandChanged);
		CommandParameterProperty.Changed.Subscribe(CommandParameterChanged);
		InputElement.FocusableProperty.OverrideDefaultValue<MenuItem>(defaultValue: true);
		HeaderedSelectingItemsControl.HeaderProperty.Changed.AddClassHandler(delegate(MenuItem x, AvaloniaPropertyChangedEventArgs e)
		{
			x.HeaderChanged(e);
		});
		IconProperty.Changed.AddClassHandler(delegate(MenuItem x, AvaloniaPropertyChangedEventArgs e)
		{
			x.IconChanged(e);
		});
		SelectingItemsControl.IsSelectedProperty.Changed.AddClassHandler(delegate(MenuItem x, AvaloniaPropertyChangedEventArgs e)
		{
			x.IsSelectedChanged(e);
		});
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<MenuItem>(DefaultPanel);
		ClickEvent.AddClassHandler(delegate(MenuItem x, RoutedEventArgs e)
		{
			x.OnClick(e);
		});
		SubmenuOpenedEvent.AddClassHandler(delegate(MenuItem x, RoutedEventArgs e)
		{
			x.OnSubmenuOpened(e);
		});
		IsSubMenuOpenProperty.Changed.AddClassHandler(delegate(MenuItem x, AvaloniaPropertyChangedEventArgs e)
		{
			x.SubMenuOpenChanged(e);
		});
	}

	public MenuItem()
	{
		IObservable<DefinitionBase.SharedSizeScope> source = (from x in this.GetObservable(Visual.VisualParentProperty)
			select (x as Control)?.GetObservable(DefinitionBase.PrivateSharedSizeScopeProperty) ?? Observable.Return<DefinitionBase.SharedSizeScope>(null)).Switch();
		Bind(DefinitionBase.PrivateSharedSizeScopeProperty, source);
	}

	bool IMenuElement.MoveSelection(NavigationDirection direction, bool wrap)
	{
		return MoveSelection(direction, wrap);
	}

	public void Open()
	{
		SetCurrentValue(IsSubMenuOpenProperty, value: true);
	}

	public void Close()
	{
		SetCurrentValue(IsSubMenuOpenProperty, value: false);
	}

	void IMenuItem.RaiseClick()
	{
		RaiseEvent(new RoutedEventArgs(ClickEvent));
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new MenuItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		if (item is MenuItem || item is Separator)
		{
			recycleKey = null;
			return false;
		}
		recycleKey = ItemsControl.DefaultRecycleKey;
		return true;
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (!_isEmbeddedInMenu)
		{
			RaiseEvent(new RoutedEventArgs(ClickEvent));
		}
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (_hotkey != null)
		{
			SetCurrentValue(HotKeyProperty, _hotkey);
		}
		base.OnAttachedToLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged += CanExecuteChanged;
		}
		TryUpdateCanExecute();
		StyledElement parent = base.Parent;
		while (parent is MenuItem)
		{
			parent = parent.Parent;
		}
		_isEmbeddedInMenu = parent?.FindLogicalAncestorOfType<IMenu>(includeSelf: true) != null;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		TryUpdateCanExecute();
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (HotKey != null)
		{
			_hotkey = HotKey;
			SetCurrentValue(HotKeyProperty, null);
		}
		base.OnDetachedFromLogicalTree(e);
		if (Command != null)
		{
			Command.CanExecuteChanged -= CanExecuteChanged;
		}
	}

	protected virtual void OnClick(RoutedEventArgs e)
	{
		if (!e.Handled)
		{
			ICommand? command = Command;
			if (command != null && command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
				e.Handled = true;
			}
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		e.Handled = UpdateSelectionFromEventSource(e.Source);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		RaiseEvent(new RoutedEventArgs(PointerEnteredItemEvent));
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		base.OnPointerExited(e);
		RaiseEvent(new RoutedEventArgs(PointerExitedItemEvent));
	}

	protected virtual void OnSubmenuOpened(RoutedEventArgs e)
	{
		if (!(e.Source is MenuItem menuItem) || menuItem.Parent != this)
		{
			return;
		}
		foreach (IMenuItem subItem in ((IMenuElement)this).SubItems)
		{
			if (subItem != menuItem && subItem.IsSubMenuOpen)
			{
				subItem.IsSubMenuOpen = false;
			}
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_popup != null)
		{
			_popup.Opened -= PopupOpened;
			_popup.Closed -= PopupClosed;
			_popup.DependencyResolver = null;
		}
		_popup = e.NameScope.Find<Popup>("PART_Popup");
		if (_popup != null)
		{
			_popup.DependencyResolver = DependencyResolver.Instance;
			_popup.Opened += PopupOpened;
			_popup.Closed += PopupClosed;
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new MenuItemAutomationPeer(this);
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		base.UpdateDataValidation(property, state, error);
		if (property == CommandProperty)
		{
			_commandBindingError = state == BindingValueType.BindingError;
			if (_commandBindingError && _commandCanExecute)
			{
				_commandCanExecute = false;
				UpdateIsEffectivelyEnabled();
			}
		}
	}

	private void CloseSubmenus()
	{
		foreach (IMenuItem subItem in ((IMenuElement)this).SubItems)
		{
			subItem.IsSubMenuOpen = false;
		}
	}

	private static void CommandChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is MenuItem { IsAttachedToLogicalTree: not false } menuItem)
		{
			if (e.OldValue is ICommand command)
			{
				command.CanExecuteChanged -= menuItem.CanExecuteChanged;
			}
			if (e.NewValue is ICommand command2)
			{
				command2.CanExecuteChanged += menuItem.CanExecuteChanged;
			}
			menuItem.TryUpdateCanExecute();
		}
	}

	private static void CommandParameterChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is MenuItem menuItem)
		{
			menuItem.TryUpdateCanExecute();
		}
	}

	private void CanExecuteChanged(object? sender, EventArgs e)
	{
		TryUpdateCanExecute();
	}

	private void TryUpdateCanExecute()
	{
		if (Command == null)
		{
			_commandCanExecute = !_commandBindingError;
			UpdateIsEffectivelyEnabled();
		}
		else if (((ILogical)this).IsAttachedToLogicalTree && !(base.Parent is MenuItem { IsSubMenuOpen: false }))
		{
			bool flag = Command.CanExecute(CommandParameter);
			if (flag != _commandCanExecute)
			{
				_commandCanExecute = flag;
				UpdateIsEffectivelyEnabled();
			}
		}
	}

	private void HeaderChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue is string text && text == "-")
		{
			base.PseudoClasses.Add(":separator");
			base.Focusable = false;
		}
		else if (e.OldValue is string text2 && text2 == "-")
		{
			base.PseudoClasses.Remove(":separator");
			base.Focusable = true;
		}
	}

	private void IconChanged(AvaloniaPropertyChangedEventArgs e)
	{
		ILogical logical = e.OldValue as ILogical;
		ILogical logical2 = e.NewValue as ILogical;
		if (logical != null)
		{
			base.LogicalChildren.Remove(logical);
			base.PseudoClasses.Remove(":icon");
		}
		if (logical2 != null)
		{
			base.LogicalChildren.Add(logical2);
			base.PseudoClasses.Add(":icon");
		}
	}

	private void IsSelectedChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Menu menu = base.Parent as Menu;
		if ((bool)e.NewValue && (menu == null || menu.IsOpen))
		{
			Focus();
		}
	}

	private void SubMenuOpenChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if ((bool)e.NewValue)
		{
			foreach (MenuItem item in base.ItemsView.OfType<MenuItem>())
			{
				item.TryUpdateCanExecute();
			}
			RaiseEvent(new RoutedEventArgs(SubmenuOpenedEvent));
			SetCurrentValue(SelectingItemsControl.IsSelectedProperty, value: true);
			base.PseudoClasses.Add(":open");
		}
		else
		{
			CloseSubmenus();
			base.SelectedIndex = -1;
			base.PseudoClasses.Remove(":open");
		}
	}

	private void PopupOpened(object? sender, EventArgs e)
	{
		ItemsPresenter? presenter = base.Presenter;
		if (presenter != null && !presenter.IsAttachedToVisualTree)
		{
			UpdateLayout();
		}
		int selectedIndex = base.SelectedIndex;
		if (selectedIndex != -1)
		{
			ContainerFromIndex(selectedIndex)?.Focus();
		}
	}

	private void PopupClosed(object? sender, EventArgs e)
	{
		base.SelectedItem = null;
	}

	private new void UpdateLayout()
	{
		(base.VisualRoot as ILayoutRoot)?.LayoutManager.ExecuteLayoutPass();
	}

	void ICommandSource.CanExecuteChanged(object sender, EventArgs e)
	{
		CanExecuteChanged(sender, e);
	}

	void IClickableControl.RaiseClick()
	{
		if (base.IsEffectivelyEnabled)
		{
			RaiseEvent(new RoutedEventArgs(ClickEvent));
		}
	}
}
