namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdProperty : XshdElement
{
	public string Name { get; set; }

	public string Value { get; set; }

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return null;
	}
}
