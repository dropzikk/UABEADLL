using System.Reflection;
using System.Text;
using LibCpp2IL.BinaryStructures;

namespace LibCpp2IL.Reflection;

public class Il2CppParameterReflectionData
{
	public string ParameterName;

	public Il2CppType RawType;

	public Il2CppTypeReflectionData Type;

	public ParameterAttributes ParameterAttributes;

	public object? DefaultValue;

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if ((ParameterAttributes & ParameterAttributes.Out) != 0)
		{
			stringBuilder.Append("out ");
		}
		stringBuilder.Append(Type).Append(" ").Append(ParameterName);
		if ((ParameterAttributes & ParameterAttributes.HasDefault) != 0)
		{
			stringBuilder.Append(" = ").Append(DefaultValue ?? "null");
		}
		return stringBuilder.ToString();
	}
}
