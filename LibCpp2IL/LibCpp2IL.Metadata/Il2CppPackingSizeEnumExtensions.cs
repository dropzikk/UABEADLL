using System;

namespace LibCpp2IL.Metadata;

public static class Il2CppPackingSizeEnumExtensions
{
	public static uint NumericalValue(this Il2CppPackingSizeEnum size)
	{
		return size switch
		{
			Il2CppPackingSizeEnum.Zero => 0u, 
			Il2CppPackingSizeEnum.One => 1u, 
			Il2CppPackingSizeEnum.Two => 2u, 
			Il2CppPackingSizeEnum.Four => 4u, 
			Il2CppPackingSizeEnum.Eight => 8u, 
			Il2CppPackingSizeEnum.Sixteen => 16u, 
			Il2CppPackingSizeEnum.ThirtyTwo => 32u, 
			Il2CppPackingSizeEnum.SixtyFour => 64u, 
			Il2CppPackingSizeEnum.OneHundredTwentyEight => 128u, 
			_ => throw new ArgumentOutOfRangeException("size", size, null), 
		};
	}
}
