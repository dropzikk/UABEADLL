using System.Collections.Generic;
using System.Text;
using TextMateSharp.Grammars;

namespace TextMateSharp.Internal.Grammars;

internal class Token : IToken
{
	public int StartIndex { get; set; }

	public int EndIndex { get; private set; }

	public int Length => EndIndex - StartIndex;

	public List<string> Scopes { get; private set; }

	public Token(int startIndex, int endIndex, List<string> scopes)
	{
		StartIndex = startIndex;
		EndIndex = endIndex;
		Scopes = scopes;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{startIndex: ");
		stringBuilder.Append(StartIndex);
		stringBuilder.Append(", endIndex: ");
		stringBuilder.Append(EndIndex);
		stringBuilder.Append(", scopes: ");
		stringBuilder.Append(string.Join(", ", Scopes));
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}
}
