using Avalonia.Metadata;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface INameScope
{
	bool IsCompleted { get; }

	void Register(string name, object element);

	SynchronousCompletionAsyncResult<object?> FindAsync(string name);

	object? Find(string name);

	void Complete();
}
