using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Avalonia.Diagnostics.ViewModels;

internal class FilterViewModel : ViewModelBase, INotifyDataErrorInfo
{
	private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

	private string _filterString = string.Empty;

	private bool _useRegexFilter;

	private bool _useCaseSensitiveFilter;

	private bool _useWholeWordFilter;

	private Regex? _filterRegex;

	public bool HasErrors => _errors.Count > 0;

	public string FilterString
	{
		get
		{
			return _filterString;
		}
		set
		{
			if (RaiseAndSetIfChanged(ref _filterString, value, "FilterString"))
			{
				UpdateFilterRegex();
				this.RefreshFilter?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public bool UseRegexFilter
	{
		get
		{
			return _useRegexFilter;
		}
		set
		{
			if (RaiseAndSetIfChanged(ref _useRegexFilter, value, "UseRegexFilter"))
			{
				UpdateFilterRegex();
				this.RefreshFilter?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public bool UseCaseSensitiveFilter
	{
		get
		{
			return _useCaseSensitiveFilter;
		}
		set
		{
			if (RaiseAndSetIfChanged(ref _useCaseSensitiveFilter, value, "UseCaseSensitiveFilter"))
			{
				UpdateFilterRegex();
				this.RefreshFilter?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public bool UseWholeWordFilter
	{
		get
		{
			return _useWholeWordFilter;
		}
		set
		{
			if (RaiseAndSetIfChanged(ref _useWholeWordFilter, value, "UseWholeWordFilter"))
			{
				UpdateFilterRegex();
				this.RefreshFilter?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public event EventHandler? RefreshFilter;

	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	public bool Filter(string input)
	{
		return _filterRegex?.IsMatch(input) ?? true;
	}

	private void UpdateFilterRegex()
	{
		try
		{
			RegexOptions regexOptions = RegexOptions.Compiled;
			string text = (UseRegexFilter ? FilterString.Trim() : Regex.Escape(FilterString.Trim()));
			if (!UseCaseSensitiveFilter)
			{
				regexOptions |= RegexOptions.IgnoreCase;
			}
			if (UseWholeWordFilter)
			{
				text = "\\b(?:" + text + ")\\b";
			}
			_filterRegex = new Regex(text, regexOptions);
			ClearError();
		}
		catch (Exception ex)
		{
			_errors["FilterString"] = ex.Message;
			this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("FilterString"));
		}
		void ClearError()
		{
			if (_errors.Remove("FilterString"))
			{
				this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("FilterString"));
			}
		}
	}

	public IEnumerable GetErrors(string? propertyName)
	{
		if (propertyName != null && _errors.TryGetValue(propertyName, out string value))
		{
			yield return value;
		}
	}
}
