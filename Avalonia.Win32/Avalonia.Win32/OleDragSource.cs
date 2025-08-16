using System;
using System.Linq;
using Avalonia.MicroCom;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class OleDragSource : CallbackBase, IDropSource, IUnknown, IDisposable
{
	private const int DRAGDROP_S_USEDEFAULTCURSORS = 262402;

	private const int DRAGDROP_S_DROP = 262400;

	private const int DRAGDROP_S_CANCEL = 262401;

	private static readonly int[] MOUSE_BUTTONS = new int[3] { 1, 16, 2 };

	public int QueryContinueDrag(int fEscapePressed, int grfKeyState)
	{
		if (fEscapePressed != 0)
		{
			return 262401;
		}
		int num = MOUSE_BUTTONS.Where((int mb) => (grfKeyState & mb) == mb).Count();
		if (num >= 2)
		{
			return 262401;
		}
		if (num == 0)
		{
			return 262400;
		}
		return 0;
	}

	public int GiveFeedback(DropEffect dwEffect)
	{
		return 262402;
	}
}
