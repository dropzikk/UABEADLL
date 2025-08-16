using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers.Nodes;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal class ExpressionParser
{
	private readonly bool _enableValidation;

	private readonly Func<string?, string, Type>? _typeResolver;

	private readonly INameScope? _nameScope;

	public ExpressionParser(bool enableValidation, Func<string?, string, Type>? typeResolver, INameScope? nameScope)
	{
		_typeResolver = typeResolver;
		_nameScope = nameScope;
		_enableValidation = enableValidation;
	}

	[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
	public (ExpressionNode Node, SourceMode Mode) Parse(ref CharacterReader r)
	{
		ExpressionNode expressionNode = null;
		ExpressionNode expressionNode2 = null;
		var (list, item) = BindingExpressionGrammar.Parse(ref r);
		foreach (BindingExpressionGrammar.INode item2 in list)
		{
			ExpressionNode expressionNode3 = null;
			if (!(item2 is BindingExpressionGrammar.EmptyExpressionNode))
			{
				if (!(item2 is BindingExpressionGrammar.NotNode))
				{
					if (!(item2 is BindingExpressionGrammar.StreamNode))
					{
						if (!(item2 is BindingExpressionGrammar.PropertyNameNode propertyNameNode))
						{
							if (!(item2 is BindingExpressionGrammar.IndexerNode indexerNode))
							{
								if (!(item2 is BindingExpressionGrammar.AttachedPropertyNameNode node))
								{
									if (!(item2 is BindingExpressionGrammar.SelfNode))
									{
										if (!(item2 is BindingExpressionGrammar.AncestorNode node2))
										{
											if (!(item2 is BindingExpressionGrammar.NameNode nameNode))
											{
												if (item2 is BindingExpressionGrammar.TypeCastNode node3)
												{
													expressionNode3 = ParseTypeCastNode(node3);
												}
											}
											else
											{
												expressionNode3 = new ElementNameNode(_nameScope ?? throw new NotSupportedException("Invalid element name binding with null name scope!"), nameNode.Name);
											}
										}
										else
										{
											expressionNode3 = ParseFindAncestor(node2);
										}
									}
									else
									{
										expressionNode3 = new SelfNode();
									}
								}
								else
								{
									expressionNode3 = ParseAttachedProperty(node);
								}
							}
							else
							{
								expressionNode3 = new StringIndexerNode(indexerNode.Arguments);
							}
						}
						else
						{
							expressionNode3 = new PropertyAccessorNode(propertyNameNode.PropertyName, _enableValidation);
						}
					}
					else
					{
						expressionNode3 = new StreamNode();
					}
				}
				else
				{
					expressionNode3 = new LogicalNotNode();
				}
			}
			else
			{
				expressionNode3 = new EmptyExpressionNode();
			}
			if (expressionNode2 == null)
			{
				expressionNode = (expressionNode2 = expressionNode3);
				continue;
			}
			expressionNode2.Next = expressionNode3;
			expressionNode2 = expressionNode3;
		}
		if (expressionNode == null)
		{
			throw new ExpressionParseException(r.Position, "Unexpected end of expression.");
		}
		return (Node: expressionNode, Mode: item);
	}

	private FindAncestorNode ParseFindAncestor(BindingExpressionGrammar.AncestorNode node)
	{
		Type ancestorType = null;
		int level = node.Level;
		if (!string.IsNullOrEmpty(node.TypeName))
		{
			if (_typeResolver == null)
			{
				throw new InvalidOperationException("Cannot parse a binding path with a typed FindAncestor without a type resolver. Maybe you can use a LINQ Expression binding path instead?");
			}
			ancestorType = _typeResolver(node.Namespace, node.TypeName);
		}
		return new FindAncestorNode(ancestorType, level);
	}

	private TypeCastNode ParseTypeCastNode(BindingExpressionGrammar.TypeCastNode node)
	{
		Type type = null;
		if (!string.IsNullOrEmpty(node.TypeName))
		{
			if (_typeResolver == null)
			{
				throw new InvalidOperationException("Cannot parse a binding path with a typed Cast without a type resolver. Maybe you can use a LINQ Expression binding path instead?");
			}
			type = _typeResolver(node.Namespace, node.TypeName);
		}
		if ((object)type == null)
		{
			throw new InvalidOperationException("Unable to determine type for cast.");
		}
		return new TypeCastNode(type);
	}

	private AvaloniaPropertyAccessorNode ParseAttachedProperty(BindingExpressionGrammar.AttachedPropertyNameNode node)
	{
		if (_typeResolver == null)
		{
			throw new InvalidOperationException("Cannot parse a binding path with an attached property without a type resolver. Maybe you can use a LINQ Expression binding path instead?");
		}
		Type type = _typeResolver(node.Namespace, node.TypeName);
		return new AvaloniaPropertyAccessorNode(AvaloniaPropertyRegistry.Instance.FindRegistered(type, node.PropertyName) ?? throw new InvalidOperationException($"Cannot find property {type}.{node.PropertyName}."), _enableValidation);
	}
}
