using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Primitives;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class RadioButton : ToggleButton
{
	private class RadioButtonGroupManager
	{
		public static readonly RadioButtonGroupManager Default = new RadioButtonGroupManager();

		private static readonly ConditionalWeakTable<IRenderRoot, RadioButtonGroupManager> s_registeredVisualRoots = new ConditionalWeakTable<IRenderRoot, RadioButtonGroupManager>();

		private readonly Dictionary<string, List<WeakReference<RadioButton>>> s_registeredGroups = new Dictionary<string, List<WeakReference<RadioButton>>>();

		public static RadioButtonGroupManager GetOrCreateForRoot(IRenderRoot? root)
		{
			if (root == null)
			{
				return Default;
			}
			return s_registeredVisualRoots.GetValue(root, (IRenderRoot key) => new RadioButtonGroupManager());
		}

		public void Add(RadioButton radioButton)
		{
			lock (s_registeredGroups)
			{
				string groupName = radioButton.GroupName;
				if (!s_registeredGroups.TryGetValue(groupName, out List<WeakReference<RadioButton>> value))
				{
					value = new List<WeakReference<RadioButton>>();
					s_registeredGroups.Add(groupName, value);
				}
				value.Add(new WeakReference<RadioButton>(radioButton));
			}
		}

		public void Remove(RadioButton radioButton, string oldGroupName)
		{
			lock (s_registeredGroups)
			{
				if (string.IsNullOrEmpty(oldGroupName) || !s_registeredGroups.TryGetValue(oldGroupName, out List<WeakReference<RadioButton>> value))
				{
					return;
				}
				int num = 0;
				while (num < value.Count)
				{
					if (!value[num].TryGetTarget(out var target) || target == radioButton)
					{
						value.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
				if (value.Count == 0)
				{
					s_registeredGroups.Remove(oldGroupName);
				}
			}
		}

		public void SetChecked(RadioButton radioButton)
		{
			lock (s_registeredGroups)
			{
				string groupName = radioButton.GroupName;
				if (!s_registeredGroups.TryGetValue(groupName, out List<WeakReference<RadioButton>> value))
				{
					return;
				}
				int num = 0;
				while (num < value.Count)
				{
					if (!value[num].TryGetTarget(out var target))
					{
						value.RemoveAt(num);
						continue;
					}
					if (target != radioButton && target.IsChecked == true)
					{
						target.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
					}
					num++;
				}
				if (value.Count == 0)
				{
					s_registeredGroups.Remove(groupName);
				}
			}
		}
	}

	public static readonly StyledProperty<string?> GroupNameProperty = AvaloniaProperty.Register<RadioButton, string>("GroupName");

	private RadioButtonGroupManager? _groupManager;

	public string? GroupName
	{
		get
		{
			return GetValue(GroupNameProperty);
		}
		set
		{
			SetValue(GroupNameProperty, value);
		}
	}

	protected override void Toggle()
	{
		if (base.IsChecked != true)
		{
			SetCurrentValue(ToggleButton.IsCheckedProperty, true);
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		if (!string.IsNullOrEmpty(GroupName))
		{
			_groupManager?.Remove(this, GroupName);
			_groupManager = RadioButtonGroupManager.GetOrCreateForRoot(e.Root);
			_groupManager.Add(this);
		}
		base.OnAttachedToVisualTree(e);
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (!string.IsNullOrEmpty(GroupName))
		{
			_groupManager?.Remove(this, GroupName);
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new RadioButtonAutomationPeer(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ToggleButton.IsCheckedProperty)
		{
			IsCheckedChanged(change.GetNewValue<bool?>());
		}
		else if (change.Property == GroupNameProperty)
		{
			var (oldGroupName, newGroupName) = change.GetOldAndNewValue<string>();
			OnGroupNameChanged(oldGroupName, newGroupName);
		}
	}

	private void OnGroupNameChanged(string? oldGroupName, string? newGroupName)
	{
		if (!string.IsNullOrEmpty(oldGroupName))
		{
			_groupManager?.Remove(this, oldGroupName);
		}
		if (!string.IsNullOrEmpty(newGroupName))
		{
			if (_groupManager == null)
			{
				_groupManager = RadioButtonGroupManager.GetOrCreateForRoot(this.GetVisualRoot());
			}
			_groupManager.Add(this);
		}
	}

	private new void IsCheckedChanged(bool? value)
	{
		if (string.IsNullOrEmpty(GroupName))
		{
			Visual visualParent = this.GetVisualParent();
			if (value != true || visualParent == null)
			{
				return;
			}
			{
				foreach (RadioButton item in from x in visualParent.GetVisualChildren().OfType<RadioButton>()
					where x != this && string.IsNullOrEmpty(x.GroupName)
					select x)
				{
					if (item.IsChecked == true)
					{
						item.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
					}
				}
				return;
			}
		}
		if (value == true && _groupManager != null)
		{
			_groupManager.SetChecked(this);
		}
	}
}
