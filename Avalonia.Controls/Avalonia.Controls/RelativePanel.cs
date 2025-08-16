using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Layout;

namespace Avalonia.Controls;

public class RelativePanel : Panel
{
	private class GraphNode
	{
		public bool Measured { get; set; }

		public Layoutable Element { get; }

		private bool HorizontalOffsetFlag { get; set; }

		private bool VerticalOffsetFlag { get; set; }

		private Size BoundingSize { get; set; }

		public Size OriginDesiredSize { get; set; }

		public double Left { get; set; } = double.NaN;

		public double Top { get; set; } = double.NaN;

		public double Right { get; set; } = double.NaN;

		public double Bottom { get; set; } = double.NaN;

		public HashSet<GraphNode> OutgoingNodes { get; }

		public GraphNode? AlignLeftWithNode { get; set; }

		public GraphNode? AlignTopWithNode { get; set; }

		public GraphNode? AlignRightWithNode { get; set; }

		public GraphNode? AlignBottomWithNode { get; set; }

		public GraphNode? LeftOfNode { get; set; }

		public GraphNode? AboveNode { get; set; }

		public GraphNode? RightOfNode { get; set; }

		public GraphNode? BelowNode { get; set; }

		public GraphNode? AlignHorizontalCenterWith { get; set; }

		public GraphNode? AlignVerticalCenterWith { get; set; }

		public GraphNode(Layoutable element)
		{
			OutgoingNodes = new HashSet<GraphNode>();
			Element = element;
		}

		public void Arrange(Size arrangeSize)
		{
			Element.Arrange(new Rect(Left, Top, Math.Max(arrangeSize.Width - Left - Right, 0.0), Math.Max(arrangeSize.Height - Top - Bottom, 0.0)));
		}

		public void Reset(bool clearPos)
		{
			if (clearPos)
			{
				Left = double.NaN;
				Top = double.NaN;
				Right = double.NaN;
				Bottom = double.NaN;
			}
			Measured = false;
		}

		public Size GetBoundingSize()
		{
			if (Left < 0.0 || Top < 0.0)
			{
				return default(Size);
			}
			if (Measured)
			{
				return BoundingSize;
			}
			if (!OutgoingNodes.Any())
			{
				BoundingSize = Element.DesiredSize;
				Measured = true;
			}
			else
			{
				BoundingSize = GetBoundingSize(this, Element.DesiredSize, OutgoingNodes);
				Measured = true;
			}
			return BoundingSize;
		}

		private static Size GetBoundingSize(GraphNode prevNode, Size prevSize, IEnumerable<GraphNode> nodes)
		{
			foreach (GraphNode node in nodes)
			{
				if (node.Measured || !node.OutgoingNodes.Any())
				{
					if ((prevNode.LeftOfNode != null && prevNode.LeftOfNode == node) || (prevNode.RightOfNode != null && prevNode.RightOfNode == node))
					{
						prevSize = prevSize.WithWidth(prevSize.Width + node.BoundingSize.Width);
						if (GetAlignHorizontalCenterWithPanel(node.Element) || node.HorizontalOffsetFlag)
						{
							prevSize = prevSize.WithWidth(prevSize.Width + prevNode.OriginDesiredSize.Width);
							prevNode.HorizontalOffsetFlag = true;
						}
						if (node.VerticalOffsetFlag)
						{
							prevNode.VerticalOffsetFlag = true;
						}
					}
					if ((prevNode.AboveNode != null && prevNode.AboveNode == node) || (prevNode.BelowNode != null && prevNode.BelowNode == node))
					{
						prevSize = prevSize.WithHeight(prevSize.Height + node.BoundingSize.Height);
						if (GetAlignVerticalCenterWithPanel(node.Element) || node.VerticalOffsetFlag)
						{
							prevSize = prevSize.WithHeight(prevSize.Height + node.OriginDesiredSize.Height);
							prevNode.VerticalOffsetFlag = true;
						}
						if (node.HorizontalOffsetFlag)
						{
							prevNode.HorizontalOffsetFlag = true;
						}
					}
					continue;
				}
				return GetBoundingSize(node, prevSize, node.OutgoingNodes);
			}
			return prevSize;
		}
	}

