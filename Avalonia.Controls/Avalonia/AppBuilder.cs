using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform;

namespace Avalonia;

public sealed class AppBuilder
{
	public delegate void AppMainDelegate(Application app, string[] args);

	private static bool s_setupWasAlreadyCalled;

	private Action? _optionsInitializers;

	private Func<Application>? _appFactory;

	private IApplicationLifetime? _lifetime;

	public Action? RuntimePlatformServicesInitializer { get; private set; }

	public string? RuntimePlatformServicesName { get; private set; }

	public Application? Instance { get; private set; }

	public Type? ApplicationType { get; private set; }

	public Action? WindowingSubsystemInitializer { get; private set; }

	public string? WindowingSubsystemName { get; private set; }

	public Action? RenderingSubsystemInitializer { get; private set; }

	public string? RenderingSubsystemName { get; private set; }

	public Action<AppBuilder> AfterSetupCallback { get; private set; } = delegate
	{
	};

	public Action<AppBuilder> AfterPlatformServicesSetupCallback { get; private set; } = delegate
	{
	};

	private AppBuilder Self => this;

	private AppBuilder()
	{
	}

	public static AppBuilder Configure<TApp>() where TApp : Application, new()
	{
		return new AppBuilder
		{
			ApplicationType = typeof(TApp),
			_appFactory = () => new TApp()
		};
	}

	public static AppBuilder Configure<TApp>(Func<TApp> appFactory) where TApp : Application
	{
		return new AppBuilder
		{
			ApplicationType = typeof(TApp),
			_appFactory = appFactory
		};
	}

	internal static AppBuilder Configure([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type entryPointType)
	{
		if (entryPointType.GetMethod("BuildAvaloniaApp", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, Array.Empty<Type>(), null)?.Invoke(null, Array.Empty<object>()) is AppBuilder result)
		{
			return result;
		}
		if (typeof(Application).IsAssignableFrom(entryPointType))
		{
			return Configure(() => (Application)Activator.CreateInstance(entryPointType));
		}
		throw new InvalidOperationException("Unable to create AppBuilder from type \"" + entryPointType.FullName + "\". Input type either needs to have BuildAvaloniaApp -> AppBuilder method or inherit Application type.");
	}

	public AppBuilder AfterSetup(Action<AppBuilder> callback)
	{
		AfterSetupCallback = (Action<AppBuilder>)Delegate.Combine(AfterSetupCallback, callback);
		return Self;
	}

	public AppBuilder AfterPlatformServicesSetup(Action<AppBuilder> callback)
	{
		AfterPlatformServicesSetupCallback = (Action<AppBuilder>)Delegate.Combine(AfterPlatformServicesSetupCallback, callback);
		return Self;
	}

	public void Start(AppMainDelegate main, string[] args)
	{
		Setup();
		main(Instance, args);
	}

	public AppBuilder SetupWithoutStarting()
	{
		Setup();
		return Self;
	}

	public AppBuilder SetupWithLifetime(IApplicationLifetime lifetime)
	{
		_lifetime = lifetime;
		Setup();
		return Self;
	}

	public AppBuilder UseWindowingSubsystem(Action initializer, string name = "")
	{
		WindowingSubsystemInitializer = initializer;
		WindowingSubsystemName = name;
		return Self;
	}

	public AppBuilder UseRenderingSubsystem(Action initializer, string name = "")
	{
		RenderingSubsystemInitializer = initializer;
		RenderingSubsystemName = name;
		return Self;
	}

	public AppBuilder UseRuntimePlatformSubsystem(Action initializer, string name = "")
	{
		RuntimePlatformServicesInitializer = initializer;
		RuntimePlatformServicesName = name;
		return Self;
	}

	public AppBuilder UseStandardRuntimePlatformSubsystem()
	{
		RuntimePlatformServicesInitializer = delegate
		{
			StandardRuntimePlatformServices.Register(ApplicationType?.Assembly);
		};
		RuntimePlatformServicesName = "StandardRuntimePlatform";
		return Self;
	}

	public AppBuilder With<T>(T options)
	{
		_optionsInitializers = (Action)Delegate.Combine(_optionsInitializers, (Action)delegate
		{
			AvaloniaLocator.CurrentMutable.Bind<T>().ToConstant(options);
		});
		return Self;
	}

	public AppBuilder With<T>(Func<T> options)
	{
		_optionsInitializers = (Action)Delegate.Combine(_optionsInitializers, (Action)delegate
		{
			AvaloniaLocator.CurrentMutable.Bind<T>().ToFunc(options);
		});
		return Self;
	}

	public AppBuilder ConfigureFonts(Action<FontManager> action)
	{
		return AfterSetup(delegate
		{
			action?.Invoke(FontManager.Current);
		});
	}

	private void Setup()
	{
		if (RuntimePlatformServicesInitializer == null)
		{
			throw new InvalidOperationException("No runtime platform services configured.");
		}
		if (WindowingSubsystemInitializer == null)
		{
			throw new InvalidOperationException("No windowing system configured.");
		}
		if (RenderingSubsystemInitializer == null)
		{
			throw new InvalidOperationException("No rendering system configured.");
		}
		if (_appFactory == null)
		{
			throw new InvalidOperationException("No Application factory configured.");
		}
		if (s_setupWasAlreadyCalled)
		{
			throw new InvalidOperationException("Setup was already called on one of AppBuilder instances");
		}
		s_setupWasAlreadyCalled = true;
		SetupUnsafe();
	}

	internal void SetupUnsafe()
	{
		_optionsInitializers?.Invoke();
		RuntimePlatformServicesInitializer?.Invoke();
		RenderingSubsystemInitializer?.Invoke();
		WindowingSubsystemInitializer?.Invoke();
		AfterPlatformServicesSetupCallback?.Invoke(Self);
		Instance = _appFactory();
		Instance.ApplicationLifetime = _lifetime;
		AvaloniaLocator.CurrentMutable.BindToSelf(Instance);
		Instance.RegisterServices();
		Instance.Initialize();
		AfterSetupCallback?.Invoke(Self);
		Instance.OnFrameworkInitializationCompleted();
	}
}
