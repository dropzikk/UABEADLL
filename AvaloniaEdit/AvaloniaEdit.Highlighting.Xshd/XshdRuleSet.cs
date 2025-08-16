using System.Collections.Generic;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdRuleSet : XshdElement
{
	private readonly NullSafeCollection<XshdElement> _elements = new NullSafeCollection<XshdElement>();

	public string Name { get; set; }

	public bool? IgnoreCase { get; set; }

	public IList<XshdElement> Elements => _elements;

	public void AcceptElements(IXshdVisitor visitor)
	{
		foreach (XshdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
	}

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitRuleSet(this);
	}
}
