using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Views;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics.ViewModels;

internal class VisualTreeNode : TreeNode
{
	internal class VisualTreeNodeCollection : TreeNodeCollection
	{
		private struct PopupRoot
		{
			public Control Root { get; }

			public string? CustomName { get; }

			public PopupRoot(Control root, string? customName = null)
			{
				Root = root;
				CustomName = customName;
			}
		}

		private readonly Visual _control;

		private readonly CompositeDisposable _subscriptions = new CompositeDisposable(2);

		public VisualTreeNodeCollection(TreeNode owner, Visual control)
			: base(owner)
		{
			_control = control;
		}

		public override void Dispose()
		{
			_subscriptions.Dispose();
		}

		private static IObservable<PopupRoot?>? GetHostedPopupRootObservable(Visual visual)
		{
			if (!(visual is Popup popupHostProvider2))
			{
				if (visual is Control o)
				{
					return new IObservable<object>[5]
					{
						o.GetObservable(Control.ContextFlyoutProperty),
						o.GetObservable(Control.ContextMenuProperty),
						o.GetObservable(FlyoutBase.AttachedFlyoutProperty),
						o.GetObservable(ToolTipDiagnostics.ToolTipProperty),
						o.GetObservable(Button.FlyoutProperty)
					}.CombineLatest().Select(delegate(object[] items)
					{
						IPopupHostProvider popupHostProvider3 = items[0] as IPopupHostProvider;
						ContextMenu contextMenu = items[1] as ContextMenu;
						IPopupHostProvider popupHostProvider4 = items[2] as IPopupHostProvider;
						IPopupHostProvider popupHostProvider5 = items[3] as IPopupHostProvider;
						IPopupHostProvider popupHostProvider6 = items[4] as IPopupHostProvider;
						if (contextMenu != null)
						{
							return Observable.Return((PopupRoot?)new PopupRoot(contextMenu));
						}
						if (popupHostProvider3 != null)
						{
							return GetPopupHostObservable(popupHostProvider3, "ContextFlyout");
						}
						if (popupHostProvider4 != null)
						{
							return GetPopupHostObservable(popupHostProvider4, "AttachedFlyout");
						}
						if (popupHostProvider5 != null)
						{
							return GetPopupHostObservable(popupHostProvider5, "ToolTip");
						}
						return (popupHostProvider6 != null) ? GetPopupHostObservable(popupHostProvider6, "Flyout") : Observable.Return<PopupRoot?>(null);
					}).Switch();
				}
				return null;
			}
			return GetPopupHostObservable(popupHostProvider2);
			static IObservable<PopupRoot?> GetPopupHostObservable(IPopupHostProvider popupHostProvider, string? providerName = null)
			{
				return from popupHost in Observable.Create(delegate(IObserver<IPopupHost?> observer)
					{
						popupHostProvider.PopupHostChanged += Handler;
						return Disposable.Create(delegate
						{
							popupHostProvider.PopupHostChanged -= Handler;
						});
						void Handler(IPopupHost? args)
						{
							observer.OnNext(args);
						}
					}).StartWith<IPopupHost>(popupHostProvider.PopupHost)
					select (popupHost is Control control) ? new PopupRoot?(new PopupRoot(control, (providerName != null) ? (providerName + " (" + control.GetType().Name + ")") : null)) : ((PopupRoot?)null);
			}
		}

		protected override void Initialize(AvaloniaList<TreeNode> nodes)
		{
			_subscriptions.Clear();
			IObservable<PopupRoot?> hostedPopupRootObservable = GetHostedPopupRootObservable(_control);
			if (hostedPopupRootObservable != null)
			{
				VisualTreeNode childNode = null;
				_subscriptions.Add(hostedPopupRootObservable.Subscribe(delegate(PopupRoot? popupRoot)
				{
					if (popupRoot.HasValue)
					{
						childNode = new VisualTreeNode(popupRoot.Value.Root, base.Owner, popupRoot.Value.CustomName);
						nodes.Add(childNode);
					}
					else if (childNode != null)
					{
						nodes.Remove(childNode);
					}
				}));
			}
			_subscriptions.Add(_control.VisualChildren.ForEachItem(delegate(int i, Visual item)
			{
				nodes.Insert(i, new VisualTreeNode(item, base.Owner));
			}, delegate(int i, Visual item)
			{
				nodes.RemoveAt(i);
			}, delegate
			{
				nodes.Clear();
			}));
		}
	}

	internal class ApplicationHostVisuals : TreeNodeCollection
	{
		private readonly Avalonia.Diagnostics.Controls.Application _application;

		private CompositeDisposable _subscriptions = new CompositeDisposable(2);

		public ApplicationHostVisuals(TreeNode owner, Avalonia.Diagnostics.Controls.Application host)
			: base(owner)
		{
			_application = host;
		}

		protected override void Initialize(AvaloniaList<TreeNode> nodes)
		{
			if (_application.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: not null } singleViewApplicationLifetime)
			{
				nodes.Add(new VisualTreeNode(singleViewApplicationLifetime.MainView, base.Owner));
			}
			if (!(_application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime))
			{
				return;
			}
			for (int i = 0; i < classicDesktopStyleApplicationLifetime.Windows.Count; i++)
			{
				Window window = classicDesktopStyleApplicationLifetime.Windows[i];
				if (!(window is MainWindow))
				{
					nodes.Add(new VisualTreeNode(window, base.Owner));
				}
			}
			_subscriptions = new CompositeDisposable(2)
			{
				Window.WindowOpenedEvent.AddClassHandler(typeof(Window), delegate(object? s, RoutedEventArgs e)
				{
					if (!(s is MainWindow))
					{
						nodes.Add(new VisualTreeNode((AvaloniaObject)s, base.Owner));
					}
				}),
				Window.WindowClosedEvent.AddClassHandler(typeof(Window), delegate(object? s, RoutedEventArgs e)
				{
					if (!(s is MainWindow))
					{
						TreeNode treeNode = nodes.FirstOrDefault((TreeNode node) => node.Visual == s);
						if (treeNode != null)
						{
							nodes.Remove(treeNode);
						}
					}
				})
			};
		}

		public override void Dispose()
		{
			_subscriptions?.Dispose();
			base.Dispose();
		}
	}

	public bool IsInTemplate { get; }

	public override TreeNodeCollection Children { get; }

	public VisualTreeNode(AvaloniaObject avaloniaObject, TreeNode? parent, string? customName = null)
		: base(avaloniaObject, parent, customName)
	{
		Children = ((avaloniaObject is Visual control) ? new VisualTreeNodeCollection(this, control) : ((!(avaloniaObject is Avalonia.Diagnostics.Controls.Application host)) ? TreeNodeCollection.Empty : new ApplicationHostVisuals(this, host)));
		if (base.Visual is StyledElement styledElement)
		{
			IsInTemplate = styledElement.TemplatedParent != null;
		}
	}

	public static VisualTreeNode[] Create(object control)
	{
		if (!(control is AvaloniaObject avaloniaObject))
		{
			return Array.Empty<VisualTreeNode>();
		}
		return new VisualTreeNode[1]
		{
			new VisualTreeNode(avaloniaObject, null)
		};
	}
}
