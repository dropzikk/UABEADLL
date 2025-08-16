using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Styling;
using Avalonia.Utilities;

namespace Avalonia.PropertyStore;

internal class ValueStore
{
	private readonly struct OldNewValue
	{
		public readonly EffectiveValue? OldValue;

		public readonly EffectiveValue? NewValue;

		public OldNewValue(EffectiveValue? oldValue)
		{
			OldValue = oldValue;
			NewValue = null;
		}

		public OldNewValue(EffectiveValue? oldValue, EffectiveValue? newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public OldNewValue WithNewValue(EffectiveValue newValue)
		{
			return new OldNewValue(OldValue, newValue);
		}
	}

	private readonly List<ValueFrame> _frames = new List<ValueFrame>();

	private Dictionary<int, IDisposable>? _localValueBindings;

	private AvaloniaPropertyDictionary<EffectiveValue> _effectiveValues;

	private int _inheritedValueCount;

	private int _isEvaluating;

	private int _frameGeneration;

	private int _styling;

	public AvaloniaObject Owner { get; }

	public ValueStore? InheritanceAncestor { get; private set; }

	public bool IsEvaluating => _isEvaluating > 0;

	public IReadOnlyList<ValueFrame> Frames => _frames;

	public ValueStore(AvaloniaObject owner)
	{
		Owner = owner;
	}

	public void BeginStyling()
	{
		_styling++;
	}

	public void EndStyling()
	{
		if (--_styling == 0)
		{
			ReevaluateEffectiveValues();
		}
	}

	public void AddFrame(ValueFrame style)
	{
		InsertFrame(style);
		ReevaluateEffectiveValues();
	}

	public IDisposable AddBinding<T>(StyledProperty<T> property, IObservable<BindingValue<T>> source, BindingPriority priority)
	{
		if (priority == BindingPriority.LocalValue)
		{
			LocalValueBindingObserver<T> localValueBindingObserver = new LocalValueBindingObserver<T>(this, property);
			DisposeExistingLocalValueBinding(property);
			if (_localValueBindings == null)
			{
				_localValueBindings = new Dictionary<int, IDisposable>();
			}
			_localValueBindings[property.Id] = localValueBindingObserver;
			localValueBindingObserver.Start(source);
			return localValueBindingObserver;
		}
		EffectiveValue effectiveValue = GetEffectiveValue(property);
		int frameIndex;
		TypedBindingEntry<T> typedBindingEntry = GetOrCreateImmediateValueFrame(property, priority, out frameIndex).AddBinding(property, source);
		if (effectiveValue == null || priority <= effectiveValue.Priority)
		{
			typedBindingEntry.Start();
		}
		return typedBindingEntry;
	}

	public IDisposable AddBinding<T>(StyledProperty<T> property, IObservable<T> source, BindingPriority priority)
	{
		if (priority == BindingPriority.LocalValue)
		{
			LocalValueBindingObserver<T> localValueBindingObserver = new LocalValueBindingObserver<T>(this, property);
			DisposeExistingLocalValueBinding(property);
			if (_localValueBindings == null)
			{
				_localValueBindings = new Dictionary<int, IDisposable>();
			}
			_localValueBindings[property.Id] = localValueBindingObserver;
			localValueBindingObserver.Start(source);
			return localValueBindingObserver;
		}
		EffectiveValue effectiveValue = GetEffectiveValue(property);
		int frameIndex;
		TypedBindingEntry<T> typedBindingEntry = GetOrCreateImmediateValueFrame(property, priority, out frameIndex).AddBinding(property, source);
		if (effectiveValue == null || priority <= effectiveValue.Priority)
		{
			typedBindingEntry.Start();
		}
		return typedBindingEntry;
	}

	public IDisposable AddBinding<T>(StyledProperty<T> property, IObservable<object?> source, BindingPriority priority)
	{
		if (priority == BindingPriority.LocalValue)
		{
			LocalValueBindingObserver<T> localValueBindingObserver = new LocalValueBindingObserver<T>(this, property);
			DisposeExistingLocalValueBinding(property);
			if (_localValueBindings == null)
			{
				_localValueBindings = new Dictionary<int, IDisposable>();
			}
			_localValueBindings[property.Id] = localValueBindingObserver;
			localValueBindingObserver.Start(source);
			return localValueBindingObserver;
		}
		EffectiveValue effectiveValue = GetEffectiveValue(property);
		int frameIndex;
		SourceUntypedBindingEntry<T> sourceUntypedBindingEntry = GetOrCreateImmediateValueFrame(property, priority, out frameIndex).AddBinding(property, source);
		if (effectiveValue == null || priority <= effectiveValue.Priority)
		{
			sourceUntypedBindingEntry.Start();
		}
		return sourceUntypedBindingEntry;
	}

	public IDisposable AddBinding<T>(DirectPropertyBase<T> property, IObservable<BindingValue<T>> source)
	{
		DirectBindingObserver<T> directBindingObserver = new DirectBindingObserver<T>(this, property);
		DisposeExistingLocalValueBinding(property);
		if (_localValueBindings == null)
		{
			_localValueBindings = new Dictionary<int, IDisposable>();
		}
		_localValueBindings[property.Id] = directBindingObserver;
		directBindingObserver.Start(source);
		return directBindingObserver;
	}

	public IDisposable AddBinding<T>(DirectPropertyBase<T> property, IObservable<T> source)
	{
		DirectBindingObserver<T> directBindingObserver = new DirectBindingObserver<T>(this, property);
		DisposeExistingLocalValueBinding(property);
		if (_localValueBindings == null)
		{
			_localValueBindings = new Dictionary<int, IDisposable>();
		}
		_localValueBindings[property.Id] = directBindingObserver;
		directBindingObserver.Start(source);
		return directBindingObserver;
	}

	public IDisposable AddBinding<T>(DirectPropertyBase<T> property, IObservable<object?> source)
	{
		DirectUntypedBindingObserver<T> directUntypedBindingObserver = new DirectUntypedBindingObserver<T>(this, property);
		DisposeExistingLocalValueBinding(property);
		if (_localValueBindings == null)
		{
			_localValueBindings = new Dictionary<int, IDisposable>();
		}
		_localValueBindings[property.Id] = directUntypedBindingObserver;
		directUntypedBindingObserver.Start(source);
		return directUntypedBindingObserver;
	}

	public void ClearValue(AvaloniaProperty property)
	{
		if (TryGetEffectiveValue(property, out EffectiveValue value) && (value.Priority == BindingPriority.LocalValue || value.IsOverridenCurrentValue))
		{
			value.IsOverridenCurrentValue = false;
			ReevaluateEffectiveValue(property, value, null, ignoreLocalValue: true);
		}
	}

	public IDisposable? SetValue<T>(StyledProperty<T> property, T value, BindingPriority priority)
	{
		Func<T, bool>? validateValue = property.ValidateValue;
		if (validateValue != null && !validateValue(value))
		{
			throw new ArgumentException($"{value} is not a valid value for '{property.Name}.");
		}
		if (priority != 0)
		{
			int frameIndex;
			ImmediateValueEntry<T> immediateValueEntry = GetOrCreateImmediateValueFrame(property, priority, out frameIndex).AddValue(property, value);
			if (TryGetEffectiveValue(property, out EffectiveValue value2))
			{
				((EffectiveValue<T>)value2).SetAndRaise(this, immediateValueEntry, priority);
			}
			else
			{
				EffectiveValue<T> effectiveValue = CreateEffectiveValue(property);
				AddEffectiveValue(property, effectiveValue);
				effectiveValue.SetAndRaise(this, immediateValueEntry, priority);
			}
			return immediateValueEntry;
		}
		SetLocalValue(property, value);
		return null;
	}

	public void SetCurrentValue<T>(StyledProperty<T> property, T value)
	{
		if (TryGetEffectiveValue(property, out EffectiveValue value2))
		{
			((EffectiveValue<T>)value2).SetCurrentValueAndRaise(this, property, value);
			return;
		}
		EffectiveValue<T> effectiveValue = CreateEffectiveValue(property);
		AddEffectiveValue(property, effectiveValue);
		effectiveValue.SetCurrentValueAndRaise(this, property, value);
	}

	public void SetLocalValue<T>(StyledProperty<T> property, T value)
	{
		if (TryGetEffectiveValue(property, out EffectiveValue value2))
		{
			((EffectiveValue<T>)value2).SetLocalValueAndRaise(this, property, value);
			return;
		}
		EffectiveValue<T> effectiveValue = CreateEffectiveValue(property);
		AddEffectiveValue(property, effectiveValue);
		effectiveValue.SetLocalValueAndRaise(this, property, value);
	}

	public object? GetValue(AvaloniaProperty property)
	{
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			return value.Value;
		}
		if (property.Inherits && TryGetInheritedValue(property, out value))
		{
			return value.Value;
		}
		return GetDefaultValue(property);
	}