	private class Graph
	{
		private readonly Dictionary<AvaloniaObject, GraphNode> _nodeDic;

		private Size AvailableSize { get; set; }

		public Graph()
		{
			_nodeDic = new Dictionary<AvaloniaObject, GraphNode>();
		}

		public IEnumerable<GraphNode> GetNodes()
		{
			return _nodeDic.Values;
		}

		public void Clear()
		{
			AvailableSize = default(Size);
			_nodeDic.Clear();
		}

		public void Reset(bool clearPos = true)
		{
			_nodeDic.Values.Do(delegate(GraphNode node)
			{
				node.Reset(clearPos);
			});
		}

		public GraphNode? AddLink(GraphNode from, Layoutable? to)
		{
			if (to == null)
			{
				return null;
			}
			GraphNode graphNode;
			if (_nodeDic.ContainsKey(to))
			{
				graphNode = _nodeDic[to];
			}
			else
			{
				graphNode = new GraphNode(to);
				_nodeDic[to] = graphNode;
			}
			from.OutgoingNodes.Add(graphNode);
			return graphNode;
		}

		public GraphNode AddNode(Layoutable value)
		{
			if (!_nodeDic.ContainsKey(value))
			{
				GraphNode graphNode = new GraphNode(value);
				_nodeDic.Add(value, graphNode);
				return graphNode;
			}
			return _nodeDic[value];
		}

		public void Measure(Size availableSize)
		{
			AvailableSize = availableSize;
			Measure(_nodeDic.Values, null);
		}

		private void Measure(IEnumerable<GraphNode> nodes, HashSet<AvaloniaObject>? set)
		{
			if (set == null)
			{
				set = new HashSet<AvaloniaObject>();
			}
			foreach (GraphNode node in nodes)
			{
				if (!node.Measured && !node.OutgoingNodes.Any())
				{
					MeasureChild(node);
					continue;
				}
				if (node.OutgoingNodes.All((GraphNode item) => item.Measured))
				{
					MeasureChild(node);
					continue;
				}
				if (!set.Add(node.Element))
				{
					throw new Exception("RelativePanel error: Circular dependency detected. Layout could not complete.");
				}
				Measure(node.OutgoingNodes, set);
				if (!node.Measured)
				{
					MeasureChild(node);
				}
			}
		}

