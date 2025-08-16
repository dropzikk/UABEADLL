using TextMateSharp.Internal.Oniguruma;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Internal.Rules;

public abstract class Rule
{
	private bool _nameIsCapturing;

	private string _name;

	private bool _contentNameIsCapturing;

	private string _contentName;

	public RuleId Id { get; private set; }

	public Rule(RuleId id, string name, string contentName)
	{
		Id = id;
		_name = name;
		_nameIsCapturing = RegexSource.HasCaptures(_name);
		_contentName = contentName;
		_contentNameIsCapturing = RegexSource.HasCaptures(_contentName);
	}

	public string GetName(string lineText, IOnigCaptureIndex[] captureIndices)
	{
		if (!_nameIsCapturing)
		{
			return _name;
		}
		return RegexSource.ReplaceCaptures(_name, lineText, captureIndices);
	}

	public string GetContentName(string lineText, IOnigCaptureIndex[] captureIndices)
	{
		if (!_contentNameIsCapturing)
		{
			return _contentName;
		}
		return RegexSource.ReplaceCaptures(_contentName, lineText, captureIndices);
	}

	public abstract void CollectPatternsRecursive(IRuleRegistry grammar, RegExpSourceList sourceList, bool isFirst);

	public abstract CompiledRule Compile(IRuleRegistry grammar, string endRegexSource, bool allowA, bool allowG);
}
