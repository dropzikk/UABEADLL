using System;
using System.Linq;
using System.Xml;

namespace AvaloniaEdit.Highlighting.Xshd;

public sealed class SaveXshdVisitor : IXshdVisitor
{
	public const string Namespace = "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008";

	private readonly XmlWriter _writer;

	public SaveXshdVisitor(XmlWriter writer)
	{
		_writer = writer ?? throw new ArgumentNullException("writer");
	}

	public void WriteDefinition(XshdSyntaxDefinition definition)
	{
		if (definition == null)
		{
			throw new ArgumentNullException("definition");
		}
		_writer.WriteStartElement("SyntaxDefinition", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (definition.Name != null)
		{
			_writer.WriteAttributeString("name", definition.Name);
		}
		if (definition.Extensions != null)
		{
			_writer.WriteAttributeString("extensions", string.Join(";", definition.Extensions.ToArray()));
		}
		definition.AcceptElements(this);
		_writer.WriteEndElement();
	}

	object IXshdVisitor.VisitRuleSet(XshdRuleSet ruleSet)
	{
		_writer.WriteStartElement("RuleSet", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (ruleSet.Name != null)
		{
			_writer.WriteAttributeString("name", ruleSet.Name);
		}
		WriteBoolAttribute("ignoreCase", ruleSet.IgnoreCase);
		ruleSet.AcceptElements(this);
		_writer.WriteEndElement();
		return null;
	}

	private void WriteBoolAttribute(string attributeName, bool? value)
	{
		if (value.HasValue)
		{
			_writer.WriteAttributeString(attributeName, value.Value ? "true" : "false");
		}
	}

	private void WriteRuleSetReference(XshdReference<XshdRuleSet> ruleSetReference)
	{
		if (ruleSetReference.ReferencedElement != null)
		{
			if (ruleSetReference.ReferencedDefinition != null)
			{
				_writer.WriteAttributeString("ruleSet", ruleSetReference.ReferencedDefinition + "/" + ruleSetReference.ReferencedElement);
			}
			else
			{
				_writer.WriteAttributeString("ruleSet", ruleSetReference.ReferencedElement);
			}
		}
	}

	private void WriteColorReference(XshdReference<XshdColor> color)
	{
		if (color.InlineElement != null)
		{
			WriteColorAttributes(color.InlineElement);
		}
		else if (color.ReferencedElement != null)
		{
			if (color.ReferencedDefinition != null)
			{
				_writer.WriteAttributeString("color", color.ReferencedDefinition + "/" + color.ReferencedElement);
			}
			else
			{
				_writer.WriteAttributeString("color", color.ReferencedElement);
			}
		}
	}

	private void WriteColorAttributes(XshdColor color)
	{
		if (color.Foreground != null)
		{
			_writer.WriteAttributeString("foreground", color.Foreground.ToString());
		}
		if (color.Background != null)
		{
			_writer.WriteAttributeString("background", color.Background.ToString());
		}
		if (color.FontWeight.HasValue)
		{
			_writer.WriteAttributeString("fontWeight", color.FontWeight.Value.ToString().ToLowerInvariant());
		}
		if (color.FontStyle.HasValue)
		{
			_writer.WriteAttributeString("fontStyle", color.FontStyle.Value.ToString().ToLowerInvariant());
		}
	}

	object IXshdVisitor.VisitColor(XshdColor color)
	{
		_writer.WriteStartElement("Color", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (color.Name != null)
		{
			_writer.WriteAttributeString("name", color.Name);
		}
		WriteColorAttributes(color);
		if (color.ExampleText != null)
		{
			_writer.WriteAttributeString("exampleText", color.ExampleText);
		}
		_writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitKeywords(XshdKeywords keywords)
	{
		_writer.WriteStartElement("Keywords", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(keywords.ColorReference);
		foreach (string word in keywords.Words)
		{
			_writer.WriteElementString("Word", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008", word);
		}
		_writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitSpan(XshdSpan span)
	{
		_writer.WriteStartElement("Span", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(span.SpanColorReference);
		if (span.BeginRegexType == XshdRegexType.Default && span.BeginRegex != null)
		{
			_writer.WriteAttributeString("begin", span.BeginRegex);
		}
		if (span.EndRegexType == XshdRegexType.Default && span.EndRegex != null)
		{
			_writer.WriteAttributeString("end", span.EndRegex);
		}
		WriteRuleSetReference(span.RuleSetReference);
		if (span.Multiline)
		{
			_writer.WriteAttributeString("multiline", "true");
		}
		if (span.BeginRegexType == XshdRegexType.IgnorePatternWhitespace)
		{
			WriteBeginEndElement("Begin", span.BeginRegex, span.BeginColorReference);
		}
		if (span.EndRegexType == XshdRegexType.IgnorePatternWhitespace)
		{
			WriteBeginEndElement("End", span.EndRegex, span.EndColorReference);
		}
		span.RuleSetReference.InlineElement?.AcceptVisitor(this);
		_writer.WriteEndElement();
		return null;
	}

	private void WriteBeginEndElement(string elementName, string regex, XshdReference<XshdColor> colorReference)
	{
		if (regex != null)
		{
			_writer.WriteStartElement(elementName, "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
			WriteColorReference(colorReference);
			_writer.WriteString(regex);
			_writer.WriteEndElement();
		}
	}

	object IXshdVisitor.VisitImport(XshdImport import)
	{
		_writer.WriteStartElement("Import", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteRuleSetReference(import.RuleSetReference);
		_writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitRule(XshdRule rule)
	{
		_writer.WriteStartElement("Rule", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(rule.ColorReference);
		_writer.WriteString(rule.Regex);
		_writer.WriteEndElement();
		return null;
	}
}
