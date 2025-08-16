using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Avalonia.Remote.Protocol;

public class DefaultMessageTypeResolver : IMessageTypeResolver
{
	private readonly Dictionary<Guid, Type> _guidsToTypes = new Dictionary<Guid, Type>();

	private readonly Dictionary<Type, Guid> _typesToGuids = new Dictionary<Type, Guid>();

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "If type was trimmed, we don't need to resolve it in the remove protocol")]
	public DefaultMessageTypeResolver(params Assembly[] assemblies)
	{
		foreach (Assembly item in (assemblies ?? Array.Empty<Assembly>()).Concat(new Assembly[1] { typeof(AvaloniaRemoteMessageGuidAttribute).GetTypeInfo().Assembly }))
		{
			foreach (Type exportedType in item.ExportedTypes)
			{
				AvaloniaRemoteMessageGuidAttribute customAttribute = exportedType.GetTypeInfo().GetCustomAttribute<AvaloniaRemoteMessageGuidAttribute>();
				if (customAttribute != null)
				{
					_guidsToTypes[customAttribute.Guid] = exportedType;
					_typesToGuids[exportedType] = customAttribute.Guid;
				}
			}
		}
	}

	public Type GetByGuid(Guid id)
	{
		return _guidsToTypes[id];
	}

	public Guid GetGuid(Type type)
	{
		return _typesToGuids[type];
	}
}
