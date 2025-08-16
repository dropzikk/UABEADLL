using System;
using System.Collections.Generic;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class NativeControlHost : Control
{
	private TopLevel? _currentRoot;

	private INativeControlHostImpl? _currentHost;

	private INativeControlHostControlTopLevelAttachment? _attachment;

	private IPlatformHandle? _nativeControlHandle;

	private bool _queuedForDestruction;

	private bool _queuedForMoveResize;

	private readonly List<Visual> _propertyChangedSubscriptions = new List<Visual>();

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		_currentRoot = e.Root as TopLevel;
		for (Visual visual = this; visual != null; visual = visual.GetVisualParent())
		{
			visual.PropertyChanged += PropertyChangedHandler;
			_propertyChangedSubscriptions.Add(visual);
		}
		UpdateHost();
	}

	private void PropertyChangedHandler(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.IsEffectiveValueChange && (e.Property == Visual.BoundsProperty || e.Property == Visual.IsVisibleProperty))
		{
			EnqueueForMoveResize();
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		_currentRoot = null;
		foreach (Visual propertyChangedSubscription in _propertyChangedSubscriptions)
		{
			propertyChangedSubscription.PropertyChanged -= PropertyChangedHandler;
		}
		_propertyChangedSubscriptions.Clear();
		UpdateHost();
	}

	private void UpdateHost()
	{
		_queuedForMoveResize = false;
		_currentHost = _currentRoot?.PlatformImpl?.TryGetFeature<INativeControlHostImpl>();
		if (_currentHost != null)
		{
			if (_attachment != null && _attachment.AttachedTo != _currentHost && _attachment != null)
			{
				if (_attachment.IsCompatibleWith(_currentHost))
				{
					_attachment.AttachedTo = _currentHost;
				}
				else
				{
					_attachment.Dispose();
					_attachment = null;
				}
			}
			if (_attachment == null && _nativeControlHandle != null)
			{
				if (_currentHost.IsCompatibleWith(_nativeControlHandle))
				{
					_attachment = _currentHost.CreateNewAttachment(_nativeControlHandle);
				}
				else
				{
					DestroyNativeControl();
				}
			}
			if (_nativeControlHandle == null)
			{
				_attachment = _currentHost.CreateNewAttachment((IPlatformHandle parent) => _nativeControlHandle = CreateNativeControlCore(parent));
			}
		}
		else
		{
			if (_attachment != null)
			{
				_attachment.AttachedTo = null;
			}
			if (_nativeControlHandle != null && !_queuedForDestruction)
			{
				_queuedForDestruction = true;
				Dispatcher.UIThread.Post(CheckDestruction, DispatcherPriority.Background);
			}
		}
		if (_attachment?.AttachedTo == _currentHost)
		{
			TryUpdateNativeControlPosition();
		}
	}

	private Rect? GetAbsoluteBounds()
	{
		Rect bounds = base.Bounds;
		Point? point = this.TranslatePoint(default(Point), _currentRoot);
		if (!point.HasValue)
		{
			return null;
		}
		return new Rect(point.Value, bounds.Size);
	}

	private void EnqueueForMoveResize()
	{
		if (!_queuedForMoveResize)
		{
			_queuedForMoveResize = true;
			Dispatcher.UIThread.Post(UpdateHost, DispatcherPriority.AfterRender);
		}
	}

	public bool TryUpdateNativeControlPosition()
	{
		if (_currentHost == null)
		{
			return false;
		}
		Rect? absoluteBounds = GetAbsoluteBounds();
		if (base.IsEffectivelyVisible && absoluteBounds.HasValue)
		{
			if (absoluteBounds.Value.Width == 0.0 && absoluteBounds.Value.Height == 0.0)
			{
				return false;
			}
			_attachment?.ShowInBounds(absoluteBounds.Value);
		}
		else
		{
			_attachment?.HideWithSize(base.Bounds.Size);
		}
		return true;
	}

	private void CheckDestruction()
	{
		_queuedForDestruction = false;
		if (_currentRoot == null)
		{
			DestroyNativeControl();
		}
	}

	protected virtual IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
	{
		if (_currentHost == null)
		{
			throw new InvalidOperationException();
		}
		return _currentHost.CreateDefaultChild(parent);
	}

	private void DestroyNativeControl()
	{
		if (_nativeControlHandle != null)
		{
			_attachment?.Dispose();
			_attachment = null;
			DestroyNativeControlCore(_nativeControlHandle);
			_nativeControlHandle = null;
		}
	}

	protected virtual void DestroyNativeControlCore(IPlatformHandle control)
	{
		((INativeControlHostDestroyableControlHandle)control).Destroy();
	}
}
