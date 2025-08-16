using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data.Core.Plugins;

namespace Avalonia.Data.Core;

[RequiresUnreferencedCode("ExpressionNode might require unreferenced code.")]
internal class PropertyAccessorNode : SettableNode
{
	private readonly bool _enableValidation;

	private IPropertyAccessorPlugin? _customPlugin;

	private IPropertyAccessor? _accessor;

	public override string Description => PropertyName;

	public string PropertyName { get; }

	public override Type? PropertyType => _accessor?.PropertyType;

	public PropertyAccessorNode(string propertyName, bool enableValidation)
	{
		PropertyName = propertyName;
		_enableValidation = enableValidation;
	}

	public PropertyAccessorNode(string propertyName, bool enableValidation, IPropertyAccessorPlugin customPlugin)
	{
		PropertyName = propertyName;
		_enableValidation = enableValidation;
		_customPlugin = customPlugin;
	}

	protected override bool SetTargetValueCore(object? value, BindingPriority priority)
	{
		if (_accessor != null)
		{
			try
			{
				return _accessor.SetValue(value, priority);
			}
			catch
			{
			}
		}
		return false;
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		if (!reference.TryGetTarget(out object target) || target == null)
		{
			return;
		}
		IPropertyAccessor propertyAccessor = (_customPlugin ?? GetPropertyAccessorPluginForObject(target))?.Start(reference, PropertyName);
		if (propertyAccessor == null)
		{
			reference.TryGetTarget(out object target2);
			propertyAccessor = new PropertyError(new BindingNotification(new MissingMemberException($"Could not find a matching property accessor for '{PropertyName}' on '{target2}'"), BindingErrorType.Error));
		}
		if (_enableValidation && base.Next == null)
		{
			foreach (IDataValidationPlugin dataValidator in ExpressionObserver.DataValidators)
			{
				if (dataValidator.Match(reference, PropertyName))
				{
					propertyAccessor = dataValidator.Start(reference, PropertyName, propertyAccessor);
				}
			}
		}
		if (propertyAccessor == null)
		{
			throw new AvaloniaInternalException("Data validators must return non-null accessor.");
		}
		_accessor = propertyAccessor;
		propertyAccessor.Subscribe(base.ValueChanged);
	}

	private IPropertyAccessorPlugin? GetPropertyAccessorPluginForObject(object target)
	{
		foreach (IPropertyAccessorPlugin propertyAccessor in ExpressionObserver.PropertyAccessors)
		{
			if (propertyAccessor.Match(target, PropertyName))
			{
				return propertyAccessor;
			}
		}
		return null;
	}

	protected override void StopListeningCore()
	{
		_accessor?.Dispose();
		_accessor = null;
	}
}
