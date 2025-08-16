using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public abstract class DefinitionBase : AvaloniaObject
{
	[Flags]
	private enum Flags : byte
	{
		UseSharedMinimum = 0x20,
		LayoutWasUpdated = 0x40
	}

	internal class SharedSizeScope
	{
		private Hashtable _registry = new Hashtable();

		internal SharedSizeState EnsureSharedState(string sharedSizeGroup)
		{
			SharedSizeState sharedSizeState = _registry[sharedSizeGroup] as SharedSizeState;
			if (sharedSizeState == null)
			{
				sharedSizeState = new SharedSizeState(this, sharedSizeGroup);
				_registry[sharedSizeGroup] = sharedSizeState;
			}
			return sharedSizeState;
		}

		internal void Remove(object key)
		{
			_registry.Remove(key);
		}
	}

	internal class SharedSizeState
	{
		private readonly SharedSizeScope _sharedSizeScope;

		private readonly string _sharedSizeGroupId;

		private readonly List<DefinitionBase> _registry;

		private readonly EventHandler _layoutUpdated;

		private Control? _layoutUpdatedHost;

		private bool _broadcastInvalidation;

		private bool _userSizeValid;

		private GridLength _userSize;

		private double _minSize;

		internal double MinSize
		{
			get
			{
				if (!_userSizeValid)
				{
					EnsureUserSizeValid();
				}
				return _minSize;
			}
		}

		internal GridLength UserSize
		{
			get
			{
				if (!_userSizeValid)
				{
					EnsureUserSizeValid();
				}
				return _userSize;
			}
		}

		internal SharedSizeState(SharedSizeScope sharedSizeScope, string sharedSizeGroupId)
		{
			_sharedSizeScope = sharedSizeScope;
			_sharedSizeGroupId = sharedSizeGroupId;
			_registry = new List<DefinitionBase>();
			_layoutUpdated = OnLayoutUpdated;
			_broadcastInvalidation = true;
		}

		internal void AddMember(DefinitionBase member)
		{
			_registry.Add(member);
			Invalidate();
		}

		internal void RemoveMember(DefinitionBase member)
		{
			Invalidate();
			_registry.Remove(member);
			if (_registry.Count == 0)
			{
				_sharedSizeScope.Remove(_sharedSizeGroupId);
			}
		}

		internal void Invalidate()
		{
			_userSizeValid = false;
			if (_broadcastInvalidation)
			{
				int i = 0;
				for (int count = _registry.Count; i < count; i++)
				{
					_registry[i].Parent.Invalidate();
				}
				_broadcastInvalidation = false;
			}
		}

		internal void EnsureDeferredValidation(Control layoutUpdatedHost)
		{
			if (_layoutUpdatedHost == null)
			{
				_layoutUpdatedHost = layoutUpdatedHost;
				_layoutUpdatedHost.LayoutUpdated += _layoutUpdated;
			}
		}

		private void EnsureUserSizeValid()
		{
			_userSize = new GridLength(1.0, GridUnitType.Auto);
			int i = 0;
			for (int count = _registry.Count; i < count; i++)
			{
				GridLength userSizeValueCache = _registry[i].UserSizeValueCache;
				if (userSizeValueCache.GridUnitType == GridUnitType.Pixel)
				{
					if (_userSize.GridUnitType == GridUnitType.Auto)
					{
						_userSize = userSizeValueCache;
					}
					else if (_userSize.Value < userSizeValueCache.Value)
					{
						_userSize = userSizeValueCache;
					}
				}
			}
			_minSize = (_userSize.IsAbsolute ? _userSize.Value : 0.0);
			_userSizeValid = true;
		}

		private void OnLayoutUpdated(object? sender, EventArgs e)
		{
			double num = 0.0;
			int i = 0;
			for (int count = _registry.Count; i < count; i++)
			{
				num = Math.Max(num, _registry[i].MinSize);
			}
			bool flag = !MathUtilities.AreClose(_minSize, num);
			int j = 0;
			for (int count2 = _registry.Count; j < count2; j++)
			{
				DefinitionBase definitionBase = _registry[j];
				bool flag2 = !MathUtilities.AreClose(definitionBase._minSize, num);
				if (!((!definitionBase.UseSharedMinimum) ? (!flag2) : ((!flag2) ? (definitionBase.LayoutWasUpdated && MathUtilities.GreaterThanOrClose(definitionBase._minSize, MinSize)) : (!flag))))
				{
					definitionBase.Parent.InvalidateMeasure();
				}
				else if (!MathUtilities.AreClose(num, definitionBase.SizeCache))
				{
					definitionBase.Parent.InvalidateArrange();
				}
				definitionBase.UseSharedMinimum = flag2;
				definitionBase.LayoutWasUpdated = false;
			}
			_minSize = num;
			_layoutUpdatedHost.LayoutUpdated -= _layoutUpdated;
			_layoutUpdatedHost = null;
			_broadcastInvalidation = true;
		}
	}

	private Flags _flags;

	internal int _parentIndex = -1;

	private Grid.LayoutTimeSizeType _sizeType;

	private double _minSize;

	private double _measureSize;

	private double _sizeCache;

	private double _offset;

	private SharedSizeState? _sharedState;

	internal static readonly AttachedProperty<SharedSizeScope?> PrivateSharedSizeScopeProperty;

	public static readonly AttachedProperty<string?> SharedSizeGroupProperty;

	public string? SharedSizeGroup
	{
		get
		{
			return GetValue(SharedSizeGroupProperty);
		}
		set
		{
			SetValue(SharedSizeGroupProperty, value);
		}
	}

	internal bool IsShared => _sharedState != null;

	internal GridLength UserSize
	{
		get
		{
			if (_sharedState == null)
			{
				return UserSizeValueCache;
			}
			return _sharedState.UserSize;
		}
	}

	internal double UserMinSize => UserMinSizeValueCache;

	internal double UserMaxSize => UserMaxSizeValueCache;

	internal int Index
	{
		get
		{
			return _parentIndex;
		}
		set
		{
			_parentIndex = value;
		}
	}

	internal Grid.LayoutTimeSizeType SizeType
	{
		get
		{
			return _sizeType;
		}
		set
		{
			_sizeType = value;
		}
	}

	internal double MeasureSize
	{
		get
		{
			return _measureSize;
		}
		set
		{
			_measureSize = value;
		}
	}

	internal double PreferredSize
	{
		get
		{
			double num = MinSize;
			if (_sizeType != Grid.LayoutTimeSizeType.Auto && num < _measureSize)
			{
				num = _measureSize;
			}
			return num;
		}
	}

	internal double SizeCache
	{
		get
		{
			return _sizeCache;
		}
		set
		{
			_sizeCache = value;
		}
	}

	internal double MinSize
	{
		get
		{
			double minSize = _minSize;
			if (UseSharedMinimum && _sharedState != null && minSize < _sharedState.MinSize)
			{
				minSize = _sharedState.MinSize;
			}
			return minSize;
		}
	}

	internal double MinSizeForArrange
	{
		get
		{
			double minSize = _minSize;
			if (_sharedState != null && (UseSharedMinimum || !LayoutWasUpdated) && minSize < _sharedState.MinSize)
			{
				minSize = _sharedState.MinSize;
			}
			return minSize;
		}
	}

	internal double FinalOffset
	{
		get
		{
			return _offset;
		}
		set
		{
			_offset = value;
		}
	}

	internal abstract GridLength UserSizeValueCache { get; }

	internal abstract double UserMinSizeValueCache { get; }

	internal abstract double UserMaxSizeValueCache { get; }

	internal Grid? Parent { get; set; }

	private bool UseSharedMinimum
	{
		get
		{
			return CheckFlagsAnd(Flags.UseSharedMinimum);
		}
		set
		{
			SetFlags(value, Flags.UseSharedMinimum);
		}
	}

	private bool LayoutWasUpdated
	{
		get
		{
			return CheckFlagsAnd(Flags.LayoutWasUpdated);
		}
		set
		{
			SetFlags(value, Flags.LayoutWasUpdated);
		}
	}

	internal void OnEnterParentTree()
	{
		base.InheritanceParent = Parent;
		if (_sharedState == null)
		{
			string sharedSizeGroup = SharedSizeGroup;
			if (sharedSizeGroup != null)
			{
				SharedSizeScope value = GetValue(PrivateSharedSizeScopeProperty);
				if (value != null)
				{
					_sharedState = value.EnsureSharedState(sharedSizeGroup);
					_sharedState.AddMember(this);
				}
			}
		}
		Parent?.InvalidateMeasure();
	}

	internal void OnExitParentTree()
	{
		_offset = 0.0;
		if (_sharedState != null)
		{
			_sharedState.RemoveMember(this);
			_sharedState = null;
		}
		Parent?.InvalidateMeasure();
	}

	internal void OnBeforeLayout(Grid grid)
	{
		_minSize = 0.0;
		LayoutWasUpdated = true;
		if (_sharedState != null)
		{
			_sharedState.EnsureDeferredValidation(grid);
		}
	}

	internal void UpdateMinSize(double minSize)
	{
		_minSize = Math.Max(_minSize, minSize);
	}

	internal void SetMinSize(double minSize)
	{
		_minSize = minSize;
	}

	internal static void OnIsSharedSizeScopePropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
	{
		if ((bool)e.NewValue)
		{
			SharedSizeScope value = new SharedSizeScope();
			d.SetValue(PrivateSharedSizeScopeProperty, value);
		}
		else
		{
			d.ClearValue(PrivateSharedSizeScopeProperty);
		}
	}

	internal static void OnUserSizePropertyChanged(DefinitionBase definition, AvaloniaPropertyChangedEventArgs e)
	{
		if (definition.Parent == null)
		{
			return;
		}
		if (definition._sharedState != null)
		{
			definition._sharedState.Invalidate();
			return;
		}
		GridUnitType gridUnitType = ((GridLength)e.OldValue).GridUnitType;
		GridUnitType gridUnitType2 = ((GridLength)e.NewValue).GridUnitType;
		if (gridUnitType != gridUnitType2)
		{
			definition.Parent.Invalidate();
		}
		else
		{
			definition.Parent.InvalidateMeasure();
		}
	}

	private void SetFlags(bool value, Flags flags)
	{
		_flags = (value ? (_flags | flags) : ((Flags)((uint)_flags & (uint)(byte)(~(int)flags))));
	}

	private bool CheckFlagsAnd(Flags flags)
	{
		return (_flags & flags) == flags;
	}

	private static void OnSharedSizeGroupPropertyChanged(DefinitionBase definition, AvaloniaPropertyChangedEventArgs<string?> e)
	{
		if (definition.Parent == null)
		{
			return;
		}
		string value = e.NewValue.Value;
		if (definition._sharedState != null)
		{
			definition._sharedState.RemoveMember(definition);
			definition._sharedState = null;
		}
		if (definition._sharedState == null && value != null)
		{
			SharedSizeScope value2 = definition.GetValue(PrivateSharedSizeScopeProperty);
			if (value2 != null)
			{
				definition._sharedState = value2.EnsureSharedState(value);
				definition._sharedState.AddMember(definition);
			}
		}
	}

	private static bool SharedSizeGroupPropertyValueValid(string? id)
	{
		if (id == null)
		{
			return true;
		}
		if (id.Length > 0)
		{
			int num = -1;
			while (++num < id.Length)
			{
				bool flag = char.IsDigit(id[num]);
				if ((num == 0 && flag) || (!flag && !char.IsLetter(id[num]) && '_' != id[num]))
				{
					break;
				}
			}
			if (num == id.Length)
			{
				return true;
			}
		}
		return false;
	}

	private static void OnPrivateSharedSizeScopePropertyChanged(DefinitionBase definition, AvaloniaPropertyChangedEventArgs<SharedSizeScope?> e)
	{
		if (definition.Parent == null)
		{
			return;
		}
		if (definition._sharedState != null)
		{
			definition._sharedState.RemoveMember(definition);
			definition._sharedState = null;
		}
		if (definition._sharedState != null)
		{
			return;
		}
		SharedSizeScope value = e.NewValue.Value;
		if (value != null)
		{
			string sharedSizeGroup = definition.SharedSizeGroup;
			if (sharedSizeGroup != null)
			{
				definition._sharedState = value.EnsureSharedState(sharedSizeGroup);
				definition._sharedState.AddMember(definition);
			}
		}
	}

	static DefinitionBase()
	{
		PrivateSharedSizeScopeProperty = AvaloniaProperty.RegisterAttached<DefinitionBase, Control, SharedSizeScope>("PrivateSharedSizeScope", null, inherits: true);
		SharedSizeGroupProperty = AvaloniaProperty.RegisterAttached<DefinitionBase, Control, string>("SharedSizeGroup", null, inherits: false, BindingMode.OneWay, SharedSizeGroupPropertyValueValid);
		SharedSizeGroupProperty.Changed.AddClassHandler<DefinitionBase, string>(OnSharedSizeGroupPropertyChanged);
		PrivateSharedSizeScopeProperty.Changed.AddClassHandler<DefinitionBase, SharedSizeScope>(OnPrivateSharedSizeScopePropertyChanged);
	}

	protected static void AffectsParentMeasure(params AvaloniaProperty[] properties)
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			(e.Sender as DefinitionBase)?.Parent?.InvalidateMeasure();
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}
}
