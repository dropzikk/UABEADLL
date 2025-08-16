using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution.Parameters;

internal static class BokehBlurKernelDataProvider
{
	private static readonly ConcurrentDictionary<BokehBlurParameters, BokehBlurKernelData> Cache = new ConcurrentDictionary<BokehBlurParameters, BokehBlurKernelData>();

	private static IReadOnlyList<float> KernelScales { get; } = new float[6] { 1.4f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f };

	private static IReadOnlyList<Vector4[]> KernelComponents { get; } = new Vector4[6][]
	{
		new Vector4[1]
		{
			new Vector4(0.862325f, 1.624835f, 0.767583f, 1.862321f)
		},
		new Vector4[2]
		{
			new Vector4(0.886528f, 5.268909f, 0.411259f, -0.548794f),
			new Vector4(1.960518f, 1.558213f, 0.513282f, 4.56111f)
		},
		new Vector4[3]
		{
			new Vector4(2.17649f, 5.043495f, 1.621035f, -2.105439f),
			new Vector4(1.019306f, 9.027613f, -0.28086f, -0.162882f),
			new Vector4(2.81511f, 1.597273f, -0.366471f, 10.300301f)
		},
		new Vector4[4]
		{
			new Vector4(4.338459f, 1.553635f, -5.767909f, 46.1644f),
			new Vector4(3.839993f, 4.693183f, 9.795391f, -15.227561f),
			new Vector4(2.79188f, 8.178137f, -3.048324f, 0.302959f),
			new Vector4(1.34219f, 12.328289f, 0.010001f, 0.24465f)
		},
		new Vector4[5]
		{
			new Vector4(4.892608f, 1.685979f, -22.356787f, 85.91246f),
			new Vector4(4.71187f, 4.998496f, 35.918938f, -28.875618f),
			new Vector4(4.052795f, 8.244168f, -13.212253f, -1.578428f),
			new Vector4(2.929212f, 11.900859f, 0.507991f, 1.816328f),
			new Vector4(1.512961f, 16.116383f, 0.138051f, -0.01f)
		},
		new Vector4[6]
		{
			new Vector4(5.143778f, 2.079813f, -82.3266f, 111.231026f),
			new Vector4(5.612426f, 6.153387f, 113.87866f, 58.00488f),
			new Vector4(5.982921f, 9.802895f, 39.479084f, -162.02888f),
			new Vector4(6.505167f, 11.059237f, -71.286026f, 95.02707f),
			new Vector4(3.869579f, 14.81052f, 1.405746f, -3.704914f),
			new Vector4(2.201904f, 19.03291f, -0.152784f, -0.107988f)
		}
	};

	public static BokehBlurKernelData GetBokehBlurKernelData(int radius, int kernelSize, int componentsCount)
	{
		BokehBlurParameters key = new BokehBlurParameters(radius, componentsCount);
		if (!Cache.TryGetValue(key, out var value))
		{
			(Vector4[] Parameters, float Scale) parameters = GetParameters(componentsCount);
			Vector4[] item = parameters.Parameters;
			float item2 = parameters.Scale;
			Complex64[][] kernels = CreateComplexKernels(item, radius, kernelSize, item2);
			NormalizeKernels(kernels, item);
			value = new BokehBlurKernelData(item, kernels);
			Cache.TryAdd(key, value);
		}
		return value;
	}

	private static (Vector4[] Parameters, float Scale) GetParameters(int componentsCount)
	{
		int index = Math.Max(0, Math.Min(componentsCount - 1, KernelComponents.Count));
		return (Parameters: KernelComponents[index], Scale: KernelScales[index]);
	}

	private static Complex64[][] CreateComplexKernels(Vector4[] kernelParameters, int radius, int kernelSize, float kernelsScale)
	{
		Complex64[][] array = new Complex64[kernelParameters.Length][];
		ref Vector4 reference = ref MemoryMarshal.GetReference(kernelParameters.AsSpan());
		for (int i = 0; i < kernelParameters.Length; i++)
		{
			ref Vector4 reference2 = ref Unsafe.Add(ref reference, (uint)i);
			array[i] = CreateComplex1DKernel(radius, kernelSize, kernelsScale, reference2.X, reference2.Y);
		}
		return array;
	}

	private static Complex64[] CreateComplex1DKernel(int radius, int kernelSize, float kernelsScale, float a, float b)
	{
		Complex64[] array = new Complex64[kernelSize];
		ref Complex64 reference = ref MemoryMarshal.GetReference(array.AsSpan());
		int num = -radius;
		int num2 = 0;
		while (num2 < kernelSize)
		{
			float num3 = (float)num * kernelsScale * (1f / (float)radius);
			num3 *= num3;
			Unsafe.Add(ref reference, (uint)num2) = new Complex64(MathF.Exp((0f - a) * num3) * MathF.Cos(b * num3), MathF.Exp((0f - a) * num3) * MathF.Sin(b * num3));
			num2++;
			num++;
		}
		return array;
	}

	private static void NormalizeKernels(Complex64[][] kernels, Vector4[] kernelParameters)
	{
		float num = 0f;
		Span<Complex64[]> span = kernels.AsSpan();
		ref Complex64[] reference = ref MemoryMarshal.GetReference(span);
		ref Vector4 reference2 = ref MemoryMarshal.GetReference(kernelParameters.AsSpan());
		for (int i = 0; i < kernelParameters.Length; i++)
		{
			ref Complex64[] reference3 = ref Unsafe.Add(ref reference, (uint)i);
			int num2 = reference3.Length;
			ref Complex64 arrayDataReference = ref MemoryMarshal.GetArrayDataReference(reference3);
			ref Vector4 reference4 = ref Unsafe.Add(ref reference2, (uint)i);
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					ref Complex64 reference5 = ref Unsafe.Add(ref arrayDataReference, (uint)j);
					ref Complex64 reference6 = ref Unsafe.Add(ref arrayDataReference, (uint)k);
					num += reference4.Z * (reference5.Real * reference6.Real - reference5.Imaginary * reference6.Imaginary) + reference4.W * (reference5.Real * reference6.Imaginary + reference5.Imaginary * reference6.Real);
				}
			}
		}
		float num3 = 1f / MathF.Sqrt(num);
		for (int l = 0; l < span.Length; l++)
		{
			ref Complex64[] reference7 = ref Unsafe.Add(ref reference, (uint)l);
			int num4 = reference7.Length;
			ref Complex64 arrayDataReference2 = ref MemoryMarshal.GetArrayDataReference(reference7);
			for (int m = 0; m < num4; m++)
			{
				Unsafe.Add(ref arrayDataReference2, (uint)m) *= num3;
			}
		}
	}
}
