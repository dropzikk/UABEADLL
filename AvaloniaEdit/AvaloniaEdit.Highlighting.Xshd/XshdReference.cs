using System;

namespace AvaloniaEdit.Highlighting.Xshd;

public struct XshdReference<T> : IEquatable<XshdReference<T>> where T : XshdElement
{
	public string ReferencedDefinition { get; }

	public string ReferencedElement { get; }

	public T InlineElement { get; }

	public XshdReference(string referencedDefinition, string referencedElement)
	{
		ReferencedDefinition = referencedDefinition;
		ReferencedElement = referencedElement ?? throw new ArgumentNullException("referencedElement");
		InlineElement = null;
	}

	public XshdReference(T inlineElement)
	{
		ReferencedDefinition = null;
		ReferencedElement = null;
		InlineElement = inlineElement ?? throw new ArgumentNullException("inlineElement");
	}

	public object AcceptVisitor(IXshdVisitor visitor)
	{
		return InlineElement?.AcceptVisitor(visitor);
	}

	public override bool Equals(object obj)
	{
		if (obj is XshdReference<T>)
		{
			return Equals((XshdReference<T>)obj);
		}
		return false;
	}

	public bool Equals(XshdReference<T> other)
	{
		if (ReferencedDefinition == other.ReferencedDefinition && ReferencedElement == other.ReferencedElement)
		{
			return InlineElement == other.InlineElement;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GetHashCode(ReferencedDefinition) ^ GetHashCode(ReferencedElement) ^ GetHashCode(InlineElement);
	}

	private static int GetHashCode(object o)
	{
		return o?.GetHashCode() ?? 0;
	}

	public static bool operator ==(XshdReference<T> left, XshdReference<T> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(XshdReference<T> left, XshdReference<T> right)
	{
		return !left.Equals(right);
	}
}
