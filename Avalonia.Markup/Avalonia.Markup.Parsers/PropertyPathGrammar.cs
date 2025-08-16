using System;
using System.Collections.Generic;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal class PropertyPathGrammar
{
	private enum State
	{
		Start,
		Next,
		AfterProperty,
		End
	}

	public interface ISyntax
	{
	}

	public class PropertySyntax : ISyntax
	{
		public string Name { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is PropertySyntax propertySyntax)
			{
				return propertySyntax.Name == Name;
			}
			return false;
		}
	}

	public class TypeQualifiedPropertySyntax : ISyntax
	{
		public string Name { get; set; } = string.Empty;

		public string TypeName { get; set; } = string.Empty;

		public string? TypeNamespace { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is TypeQualifiedPropertySyntax typeQualifiedPropertySyntax && typeQualifiedPropertySyntax.Name == Name && typeQualifiedPropertySyntax.TypeName == TypeName)
			{
				return typeQualifiedPropertySyntax.TypeNamespace == TypeNamespace;
			}
			return false;
		}
	}

	public class ChildTraversalSyntax : ISyntax
	{
		public static ChildTraversalSyntax Instance { get; } = new ChildTraversalSyntax();

		public override bool Equals(object? obj)
		{
			return obj is ChildTraversalSyntax;
		}
	}

	public class EnsureTypeSyntax : ISyntax
	{
		public string TypeName { get; set; } = string.Empty;

		public string? TypeNamespace { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is EnsureTypeSyntax ensureTypeSyntax && ensureTypeSyntax.TypeName == TypeName)
			{
				return ensureTypeSyntax.TypeNamespace == TypeNamespace;
			}
			return false;
		}
	}

	public class CastTypeSyntax : ISyntax
	{
		public string TypeName { get; set; } = string.Empty;

		public string? TypeNamespace { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is CastTypeSyntax castTypeSyntax && castTypeSyntax.TypeName == TypeName)
			{
				return castTypeSyntax.TypeNamespace == TypeNamespace;
			}
			return false;
		}
	}

	public static IEnumerable<ISyntax> Parse(string s)
	{
		CharacterReader r = new CharacterReader(s.AsSpan());
		return Parse(ref r);
	}

	private static IEnumerable<ISyntax> Parse(ref CharacterReader r)
	{
		State state = State.Start;
		List<ISyntax> list = new List<ISyntax>();
		while (state != State.End)
		{
			ISyntax syntax = null;
			switch (state)
			{
			case State.Start:
				(state, syntax) = ParseStart(ref r);
				break;
			case State.Next:
				(state, syntax) = ParseNext(ref r);
				break;
			case State.AfterProperty:
				(state, syntax) = ParseAfterProperty(ref r);
				break;
			}
			if (syntax != null)
			{
				list.Add(syntax);
			}
		}
		if (state != State.End && r.End)
		{
			throw new ExpressionParseException(r.Position, "Unexpected end of property path");
		}
		return list;
	}

	private static (State, ISyntax?) ParseNext(ref CharacterReader r)
	{
		r.SkipWhitespace();
		if (r.End)
		{
			return (State.End, null);
		}
		return ParseStart(ref r);
	}

	private static (State, ISyntax) ParseStart(ref CharacterReader r)
	{
		if (TryParseCasts(ref r, out (State, ISyntax) rv))
		{
			return rv;
		}
		r.SkipWhitespace();
		if (r.TakeIf('('))
		{
			return ParseTypeQualifiedProperty(ref r);
		}
		return ParseProperty(ref r);
	}

	private static (State, ISyntax) ParseTypeQualifiedProperty(ref CharacterReader r)
	{
		r.SkipWhitespace();
		(string, string) tuple = ParseXamlIdentifier(ref r);
		if (!r.TakeIf('.'))
		{
			throw new ExpressionParseException(r.Position, "Unable to parse qualified property name, expected `(ns:TypeName.PropertyName)` or `(TypeName.PropertyName)` after `(`");
		}
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Unable to parse qualified property name, expected `(ns:TypeName.PropertyName)` or `(TypeName.PropertyName)` after `(`");
		}
		r.SkipWhitespace();
		if (!r.TakeIf(')'))
		{
			throw new ExpressionParseException(r.Position, "Expected ')' after qualified property name " + tuple.Item1 + ":" + tuple.Item2 + "." + readOnlySpan);
		}
		TypeQualifiedPropertySyntax obj = new TypeQualifiedPropertySyntax
		{
			Name = readOnlySpan.ToString(),
			TypeName = tuple.Item2
		};
		(obj.TypeNamespace, _) = tuple;
		return (State.AfterProperty, obj);
	}

	private static (string? ns, string name) ParseXamlIdentifier(ref CharacterReader r)
	{
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Expected identifier");
		}
		if (r.TakeIf(':'))
		{
			ReadOnlySpan<char> readOnlySpan2 = r.ParseIdentifier();
			if (readOnlySpan2.IsEmpty)
			{
				throw new ExpressionParseException(r.Position, "Expected the rest of the identifier after " + readOnlySpan.ToString() + ":");
			}
			return (ns: readOnlySpan.ToString(), name: readOnlySpan2.ToString());
		}
		return (ns: null, name: readOnlySpan.ToString());
	}

	private static (State, ISyntax) ParseProperty(ref CharacterReader r)
	{
		r.SkipWhitespace();
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Unable to parse property name");
		}
		return (State.AfterProperty, new PropertySyntax
		{
			Name = readOnlySpan.ToString()
		});
	}

	private static bool TryParseCasts(ref CharacterReader r, out (State, ISyntax) rv)
	{
		if (r.TakeIfKeyword(":="))
		{
			rv = ParseEnsureType(ref r);
		}
		else
		{
			if (!r.TakeIfKeyword(":>") && !r.TakeIfKeyword("as "))
			{
				rv = default((State, ISyntax));
				return false;
			}
			rv = ParseCastType(ref r);
		}
		return true;
	}

	private static (State, ISyntax?) ParseAfterProperty(ref CharacterReader r)
	{
		if (TryParseCasts(ref r, out (State, ISyntax) rv))
		{
			return rv;
		}
		r.SkipWhitespace();
		if (r.End)
		{
			return (State.End, null);
		}
		if (r.TakeIf('.'))
		{
			return (State.Next, ChildTraversalSyntax.Instance);
		}
		throw new ExpressionParseException(r.Position, "Unexpected character " + r.Peek + " after property name");
	}

	private static (State, ISyntax) ParseEnsureType(ref CharacterReader r)
	{
		r.SkipWhitespace();
		(string, string) tuple = ParseXamlIdentifier(ref r);
		EnsureTypeSyntax obj = new EnsureTypeSyntax
		{
			TypeName = tuple.Item2
		};
		(obj.TypeNamespace, _) = tuple;
		return (State.AfterProperty, obj);
	}

	private static (State, ISyntax) ParseCastType(ref CharacterReader r)
	{
		r.SkipWhitespace();
		(string, string) tuple = ParseXamlIdentifier(ref r);
		CastTypeSyntax obj = new CastTypeSyntax
		{
			TypeName = tuple.Item2
		};
		(obj.TypeNamespace, _) = tuple;
		return (State.AfterProperty, obj);
	}
}
