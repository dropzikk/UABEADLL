using System;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.Input;

internal struct Imm32CaretManager
{
	private bool _isCaretCreated;

	public void TryCreate(IntPtr hwnd)
	{
		if (!_isCaretCreated)
		{
			_isCaretCreated = UnmanagedMethods.CreateCaret(hwnd, IntPtr.Zero, 2, 2);
		}
	}

	public void TryMove(int x, int y)
	{
		if (_isCaretCreated)
		{
			UnmanagedMethods.SetCaretPos(x, y);
		}
	}

	public void TryDestroy()
	{
		if (_isCaretCreated)
		{
			UnmanagedMethods.DestroyCaret();
			_isCaretCreated = false;
		}
	}
}
