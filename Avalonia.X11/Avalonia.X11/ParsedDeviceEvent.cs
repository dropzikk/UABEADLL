using System.Collections.Generic;
using Avalonia.Input;

namespace Avalonia.X11;

internal class ParsedDeviceEvent
{
	public XiEventType Type { get; }

	public RawInputModifiers Modifiers { get; }

	public ulong Timestamp { get; }

	public Point Position { get; }

	public int Button { get; set; }

	public int Detail { get; set; }

	public bool Emulated { get; set; }

	public Dictionary<int, double> Valuators { get; }

	public unsafe static RawInputModifiers ParseButtonState(int len, byte* buttons)
	{
		RawInputModifiers rawInputModifiers = RawInputModifiers.None;
		if (len > 0)
		{
			if (XLib.XIMaskIsSet(buttons, 1))
			{
				rawInputModifiers |= RawInputModifiers.LeftMouseButton;
			}
			if (XLib.XIMaskIsSet(buttons, 2))
			{
				rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
			}
			if (XLib.XIMaskIsSet(buttons, 3))
			{
				rawInputModifiers |= RawInputModifiers.RightMouseButton;
			}
			if (len > 1)
			{
				if (XLib.XIMaskIsSet(buttons, 8))
				{
					rawInputModifiers |= RawInputModifiers.XButton1MouseButton;
				}
				if (XLib.XIMaskIsSet(buttons, 9))
				{
					rawInputModifiers |= RawInputModifiers.XButton2MouseButton;
				}
			}
		}
		return rawInputModifiers;
	}

	public unsafe ParsedDeviceEvent(XIDeviceEvent* ev)
	{
		Type = ev->evtype;
		Timestamp = (ulong)ev->time.ToInt64();
		int effective = ev->mods.Effective;
		if (((XModifierMask)effective).HasAllFlags(XModifierMask.ShiftMask))
		{
			Modifiers |= RawInputModifiers.Shift;
		}
		if (((XModifierMask)effective).HasAllFlags(XModifierMask.ControlMask))
		{
			Modifiers |= RawInputModifiers.Control;
		}
		if (((XModifierMask)effective).HasAllFlags(XModifierMask.Mod1Mask))
		{
			Modifiers |= RawInputModifiers.Alt;
		}
		if (((XModifierMask)effective).HasAllFlags(XModifierMask.Mod4Mask))
		{
			Modifiers |= RawInputModifiers.Meta;
		}
		Modifiers |= ParseButtonState(ev->buttons.MaskLen, ev->buttons.Mask);
		Valuators = new Dictionary<int, double>();
		Position = new Point(ev->event_x, ev->event_y);
		double* values = ev->valuators.Values;
		if (ev->valuators.Mask != null)
		{
			for (int i = 0; i < ev->valuators.MaskLen * 8; i++)
			{
				if (XLib.XIMaskIsSet(ev->valuators.Mask, i))
				{
					Valuators[i] = *(values++);
				}
			}
		}
		if (Type == XiEventType.XI_ButtonPress || Type == XiEventType.XI_ButtonRelease)
		{
			Button = ev->detail;
		}
		Detail = ev->detail;
		Emulated = ev->flags.HasAllFlags(XiDeviceEventFlags.XIPointerEmulated);
	}
}
