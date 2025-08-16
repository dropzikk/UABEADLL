using Avalonia.Metadata;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface IPseudoClasses
{
	void Add(string name);

	bool Remove(string name);

	bool Contains(string name);
}
