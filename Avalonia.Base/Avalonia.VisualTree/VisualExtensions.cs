using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Rendering;
using Avalonia.Utilities;

namespace Avalonia.VisualTree;

public static class VisualExtensions
{
	private class ZOrderElement : IComparable<ZOrderElement>
	{
		private class ZOrderComparer : IComparer<ZOrderElement>
		{
			public int Compare(ZOrderElement? x, ZOrderElement? y)
			{
				if (x == y)
				{
					return 0;
				}
				if (y == null)
				{
					return 1;
				}
				return x?.CompareTo(y) ?? (-1);
			}
		}

		public Visual? Element { get; set; }

		public int Index { get; set; }

		public int ZIndex { get; set; }

		public static IComparer<ZOrderElement> Comparer { get; } = new ZOrderComparer();

		public int CompareTo(ZOrderElement? other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = other.ZIndex - ZIndex;
			if (num != 0)
			{
				return num;
			}
			return other.Index - Index;
		}
	}

	public static int CalculateDistanceFromAncestor(this Visual visual, Visual? ancestor)
	{
		Visual visual2 = visual ?? throw new ArgumentNullException("visual");
		int num = 0;
		while (visual2 != null && visual2 != ancestor)
		{
			visual2 = visual2.VisualParent;
			num++;
		}
		if (visual2 == null)
		{
			return -1;
		}
		return num;
	}

	public static int CalculateDistanceFromRoot(Visual visual)
	{
		Visual visual2 = visual ?? throw new ArgumentNullException("visual");
		int num = 0;
		visual2 = visual2.VisualParent;
		while (visual2 != null)
		{
			visual2 = visual2.VisualParent;
			num++;
		}
		return num;
	}

	public static Visual? FindCommonVisualAncestor(this Visual? visual, Visual? target)
	{
		if (visual == null || target == null)
		{
			return null;
		}
		Visual node2 = visual;
		Visual node3 = target;
		int num = CalculateDistanceFromRoot(node2);
		int num2 = CalculateDistanceFromRoot(node3);
		if (num > num2)
		{
			GoUpwards(ref node2, num - num2);
		}
		else
		{
			GoUpwards(ref node3, num2 - num);
		}
		if (node2 == node3)
		{
			return node2;
		}
		while (node2 != null && node3 != null)
		{
			Visual visualParent = node2.VisualParent;
			Visual visualParent2 = node3.VisualParent;
			if (visualParent == visualParent2)
			{
				return visualParent;
			}
			node2 = node2.VisualParent;
			node3 = node3.VisualParent;
		}
		return null;
		static void GoUpwards(ref Visual? node, int count)
		{
			for (int i = 0; i < count; i++)
			{
				node = node?.VisualParent;
			}
		}
	}

