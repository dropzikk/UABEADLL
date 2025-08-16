using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class XmlnsDefinitionAttribute : Attribute
{
	public string XmlNamespace { get; }

	public string ClrNamespace { get; }

	public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
	{
		XmlNamespace = xmlNamespace;
		ClrNamespace = clrNamespace;
	}
}
