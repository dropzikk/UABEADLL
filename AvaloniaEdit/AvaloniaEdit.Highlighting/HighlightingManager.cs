using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using AvaloniaEdit.Highlighting.Xshd;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class HighlightingManager : IHighlightingDefinitionReferenceResolver
{
	private sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
	{
		private readonly object _lockObj = new object();

		private readonly string _name;

		private Func<IHighlightingDefinition> _lazyLoadingFunction;

		private IHighlightingDefinition _definition;

		private Exception _storedException;

		public string Name => _name ?? GetDefinition().Name;

		public HighlightingRuleSet MainRuleSet => GetDefinition().MainRuleSet;

		public IEnumerable<HighlightingColor> NamedHighlightingColors => GetDefinition().NamedHighlightingColors;

		public IDictionary<string, string> Properties => GetDefinition().Properties;

		public DelayLoadedHighlightingDefinition(string name, Func<IHighlightingDefinition> lazyLoadingFunction)
		{
			_name = name;
			_lazyLoadingFunction = lazyLoadingFunction;
		}

		private IHighlightingDefinition GetDefinition()
		{
			Func<IHighlightingDefinition> lazyLoadingFunction;
			lock (_lockObj)
			{
				if (_definition != null)
				{
					return _definition;
				}
				lazyLoadingFunction = _lazyLoadingFunction;
			}
			Exception storedException = null;
			IHighlightingDefinition highlightingDefinition = null;
			try
			{
				using (BusyManager.BusyLock busyLock = BusyManager.Enter(this))
				{
					if (!busyLock.Success)
					{
						throw new InvalidOperationException("Tried to create delay-loaded highlighting definition recursively. Make sure the are no cyclic references between the highlighting definitions.");
					}
					highlightingDefinition = lazyLoadingFunction();
				}
				if (highlightingDefinition == null)
				{
					throw new InvalidOperationException("Function for delay-loading highlighting definition returned null");
				}
			}
			catch (Exception ex)
			{
				storedException = ex;
			}
			lock (_lockObj)
			{
				_lazyLoadingFunction = null;
				if (_definition == null && _storedException == null)
				{
					_definition = highlightingDefinition;
					_storedException = storedException;
				}
				if (_storedException != null)
				{
					throw new HighlightingDefinitionInvalidException("Error delay-loading highlighting definition", _storedException);
				}
				return _definition;
			}
		}

		public HighlightingRuleSet GetNamedRuleSet(string name)
		{
			return GetDefinition().GetNamedRuleSet(name);
		}

		public HighlightingColor GetNamedColor(string name)
		{
			return GetDefinition().GetNamedColor(name);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	internal sealed class DefaultHighlightingManager : HighlightingManager
	{
		public new static DefaultHighlightingManager Instance { get; } = new DefaultHighlightingManager();

		public DefaultHighlightingManager()
		{
			Resources.RegisterBuiltInHighlightings(this);
		}

		internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
		{
			try
			{
				RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
			}
			catch (HighlightingDefinitionInvalidException innerException)
			{
				throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", innerException);
			}
		}

		private Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
		{
			return Func;
			IHighlightingDefinition Func()
			{
				XshdSyntaxDefinition syntaxDefinition;
				using (Stream input = Resources.OpenStream(resourceName))
				{
					using XmlReader reader = XmlReader.Create(input);
					syntaxDefinition = HighlightingLoader.LoadXshd(reader, skipValidation: true);
				}
				return HighlightingLoader.Load(syntaxDefinition, this);
			}
		}
	}

	private readonly object _lockObj = new object();

	private readonly Dictionary<string, IHighlightingDefinition> _highlightingsByName = new Dictionary<string, IHighlightingDefinition>();

	private readonly Dictionary<string, IHighlightingDefinition> _highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);

	private readonly List<IHighlightingDefinition> _allHighlightings = new List<IHighlightingDefinition>();

	public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
	{
		get
		{
			lock (_lockObj)
			{
				return new ReadOnlyCollection<IHighlightingDefinition>(_allHighlightings);
			}
		}
	}

	public static HighlightingManager Instance => DefaultHighlightingManager.Instance;

	public IHighlightingDefinition GetDefinition(string name)
	{
		lock (_lockObj)
		{
			IHighlightingDefinition value;
			return _highlightingsByName.TryGetValue(name, out value) ? value : null;
		}
	}

	public IHighlightingDefinition GetDefinitionByExtension(string extension)
	{
		lock (_lockObj)
		{
			IHighlightingDefinition value;
			return _highlightingsByExtension.TryGetValue(extension, out value) ? value : null;
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
	{
		if (highlighting == null)
		{
			throw new ArgumentNullException("highlighting");
		}
		lock (_lockObj)
		{
			_allHighlightings.Add(highlighting);
			if (name != null)
			{
				_highlightingsByName[name] = highlighting;
			}
			if (extensions != null)
			{
				foreach (string key in extensions)
				{
					_highlightingsByExtension[key] = highlighting;
				}
			}
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
	{
		if (lazyLoadedHighlighting == null)
		{
			throw new ArgumentNullException("lazyLoadedHighlighting");
		}
		RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
	}
}
