using System;
using System.Collections.Generic;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal static class BindingExpressionGrammar
{
	private enum State
	{
		Start,
		RelativeSource,
		ElementName,
		AfterMember,
		BeforeMember,
		AttachedProperty,
		Indexer,
		TypeCast,
		End
	}

	private readonly ref struct TypeName
	{
		public readonly ReadOnlySpan<char> Namespace;

		public readonly ReadOnlySpan<char> Type;

		public TypeName(ReadOnlySpan<char> ns, ReadOnlySpan<char> typeName)
		{
			Namespace = ns;
			Type = typeName;
		}

		public void Deconstruct(out string ns, out string typeName)
		{
			ns = Namespace.ToString();
			typeName = Type.ToString();
		}
	}

	public interface INode
	{
	}

	public interface ITransformNode
	{
	}

	public class EmptyExpressionNode : INode
	{
	}

	public class PropertyNameNode : INode
	{
		public string PropertyName { get; set; } = string.Empty;
	}

	public class AttachedPropertyNameNode : INode
	{
		public string Namespace { get; set; } = string.Empty;

		public string TypeName { get; set; } = string.Empty;

		public string PropertyName { get; set; } = string.Empty;
	}

	public class IndexerNode : INode
	{
		public IList<string> Arguments { get; set; } = Array.Empty<string>();
	}

	public class NotNode : INode, ITransformNode
	{
	}

	public class StreamNode : INode
	{
	}

	public class SelfNode : INode
	{
	}

	public class NameNode : INode
	{
		public string Name { get; set; } = string.Empty;
	}

	public class AncestorNode : INode
	{
		public string? Namespace { get; set; }

		public string? TypeName { get; set; }

		public int Level { get; set; }
	}

	public class TypeCastNode : INode
	{
		public string Namespace { get; set; } = string.Empty;

		public string TypeName { get; set; } = string.Empty;
	}

	public static (List<INode> Nodes, SourceMode Mode) Parse(ref CharacterReader r)
	{
		List<INode> list = new List<INode>();
		State state = State.Start;
		SourceMode item = SourceMode.Data;
		while (!r.End)
		{
			switch (state)
			{
			case State.Start:
				state = ParseStart(ref r, list);
				continue;
			case State.AfterMember:
				state = ParseAfterMember(ref r, list);
				continue;
			case State.BeforeMember:
				state = ParseBeforeMember(ref r, list);
				continue;
			case State.AttachedProperty:
				state = ParseAttachedProperty(ref r, list);
				continue;
			case State.Indexer:
				state = ParseIndexer(ref r, list);
				continue;
			case State.TypeCast:
				state = ParseTypeCast(ref r, list);
				continue;
			case State.ElementName:
				state = ParseElementName(ref r, list);
				item = SourceMode.Control;
				continue;
			case State.RelativeSource:
				state = ParseRelativeSource(ref r, list);
				item = SourceMode.Control;
				continue;
			default:
				continue;
			case State.End:
				break;
			}
			break;
		}
		if (state == State.BeforeMember)
		{
			throw new ExpressionParseException(r.Position, "Unexpected end of expression.");
		}
		return (Nodes: list, Mode: item);
	}

	private static State ParseStart(ref CharacterReader r, IList<INode> nodes)
	{
		if (ParseNot(ref r))
		{
			nodes.Add(new NotNode());
			return State.Start;
		}
		if (ParseSharp(ref r))
		{
			return State.ElementName;
		}
		if (ParseDollarSign(ref r))
		{
			return State.RelativeSource;
		}
		if (ParseOpenBrace(ref r))
		{
			if (PeekOpenBrace(ref r))
			{
				return State.TypeCast;
			}
			return State.AttachedProperty;
		}
		if (PeekOpenBracket(ref r))
		{
			return State.Indexer;
		}
		if (ParseDot(ref r))
		{
			nodes.Add(new EmptyExpressionNode());
			return State.End;
		}
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (!readOnlySpan.IsEmpty)
		{
			nodes.Add(new PropertyNameNode
			{
				PropertyName = readOnlySpan.ToString()
			});
			return State.AfterMember;
		}
		return State.End;
	}

	private static State ParseAfterMember(ref CharacterReader r, IList<INode> nodes)
	{
		if (ParseMemberAccessor(ref r))
		{
			return State.BeforeMember;
		}
		if (ParseStreamOperator(ref r))
		{
			nodes.Add(new StreamNode());
			return State.AfterMember;
		}
		if (PeekOpenBracket(ref r))
		{
			return State.Indexer;
		}
		if (ParseOpenBrace(ref r))
		{
			return State.TypeCast;
		}
		return State.End;
	}

	private static State ParseBeforeMember(ref CharacterReader r, IList<INode> nodes)
	{
		if (ParseOpenBrace(ref r))
		{
			if (PeekOpenBrace(ref r))
			{
				return State.TypeCast;
			}
			return State.AttachedProperty;
		}
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (!readOnlySpan.IsEmpty)
		{
			nodes.Add(new PropertyNameNode
			{
				PropertyName = readOnlySpan.ToString()
			});
			return State.AfterMember;
		}
		return State.End;
	}

	private static State ParseAttachedProperty(scoped ref CharacterReader r, List<INode> nodes)
	{
		var (@namespace, typeName2) = (TypeName)(ref ParseTypeName(ref r));
		if (!r.End && r.TakeIf(')'))
		{
			nodes.Add(new TypeCastNode
			{
				Namespace = @namespace,
				TypeName = typeName2
			});
			return State.AfterMember;
		}
		if (r.End || !r.TakeIf('.'))
		{
			throw new ExpressionParseException(r.Position, "Invalid attached property name.");
		}
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.Length == 0)
		{
			throw new ExpressionParseException(r.Position, "Attached Property name expected after '.'.");
		}
		if (r.End || !r.TakeIf(')'))
		{
			throw new ExpressionParseException(r.Position, "Expected ')'.");
		}
		nodes.Add(new AttachedPropertyNameNode
		{
			Namespace = @namespace,
			TypeName = typeName2,
			PropertyName = readOnlySpan.ToString()
		});
		return State.AfterMember;
	}

	private static State ParseIndexer(ref CharacterReader r, List<INode> nodes)
	{
		IList<string> list = r.ParseArguments('[', ']');
		if (list.Count == 0)
		{
			throw new ExpressionParseException(r.Position, "Indexer may not be empty.");
		}
		nodes.Add(new IndexerNode
		{
			Arguments = list
		});
		return State.AfterMember;
	}

	private static State ParseTypeCast(ref CharacterReader r, List<INode> nodes)
	{
		bool num = ParseOpenBrace(ref r);
		ParseTypeName(ref r).Deconstruct(out string ns, out string typeName);
		string @namespace = ns;
		string typeName2 = typeName;
		State result = State.AfterMember;
		if (num)
		{
			if (!ParseCloseBrace(ref r))
			{
				throw new ExpressionParseException(r.Position, "Expected ')'.");
			}
			result = ParseBeforeMember(ref r, nodes);
			if (r.Peek == '[')
			{
				result = ParseIndexer(ref r, nodes);
			}
		}
		nodes.Add(new TypeCastNode
		{
			Namespace = @namespace,
			TypeName = typeName2
		});
		if (r.End || !r.TakeIf(')'))
		{
			throw new ExpressionParseException(r.Position, "Expected ')'.");
		}
		return result;
	}

	private static State ParseElementName(ref CharacterReader r, List<INode> nodes)
	{
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		if (readOnlySpan.IsEmpty)
		{
			throw new ExpressionParseException(r.Position, "Element name expected after '#'.");
		}
		nodes.Add(new NameNode
		{
			Name = readOnlySpan.ToString()
		});
		return State.AfterMember;
	}

	private static State ParseRelativeSource(ref CharacterReader r, List<INode> nodes)
	{
		ReadOnlySpan<char> span = r.ParseIdentifier();
		if (span.SequenceEqual("self".AsSpan()))
		{
			nodes.Add(new SelfNode());
		}
		else
		{
			if (!span.SequenceEqual("parent".AsSpan()))
			{
				throw new ExpressionParseException(r.Position, "Unknown RelativeSource mode.");
			}
			string @namespace = null;
			string typeName = null;
			int level = 0;
			if (PeekOpenBracket(ref r))
			{
				IList<string> list = r.ParseArguments('[', ']', ';');
				if (list.Count > 2 || list.Count == 0)
				{
					throw new ExpressionParseException(r.Position, "Too many arguments in RelativeSource syntax sugar");
				}
				string typeName2;
				string ns;
				if (list.Count == 1)
				{
					if (int.TryParse(list[0], out var result))
					{
						typeName = null;
						level = result;
					}
					else
					{
						CharacterReader r2 = new CharacterReader(list[0].AsSpan());
						ParseTypeName(ref r2).Deconstruct(out typeName2, out ns);
						@namespace = typeName2;
						typeName = ns;
					}
				}
				else
				{
					CharacterReader r3 = new CharacterReader(list[0].AsSpan());
					ParseTypeName(ref r3).Deconstruct(out ns, out typeName2);
					@namespace = ns;
					typeName = typeName2;
					level = int.Parse(list[1]);
				}
			}
			nodes.Add(new AncestorNode
			{
				Namespace = @namespace,
				TypeName = typeName,
				Level = level
			});
		}
		return State.AfterMember;
	}

	private static TypeName ParseTypeName(scoped ref CharacterReader r)
	{
		ReadOnlySpan<char> ns = ReadOnlySpan<char>.Empty;
		ReadOnlySpan<char> readOnlySpan = r.ParseIdentifier();
		ReadOnlySpan<char> typeName;
		if (!r.End && r.TakeIf(':'))
		{
			ns = readOnlySpan;
			typeName = r.ParseIdentifier();
		}
		else
		{
			typeName = readOnlySpan;
		}
		return new TypeName(ns, typeName);
	}

	private static bool ParseNot(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('!');
		}
		return false;
	}

	private static bool ParseMemberAccessor(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('.');
		}
		return false;
	}

	private static bool ParseOpenBrace(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('(');
		}
		return false;
	}

	private static bool ParseCloseBrace(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf(')');
		}
		return false;
	}

	private static bool PeekOpenBracket(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.Peek == '[';
		}
		return false;
	}

	private static bool PeekOpenBrace(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.Peek == '(';
		}
		return false;
	}

	private static bool ParseStreamOperator(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('^');
		}
		return false;
	}

	private static bool ParseDollarSign(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('$');
		}
		return false;
	}

	private static bool ParseSharp(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('#');
		}
		return false;
	}

	private static bool ParseDot(ref CharacterReader r)
	{
		if (!r.End)
		{
			return r.TakeIf('.');
		}
		return false;
	}
}
