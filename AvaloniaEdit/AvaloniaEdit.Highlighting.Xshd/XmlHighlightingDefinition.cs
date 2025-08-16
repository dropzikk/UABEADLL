using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting.Xshd;

internal sealed class XmlHighlightingDefinition : IHighlightingDefinition
{
	private sealed class RegisterNamedElementsVisitor : IXshdVisitor
	{
		private readonly XmlHighlightingDefinition _def;

		internal readonly Dictionary<XshdRuleSet, HighlightingRuleSet> RuleSets = new Dictionary<XshdRuleSet, HighlightingRuleSet>();

		public RegisterNamedElementsVisitor(XmlHighlightingDefinition def)
		{
			_def = def;
		}

		public object VisitRuleSet(XshdRuleSet ruleSet)
		{
			HighlightingRuleSet value = new HighlightingRuleSet();
			RuleSets.Add(ruleSet, value);
			if (ruleSet.Name != null)
			{
				if (ruleSet.Name.Length == 0)
				{
					throw Error(ruleSet, "Name must not be the empty string");
				}
				if (_def._ruleSetDict.ContainsKey(ruleSet.Name))
				{
					throw Error(ruleSet, "Duplicate rule set name '" + ruleSet.Name + "'.");
				}
				_def._ruleSetDict.Add(ruleSet.Name, value);
			}
			ruleSet.AcceptElements(this);
			return null;
		}

		public object VisitColor(XshdColor color)
		{
			if (color.Name != null)
			{
				if (color.Name.Length == 0)
				{
					throw Error(color, "Name must not be the empty string");
				}
				if (_def._colorDict.ContainsKey(color.Name))
				{
					throw Error(color, "Duplicate color name '" + color.Name + "'.");
				}
				_def._colorDict.Add(color.Name, new HighlightingColor());
			}
			return null;
		}

		public object VisitKeywords(XshdKeywords keywords)
		{
			return keywords.ColorReference.AcceptVisitor(this);
		}

		public object VisitSpan(XshdSpan span)
		{
			span.BeginColorReference.AcceptVisitor(this);
			span.SpanColorReference.AcceptVisitor(this);
			span.EndColorReference.AcceptVisitor(this);
			return span.RuleSetReference.AcceptVisitor(this);
		}

		public object VisitImport(XshdImport import)
		{
			return import.RuleSetReference.AcceptVisitor(this);
		}

		public object VisitRule(XshdRule rule)
		{
			return rule.ColorReference.AcceptVisitor(this);
		}
	}

	private sealed class TranslateElementVisitor : IXshdVisitor
	{
		private readonly XmlHighlightingDefinition _def;

		private readonly Dictionary<XshdRuleSet, HighlightingRuleSet> _ruleSetDict;

		private readonly Dictionary<HighlightingRuleSet, XshdRuleSet> _reverseRuleSetDict;

		private readonly IHighlightingDefinitionReferenceResolver _resolver;

		private readonly HashSet<XshdRuleSet> _processingStartedRuleSets = new HashSet<XshdRuleSet>();

		private readonly HashSet<XshdRuleSet> _processedRuleSets = new HashSet<XshdRuleSet>();

		private bool _ignoreCase;

		public TranslateElementVisitor(XmlHighlightingDefinition def, Dictionary<XshdRuleSet, HighlightingRuleSet> ruleSetDict, IHighlightingDefinitionReferenceResolver resolver)
		{
			_def = def;
			_ruleSetDict = ruleSetDict;
			_resolver = resolver;
			_reverseRuleSetDict = new Dictionary<HighlightingRuleSet, XshdRuleSet>();
			foreach (KeyValuePair<XshdRuleSet, HighlightingRuleSet> item in ruleSetDict)
			{
				_reverseRuleSetDict.Add(item.Value, item.Key);
			}
		}

		public object VisitRuleSet(XshdRuleSet ruleSet)
		{
			HighlightingRuleSet highlightingRuleSet = _ruleSetDict[ruleSet];
			if (_processedRuleSets.Contains(ruleSet))
			{
				return highlightingRuleSet;
			}
			if (!_processingStartedRuleSets.Add(ruleSet))
			{
				throw Error(ruleSet, "RuleSet cannot be processed because it contains cyclic <Import>");
			}
			bool ignoreCase = _ignoreCase;
			if (ruleSet.IgnoreCase.HasValue)
			{
				_ignoreCase = ruleSet.IgnoreCase.Value;
			}
			highlightingRuleSet.Name = ruleSet.Name;
			foreach (XshdElement element in ruleSet.Elements)
			{
				object obj = element.AcceptVisitor(this);
				if (obj is HighlightingRuleSet source)
				{
					Merge(highlightingRuleSet, source);
				}
				else if (obj is HighlightingSpan item)
				{
					highlightingRuleSet.Spans.Add(item);
				}
				else if (obj is HighlightingRule item2)
				{
					highlightingRuleSet.Rules.Add(item2);
				}
			}
			_ignoreCase = ignoreCase;
			_processedRuleSets.Add(ruleSet);
			return highlightingRuleSet;
		}

