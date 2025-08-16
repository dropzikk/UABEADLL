using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Utilities;

namespace Avalonia.Media;

public class TextDecorationCollection : AvaloniaList<TextDecoration>
{
	public TextDecorationCollection()
	{
	}

	public TextDecorationCollection(IEnumerable<TextDecoration> textDecorations)
		: base(textDecorations)
	{
	}

	public static TextDecorationCollection Parse(string s)
	{
		List<TextDecorationLocation> list = new List<TextDecorationLocation>();
		using (StringTokenizer stringTokenizer = new StringTokenizer(s, ',', "Invalid text decoration."))
		{
			string result;
			while (stringTokenizer.TryReadString(out result, null))
			{
				TextDecorationLocation textDecorationLocation = GetTextDecorationLocation(result);
				if (list.Contains(textDecorationLocation))
				{
					throw new ArgumentException("Text decoration already specified.", "s");
				}
				list.Add(textDecorationLocation);
			}
		}
		TextDecorationCollection textDecorationCollection = new TextDecorationCollection();
		foreach (TextDecorationLocation item in list)
		{
			textDecorationCollection.Add(new TextDecoration
			{
				Location = item
			});
		}
		return textDecorationCollection;
	}

	private static TextDecorationLocation GetTextDecorationLocation(string s)
	{
		if (Enum.TryParse<TextDecorationLocation>(s, ignoreCase: true, out var result))
		{
			return result;
		}
		throw new ArgumentException("Could not parse text decoration.", "s");
	}
}
