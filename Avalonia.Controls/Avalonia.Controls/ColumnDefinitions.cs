using System.Linq;

namespace Avalonia.Controls;

public class ColumnDefinitions : DefinitionList<ColumnDefinition>
{
	public ColumnDefinitions()
	{
	}

	public ColumnDefinitions(string s)
		: this()
	{
		AddRange(from x in GridLength.ParseLengths(s)
			select new ColumnDefinition(x));
	}

	public override string ToString()
	{
		return string.Join(",", this.Select((ColumnDefinition x) => x.Width));
	}

	public static ColumnDefinitions Parse(string s)
	{
		return new ColumnDefinitions(s);
	}
}
