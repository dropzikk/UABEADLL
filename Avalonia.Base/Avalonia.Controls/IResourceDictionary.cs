using System.Collections;
using System.Collections.Generic;
using Avalonia.Styling;

namespace Avalonia.Controls;

public interface IResourceDictionary : IResourceProvider, IResourceNode, IDictionary<object, object?>, ICollection<KeyValuePair<object, object?>>, IEnumerable<KeyValuePair<object, object?>>, IEnumerable
{
	IList<IResourceProvider> MergedDictionaries { get; }

	IDictionary<ThemeVariant, IThemeVariantProvider> ThemeDictionaries { get; }
}
