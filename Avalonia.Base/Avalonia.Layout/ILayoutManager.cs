using System;
using Avalonia.Metadata;

namespace Avalonia.Layout;

[PrivateApi]
public interface ILayoutManager : IDisposable
{
	event EventHandler LayoutUpdated;

	void InvalidateMeasure(Layoutable control);

	void InvalidateArrange(Layoutable control);

	void ExecuteLayoutPass();

	void ExecuteInitialLayoutPass();

	void RegisterEffectiveViewportListener(Layoutable control);

	void UnregisterEffectiveViewportListener(Layoutable control);
}
