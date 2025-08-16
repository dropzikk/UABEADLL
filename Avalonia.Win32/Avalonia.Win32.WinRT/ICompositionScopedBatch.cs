using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionScopedBatch : IInspectable, IUnknown, IDisposable
{
	int IsActive { get; }

	int IsEnded { get; }

	void End();

	void Resume();

	void Suspend();

	unsafe int AddCompleted(void* handler);

	void RemoveCompleted(int token);
}