		private static void Merge(HighlightingRuleSet target, HighlightingRuleSet source)
		{
			target.Rules.AddRange(source.Rules);
			target.Spans.AddRange(source.Spans);
		}

		public object VisitColor(XshdColor color)
		{
			HighlightingColor highlightingColor;
			if (color.Name != null)
			{
				highlightingColor = _def._colorDict[color.Name];
			}
			else
			{
				if (color.Foreground == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue)
				{
					return null;
				}
				highlightingColor = new HighlightingColor();
			}
			highlightingColor.Name = color.Name;
			highlightingColor.Foreground = color.Foreground;
			highlightingColor.Background = color.Background;
			highlightingColor.Underline = color.Underline;
			highlightingColor.Strikethrough = color.Strikethrough;
			highlightingColor.FontStyle = color.FontStyle;
			highlightingColor.FontWeight = color.FontWeight;
			highlightingColor.FontFamily = color.FontFamily;
			highlightingColor.FontSize = color.FontSize;
			return highlightingColor;
		}

		public object VisitKeywords(XshdKeywords keywords)
		{
			if (keywords.Words.Count == 0)
			{
				return Error(keywords, "Keyword group must not be empty.");
			}
			foreach (string word in keywords.Words)
			{
				if (string.IsNullOrEmpty(word))
				{
					throw Error(keywords, "Cannot use empty string as keyword");
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (keywords.Words.All(IsSimpleWord))
			{
				stringBuilder.Append("\\b(?>");
				int num = 0;
				foreach (string item in keywords.Words.OrderByDescending((string w) => w.Length))
				{
					if (num++ > 0)
					{
						stringBuilder.Append('|');
					}
					stringBuilder.Append(Regex.Escape(item));
				}
				stringBuilder.Append(")\\b");
			}
			else
			{
				stringBuilder.Append('(');
				int num2 = 0;
				foreach (string word2 in keywords.Words)
				{
					if (num2++ > 0)
					{
						stringBuilder.Append('|');
					}
					if (char.IsLetterOrDigit(word2[0]))
					{
						stringBuilder.Append("\\b");
					}
					stringBuilder.Append(Regex.Escape(word2));
					if (char.IsLetterOrDigit(word2[word2.Length - 1]))
					{
						stringBuilder.Append("\\b");
					}
				}
				stringBuilder.Append(')');
			}
			return new HighlightingRule
			{
				Color = GetColor(keywords, keywords.ColorReference),
				Regex = CreateRegex(keywords, stringBuilder.ToString(), XshdRegexType.Default)
			};
		}

		private static bool IsSimpleWord(string word)
		{
			if (char.IsLetterOrDigit(word[0]))
			{
				return char.IsLetterOrDigit(word, word.Length - 1);
			}
			return false;
		}

		private Regex CreateRegex(XshdElement position, string regex, XshdRegexType regexType)
		{
			if (regex == null)
			{
				throw Error(position, "Regex missing");
			}
			RegexOptions regexOptions = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
			if (regexType == XshdRegexType.IgnorePatternWhitespace)
			{
				regexOptions |= RegexOptions.IgnorePatternWhitespace;
			}
			if (_ignoreCase)
			{
				regexOptions |= RegexOptions.IgnoreCase;
			}
			try
			{
				return new Regex(regex, regexOptions);
			}
			catch (ArgumentException ex)
			{
				throw Error(position, ex.Message);
			}
		}

		private HighlightingColor GetColor(XshdElement position, XshdReference<XshdColor> colorReference)
		{
			if (colorReference.InlineElement != null)
			{
				return (HighlightingColor)colorReference.InlineElement.AcceptVisitor(this);
			}
			if (colorReference.ReferencedElement != null)
			{
				return GetDefinition(position, colorReference.ReferencedDefinition).GetNamedColor(colorReference.ReferencedElement) ?? throw Error(position, "Could not find color named '" + colorReference.ReferencedElement + "'.");
			}
			return null;
		}

		private IHighlightingDefinition GetDefinition(XshdElement position, string definitionName)
		{
			if (definitionName == null)
			{
				return _def;
			}
			if (_resolver == null)
			{
				throw Error(position, "Resolving references to other syntax definitions is not possible because the IHighlightingDefinitionReferenceResolver is null.");
			}
			return _resolver.GetDefinition(definitionName) ?? throw Error(position, "Could not find definition with name '" + definitionName + "'.");
		}

		private HighlightingRuleSet GetRuleSet(XshdElement position, XshdReference<XshdRuleSet> ruleSetReference)
		{
			if (ruleSetReference.InlineElement != null)
			{
				return (HighlightingRuleSet)ruleSetReference.InlineElement.AcceptVisitor(this);
			}
			if (ruleSetReference.ReferencedElement != null)
			{
				return GetDefinition(position, ruleSetReference.ReferencedDefinition).GetNamedRuleSet(ruleSetReference.ReferencedElement) ?? throw Error(position, "Could not find rule set named '" + ruleSetReference.ReferencedElement + "'.");
			}
			return null;
		}

		public object VisitSpan(XshdSpan span)
		{
			string text = span.EndRegex;
			if (string.IsNullOrEmpty(span.BeginRegex) && string.IsNullOrEmpty(span.EndRegex))
			{
				throw Error(span, "Span has no start/end regex.");
			}
			if (!span.Multiline)
			{
				text = ((text == null) ? "$" : ((span.EndRegexType != XshdRegexType.IgnorePatternWhitespace) ? ("($|" + text + ")") : ("($|" + text + "\n)")));
			}
			HighlightingColor color = GetColor(span, span.SpanColorReference);
			return new HighlightingSpan
			{
				StartExpression = CreateRegex(span, span.BeginRegex, span.BeginRegexType),
				EndExpression = CreateRegex(span, text, span.EndRegexType),
				RuleSet = GetRuleSet(span, span.RuleSetReference),
				StartColor = GetColor(span, span.BeginColorReference),
				SpanColor = color,
				EndColor = GetColor(span, span.EndColorReference),
				SpanColorIncludesStart = true,
				SpanColorIncludesEnd = true
			};
		}

		public object VisitImport(XshdImport import)
		{
			HighlightingRuleSet ruleSet = GetRuleSet(import, import.RuleSetReference);
			if (_reverseRuleSetDict.TryGetValue(ruleSet, out var value))
			{
				VisitRuleSet(value);
			}
			return ruleSet;
		}

		public object VisitRule(XshdRule rule)
		{
			return new HighlightingRule
			{
				Color = GetColor(rule, rule.ColorReference),
				Regex = CreateRegex(rule, rule.Regex, rule.RegexType)
			};
		}
	}

