using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Platform.Storage;

namespace Avalonia.Dialogs.Internal;

public class ManagedFileChooserFilterViewModel : AvaloniaDialogsInternalViewModelBase
{
	private readonly Regex[] _patterns;

	public string Name { get; }

	public ManagedFileChooserFilterViewModel(FilePickerFileType filter)
	{
		Name = filter.Name;
		IReadOnlyList<string>? patterns = filter.Patterns;
		if (patterns == null || !patterns.Contains("*.*"))
		{
			_patterns = filter.Patterns?.Select((string e) => new Regex("^" + Regex.Escape(e).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase | RegexOptions.Singleline)).ToArray();
		}
	}

	public bool Match(string filename)
	{
		if (_patterns != null)
		{
			return _patterns.Any((Regex ext) => ext.IsMatch(filename));
		}
		return true;
	}

	public override string ToString()
	{
		return Name;
	}
}
