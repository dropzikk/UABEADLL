using System.Collections.Generic;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdSyntaxDefinition
{
	public string Name { get; set; }

	public IList<string> Extensions { get; }

	public IList<XshdElement> Elements { get; }

	public XshdSyntaxDefinition()
	{
		Elements = new NullSafeCollection<XshdElement>();
		Extensions = new NullSafeCollection<string>();
	}

	public void AcceptElements(IXshdVisitor visitor)
	{
		foreach (XshdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
	}
}