	private readonly Dictionary<string, HighlightingRuleSet> _ruleSetDict = new Dictionary<string, HighlightingRuleSet>();

	private readonly Dictionary<string, HighlightingColor> _colorDict = new Dictionary<string, HighlightingColor>();

	private readonly Dictionary<string, string> _propDict = new Dictionary<string, string>();

	public string Name { get; }

	public HighlightingRuleSet MainRuleSet { get; }

	public IEnumerable<HighlightingColor> NamedHighlightingColors => _colorDict.Values;

	public IDictionary<string, string> Properties => _propDict;

	public XmlHighlightingDefinition(XshdSyntaxDefinition xshd, IHighlightingDefinitionReferenceResolver resolver)
	{
		Name = xshd.Name;
		RegisterNamedElementsVisitor registerNamedElementsVisitor = new RegisterNamedElementsVisitor(this);
		xshd.AcceptElements(registerNamedElementsVisitor);
		foreach (XshdElement element in xshd.Elements)
		{
			if (element is XshdRuleSet { Name: null } xshdRuleSet)
			{
				if (MainRuleSet != null)
				{
					throw Error(element, "Duplicate main RuleSet. There must be only one nameless RuleSet!");
				}
				MainRuleSet = registerNamedElementsVisitor.RuleSets[xshdRuleSet];
			}
		}
		if (MainRuleSet == null)
		{
			throw new HighlightingDefinitionInvalidException("Could not find main RuleSet.");
		}
		xshd.AcceptElements(new TranslateElementVisitor(this, registerNamedElementsVisitor.RuleSets, resolver));
		foreach (XshdProperty item in xshd.Elements.OfType<XshdProperty>())
		{
			_propDict.Add(item.Name, item.Value);
		}
	}

	private static Exception Error(XshdElement element, string message)
	{
		if (element.LineNumber > 0)
		{
			return new HighlightingDefinitionInvalidException("Error at line " + element.LineNumber + ":\n" + message);
		}
		return new HighlightingDefinitionInvalidException(message);
	}

	public HighlightingRuleSet GetNamedRuleSet(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return MainRuleSet;
		}
		if (!_ruleSetDict.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public HighlightingColor GetNamedColor(string name)
	{
		if (!_colorDict.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public override string ToString()
	{
		return Name;
	}
}
