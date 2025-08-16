using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Threading;
using Avalonia.Win32.Interop.Automation;

namespace Avalonia.Win32.Automation;

[ComVisible(true)]
[RequiresUnreferencedCode("Requires .NET COM interop")]
internal class AutomationNode : MarshalByRefObject, IRawElementProviderSimple, IRawElementProviderSimple2, IRawElementProviderFragment, Avalonia.Win32.Interop.Automation.IInvokeProvider, Avalonia.Win32.Interop.Automation.IExpandCollapseProvider, Avalonia.Win32.Interop.Automation.IRangeValueProvider, Avalonia.Win32.Interop.Automation.IScrollProvider, IScrollItemProvider, Avalonia.Win32.Interop.Automation.ISelectionProvider, Avalonia.Win32.Interop.Automation.ISelectionItemProvider, Avalonia.Win32.Interop.Automation.IToggleProvider, Avalonia.Win32.Interop.Automation.IValueProvider
{
	private static Dictionary<AutomationProperty, UiaPropertyId> s_propertyMap = new Dictionary<AutomationProperty, UiaPropertyId>
	{
		{
			AutomationElementIdentifiers.BoundingRectangleProperty,
			UiaPropertyId.BoundingRectangle
		},
		{
			AutomationElementIdentifiers.ClassNameProperty,
			UiaPropertyId.ClassName
		},
		{
			AutomationElementIdentifiers.NameProperty,
			UiaPropertyId.Name
		},
		{
			ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
			UiaPropertyId.ExpandCollapseExpandCollapseState
		},
		{
			RangeValuePatternIdentifiers.IsReadOnlyProperty,
			UiaPropertyId.RangeValueIsReadOnly
		},
		{
			RangeValuePatternIdentifiers.MaximumProperty,
			UiaPropertyId.RangeValueMaximum
		},
		{
			RangeValuePatternIdentifiers.MinimumProperty,
			UiaPropertyId.RangeValueMinimum
		},
		{
			RangeValuePatternIdentifiers.ValueProperty,
			UiaPropertyId.RangeValueValue
		},
		{
			ScrollPatternIdentifiers.HorizontallyScrollableProperty,
			UiaPropertyId.ScrollHorizontallyScrollable
		},
		{
			ScrollPatternIdentifiers.HorizontalScrollPercentProperty,
			UiaPropertyId.ScrollHorizontalScrollPercent
		},
		{
			ScrollPatternIdentifiers.HorizontalViewSizeProperty,
			UiaPropertyId.ScrollHorizontalViewSize
		},
		{
			ScrollPatternIdentifiers.VerticallyScrollableProperty,
			UiaPropertyId.ScrollVerticallyScrollable
		},
		{
			ScrollPatternIdentifiers.VerticalScrollPercentProperty,
			UiaPropertyId.ScrollVerticalScrollPercent
		},
		{
			ScrollPatternIdentifiers.VerticalViewSizeProperty,
			UiaPropertyId.ScrollVerticalViewSize
		},
		{
			SelectionPatternIdentifiers.CanSelectMultipleProperty,
			UiaPropertyId.SelectionCanSelectMultiple
		},
		{
			SelectionPatternIdentifiers.IsSelectionRequiredProperty,
			UiaPropertyId.SelectionIsSelectionRequired
		},
		{
			SelectionPatternIdentifiers.SelectionProperty,
			UiaPropertyId.SelectionSelection
		},
		{
			SelectionItemPatternIdentifiers.IsSelectedProperty,
			UiaPropertyId.SelectionItemIsSelected
		},
		{
			SelectionItemPatternIdentifiers.SelectionContainerProperty,
			UiaPropertyId.SelectionItemSelectionContainer
		}
	};

	private static ConditionalWeakTable<AutomationPeer, AutomationNode> s_nodes = new ConditionalWeakTable<AutomationPeer, AutomationNode>();

	private readonly int[] _runtimeId;

	public AutomationPeer Peer { get; protected set; }

	public Rect BoundingRectangle => InvokeSync(() => (GetRoot() is RootAutomationNode rootAutomationNode) ? rootAutomationNode.ToScreen(Peer.GetBoundingRectangle()) : default(Rect));

	public virtual IRawElementProviderFragmentRoot? FragmentRoot => InvokeSync(() => GetRoot()) as IRawElementProviderFragmentRoot;

	public virtual IRawElementProviderSimple? HostRawElementProvider => null;

