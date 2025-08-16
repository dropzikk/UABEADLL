using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Diagnostics.Models;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.ViewModels;

internal class EventsPageViewModel : ViewModelBase
{
	private static readonly HashSet<RoutedEvent> s_defaultEvents = new HashSet<RoutedEvent>
	{
		Button.ClickEvent,
		InputElement.KeyDownEvent,
		InputElement.KeyUpEvent,
		InputElement.TextInputEvent,
		InputElement.PointerReleasedEvent,
		InputElement.PointerPressedEvent
	};

	private readonly MainViewModel _mainViewModel;

	private FiredEvent? _selectedEvent;

	private EventTreeNodeBase? _selectedNode;

	public string Name => "Events";

	public EventTreeNodeBase[] Nodes { get; }

	public ObservableCollection<FiredEvent> RecordedEvents { get; } = new ObservableCollection<FiredEvent>();

	public FiredEvent? SelectedEvent
	{
		get
		{
			return _selectedEvent;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedEvent, value, "SelectedEvent");
		}
	}

	public EventTreeNodeBase? SelectedNode
	{
		get
		{
			return _selectedNode;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedNode, value, "SelectedNode");
		}
	}

	public FilterViewModel EventsFilter { get; }

	public EventsPageViewModel(MainViewModel mainViewModel)
	{
		_mainViewModel = mainViewModel;
		EventTreeNodeBase[] array = (from e in RoutedEventRegistry.Instance.GetAllRegistered()
			group e by e.OwnerType into e
			orderby e.Key.Name
			select e into g
			select new EventOwnerTreeNode(g.Key, g, this)).ToArray();
		Nodes = array;
		EventsFilter = new FilterViewModel();
		EventsFilter.RefreshFilter += delegate
		{
			UpdateEventFilters();
		};
		EnableDefault();
	}

	public void Clear()
	{
		RecordedEvents.Clear();
	}

	public void DisableAll()
	{
		EvaluateNodeEnabled((EventTreeNode _) => false);
	}

	public void EnableDefault()
	{
		EvaluateNodeEnabled((EventTreeNode node) => s_defaultEvents.Contains(node.Event));
	}

	public void RequestTreeNavigateTo(EventChainLink navTarget)
	{
		if (navTarget.Handler is Control control)
		{
			_mainViewModel.RequestTreeNavigateTo(control, isVisualTree: true);
		}
	}

	public void SelectEventByType(RoutedEvent evt)
	{
		EventTreeNodeBase[] nodes = Nodes;
		for (int i = 0; i < nodes.Length; i++)
		{
			EventTreeNodeBase eventTreeNodeBase = FindNode(nodes[i], evt);
			if (eventTreeNodeBase != null && eventTreeNodeBase.IsVisible)
			{
				SelectedNode = eventTreeNodeBase;
				break;
			}
		}
		static EventTreeNodeBase? FindNode(EventTreeNodeBase node, RoutedEvent eventType)
		{
			if (node is EventTreeNode eventTreeNode && eventTreeNode.Event == eventType)
			{
				return node;
			}
			if (node.Children != null)
			{
				foreach (EventTreeNodeBase child in node.Children)
				{
					EventTreeNodeBase eventTreeNodeBase2 = FindNode(child, eventType);
					if (eventTreeNodeBase2 != null)
					{
						return eventTreeNodeBase2;
					}
				}
			}
			return null;
		}
	}

	private void EvaluateNodeEnabled(Func<EventTreeNode, bool> eval)
	{
		EventTreeNodeBase[] nodes = Nodes;
		for (int i = 0; i < nodes.Length; i++)
		{
			ProcessNode(nodes[i]);
		}
		void ProcessNode(EventTreeNodeBase node)
		{
			if (node is EventTreeNode arg)
			{
				node.IsEnabled = eval(arg);
			}
			if (node.Children != null)
			{
				foreach (EventTreeNodeBase child in node.Children)
				{
					ProcessNode(child);
				}
			}
		}
	}

	private void UpdateEventFilters()
	{
		EventTreeNodeBase[] nodes = Nodes;
		foreach (EventTreeNodeBase node2 in nodes)
		{
			FilterNode(node2, isParentVisible: false);
		}
		bool FilterNode(EventTreeNodeBase node, bool isParentVisible)
		{
			bool flag = EventsFilter.Filter(node.Text);
			bool flag2 = false;
			if (node.Children != null)
			{
				foreach (EventTreeNodeBase child in node.Children)
				{
					flag2 |= FilterNode(child, flag);
				}
			}
			node.IsVisible = flag2 || flag || isParentVisible;
			return node.IsVisible;
		}
	}
}
