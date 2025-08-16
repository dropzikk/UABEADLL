using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Views;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics.ViewModels;

internal class LogicalTreeNode : TreeNode
{
	internal class LogicalTreeNodeCollection : TreeNodeCollection
	{
		private readonly ILogical _control;

		private IDisposable? _subscription;

		public LogicalTreeNodeCollection(TreeNode owner, ILogical control)
			: base(owner)
		{
			_control = control;
		}

		public override void Dispose()
		{
			base.Dispose();
			_subscription?.Dispose();
		}

		protected override void Initialize(AvaloniaList<TreeNode> nodes)
		{
			_subscription = _control.LogicalChildren.ForEachItem(delegate(int i, ILogical item)
			{
				nodes.Insert(i, new LogicalTreeNode((AvaloniaObject)item, base.Owner));
			}, delegate(int i, ILogical item)
			{
				nodes.RemoveAt(i);
			}, delegate
			{
				nodes.Clear();
			});
		}
	}

	internal class TopLevelGroupHostLogical : TreeNodeCollection
	{
		private readonly TopLevelGroup _group;

		private readonly CompositeDisposable _subscriptions = new CompositeDisposable(1);

		public TopLevelGroupHostLogical(TreeNode owner, TopLevelGroup host)
			: base(owner)
		{
			_group = host;
		}

		protected override void Initialize(AvaloniaList<TreeNode> nodes)
		{
			for (int i = 0; i < _group.Items.Count; i++)
			{
				TopLevel topLevel = _group.Items[i];
				if (!(topLevel is MainWindow))
				{
					nodes.Add(new LogicalTreeNode(topLevel, base.Owner));
				}
			}
			_group.Added += GroupOnAdded;
			_group.Removed += GroupOnRemoved;
			_subscriptions.Add(new Disposable.AnonymousDisposable(delegate
			{
				_group.Added -= GroupOnAdded;
				_group.Removed -= GroupOnRemoved;
			}));
			void GroupOnAdded(object? sender, TopLevel e)
			{
				if (!(e is MainWindow))
				{
					nodes.Add(new LogicalTreeNode(e, base.Owner));
				}
			}
			void GroupOnRemoved(object? sender, TopLevel e)
			{
				if (!(e is MainWindow))
				{
					nodes.Add(new LogicalTreeNode(e, base.Owner));
				}
			}
		}

		public override void Dispose()
		{
			_subscriptions?.Dispose();
			base.Dispose();
		}
	}

	public override TreeNodeCollection Children { get; }

	public LogicalTreeNode(AvaloniaObject avaloniaObject, TreeNode? parent)
		: base(avaloniaObject, parent)
	{
		Children = ((avaloniaObject is ILogical control) ? new LogicalTreeNodeCollection(this, control) : ((!(avaloniaObject is TopLevelGroup host)) ? TreeNodeCollection.Empty : new TopLevelGroupHostLogical(this, host)));
	}

	public static LogicalTreeNode[] Create(object control)
	{
		if (!(control is AvaloniaObject avaloniaObject))
		{
			return Array.Empty<LogicalTreeNode>();
		}
		return new LogicalTreeNode[1]
		{
			new LogicalTreeNode(avaloniaObject, null)
		};
	}
}
