using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input.GestureRecognizers;
using Avalonia.VisualTree;

namespace Avalonia.Input;

public class Pointer : IPointer, IDisposable
{
	private static int s_NextFreePointerId = 1000;

	public int Id { get; }

	public IInputElement? Captured { get; private set; }

	public PointerType Type { get; }

	public bool IsPrimary { get; }

	internal GestureRecognizer? CapturedGestureRecognizer { get; private set; }

	public static int GetNextFreeId()
	{
		return s_NextFreePointerId++;
	}

	public Pointer(int id, PointerType type, bool isPrimary)
	{
		Id = id;
		Type = type;
		IsPrimary = isPrimary;
	}

	private static IInputElement? FindCommonParent(IInputElement? control1, IInputElement? control2)
	{
		if (!(control1 is Visual visual) || !(control2 is Visual visual2))
		{
			return null;
		}
		HashSet<IInputElement> hashSet = new HashSet<IInputElement>(visual.GetSelfAndVisualAncestors().OfType<IInputElement>());
		return visual2.GetSelfAndVisualAncestors().OfType<IInputElement>().FirstOrDefault(hashSet.Contains);
	}

	protected virtual void PlatformCapture(IInputElement? element)
	{
	}

	public void Capture(IInputElement? control)
	{
		if (Captured is Visual visual)
		{
			visual.DetachedFromVisualTree -= OnCaptureDetached;
		}
		IInputElement captured = Captured;
		Captured = control;
		PlatformCapture(control);
		if (captured is Visual visual2)
		{
			IInputElement inputElement = FindCommonParent(control, captured);
			foreach (IInputElement item in visual2.GetSelfAndVisualAncestors().OfType<IInputElement>())
			{
				if (item == inputElement)
				{
					break;
				}
				item.RaiseEvent(new PointerCaptureLostEventArgs(item, this));
			}
		}
		if (Captured is Visual visual3)
		{
			visual3.DetachedFromVisualTree += OnCaptureDetached;
		}
		if (Captured != null)
		{
			CaptureGestureRecognizer(null);
		}
	}

	private static IInputElement? GetNextCapture(Visual parent)
	{
		return (parent as IInputElement) ?? parent.FindAncestorOfType<IInputElement>();
	}

	private void OnCaptureDetached(object? sender, VisualTreeAttachmentEventArgs e)
	{
		Capture(GetNextCapture(e.Parent));
	}

	public void Dispose()
	{
		Capture(null);
	}

	internal void CaptureGestureRecognizer(GestureRecognizer? gestureRecognizer)
	{
		if (CapturedGestureRecognizer != gestureRecognizer)
		{
			CapturedGestureRecognizer?.PointerCaptureLostInternal(this);
		}
		if (gestureRecognizer != null)
		{
			Capture(null);
		}
		CapturedGestureRecognizer = gestureRecognizer;
	}
}
