using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Avalonia.Data.Core.Plugins;

public class ExceptionValidationPlugin : IDataValidationPlugin
{
	private sealed class Validator : DataValidationBase
	{
		public Validator(WeakReference<object?> reference, string name, IPropertyAccessor inner)
			: base(inner)
		{
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			try
			{
				return base.SetValue(value, priority);
			}
			catch (TargetInvocationException ex) when (ex.InnerException != null)
			{
				PublishValue(new BindingNotification(ex.InnerException, BindingErrorType.DataValidationError));
			}
			catch (Exception error)
			{
				PublishValue(new BindingNotification(error, BindingErrorType.DataValidationError));
			}
			return false;
		}
	}

	[RequiresUnreferencedCode("DataValidationPlugin might require unreferenced code.")]
	public bool Match(WeakReference<object?> reference, string memberName)
	{
		return true;
	}

	[RequiresUnreferencedCode("DataValidationPlugin might require unreferenced code.")]
	public IPropertyAccessor Start(WeakReference<object?> reference, string name, IPropertyAccessor inner)
	{
		return new Validator(reference, name, inner);
	}
}
