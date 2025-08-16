using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.VisualTree;

namespace Avalonia.Input;

public static class InputExtensions
{
	private static readonly Func<Visual, bool> s_hitTestDelegate = IsHitTestVisible;

	public static IEnumerable<IInputElement> GetInputElementsAt(this IInputElement element, Point p)
	{
		element = element ?? throw new ArgumentNullException("element");
		return (element as Visual)?.GetVisualsAt(p, s_hitTestDelegate).Cast<IInputElement>() ?? Enumerable.Empty<IInputElement>();
	}

	public static IInputElement? InputHitTest(this IInputElement element, Point p)
	{
		element = element ?? throw new ArgumentNullException("element");
		return (element as Visual)?.GetVisualAt(p, s_hitTestDelegate) as IInputElement;
	}

	public static IInputElement? InputHitTest(this IInputElement element, Point p, Func<Visual, bool> filter)
	{
		element = element ?? throw new ArgumentNullException("element");
		filter = filter ?? throw new ArgumentNullException("filter");
		return (element as Visual)?.GetVisualAt(p, (Visual x) => s_hitTestDelegate(x) && filter(x)) as IInputElement;
	}

	private static bool IsHitTestVisible(Visual visual)
	{
		if (visual is IInputElement inputElement && visual.IsVisible && inputElement.IsHitTestVisible && inputElement.IsEffectivelyEnabled)
		{
			return visual.IsAttachedToVisualTree;
		}
		return false;
	}
}
