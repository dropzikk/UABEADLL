using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextMateSharp.Internal.Rules;
using TextMateSharp.Internal.Types;

namespace TextMateSharp.Internal.Grammars.Parser;

public class Raw : Dictionary<string, object>, IRawRepository, IRawRule, IRawGrammar, IRawCaptures, IEnumerable<string>, IEnumerable
{
	private static string FIRST_LINE_MATCH = "firstLineMatch";

	private static string FILE_TYPES = "fileTypes";

	private static string SCOPE_NAME = "scopeName";

	private static string APPLY_END_PATTERN_LAST = "applyEndPatternLast";

	private static string REPOSITORY = "repository";

	private static string INJECTION_SELECTOR = "injectionSelector";

	private static string INJECTIONS = "injections";

	private static string PATTERNS = "patterns";

	private static string WHILE_CAPTURES = "whileCaptures";

	private static string END_CAPTURES = "endCaptures";

	private static string INCLUDE = "include";

	private static string WHILE = "while";

	private static string END = "end";

	private static string BEGIN = "begin";

	private static string CAPTURES = "captures";

	private static string MATCH = "match";

	private static string BEGIN_CAPTURES = "beginCaptures";

	private static string CONTENT_NAME = "contentName";

	private static string NAME = "name";

	private static string ID = "id";

	private static string DOLLAR_SELF = "$self";

	private static string DOLLAR_BASE = "$base";

	private List<string> fileTypes;

	public IRawRepository Merge(params IRawRepository[] sources)
	{
		Raw target = new Raw();
		for (int i = 0; i < sources.Length; i++)
		{
			Raw sourceRaw = (Raw)sources[i];
			foreach (string key in sourceRaw.Keys)
			{
				target[key] = sourceRaw[key];
			}
		}
		return target;
	}

	public IRawRule GetProp(string name)
	{
		return TryGetObject<IRawRule>(name);
	}

	public IRawRule GetBase()
	{
		return TryGetObject<IRawRule>(DOLLAR_BASE);
	}

	public void SetBase(IRawRule ruleBase)
	{
		base[DOLLAR_BASE] = ruleBase;
	}

	public IRawRule GetSelf()
	{
		return TryGetObject<IRawRule>(DOLLAR_SELF);
	}

	public void SetSelf(IRawRule self)
	{
		base[DOLLAR_SELF] = self;
	}

	public RuleId GetId()
	{
		return TryGetObject<RuleId>(ID);
	}

	public void SetId(RuleId id)
	{
		base[ID] = id;
	}

	public string GetName()
	{
		return TryGetObject<string>(NAME);
	}

	public void SetName(string name)
	{
		base[NAME] = name;
	}

	public string GetContentName()
	{
		return TryGetObject<string>(CONTENT_NAME);
	}

	public string GetMatch()
	{
		return TryGetObject<string>(MATCH);
	}

	public IRawCaptures GetCaptures()
	{
		UpdateCaptures(CAPTURES);
		return TryGetObject<IRawCaptures>(CAPTURES);
	}

	private void UpdateCaptures(string name)
	{
		object captures = TryGetObject<object>(name);
		if (!(captures is IList))
		{
			return;
		}
		Raw rawCaptures = new Raw();
		int i = 0;
		foreach (object capture in (IList)captures)
		{
			i++;
			rawCaptures[i.ToString() ?? ""] = capture;
		}
		base[name] = rawCaptures;
	}

	public string GetBegin()
	{
		return TryGetObject<string>(BEGIN);
	}

	public string GetWhile()
	{
		return TryGetObject<string>(WHILE);
	}

	public string GetInclude()
	{
		return TryGetObject<string>(INCLUDE);
	}

	public void SetInclude(string include)
	{
		base[INCLUDE] = include;
	}

	public IRawCaptures GetBeginCaptures()
	{
		UpdateCaptures(BEGIN_CAPTURES);
		return TryGetObject<IRawCaptures>(BEGIN_CAPTURES);
	}

	public void SetBeginCaptures(IRawCaptures beginCaptures)
	{
		base[BEGIN_CAPTURES] = beginCaptures;
	}

