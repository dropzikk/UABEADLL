using System;
using Avalonia.Input.TextInput;
using Avalonia.Native.Interop;
using Avalonia.Rendering;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvaloniaNativeTextInputMethod : ITextInputMethodImpl, IDisposable
{
	private class AvnTextInputMethodClient : NativeCallbackBase, IAvnTextInputMethodClient, IUnknown, IDisposable
	{
		private readonly TextInputMethodClient _client;

		public AvnTextInputMethodClient(TextInputMethodClient client)
		{
			_client = client;
		}

		public void SetPreeditText(string preeditText)
		{
			if (_client.SupportsPreedit)
			{
				_client.SetPreeditText(preeditText);
			}
		}

		public void SelectInSurroundingText(int start, int end)
		{
			if (_client.SupportsSurroundingText)
			{
				_client.Selection = new TextSelection(start, end);
			}
		}
	}

	private TextInputMethodClient? _client;

	private IAvnTextInputMethodClient? _nativeClient;

	private readonly IAvnTextInputMethod _inputMethod;

	public AvaloniaNativeTextInputMethod(IAvnWindowBase nativeWindow)
	{
		_inputMethod = nativeWindow.InputMethod;
	}

	public void Dispose()
	{
		_inputMethod.Dispose();
		_nativeClient?.Dispose();
	}

	public void Reset()
	{
		_inputMethod.Reset();
	}

	public void SetClient(TextInputMethodClient? client)
	{
		TextInputMethodClient client2 = _client;
		if (client2 != null && client2.SupportsSurroundingText)
		{
			_client.SurroundingTextChanged -= OnSurroundingTextChanged;
			_client.CursorRectangleChanged -= OnCursorRectangleChanged;
			_nativeClient?.Dispose();
		}
		_nativeClient = null;
		_client = client;
		if (_client != null)
		{
			_nativeClient = new AvnTextInputMethodClient(_client);
			OnSurroundingTextChanged(this, EventArgs.Empty);
			OnCursorRectangleChanged(this, EventArgs.Empty);
			_client.SurroundingTextChanged += OnSurroundingTextChanged;
			_client.CursorRectangleChanged += OnCursorRectangleChanged;
		}
		_inputMethod.SetClient(_nativeClient);
	}

	private void OnCursorRectangleChanged(object? sender, EventArgs e)
	{
		if (_client == null)
		{
			return;
		}
		Visual textViewVisual = _client.TextViewVisual;
		if (textViewVisual == null)
		{
			return;
		}
		IRenderRoot visualRoot = textViewVisual.VisualRoot;
		if (visualRoot != null)
		{
			Matrix? matrix = textViewVisual.TransformToVisual((Visual)visualRoot);
			if (matrix.HasValue)
			{
				Rect rect = _client.CursorRectangle.TransformToAABB(matrix.Value);
				_inputMethod.SetCursorRect(rect.ToAvnRect());
			}
		}
	}

	private void OnSurroundingTextChanged(object? sender, EventArgs e)
	{
		if (_client != null)
		{
			string surroundingText = _client.SurroundingText;
			TextSelection selection = _client.Selection;
			_inputMethod.SetSurroundingText(surroundingText ?? "", selection.Start, selection.End);
		}
	}

	public void SetCursorRect(Rect rect)
	{
		_inputMethod.SetCursorRect(rect.ToAvnRect());
	}

	public void SetOptions(TextInputOptions options)
	{
	}
}
