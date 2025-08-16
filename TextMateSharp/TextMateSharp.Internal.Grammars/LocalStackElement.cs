namespace TextMateSharp.Internal.Grammars;

internal class LocalStackElement
{
	public AttributedScopeStack Scopes { get; private set; }

	public int EndPos { get; private set; }

	public LocalStackElement(AttributedScopeStack scopes, int endPos)
	{
		Scopes = scopes;
		EndPos = endPos;
	}
}
