using System;
using System.Collections.Generic;
using System.Xml;
using Avalonia.Media;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting.Xshd;

internal static class V2Loader
{
	public const string Namespace = "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008";

	public static XshdSyntaxDefinition LoadDefinition(XmlReader reader, bool skipValidation)
	{
		reader = HighlightingLoader.GetValidatingReader(reader, ignoreWhitespace: true);
		reader.Read();
		return ParseDefinition(reader);
	}

	private static XshdSyntaxDefinition ParseDefinition(XmlReader reader)
	{
		XshdSyntaxDefinition xshdSyntaxDefinition = new XshdSyntaxDefinition
		{
			Name = reader.GetAttribute("name")
		};
		string attribute = reader.GetAttribute("extensions");
		if (attribute != null)
		{
			xshdSyntaxDefinition.Extensions.AddRange(attribute.Split(new char[1] { ';' }));
		}
		ParseElements(xshdSyntaxDefinition.Elements, reader);
		return xshdSyntaxDefinition;
	}

	private static void ParseElements(ICollection<XshdElement> c, XmlReader reader)
	{
		if (reader.IsEmptyElement)
		{
			return;
		}
		while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
		{
			if (reader.NamespaceURI != "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008")
			{
				if (!reader.IsEmptyElement)
				{
					reader.Skip();
				}
				continue;
			}
			switch (reader.Name)
			{
			case "RuleSet":
				c.Add(ParseRuleSet(reader));
				break;
			case "Property":
				c.Add(ParseProperty(reader));
				break;
			case "Color":
				c.Add(ParseNamedColor(reader));
				break;
			case "Keywords":
				c.Add(ParseKeywords(reader));
				break;
			case "Span":
				c.Add(ParseSpan(reader));
				break;
			case "Import":
				c.Add(ParseImport(reader));
				break;
			case "Rule":
				c.Add(ParseRule(reader));
				break;
			default:
				throw new NotSupportedException("Unknown element " + reader.Name);
			}
		}
	}

	private static XshdElement ParseProperty(XmlReader reader)
	{
		XshdProperty xshdProperty = new XshdProperty();
		SetPosition(xshdProperty, reader);
		xshdProperty.Name = reader.GetAttribute("name");
		xshdProperty.Value = reader.GetAttribute("value");
		return xshdProperty;
	}

	private static XshdRuleSet ParseRuleSet(XmlReader reader)
	{
		XshdRuleSet xshdRuleSet = new XshdRuleSet();
		SetPosition(xshdRuleSet, reader);
		xshdRuleSet.Name = reader.GetAttribute("name");
		xshdRuleSet.IgnoreCase = reader.GetBoolAttribute("ignoreCase");
		CheckElementName(reader, xshdRuleSet.Name);
		ParseElements(xshdRuleSet.Elements, reader);
		return xshdRuleSet;
	}

	private static XshdRule ParseRule(XmlReader reader)
	{
		XshdRule xshdRule = new XshdRule();
		SetPosition(xshdRule, reader);
		xshdRule.ColorReference = ParseColorReference(reader);
		if (!reader.IsEmptyElement)
		{
			reader.Read();
			if (reader.NodeType == XmlNodeType.Text)
			{
				xshdRule.Regex = reader.ReadContentAsString();
				xshdRule.RegexType = XshdRegexType.IgnorePatternWhitespace;
			}
		}
		return xshdRule;
	}

	private static XshdKeywords ParseKeywords(XmlReader reader)
	{
		XshdKeywords xshdKeywords = new XshdKeywords();
		SetPosition(xshdKeywords, reader);
		xshdKeywords.ColorReference = ParseColorReference(reader);
		reader.Read();
		while (reader.NodeType != XmlNodeType.EndElement)
		{
			xshdKeywords.Words.Add(reader.ReadElementContentAsString());
		}
		return xshdKeywords;
	}

	private static XshdImport ParseImport(XmlReader reader)
	{
		XshdImport xshdImport = new XshdImport();
		SetPosition(xshdImport, reader);
		xshdImport.RuleSetReference = ParseRuleSetReference(reader);
		if (!reader.IsEmptyElement)
		{
			reader.Skip();
		}
		return xshdImport;
	}