		private void MeasureChild(GraphNode node)
		{
			Layoutable element = node.Element;
			element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			node.OriginDesiredSize = element.DesiredSize;
			bool alignLeftWithPanel = GetAlignLeftWithPanel(element);
			bool alignTopWithPanel = GetAlignTopWithPanel(element);
			bool alignRightWithPanel = GetAlignRightWithPanel(element);
			bool alignBottomWithPanel = GetAlignBottomWithPanel(element);
			if (alignLeftWithPanel)
			{
				node.Left = 0.0;
			}
			if (alignTopWithPanel)
			{
				node.Top = 0.0;
			}
			if (alignRightWithPanel)
			{
				node.Right = 0.0;
			}
			if (alignBottomWithPanel)
			{
				node.Bottom = 0.0;
			}
			if (node.AlignLeftWithNode != null)
			{
				node.Left = (node.Left.IsNaN() ? node.AlignLeftWithNode.Left : (node.AlignLeftWithNode.Left * 0.5));
			}
			if (node.AlignTopWithNode != null)
			{
				node.Top = (node.Top.IsNaN() ? node.AlignTopWithNode.Top : (node.AlignTopWithNode.Top * 0.5));
			}
			if (node.AlignRightWithNode != null)
			{
				node.Right = (node.Right.IsNaN() ? node.AlignRightWithNode.Right : (node.AlignRightWithNode.Right * 0.5));
			}
			if (node.AlignBottomWithNode != null)
			{
				node.Bottom = (node.Bottom.IsNaN() ? node.AlignBottomWithNode.Bottom : (node.AlignBottomWithNode.Bottom * 0.5));
			}
			double num = AvailableSize.Height - node.Top - node.Bottom;
			if (num.IsNaN())
			{
				num = AvailableSize.Height;
				if (!node.Top.IsNaN() && node.Bottom.IsNaN())
				{
					num -= node.Top;
				}
				else if (node.Top.IsNaN() && !node.Bottom.IsNaN())
				{
					num -= node.Bottom;
				}
			}
			double num2 = AvailableSize.Width - node.Left - node.Right;
			if (num2.IsNaN())
			{
				num2 = AvailableSize.Width;
				if (!node.Left.IsNaN() && node.Right.IsNaN())
				{
					num2 -= node.Left;
				}
				else if (node.Left.IsNaN() && !node.Right.IsNaN())
				{
					num2 -= node.Right;
				}
			}
			element.Measure(new Size(Math.Max(num2, 0.0), Math.Max(num, 0.0)));
			Size desiredSize = element.DesiredSize;
			if (node.LeftOfNode != null && node.Left.IsNaN())
			{
				node.Left = node.LeftOfNode.Left - desiredSize.Width;
			}
			if (node.AboveNode != null && node.Top.IsNaN())
			{
				node.Top = node.AboveNode.Top - desiredSize.Height;
			}
			if (node.RightOfNode != null)
			{
				if (node.Right.IsNaN())
				{
					node.Right = node.RightOfNode.Right - desiredSize.Width;
				}
				if (node.Left.IsNaN())
				{
					node.Left = AvailableSize.Width - node.RightOfNode.Right;
				}
			}
			if (node.BelowNode != null)
			{
				if (node.Bottom.IsNaN())
				{
					node.Bottom = node.BelowNode.Bottom - desiredSize.Height;
				}
				if (node.Top.IsNaN())
				{
					node.Top = AvailableSize.Height - node.BelowNode.Bottom;
				}
			}
			if (node.AlignHorizontalCenterWith != null)
			{
				double num3 = (AvailableSize.Width + node.AlignHorizontalCenterWith.Left - node.AlignHorizontalCenterWith.Right - desiredSize.Width) * 0.5;
				double num4 = (AvailableSize.Width - node.AlignHorizontalCenterWith.Left + node.AlignHorizontalCenterWith.Right - desiredSize.Width) * 0.5;
				if (node.Left.IsNaN())
				{
					node.Left = num3;
				}
				else
				{
					node.Left = (node.Left + num3) * 0.5;
				}
				if (node.Right.IsNaN())
				{
					node.Right = num4;
				}
				else
				{
					node.Right = (node.Right + num4) * 0.5;
				}
			}
			if (node.AlignVerticalCenterWith != null)
			{
				double num5 = (AvailableSize.Height + node.AlignVerticalCenterWith.Top - node.AlignVerticalCenterWith.Bottom - desiredSize.Height) * 0.5;
				double num6 = (AvailableSize.Height - node.AlignVerticalCenterWith.Top + node.AlignVerticalCenterWith.Bottom - desiredSize.Height) * 0.5;
				if (node.Top.IsNaN())
				{
					node.Top = num5;
				}
				else
				{
					node.Top = (node.Top + num5) * 0.5;
				}
				if (node.Bottom.IsNaN())
				{
					node.Bottom = num6;
				}
				else
				{
					node.Bottom = (node.Bottom + num6) * 0.5;
				}
			}
			if (GetAlignHorizontalCenterWithPanel(element))
			{
				double num7 = (AvailableSize.Width - desiredSize.Width) * 0.5;
				if (node.Left.IsNaN())
				{
					node.Left = num7;
				}
				else
				{
					node.Left = (node.Left + num7) * 0.5;
				}
				if (node.Right.IsNaN())
				{
					node.Right = num7;
				}
				else
				{
					node.Right = (node.Right + num7) * 0.5;
				}
			}
			if (GetAlignVerticalCenterWithPanel(element))
			{
				double num8 = (AvailableSize.Height - desiredSize.Height) * 0.5;
				if (node.Top.IsNaN())
				{
					node.Top = num8;
				}
				else
				{
					node.Top = (node.Top + num8) * 0.5;
				}
				if (node.Bottom.IsNaN())
				{
					node.Bottom = num8;
				}
				else
				{
					node.Bottom = (node.Bottom + num8) * 0.5;
				}
			}
			if (node.Left.IsNaN())
			{
				if (!node.Right.IsNaN())
				{
					node.Left = AvailableSize.Width - node.Right - desiredSize.Width;
				}
				else
				{
					node.Left = 0.0;
					node.Right = AvailableSize.Width - desiredSize.Width;
				}
			}
			else if (!node.Left.IsNaN() && node.Right.IsNaN())
			{
				node.Right = AvailableSize.Width - node.Left - desiredSize.Width;
			}
			if (node.Top.IsNaN())
			{
				if (!node.Bottom.IsNaN())
				{
					node.Top = AvailableSize.Height - node.Bottom - desiredSize.Height;
				}
				else
				{
					node.Top = 0.0;
					node.Bottom = AvailableSize.Height - desiredSize.Height;
				}
			}
			else if (!node.Top.IsNaN() && node.Bottom.IsNaN())
			{
				node.Bottom = AvailableSize.Height - node.Top - desiredSize.Height;
			}
			node.Measured = true;
		}