	public ProviderOptions ProviderOptions => ProviderOptions.ServerSideProvider;

	ExpandCollapseState Avalonia.Win32.Interop.Automation.IExpandCollapseProvider.ExpandCollapseState => InvokeSync((Avalonia.Automation.Provider.IExpandCollapseProvider x) => x.ExpandCollapseState);

	double Avalonia.Win32.Interop.Automation.IRangeValueProvider.Value => InvokeSync((Avalonia.Automation.Provider.IRangeValueProvider x) => x.Value);

	bool Avalonia.Win32.Interop.Automation.IRangeValueProvider.IsReadOnly => InvokeSync((Avalonia.Automation.Provider.IRangeValueProvider x) => x.IsReadOnly);

	double Avalonia.Win32.Interop.Automation.IRangeValueProvider.Maximum => InvokeSync((Avalonia.Automation.Provider.IRangeValueProvider x) => x.Maximum);

	double Avalonia.Win32.Interop.Automation.IRangeValueProvider.Minimum => InvokeSync((Avalonia.Automation.Provider.IRangeValueProvider x) => x.Minimum);

	double Avalonia.Win32.Interop.Automation.IRangeValueProvider.LargeChange => 1.0;

	double Avalonia.Win32.Interop.Automation.IRangeValueProvider.SmallChange => 1.0;

