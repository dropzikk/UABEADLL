using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls.Embedding.Offscreen;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Input;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Threading;

namespace Avalonia.Controls.Remote.Server;

[Unstable]
internal class RemoteServerTopLevelImpl : OffscreenTopLevelImplBase, IFramebufferPlatformSurface, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private enum FrameStatus
	{
		NotRendered,
		Rendered,
		CopiedToMessage
	}

	private sealed class Framebuffer
	{
		private readonly double _dpi;

		private readonly PixelSize _frameSize;

		private readonly object _dataLock = new object();

		private readonly byte[] _data;

		private readonly byte[] _dataCopy;

		private FrameStatus _status;

		public static Framebuffer Empty { get; } = new Framebuffer(Avalonia.Remote.Protocol.Viewport.PixelFormat.Rgba8888, default(Size), 1.0);

		public Avalonia.Remote.Protocol.Viewport.PixelFormat Format { get; }

		public Size ClientSize { get; }

		public double RenderScaling { get; }

		public int Stride { get; }

		public Framebuffer(Avalonia.Remote.Protocol.Viewport.PixelFormat format, Size clientSize, double renderScaling)
		{
			PixelSize frameSize = PixelSize.FromSize(clientSize, renderScaling);
			if (frameSize.Width <= 0 || frameSize.Height <= 0)
			{
				frameSize = PixelSize.Empty;
			}
			int num = frameSize.Width * ((format == Avalonia.Remote.Protocol.Viewport.PixelFormat.Rgb565) ? 2 : 4);
			int num2 = Math.Max(0, num * frameSize.Height);
			_dpi = renderScaling * 96.0;
			_frameSize = frameSize;
			Format = format;
			ClientSize = clientSize;
			RenderScaling = renderScaling;
			if (num2 <= 0)
			{
				byte[] data = Array.Empty<byte>();
				byte[] dataCopy = Array.Empty<byte>();
				Stride = 0;
				_data = data;
				_dataCopy = dataCopy;
			}
			else
			{
				int num3 = num;
				byte[] dataCopy = new byte[num2];
				byte[] data = new byte[num2];
				Stride = num3;
				_data = dataCopy;
				_dataCopy = data;
			}
		}

		public FrameStatus GetStatus()
		{
			lock (_dataLock)
			{
				return _status;
			}
		}

		public ILockedFramebuffer Lock(Action onUnlocked)
		{
			GCHandle handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
			Monitor.Enter(_dataLock);
			try
			{
				return new LockedFramebuffer(handle.AddrOfPinnedObject(), _frameSize, Stride, new Vector(_dpi, _dpi), new Avalonia.Platform.PixelFormat((PixelFormatEnum)Format), delegate
				{
					handle.Free();
					Array.Copy(_data, _dataCopy, _data.Length);
					_status = FrameStatus.Rendered;
					Monitor.Exit(_dataLock);
					onUnlocked();
				});
			}
			catch
			{
				handle.Free();
				Monitor.Exit(_dataLock);
				throw;
			}
		}

		public FrameMessage ToMessage(long sequenceId)
		{
			lock (_dataLock)
			{
				_status = FrameStatus.CopiedToMessage;
			}
			return new FrameMessage
			{
				SequenceId = sequenceId,
				Data = _dataCopy,
				Format = Format,
				Width = _frameSize.Width,
				Height = _frameSize.Height,
				Stride = Stride,
				DpiX = _dpi,
				DpiY = _dpi
			};
		}
	}

	private readonly IAvaloniaRemoteTransportConnection _transport;

	private readonly object _lock = new object();

	private readonly Action _sendLastFrameIfNeeded;

	private readonly Action _renderAndSendFrameIfNeeded;

	private Framebuffer _framebuffer = Framebuffer.Empty;

	private long _lastSentFrame = -1L;

	private long _lastReceivedFrame = -1L;

	private long _nextFrameNumber = 1L;

	private ClientViewportAllocatedMessage? _pendingAllocation;

	private Avalonia.Remote.Protocol.Viewport.PixelFormat? _format;

	public override IEnumerable<object> Surfaces => new RemoteServerTopLevelImpl[1] { this };

	public override IMouseDevice MouseDevice { get; } = new MouseDevice();

	public IKeyboardDevice KeyboardDevice { get; }

	public RemoteServerTopLevelImpl(IAvaloniaRemoteTransportConnection transport)
	{
		_sendLastFrameIfNeeded = SendLastFrameIfNeeded;
		_renderAndSendFrameIfNeeded = RenderAndSendFrameIfNeeded;
		_transport = transport;
		_transport.OnMessage += OnMessage;
		KeyboardDevice = AvaloniaLocator.Current.GetRequiredService<IKeyboardDevice>();
	}

	private static RawPointerEventType GetAvaloniaEventType(Avalonia.Remote.Protocol.Input.MouseButton button, bool pressed)
	{
		switch (button)
		{
		case Avalonia.Remote.Protocol.Input.MouseButton.Left:
			if (!pressed)
			{
				return RawPointerEventType.LeftButtonUp;
			}
			return RawPointerEventType.LeftButtonDown;
		case Avalonia.Remote.Protocol.Input.MouseButton.Middle:
			if (!pressed)
			{
				return RawPointerEventType.MiddleButtonUp;
			}
			return RawPointerEventType.MiddleButtonDown;
		case Avalonia.Remote.Protocol.Input.MouseButton.Right:
			if (!pressed)
			{
				return RawPointerEventType.RightButtonUp;
			}
			return RawPointerEventType.RightButtonDown;
		default:
			return RawPointerEventType.Move;
		}
	}

	private static RawInputModifiers GetAvaloniaRawInputModifiers(InputModifiers[]? modifiers)
	{
		RawInputModifiers rawInputModifiers = RawInputModifiers.None;
		if (modifiers == null)
		{
			return rawInputModifiers;
		}
		for (int i = 0; i < modifiers.Length; i++)
		{
			switch (modifiers[i])
			{
			case InputModifiers.Control:
				rawInputModifiers |= RawInputModifiers.Control;
				break;
			case InputModifiers.Alt:
				rawInputModifiers |= RawInputModifiers.Alt;
				break;
			case InputModifiers.Shift:
				rawInputModifiers |= RawInputModifiers.Shift;
				break;
			case InputModifiers.Windows:
				rawInputModifiers |= RawInputModifiers.Meta;
				break;
			case InputModifiers.LeftMouseButton:
				rawInputModifiers |= RawInputModifiers.LeftMouseButton;
				break;
			case InputModifiers.MiddleMouseButton:
				rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
				break;
			case InputModifiers.RightMouseButton:
				rawInputModifiers |= RawInputModifiers.RightMouseButton;
				break;
			}
		}
		return rawInputModifiers;
	}

	protected virtual void OnMessage(IAvaloniaRemoteTransportConnection transport, object obj)
	{
		lock (_lock)
		{
			if (!(obj is FrameReceivedMessage frameReceivedMessage))
			{
				ClientRenderInfoMessage clientRenderInfoMessage = obj as ClientRenderInfoMessage;
				if (clientRenderInfoMessage == null)
				{
					if (!(obj is ClientSupportedPixelFormatsMessage clientSupportedPixelFormatsMessage))
					{
						MeasureViewportMessage measureViewportMessage = obj as MeasureViewportMessage;
						if (measureViewportMessage == null)
						{
							if (!(obj is ClientViewportAllocatedMessage pendingAllocation))
							{
								PointerMovedEventMessage pointerMovedEventMessage = obj as PointerMovedEventMessage;
								if (pointerMovedEventMessage == null)
								{
									PointerPressedEventMessage pointerPressedEventMessage = obj as PointerPressedEventMessage;
									if (pointerPressedEventMessage == null)
									{
										PointerReleasedEventMessage pointerReleasedEventMessage = obj as PointerReleasedEventMessage;
										if (pointerReleasedEventMessage == null)
										{
											ScrollEventMessage scrollEventMessage = obj as ScrollEventMessage;
											if (scrollEventMessage == null)
											{
												KeyEventMessage keyEventMessage = obj as KeyEventMessage;
												if (keyEventMessage == null)
												{
													TextInputEventMessage textInputEventMessage = obj as TextInputEventMessage;
													if (textInputEventMessage != null)
													{
														Dispatcher.UIThread.Post(delegate
														{
															Dispatcher.UIThread.RunJobs((int)DispatcherPriority.Input + 1);
															base.Input?.Invoke(new RawTextInputEventArgs(KeyboardDevice, 0uL, base.InputRoot, textInputEventMessage.Text));
														}, DispatcherPriority.Input);
													}
												}
												else
												{
													Dispatcher.UIThread.Post(delegate
													{
														Dispatcher.UIThread.RunJobs((int)DispatcherPriority.Input + 1);
														base.Input?.Invoke(new RawKeyEventArgs(KeyboardDevice, 0uL, base.InputRoot, (!keyEventMessage.IsDown) ? RawKeyEventType.KeyUp : RawKeyEventType.KeyDown, (Avalonia.Input.Key)keyEventMessage.Key, GetAvaloniaRawInputModifiers(keyEventMessage.Modifiers)));
													}, DispatcherPriority.Input);
												}
											}
											else
											{
												Dispatcher.UIThread.Post(delegate
												{
													base.Input?.Invoke(new RawMouseWheelEventArgs(MouseDevice, 0uL, base.InputRoot, new Point(scrollEventMessage.X, scrollEventMessage.Y), new Vector(scrollEventMessage.DeltaX, scrollEventMessage.DeltaY), GetAvaloniaRawInputModifiers(scrollEventMessage.Modifiers)));
												}, DispatcherPriority.Input);
											}
										}
										else
										{
											Dispatcher.UIThread.Post(delegate
											{
												base.Input?.Invoke(new RawPointerEventArgs(MouseDevice, 0uL, base.InputRoot, GetAvaloniaEventType(pointerReleasedEventMessage.Button, pressed: false), new Point(pointerReleasedEventMessage.X, pointerReleasedEventMessage.Y), GetAvaloniaRawInputModifiers(pointerReleasedEventMessage.Modifiers)));
											}, DispatcherPriority.Input);
										}
									}
									else
									{
										Dispatcher.UIThread.Post(delegate
										{
											base.Input?.Invoke(new RawPointerEventArgs(MouseDevice, 0uL, base.InputRoot, GetAvaloniaEventType(pointerPressedEventMessage.Button, pressed: true), new Point(pointerPressedEventMessage.X, pointerPressedEventMessage.Y), GetAvaloniaRawInputModifiers(pointerPressedEventMessage.Modifiers)));
										}, DispatcherPriority.Input);
									}
								}
								else
								{
									Dispatcher.UIThread.Post(delegate
									{
										base.Input?.Invoke(new RawPointerEventArgs(MouseDevice, 0uL, base.InputRoot, RawPointerEventType.Move, new Point(pointerMovedEventMessage.X, pointerMovedEventMessage.Y), GetAvaloniaRawInputModifiers(pointerMovedEventMessage.Modifiers)));
									}, DispatcherPriority.Input);
								}
								return;
							}
							if (_pendingAllocation == null)
							{
								Dispatcher.UIThread.Post(delegate
								{
									ClientViewportAllocatedMessage pendingAllocation2;
									lock (_lock)
									{
										pendingAllocation2 = _pendingAllocation;
										_pendingAllocation = null;
									}
									base.RenderScaling = pendingAllocation2.DpiX / 96.0;
									base.ClientSize = new Size(pendingAllocation2.Width, pendingAllocation2.Height);
									RenderAndSendFrameIfNeeded();
								});
							}
							_pendingAllocation = pendingAllocation;
						}
						else
						{
							Dispatcher.UIThread.Post(delegate
							{
								Size size = Measure(new Size(measureViewportMessage.Width, measureViewportMessage.Height));
								_transport.Send(new MeasureViewportMessage
								{
									Width = size.Width,
									Height = size.Height
								});
							});
						}
					}
					else
					{
						_format = TryGetValidPixelFormat(clientSupportedPixelFormatsMessage.Formats);
						Dispatcher.UIThread.Post(_renderAndSendFrameIfNeeded);
					}
				}
				else
				{
					Dispatcher.UIThread.Post(delegate
					{
						base.RenderScaling = clientRenderInfoMessage.DpiX / 96.0;
						RenderAndSendFrameIfNeeded();
					});
				}
			}
			else
			{
				_lastReceivedFrame = Math.Max(frameReceivedMessage.SequenceId, _lastReceivedFrame);
				Dispatcher.UIThread.Post(_sendLastFrameIfNeeded);
			}
		}
	}

	private static Avalonia.Remote.Protocol.Viewport.PixelFormat? TryGetValidPixelFormat(Avalonia.Remote.Protocol.Viewport.PixelFormat[]? formats)
	{
		if (formats != null)
		{
			foreach (Avalonia.Remote.Protocol.Viewport.PixelFormat pixelFormat in formats)
			{
				if (pixelFormat >= Avalonia.Remote.Protocol.Viewport.PixelFormat.Rgb565 && pixelFormat <= Avalonia.Remote.Protocol.Viewport.PixelFormat.Bgra8888)
				{
					return pixelFormat;
				}
			}
		}
		return null;
	}

	protected virtual Size Measure(Size constraint)
	{
		Layoutable obj = (Layoutable)base.InputRoot;
		obj.Measure(constraint);
		return obj.DesiredSize;
	}

	private Framebuffer GetOrCreateFramebuffer()
	{
		lock (_lock)
		{
			Avalonia.Remote.Protocol.Viewport.PixelFormat? format = _format;
			if (format.HasValue)
			{
				Avalonia.Remote.Protocol.Viewport.PixelFormat valueOrDefault = format.GetValueOrDefault();
				if (_framebuffer.Format != valueOrDefault || _framebuffer.ClientSize != base.ClientSize || _framebuffer.RenderScaling != base.RenderScaling)
				{
					_framebuffer = new Framebuffer(valueOrDefault, base.ClientSize, base.RenderScaling);
				}
			}
			else
			{
				_framebuffer = Framebuffer.Empty;
			}
			return _framebuffer;
		}
	}

	private void SendLastFrameIfNeeded()
	{
		if (base.IsDisposed)
		{
			return;
		}
		Framebuffer framebuffer;
		long lastSentFrame;
		lock (_lock)
		{
			if (_lastReceivedFrame != _lastSentFrame || _framebuffer.GetStatus() != FrameStatus.Rendered)
			{
				return;
			}
			framebuffer = _framebuffer;
			_lastSentFrame = _nextFrameNumber++;
			lastSentFrame = _lastSentFrame;
		}
		_transport.Send(framebuffer.ToMessage(lastSentFrame));
	}

	protected void RenderAndSendFrameIfNeeded()
	{
		if (base.IsDisposed)
		{
			return;
		}
		lock (_lock)
		{
			if (_lastReceivedFrame != _lastSentFrame)
			{
				return;
			}
			Avalonia.Remote.Protocol.Viewport.PixelFormat? format = _format;
			if (!format.HasValue)
			{
				return;
			}
		}
		Framebuffer orCreateFramebuffer = GetOrCreateFramebuffer();
		if (orCreateFramebuffer.Stride > 0)
		{
			base.Paint?.Invoke(new Rect(orCreateFramebuffer.ClientSize));
		}
		SendLastFrameIfNeeded();
	}

	public IFramebufferRenderTarget CreateFramebufferRenderTarget()
	{
		return new FuncFramebufferRenderTarget(() => GetOrCreateFramebuffer().Lock(_sendLastFrameIfNeeded));
	}
}
