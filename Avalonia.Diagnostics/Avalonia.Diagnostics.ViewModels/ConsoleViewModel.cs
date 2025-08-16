using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Diagnostics.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;

namespace Avalonia.Diagnostics.ViewModels;

internal class ConsoleViewModel : ViewModelBase
{
	private readonly ConsoleContext _context;

	private readonly Action<ConsoleContext> _updateContext;

	private int _historyIndex = -1;

	private string _input;

	private bool _isVisible;

	private ScriptState<object>? _state;

	public string Input
	{
		get
		{
			return _input;
		}
		set
		{
			RaiseAndSetIfChanged(ref _input, value, "Input");
		}
	}

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isVisible, value, "IsVisible");
		}
	}

	public AvaloniaList<ConsoleHistoryItem> History { get; } = new AvaloniaList<ConsoleHistoryItem>();

	public ConsoleViewModel(Action<ConsoleContext> updateContext)
	{
		_context = new ConsoleContext(this);
		_input = string.Empty;
		_updateContext = updateContext;
	}

	public async Task Execute()
	{
		if (string.IsNullOrWhiteSpace(Input))
		{
			return;
		}
		try
		{
			ScriptOptions options = ScriptOptions.Default.AddReferences(Assembly.GetAssembly(typeof(CSharpArgumentInfo)));
			_updateContext(_context);
			if (_state == null)
			{
				_state = await CSharpScript.RunAsync(Input, options, _context);
			}
			else
			{
				_state = await _state.ContinueWithAsync(Input);
			}
			if (_state.ReturnValue != ConsoleContext.NoOutput)
			{
				History.Add(new ConsoleHistoryItem(Input, _state.ReturnValue ?? "(null)"));
			}
		}
		catch (Exception output)
		{
			History.Add(new ConsoleHistoryItem(Input, output));
		}
		Input = string.Empty;
		_historyIndex = -1;
	}

	public void HistoryUp()
	{
		if (History.Count > 0)
		{
			if (_historyIndex == -1)
			{
				_historyIndex = History.Count - 1;
			}
			else if (_historyIndex > 0)
			{
				_historyIndex--;
			}
			Input = History[_historyIndex].Input;
		}
	}

	public void HistoryDown()
	{
		if (History.Count > 0 && _historyIndex >= 0)
		{
			if (_historyIndex == History.Count - 1)
			{
				_historyIndex = -1;
				Input = string.Empty;
			}
			else
			{
				Input = History[++_historyIndex].Input;
			}
		}
	}

	public void ToggleVisibility()
	{
		IsVisible = !IsVisible;
	}
}
