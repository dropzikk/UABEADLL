using System;
using Avalonia.Reactive;

namespace Avalonia.Input.TextInput;

internal class TextInputMethodManager
{
	private ITextInputMethodImpl? _im;

	private IInputElement? _focusedElement;

	private TextInputMethodClient? _client;

	private readonly TransformTrackingHelper _transformTracker = new TransformTrackingHelper();

	private TextInputMethodClient? Client
	{
		get
		{
			return _client;
		}
		set
		{
			if (_client == value)
			{
				return;
			}
			if (_client != null)
			{
				_client.CursorRectangleChanged -= OnCursorRectangleChanged;
				_client.TextViewVisualChanged -= OnTextViewVisualChanged;
				_client = null;
				_im?.Reset();
			}
			_client = value;
			if (_client != null)
			{
				_client.CursorRectangleChanged += OnCursorRectangleChanged;
				_client.TextViewVisualChanged += OnTextViewVisualChanged;
				if (_focusedElement is StyledElement avaloniaObject)
				{
					_im?.SetOptions(TextInputOptions.FromStyledElement(avaloniaObject));
				}
				else
				{
					_im?.SetOptions(TextInputOptions.Default);
				}
				_transformTracker.SetVisual(_client?.TextViewVisual);
				_im?.SetClient(_client);
				UpdateCursorRect();
			}
			else
			{
				_im?.SetClient(null);
				_transformTracker.SetVisual(null);
			}
		}
	}

	public TextInputMethodManager()
	{
		_transformTracker.MatrixChanged += UpdateCursorRect;
		InputMethod.IsInputMethodEnabledProperty.Changed.Subscribe(OnIsInputMethodEnabledChanged);
	}

	private void OnIsInputMethodEnabledChanged(AvaloniaPropertyChangedEventArgs<bool> obj)
	{
		if (obj.Sender == _focusedElement)
		{
			TryFindAndApplyClient();
		}
	}

	private void OnTextViewVisualChanged(object? sender, EventArgs e)
	{
		_transformTracker.SetVisual(_client?.TextViewVisual);
	}

	private void UpdateCursorRect()
	{
		if (_im != null && _client != null && _focusedElement is Visual { VisualRoot: Visual visualRoot } visual)
		{
			Matrix? matrix = visual.TransformToVisual(visualRoot);
			if (!matrix.HasValue)
			{
				_im.SetCursorRect(default(Rect));
			}
			else
			{
				_im.SetCursorRect(_client.CursorRectangle.TransformToAABB(matrix.Value));
			}
		}
	}

	private void OnCursorRectangleChanged(object? sender, EventArgs e)
	{
		if (sender == _client)
		{
			UpdateCursorRect();
		}
	}

	public void SetFocusedElement(IInputElement? element)
	{
		if (_focusedElement != element)
		{
			_focusedElement = element;
			ITextInputMethodImpl textInputMethodImpl = ((element as Visual)?.VisualRoot as ITextInputMethodRoot)?.InputMethod;
			if (_im != textInputMethodImpl)
			{
				_im?.SetClient(null);
			}
			_im = textInputMethodImpl;
			TryFindAndApplyClient();
		}
	}

	private void TryFindAndApplyClient()
	{
		if (!(_focusedElement is InputElement target) || _im == null || !InputMethod.GetIsInputMethodEnabled(target))
		{
			Client = null;
			return;
		}
		TextInputMethodClientRequestedEventArgs textInputMethodClientRequestedEventArgs = new TextInputMethodClientRequestedEventArgs
		{
			RoutedEvent = InputElement.TextInputMethodClientRequestedEvent
		};
		_focusedElement.RaiseEvent(textInputMethodClientRequestedEventArgs);
		Client = textInputMethodClientRequestedEventArgs.Client;
	}
}
