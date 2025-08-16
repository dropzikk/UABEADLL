using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Parsers.Nodes;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

public class CompiledBindingPath
{
	private readonly ICompiledBindingPathElement[] _elements;

	internal IEnumerable<ICompiledBindingPathElement> Elements => _elements;

	internal SourceMode SourceMode
	{
		get
		{
			if (!Array.Exists(_elements, (ICompiledBindingPathElement e) => e is IControlSourceBindingPathElement))
			{
				return SourceMode.Data;
			}
			return SourceMode.Control;
		}
	}

	internal object? RawSource { get; }

	public CompiledBindingPath()
	{
		_elements = Array.Empty<ICompiledBindingPathElement>();
	}

	internal CompiledBindingPath(ICompiledBindingPathElement[] elements, object? rawSource)
	{
		_elements = elements;
		RawSource = rawSource;
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "CompiledBinding preserves members used in the expression tree.")]
	internal ExpressionNode BuildExpression(bool enableValidation)
	{
		ExpressionNode expressionNode = null;
		ExpressionNode expressionNode2 = null;
		ICompiledBindingPathElement[] elements = _elements;
		foreach (ICompiledBindingPathElement compiledBindingPathElement in elements)
		{
			ExpressionNode expressionNode3;
			if (!(compiledBindingPathElement is NotExpressionPathElement))
			{
				if (!(compiledBindingPathElement is PropertyElement propertyElement))
				{
					if (!(compiledBindingPathElement is MethodAsCommandElement methodAsCommandElement))
					{
						if (!(compiledBindingPathElement is MethodAsDelegateElement methodAsDelegateElement))
						{
							if (!(compiledBindingPathElement is ArrayElementPathElement arrayElementPathElement))
							{
								if (!(compiledBindingPathElement is VisualAncestorPathElement visualAncestorPathElement))
								{
									if (!(compiledBindingPathElement is AncestorPathElement ancestorPathElement))
									{
										if (!(compiledBindingPathElement is SelfPathElement))
										{
											if (!(compiledBindingPathElement is ElementNameElement elementNameElement))
											{
												if (!(compiledBindingPathElement is IStronglyTypedStreamElement stronglyTypedStreamElement))
												{
													if (!(compiledBindingPathElement is ITypeCastElement typeCastElement))
													{
														throw new InvalidOperationException("Unknown binding path element type " + compiledBindingPathElement.GetType().FullName);
													}
													expressionNode3 = new StrongTypeCastNode(typeCastElement.Type, typeCastElement.Cast);
												}
												else
												{
													expressionNode3 = new StreamNode(stronglyTypedStreamElement.CreatePlugin());
												}
											}
											else
											{
												expressionNode3 = new ElementNameNode(elementNameElement.NameScope, elementNameElement.Name);
											}
										}
										else
										{
											expressionNode3 = new SelfNode();
										}
									}
									else
									{
										expressionNode3 = new FindAncestorNode(ancestorPathElement.AncestorType, ancestorPathElement.Level);
									}
								}
								else
								{
									expressionNode3 = new FindVisualAncestorNode(visualAncestorPathElement.AncestorType, visualAncestorPathElement.Level);
								}
							}
							else
							{
								expressionNode3 = new PropertyAccessorNode("Item", enableValidation, new ArrayElementPlugin(arrayElementPathElement.Indices, arrayElementPathElement.ElementType));
							}
						}
						else
						{
							expressionNode3 = new PropertyAccessorNode(methodAsDelegateElement.Method.Name, enableValidation, new MethodAccessorPlugin(methodAsDelegateElement.Method, methodAsDelegateElement.DelegateType));
						}
					}
					else
					{
						expressionNode3 = new PropertyAccessorNode(methodAsCommandElement.MethodName, enableValidation, new CommandAccessorPlugin(methodAsCommandElement.ExecuteMethod, methodAsCommandElement.CanExecuteMethod, methodAsCommandElement.DependsOnProperties));
					}
				}
				else
				{
					expressionNode3 = new PropertyAccessorNode(propertyElement.Property.Name, enableValidation, new PropertyInfoAccessorPlugin(propertyElement.Property, propertyElement.AccessorFactory));
				}
			}
			else
			{
				expressionNode3 = new LogicalNotNode();
			}
			ExpressionNode expressionNode6;
			if (expressionNode != null)
			{
				ExpressionNode expressionNode5 = (expressionNode2.Next = expressionNode3);
				expressionNode6 = expressionNode5;
			}
			else
			{
				expressionNode6 = (expressionNode = expressionNode3);
			}
			expressionNode2 = expressionNode6;
		}
		return expressionNode ?? new EmptyExpressionNode();
	}

	public override string ToString()
	{
		return string.Concat((IEnumerable<ICompiledBindingPathElement>)_elements);
	}
}
