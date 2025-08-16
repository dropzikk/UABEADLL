using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;

[assembly: InternalsVisibleTo("TextMateSharp.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001000db16f8de24159e7ee94e32addce2a9b60f3ea5be200ae7b5abbf8676705064a1b5a5a44d570a884bd86bd2d3e83411fb88914e00028bc7d4b5be1ba8fd8db4335e3ad911d0ef7e694cf433f3314e991100c72c7473641a9e3437deeab402c8f4a03fdf9c174cbae00142a28ce43475ca61f0016ede73dc778b5ed5a0344cfc2")]
[assembly: AssemblyCompany("Daniel Peñalba")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("An interpreter for grammar files as defined by TextMate. TextMate grammars use the oniguruma dialect (https://github.com/kkos/oniguruma). Supports loading grammar files only from JSON format. Cross - grammar injections are currently not supported.\n\nTextMateSharp is a port of microsoft/vscode-textmate that brings TextMate grammars to dotnet ecosystem. The implementation is based the Java port eclipse/tm4e.\n\nTextMateSharp uses a wrapper around Oniguruma regex engine. Read below to learn how to build Oniguruma bindings.")]
[assembly: AssemblyFileVersion("1.0.55.0")]
[assembly: AssemblyInformationalVersion("1.0.55+c2d1d7d228a59b30d429b3f288092a3e326fa4c5")]
[assembly: AssemblyProduct("TextMateSharp")]
[assembly: AssemblyTitle("TextMateSharp")]
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/danipen/TextMateSharp")]
[assembly: AssemblyVersion("1.0.55.0")]
[module: System.Runtime.CompilerServices.RefSafetyRules(11)]
