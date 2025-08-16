using System;
using Avalonia.Rendering;

namespace Avalonia.OpenGL.Controls;

internal interface IGlSwapchainImage : ISwapchainImage, IAsyncDisposable, IGlTexture
{
}
