using System;
using System.Collections.Generic;

namespace Avalonia.Automation.Peers;

public abstract class AutomationPeer
{
	public event EventHandler? ChildrenChanged;

	public event EventHandler<AutomationPropertyChangedEventArgs>? PropertyChanged;

	public void BringIntoView()
	{
		BringIntoViewCore();
	}

	public string? GetAcceleratorKey()
	{
		return GetAcceleratorKeyCore();
	}

	public string? GetAccessKey()
	{
		return GetAccessKeyCore();
	}

	public AutomationControlType GetAutomationControlType()
	{
		return GetControlTypeOverrideCore();
	}

	public string? GetAutomationId()
	{
		return GetAutomationIdCore();
	}

	public Rect GetBoundingRectangle()
	{
		return GetBoundingRectangleCore();
	}

	public IReadOnlyList<AutomationPeer> GetChildren()
	{
		return GetOrCreateChildrenCore();
	}

	public string GetClassName()
	{
		return GetClassNameCore() ?? string.Empty;
	}

	public AutomationPeer? GetLabeledBy()
	{
		return GetLabeledByCore();
	}

	public string GetLocalizedControlType()
	{
		return GetLocalizedControlTypeCore();
	}

	public string GetName()
	{
		return GetNameCore() ?? string.Empty;
	}

	public AutomationPeer? GetParent()
	{
		return GetParentCore();
	}

	public bool HasKeyboardFocus()
	{
		return HasKeyboardFocusCore();
	}

	public bool IsContentElement()
	{
		return IsContentElementOverrideCore();
	}

	public bool IsControlElement()
	{
		return IsControlElementOverrideCore();
	}

	public bool IsEnabled()
	{
		return IsEnabledCore();
	}

	public bool IsKeyboardFocusable()
	{
		return IsKeyboardFocusableCore();
	}

	public void SetFocus()
	{
		SetFocusCore();
	}

	public bool ShowContextMenu()
	{
		return ShowContextMenuCore();
	}

	public T? GetProvider<T>()
	{
		return (T)GetProviderCore(typeof(T));
	}

	protected void RaiseChildrenChangedEvent()
	{
		this.ChildrenChanged?.Invoke(this, EventArgs.Empty);
	}

	public void RaisePropertyChangedEvent(AutomationProperty property, object? oldValue, object? newValue)
	{
		this.PropertyChanged?.Invoke(this, new AutomationPropertyChangedEventArgs(property, oldValue, newValue));
	}

	protected virtual string GetLocalizedControlTypeCore()
	{
		AutomationControlType automationControlType = GetAutomationControlType();
		return automationControlType switch
		{
			AutomationControlType.CheckBox => "check box", 
			AutomationControlType.ComboBox => "combo box", 
			AutomationControlType.ListItem => "list item", 
			AutomationControlType.MenuBar => "menu bar", 
			AutomationControlType.MenuItem => "menu item", 
			AutomationControlType.ProgressBar => "progress bar", 
			AutomationControlType.RadioButton => "radio button", 
			AutomationControlType.ScrollBar => "scroll bar", 
			AutomationControlType.StatusBar => "status bar", 
			AutomationControlType.TabItem => "tab item", 
			AutomationControlType.ToolBar => "toolbar", 
			AutomationControlType.ToolTip => "tooltip", 
			AutomationControlType.TreeItem => "tree item", 
			AutomationControlType.Custom => "custom", 
			AutomationControlType.DataGrid => "data grid", 
			AutomationControlType.DataItem => "data item", 
			AutomationControlType.SplitButton => "split button", 
			AutomationControlType.HeaderItem => "header item", 
			AutomationControlType.TitleBar => "title bar", 
			_ => automationControlType.ToString().ToLowerInvariant(), 
		};
	}

	protected abstract void BringIntoViewCore();

	protected abstract string? GetAcceleratorKeyCore();

	protected abstract string? GetAccessKeyCore();

	protected abstract AutomationControlType GetAutomationControlTypeCore();

	protected abstract string? GetAutomationIdCore();

	protected abstract Rect GetBoundingRectangleCore();

	protected abstract IReadOnlyList<AutomationPeer> GetOrCreateChildrenCore();

	protected abstract string GetClassNameCore();

	protected abstract AutomationPeer? GetLabeledByCore();

	protected abstract string? GetNameCore();

	protected abstract AutomationPeer? GetParentCore();

	protected abstract bool HasKeyboardFocusCore();

	protected abstract bool IsContentElementCore();

	protected abstract bool IsControlElementCore();

	protected abstract bool IsEnabledCore();

	protected abstract bool IsKeyboardFocusableCore();

	protected abstract void SetFocusCore();

	protected abstract bool ShowContextMenuCore();

	protected virtual AutomationControlType GetControlTypeOverrideCore()
	{
		return GetAutomationControlTypeCore();
	}

	protected virtual bool IsContentElementOverrideCore()
	{
		if (IsControlElement())
		{
			return IsContentElementCore();
		}
		return false;
	}

	protected virtual bool IsControlElementOverrideCore()
	{
		return IsControlElementCore();
	}

	protected virtual object? GetProviderCore(Type providerType)
	{
		if (!providerType.IsAssignableFrom(GetType()))
		{
			return null;
		}
		return this;
	}

	protected internal abstract bool TrySetParent(AutomationPeer? parent);

	protected void EnsureEnabled()
	{
		if (!IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
	}
}
