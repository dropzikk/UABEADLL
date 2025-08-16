using Avalonia.Metadata;

namespace Avalonia.Rendering;

[NotClientImplementable]
internal interface IRenderLoop
{
	bool RunsInBackground { get; }

	void Add(IRenderLoopTask i);

	void Remove(IRenderLoopTask i);
}