	public string GetEnd()
	{
		return TryGetObject<string>(END);
	}

	public IRawCaptures GetEndCaptures()
	{
		UpdateCaptures(END_CAPTURES);
		return TryGetObject<IRawCaptures>(END_CAPTURES);
	}

	public IRawCaptures GetWhileCaptures()
	{
		UpdateCaptures(WHILE_CAPTURES);
		return TryGetObject<IRawCaptures>(WHILE_CAPTURES);
	}

	public ICollection<IRawRule> GetPatterns()
	{
		return TryGetObject<ICollection>(PATTERNS)?.Cast<IRawRule>().ToList();
	}

	public void SetPatterns(ICollection<IRawRule> patterns)
	{
		base[PATTERNS] = patterns;
	}

	public Dictionary<string, IRawRule> GetInjections()
	{
		Raw result = TryGetObject<Raw>(INJECTIONS);
		if (result == null)
		{
			return null;
		}
		return ConvertToDictionary<IRawRule>(result);
	}

	public string GetInjectionSelector()
	{
		return (string)base[INJECTION_SELECTOR];
	}

	public IRawRepository GetRepository()
	{
		return TryGetObject<IRawRepository>(REPOSITORY);
	}

	public void SetRepository(IRawRepository repository)
	{
		base[REPOSITORY] = repository;
	}

	public bool IsApplyEndPatternLast()
	{
		object applyEndPatternLast = TryGetObject<object>(APPLY_END_PATTERN_LAST);
		if (applyEndPatternLast == null)
		{
			return false;
		}
		if (applyEndPatternLast is bool)
		{
			return (bool)applyEndPatternLast;
		}
		if (applyEndPatternLast is int)
		{
			return (int)applyEndPatternLast == 1;
		}
		return false;
	}

	public void SetApplyEndPatternLast(bool applyEndPatternLast)
	{
		base[APPLY_END_PATTERN_LAST] = applyEndPatternLast;
	}

	public string GetScopeName()
	{
		return TryGetObject<string>(SCOPE_NAME);
	}

	public ICollection<string> GetFileTypes()
	{
		if (fileTypes == null)
		{
			List<string> list = new List<string>();
			ICollection unparsedFileTypes = TryGetObject<ICollection>(FILE_TYPES);
			if (unparsedFileTypes != null)
			{
				foreach (object item in unparsedFileTypes)
				{
					string str = item.ToString();
					if (str.StartsWith("."))
					{
						str = str.Substring(1);
					}
					list.Add(str);
				}
			}
			fileTypes = list;
		}
		return fileTypes;
	}

	public string GetFirstLineMatch()
	{
		return TryGetObject<string>(FIRST_LINE_MATCH);
	}

	public IRawRule GetCapture(string captureId)
	{
		return GetProp(captureId);
	}

	public IRawGrammar Clone()
	{
		return (IRawGrammar)Clone(this);
	}

	public object Clone(object value)
	{
		if (value is Raw)
		{
			Raw rawToClone = (Raw)value;
			Raw raw = new Raw();
			{
				foreach (string key in rawToClone.Keys)
				{
					raw[key] = Clone(rawToClone[key]);
				}
				return raw;
			}
		}
		if (value is IList)
		{
			List<object> result = new List<object>();
			{
				foreach (object obj in (IList)value)
				{
					result.Add(Clone(obj));
				}
				return result;
			}
		}
		if (value is string)
		{
			return value;
		}
		if (value is int)
		{
			return value;
		}
		_ = value is bool;
		return value;
	}

	IEnumerator<string> IEnumerable<string>.GetEnumerator()
	{
		return base.Keys.GetEnumerator();
	}

	private Dictionary<string, T> ConvertToDictionary<T>(Raw raw)
	{
		Dictionary<string, T> result = new Dictionary<string, T>();
		foreach (string key in raw.Keys)
		{
			result.Add(key, (T)raw[key]);
		}
		return result;
	}

	private T TryGetObject<T>(string key)
	{
		if (!TryGetValue(key, out var result))
		{
			return default(T);
		}
		return (T)result;
	}
}