		public Size GetBoundingSize(bool calcWidth, bool calcHeight)
		{
			Size size = default(Size);
			foreach (GraphNode value in _nodeDic.Values)
			{
				Size boundingSize = value.GetBoundingSize();
				size = size.WithWidth(Math.Max(size.Width, boundingSize.Width));
				size = size.WithHeight(Math.Max(size.Height, boundingSize.Height));
			}
			double num = (double.IsInfinity(AvailableSize.Width) ? size.Width : AvailableSize.Width);
			double num2 = (double.IsInfinity(AvailableSize.Height) ? size.Height : AvailableSize.Height);
			size = size.WithWidth(calcWidth ? size.Width : num);
			return size.WithHeight(calcHeight ? size.Height : num2);
		}
	}

	public static readonly AttachedProperty<object> AboveProperty;

	public static readonly AttachedProperty<bool> AlignBottomWithPanelProperty;

	public static readonly AttachedProperty<object> AlignBottomWithProperty;

	public static readonly AttachedProperty<bool> AlignHorizontalCenterWithPanelProperty;

	public static readonly AttachedProperty<object> AlignHorizontalCenterWithProperty;

	public static readonly AttachedProperty<bool> AlignLeftWithPanelProperty;

	public static readonly AttachedProperty<object> AlignLeftWithProperty;

	public static readonly AttachedProperty<bool> AlignRightWithPanelProperty;

	public static readonly AttachedProperty<object> AlignRightWithProperty;

	public static readonly AttachedProperty<bool> AlignTopWithPanelProperty;

	public static readonly AttachedProperty<object> AlignTopWithProperty;

	public static readonly AttachedProperty<bool> AlignVerticalCenterWithPanelProperty;

	public static readonly AttachedProperty<object> AlignVerticalCenterWithProperty;

	public static readonly AttachedProperty<object> BelowProperty;

	public static readonly AttachedProperty<object> LeftOfProperty;

	public static readonly AttachedProperty<object> RightOfProperty;

	private readonly Graph _childGraph;

