using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Avalonia.X11;

internal class XI2Manager
{
	private class DeviceInfo
	{
		public int Id { get; }

		public XIValuatorClassInfo[] Valuators { get; private set; }

		public XIScrollClassInfo[] Scrollers { get; private set; }

		public unsafe DeviceInfo(XIDeviceInfo info)
		{
			Id = info.Deviceid;
			Update(info.Classes, info.NumClasses);
		}

		public unsafe virtual void Update(XIAnyClassInfo** classes, int num)
		{
			List<XIValuatorClassInfo> list = new List<XIValuatorClassInfo>();
			List<XIScrollClassInfo> list2 = new List<XIScrollClassInfo>();
			for (int i = 0; i < num; i++)
			{
				if (classes[i]->Type == XiDeviceClass.XIValuatorClass)
				{
					list.Add(*(*(XIValuatorClassInfo**)((byte*)classes + (nint)i * (nint)sizeof(XIValuatorClassInfo*))));
				}
				if (classes[i]->Type == XiDeviceClass.XIScrollClass)
				{
					list2.Add(*(*(XIScrollClassInfo**)((byte*)classes + (nint)i * (nint)sizeof(XIScrollClassInfo*))));
				}
			}
			Valuators = list.ToArray();
			Scrollers = list2.ToArray();
		}

		public void UpdateValuators(Dictionary<int, double> valuators)
		{
			foreach (KeyValuePair<int, double> valuator in valuators)
			{
				if (Valuators.Length > valuator.Key)
				{
					Valuators[valuator.Key].Value = valuator.Value;
				}
			}
		}
	}

	private class PointerDeviceInfo : DeviceInfo
	{
		public PointerDeviceInfo(XIDeviceInfo info)
			: base(info)
		{
		}

