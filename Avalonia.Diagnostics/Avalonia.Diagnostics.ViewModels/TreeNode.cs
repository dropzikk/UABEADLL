using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics.ViewModels;

internal abstract class TreeNode : ViewModelBase, IDisposable
{
	private readonly IDisposable? _classesSubscription;

	private string _classes;

	private bool _isExpanded;

	private bool IsRoot
	{
		get
		{
			if (!(Visual is TopLevel) && !(Visual is ContextMenu))
			{
				return Visual is IPopupHost;
			}
			return true;
		}
	}

	public FontWeight FontWeight { get; }

	public abstract TreeNodeCollection Children { get; }

	public string Classes
	{
		get
		{
			return _classes;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _classes, value, "Classes");
		}
	}

	public string? ElementName { get; }

	public AvaloniaObject Visual { get; }

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

	public TreeNode? Parent { get; }

	public string Type { get; private set; }

	protected TreeNode(AvaloniaObject avaloniaObject, TreeNode? parent, string? customName = null)
	{
		TreeNode treeNode = this;
		_classes = string.Empty;
		Parent = parent;
		Type = customName ?? avaloniaObject.GetType().Name;
		Visual = avaloniaObject;
		FontWeight = (IsRoot ? FontWeight.Bold : FontWeight.Normal);
		Control control = avaloniaObject as Control;
		if (control == null)
		{
			return;
		}
		ElementName = control.Name;
		_classesSubscription = ((IObservable<object>)control.Classes.GetWeakCollectionChangedObservable()).StartWith((object)null).Subscribe(delegate
		{
			if (control.Classes.Count > 0)
			{
				treeNode.Classes = "(" + string.Join(" ", control.Classes) + ")";
			}
			else
			{
				treeNode.Classes = string.Empty;
			}
		});
	}

	public void Dispose()
	{
		_classesSubscription?.Dispose();
		Children.Dispose();
	}
}
