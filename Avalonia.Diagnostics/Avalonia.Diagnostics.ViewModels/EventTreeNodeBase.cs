using Avalonia.Collections;

namespace Avalonia.Diagnostics.ViewModels;

internal abstract class EventTreeNodeBase : ViewModelBase
{
	internal bool _updateChildren = true;

	internal bool _updateParent = true;

	private bool _isExpanded;

	private bool? _isEnabled = false;

	private bool _isVisible;

	public IAvaloniaReadOnlyList<EventTreeNodeBase>? Children { get; protected set; }

	public bool IsExpanded
	{
		get
		{
			return _isExpanded;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isExpanded, value, "IsExpanded");
		}
	}

	public virtual bool? IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isEnabled, value, "IsEnabled");
		}
	}

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isVisible, value, "IsVisible");
		}
	}

	public EventTreeNodeBase? Parent { get; }

	public string Text { get; }

	protected EventTreeNodeBase(EventTreeNodeBase? parent, string text)
	{
		Parent = parent;
		Text = text;
		IsVisible = true;
	}

	internal void UpdateChecked()
	{
		IsEnabled = GetValue();
		bool? GetValue()
		{
			if (Children == null)
			{
				return false;
			}
			bool? flag = false;
			for (int i = 0; i < Children.Count; i++)
			{
				if (i == 0)
				{
					flag = Children[i].IsEnabled;
				}
				else if (flag != Children[i].IsEnabled)
				{
					flag = null;
					break;
				}
			}
			return flag;
		}
	}
}