	static RelativePanel()
	{
		AboveProperty = AvaloniaProperty.RegisterAttached<Layoutable, object>("Above", typeof(RelativePanel));
		AlignBottomWithPanelProperty = AvaloniaProperty.RegisterAttached<Layoutable, bool>("AlignBottomWithPanel", typeof(RelativePanel), defaultValue: false);
		AlignBottomWithProperty = AvaloniaProperty.RegisterAttached<Layoutable, object>("AlignBottomWith", typeof(RelativePanel));
		AlignHorizontalCenterWithPanelProperty = AvaloniaProperty.RegisterAttached<Layoutable, bool>("AlignHorizontalCenterWithPanel", typeof(RelativePanel), defaultValue: false);
		AlignHorizontalCenterWithProperty = AvaloniaProperty.RegisterAttached<Layoutable, object>("AlignHorizontalCenterWith", typeof(object), typeof(RelativePanel));
		AlignLeftWithPanelProperty = AvaloniaProperty.RegisterAttached<Layoutable, bool>("AlignLeftWithPanel", typeof(RelativePanel), defaultValue: false);
		AlignLeftWithProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("AlignLeftWith");
		AlignRightWithPanelProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, bool>("AlignRightWithPanel", defaultValue: false);
		AlignRightWithProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("AlignRightWith");
		AlignTopWithPanelProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, bool>("AlignTopWithPanel", defaultValue: false);
		AlignTopWithProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("AlignTopWith");
		AlignVerticalCenterWithPanelProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, bool>("AlignVerticalCenterWithPanel", defaultValue: false);
		AlignVerticalCenterWithProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("AlignVerticalCenterWith");
		BelowProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("Below");
		LeftOfProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("LeftOf");
		RightOfProperty = AvaloniaProperty.RegisterAttached<RelativePanel, Layoutable, object>("RightOf");
		Visual.ClipToBoundsProperty.OverrideDefaultValue<RelativePanel>(defaultValue: true);
		Panel.AffectsParentArrange<RelativePanel>(new AvaloniaProperty[16]
		{
			AlignLeftWithPanelProperty, AlignLeftWithProperty, LeftOfProperty, AlignRightWithPanelProperty, AlignRightWithProperty, RightOfProperty, AlignTopWithPanelProperty, AlignTopWithProperty, AboveProperty, AlignBottomWithPanelProperty,
			AlignBottomWithProperty, BelowProperty, AlignHorizontalCenterWithPanelProperty, AlignHorizontalCenterWithProperty, AlignVerticalCenterWithPanelProperty, AlignVerticalCenterWithProperty
		});
		Panel.AffectsParentMeasure<RelativePanel>(new AvaloniaProperty[16]
		{
			AlignLeftWithPanelProperty, AlignLeftWithProperty, LeftOfProperty, AlignRightWithPanelProperty, AlignRightWithProperty, RightOfProperty, AlignTopWithPanelProperty, AlignTopWithProperty, AboveProperty, AlignBottomWithPanelProperty,
			AlignBottomWithProperty, BelowProperty, AlignHorizontalCenterWithPanelProperty, AlignHorizontalCenterWithProperty, AlignVerticalCenterWithPanelProperty, AlignVerticalCenterWithProperty
		});
	}

	[ResolveByName]
	public static object GetAbove(AvaloniaObject obj)
	{
		return obj.GetValue(AboveProperty);
	}

	[ResolveByName]
	public static void SetAbove(AvaloniaObject obj, object value)
	{
		obj.SetValue(AboveProperty, value);
	}

