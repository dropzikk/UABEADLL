using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Avalonia.Diagnostics.ViewModels;

internal class TreePageViewModel : ViewModelBase, IDisposable
{
	private TreeNode? _selectedNode;

	private ControlDetailsViewModel? _details;

	public MainViewModel MainView { get; }

	public FilterViewModel PropertiesFilter { get; }

	public FilterViewModel SettersFilter { get; }

	public TreeNode[] Nodes { get; protected set; }

	public TreeNode? SelectedNode
	{
		get
		{
			return _selectedNode;
		}
		set
		{
			if (RaiseAndSetIfChanged(ref _selectedNode, value, "SelectedNode"))
			{
				Details = ((value != null) ? new ControlDetailsViewModel(this, value.Visual) : null);
				Details?.UpdatePropertiesView(MainView.ShowImplementedInterfaces);
				Details?.UpdateStyleFilters();
			}
		}
	}

	public ControlDetailsViewModel? Details
	{
		get
		{
			return _details;
		}
		private set
		{
			ControlDetailsViewModel details = _details;
			if (RaiseAndSetIfChanged(ref _details, value, "Details"))
			{
				details?.Dispose();
			}
		}
	}

	public event EventHandler<string>? ClipboardCopyRequested;

	public TreePageViewModel(MainViewModel mainView, TreeNode[] nodes)
	{
		MainView = mainView;
		Nodes = nodes;
		PropertiesFilter = new FilterViewModel();
		PropertiesFilter.RefreshFilter += delegate
		{
			Details?.PropertiesView?.Refresh();
		};
		SettersFilter = new FilterViewModel();
		SettersFilter.RefreshFilter += delegate
		{
			Details?.UpdateStyleFilters();
		};
	}

	public void Dispose()
	{
		TreeNode[] nodes = Nodes;
		for (int i = 0; i < nodes.Length; i++)
		{
			nodes[i].Dispose();
		}
		_details?.Dispose();
	}

	public TreeNode? FindNode(Control control)
	{
		TreeNode[] nodes = Nodes;
		foreach (TreeNode node in nodes)
		{
			TreeNode treeNode = FindNode(node, control);
			if (treeNode != null)
			{
				return treeNode;
			}
		}
		return null;
	}

	public void SelectControl(Control control)
	{
		TreeNode treeNode = null;
		Control control2 = control;
		while (treeNode == null && control2 != null)
		{
			treeNode = FindNode(control2);
			if (treeNode == null)
			{
				control2 = control2.GetVisualParent<Control>();
			}
		}
		if (treeNode != null)
		{
			SelectedNode = treeNode;
			ExpandNode(treeNode.Parent);
		}
	}

	public void CopySelector()
	{
		if (SelectedNode?.Visual is Visual visual)
		{
			string visualSelector = GetVisualSelector(visual);
			this.ClipboardCopyRequested?.Invoke(this, visualSelector);
		}
	}

	public void CopySelectorFromTemplateParent()
	{
		List<string> list = new List<string>();
		for (Visual visual = SelectedNode?.Visual as Visual; visual != null; visual = visual.TemplatedParent as Visual)
		{
			list.Add(GetVisualSelector(visual));
		}
		if (list.Any())
		{
			list.Reverse();
			string e = string.Join(" /template/ ", list);
			this.ClipboardCopyRequested?.Invoke(this, e);
		}
	}

	public void ExpandRecursively()
	{
		TreeNode selectedNode = SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		ExpandNode(selectedNode);
		Stack<TreeNode> stack = new Stack<TreeNode>();
		stack.Push(selectedNode);
		while (stack.Count > 0)
		{
			TreeNode treeNode = stack.Pop();
			treeNode.IsExpanded = true;
			foreach (TreeNode child in treeNode.Children)
			{
				stack.Push(child);
			}
		}
	}

	public void CollapseChildren()
	{
		TreeNode selectedNode = SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		Stack<TreeNode> stack = new Stack<TreeNode>();
		stack.Push(selectedNode);
		while (stack.Count > 0)
		{
			TreeNode treeNode = stack.Pop();
			treeNode.IsExpanded = false;
			foreach (TreeNode child in treeNode.Children)
			{
				stack.Push(child);
			}
		}
	}

	public void CaptureNodeScreenshot()
	{
		MainView.Shot(null);
	}

	public void BringIntoView()
	{
		(SelectedNode?.Visual as Control)?.BringIntoView();
	}

	public void Focus()
	{
		(SelectedNode?.Visual as Control)?.Focus();
	}

	private static string GetVisualSelector(Visual visual)
	{
		string value = (string.IsNullOrEmpty(visual.Name) ? "" : ("#" + visual.Name));
		string value2 = string.Concat(from c in visual.Classes
			where !c.StartsWith(":")
			select "." + c);
		Type styleKey = StyledElement.GetStyleKey(visual);
		return $"{styleKey}{value}{value2}";
	}

	private void ExpandNode(TreeNode? node)
	{
		if (node != null)
		{
			node.IsExpanded = true;
			ExpandNode(node.Parent);
		}
	}

	private TreeNode? FindNode(TreeNode node, Control control)
	{
		if (node.Visual == control)
		{
			return node;
		}
		foreach (TreeNode child in node.Children)
		{
			TreeNode treeNode = FindNode(child, control);
			if (treeNode != null)
			{
				return treeNode;
			}
		}
		return null;
	}

	internal void UpdatePropertiesView()
	{
		Details?.UpdatePropertiesView(MainView?.ShowImplementedInterfaces ?? true);
	}
}
