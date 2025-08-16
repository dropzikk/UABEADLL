using System;
using System.Runtime.InteropServices;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Threading;

namespace Avalonia.Controls.Remote;

public class RemoteWidget : Control
{
	public enum SizingMode
	{
		Local,
		Remote
	}

	private readonly IAvaloniaRemoteTransportConnection _connection;

	private FrameMessage? _lastFrame;

	private WriteableBitmap? _bitmap;

	public SizingMode Mode { get; set; }

	public RemoteWidget(IAvaloniaRemoteTransportConnection connection)
	{
		Mode = SizingMode.Local;
		_connection = connection;
		_connection.OnMessage += delegate(IAvaloniaRemoteTransportConnection t, object msg)
		{
			Dispatcher.UIThread.Post(delegate
			{
				OnMessage(msg);
			});
		};
		_connection.Send(new ClientSupportedPixelFormatsMessage
		{
			Formats = new Avalonia.Remote.Protocol.Viewport.PixelFormat[2]
			{
				Avalonia.Remote.Protocol.Viewport.PixelFormat.Bgra8888,
				Avalonia.Remote.Protocol.Viewport.PixelFormat.Rgba8888
			}
		});
	}

	private void OnMessage(object msg)
	{
		if (msg is FrameMessage frameMessage)
		{
			_connection.Send(new FrameReceivedMessage
			{
				SequenceId = frameMessage.SequenceId
			});
			_lastFrame = frameMessage;
			InvalidateVisual();
		}
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		if (Mode == SizingMode.Local)
		{
			_connection.Send(new ClientViewportAllocatedMessage
			{
				Width = finalRect.Width,
				Height = finalRect.Height,
				DpiX = 960.0,
				DpiY = 960.0
			});
		}
		base.ArrangeCore(finalRect);
	}

	public sealed override void Render(DrawingContext context)
	{
		if (_lastFrame != null && _lastFrame.Width != 0 && _lastFrame.Height != 0)
		{
			Avalonia.Platform.PixelFormat pixelFormat = new Avalonia.Platform.PixelFormat((PixelFormatEnum)_lastFrame.Format);
			if (_bitmap == null || _bitmap.PixelSize.Width != _lastFrame.Width || _bitmap.PixelSize.Height != _lastFrame.Height)
			{
				_bitmap?.Dispose();
				_bitmap = new WriteableBitmap(new PixelSize(_lastFrame.Width, _lastFrame.Height), new Vector(96.0, 96.0), pixelFormat, null);
			}
			using (ILockedFramebuffer lockedFramebuffer = _bitmap.Lock())
			{
				int length = ((pixelFormat == Avalonia.Platform.PixelFormat.Rgb565) ? 2 : 4) * _lastFrame.Width;
				for (int i = 0; i < _lastFrame.Height; i++)
				{
					Marshal.Copy(_lastFrame.Data, i * _lastFrame.Stride, new IntPtr(lockedFramebuffer.Address.ToInt64() + lockedFramebuffer.RowBytes * i), length);
				}
			}
			context.DrawImage(_bitmap, new Rect(0.0, 0.0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height), new Rect(base.Bounds.Size));
		}
		base.Render(context);
	}
}
