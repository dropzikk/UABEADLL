using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class TrayIconImpl : ITrayIconImpl, IDisposable
{
	private enum CustomWindowsMessage : uint
	{
		WM_TRAYICON = 33792u,
		WM_TRAYMOUSE = 2048u
	}

	private class TrayIconMenuFlyoutPresenter : MenuFlyoutPresenter
	{
		protected override Type StyleKeyOverride => typeof(MenuFlyoutPresenter);

		public override void Close()
		{
			TrayPopupRoot trayPopupRoot = this.FindLogicalAncestorOfType<TrayPopupRoot>();
			if (trayPopupRoot != null)
			{
				base.SelectedIndex = -1;
				trayPopupRoot.Close();
			}
		}
	}

	private class TrayPopupRoot : Window
	{
		private class TrayIconManagedPopupPositionerPopupImplHelper : IManagedPopupPositionerPopup
		{
			private readonly Action<PixelPoint, Size, double> _moveResize;

			private readonly Window _hiddenWindow;

			public IReadOnlyList<ManagedPopupPositionerScreenInfo> Screens => _hiddenWindow.Screens.All.Select((Screen s) => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1.0), s.Bounds.ToRect(1.0))).ToArray();

			public Rect ParentClientAreaScreenGeometry
			{
				get
				{
					Screen primary = _hiddenWindow.Screens.Primary;
					if (primary != null)
					{
						PixelPoint topLeft = primary.Bounds.TopLeft;
						PixelSize size = primary.Bounds.Size;
						return new Rect(topLeft.X, topLeft.Y, (double)size.Width * primary.Scaling, (double)size.Height * primary.Scaling);
					}
					return default(Rect);
				}
			}

			public double Scaling => _hiddenWindow.Screens.Primary?.Scaling ?? 1.0;

			public TrayIconManagedPopupPositionerPopupImplHelper(Action<PixelPoint, Size, double> moveResize)
			{
				_moveResize = moveResize;
				_hiddenWindow = new Window();
			}

			public void MoveAndResize(Point devicePoint, Size virtualSize)
			{
				_moveResize(new PixelPoint((int)devicePoint.X, (int)devicePoint.Y), virtualSize, Scaling);
			}
		}

		private readonly ManagedPopupPositioner _positioner;

		public TrayPopupRoot()
		{
			_positioner = new ManagedPopupPositioner(new TrayIconManagedPopupPositionerPopupImplHelper(MoveResize));
			base.Topmost = true;
			base.Deactivated += TrayPopupRoot_Deactivated;
			base.ShowInTaskbar = false;
			base.ShowActivated = true;
		}

		private void TrayPopupRoot_Deactivated(object? sender, EventArgs e)
		{
			Close();
		}

		private void MoveResize(PixelPoint position, Size size, double scaling)
		{
			IWindowImpl platformImpl = base.PlatformImpl;
			if (platformImpl != null)
			{
				platformImpl.Move(position);
				platformImpl.Resize(size, WindowResizeReason.Layout);
			}
		}

		protected override void ArrangeCore(Rect finalRect)
		{
			base.ArrangeCore(finalRect);
			_positioner.Update(new PopupPositionerParameters
			{
				Anchor = PopupAnchor.TopLeft,
				Gravity = PopupGravity.BottomRight,
				AnchorRectangle = new Rect(base.Position.ToPoint(base.Screens.Primary?.Scaling ?? 1.0), new Size(1.0, 1.0)),
				Size = finalRect.Size,
				ConstraintAdjustment = (PopupPositionerConstraintAdjustment.FlipX | PopupPositionerConstraintAdjustment.FlipY)
			});
		}
	}

	private static readonly IntPtr s_emptyIcon = new Bitmap(32, 32).GetHicon();

	private readonly int _uniqueId;

	private static int s_nextUniqueId;

	private bool _iconAdded;

	private IconImpl? _icon;

	private string? _tooltipText;

	private readonly Win32NativeToManagedMenuExporter _exporter;

	private static readonly Dictionary<int, TrayIconImpl> s_trayIcons = new Dictionary<int, TrayIconImpl>();

	private bool _disposedValue;

	private static readonly uint WM_TASKBARCREATED = UnmanagedMethods.RegisterWindowMessage("TaskbarCreated");

	public Action? OnClicked { get; set; }

	public INativeMenuExporter MenuExporter => _exporter;

	public TrayIconImpl()
	{
		_exporter = new Win32NativeToManagedMenuExporter();
		_uniqueId = ++s_nextUniqueId;
		s_trayIcons.Add(_uniqueId, this);
	}

	internal static void ProcWnd(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		if (msg == 2048 && s_trayIcons.TryGetValue(wParam.ToInt32(), out TrayIconImpl value))
		{
			value.WndProc(hWnd, msg, wParam, lParam);
		}
		if (msg != WM_TASKBARCREATED)
		{
			return;
		}
		foreach (TrayIconImpl value2 in s_trayIcons.Values)
		{
			if (value2._iconAdded)
			{
				value2.UpdateIcon(remove: true);
				value2.UpdateIcon();
			}
		}
	}

	public void SetIcon(IWindowIconImpl? icon)
	{
		_icon = icon as IconImpl;
		UpdateIcon();
	}

	public void SetIsVisible(bool visible)
	{
		UpdateIcon(!visible);
	}

	public void SetToolTipText(string? text)
	{
		_tooltipText = text;
		UpdateIcon(!_iconAdded);
	}

	private void UpdateIcon(bool remove = false)
	{
		NOTIFYICONDATA nOTIFYICONDATA = new NOTIFYICONDATA
		{
			hWnd = Win32Platform.Instance.Handle,
			uID = _uniqueId
		};
		if (!remove)
		{
			nOTIFYICONDATA.uFlags = NIF.MESSAGE | NIF.ICON | NIF.TIP;
			nOTIFYICONDATA.uCallbackMessage = 2048;
			nOTIFYICONDATA.hIcon = _icon?.HIcon ?? s_emptyIcon;
			nOTIFYICONDATA.szTip = _tooltipText ?? "";
			if (!_iconAdded)
			{
				UnmanagedMethods.Shell_NotifyIcon(NIM.ADD, nOTIFYICONDATA);
				_iconAdded = true;
			}
			else
			{
				UnmanagedMethods.Shell_NotifyIcon(NIM.MODIFY, nOTIFYICONDATA);
			}
		}
		else
		{
			nOTIFYICONDATA.uFlags = (NIF)0u;
			UnmanagedMethods.Shell_NotifyIcon(NIM.DELETE, nOTIFYICONDATA);
			_iconAdded = false;
		}
	}

	private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		if (msg == 2048)
		{
			switch (lParam.ToInt32())
			{
			case 514:
				OnClicked?.Invoke();
				break;
			case 517:
				OnRightClicked();
				break;
			}
			return IntPtr.Zero;
		}
		return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
	}

	private void OnRightClicked()
	{
		AvaloniaList<MenuItem> menu = _exporter.GetMenu();
		if (menu != null && menu.Count != 0)
		{
			TrayPopupRoot trayPopupRoot = new TrayPopupRoot();
			trayPopupRoot.SystemDecorations = SystemDecorations.None;
			trayPopupRoot.SizeToContent = SizeToContent.WidthAndHeight;
			trayPopupRoot.Background = null;
			trayPopupRoot.TransparencyLevelHint = new WindowTransparencyLevel[1] { WindowTransparencyLevel.Transparent };
			trayPopupRoot.Content = new TrayIconMenuFlyoutPresenter
			{
				ItemsSource = menu
			};
			UnmanagedMethods.GetCursorPos(out var lpPoint);
			trayPopupRoot.Position = new PixelPoint(lpPoint.X, lpPoint.Y);
			trayPopupRoot.Show();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			UpdateIcon(remove: true);
			_disposedValue = true;
		}
	}

	~TrayIconImpl()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