		public bool HasScroll(ParsedDeviceEvent ev)
		{
			foreach (KeyValuePair<int, double> val in ev.Valuators)
			{
				if (base.Scrollers.Any((XIScrollClassInfo s) => s.Number == val.Key))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasMotion(ParsedDeviceEvent ev)
		{
			foreach (KeyValuePair<int, double> val in ev.Valuators)
			{
				if (base.Scrollers.All((XIScrollClassInfo s) => s.Number != val.Key))
				{
					return true;
				}
			}
			return false;
		}
	}

	private static readonly XiEventType[] DefaultEventTypes = new XiEventType[5]
	{
		XiEventType.XI_Motion,
		XiEventType.XI_ButtonPress,
		XiEventType.XI_ButtonRelease,
		XiEventType.XI_Leave,
		XiEventType.XI_Enter
	};

	private static readonly XiEventType[] MultiTouchEventTypes = new XiEventType[3]
	{
		XiEventType.XI_TouchBegin,
		XiEventType.XI_TouchUpdate,
		XiEventType.XI_TouchEnd
	};

	private X11Info _x11;

	private bool _multitouch;

	private Dictionary<IntPtr, IXI2Client> _clients = new Dictionary<IntPtr, IXI2Client>();

	private PointerDeviceInfo _pointerDevice;

	private AvaloniaX11Platform _platform;

	public unsafe bool Init(AvaloniaX11Platform platform)
	{
		_platform = platform;
		_x11 = platform.Info;
		_multitouch = platform.Options?.EnableMultiTouch ?? true;
		int ndevices_return;
		XIDeviceInfo* ptr = (XIDeviceInfo*)(void*)XLib.XIQueryDevice(_x11.Display, 1, out ndevices_return);
		for (int i = 0; i < ndevices_return; i++)
		{
			if (ptr[i].Use == XiDeviceType.XIMasterPointer)
			{
				_pointerDevice = new PointerDeviceInfo(ptr[i]);
				break;
			}
		}
		if (_pointerDevice == null)
		{
			return false;
		}
		return XLib.XiSelectEvents(_x11.Display, _x11.RootWindow, new Dictionary<int, List<XiEventType>> { [_pointerDevice.Id] = new List<XiEventType> { XiEventType.XI_DeviceChanged } }) == Status.Success;
	}

	public XEventMask AddWindow(IntPtr xid, IXI2Client window)
	{
		_clients[xid] = window;
		int num = DefaultEventTypes.Length;
		if (_multitouch)
		{
			num += MultiTouchEventTypes.Length;
		}
		List<XiEventType> list = new List<XiEventType>(num);
		list.AddRange(DefaultEventTypes);
		if (_multitouch)
		{
			list.AddRange(MultiTouchEventTypes);
		}
		XLib.XiSelectEvents(_x11.Display, xid, new Dictionary<int, List<XiEventType>> { [_pointerDevice.Id] = list });
		return XEventMask.ButtonPressMask | XEventMask.ButtonReleaseMask | XEventMask.EnterWindowMask | XEventMask.LeaveWindowMask | XEventMask.PointerMotionMask | XEventMask.Button1MotionMask | XEventMask.Button2MotionMask | XEventMask.Button3MotionMask | XEventMask.Button4MotionMask | XEventMask.Button5MotionMask | XEventMask.ButtonMotionMask;
	}

	public void OnWindowDestroyed(IntPtr xid)
	{
		_clients.Remove(xid);
	}

	public unsafe void OnEvent(XIEvent* xev)
	{
		if (xev->evtype == XiEventType.XI_DeviceChanged)
		{
			_pointerDevice.Update(((XIDeviceChangedEvent*)xev)->Classes, ((XIDeviceChangedEvent*)xev)->NumClasses);
		}
		if ((xev->evtype >= XiEventType.XI_ButtonPress && xev->evtype <= XiEventType.XI_Motion) || (xev->evtype >= XiEventType.XI_TouchBegin && xev->evtype <= XiEventType.XI_TouchEnd))
		{
			if (_clients.TryGetValue(((XIDeviceEvent*)xev)->EventWindow, out var value))
			{
				OnDeviceEvent(value, new ParsedDeviceEvent((XIDeviceEvent*)xev));
			}
		}
		if (xev->evtype == XiEventType.XI_Leave || xev->evtype == XiEventType.XI_Enter)
		{
			if (_clients.TryGetValue(((XIEnterLeaveEvent*)xev)->EventWindow, out var value2))
			{
				OnEnterLeaveEvent(value2, ref *(XIEnterLeaveEvent*)xev);
			}
		}
	}

	private unsafe void OnEnterLeaveEvent(IXI2Client client, ref XIEnterLeaveEvent ev)
	{
		if (ev.evtype == XiEventType.XI_Leave)
		{
			RawInputModifiers rawInputModifiers = ParsedDeviceEvent.ParseButtonState(ev.buttons.MaskLen, ev.buttons.Mask);
			XiEnterLeaveDetail detail = ev.detail;
			if ((detail == XiEnterLeaveDetail.XINotifyNonlinearVirtual || detail == XiEnterLeaveDetail.XINotifyNonlinear || detail == XiEnterLeaveDetail.XINotifyVirtual) && rawInputModifiers == RawInputModifiers.None)
			{
				client.ScheduleXI2Input(new RawPointerEventArgs(client.MouseDevice, (ulong)ev.time.ToInt64(), client.InputRoot, RawPointerEventType.LeaveWindow, new Point(ev.event_x, ev.event_y), rawInputModifiers));
			}
		}
	}

	private void OnDeviceEvent(IXI2Client client, ParsedDeviceEvent ev)
	{
		if (ev.Type == XiEventType.XI_TouchBegin || ev.Type == XiEventType.XI_TouchUpdate || ev.Type == XiEventType.XI_TouchEnd)
		{
			RawPointerEventType type = ((ev.Type == XiEventType.XI_TouchBegin) ? RawPointerEventType.TouchBegin : ((ev.Type == XiEventType.XI_TouchUpdate) ? RawPointerEventType.TouchUpdate : RawPointerEventType.TouchEnd));
			client.ScheduleXI2Input(new RawTouchEventArgs(client.TouchDevice, ev.Timestamp, client.InputRoot, type, ev.Position, ev.Modifiers, ev.Detail));
		}
		else
		{
			if (!client.IsEnabled || (_multitouch && ev.Emulated))
			{
				return;
			}
			if (ev.Type == XiEventType.XI_Motion)
			{
				Vector vector = default(Vector);
				foreach (KeyValuePair<int, double> valuator in ev.Valuators)
				{
					XIScrollClassInfo[] scrollers = _pointerDevice.Scrollers;
					for (int i = 0; i < scrollers.Length; i++)
					{
						XIScrollClassInfo xIScrollClassInfo = scrollers[i];
						if (xIScrollClassInfo.Number == valuator.Key)
						{
							double value = _pointerDevice.Valuators[xIScrollClassInfo.Number].Value;
							if (value != 0.0)
							{
								double num = (value - valuator.Value) / xIScrollClassInfo.Increment;
								vector = ((xIScrollClassInfo.ScrollType != XiScrollType.Horizontal) ? vector.WithY(vector.Y + num) : vector.WithX(vector.X + num));
							}
						}
					}
				}
				if (vector != default(Vector))
				{
					client.ScheduleXI2Input(new RawMouseWheelEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, ev.Position, vector, ev.Modifiers));
				}
				if (_pointerDevice.HasMotion(ev))
				{
					client.ScheduleXI2Input(new RawPointerEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, RawPointerEventType.Move, ev.Position, ev.Modifiers));
				}
			}
			if (ev.Type == XiEventType.XI_ButtonPress && ev.Button >= 4 && ev.Button <= 7 && !ev.Emulated)
			{
				Vector? vector2 = ev.Button switch
				{
					4 => new Vector(0.0, 1.0), 
					5 => new Vector(0.0, -1.0), 
					6 => new Vector(1.0, 0.0), 
					7 => new Vector(-1.0, 0.0), 
					_ => null, 
				};
				if (vector2.HasValue)
				{
					client.ScheduleXI2Input(new RawMouseWheelEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, ev.Position, vector2.Value, ev.Modifiers));
				}
			}
			if (ev.Type == XiEventType.XI_ButtonPress || ev.Type == XiEventType.XI_ButtonRelease)
			{
				bool flag = ev.Type == XiEventType.XI_ButtonPress;
				RawPointerEventType? rawPointerEventType = ev.Button switch
				{
					1 => flag ? RawPointerEventType.LeftButtonDown : RawPointerEventType.LeftButtonUp, 
					2 => flag ? RawPointerEventType.MiddleButtonDown : RawPointerEventType.MiddleButtonUp, 
					3 => flag ? RawPointerEventType.RightButtonDown : RawPointerEventType.RightButtonUp, 
					8 => flag ? RawPointerEventType.XButton1Down : RawPointerEventType.XButton1Up, 
					9 => flag ? RawPointerEventType.XButton2Down : RawPointerEventType.XButton2Up, 
					_ => null, 
				};
				if (rawPointerEventType.HasValue)
				{
					client.ScheduleXI2Input(new RawPointerEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, rawPointerEventType.Value, ev.Position, ev.Modifiers));
				}
			}
			_pointerDevice.UpdateValuators(ev.Valuators);
		}
	}
}
