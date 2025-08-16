using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace Avalonia.Diagnostics.ViewModels;

internal class ControlDetailsViewModel : ViewModelBase, IDisposable, IClassesChangedListener
{
	private class PropertyComparer : IComparer<PropertyViewModel>
	{
		public static PropertyComparer Instance { get; } = new PropertyComparer();

		public int Compare(PropertyViewModel? x, PropertyViewModel? y)
		{
			int num = GroupIndex(x?.Group);
			int num2 = GroupIndex(y?.Group);
			if (num != num2)
			{
				return num - num2;
			}
			return string.CompareOrdinal(x?.Name, y?.Name);
		}

		private static int GroupIndex(string? group)
		{
			return group switch
			{
				"Properties" => 0, 
				"Attached Properties" => 1, 
				"CLR Properties" => 2, 
				_ => 3, 
			};
		}
	}

	private readonly AvaloniaObject _avaloniaObject;

	private IDictionary<object, PropertyViewModel[]>? _propertyIndex;

	private PropertyViewModel? _selectedProperty;

	private DataGridCollectionView? _propertiesView;

	private bool _snapshotStyles;

	private bool _showInactiveStyles;

	private string? _styleStatus;

	private object? _selectedEntity;

	private readonly Stack<(string Name, object Entry)> _selectedEntitiesStack = new Stack<(string, object)>();

	private string? _selectedEntityName;

	private string? _selectedEntityType;

	private bool _showImplementedInterfaces;

	public bool CanNavigateToParentProperty => _selectedEntitiesStack.Count >= 1;

	public TreePageViewModel TreePage { get; }

