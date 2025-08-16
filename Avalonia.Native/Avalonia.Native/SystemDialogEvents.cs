using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class SystemDialogEvents : NativeCallbackBase, IAvnSystemDialogEvents, IUnknown, IDisposable
{
	private readonly TaskCompletionSource<string[]> _tcs;

	public Task<string[]> Task => _tcs.Task;

	public SystemDialogEvents()
	{
		_tcs = new TaskCompletionSource<string[]>();
	}

	public unsafe void OnCompleted(int numResults, void* trFirstResultRef)
	{
		string[] array = new string[numResults];
		IntPtr* ptr = (IntPtr*)trFirstResultRef;
		for (int i = 0; i < numResults; i++)
		{
			array[i] = Marshal.PtrToStringAnsi(*ptr) ?? string.Empty;
			ptr++;
		}
		_tcs.SetResult(array);
	}
}