	public static bool GetAlignBottomWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignBottomWithPanelProperty);
	}

	public static void SetAlignBottomWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignBottomWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignBottomWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignBottomWithProperty);
	}

	[ResolveByName]
	public static void SetAlignBottomWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignBottomWithProperty, value);
	}

	public static bool GetAlignHorizontalCenterWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignHorizontalCenterWithPanelProperty);
	}

	public static void SetAlignHorizontalCenterWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignHorizontalCenterWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignHorizontalCenterWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignHorizontalCenterWithProperty);
	}

	[ResolveByName]
	public static void SetAlignHorizontalCenterWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignHorizontalCenterWithProperty, value);
	}

	public static bool GetAlignLeftWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignLeftWithPanelProperty);
	}

	public static void SetAlignLeftWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignLeftWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignLeftWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignLeftWithProperty);
	}

	[ResolveByName]
	public static void SetAlignLeftWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignLeftWithProperty, value);
	}

	public static bool GetAlignRightWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignRightWithPanelProperty);
	}

	public static void SetAlignRightWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignRightWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignRightWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignRightWithProperty);
	}

	[ResolveByName]
	public static void SetAlignRightWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignRightWithProperty, value);
	}

	public static bool GetAlignTopWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignTopWithPanelProperty);
	}

	public static void SetAlignTopWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignTopWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignTopWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignTopWithProperty);
	}

	[ResolveByName]
	public static void SetAlignTopWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignTopWithProperty, value);
	}

	public static bool GetAlignVerticalCenterWithPanel(AvaloniaObject obj)
	{
		return obj.GetValue(AlignVerticalCenterWithPanelProperty);
	}

	public static void SetAlignVerticalCenterWithPanel(AvaloniaObject obj, bool value)
	{
		obj.SetValue(AlignVerticalCenterWithPanelProperty, value);
	}

	[ResolveByName]
	public static object GetAlignVerticalCenterWith(AvaloniaObject obj)
	{
		return obj.GetValue(AlignVerticalCenterWithProperty);
	}

	[ResolveByName]
	public static void SetAlignVerticalCenterWith(AvaloniaObject obj, object value)
	{
		obj.SetValue(AlignVerticalCenterWithProperty, value);
	}

	[ResolveByName]
	public static object GetBelow(AvaloniaObject obj)
	{
		return obj.GetValue(BelowProperty);
	}

	[ResolveByName]
	public static void SetBelow(AvaloniaObject obj, object value)
	{
		obj.SetValue(BelowProperty, value);
	}

	[ResolveByName]
	public static object GetLeftOf(AvaloniaObject obj)
	{
		return obj.GetValue(LeftOfProperty);
	}

	[ResolveByName]
	public static void SetLeftOf(AvaloniaObject obj, object value)
	{
		obj.SetValue(LeftOfProperty, value);
	}

	[ResolveByName]
	public static object GetRightOf(AvaloniaObject obj)
	{
		return obj.GetValue(RightOfProperty);
	}

	[ResolveByName]
	public static void SetRightOf(AvaloniaObject obj, object value)
	{
		obj.SetValue(RightOfProperty, value);
	}

	public RelativePanel()
	{
		_childGraph = new Graph();
	}

	private Layoutable? GetDependencyElement(AvaloniaProperty property, AvaloniaObject child)
	{
		if (child.GetValue(property) is Layoutable layoutable)
		{
			if (base.Children.Contains(layoutable))
			{
				return layoutable;
			}
			throw new ArgumentException("RelativePanel error: Element does not exist in the current context: " + property.Name);
		}
		return null;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		_childGraph.Clear();
		foreach (Control child in base.Children)
		{
			GraphNode graphNode = _childGraph.AddNode(child);
			graphNode.AlignLeftWithNode = _childGraph.AddLink(graphNode, GetDependencyElement(AlignLeftWithProperty, child));
			graphNode.AlignTopWithNode = _childGraph.AddLink(graphNode, GetDependencyElement(AlignTopWithProperty, child));
			graphNode.AlignRightWithNode = _childGraph.AddLink(graphNode, GetDependencyElement(AlignRightWithProperty, child));
			graphNode.AlignBottomWithNode = _childGraph.AddLink(graphNode, GetDependencyElement(AlignBottomWithProperty, child));
			graphNode.LeftOfNode = _childGraph.AddLink(graphNode, GetDependencyElement(LeftOfProperty, child));
			graphNode.AboveNode = _childGraph.AddLink(graphNode, GetDependencyElement(AboveProperty, child));
			graphNode.RightOfNode = _childGraph.AddLink(graphNode, GetDependencyElement(RightOfProperty, child));
			graphNode.BelowNode = _childGraph.AddLink(graphNode, GetDependencyElement(BelowProperty, child));
			graphNode.AlignHorizontalCenterWith = _childGraph.AddLink(graphNode, GetDependencyElement(AlignHorizontalCenterWithProperty, child));
			graphNode.AlignVerticalCenterWith = _childGraph.AddLink(graphNode, GetDependencyElement(AlignVerticalCenterWithProperty, child));
		}
		_childGraph.Measure(availableSize);
		_childGraph.Reset(clearPos: false);
		bool calcWidth = base.Width.IsNaN() && base.HorizontalAlignment != HorizontalAlignment.Stretch;
		bool calcHeight = base.Height.IsNaN() && base.VerticalAlignment != VerticalAlignment.Stretch;
		Size boundingSize = _childGraph.GetBoundingSize(calcWidth, calcHeight);
		_childGraph.Reset();
		_childGraph.Measure(boundingSize);
		return boundingSize;
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		_childGraph.GetNodes().Do(delegate(GraphNode node)
		{
			node.Arrange(arrangeSize);
		});
		return arrangeSize;
	}
}
