using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class XmlnsPrefixAttribute : Attribute
{
	public string XmlNamespace { get; }

	public string Prefix { get; }

	public XmlnsPrefixAttribute(string xmlNamespace, string prefix)
	{
		XmlNamespace = xmlNamespace ?? throw new ArgumentNullException("xmlNamespace");
		Prefix = prefix ?? throw new ArgumentNullException("prefix");
	}
}
