using System;
using Avalonia.Rendering.Composition;

namespace Avalonia.Rendering;

internal interface IRendererWithCompositor : IRenderer, IDisposable
{
	Compositor Compositor { get; }
}