	private static XshdSpan ParseSpan(XmlReader reader)
	{
		XshdSpan xshdSpan = new XshdSpan();
		SetPosition(xshdSpan, reader);
		xshdSpan.BeginRegex = reader.GetAttribute("begin");
		xshdSpan.EndRegex = reader.GetAttribute("end");
		xshdSpan.Multiline = reader.GetBoolAttribute("multiline") == true;
		xshdSpan.SpanColorReference = ParseColorReference(reader);
		xshdSpan.RuleSetReference = ParseRuleSetReference(reader);
		if (!reader.IsEmptyElement)
		{
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
				case "Begin":
					if (xshdSpan.BeginRegex != null)
					{
						throw Error(reader, "Duplicate Begin regex");
					}
					xshdSpan.BeginColorReference = ParseColorReference(reader);
					xshdSpan.BeginRegex = reader.ReadElementContentAsString();
					xshdSpan.BeginRegexType = XshdRegexType.IgnorePatternWhitespace;
					break;
				case "End":
					if (xshdSpan.EndRegex != null)
					{
						throw Error(reader, "Duplicate End regex");
					}
					xshdSpan.EndColorReference = ParseColorReference(reader);
					xshdSpan.EndRegex = reader.ReadElementContentAsString();
					xshdSpan.EndRegexType = XshdRegexType.IgnorePatternWhitespace;
					break;
				case "RuleSet":
					if (xshdSpan.RuleSetReference.ReferencedElement != null)
					{
						throw Error(reader, "Cannot specify both inline RuleSet and RuleSet reference");
					}
					xshdSpan.RuleSetReference = new XshdReference<XshdRuleSet>(ParseRuleSet(reader));
					reader.Read();
					break;
				default:
					throw new NotSupportedException("Unknown element " + reader.Name);
				}
			}
		}
		return xshdSpan;
	}

	private static Exception Error(XmlReader reader, string message)
	{
		return Error(reader as IXmlLineInfo, message);
	}

	private static Exception Error(IXmlLineInfo lineInfo, string message)
	{
		if (lineInfo != null)
		{
			return new HighlightingDefinitionInvalidException(HighlightingLoader.FormatExceptionMessage(message, lineInfo.LineNumber, lineInfo.LinePosition));
		}
		return new HighlightingDefinitionInvalidException(message);
	}

	private static void SetPosition(XshdElement element, XmlReader reader)
	{
		if (reader is IXmlLineInfo xmlLineInfo)
		{
			element.LineNumber = xmlLineInfo.LineNumber;
			element.ColumnNumber = xmlLineInfo.LinePosition;
		}
	}

	private static XshdReference<XshdRuleSet> ParseRuleSetReference(XmlReader reader)
	{
		string attribute = reader.GetAttribute("ruleSet");
		if (attribute != null)
		{
			int num = attribute.LastIndexOf('/');
			if (num >= 0)
			{
				return new XshdReference<XshdRuleSet>(attribute.Substring(0, num), attribute.Substring(num + 1));
			}
			return new XshdReference<XshdRuleSet>(null, attribute);
		}
		return default(XshdReference<XshdRuleSet>);
	}

	private static void CheckElementName(XmlReader reader, string name)
	{
		if (name != null)
		{
			if (name.Length == 0)
			{
				throw Error(reader, "The empty string is not a valid name.");
			}
			if (name.IndexOf('/') >= 0)
			{
				throw Error(reader, "Element names must not contain a slash.");
			}
		}
	}

	private static XshdColor ParseNamedColor(XmlReader reader)
	{
		XshdColor xshdColor = ParseColorAttributes(reader);
		xshdColor.Name = reader.GetAttribute("name");
		CheckElementName(reader, xshdColor.Name);
		xshdColor.ExampleText = reader.GetAttribute("exampleText");
		return xshdColor;
	}

	private static XshdReference<XshdColor> ParseColorReference(XmlReader reader)
	{
		string attribute = reader.GetAttribute("color");
		if (attribute != null)
		{
			int num = attribute.LastIndexOf('/');
			if (num >= 0)
			{
				return new XshdReference<XshdColor>(attribute.Substring(0, num), attribute.Substring(num + 1));
			}
			return new XshdReference<XshdColor>(null, attribute);
		}
		return new XshdReference<XshdColor>(ParseColorAttributes(reader));
	}

	private static XshdColor ParseColorAttributes(XmlReader reader)
	{
		XshdColor xshdColor = new XshdColor();
		SetPosition(xshdColor, reader);
		xshdColor.Foreground = ParseColor(reader.GetAttribute("foreground"));
		xshdColor.Background = ParseColor(reader.GetAttribute("background"));
		xshdColor.FontWeight = ParseFontWeight(reader.GetAttribute("fontWeight"));
		xshdColor.FontStyle = ParseFontStyle(reader.GetAttribute("fontStyle"));
		xshdColor.Underline = reader.GetBoolAttribute("underline");
		xshdColor.Strikethrough = reader.GetBoolAttribute("strikethrough");
		xshdColor.FontFamily = ParseFontFamily(reader.GetAttribute("fontFamily"));
		xshdColor.FontSize = ParseFontSize(reader.GetAttribute("fontSize"));
		return xshdColor;
	}

	private static HighlightingBrush ParseColor(string color)
	{
		if (string.IsNullOrEmpty(color))
		{
			return null;
		}
		return FixedColorHighlightingBrush(Color.Parse(color));
	}

	private static HighlightingBrush FixedColorHighlightingBrush(Color? color)
	{
		if (!color.HasValue)
		{
			return null;
		}
		return new SimpleHighlightingBrush(color.Value);
	}

	private static FontFamily ParseFontFamily(string fontFamily)
	{
		if (string.IsNullOrEmpty(fontFamily))
		{
			return null;
		}
		return new FontFamily(fontFamily);
	}

	private static int? ParseFontSize(string size)
	{
		if (!int.TryParse(size, out var result))
		{
			return null;
		}
		return result;
	}

	private static FontWeight? ParseFontWeight(string fontWeight)
	{
		if (string.IsNullOrEmpty(fontWeight))
		{
			return null;
		}
		return (FontWeight)Enum.Parse(typeof(FontWeight), fontWeight, ignoreCase: true);
	}

	private static FontStyle? ParseFontStyle(string fontStyle)
	{
		if (string.IsNullOrEmpty(fontStyle))
		{
			return null;
		}
		return (FontStyle)Enum.Parse(typeof(FontStyle), fontStyle, ignoreCase: true);
	}
}
