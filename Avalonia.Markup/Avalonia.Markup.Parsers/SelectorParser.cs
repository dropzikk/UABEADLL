using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Avalonia.Styling;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal class SelectorParser
{
	private readonly Func<string, string, Type> _typeResolver;

	public SelectorParser(Func<string, string, Type> typeResolver)
	{
		_typeResolver = typeResolver;
	}

	[RequiresUnreferencedCode("Selectors runtime parser might require unreferenced code. Consider using stronly typed selectors factory with 'new Style(s => s.OfType<Button>())' syntax.")]
	public Selector? Parse(string s)
	{
		IEnumerable<SelectorGrammar.ISyntax> syntax = SelectorGrammar.Parse(s);
		return Create(syntax);
	}

	[RequiresUnreferencedCode("Selectors runtime parser might require unreferenced code. Consider using stronly typed selectors factory with 'new Style(s => s.OfType<Button>())' syntax.")]
	private Selector? Create(IEnumerable<SelectorGrammar.ISyntax> syntax)
	{
		Selector selector = null;
		List<Selector> list = null;
		foreach (SelectorGrammar.ISyntax item in syntax)
		{
			if (!(item is SelectorGrammar.OfTypeSyntax ofTypeSyntax))
			{
				if (!(item is SelectorGrammar.IsSyntax isSyntax))
				{
					if (!(item is SelectorGrammar.ClassSyntax classSyntax))
					{
						if (!(item is SelectorGrammar.NameSyntax nameSyntax))
						{
							if (!(item is SelectorGrammar.PropertySyntax propertySyntax))
							{
								SelectorGrammar.AttachedPropertySyntax attachedPropertySyntax = item as SelectorGrammar.AttachedPropertySyntax;
								if (attachedPropertySyntax == null)
								{
									if (!(item is SelectorGrammar.ChildSyntax))
									{
										if (!(item is SelectorGrammar.DescendantSyntax))
										{
											if (!(item is SelectorGrammar.TemplateSyntax))
											{
												SelectorGrammar.NotSyntax notSyntax = item as SelectorGrammar.NotSyntax;
												if (notSyntax == null)
												{
													if (!(item is SelectorGrammar.NthChildSyntax nthChildSyntax))
													{
														if (!(item is SelectorGrammar.NthLastChildSyntax nthLastChildSyntax))
														{
															if (!(item is SelectorGrammar.CommaSyntax))
															{
																throw new NotSupportedException($"Unsupported selector grammar '{item.GetType()}'.");
															}
															if (list == null)
															{
																list = new List<Selector>();
															}
															list.Add(selector ?? throw new NotSupportedException("Invalid selector!"));
															selector = null;
														}
														else
														{
															selector = selector.NthLastChild(nthLastChildSyntax.Step, nthLastChildSyntax.Offset);
														}
													}
													else
													{
														selector = selector.NthChild(nthChildSyntax.Step, nthChildSyntax.Offset);
													}
												}
												else
												{
													selector = selector.Not((Selector? x) => Create(notSyntax.Argument));
												}
											}
											else
											{
												selector = selector.Template();
											}
										}
										else
										{
											selector = selector.Descendant();
										}
									}
									else
									{
										selector = selector.Child();
									}
								}
								else
								{
									Type type = selector?.TargetType;
									if (type == null)
									{
										throw new InvalidOperationException("Attached Property selectors must be applied to a type.");
									}
									Type attachedPropertyOwnerType = Resolve(attachedPropertySyntax.Xmlns, attachedPropertySyntax.TypeName);
									if ((object)attachedPropertyOwnerType == null)
									{
										throw new InvalidOperationException("Cannot find '" + attachedPropertySyntax.Xmlns + ":" + attachedPropertySyntax.TypeName);
									}
									AvaloniaProperty avaloniaProperty = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(type).FirstOrDefault((AvaloniaProperty ap) => ap.OwnerType == attachedPropertyOwnerType && ap.Name == attachedPropertySyntax.Property);
									if (avaloniaProperty == null)
									{
										throw new InvalidOperationException($"Cannot find '{attachedPropertySyntax.Property}' on '{attachedPropertyOwnerType}");
									}
									if (!TypeUtilities.TryConvert(avaloniaProperty.PropertyType, attachedPropertySyntax.Value, CultureInfo.InvariantCulture, out object result))
									{
										throw new InvalidOperationException($"Could not convert '{attachedPropertySyntax.Value}' to '{avaloniaProperty.PropertyType}");
									}
									selector = selector.PropertyEquals(avaloniaProperty, result);
								}
							}
							else
							{
								Type type2 = selector?.TargetType;
								if (type2 == null)
								{
									throw new InvalidOperationException("Property selectors must be applied to a type.");
								}
								AvaloniaProperty avaloniaProperty2 = AvaloniaPropertyRegistry.Instance.FindRegistered(type2, propertySyntax.Property);
								if (avaloniaProperty2 == null)
								{
									throw new InvalidOperationException($"Cannot find '{propertySyntax.Property}' on '{type2}");
								}
								if (!TypeUtilities.TryConvert(avaloniaProperty2.PropertyType, propertySyntax.Value, CultureInfo.InvariantCulture, out object result2))
								{
									throw new InvalidOperationException($"Could not convert '{propertySyntax.Value}' to '{avaloniaProperty2.PropertyType}");
								}
								selector = selector.PropertyEquals(avaloniaProperty2, result2);
							}
						}
						else
						{
							selector = selector.Name(nameSyntax.Name);
						}
					}
					else
					{
						selector = selector.Class(classSyntax.Class);
					}
				}
				else
				{
					selector = selector.Is(Resolve(isSyntax.Xmlns, isSyntax.TypeName));
				}
			}
			else
			{
				selector = selector.OfType(Resolve(ofTypeSyntax.Xmlns, ofTypeSyntax.TypeName));
			}
		}
		if (list != null)
		{
			if (selector != null)
			{
				list.Add(selector);
			}
			selector = ((list.Count > 1) ? Selectors.Or(list) : list[0]);
		}
		return selector;
	}

	private Type Resolve(string xmlns, string typeName)
	{
		Type type = _typeResolver(xmlns, typeName);
		if (type == null)
		{
			string text = (string.IsNullOrWhiteSpace(xmlns) ? typeName : (xmlns + ":" + typeName));
			throw new InvalidOperationException("Could not resolve type '" + text + "'");
		}
		return type;
	}
}
