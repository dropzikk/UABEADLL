using System;
using System.Xml;

namespace AvaloniaEdit.Highlighting.Xshd;

public static class HighlightingLoader
{
	public static XshdSyntaxDefinition LoadXshd(XmlReader reader)
	{
		return LoadXshd(reader, skipValidation: false);
	}

	internal static XshdSyntaxDefinition LoadXshd(XmlReader reader, bool skipValidation)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		try
		{
			reader.MoveToContent();
			return V2Loader.LoadDefinition(reader, skipValidation);
		}
		catch (XmlException ex)
		{
			throw WrapException(ex, ex.LineNumber, ex.LinePosition);
		}
	}

	private static Exception WrapException(Exception ex, int lineNumber, int linePosition)
	{
		return new HighlightingDefinitionInvalidException(FormatExceptionMessage(ex.Message, lineNumber, linePosition), ex);
	}

	internal static string FormatExceptionMessage(string message, int lineNumber, int linePosition)
	{
		if (lineNumber <= 0)
		{
			return message;
		}
		return "Error at position (line " + lineNumber + ", column " + linePosition + "):\n" + message;
	}

	internal static XmlReader GetValidatingReader(XmlReader input, bool ignoreWhitespace)
	{
		XmlReaderSettings settings = new XmlReaderSettings
		{
			CloseInput = true,
			IgnoreComments = true,
			IgnoreWhitespace = ignoreWhitespace
		};
		return XmlReader.Create(input, settings);
	}

	public static IHighlightingDefinition Load(XshdSyntaxDefinition syntaxDefinition, IHighlightingDefinitionReferenceResolver resolver)
	{
		if (syntaxDefinition == null)
		{
			throw new ArgumentNullException("syntaxDefinition");
		}
		return new XmlHighlightingDefinition(syntaxDefinition, resolver);
	}

	public static IHighlightingDefinition Load(XmlReader reader, IHighlightingDefinitionReferenceResolver resolver)
	{
		return Load(LoadXshd(reader), resolver);
	}
}
