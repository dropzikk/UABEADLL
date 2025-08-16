using System;

namespace Avalonia.Media;

internal interface IAffectsRender
{
	event EventHandler Invalidated;
}
