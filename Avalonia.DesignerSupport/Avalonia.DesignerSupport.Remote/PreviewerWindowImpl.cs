using System;
using Avalonia.Controls;
using Avalonia.Controls.Remote.Server;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Threading;

namespace Avalonia.DesignerSupport.Remote;

internal class PreviewerWindowImpl : RemoteServerTopLevelImpl, IWindowImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private readonly IAvaloniaRemoteTransportConnection _transport;

	public double DesktopScaling => 1.0;

	public PixelPoint Position { get; set; }

	public Action<PixelPoint> PositionChanged { get; set; }

	public Action Deactivated { get; set; }

	public Action Activated { get; set; }

	public Func<WindowCloseReason, bool> Closing { get; set; }

	public IPlatformHandle Handle { get; }

	public WindowState WindowState { get; set; }

	public Action<WindowState> WindowStateChanged { get; set; }

	public Size MaxAutoSizeHint { get; } = new Size(4096.0, 4096.0);

	public IScreenImpl Screen { get; } = new ScreenStub();

	public Action GotInputWhenDisabled { get; set; }

	public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

	public Thickness ExtendedMargins { get; }

	public bool IsClientAreaExtendedToDecorations { get; }

	public Thickness OffScreenMargin { get; }

	public bool NeedsManagedDecorations => false;

	public PreviewerWindowImpl(IAvaloniaRemoteTransportConnection transport)
		: base(transport)
	{
		_transport = transport;
		base.ClientSize = new Size(1.0, 1.0);
	}

	public void Show(bool activate, bool isDialog)
	{
	}

	public void Hide()
	{
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
	}

	protected override void OnMessage(IAvaloniaRemoteTransportConnection transport, object obj)
	{
		ClientViewportAllocatedMessage alloc = obj as ClientViewportAllocatedMessage;
		if (alloc != null)
		{
			Dispatcher.UIThread.Post(delegate
			{
				base.RenderScaling = alloc.DpiX / 96.0;
				RenderAndSendFrameIfNeeded();
			});
		}
		else
		{
			base.OnMessage(transport, obj);
		}
	}

	public void Resize(Size clientSize, WindowResizeReason reason)
	{
		_transport.Send(new RequestViewportResizeMessage
		{
			Width = Math.Ceiling(clientSize.Width * base.RenderScaling),
			Height = Math.Ceiling(clientSize.Height * base.RenderScaling)
		});
		base.ClientSize = clientSize;
		RenderAndSendFrameIfNeeded();
	}

	public void Move(PixelPoint point)
	{
	}

	public void SetMinMaxSize(Size minSize, Size maxSize)
	{
	}

	public override object TryGetFeature(Type featureType)
	{
		if (featureType == typeof(IStorageProvider))
		{
			return new NoopStorageProvider();
		}
		return base.TryGetFeature(featureType);
	}

	public void Activate()
	{
	}

	public void SetTitle(string title)
	{
	}

	public void SetSystemDecorations(SystemDecorations enabled)
	{
	}

	public void SetIcon(IWindowIconImpl icon)
	{
	}

	public void ShowTaskbarIcon(bool value)
	{
	}

	public void CanResize(bool value)
	{
	}

	public void SetTopmost(bool value)
	{
	}

	public void SetParent(IWindowImpl parent)
	{
	}

	public void SetEnabled(bool enable)
	{
	}

	public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
	{
	}

	public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
	{
	}

	public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
	{
	}
}
