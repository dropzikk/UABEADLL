using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AvaloniaEdit.Utils;

public static class ExtensionMethods
{
	public const double Epsilon = 0.01;

	public static bool IsClose(this double d1, double d2)
	{
		if (d1 == d2)
		{
			return true;
		}
		return Math.Abs(d1 - d2) < 0.01;
	}

	public static bool IsClose(this Avalonia.Size d1, Avalonia.Size d2)
	{
		if (d1.Width.IsClose(d2.Width))
		{
			return d1.Height.IsClose(d2.Height);
		}
		return false;
	}

	public static bool IsClose(this Vector d1, Vector d2)
	{
		if (d1.X.IsClose(d2.X))
		{
			return d1.Y.IsClose(d2.Y);
		}
		return false;
	}

	public static double CoerceValue(this double value, double minimum, double maximum)
	{
		return Math.Max(Math.Min(value, maximum), minimum);
	}

	public static int CoerceValue(this int value, int minimum, int maximum)
	{
		return Math.Max(Math.Min(value, maximum), minimum);
	}

	public static Typeface CreateTypeface(this Control fe)
	{
		return new Typeface(fe.GetValue(TextElement.FontFamilyProperty), fe.GetValue(TextElement.FontStyleProperty), fe.GetValue(TextElement.FontWeightProperty), fe.GetValue(TextElement.FontStretchProperty));
	}

	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
	{
		foreach (T element in elements)
		{
			collection.Add(element);
		}
	}

	public static IEnumerable<T> Sequence<T>(T value)
	{
		yield return value;
	}

	public static bool? GetBoolAttribute(this XmlReader reader, string attributeName)
	{
		string attribute = reader.GetAttribute(attributeName);
		if (attribute == null)
		{
			return null;
		}
		return XmlConvert.ToBoolean(attribute);
	}

	public static System.Drawing.Point ToSystemDrawing(this Avalonia.Point p)
	{
		return new System.Drawing.Point((int)p.X, (int)p.Y);
	}

	public static Avalonia.Point ToAvalonia(this System.Drawing.Point p)
	{
		return new Avalonia.Point(p.X, p.Y);
	}

	public static Avalonia.Size ToAvalonia(this System.Drawing.Size s)
	{
		return new Avalonia.Size(s.Width, s.Height);
	}

	public static Rect ToAvalonia(this Rectangle rect)
	{
		return new Rect(rect.Location.ToAvalonia(), rect.Size.ToAvalonia());
	}

	public static Avalonia.Point SnapToDevicePixels(this Avalonia.Point p, Visual targetVisual)
	{
		IRenderRoot visualRoot = targetVisual.GetVisualRoot();
		Vector scales = new Vector(visualRoot.RenderScaling, visualRoot.RenderScaling);
		Matrix? matrix = targetVisual.TransformToVisual((Control)visualRoot) * Matrix.CreateScale(scales);
		if (!matrix.HasValue)
		{
			return p;
		}
		Avalonia.Point point = p.Transform(matrix.Value);
		point = new Avalonia.Point((double)(int)point.X + 0.5, (double)(int)point.Y + 0.5);
		Matrix transform = matrix.Value.Invert();
		return point.Transform(transform);
	}

	public static IEnumerable<AvaloniaObject> VisualAncestorsAndSelf(this AvaloniaObject obj)
	{
		while (obj != null)
		{
			yield return obj;
			if (obj is Visual visual)
			{
				obj = visual.GetVisualParent();
				continue;
			}
			break;
		}
	}

	public static IEnumerable<char> AsEnumerable(this string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			yield return s[i];
		}
	}

	[Conditional("DEBUG")]
	public static void CheckIsFrozen(object o)
	{
		if (o is IFreezable freezable)
		{
			_ = freezable.IsFrozen;
		}
	}

	[Conditional("DEBUG")]
	public static void Log(bool condition, string format, params object[] args)
	{
		if (condition)
		{
			_ = DateTime.Now.ToString("hh:MM:ss") + ": " + string.Format(format, args);
		}
	}

	public static bool CapturePointer(this IInputElement element, IPointer device)
	{
		device.Capture(element);
		return device.Captured == element;
	}

	public static void ReleasePointerCapture(this IInputElement element, IPointer device)
	{
		if (element == device.Captured)
		{
			device.Capture(null);
		}
	}

	public static T PeekOrDefault<T>(this ImmutableStack<T> stack)
	{
		if (!stack.IsEmpty)
		{
			return stack.Peek();
		}
		return default(T);
	}

	public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> action)
	{
		return observable.Subscribe(new AnonymousObserver<T>(action));
	}
}
