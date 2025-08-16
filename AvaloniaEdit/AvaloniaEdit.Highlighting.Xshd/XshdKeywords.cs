using System.Collections.Generic;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdKeywords : XshdElement
{
	private readonly NullSafeCollection<string> _words = new NullSafeCollection<string>();

	public XshdReference<XshdColor> ColorReference { get; set; }

	public IList<string> Words => _words;

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitKeywords(this);
	}
}
