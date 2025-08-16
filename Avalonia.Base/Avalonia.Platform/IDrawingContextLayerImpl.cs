using System;

namespace Avalonia.Platform;

public interface IDrawingContextLayerImpl : IRenderTargetBitmapImpl, IBitmapImpl, IDisposable, IRenderTarget
{
	bool CanBlit { get; }

	void Blit(IDrawingContextImpl context);
}
