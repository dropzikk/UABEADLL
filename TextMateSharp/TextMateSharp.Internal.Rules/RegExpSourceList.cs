using System.Collections.Generic;
using TextMateSharp.Internal.Oniguruma;

namespace TextMateSharp.Internal.Rules;

public class RegExpSourceList
{
	private class RegExpSourceListAnchorCache
	{
		public CompiledRule A0_G0;

		public CompiledRule A0_G1;

		public CompiledRule A1_G0;

		public CompiledRule A1_G1;
	}

	private List<RegExpSource> _items;

	private bool _hasAnchors;

	private CompiledRule _cached;

	private RegExpSourceListAnchorCache _anchorCache;

	public RegExpSourceList()
	{
		_items = new List<RegExpSource>();
		_hasAnchors = false;
		_cached = null;
		_anchorCache = new RegExpSourceListAnchorCache();
	}

	public void Push(RegExpSource item)
	{
		_items.Add(item);
		_hasAnchors = (_hasAnchors ? _hasAnchors : item.HasAnchor());
	}

	public void UnShift(RegExpSource item)
	{
		_items.Insert(0, item);
		_hasAnchors = (_hasAnchors ? _hasAnchors : item.HasAnchor());
	}

	public int Length()
	{
		return _items.Count;
	}

	public void SetSource(int index, string newSource)
	{
		RegExpSource r = _items[index];
		if (!r.GetSource().Equals(newSource))
		{
			_cached = null;
			_anchorCache.A0_G0 = null;
			_anchorCache.A0_G1 = null;
			_anchorCache.A1_G0 = null;
			_anchorCache.A1_G1 = null;
			r.SetSource(newSource);
		}
	}

	public CompiledRule Compile(bool allowA, bool allowG)
	{
		if (!_hasAnchors)
		{
			if (_cached == null)
			{
				List<string> regexps = new List<string>();
				foreach (RegExpSource regExpSource in _items)
				{
					regexps.Add(regExpSource.GetSource());
				}
				_cached = new CompiledRule(CreateOnigScanner(regexps.ToArray()), GetRules());
			}
			return _cached;
		}
		if (_anchorCache.A0_G0 == null)
		{
			_anchorCache.A0_G0 = ((!allowA && !allowG) ? ResolveAnchors(allowA, allowG) : null);
		}
		if (_anchorCache.A0_G1 == null)
		{
			_anchorCache.A0_G1 = ((!allowA && allowG) ? ResolveAnchors(allowA, allowG) : null);
		}
		if (_anchorCache.A1_G0 == null)
		{
			_anchorCache.A1_G0 = ((allowA && !allowG) ? ResolveAnchors(allowA, allowG) : null);
		}
		if (_anchorCache.A1_G1 == null)
		{
			_anchorCache.A1_G1 = ((allowA && allowG) ? ResolveAnchors(allowA, allowG) : null);
		}
		if (allowA)
		{
			if (allowG)
			{
				return _anchorCache.A1_G1;
			}
			return _anchorCache.A1_G0;
		}
		if (allowG)
		{
			return _anchorCache.A0_G1;
		}
		return _anchorCache.A0_G0;
	}

	private CompiledRule ResolveAnchors(bool allowA, bool allowG)
	{
		List<string> regexps = new List<string>();
		foreach (RegExpSource regExpSource in _items)
		{
			regexps.Add(regExpSource.ResolveAnchors(allowA, allowG));
		}
		return new CompiledRule(CreateOnigScanner(regexps.ToArray()), GetRules());
	}

	private OnigScanner CreateOnigScanner(string[] regexps)
	{
		return new OnigScanner(regexps);
	}

	private IList<RuleId> GetRules()
	{
		List<RuleId> ruleIds = new List<RuleId>();
		foreach (RegExpSource item in _items)
		{
			ruleIds.Add(item.GetRuleId());
		}
		return ruleIds.ToArray();
	}
}
