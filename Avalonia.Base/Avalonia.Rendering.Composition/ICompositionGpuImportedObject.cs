using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Rendering.Composition;

[NotClientImplementable]
public interface ICompositionGpuImportedObject : IAsyncDisposable
{
	Task ImportCompleted { get; }

	[Obsolete("Please use ICompositionGpuImportedObject.ImportCompleted instead")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	Task ImportCompeted { get; }

	bool IsLost { get; }
}
