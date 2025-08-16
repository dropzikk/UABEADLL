using System;

namespace LibCpp2IL;

public static class UnityVersionTypeExtentions
{
	public static string ToLiteral(this UnityVersionType _this)
	{
		return _this switch
		{
			UnityVersionType.Alpha => "a", 
			UnityVersionType.Beta => "b", 
			UnityVersionType.Final => "f", 
			UnityVersionType.Patch => "p", 
			_ => throw new Exception($"Unsupported vertion type {_this}"), 
		};
	}
}
