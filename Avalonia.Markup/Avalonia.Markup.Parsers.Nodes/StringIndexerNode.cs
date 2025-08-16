using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers.Nodes;

[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
internal class StringIndexerNode : IndexerNodeBase
{
	public override string Description => "[" + string.Join(",", Arguments) + "]";

	public IList<string> Arguments { get; }

	public override Type? PropertyType
	{
		get
		{
			if (!base.Target.TryGetTarget(out object target) || target == null)
			{
				return null;
			}
			return GetIndexer(target.GetType().GetTypeInfo())?.PropertyType;
		}
	}

	public StringIndexerNode(IList<string> arguments)
	{
		Arguments = arguments;
	}

	protected override bool SetTargetValueCore(object? value, BindingPriority priority)
	{
		if (!base.Target.TryGetTarget(out object target) || target == null)
		{
			return false;
		}
		TypeInfo typeInfo = target.GetType().GetTypeInfo();
		IList list = target as IList;
		IDictionary dictionary = target as IDictionary;
		PropertyInfo indexer = GetIndexer(typeInfo);
		ParameterInfo[] indexParameters;
		if (indexer != null && (indexParameters = indexer.GetIndexParameters()).Length == Arguments.Count)
		{
			object[] array = new object[indexParameters.Length];
			for (int i = 0; i < Arguments.Count; i++)
			{
				object result = null;
				if (!TypeUtilities.TryConvert(indexParameters[i].ParameterType, Arguments[i], CultureInfo.InvariantCulture, out result))
				{
					return false;
				}
				array[i] = result;
			}
			int[] array2 = array.OfType<int>().ToArray();
			if (typeInfo.IsArray)
			{
				return SetValueInArray((Array)target, array2, value);
			}
			if (Arguments.Count == 1)
			{
				if (list != null)
				{
					if (array2.Length == Arguments.Count && array2[0] >= 0 && array2[0] < list.Count)
					{
						list[array2[0]] = value;
						return true;
					}
					return false;
				}
				if (dictionary != null)
				{
					if (dictionary.Contains(array[0]))
					{
						dictionary[array[0]] = value;
						return true;
					}
					dictionary.Add(array[0], value);
					return true;
				}
				indexer.SetValue(target, value, array);
				return true;
			}
			indexer.SetValue(target, value, array);
			return true;
		}
		if (typeInfo.IsArray)
		{
			SetValueInArray((Array)target, value);
			return true;
		}
		return false;
	}

	private bool SetValueInArray(Array array, object? value)
	{
		if (!ConvertArgumentsToInts(out int[] intArgs))
		{
			return false;
		}
		return SetValueInArray(array, intArgs);
	}

	private static bool SetValueInArray(Array array, int[] indices, object? value)
	{
		if (ValidBounds(indices, array))
		{
			array.SetValue(value, indices);
			return true;
		}
		return false;
	}

	protected override object? GetValue(object? target)
	{
		if (target == null)
		{
			return null;
		}
		TypeInfo typeInfo = target.GetType().GetTypeInfo();
		IList list = target as IList;
		IDictionary dictionary = target as IDictionary;
		PropertyInfo indexer = GetIndexer(typeInfo);
		ParameterInfo[] indexParameters;
		if (indexer != null && (indexParameters = indexer.GetIndexParameters()).Length == Arguments.Count)
		{
			object[] array = new object[indexParameters.Length];
			for (int i = 0; i < Arguments.Count; i++)
			{
				object result = null;
				if (!TypeUtilities.TryConvert(indexParameters[i].ParameterType, Arguments[i], CultureInfo.InvariantCulture, out result))
				{
					return AvaloniaProperty.UnsetValue;
				}
				array[i] = result;
			}
			int[] array2 = array.OfType<int>().ToArray();
			if (typeInfo.IsArray)
			{
				return GetValueFromArray((Array)target, array2);
			}
			if (Arguments.Count == 1)
			{
				if (list != null)
				{
					if (array2.Length == Arguments.Count && array2[0] >= 0 && array2[0] < list.Count)
					{
						return list[array2[0]];
					}
					return AvaloniaProperty.UnsetValue;
				}
				if (dictionary != null)
				{
					if (dictionary.Contains(array[0]))
					{
						return dictionary[array[0]];
					}
					return AvaloniaProperty.UnsetValue;
				}
				return indexer.GetValue(target, array);
			}
			return indexer.GetValue(target, array);
		}
		if (typeInfo.IsArray)
		{
			return GetValueFromArray((Array)target);
		}
		return AvaloniaProperty.UnsetValue;
	}

	private object? GetValueFromArray(Array array)
	{
		if (!ConvertArgumentsToInts(out int[] intArgs))
		{
			return AvaloniaProperty.UnsetValue;
		}
		return GetValueFromArray(array, intArgs);
	}

	private static object? GetValueFromArray(Array array, int[] indices)
	{
		if (ValidBounds(indices, array))
		{
			return array.GetValue(indices);
		}
		return AvaloniaProperty.UnsetValue;
	}

	private bool ConvertArgumentsToInts(out int[] intArgs)
	{
		intArgs = new int[Arguments.Count];
		for (int i = 0; i < Arguments.Count; i++)
		{
			if (!TypeUtilities.TryConvert(typeof(int), Arguments[i], CultureInfo.InvariantCulture, out object result))
			{
				return false;
			}
			intArgs[i] = (int)result;
		}
		return true;
	}

	private static PropertyInfo? GetIndexer(TypeInfo? typeInfo)
	{
		while (typeInfo != null)
		{
			PropertyInfo declaredProperty;
			if ((declaredProperty = typeInfo.GetDeclaredProperty("Item")) != null)
			{
				return declaredProperty;
			}
			foreach (PropertyInfo declaredProperty2 in typeInfo.DeclaredProperties)
			{
				if (declaredProperty2.GetIndexParameters().Any())
				{
					return declaredProperty2;
				}
			}
			typeInfo = typeInfo.BaseType?.GetTypeInfo();
		}
		return null;
	}

	private static bool ValidBounds(int[] indices, Array array)
	{
		if (indices.Length == array.Rank)
		{
			for (int i = 0; i < indices.Length; i++)
			{
				if (indices[i] >= array.GetLength(i))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	protected override bool ShouldUpdate(object? sender, PropertyChangedEventArgs e)
	{
		if (sender == null || e.PropertyName == null)
		{
			return false;
		}
		return sender.GetType().GetTypeInfo().GetDeclaredProperty(e.PropertyName)?.GetIndexParameters().Any() ?? false;
	}

	protected override int? TryGetFirstArgumentAsInt()
	{
		if (TypeUtilities.TryConvert(typeof(int), Arguments[0], CultureInfo.InvariantCulture, out object result))
		{
			return (int?)result;
		}
		return null;
	}
}
