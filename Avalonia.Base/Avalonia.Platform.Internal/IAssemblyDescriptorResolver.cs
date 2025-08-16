namespace Avalonia.Platform.Internal;

internal interface IAssemblyDescriptorResolver
{
	IAssemblyDescriptor GetAssembly(string name);
}
