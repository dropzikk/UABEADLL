using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Platform;

namespace Avalonia.Markup.Xaml.XamlIl.Runtime;

public static class XamlIlRuntimeHelpers
{
	private class DeferredParentServiceProvider : IAvaloniaXamlIlParentStackProvider, IServiceProvider, IRootObjectProvider, IAvaloniaXamlIlControlTemplateProvider
	{
		private readonly IServiceProvider? _parentProvider;

		private readonly List<IResourceNode>? _parentResourceNodes;

		private readonly INameScope _nameScope;

		private IRuntimePlatform? _runtimePlatform;

		public IEnumerable<object> Parents => GetParents();

		public object RootObject { get; }

		public object IntermediateRootObject => RootObject;

		public DeferredParentServiceProvider(IServiceProvider? parentProvider, List<IResourceNode>? parentResourceNodes, object rootObject, INameScope nameScope)
		{
			_parentProvider = parentProvider;
			_parentResourceNodes = parentResourceNodes;
			_nameScope = nameScope;
			RootObject = rootObject;
		}

		private IEnumerable<object> GetParents()
		{
			if (_parentResourceNodes == null)
			{
				yield break;
			}
			foreach (IResourceNode parentResourceNode in _parentResourceNodes)
			{
				yield return parentResourceNode;
			}
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(INameScope))
			{
				return _nameScope;
			}
			if (serviceType == typeof(IAvaloniaXamlIlParentStackProvider))
			{
				return this;
			}
			if (serviceType == typeof(IRootObjectProvider))
			{
				return this;
			}
			if (serviceType == typeof(IAvaloniaXamlIlControlTemplateProvider))
			{
				return this;
			}
			if (serviceType == typeof(IRuntimePlatform))
			{
				if (_runtimePlatform == null)
				{
					_runtimePlatform = AvaloniaLocator.Current.GetService<IRuntimePlatform>();
				}
				return _runtimePlatform;
			}
			return _parentProvider?.GetService(serviceType);
		}
	}

	private class InnerServiceProvider : IServiceProvider
	{
		private readonly IServiceProvider _compiledProvider;

		private XamlTypeResolver? _resolver;

		public InnerServiceProvider(IServiceProvider compiledProvider)
		{
			_compiledProvider = compiledProvider;
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(IXamlTypeResolver))
			{
				return _resolver ?? (_resolver = new XamlTypeResolver(_compiledProvider.GetRequiredService<IAvaloniaXamlIlXmlNamespaceInfoProvider>()));
			}
			return null;
		}
	}

	private class XamlTypeResolver : IXamlTypeResolver
	{
		private readonly IAvaloniaXamlIlXmlNamespaceInfoProvider _nsInfo;

		public XamlTypeResolver(IAvaloniaXamlIlXmlNamespaceInfoProvider nsInfo)
		{
			_nsInfo = nsInfo;
		}

		[RequiresUnreferencedCode("XamlTypeResolver might require unreferenced code.")]
		public Type Resolve(string qualifiedTypeName)
		{
			string[] array = qualifiedTypeName.Split(new char[1] { ':' }, 2);
			string key;
			string text3;
			if (array.Length != 1)
			{
				string text = array[0];
				string text2 = array[1];
				key = text;
				text3 = text2;
			}
			else
			{
				string text2 = qualifiedTypeName;
				key = "";
				text3 = text2;
			}
			if (!_nsInfo.XmlNamespaces.TryGetValue(key, out IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo> value))
			{
				throw new ArgumentException("Unable to resolve namespace for type " + qualifiedTypeName);
			}
			IEnumerable<AvaloniaXamlIlXmlNamespaceInfo> enumerable = value.Where(delegate(AvaloniaXamlIlXmlNamespaceInfo e)
			{
				string clrAssemblyName = e.ClrAssemblyName;
				return clrAssemblyName != null && clrAssemblyName.Length > 0;
			});
			foreach (AvaloniaXamlIlXmlNamespaceInfo item in enumerable)
			{
				Type type = Assembly.Load(new AssemblyName(item.ClrAssemblyName)).GetType(item.ClrNamespace + "." + text3);
				if (type != null)
				{
					return type;
				}
			}
			throw new ArgumentException("Unable to resolve type " + qualifiedTypeName + " from any of the following locations: " + string.Join(",", enumerable.Select((AvaloniaXamlIlXmlNamespaceInfo e) => $"`clr-namespace:{e.ClrNamespace};assembly={e.ClrAssemblyName}`")))
			{
				HelpLink = "https://docs.avaloniaui.net/guides/basics/introduction-to-xaml#valid-xaml-namespaces"
			};
		}
	}

	private class RootServiceProvider : IServiceProvider
	{
		private class DefaultAvaloniaXamlIlParentStackProvider : IAvaloniaXamlIlParentStackProvider
		{
			public static DefaultAvaloniaXamlIlParentStackProvider Instance { get; } = new DefaultAvaloniaXamlIlParentStackProvider();

			public IEnumerable<object> Parents
			{
				get
				{
					if (Application.Current != null)
					{
						yield return Application.Current;
					}
				}
			}
		}

		private readonly INameScope _nameScope;

		private readonly IServiceProvider? _parentServiceProvider;

		private readonly IRuntimePlatform? _runtimePlatform;

		public RootServiceProvider(INameScope nameScope, IServiceProvider? parentServiceProvider)
		{
			_nameScope = nameScope;
			_parentServiceProvider = parentServiceProvider;
			_runtimePlatform = AvaloniaLocator.Current.GetService<IRuntimePlatform>();
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == typeof(INameScope))
			{
				return _nameScope;
			}
			if (serviceType == typeof(IAvaloniaXamlIlParentStackProvider))
			{
				return _parentServiceProvider?.GetService<IAvaloniaXamlIlParentStackProvider>() ?? DefaultAvaloniaXamlIlParentStackProvider.Instance;
			}
			if (serviceType == typeof(IRuntimePlatform))
			{
				return _runtimePlatform ?? throw new KeyNotFoundException("IRuntimePlatform was not registered");
			}
			return null;
		}
	}

	public static Func<IServiceProvider, object> DeferredTransformationFactoryV1(Func<IServiceProvider, object> builder, IServiceProvider provider)
	{
		return DeferredTransformationFactoryV2<Control>(builder, provider);
	}

	public static Func<IServiceProvider, object> DeferredTransformationFactoryV2<T>(Func<IServiceProvider, object> builder, IServiceProvider provider)
	{
		List<IResourceNode> resourceNodes = provider.GetRequiredService<IAvaloniaXamlIlParentStackProvider>().Parents.OfType<IResourceNode>().ToList();
		object rootObject = provider.GetRequiredService<IRootObjectProvider>().RootObject;
		INameScope parentScope = provider.GetService<INameScope>();
		return delegate(IServiceProvider sp)
		{
			INameScope nameScope2;
			if (parentScope == null)
			{
				INameScope nameScope = new NameScope();
				nameScope2 = nameScope;
			}
			else
			{
				INameScope nameScope = new ChildNameScope(parentScope);
				nameScope2 = nameScope;
			}
			INameScope nameScope3 = nameScope2;
			object obj = builder(new DeferredParentServiceProvider(sp, resourceNodes, rootObject, nameScope3));
			nameScope3.Complete();
			return (typeof(T) == typeof(Control)) ? ((ITemplateResult)new TemplateResult<Control>((Control)obj, nameScope3)) : ((ITemplateResult)new TemplateResult<T>((T)obj, nameScope3));
		};
	}

	public static void ApplyNonMatchingMarkupExtensionV1(object target, object property, IServiceProvider prov, object value)
	{
		if (value is IBinding binding)
		{
			if (property is AvaloniaProperty property2)
			{
				((AvaloniaObject)target).Bind(property2, binding);
				return;
			}
			throw new ArgumentException("Attempt to apply binding to non-avalonia property " + property);
		}
		if (value is UnsetValueType value2)
		{
			if (property is AvaloniaProperty property3)
			{
				((AvaloniaObject)target).SetValue(property3, value2);
			}
			return;
		}
		throw new ArgumentException("Don't know what to do with " + value.GetType());
	}

	public static IServiceProvider CreateInnerServiceProviderV1(IServiceProvider compiled)
	{
		return new InnerServiceProvider(compiled);
	}

	public static IServiceProvider CreateRootServiceProviderV2()
	{
		return new RootServiceProvider(new NameScope(), null);
	}

	public static IServiceProvider CreateRootServiceProviderV3(IServiceProvider parentServiceProvider)
	{
		return new RootServiceProvider(new NameScope(), parentServiceProvider);
	}
}
