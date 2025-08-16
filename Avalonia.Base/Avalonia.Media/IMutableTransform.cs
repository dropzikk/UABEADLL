using System;

namespace Avalonia.Media;

public interface IMutableTransform : ITransform
{
	event EventHandler Changed;
}