	public DataGridCollectionView? PropertiesView
	{
		get
		{
			return _propertiesView;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _propertiesView, value, "PropertiesView");
		}
	}

	public ObservableCollection<StyleViewModel> AppliedStyles { get; }

	public ObservableCollection<PseudoClassViewModel> PseudoClasses { get; }

	public object? SelectedEntity
	{
		get
		{
			return _selectedEntity;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedEntity, value, "SelectedEntity");
		}
	}

	public string? SelectedEntityName
	{
		get
		{
			return _selectedEntityName;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedEntityName, value, "SelectedEntityName");
		}
	}

	public string? SelectedEntityType
	{
		get
		{
			return _selectedEntityType;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedEntityType, value, "SelectedEntityType");
		}
	}

	public PropertyViewModel? SelectedProperty
	{
		get
		{
			return _selectedProperty;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedProperty, value, "SelectedProperty");
		}
	}

	public bool SnapshotStyles
	{
		get
		{
			return _snapshotStyles;
		}
		set
		{
			RaiseAndSetIfChanged(ref _snapshotStyles, value, "SnapshotStyles");
		}
	}

	public bool ShowInactiveStyles
	{
		get
		{
			return _showInactiveStyles;
		}
		set
		{
			RaiseAndSetIfChanged(ref _showInactiveStyles, value, "ShowInactiveStyles");
		}
	}

	public string? StyleStatus
	{
		get
		{
			return _styleStatus;
		}
		set
		{
			RaiseAndSetIfChanged(ref _styleStatus, value, "StyleStatus");
		}
	}

	public ControlLayoutViewModel? Layout { get; }

	public ControlDetailsViewModel(TreePageViewModel treePage, AvaloniaObject avaloniaObject)
	{
		_avaloniaObject = avaloniaObject;
		TreePage = treePage;
		Layout = ((avaloniaObject is Visual control) ? new ControlLayoutViewModel(control) : null);
		NavigateToProperty(_avaloniaObject, (_avaloniaObject as Control)?.Name ?? _avaloniaObject.ToString());
		AppliedStyles = new ObservableCollection<StyleViewModel>();
		PseudoClasses = new ObservableCollection<PseudoClassViewModel>();
		if (!(avaloniaObject is StyledElement styledElement))
		{
			return;
		}
		styledElement.Classes.AddListener(this);
		foreach (PseudoClassesAttribute customAttribute in styledElement.GetType().GetCustomAttributes<PseudoClassesAttribute>(inherit: true))
		{
			foreach (string pseudoClass in customAttribute.PseudoClasses)
			{
				PseudoClasses.Add(new PseudoClassViewModel(pseudoClass, styledElement));
			}
		}
		StyleDiagnostics styleDiagnostics = styledElement.GetStyleDiagnostics();
		IClipboard clipboard = TopLevel.GetTopLevel(_avaloniaObject as Visual)?.Clipboard;
		foreach (AppliedStyle item3 in styleDiagnostics.AppliedStyles.OrderBy((AppliedStyle s) => s.HasActivator))
		{
			StyleBase style = item3.Style;
			List<SetterViewModel> list = new List<SetterViewModel>();
			if (style == null)
			{
				continue;
			}
			StyleBase styleBase = style;
			string text = ((styleBase is Style style2) ? style2.Selector?.ToString() : ((!(styleBase is ControlTheme controlTheme)) ? null : controlTheme.TargetType?.Name.ToString()));
			string text2 = text;
			foreach (SetterBase setter2 in styleBase.Setters)
			{
				if (setter2 is Setter setter && setter.Property != null)
				{
					object value = setter.Value;
					(object, bool)? resourceInfo = GetResourceInfo(value);
					SetterViewModel item;
					if (!resourceInfo.HasValue)
					{
						item = ((!IsBinding(value)) ? new SetterViewModel(setter.Property, value, clipboard) : new BindingSetterViewModel(setter.Property, value, clipboard));
					}
					else
					{
						object item2 = resourceInfo.Value.Item1;
						object resourceValue = styledElement.FindResource(item2);
						item = new ResourceSetterViewModel(setter.Property, item2, resourceValue, resourceInfo.Value.Item2, clipboard);
					}
					list.Add(item);
				}
			}
			AppliedStyles.Add(new StyleViewModel(item3, text2 ?? "No selector", list));
		}
		UpdateStyles();
	}

	private static (object resourceKey, bool isDynamic)? GetResourceInfo(object? value)
	{
		if (value is StaticResourceExtension { ResourceKey: not null } staticResourceExtension)
		{
			return (staticResourceExtension.ResourceKey, false);
		}
		if (value is DynamicResourceExtension { ResourceKey: not null } dynamicResourceExtension)
		{
			return (dynamicResourceExtension.ResourceKey, true);
		}
		return null;
	}

	private static bool IsBinding(object? value)
	{
		if (value is Binding || value is CompiledBindingExtension || value is TemplateBinding)
		{
			return true;
		}
		return false;
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (e.PropertyName == "SnapshotStyles" && !SnapshotStyles)
		{
			UpdateStyles();
		}
	}

	public void UpdateStyleFilters()
	{
		foreach (StyleViewModel appliedStyle in AppliedStyles)
		{
			bool flag = false;
			foreach (SetterViewModel setter in appliedStyle.Setters)
			{
				setter.IsVisible = TreePage.SettersFilter.Filter(setter.Name);
				flag |= setter.IsVisible;
			}
			appliedStyle.IsVisible = flag;
		}
	}

	public void Dispose()
	{
		INotifyPropertyChanged avaloniaObject = _avaloniaObject;
		if (avaloniaObject != null)
		{
			avaloniaObject.PropertyChanged -= ControlPropertyChanged;
		}
		AvaloniaObject avaloniaObject2 = _avaloniaObject;
		if (avaloniaObject2 != null)
		{
			avaloniaObject2.PropertyChanged -= ControlPropertyChanged;
		}
		if (_avaloniaObject is StyledElement styledElement)
		{
			styledElement.Classes.RemoveListener(this);
		}
	}

	private static IEnumerable<PropertyViewModel> GetAvaloniaProperties(object o)
	{
		AvaloniaObject ao = o as AvaloniaObject;
		if (ao != null)
		{
			return from x in AvaloniaPropertyRegistry.Instance.GetRegistered(ao).Union(AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(ao.GetType()))
				select new AvaloniaPropertyViewModel(ao, x);
		}
		return Enumerable.Empty<AvaloniaPropertyViewModel>();
	}

	private static IEnumerable<PropertyViewModel> GetClrProperties(object o, bool showImplementedInterfaces)
	{
		foreach (PropertyViewModel clrProperty in GetClrProperties(o, o.GetType()))
		{
			yield return clrProperty;
		}
		if (!showImplementedInterfaces)
		{
			yield break;
		}
		Type[] interfaces = o.GetType().GetInterfaces();
		foreach (Type t in interfaces)
		{
			foreach (PropertyViewModel clrProperty2 in GetClrProperties(o, t))
			{
				yield return clrProperty2;
			}
		}
	}

	private static IEnumerable<PropertyViewModel> GetClrProperties(object o, Type t)
	{
		return from x in t.GetProperties()
			where x.GetIndexParameters().Length == 0
			select new ClrPropertyViewModel(o, x);
	}

	private void ControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (_propertyIndex != null && _propertyIndex.TryGetValue(e.Property, out PropertyViewModel[] value))
		{
			PropertyViewModel[] array = value;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Update();
			}
		}
		Layout?.ControlPropertyChanged(sender, e);
	}

	private void ControlPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName != null && _propertyIndex != null && _propertyIndex.TryGetValue(e.PropertyName, out PropertyViewModel[] value))
		{
			PropertyViewModel[] array = value;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Update();
			}
		}
		if (!SnapshotStyles)
		{
			UpdateStyles();
		}
	}

	void IClassesChangedListener.Changed()
	{
		if (!SnapshotStyles)
		{
			UpdateStyles();
		}
	}

	private void UpdateStyles()
	{
		int num = 0;
		foreach (StyleViewModel appliedStyle in AppliedStyles)
		{
			appliedStyle.Update();
			if (appliedStyle.IsActive)
			{
				num++;
			}
		}
		Dictionary<AvaloniaProperty, List<SetterViewModel>> dictionary = new Dictionary<AvaloniaProperty, List<SetterViewModel>>();
		foreach (StyleViewModel appliedStyle2 in AppliedStyles)
		{
			if (!appliedStyle2.IsActive)
			{
				continue;
			}
			foreach (SetterViewModel setter in appliedStyle2.Setters)
			{
				if (dictionary.TryGetValue(setter.Property, out var value))
				{
					foreach (SetterViewModel item in value)
					{
						item.IsActive = false;
					}
					setter.IsActive = true;
					value.Add(setter);
				}
				else
				{
					setter.IsActive = true;
					value = new List<SetterViewModel> { setter };
					dictionary.Add(setter.Property, value);
				}
			}
		}
		foreach (PseudoClassViewModel pseudoClass in PseudoClasses)
		{
			pseudoClass.Update();
		}
		StyleStatus = $"Styles ({num}/{AppliedStyles.Count} active)";
	}

	private bool FilterProperty(object arg)
	{
		if (arg is PropertyViewModel propertyViewModel)
		{
			return TreePage.PropertiesFilter.Filter(propertyViewModel.Name);
		}
		return true;
	}

	private static IEnumerable<PropertyInfo> GetAllPublicProperties(Type type)
	{
		return type.GetProperties().Concat(type.GetInterfaces().SelectMany((Type i) => i.GetProperties()));
	}

	public void NavigateToSelectedProperty()
	{
		PropertyViewModel selectedProperty = SelectedProperty;
		object selectedEntity = SelectedEntity;
		string selectedEntityName = SelectedEntityName;
		if (selectedEntity == null || selectedProperty == null || selectedProperty.PropertyType == typeof(string) || selectedProperty.PropertyType.IsValueType)
		{
			return;
		}
		object obj = null;
		if (!(selectedProperty is AvaloniaPropertyViewModel avaloniaPropertyViewModel))
		{
			ClrPropertyViewModel clrPropertyViewModel = selectedProperty as ClrPropertyViewModel;
			if (clrPropertyViewModel != null)
			{
				obj = GetAllPublicProperties(selectedEntity.GetType()).FirstOrDefault((PropertyInfo pi) => clrPropertyViewModel.Property == pi)?.GetValue(selectedEntity);
			}
		}
		else
		{
			obj = (_selectedEntity as Control)?.GetValue(avaloniaPropertyViewModel.Property);
		}
		if (obj != null)
		{
			_selectedEntitiesStack.Push((selectedEntityName, selectedEntity));
			string text = selectedProperty.Name;
			int num = text.LastIndexOf('.');
			if (num != -1)
			{
				text = text.Substring(num + 1);
			}
			NavigateToProperty(obj, selectedEntityName + "." + text);
			RaisePropertyChanged("CanNavigateToParentProperty");
		}
	}

	public void NavigateToParentProperty()
	{
		if (_selectedEntitiesStack.Count > 0)
		{
			(string, object) tuple = _selectedEntitiesStack.Pop();
			NavigateToProperty(tuple.Item2, tuple.Item1);
			RaisePropertyChanged("CanNavigateToParentProperty");
		}
	}

	protected void NavigateToProperty(object o, string? entityName)
	{
		object selectedEntity = SelectedEntity;
		if (!(selectedEntity is AvaloniaObject avaloniaObject))
		{
			if (selectedEntity is INotifyPropertyChanged notifyPropertyChanged)
			{
				notifyPropertyChanged.PropertyChanged -= ControlPropertyChanged;
			}
		}
		else
		{
			avaloniaObject.PropertyChanged -= ControlPropertyChanged;
		}
		SelectedEntity = o;
		SelectedEntityName = entityName;
		SelectedEntityType = o.ToString();
		PropertyViewModel[] source = GetAvaloniaProperties(o).Concat(GetClrProperties(o, _showImplementedInterfaces)).OrderBy((PropertyViewModel x) => x, PropertyComparer.Instance).ThenBy((PropertyViewModel x) => x.Name)
			.ToArray();
		_propertyIndex = (from x in source
			group x by x.Key).ToDictionary((IGrouping<object, PropertyViewModel> x) => x.Key, (IGrouping<object, PropertyViewModel> x) => x.ToArray());
		DataGridCollectionView dataGridCollectionView = new DataGridCollectionView(source);
		dataGridCollectionView.GroupDescriptions.Add(new DataGridPathGroupDescription("Group"));
		dataGridCollectionView.Filter = FilterProperty;
		PropertiesView = dataGridCollectionView;
		if (!(o is AvaloniaObject avaloniaObject2))
		{
			if (o is INotifyPropertyChanged notifyPropertyChanged2)
			{
				notifyPropertyChanged2.PropertyChanged += ControlPropertyChanged;
			}
		}
		else
		{
			avaloniaObject2.PropertyChanged += ControlPropertyChanged;
		}
	}

	internal void SelectProperty(AvaloniaProperty property)
	{
		SelectedProperty = null;
		if (SelectedEntity != _avaloniaObject)
		{
			NavigateToProperty(_avaloniaObject, (_avaloniaObject as Control)?.Name ?? _avaloniaObject.ToString());
		}
		if (PropertiesView == null)
		{
			return;
		}
		foreach (object item in PropertiesView)
		{
			if (item is AvaloniaPropertyViewModel avaloniaPropertyViewModel && avaloniaPropertyViewModel.Property == property)
			{
				SelectedProperty = avaloniaPropertyViewModel;
				break;
			}
		}
	}

	internal void UpdatePropertiesView(bool showImplementedInterfaces)
	{
		_showImplementedInterfaces = showImplementedInterfaces;
		SelectedProperty = null;
		NavigateToProperty(_avaloniaObject, (_avaloniaObject as Control)?.Name ?? _avaloniaObject.ToString());
	}
}
