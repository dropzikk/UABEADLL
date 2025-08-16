using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class PopupImpl : WindowImpl, IPopupImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private readonly IWindowBaseImpl? _parent;

	private bool _dropShadowHint = true;

	private Size? _maxAutoSize;

	[ThreadStatic]
	private static IntPtr s_parentHandle;

	protected override bool ShouldTakeFocusOnClick => false;

	public override Size MaxAutoSizeHint
	{
		get
		{
			Size? maxAutoSize = _maxAutoSize;
			if (!maxAutoSize.HasValue)
			{
				IntPtr intPtr = UnmanagedMethods.MonitorFromWindow(base.Hwnd, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST);
				if (intPtr != IntPtr.Zero)
				{
					UnmanagedMethods.MONITORINFO lpmi = UnmanagedMethods.MONITORINFO.Create();
					UnmanagedMethods.GetMonitorInfo(intPtr, ref lpmi);
					_maxAutoSize = lpmi.rcWork.ToPixelRect().ToRect(base.RenderScaling).Size;
				}
			}
			return _maxAutoSize ?? Size.Infinity;
		}
	}

	public IPopupPositioner PopupPositioner { get; }

	public override void Show(bool activate, bool isDialog)
	{
		UnmanagedMethods.ShowWindow(base.Handle.Handle, UnmanagedMethods.ShowWindowCommand.ShowNoActivate);
		IWindowBaseImpl parent = _parent;
		while (parent != null && parent is PopupImpl popupImpl)
		{
			parent = popupImpl._parent;
		}
		if (parent != null)
		{
			IntPtr focus = UnmanagedMethods.GetFocus();
			if (focus != IntPtr.Zero && UnmanagedMethods.GetAncestor(focus, UnmanagedMethods.GetAncestorFlags.GA_ROOT) == parent.Handle.Handle)
			{
				UnmanagedMethods.SetFocus(parent.Handle.Handle);
			}
		}
	}

	protected override IntPtr CreateWindowOverride(ushort atom)
	{
		UnmanagedMethods.WindowStyles dwStyle = UnmanagedMethods.WindowStyles.WS_CLIPCHILDREN | UnmanagedMethods.WindowStyles.WS_CLIPSIBLINGS | UnmanagedMethods.WindowStyles.WS_POPUP;
		IntPtr intPtr = UnmanagedMethods.CreateWindowEx(136, atom, null, (uint)dwStyle, int.MinValue, int.MinValue, int.MinValue, int.MinValue, s_parentHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		s_parentHandle = IntPtr.Zero;
		EnableBoxShadow(intPtr, _dropShadowHint);
		return intPtr;
	}

	protected override IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		switch ((UnmanagedMethods.WindowsMessage)msg)
		{
		case UnmanagedMethods.WindowsMessage.WM_DISPLAYCHANGE:
			_maxAutoSize = null;
			break;
		case UnmanagedMethods.WindowsMessage.WM_MOUSEACTIVATE:
			return (IntPtr)3;
		}
		return base.WndProc(hWnd, msg, wParam, lParam);
	}

	private static IWindowBaseImpl SaveParentHandle(IWindowBaseImpl parent)
	{
		s_parentHandle = parent.Handle.Handle;
		return parent;
	}

	public PopupImpl(IWindowBaseImpl parent)
		: this(SaveParentHandle(parent), dummy: false)
	{
	}

	private PopupImpl(IWindowBaseImpl parent, bool dummy)
	{
		_parent = parent;
		PopupPositioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(parent, MoveResize));
	}

	private void MoveResize(PixelPoint position, Size size, double scaling)
	{
		Move(position);
		Resize(size, WindowResizeReason.Layout);
	}

	private static void EnableBoxShadow(IntPtr hwnd, bool enabled)
	{
		int num = (int)UnmanagedMethods.GetClassLongPtr(hwnd, -26);
		num = ((!enabled) ? (num & -131073) : (num | 0x20000));
		UnmanagedMethods.SetClassLong(hwnd, UnmanagedMethods.ClassLongIndex.GCL_STYLE, new IntPtr(num));
	}

	public void SetWindowManagerAddShadowHint(bool enabled)
	{
		_dropShadowHint = enabled;
		EnableBoxShadow(base.Handle.Handle, enabled);
	}
}
