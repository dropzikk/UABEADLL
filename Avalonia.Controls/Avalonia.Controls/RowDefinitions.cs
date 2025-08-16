using System.Linq;

namespace Avalonia.Controls;

public class RowDefinitions : DefinitionList<RowDefinition>
{
	public RowDefinitions()
	{
	}

	public RowDefinitions(string s)
		: this()
	{
		AddRange(from x in GridLength.ParseLengths(s)
			select new RowDefinition(x));
	}

	public override string ToString()
	{
		return string.Join(",", this.Select((RowDefinition x) => x.Height));
	}

	public static RowDefinitions Parse(string s)
	{
		return new RowDefinitions(s);
	}
}
