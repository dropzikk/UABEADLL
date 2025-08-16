using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Data.Converters;

public static class BoolConverters
{
	public static readonly IMultiValueConverter And = new FuncMultiValueConverter<bool, bool>((IEnumerable<bool> x) => x.All((bool y) => y));

	public static readonly IMultiValueConverter Or = new FuncMultiValueConverter<bool, bool>((IEnumerable<bool> x) => x.Any((bool y) => y));

	public static readonly IValueConverter Not = new FuncValueConverter<bool, bool>((bool x) => !x);
}
