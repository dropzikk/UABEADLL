using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TextMateSharp.Grammars;

namespace TextMateSharp.Model;

public class TMModel : ITMModel
{
	private class TokenizerThread
	{
		public volatile bool IsStopped;

		private string name;

		private TMModel model;

		private TMState lastState;

		public TokenizerThread(string name, TMModel model)
		{
			this.name = name;
			this.model = model;
			IsStopped = true;
		}

		public void Run()
		{
			IsStopped = false;
			ThreadPool.QueueUserWorkItem(ThreadWorker);
		}

		public void Stop()
		{
			IsStopped = true;
		}

		private void ThreadWorker(object state)
		{
			if (IsStopped)
			{
				return;
			}
			do
			{
				int toProcess = -1;
				if (model._grammar.IsCompiling)
				{
					model._resetEvent.Reset();
					model._resetEvent.WaitOne();
					continue;
				}
				lock (model._lock)
				{
					if (model._invalidLines.Count > 0)
					{
						toProcess = model._invalidLines.Dequeue();
					}
				}
				if (toProcess == -1)
				{
					model._resetEvent.Reset();
					model._resetEvent.WaitOne();
					continue;
				}
				ModelLine modelLine = model._lines.Get(toProcess);
				if (modelLine == null || !modelLine.IsInvalid)
				{
					continue;
				}
				try
				{
					RevalidateTokens(toProcess, null);
				}
				catch (Exception)
				{
					if (toProcess < model._lines.GetNumberOfLines())
					{
						model.InvalidateLine(toProcess);
					}
				}
			}
			while (!IsStopped && model._thread != null);
		}

		private void RevalidateTokens(int startLine, int? toLineIndexOrNull)
		{
			if (model._tokenizer == null)
			{
				return;
			}
			model.BuildEventWithCallback(delegate(ModelTokensChangedEventBuilder eventBuilder)
			{
				int num = toLineIndexOrNull.GetValueOrDefault();
				if (!toLineIndexOrNull.HasValue || num >= model._lines.GetNumberOfLines())
				{
					num = model._lines.GetNumberOfLines() - 1;
				}
				long num2 = 0L;
				long num3 = 0L;
				long num4 = 5L;
				long num5 = 0L;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				int num6 = startLine;
				while (num6 <= num && num6 < model.GetLines().GetNumberOfLines())
				{
					long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					if (elapsedMilliseconds > num4)
					{
						model.InvalidateLine(num6);
						break;
					}
					try
					{
						num3 = model._lines.GetLineLength(num6);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					if (num2 > 0)
					{
						num5 = (long)((double)elapsedMilliseconds / (double)num2) * num3;
						if (elapsedMilliseconds + num5 > num4)
						{
							model.InvalidateLine(num6);
							break;
						}
					}
					num6 = UpdateTokensInRange(eventBuilder, num6, num6) + 1;
					num2 += num3;
				}
			});
		}

		public int UpdateTokensInRange(ModelTokensChangedEventBuilder eventBuilder, int startIndex, int endLineIndex)
		{
			TimeSpan stopLineTokenizationAfter = TimeSpan.FromMilliseconds(3000.0);
			int nextInvalidLineIndex = startIndex;
			int lineIndex = startIndex;
			while (lineIndex <= endLineIndex && lineIndex < model._lines.GetNumberOfLines())
			{
				if (model._grammar != null && model._grammar.IsCompiling)
				{
					lineIndex++;
					continue;
				}
				int endStateIndex = lineIndex + 1;
				LineTokens r = null;
				string text = null;
				ModelLine modeLine = model._lines.Get(lineIndex);
				try
				{
					text = model._lines.GetLineText(lineIndex);
					if (text == null)
					{
						continue;
					}
					r = model._tokenizer.Tokenize(text, modeLine.State, 0, 10000, stopLineTokenizationAfter);
					goto IL_00a7;
				}
				catch (Exception)
				{
					lineIndex++;
				}
				continue;
				IL_00a7:
				if (r != null && r.Tokens != null && r.Tokens.Count != 0)
				{
					r.ActualStopOffset = Math.Max(r.ActualStopOffset, r.Tokens[r.Tokens.Count - 1].StartIndex + 1);
				}
				if (r != null && r.ActualStopOffset < text.Length)
				{
					r.Tokens.Add(new TMToken(r.ActualStopOffset, new List<string>()));
					r.EndState = modeLine.State;
				}
				if (r == null)
				{
					r = new LineTokens(new List<TMToken>
					{
						new TMToken(0, new List<string>())
					}, text.Length, modeLine.State);
				}
				modeLine.Tokens = r.Tokens;
				eventBuilder.registerChangedTokens(lineIndex + 1);
				modeLine.IsInvalid = false;
				if (endStateIndex < model._lines.GetNumberOfLines())
				{
					ModelLine endStateLine = model._lines.Get(endStateIndex);
					if (endStateLine.State != null && r.EndState.Equals(endStateLine.State))
					{
						for (nextInvalidLineIndex = lineIndex + 1; nextInvalidLineIndex < model._lines.GetNumberOfLines(); nextInvalidLineIndex++)
						{
							bool isLastLine = nextInvalidLineIndex + 1 >= model._lines.GetNumberOfLines();
							if (model._lines.Get(nextInvalidLineIndex).IsInvalid || (!isLastLine && model._lines.Get(nextInvalidLineIndex + 1).State == null) || (isLastLine && lastState == null))
							{
								break;
							}
						}
						lineIndex = nextInvalidLineIndex;
					}
					else
					{
						endStateLine.State = r.EndState;
						lineIndex++;
					}
				}
				else
				{
					lastState = r.EndState;
					lineIndex++;
				}
			}
			return nextInvalidLineIndex;
		}
	}

