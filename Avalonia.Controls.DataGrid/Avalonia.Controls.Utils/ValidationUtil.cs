using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Avalonia.Data;

namespace Avalonia.Controls.Utils;

internal static class ValidationUtil
{
	public static bool ContainsMemberName(this ValidationResult validationResult, string target)
	{
		int num = 0;
		foreach (string memberName in validationResult.MemberNames)
		{
			if (string.Equals(target, memberName))
			{
				return true;
			}
			num++;
		}
		if (num == 0)
		{
			return string.IsNullOrEmpty(target);
		}
		return false;
	}

	public static ValidationResult FindEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
	{
		foreach (ValidationResult item in collection)
		{
			if (!(item.ErrorMessage == target.ErrorMessage))
			{
				continue;
			}
			bool flag = true;
			bool flag2 = true;
			IEnumerator<string> enumerator2 = item.MemberNames.GetEnumerator();
			IEnumerator<string> enumerator3 = target.MemberNames.GetEnumerator();
			while (flag && flag2)
			{
				flag = enumerator2.MoveNext();
				flag2 = enumerator3.MoveNext();
				if (!flag && !flag2)
				{
					return item;
				}
				if (flag != flag2 || enumerator2.Current != enumerator3.Current)
				{
					break;
				}
			}
		}
		return null;
	}

	public static bool IsValid(this ValidationResult result)
	{
		if (result != null)
		{
			return result == ValidationResult.Success;
		}
		return true;
	}

	public static IEnumerable<Exception> UnpackException(Exception exception)
	{
		if (exception != null)
		{
			IEnumerable<Exception> source;
			if (!(exception is AggregateException ex))
			{
				IEnumerable<Exception> enumerable = new Exception[1] { exception };
				source = enumerable;
			}
			else
			{
				IEnumerable<Exception> enumerable = ex.InnerExceptions;
				source = enumerable;
			}
			return source.Where((Exception x) => !(x is BindingChainException)).ToList();
		}
		return Array.Empty<Exception>();
	}

	public static object UnpackDataValidationException(Exception exception)
	{
		if (exception is DataValidationException ex)
		{
			return ex.ErrorData;
		}
		return exception;
	}

	public static bool ContainsEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
	{
		return collection.FindEqualValidationResult(target) != null;
	}

	public static void AddIfNew(this ICollection<ValidationResult> collection, ValidationResult value)
	{
		if (!collection.ContainsEqualValidationResult(value))
		{
			collection.Add(value);
		}
	}

	private static bool ExceptionsMatch(Exception e1, Exception e2)
	{
		return e1.Message == e2.Message;
	}

	public static void AddExceptionIfNew(this ICollection<Exception> collection, Exception value)
	{
		if (!collection.Any((Exception e) => ExceptionsMatch(e, value)))
		{
			collection.Add(value);
		}
	}

	public static void CatchNonCriticalExceptions(Action action)
	{
		try
		{
			action();
		}
		catch (Exception exception)
		{
			if (IsCriticalException(exception))
			{
				throw;
			}
		}
	}

	public static bool IsCriticalException(Exception exception)
	{
		if (!(exception is OutOfMemoryException) && !(exception is StackOverflowException) && !(exception is AccessViolationException))
		{
			return exception is ThreadAbortException;
		}
		return true;
	}
}
