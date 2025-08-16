using System.Collections.Generic;

namespace Avalonia.Input;

public interface IDataObject
{
	IEnumerable<string> GetDataFormats();

	bool Contains(string dataFormat);

	object? Get(string dataFormat);
}
