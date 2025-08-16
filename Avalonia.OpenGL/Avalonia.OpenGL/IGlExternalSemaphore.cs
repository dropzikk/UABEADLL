using System;

namespace Avalonia.OpenGL;

public interface IGlExternalSemaphore : IDisposable
{
	void WaitSemaphore(IGlExternalImageTexture texture);

	void SignalSemaphore(IGlExternalImageTexture texture);
}
