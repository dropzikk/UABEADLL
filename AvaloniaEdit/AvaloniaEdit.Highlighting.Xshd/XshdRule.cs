namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdRule : XshdElement
{
	public string Regex { get; set; }

	public XshdRegexType RegexType { get; set; }

	public XshdReference<XshdColor> ColorReference { get; set; }

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitRule(this);
	}
}