	bool Avalonia.Win32.Interop.Automation.IScrollProvider.HorizontallyScrollable => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.HorizontallyScrollable);

	double Avalonia.Win32.Interop.Automation.IScrollProvider.HorizontalScrollPercent => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.HorizontalScrollPercent);

	double Avalonia.Win32.Interop.Automation.IScrollProvider.HorizontalViewSize => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.HorizontalViewSize);

	bool Avalonia.Win32.Interop.Automation.IScrollProvider.VerticallyScrollable => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.VerticallyScrollable);

	double Avalonia.Win32.Interop.Automation.IScrollProvider.VerticalScrollPercent => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.VerticalScrollPercent);

	double Avalonia.Win32.Interop.Automation.IScrollProvider.VerticalViewSize => InvokeSync((Avalonia.Automation.Provider.IScrollProvider x) => x.VerticalViewSize);

	bool Avalonia.Win32.Interop.Automation.ISelectionProvider.CanSelectMultiple => InvokeSync((Avalonia.Automation.Provider.ISelectionProvider x) => x.CanSelectMultiple);

	bool Avalonia.Win32.Interop.Automation.ISelectionProvider.IsSelectionRequired => InvokeSync((Avalonia.Automation.Provider.ISelectionProvider x) => x.IsSelectionRequired);

	bool Avalonia.Win32.Interop.Automation.ISelectionItemProvider.IsSelected => InvokeSync((Avalonia.Automation.Provider.ISelectionItemProvider x) => x.IsSelected);

	IRawElementProviderSimple? Avalonia.Win32.Interop.Automation.ISelectionItemProvider.SelectionContainer => GetOrCreate(InvokeSync((Avalonia.Automation.Provider.ISelectionItemProvider x) => x.SelectionContainer) as AutomationPeer);

	ToggleState Avalonia.Win32.Interop.Automation.IToggleProvider.ToggleState => InvokeSync((Avalonia.Automation.Provider.IToggleProvider x) => x.ToggleState);

	bool Avalonia.Win32.Interop.Automation.IValueProvider.IsReadOnly => InvokeSync((Avalonia.Automation.Provider.IValueProvider x) => x.IsReadOnly);

	string? Avalonia.Win32.Interop.Automation.IValueProvider.Value => InvokeSync((Avalonia.Automation.Provider.IValueProvider x) => x.Value);

	public AutomationNode(AutomationPeer peer)
	{
		_runtimeId = new int[2]
		{
			3,
			GetHashCode()
		};
		Peer = peer;
		s_nodes.Add(peer, this);
		peer.ChildrenChanged += Peer_ChildrenChanged;
		peer.PropertyChanged += Peer_PropertyChanged;
	}

	private void Peer_ChildrenChanged(object? sender, EventArgs e)
	{
		ChildrenChanged();
	}

	private void Peer_PropertyChanged(object? sender, AutomationPropertyChangedEventArgs e)
	{
		if (s_propertyMap.TryGetValue(e.Property, out var value))
		{
			UiaCoreProviderApi.UiaRaiseAutomationPropertyChangedEvent(this, (int)value, e.OldValue as IConvertible, e.NewValue as IConvertible);
		}
	}

	public void ChildrenChanged()
	{
		UiaCoreProviderApi.UiaRaiseStructureChangedEvent(this, StructureChangeType.ChildrenInvalidated, null, 0);
	}

	[return: MarshalAs(UnmanagedType.IUnknown)]
	public virtual object? GetPatternProvider(int patternId)
	{
		return (UiaPatternId)patternId switch
		{
			UiaPatternId.ExpandCollapse => ThisIfPeerImplementsProvider<Avalonia.Win32.Interop.Automation.IExpandCollapseProvider>(), 
			UiaPatternId.Invoke => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.IInvokeProvider>(), 
			UiaPatternId.RangeValue => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.IRangeValueProvider>(), 
			UiaPatternId.Scroll => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.IScrollProvider>(), 
			UiaPatternId.ScrollItem => this, 
			UiaPatternId.Selection => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.ISelectionProvider>(), 
			UiaPatternId.SelectionItem => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.ISelectionItemProvider>(), 
			UiaPatternId.Toggle => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.IToggleProvider>(), 
			UiaPatternId.Value => ThisIfPeerImplementsProvider<Avalonia.Automation.Provider.IValueProvider>(), 
			_ => null, 
		};
		AutomationNode? ThisIfPeerImplementsProvider<T>()
		{
			if (Peer.GetProvider<T>() == null)
			{
				return null;
			}
			return this;
		}
	}

	public virtual object? GetPropertyValue(int propertyId)
	{
		return (UiaPropertyId)propertyId switch
		{
			UiaPropertyId.AcceleratorKey => InvokeSync(() => Peer.GetAcceleratorKey()), 
			UiaPropertyId.AccessKey => InvokeSync(() => Peer.GetAccessKey()), 
			UiaPropertyId.AutomationId => InvokeSync(() => Peer.GetAutomationId()), 
			UiaPropertyId.ClassName => InvokeSync(() => Peer.GetClassName()), 
			UiaPropertyId.ClickablePoint => new double[2]
			{
				BoundingRectangle.Center.X,
				BoundingRectangle.Center.Y
			}, 
			UiaPropertyId.ControlType => InvokeSync(() => ToUiaControlType(Peer.GetAutomationControlType())), 
			UiaPropertyId.Culture => CultureInfo.CurrentCulture.LCID, 
			UiaPropertyId.FrameworkId => "Avalonia", 
			UiaPropertyId.HasKeyboardFocus => InvokeSync(() => Peer.HasKeyboardFocus()), 
			UiaPropertyId.IsContentElement => InvokeSync(() => Peer.IsContentElement()), 
			UiaPropertyId.IsControlElement => InvokeSync(() => Peer.IsControlElement()), 
			UiaPropertyId.IsEnabled => InvokeSync(() => Peer.IsEnabled()), 
			UiaPropertyId.IsKeyboardFocusable => InvokeSync(() => Peer.IsKeyboardFocusable()), 
			UiaPropertyId.LocalizedControlType => InvokeSync(() => Peer.GetLocalizedControlType()), 
			UiaPropertyId.Name => InvokeSync(() => Peer.GetName()), 
			UiaPropertyId.ProcessId => Process.GetCurrentProcess().Id, 
			UiaPropertyId.RuntimeId => _runtimeId, 
			_ => null, 
		};
	}

	public int[]? GetRuntimeId()
	{
		return _runtimeId;
	}

	public virtual IRawElementProviderFragment? Navigate(NavigateDirection direction)
	{
		return InvokeSync(() => direction switch
		{
			NavigateDirection.Parent => GetOrCreate(Peer.GetParent()), 
			NavigateDirection.NextSibling => GetSibling(1), 
			NavigateDirection.PreviousSibling => GetSibling(-1), 
			NavigateDirection.FirstChild => GetOrCreate(Peer.GetChildren().FirstOrDefault()), 
			NavigateDirection.LastChild => GetOrCreate(Peer.GetChildren().LastOrDefault()), 
			_ => null, 
		});
		AutomationNode? GetSibling(int direction)
		{
			IReadOnlyList<AutomationPeer> readOnlyList = Peer.GetParent()?.GetChildren();
			for (int i = 0; i < (readOnlyList?.Count ?? 0); i++)
			{
				if (readOnlyList[i] == Peer)
				{
					int num = i + direction;
					if (num >= 0 && num < readOnlyList.Count)
					{
						return GetOrCreate(readOnlyList[num]);
					}
				}
			}
			return null;
		}
	}

	public void SetFocus()
	{
		InvokeSync(delegate
		{
			Peer.SetFocus();
		});
	}

	[return: NotNullIfNotNull("peer")]
	public static AutomationNode? GetOrCreate(AutomationPeer? peer)
	{
		if (peer != null)
		{
			return s_nodes.GetValue(peer, Create);
		}
		return null;
	}

	public static void Release(AutomationPeer peer)
	{
		s_nodes.Remove(peer);
	}

	IRawElementProviderSimple[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots()
	{
		return null;
	}

	void IRawElementProviderSimple2.ShowContextMenu()
	{
		InvokeSync(() => Peer.ShowContextMenu());
	}

	void Avalonia.Win32.Interop.Automation.IInvokeProvider.Invoke()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IInvokeProvider x)
		{
			x.Invoke();
		});
	}

	protected void InvokeSync(Action action)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			action();
		}
		else
		{
			Dispatcher.UIThread.InvokeAsync(action).Wait();
		}
	}

	protected T InvokeSync<T>(Func<T> func)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			return func();
		}
		return Dispatcher.UIThread.InvokeAsync(func).Result;
	}

	protected void InvokeSync<TInterface>(Action<TInterface> action)
	{
		TInterface i = Peer.GetProvider<TInterface>();
		if (i != null)
		{
			try
			{
				InvokeSync(delegate
				{
					action(i);
				});
				return;
			}
			catch (AggregateException ex) when (ex.InnerException is ElementNotEnabledException)
			{
				throw new COMException(ex.Message, -2147220992);
			}
		}
		throw new NotSupportedException();
	}

	protected TResult InvokeSync<TInterface, TResult>(Func<TInterface, TResult> func)
	{
		TInterface i = Peer.GetProvider<TInterface>();
		if (i != null)
		{
			try
			{
				return InvokeSync(() => func(i));
			}
			catch (AggregateException ex) when (ex.InnerException is ElementNotEnabledException)
			{
				throw new COMException(ex.Message, -2147220992);
			}
		}
		throw new NotSupportedException();
	}

	private AutomationNode? GetRoot()
	{
		Dispatcher.UIThread.VerifyAccess();
		AutomationPeer automationPeer = Peer;
		AutomationPeer parent = automationPeer.GetParent();
		while (automationPeer.GetProvider<IRootProvider>() == null && parent != null)
		{
			automationPeer = parent;
			parent = automationPeer.GetParent();
		}
		if (automationPeer == null)
		{
			return null;
		}
		return GetOrCreate(automationPeer);
	}

	private static AutomationNode Create(AutomationPeer peer)
	{
		if (peer.GetProvider<IRootProvider>() == null)
		{
			return new AutomationNode(peer);
		}
		return new RootAutomationNode(peer);
	}

	private static UiaControlTypeId ToUiaControlType(AutomationControlType role)
	{
		return role switch
		{
			AutomationControlType.None => UiaControlTypeId.Group, 
			AutomationControlType.Button => UiaControlTypeId.Button, 
			AutomationControlType.Calendar => UiaControlTypeId.Calendar, 
			AutomationControlType.CheckBox => UiaControlTypeId.CheckBox, 
			AutomationControlType.ComboBox => UiaControlTypeId.ComboBox, 
			AutomationControlType.ComboBoxItem => UiaControlTypeId.ListItem, 
			AutomationControlType.Edit => UiaControlTypeId.Edit, 
			AutomationControlType.Hyperlink => UiaControlTypeId.Hyperlink, 
			AutomationControlType.Image => UiaControlTypeId.Image, 
			AutomationControlType.ListItem => UiaControlTypeId.ListItem, 
			AutomationControlType.List => UiaControlTypeId.List, 
			AutomationControlType.Menu => UiaControlTypeId.Menu, 
			AutomationControlType.MenuBar => UiaControlTypeId.MenuBar, 
			AutomationControlType.MenuItem => UiaControlTypeId.MenuItem, 
			AutomationControlType.ProgressBar => UiaControlTypeId.ProgressBar, 
			AutomationControlType.RadioButton => UiaControlTypeId.RadioButton, 
			AutomationControlType.ScrollBar => UiaControlTypeId.ScrollBar, 
			AutomationControlType.Slider => UiaControlTypeId.Slider, 
			AutomationControlType.Spinner => UiaControlTypeId.Spinner, 
			AutomationControlType.StatusBar => UiaControlTypeId.StatusBar, 
			AutomationControlType.Tab => UiaControlTypeId.Tab, 
			AutomationControlType.TabItem => UiaControlTypeId.TabItem, 
			AutomationControlType.Text => UiaControlTypeId.Text, 
			AutomationControlType.ToolBar => UiaControlTypeId.ToolBar, 
			AutomationControlType.ToolTip => UiaControlTypeId.ToolTip, 
			AutomationControlType.Tree => UiaControlTypeId.Tree, 
			AutomationControlType.TreeItem => UiaControlTypeId.TreeItem, 
			AutomationControlType.Custom => UiaControlTypeId.Custom, 
			AutomationControlType.Group => UiaControlTypeId.Group, 
			AutomationControlType.Thumb => UiaControlTypeId.Thumb, 
			AutomationControlType.DataGrid => UiaControlTypeId.DataGrid, 
			AutomationControlType.DataItem => UiaControlTypeId.DataItem, 
			AutomationControlType.Document => UiaControlTypeId.Document, 
			AutomationControlType.SplitButton => UiaControlTypeId.SplitButton, 
			AutomationControlType.Window => UiaControlTypeId.Window, 
			AutomationControlType.Pane => UiaControlTypeId.Pane, 
			AutomationControlType.Header => UiaControlTypeId.Header, 
			AutomationControlType.HeaderItem => UiaControlTypeId.HeaderItem, 
			AutomationControlType.Table => UiaControlTypeId.Table, 
			AutomationControlType.TitleBar => UiaControlTypeId.TitleBar, 
			AutomationControlType.Separator => UiaControlTypeId.Separator, 
			_ => UiaControlTypeId.Custom, 
		};
	}

	void Avalonia.Win32.Interop.Automation.IExpandCollapseProvider.Expand()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IExpandCollapseProvider x)
		{
			x.Expand();
		});
	}

	void Avalonia.Win32.Interop.Automation.IExpandCollapseProvider.Collapse()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IExpandCollapseProvider x)
		{
			x.Collapse();
		});
	}

	public void SetValue(double value)
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IRangeValueProvider x)
		{
			x.SetValue(value);
		});
	}

	void Avalonia.Win32.Interop.Automation.IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IScrollProvider x)
		{
			x.Scroll(horizontalAmount, verticalAmount);
		});
	}

	void Avalonia.Win32.Interop.Automation.IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IScrollProvider x)
		{
			x.SetScrollPercent(horizontalPercent, verticalPercent);
		});
	}

	void IScrollItemProvider.ScrollIntoView()
	{
		InvokeSync(delegate
		{
			Peer.BringIntoView();
		});
	}

	IRawElementProviderSimple[] Avalonia.Win32.Interop.Automation.ISelectionProvider.GetSelection()
	{
		return ((IEnumerable<AutomationPeer>)InvokeSync((Avalonia.Automation.Provider.ISelectionProvider x) => x.GetSelection())).Select((Func<AutomationPeer, IRawElementProviderSimple>)((AutomationPeer x) => GetOrCreate(x))).ToArray();
	}

	void Avalonia.Win32.Interop.Automation.ISelectionItemProvider.AddToSelection()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.ISelectionItemProvider x)
		{
			x.AddToSelection();
		});
	}

	void Avalonia.Win32.Interop.Automation.ISelectionItemProvider.RemoveFromSelection()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.ISelectionItemProvider x)
		{
			x.RemoveFromSelection();
		});
	}

	void Avalonia.Win32.Interop.Automation.ISelectionItemProvider.Select()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.ISelectionItemProvider x)
		{
			x.Select();
		});
	}

	void Avalonia.Win32.Interop.Automation.IToggleProvider.Toggle()
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IToggleProvider x)
		{
			x.Toggle();
		});
	}

	void Avalonia.Win32.Interop.Automation.IValueProvider.SetValue([MarshalAs(UnmanagedType.LPWStr)] string? value)
	{
		InvokeSync(delegate(Avalonia.Automation.Provider.IValueProvider x)
		{
			x.SetValue(value);
		});
	}
}
