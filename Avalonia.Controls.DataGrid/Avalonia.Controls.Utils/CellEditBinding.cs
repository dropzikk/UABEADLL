using System;
using System.Collections.Generic;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia.Controls.Utils;

internal class CellEditBinding : ICellEditBinding
{
	private class SubjectWrapper : LightweightObservableBase<object>, IAvaloniaSubject<object>, IObserver<object>, IObservable<object>, IDisposable
	{
		private readonly IAvaloniaSubject<object> _sourceSubject;

		private readonly CellEditBinding _editBinding;

		private IDisposable _subscription;

		private object _controlValue;

		private bool _isControlValueSet;

		private bool _settingSourceValue;

		public SubjectWrapper(IAvaloniaSubject<object> bindingSourceSubject, CellEditBinding editBinding)
		{
			_sourceSubject = bindingSourceSubject;
			_editBinding = editBinding;
		}

		private void SetSourceValue(object value)
		{
			if (!_settingSourceValue)
			{
				_settingSourceValue = true;
				_sourceSubject.OnNext(value);
				_settingSourceValue = false;
			}
		}

		private void SetControlValue(object value)
		{
			PublishNext(value);
		}

		private void OnValidationError(BindingNotification notification)
		{
			if (notification.Error == null)
			{
				return;
			}
			_editBinding.AlterValidationErrors(delegate(List<Exception> errors)
			{
				errors.Clear();
				IEnumerable<Exception> enumerable = ValidationUtil.UnpackException(notification.Error);
				if (enumerable != null)
				{
					errors.AddRange(enumerable);
				}
			});
		}

		private void OnControlValueUpdated(object value)
		{
			_controlValue = value;
			_isControlValueSet = true;
			if (!_editBinding.IsValid)
			{
				SetSourceValue(value);
			}
		}

		private void OnSourceValueUpdated(object value)
		{
			if (value is BindingNotification bindingNotification)
			{
				if (bindingNotification.ErrorType != 0)
				{
					OnValidationError(bindingNotification);
				}
				else
				{
					OnValidValue(value);
				}
			}
			else
			{
				OnValidValue(value);
			}
			void OnValidValue(object val)
			{
				SetControlValue(val);
				_editBinding.AlterValidationErrors(delegate(List<Exception> errors)
				{
					errors.Clear();
				});
			}
		}

		protected override void Deinitialize()
		{
			_subscription?.Dispose();
			_subscription = null;
		}

		protected override void Initialize()
		{
			_subscription = _sourceSubject.Subscribe(OnSourceValueUpdated);
		}

		void IObserver<object>.OnCompleted()
		{
			throw new NotImplementedException();
		}

		void IObserver<object>.OnError(Exception error)
		{
			throw new NotImplementedException();
		}

		void IObserver<object>.OnNext(object value)
		{
			OnControlValueUpdated(value);
		}

		public void Dispose()
		{
			_subscription?.Dispose();
			_subscription = null;
		}

		public void CommitEdit()
		{
			if (_isControlValueSet)
			{
				SetSourceValue(_controlValue);
			}
		}
	}

	private readonly LightweightSubject<bool> _changedSubject = new LightweightSubject<bool>();

	private readonly List<Exception> _validationErrors = new List<Exception>();

	private readonly SubjectWrapper _inner;

	public bool IsValid => _validationErrors.Count <= 0;

	public IEnumerable<Exception> ValidationErrors => _validationErrors;

	public IObservable<bool> ValidationChanged => _changedSubject;

	public IAvaloniaSubject<object> InternalSubject => _inner;

	public CellEditBinding(IAvaloniaSubject<object> bindingSourceSubject)
	{
		_inner = new SubjectWrapper(bindingSourceSubject, this);
	}

	private void AlterValidationErrors(Action<List<Exception>> action)
	{
		bool isValid = IsValid;
		action(_validationErrors);
		bool isValid2 = IsValid;
		if (!isValid2 || !isValid)
		{
			_changedSubject.OnNext(isValid2);
		}
	}

	public bool CommitEdit()
	{
		_inner.CommitEdit();
		return IsValid;
	}
}
