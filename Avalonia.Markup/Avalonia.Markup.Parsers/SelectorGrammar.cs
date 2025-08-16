using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal static class SelectorGrammar
{
	private enum State
	{
		Start,
		Middle,
		Colon,
		Class,
		Name,
		CanHaveType,
		Traversal,
		TypeName,
		Property,
		AttachedProperty,
		Template,
		End
	}

	public interface ISyntax
	{
	}

	public interface ITypeSyntax
	{
		string TypeName { get; set; }

		string Xmlns { get; set; }
	}

	public class OfTypeSyntax : ISyntax, ITypeSyntax
	{
		public string TypeName { get; set; } = string.Empty;

		public string Xmlns { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is OfTypeSyntax ofTypeSyntax && ofTypeSyntax.TypeName == TypeName)
			{
				return ofTypeSyntax.Xmlns == Xmlns;
			}
			return false;
		}
	}

	public class AttachedPropertySyntax : ISyntax, ITypeSyntax
	{
		public string Xmlns { get; set; } = string.Empty;

		public string TypeName { get; set; } = string.Empty;

		public string Property { get; set; } = string.Empty;

		public string Value { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is AttachedPropertySyntax attachedPropertySyntax && attachedPropertySyntax.Xmlns == Xmlns && attachedPropertySyntax.TypeName == TypeName && attachedPropertySyntax.Property == Property)
			{
				return attachedPropertySyntax.Value == Value;
			}
			return false;
		}
	}

	public class IsSyntax : ISyntax, ITypeSyntax
	{
		public string TypeName { get; set; } = string.Empty;

		public string Xmlns { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is IsSyntax isSyntax && isSyntax.TypeName == TypeName)
			{
				return isSyntax.Xmlns == Xmlns;
			}
			return false;
		}
	}

	public class ClassSyntax : ISyntax
	{
		public string Class { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is ClassSyntax classSyntax)
			{
				return classSyntax.Class == Class;
			}
			return false;
		}
	}

	public class NameSyntax : ISyntax
	{
		public string Name { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is NameSyntax nameSyntax)
			{
				return nameSyntax.Name == Name;
			}
			return false;
		}
	}

	public class PropertySyntax : ISyntax
	{
		public string Property { get; set; } = string.Empty;

		public string Value { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj is PropertySyntax propertySyntax && propertySyntax.Property == Property)
			{
				return propertySyntax.Value == Value;
			}
			return false;
		}
	}

	public class ChildSyntax : ISyntax
	{
		public override bool Equals(object? obj)
		{
			return obj is ChildSyntax;
		}
	}

	public class DescendantSyntax : ISyntax
	{
		public override bool Equals(object? obj)
		{
			return obj is DescendantSyntax;
		}
	}

	public class TemplateSyntax : ISyntax
	{
		public override bool Equals(object? obj)
		{
			return obj is TemplateSyntax;
		}
	}

	public class NotSyntax : ISyntax
	{
		public IEnumerable<ISyntax> Argument { get; set; } = Enumerable.Empty<ISyntax>();

		public override bool Equals(object? obj)
		{
			if (obj is NotSyntax notSyntax)
			{
				return Argument.SequenceEqual(notSyntax.Argument);
			}
			return false;
		}
	}

	public class NthChildSyntax : ISyntax
	{
		public int Offset { get; set; }

		public int Step { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is NthChildSyntax nthChildSyntax && nthChildSyntax.Offset == Offset)
			{
				return nthChildSyntax.Step == Step;
			}
			return false;
		}
	}

	public class NthLastChildSyntax : ISyntax
	{
		public int Offset { get; set; }

		public int Step { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is NthLastChildSyntax nthLastChildSyntax && nthLastChildSyntax.Offset == Offset)
			{
				return nthLastChildSyntax.Step == Step;
			}
			return false;
		}
	}

	public class CommaSyntax : ISyntax
	{
		public override bool Equals(object? obj)
		{
			return obj is CommaSyntax;
		}
	}

	public class NestingSyntax : ISyntax
	{
		public override bool Equals(object? obj)
		{
			return obj is NestingSyntax;
		}
	}

	public static IEnumerable<ISyntax> Parse(string s)
	{
		CharacterReader r = new CharacterReader(s.AsSpan());
		return Parse(ref r, null);
	}

	private static IEnumerable<ISyntax> Parse(ref CharacterReader r, char? end)
	{
		State state = State.Start;
		List<ISyntax> list = new List<ISyntax>();
		while (!r.End && state != State.End)
		{
			ISyntax syntax = null;
			switch (state)
			{
			case State.Start:
				(state, syntax) = ParseStart(ref r);
				break;
			case State.Middle:
				(state, syntax) = ParseMiddle(ref r, end);
				break;
			case State.CanHaveType:
				state = ParseCanHaveType(ref r);
				break;
			case State.Colon:
				(state, syntax) = ParseColon(ref r);
				break;
			case State.Class:
				(state, syntax) = ParseClass(ref r);
				break;
			case State.Traversal:
				(state, syntax) = ParseTraversal(ref r);
				break;
			case State.TypeName:
				(state, syntax) = ParseTypeName(ref r);
				break;
			case State.Property:
				(state, syntax) = ParseProperty(ref r);
				break;
			case State.Template:
				(state, syntax) = ParseTemplate(ref r);
				break;
			case State.Name:
				(state, syntax) = ParseName(ref r);
				break;
			case State.AttachedProperty:
				(state, syntax) = ParseAttachedProperty(ref r);
				break;
			}
			if (syntax != null)
			{
				list.Add(syntax);
			}
		}
		if (state != 0 && state != State.Middle && state != State.End && state != State.CanHaveType)
		{
			throw new ExpressionParseException(r.Position, "Unexpected end of selector");
		}
		return list;
	}

	private static (State, ISyntax?) ParseStart(ref CharacterReader r)
	{
		r.SkipWhitespace();
		if (r.End)
		{
			return (State.End, null);
		}
		if (r.TakeIf(':'))
		{
			return (State.Colon, null);
		}
		if (r.TakeIf('.'))
		{
			return (State.Class, null);
		}
		if (r.TakeIf('#'))
		{
			return (State.Name, null);
		}
		if (r.TakeIf('^'))
		{
			return (State.CanHaveType, new NestingSyntax());
		}
		return (State.TypeName, null);
	}

	private static (State, ISyntax?) ParseMiddle(ref CharacterReader r, char? end)
	{
		if (r.TakeIf(':'))
		{
			return (State.Colon, null);
		}
		if (r.TakeIf('.'))
		{
			return (State.Class, null);
		}
		if (r.TakeIf(char.IsWhiteSpace) || r.Peek == '>')
		{
			return (State.Traversal, null);
		}
		if (r.TakeIf('/'))
		{
			return (State.Template, null);
		}
		if (r.TakeIf('#'))
		{
			return (State.Name, null);
		}
		if (r.TakeIf(','))
		{
			return (State.Start, new CommaSyntax());
		}
		if (r.TakeIf('^'))
		{
			return (State.CanHaveType, new NestingSyntax());
		}
		if (end.HasValue && !r.End && r.Peek == end.Value)
		{
			return (State.End, null);
		}
		return (State.TypeName, null);
	}

	private static State ParseCanHaveType(ref CharacterReader r)
	{
		if (r.TakeIf('['))
		{
			return State.Property;
		}
		return State.Middle;
	}

	private static (State, ISyntax) ParseColon(ref CharacterReader r)
	{
		ReadOnlySpan<char> span = r.ParseStyleClass();
		if (span.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Expected class name, is, nth-child or nth-last-child selector after ':'.");
		}
		if (span.SequenceEqual("is".AsSpan()) && r.TakeIf('('))
		{
			IsSyntax item = ParseType(ref r, new IsSyntax());
			Expect(ref r, ')');
			return (State.CanHaveType, item);
		}
		if (span.SequenceEqual("not".AsSpan()) && r.TakeIf('('))
		{
			IEnumerable<ISyntax> argument = Parse(ref r, ')');
			Expect(ref r, ')');
			NotSyntax item2 = new NotSyntax
			{
				Argument = argument
			};
			return (State.Middle, item2);
		}
		if (span.SequenceEqual("nth-child".AsSpan()) && r.TakeIf('('))
		{
			(int step, int offset) tuple = ParseNthChildArguments(ref r);
			int item3 = tuple.step;
			int item4 = tuple.offset;
			NthChildSyntax item5 = new NthChildSyntax
			{
				Step = item3,
				Offset = item4
			};
			return (State.Middle, item5);
		}
		if (span.SequenceEqual("nth-last-child".AsSpan()) && r.TakeIf('('))
		{
			(int step, int offset) tuple2 = ParseNthChildArguments(ref r);
			int item6 = tuple2.step;
			int item7 = tuple2.offset;
			NthLastChildSyntax item8 = new NthLastChildSyntax
			{
				Step = item6,
				Offset = item7
			};
			return (State.Middle, item8);
		}
		return (State.CanHaveType, new ClassSyntax
		{
			Class = ":" + span
		});
	}

	private static (State, ISyntax?) ParseTraversal(ref CharacterReader r)
	{
		r.SkipWhitespace();
		if (r.TakeIf('>'))
		{
			r.SkipWhitespace();
			return (State.Middle, new ChildSyntax());
		}
		if (r.TakeIf('/'))
		{
			return (State.Template, null);
		}
		if (!r.End)
		{
			return (State.Middle, new DescendantSyntax());
		}
		return (State.End, null);
	}

	private static (State, ISyntax) ParseClass(ref CharacterReader r)
	{
		ReadOnlySpan<char> readOnlySpan = r.ParseStyleClass();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Expected a class name after '.'.");
		}
		return (State.CanHaveType, new ClassSyntax
		{
			Class = readOnlySpan.ToString()
		});
	}

	private static (State, ISyntax) ParseTemplate(ref CharacterReader r)
	{
		ReadOnlySpan<char> span = r.ParseIdentifier();
		if (!span.SequenceEqual("template".AsSpan()))
		{
			throw new ExpressionParseException(r.Position, "Expected 'template', got '" + span.ToString() + "'");
		}
		if (!r.TakeIf('/'))
		{
			throw new ExpressionParseException(r.Position, "Expected '/'");
		}
		return (State.Start, new TemplateSyntax());
	}

	private static (State, ISyntax) ParseName(ref CharacterReader r)
	{
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Expected a name after '#'.");
		}
		return (State.CanHaveType, new NameSyntax
		{
			Name = readOnlySpan.ToString()
		});
	}

	private static (State, ISyntax) ParseTypeName(ref CharacterReader r)
	{
		return (State.CanHaveType, ParseType(ref r, new OfTypeSyntax()));
	}

	private static (State, ISyntax?) ParseProperty(ref CharacterReader r)
	{
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (r.TakeIf('('))
		{
			return (State.AttachedProperty, null);
		}
		if (!r.TakeIf('='))
		{
			throw new ExpressionParseException(r.Position, $"Expected '=', got '{r.Peek}'");
		}
		ReadOnlySpan<char> readOnlySpan2 = r.TakeUntil(']');
		r.Take();
		return (State.CanHaveType, new PropertySyntax
		{
			Property = readOnlySpan.ToString(),
			Value = readOnlySpan2.ToString()
		});
	}

	private static (State, ISyntax) ParseAttachedProperty(ref CharacterReader r)
	{
		AttachedPropertySyntax attachedPropertySyntax = ParseType(ref r, new AttachedPropertySyntax());
		if (!r.TakeIf('.'))
		{
			throw new ExpressionParseException(r.Position, $"Expected '.', got '{r.Peek}'");
		}
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, $"Expected Attached Property Name, got '{r.Peek}'");
		}
		attachedPropertySyntax.Property = readOnlySpan.ToString();
		if (!r.TakeIf(')'))
		{
			throw new ExpressionParseException(r.Position, $"Expected ')', got '{r.Peek}'");
		}
		if (!r.TakeIf('='))
		{
			throw new ExpressionParseException(r.Position, $"Expected '=', got '{r.Peek}'");
		}
		attachedPropertySyntax.Value = r.TakeUntil(']').ToString();
		r.Take();
		return ((!r.End) ? State.Middle : State.End, attachedPropertySyntax);
	}

	private static TSyntax ParseType<TSyntax>(ref CharacterReader r, TSyntax syntax) where TSyntax : ITypeSyntax
	{
		ReadOnlySpan<char> readOnlySpan = default(ReadOnlySpan<char>);
		ReadOnlySpan<char> readOnlySpan2 = r.ParseIdentifier();
		if (readOnlySpan2.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, $"Expected an identifier, got '{r.Peek}");
		}
		ReadOnlySpan<char> readOnlySpan3;
		if (!r.End && r.TakeIf('|'))
		{
			readOnlySpan = readOnlySpan2;
			if (r.End)
			{
				throw new ExpressionParseException(r.Position, "Unexpected end of selector.");
			}
			readOnlySpan3 = r.ParseIdentifier();
		}
		else
		{
			readOnlySpan3 = readOnlySpan2;
		}
		syntax.Xmlns = readOnlySpan.ToString();
		syntax.TypeName = readOnlySpan3.ToString();
		return syntax;
	}

	private static (int step, int offset) ParseNthChildArguments(ref CharacterReader r)
	{
		int num = 0;
		int value = 0;
		if (r.Peek == 'o')
		{
			ReadOnlySpan<char> span = r.TakeUntil(')').Trim();
			if (!span.SequenceEqual("odd".AsSpan()))
			{
				throw new ExpressionParseException(r.Position, "Expected nth-child(odd). Actual '" + span.ToString() + "'.");
			}
			num = 2;
			value = 1;
		}
		else if (r.Peek == 'e')
		{
			ReadOnlySpan<char> span2 = r.TakeUntil(')').Trim();
			if (!span2.SequenceEqual("even".AsSpan()))
			{
				throw new ExpressionParseException(r.Position, "Expected nth-child(even). Actual '" + span2.ToString() + "'.");
			}
			num = 2;
			value = 0;
		}
		else
		{
			r.SkipWhitespace();
			int value2 = 0;
			ReadOnlySpan<char> span3 = r.TakeWhile((char c) => char.IsDigit(c) || c == '-' || c == '+');
			if (span3.Length == 0 || (span3.Length == 1 && span3[0] == '+'))
			{
				value2 = 1;
			}
			else if (span3.Length == 1 && span3[0] == '-')
			{
				value2 = -1;
			}
			else if (!span3.TryParseInt(out value2))
			{
				throw new ExpressionParseException(r.Position, "Couldn't parse nth-child step or offset value. Integer was expected.");
			}
			r.SkipWhitespace();
			if (r.Peek == ')')
			{
				num = 0;
				value = value2;
			}
			else
			{
				num = value2;
				if (r.Peek != 'n')
				{
					throw new ExpressionParseException(r.Position, "Couldn't parse nth-child step value, \"xn+y\" pattern was expected.");
				}
				r.Skip(1);
				r.SkipWhitespace();
				if (r.Peek != ')')
				{
					int num2 = r.Take() switch
					{
						'+' => 1, 
						'-' => -1, 
						_ => throw new ExpressionParseException(r.Position, "Couldn't parse nth-child sign. '+' or '-' was expected."), 
					};
					r.SkipWhitespace();
					if (num2 != 0 && !r.TakeUntil(')').TryParseInt(out value))
					{
						throw new ExpressionParseException(r.Position, "Couldn't parse nth-child offset value. Integer was expected.");
					}
					value *= num2;
				}
			}
		}
		Expect(ref r, ')');
		return (step: num, offset: value);
	}

	private static void Expect(ref CharacterReader r, char c)
	{
		if (r.End)
		{
			throw new ExpressionParseException(r.Position, $"Expected '{c}', got end of selector.");
		}
		if (!r.TakeIf(')'))
		{
			throw new ExpressionParseException(r.Position, $"Expected '{c}', got '{r.Peek}'.");
		}
	}
}
