using System;
using System.Text.RegularExpressions;

namespace AvaloniaEdit.Rendering;

internal sealed class MailLinkElementGenerator : LinkElementGenerator
{
	public MailLinkElementGenerator()
		: base(LinkElementGenerator.DefaultMailRegex)
	{
	}

	protected override Uri GetUriFromMatch(Match match)
	{
		string uriString = "mailto:" + match.Value;
		if (!Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
		{
			return null;
		}
		return new Uri(uriString);
	}
}
