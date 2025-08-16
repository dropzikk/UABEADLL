using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics.ViewModels;

internal static class ReactiveExtensions
{
	public static IObservable<TValue> GetObservable<TOwner, TValue>(this TOwner vm, Expression<Func<TOwner, TValue>> property, bool fireImmediately = true) where TOwner : INotifyPropertyChanged
	{
		return Observable.Create(delegate(IObserver<TValue> o)
		{
			PropertyInfo propertyInfo = property.GetPropertyInfo();
			if (fireImmediately)
			{
				Fire();
			}
			vm.PropertyChanged += OnPropertyChanged;
			return Disposable.Create(delegate
			{
				vm.PropertyChanged -= OnPropertyChanged;
			});
			void Fire()
			{
				o.OnNext((TValue)propertyInfo.GetValue(vm));
			}
			void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == propertyInfo.Name)
				{
					Fire();
				}
			}
		});
	}

	private static PropertyInfo GetPropertyInfo<TOwner, TValue>(this Expression<Func<TOwner, TValue>> property)
	{
		if (property.Body is UnaryExpression unaryExpression)
		{
			return (PropertyInfo)((MemberExpression)unaryExpression.Operand).Member;
		}
		return (PropertyInfo)((MemberExpression)property.Body).Member;
	}
}