	private const int MAX_LEN_TO_TOKENIZE = 10000;

	private IGrammar _grammar;

	private List<IModelTokensChangedListener> listeners;

	private Tokenizer _tokenizer;

	private TokenizerThread _thread;

	private IModelLines _lines;

	private Queue<int> _invalidLines = new Queue<int>();

	private object _lock = new object();

	private ManualResetEvent _resetEvent = new ManualResetEvent(initialState: false);

	public bool IsStopped
	{
		get
		{
			if (_thread != null)
			{
				return _thread.IsStopped;
			}
			return true;
		}
	}

	public TMModel(IModelLines lines)
	{
		listeners = new List<IModelTokensChangedListener>();
		_lines = lines;
		((AbstractLineList)lines).SetModel(this);
	}

	public IGrammar GetGrammar()
	{
		return _grammar;
	}

	public void SetGrammar(IGrammar grammar)
	{
		if (!object.Equals(grammar, _grammar))
		{
			Stop();
			_grammar = grammar;
			_lines.ForEach(delegate(ModelLine line)
			{
				line.ResetTokenizationState();
			});
			if (grammar != null)
			{
				_tokenizer = new Tokenizer(grammar);
				_lines.Get(0).State = _tokenizer.GetInitialState();
				Start();
				InvalidateLine(0);
			}
		}
	}

	public void AddModelTokensChangedListener(IModelTokensChangedListener listener)
	{
		if (_grammar != null)
		{
			Start();
		}
		lock (listeners)
		{
			if (!listeners.Contains(listener))
			{
				listeners.Add(listener);
			}
		}
	}

	public void RemoveModelTokensChangedListener(IModelTokensChangedListener listener)
	{
		lock (listeners)
		{
			listeners.Remove(listener);
			if (listeners.Count == 0)
			{
				Stop();
			}
		}
	}

	public void Dispose()
	{
		listeners.Clear();
		Stop();
		GetLines().Dispose();
	}

	private void Stop()
	{
		if (_thread != null)
		{
			_thread.Stop();
			_resetEvent.Set();
			_thread = null;
		}
	}

	private void Start()
	{
		if (_thread == null || _thread.IsStopped)
		{
			_thread = new TokenizerThread("TMModelThread", this);
		}
		if (_thread.IsStopped)
		{
			_thread.Run();
		}
	}

	private void BuildEventWithCallback(Action<ModelTokensChangedEventBuilder> callback)
	{
		if (_thread != null && !_thread.IsStopped)
		{
			ModelTokensChangedEventBuilder eventBuilder = new ModelTokensChangedEventBuilder(this);
			callback(eventBuilder);
			ModelTokensChangedEvent e = eventBuilder.Build();
			if (e != null)
			{
				Emit(e);
			}
		}
	}

	private void Emit(ModelTokensChangedEvent e)
	{
		foreach (IModelTokensChangedListener listener in listeners)
		{
			try
			{
				listener.ModelTokensChanged(e);
			}
			catch (Exception)
			{
			}
		}
	}

	public void ForceTokenization(int lineIndex)
	{
		ForceTokenization(lineIndex, lineIndex);
	}

	public void ForceTokenization(int startLineIndex, int endLineIndex)
	{
		if (_grammar == null)
		{
			return;
		}
		TokenizerThread tokenizerThread = _thread;
		if (tokenizerThread != null && !tokenizerThread.IsStopped)
		{
			BuildEventWithCallback(delegate(ModelTokensChangedEventBuilder eventBuilder)
			{
				tokenizerThread.UpdateTokensInRange(eventBuilder, startLineIndex, endLineIndex);
			});
		}
	}

	public List<TMToken> GetLineTokens(int lineIndex)
	{
		if (lineIndex < 0 || lineIndex > _lines.GetNumberOfLines() - 1)
		{
			return null;
		}
		return _lines.Get(lineIndex).Tokens;
	}

	public bool IsLineInvalid(int lineIndex)
	{
		return _lines.Get(lineIndex).IsInvalid;
	}

	public void InvalidateLine(int lineIndex)
	{
		_lines.Get(lineIndex).IsInvalid = true;
		lock (_lock)
		{
			_invalidLines.Enqueue(lineIndex);
			_resetEvent.Set();
		}
	}

	public void InvalidateLineRange(int iniLineIndex, int endLineIndex)
	{
		lock (_lock)
		{
			for (int i = iniLineIndex; i <= endLineIndex; i++)
			{
				_lines.Get(i).IsInvalid = true;
				_invalidLines.Enqueue(i);
			}
			_resetEvent.Set();
		}
	}

	public IModelLines GetLines()
	{
		return _lines;
	}
}