	public T GetValue<T>(StyledProperty<T> property)
	{
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			return ((EffectiveValue<T>)value).Value;
		}
		if (property.Inherits && TryGetInheritedValue(property, out value))
		{
			return ((EffectiveValue<T>)value).Value;
		}
		return property.GetDefaultValue(Owner.GetType());
	}

	public bool IsAnimating(AvaloniaProperty property)
	{
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			return value.Priority <= BindingPriority.Animation;
		}
		return false;
	}

	public bool IsSet(AvaloniaProperty property)
	{
		EffectiveValue value;
		return _effectiveValues.TryGetValue(property, out value);
	}

	public void CoerceValue(AvaloniaProperty property)
	{
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			value.CoerceValue(this, property);
		}
		else
		{
			property.RouteCoerceDefaultValue(Owner);
		}
	}

	public void CoerceDefaultValue<T>(StyledProperty<T> property)
	{
		StyledPropertyMetadata<T> metadata = property.GetMetadata(Owner.GetType());
		if (metadata.CoerceValue != null)
		{
			T val = metadata.CoerceValue(Owner, metadata.DefaultValue);
			if (!EqualityComparer<T>.Default.Equals(metadata.DefaultValue, val))
			{
				EffectiveValue<T> effectiveValue = CreateEffectiveValue(property);
				AddEffectiveValue(property, effectiveValue);
				effectiveValue.SetCoercedDefaultValueAndRaise(this, property, val);
			}
		}
	}

	public Optional<T> GetBaseValue<T>(StyledProperty<T> property)
	{
		if (TryGetEffectiveValue(property, out EffectiveValue value) && ((EffectiveValue<T>)value).TryGetBaseValue(out var value2))
		{
			return value2;
		}
		return default(Optional<T>);
	}

	public bool TryGetInheritedValue(AvaloniaProperty property, [NotNullWhen(true)] out EffectiveValue? result)
	{
		for (ValueStore inheritanceAncestor = InheritanceAncestor; inheritanceAncestor != null; inheritanceAncestor = inheritanceAncestor.InheritanceAncestor)
		{
			if (inheritanceAncestor.TryGetEffectiveValue(property, out result))
			{
				return true;
			}
		}
		result = null;
		return false;
	}

	public EffectiveValue<T> CreateEffectiveValue<T>(StyledProperty<T> property)
	{
		EffectiveValue<T> inherited = null;
		if (property.Inherits && TryGetInheritedValue(property, out EffectiveValue result))
		{
			inherited = (EffectiveValue<T>)result;
		}
		return new EffectiveValue<T>(Owner, property, inherited);
	}

	public void SetInheritanceParent(AvaloniaObject? newParent)
	{
		AvaloniaPropertyDictionary<OldNewValue> dictionary = AvaloniaPropertyDictionaryPool<OldNewValue>.Get();
		ValueStore inheritanceAncestor = InheritanceAncestor;
		ValueStore valueStore = newParent?.GetValueStore();
		if (valueStore != null && valueStore._inheritedValueCount == 0)
		{
			valueStore = valueStore.InheritanceAncestor;
		}
		if (inheritanceAncestor == valueStore)
		{
			return;
		}
		for (ValueStore valueStore2 = inheritanceAncestor; valueStore2 != null; valueStore2 = valueStore2.InheritanceAncestor)
		{
			int count = valueStore2._effectiveValues.Count;
			for (int i = 0; i < count; i++)
			{
				valueStore2._effectiveValues.GetKeyValue(i, out AvaloniaProperty key, out EffectiveValue value);
				if (key.Inherits)
				{
					dictionary.TryAdd(key, new OldNewValue(value));
				}
			}
		}
		for (ValueStore valueStore2 = valueStore; valueStore2 != null; valueStore2 = valueStore2.InheritanceAncestor)
		{
			int count2 = valueStore2._effectiveValues.Count;
			for (int j = 0; j < count2; j++)
			{
				valueStore2._effectiveValues.GetKeyValue(j, out AvaloniaProperty key2, out EffectiveValue value2);
				if (!key2.Inherits)
				{
					continue;
				}
				if (dictionary.TryGetValue(key2, out var value3))
				{
					if (value3.NewValue == null)
					{
						dictionary[key2] = value3.WithNewValue(value2);
					}
				}
				else
				{
					dictionary.Add(key2, new OldNewValue(null, value2));
				}
			}
		}
		OnInheritanceAncestorChanged(valueStore);
		int count3 = dictionary.Count;
		for (int k = 0; k < count3; k++)
		{
			dictionary.GetKeyValue(k, out AvaloniaProperty key3, out OldNewValue value4);
			EffectiveValue oldValue = value4.OldValue;
			EffectiveValue newValue = value4.NewValue;
			if (oldValue != newValue)
			{
				InheritedValueChanged(key3, oldValue, newValue);
			}
		}
		AvaloniaPropertyDictionaryPool<OldNewValue>.Release(dictionary);
	}

	public void OnBindingValueChanged(IValueEntry entry, BindingPriority priority)
	{
		AvaloniaProperty property = entry.Property;
		if (TryGetEffectiveValue(property, out EffectiveValue value))
		{
			if (priority <= value.BasePriority)
			{
				ReevaluateEffectiveValue(property, value, entry);
			}
		}
		else
		{
			AddEffectiveValueAndRaise(property, entry, priority);
		}
	}

	public void OnFrameActivationChanged(ValueFrame frame)
	{
		if (frame.EntryCount != 0)
		{
			if (frame.EntryCount == 1)
			{
				AvaloniaProperty property = frame.GetEntry(0).Property;
				_effectiveValues.TryGetValue(property, out EffectiveValue value);
				ReevaluateEffectiveValue(property, value);
			}
			else
			{
				ReevaluateEffectiveValues();
			}
		}
	}

	public void OnInheritanceAncestorChanged(ValueStore? ancestor)
	{
		if (ancestor != this)
		{
			InheritanceAncestor = ancestor;
			if (_inheritedValueCount > 0)
			{
				return;
			}
		}
		IReadOnlyList<AvaloniaObject> inheritanceChildren = Owner.GetInheritanceChildren();
		if (inheritanceChildren != null)
		{
			int count = inheritanceChildren.Count;
			for (int i = 0; i < count; i++)
			{
				inheritanceChildren[i].GetValueStore().OnInheritanceAncestorChanged(ancestor);
			}
		}
	}

	public void OnInheritedEffectiveValueChanged<T>(StyledProperty<T> property, T oldValue, EffectiveValue<T> value)
	{
		IReadOnlyList<AvaloniaObject> inheritanceChildren = Owner.GetInheritanceChildren();
		if (inheritanceChildren != null)
		{
			int count = inheritanceChildren.Count;
			for (int i = 0; i < count; i++)
			{
				inheritanceChildren[i].GetValueStore().OnAncestorInheritedValueChanged(property, oldValue, value.Value);
			}
		}
	}

	public void OnInheritedEffectiveValueDisposed<T>(StyledProperty<T> property, T oldValue, T newValue)
	{
		IReadOnlyList<AvaloniaObject> inheritanceChildren = Owner.GetInheritanceChildren();
		if (inheritanceChildren != null)
		{
			int count = inheritanceChildren.Count;
			for (int i = 0; i < count; i++)
			{
				inheritanceChildren[i].GetValueStore().OnAncestorInheritedValueChanged(property, oldValue, newValue);
			}
		}
	}

	public void OnLocalValueBindingCompleted(AvaloniaProperty property, IDisposable observer)
	{
		if (_localValueBindings != null && _localValueBindings.TryGetValue(property.Id, out IDisposable value) && value == observer)
		{
			_localValueBindings?.Remove(property.Id);
			ClearValue(property);
		}
	}

	public void OnAncestorInheritedValueChanged<T>(StyledProperty<T> property, T oldValue, T newValue)
	{
		if (_effectiveValues.ContainsKey(property))
		{
			return;
		}
		using (PropertyNotifying.Start(Owner, property))
		{
			Owner.RaisePropertyChanged(property, oldValue, newValue, BindingPriority.Inherited, isEffectiveValue: true);
			IReadOnlyList<AvaloniaObject> inheritanceChildren = Owner.GetInheritanceChildren();
			if (inheritanceChildren != null)
			{
				int count = inheritanceChildren.Count;
				for (int i = 0; i < count; i++)
				{
					inheritanceChildren[i].GetValueStore().OnAncestorInheritedValueChanged(property, oldValue, newValue);
				}
			}
		}
	}

	public void OnValueEntryRemoved(ValueFrame frame, AvaloniaProperty property)
	{
		if (frame.EntryCount == 0)
		{
			_frames.Remove(frame);
		}
		if (TryGetEffectiveValue(property, out EffectiveValue value) && frame.Priority <= value.Priority)
		{
			ReevaluateEffectiveValue(property, value);
		}
	}

	public bool RemoveFrame(ValueFrame frame)
	{
		if (_frames.Remove(frame))
		{
			frame.Dispose();
			_frameGeneration++;
			ReevaluateEffectiveValues();
		}
		return false;
	}

	public void RemoveFrames(FrameType type)
	{
		bool flag = false;
		for (int num = _frames.Count - 1; num >= 0; num--)
		{
			ValueFrame valueFrame = _frames[num];
			if (!(valueFrame is ImmediateValueFrame) && valueFrame.FramePriority.IsType(type))
			{
				_frames.RemoveAt(num);
				valueFrame.Dispose();
				flag = true;
			}
		}
		if (flag)
		{
			_frameGeneration++;
			ReevaluateEffectiveValues();
		}
	}

	public void RemoveFrames(IReadOnlyList<IStyle> styles)
	{
		bool flag = false;
		for (int num = _frames.Count - 1; num >= 0; num--)
		{
			ValueFrame valueFrame = _frames[num];
			if (valueFrame is StyleInstance styleInstance && styles.Contains(styleInstance.Source))
			{
				_frames.RemoveAt(num);
				valueFrame.Dispose();
				flag = true;
			}
		}
		if (flag)
		{
			_frameGeneration++;
			ReevaluateEffectiveValues();
		}
	}

	public AvaloniaPropertyValue GetDiagnostic(AvaloniaProperty property)
	{
		bool isOverriddenCurrentValue = false;
		object value2;
		BindingPriority priority;
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			value2 = value.Value;
			priority = value.Priority;
			isOverriddenCurrentValue = value.IsOverridenCurrentValue;
		}
		else if (property.Inherits && TryGetInheritedValue(property, out value))
		{
			value2 = value.Value;
			priority = BindingPriority.Inherited;
		}
		else
		{
			value2 = GetDefaultValue(property);
			priority = BindingPriority.Unset;
		}
		return new AvaloniaPropertyValue(property, value2, priority, null, isOverriddenCurrentValue);
	}

	private int InsertFrame(ValueFrame frame)
	{
		int num = BinarySearchFrame(frame.FramePriority);
		_frames.Insert(num, frame);
		_frameGeneration++;
		frame.SetOwner(this);
		return num;
	}

	private ImmediateValueFrame GetOrCreateImmediateValueFrame(AvaloniaProperty property, BindingPriority priority, out int frameIndex)
	{
		int num = BinarySearchFrame(priority.ToFramePriority());
		if (num > 0 && _frames[num - 1] is ImmediateValueFrame immediateValueFrame && immediateValueFrame.Priority == priority && !immediateValueFrame.Contains(property))
		{
			frameIndex = num - 1;
			return immediateValueFrame;
		}
		ImmediateValueFrame immediateValueFrame2 = new ImmediateValueFrame(priority);
		frameIndex = InsertFrame(immediateValueFrame2);
		return immediateValueFrame2;
	}

	private void AddEffectiveValue(AvaloniaProperty property, EffectiveValue effectiveValue)
	{
		_effectiveValues.Add(property, effectiveValue);
		if (property.Inherits && _inheritedValueCount++ == 0)
		{
			OnInheritanceAncestorChanged(this);
		}
	}

	private void AddEffectiveValueAndRaise(AvaloniaProperty property, IValueEntry entry, BindingPriority priority)
	{
		EffectiveValue effectiveValue = property.CreateEffectiveValue(Owner);
		AddEffectiveValue(property, effectiveValue);
		effectiveValue.SetAndRaise(this, entry, priority);
	}

	private void RemoveEffectiveValue(AvaloniaProperty property, int index)
	{
		_effectiveValues.RemoveAt(index);
		if (property.Inherits && --_inheritedValueCount == 0)
		{
			OnInheritanceAncestorChanged(InheritanceAncestor);
		}
	}

	private bool RemoveEffectiveValue(AvaloniaProperty property)
	{
		if (_effectiveValues.Remove(property))
		{
			if (property.Inherits && --_inheritedValueCount == 0)
			{
				OnInheritanceAncestorChanged(InheritanceAncestor);
			}
			return true;
		}
		return false;
	}

	private void InheritedValueChanged(AvaloniaProperty property, EffectiveValue? oldValue, EffectiveValue? newValue)
	{
		if (_effectiveValues.ContainsKey(property))
		{
			return;
		}
		using (PropertyNotifying.Start(Owner, property))
		{
			(oldValue ?? newValue).RaiseInheritedValueChanged(Owner, property, oldValue, newValue);
			IReadOnlyList<AvaloniaObject> inheritanceChildren = Owner.GetInheritanceChildren();
			if (inheritanceChildren != null)
			{
				int count = inheritanceChildren.Count;
				for (int i = 0; i < count; i++)
				{
					inheritanceChildren[i].GetValueStore().InheritedValueChanged(property, oldValue, newValue);
				}
			}
		}
	}

	private void ReevaluateEffectiveValue(AvaloniaProperty property, EffectiveValue? current, IValueEntry? changedValueEntry = null, bool ignoreLocalValue = false)
	{
		_isEvaluating++;
		try
		{
			while (_styling <= 0)
			{
				int frameGeneration = _frameGeneration;
				current?.BeginReevaluation(ignoreLocalValue);
				int num = _frames.Count - 1;
				while (true)
				{
					if (num >= 0)
					{
						ValueFrame valueFrame = _frames[num];
						BindingPriority priority = valueFrame.Priority;
						if (current == null || current.Priority >= priority || current == null || current.BasePriority >= priority)
						{
							IValueEntry entry;
							bool activeChanged;
							bool flag = valueFrame.TryGetEntryIfActive(property, out entry, out activeChanged);
							if (activeChanged && valueFrame.EntryCount > 1)
							{
								ReevaluateEffectiveValues(changedValueEntry);
								return;
							}
							if (flag && HasHigherPriority(entry, priority, current, changedValueEntry) && entry.HasValue)
							{
								if (current != null)
								{
									current.SetAndRaise(this, entry, priority);
								}
								else
								{
									current = property.CreateEffectiveValue(Owner);
									AddEffectiveValue(property, current);
									current.SetAndRaise(this, entry, priority);
								}
							}
							if (frameGeneration != _frameGeneration)
							{
								break;
							}
							num--;
							continue;
						}
					}
					if (current == null)
					{
						return;
					}
					current.EndReevaluation(this, property);
					if (current.CanRemove())
					{
						if (current.BasePriority == BindingPriority.Unset)
						{
							RemoveEffectiveValue(property);
							current.DisposeAndRaiseUnset(this, property);
						}
						else
						{
							current.RemoveAnimationAndRaise(this, property);
						}
					}
					current.UnsubscribeIfNecessary();
					return;
				}
			}
		}
		finally
		{
			_isEvaluating--;
		}
	}

	private void ReevaluateEffectiveValues(IValueEntry? changedValueEntry = null)
	{
		_isEvaluating++;
		try
		{
			while (_styling <= 0)
			{
				int frameGeneration = _frameGeneration;
				int count = _effectiveValues.Count;
				for (int i = 0; i < count; i++)
				{
					_effectiveValues[i].BeginReevaluation();
				}
				int num = _frames.Count - 1;
				while (true)
				{
					if (num >= 0)
					{
						ValueFrame valueFrame = _frames[num];
						if (valueFrame.IsActive)
						{
							BindingPriority priority = valueFrame.Priority;
							count = valueFrame.EntryCount;
							for (int j = 0; j < count; j++)
							{
								IValueEntry entry = valueFrame.GetEntry(j);
								AvaloniaProperty property = entry.Property;
								_effectiveValues.TryGetValue(property, out EffectiveValue value);
								if (HasHigherPriority(entry, priority, value, changedValueEntry) && entry.HasValue)
								{
									if (value != null)
									{
										value.SetAndRaise(this, entry, priority);
									}
									else
									{
										EffectiveValue effectiveValue = property.CreateEffectiveValue(Owner);
										AddEffectiveValue(property, effectiveValue);
										effectiveValue.SetAndRaise(this, entry, priority);
									}
									if (frameGeneration != _frameGeneration)
									{
										goto end_IL_0123;
									}
								}
							}
						}
						num--;
						continue;
					}
					for (int num2 = _effectiveValues.Count - 1; num2 >= 0; num2--)
					{
						_effectiveValues.GetKeyValue(num2, out AvaloniaProperty key, out EffectiveValue value2);
						value2.EndReevaluation(this, key);
						if (value2.CanRemove())
						{
							RemoveEffectiveValue(key, num2);
							value2.DisposeAndRaiseUnset(this, key);
							if (num2 > _effectiveValues.Count)
							{
								break;
							}
						}
						value2.UnsubscribeIfNecessary();
					}
					return;
					continue;
					end_IL_0123:
					break;
				}
			}
		}
		finally
		{
			_isEvaluating--;
		}
	}

	private static bool HasHigherPriority(IValueEntry entry, BindingPriority entryPriority, EffectiveValue? current, IValueEntry? changedValueEntry)
	{
		if (current == null)
		{
			return true;
		}
		if (entryPriority < current.Priority && entryPriority < current.BasePriority)
		{
			return true;
		}
		if (entryPriority == current.Priority && current.IsOverridenCurrentValue && (current.ValueEntry != entry || entry == changedValueEntry))
		{
			return true;
		}
		if (entryPriority > BindingPriority.Animation && entryPriority < current.BasePriority)
		{
			return true;
		}
		return false;
	}

	private bool TryGetEffectiveValue(AvaloniaProperty property, [NotNullWhen(true)] out EffectiveValue? value)
	{
		if (_effectiveValues.TryGetValue(property, out value))
		{
			return true;
		}
		value = null;
		return false;
	}

	private EffectiveValue? GetEffectiveValue(AvaloniaProperty property)
	{
		if (_effectiveValues.TryGetValue(property, out EffectiveValue value))
		{
			return value;
		}
		return null;
	}

	private object? GetDefaultValue(AvaloniaProperty property)
	{
		return ((IStyledPropertyAccessor)property).GetDefaultValue(Owner.GetType());
	}

	private void DisposeExistingLocalValueBinding(AvaloniaProperty property)
	{
		if (_localValueBindings != null && _localValueBindings.TryGetValue(property.Id, out IDisposable value))
		{
			value.Dispose();
		}
	}

	private int BinarySearchFrame(FramePriority priority)
	{
		int num = 0;
		int num2 = _frames.Count - 1;
		while (num <= num2)
		{
			int num3 = num + (num2 - num >> 1);
			if (priority - _frames[num3].FramePriority <= 0)
			{
				num = num3 + 1;
			}
			else
			{
				num2 = num3 - 1;
			}
		}
		return num;
	}
}
