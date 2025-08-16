using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Avalonia.Metadata;

[assembly: XmlnsDefinition("https://github.com/avaloniaui/avaloniaedit", "AvaloniaEdit")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui/avaloniaedit", "AvaloniaEdit.Editing")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui/avaloniaedit", "AvaloniaEdit.Rendering")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui/avaloniaedit", "AvaloniaEdit.Highlighting")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui/avaloniaedit", "AvaloniaEdit.Search")]
[assembly: InternalsVisibleTo("AvaloniaEdit.Tests")]
[assembly: AssemblyCompany("AvaloniaEdit")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("This project is a port of AvalonEdit, a WPF-based text editor for Avalonia.")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-beta")]
[assembly: AssemblyProduct("AvaloniaEdit")]
[assembly: AssemblyTitle("AvaloniaEdit")]
[assembly: AssemblyMetadata("AvaloniaUseCompiledBindingsByDefault", "True")]
[assembly: AssemblyVersion("1.0.0.0")]
[module: System.Runtime.CompilerServices.RefSafetyRules(11)]