	public static IEnumerable<Visual> GetVisualAncestors(this Visual visual)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		for (Visual v = visual.VisualParent; v != null; v = v.VisualParent)
		{
			yield return v;
		}
	}

	public static T? FindAncestorOfType<T>(this Visual? visual, bool includeSelf = false) where T : class
	{
		if (visual == null)
		{
			return null;
		}
		for (Visual visual2 = (includeSelf ? visual : visual.VisualParent); visual2 != null; visual2 = visual2.VisualParent)
		{
			if (visual2 is T result)
			{
				return result;
			}
		}
		return null;
	}

	public static T? FindDescendantOfType<T>(this Visual? visual, bool includeSelf = false) where T : class
	{
		if (visual == null)
		{
			return null;
		}
		if (includeSelf && visual is T result)
		{
			return result;
		}
		return FindDescendantOfTypeCore<T>(visual);
	}

	public static IEnumerable<Visual> GetSelfAndVisualAncestors(this Visual visual)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		yield return visual;
		foreach (Visual visualAncestor in visual.GetVisualAncestors())
		{
			yield return visualAncestor;
		}
	}

	public static TransformedBounds? GetTransformedBounds(this Visual visual)
	{
		Rect clip = default(Rect);
		Matrix transform = Matrix.Identity;
		if (!Visit(visual))
		{
			return null;
		}
		return new TransformedBounds(new Rect(visual.Bounds.Size), clip, transform);
		bool Visit(Visual visual)
		{
			if (!visual.IsVisible)
			{
				return false;
			}
			Rect rect = new Rect(visual.Bounds.Size);
			Visual visualParent = visual.GetVisualParent();
			if (visualParent == null)
			{
				clip = rect;
				return true;
			}
			if (!Visit(visualParent))
			{
				return false;
			}
			Matrix identity = Matrix.Identity;
			if (visual.HasMirrorTransform)
			{
				Matrix matrix = new Matrix(-1.0, 0.0, 0.0, 1.0, visual.Bounds.Width, 0.0);
				identity *= matrix;
			}
			if (visual.RenderTransform != null)
			{
				Matrix matrix2 = Matrix.CreateTranslation(visual.RenderTransformOrigin.ToPixels(rect.Size));
				Matrix matrix3 = -matrix2 * visual.RenderTransform.Value * matrix2;
				identity *= matrix3;
			}
			transform = identity * Matrix.CreateTranslation(visual.Bounds.Position) * transform;
			if (visual.ClipToBounds)
			{
				Rect rect2 = rect.TransformToAABB(transform);
				Rect rect3 = (visual.ClipToBounds ? rect2.Intersect(clip) : clip);
				clip = clip.Intersect(rect3);
			}
			return true;
		}
	}

	public static Visual? GetVisualAt(this Visual visual, Point p)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		return visual.GetVisualAt(p, (Visual x) => x.IsVisible);
	}

	public static Visual? GetVisualAt(this Visual visual, Point p, Func<Visual, bool> filter)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		IRenderRoot visualRoot = visual.GetVisualRoot();
		if (visualRoot == null)
		{
			return null;
		}
		Point? point = visual.TranslatePoint(p, (Visual)visualRoot);
		if (point.HasValue)
		{
			return visualRoot.HitTester.HitTestFirst(point.Value, visual, filter);
		}
		return null;
	}

	public static IEnumerable<Visual> GetVisualsAt(this Visual visual, Point p)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		return visual.GetVisualsAt(p, (Visual x) => x.IsVisible);
	}

	public static IEnumerable<Visual> GetVisualsAt(this Visual visual, Point p, Func<Visual, bool> filter)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		IRenderRoot visualRoot = visual.GetVisualRoot();
		if (visualRoot == null)
		{
			return Array.Empty<Visual>();
		}
		Point? point = visual.TranslatePoint(p, (Visual)visualRoot);
		if (point.HasValue)
		{
			return visualRoot.HitTester.HitTest(point.Value, visual, filter);
		}
		return Enumerable.Empty<Visual>();
	}

	public static IEnumerable<Visual> GetVisualChildren(this Visual visual)
	{
		return visual.VisualChildren;
	}

	public static IEnumerable<Visual> GetVisualDescendants(this Visual visual)
	{
		foreach (Visual child in visual.VisualChildren)
		{
			yield return child;
			foreach (Visual visualDescendant in child.GetVisualDescendants())
			{
				yield return visualDescendant;
			}
		}
	}

	public static IEnumerable<Visual> GetSelfAndVisualDescendants(this Visual visual)
	{
		yield return visual;
		foreach (Visual visualDescendant in visual.GetVisualDescendants())
		{
			yield return visualDescendant;
		}
	}

	public static Visual? GetVisualParent(this Visual visual)
	{
		return visual.VisualParent;
	}

	public static T? GetVisualParent<T>(this Visual visual) where T : class
	{
		return visual.VisualParent as T;
	}

	public static IRenderRoot? GetVisualRoot(this Visual visual)
	{
		ThrowHelper.ThrowIfNull(visual, "visual");
		return (visual as IRenderRoot) ?? visual.VisualRoot;
	}

	public static bool IsAttachedToVisualTree(this Visual visual)
	{
		return visual.IsAttachedToVisualTree;
	}

	public static bool IsVisualAncestorOf(this Visual? visual, Visual? target)
	{
		for (Visual visual2 = target?.VisualParent; visual2 != null; visual2 = visual2.VisualParent)
		{
			if (visual2 == visual)
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<Visual> SortByZIndex(this IEnumerable<Visual> elements)
	{
		return from x in elements.Select((Visual element, int index) => new ZOrderElement
			{
				Element = element,
				Index = index,
				ZIndex = element.ZIndex
			}).OrderBy((ZOrderElement x) => x, ZOrderElement.Comparer)
			select x.Element;
	}

	private static T? FindDescendantOfTypeCore<T>(Visual visual) where T : class
	{
		IAvaloniaList<Visual> visualChildren = visual.VisualChildren;
		int count = visualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			Visual visual2 = visualChildren[i];
			if (visual2 is T result)
			{
				return result;
			}
			T val = FindDescendantOfTypeCore<T>(visual2);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}
}
