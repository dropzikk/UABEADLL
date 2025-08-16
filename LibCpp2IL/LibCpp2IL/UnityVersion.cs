using System;
using System.IO;

namespace LibCpp2IL;

public readonly struct UnityVersion : IEquatable<UnityVersion>, IComparable, IComparable<UnityVersion>
{
	private readonly ulong m_data;

	public static UnityVersion MinVersion => new UnityVersion(0uL);

	public static UnityVersion MaxVersion => new UnityVersion(ulong.MaxValue);

	public static UnityVersion DefaultVersion => new UnityVersion(2017, 3, 0, UnityVersionType.Final, 3);

	public int Major => (int)((m_data >> 48) & 0xFFFF);

	public int Minor => (int)((m_data >> 40) & 0xFF);

	public int Build => (int)((m_data >> 32) & 0xFF);

	public UnityVersionType Type => (UnityVersionType)((m_data >> 24) & 0xFF);

	public int TypeNumber => (int)((m_data >> 16) & 0xFF);

	public UnityVersion(int major)
	{
		m_data = (ulong)((long)(major & 0xFFFF) << 48);
	}

	public UnityVersion(int major, int minor)
	{
		m_data = (ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40));
	}

	public UnityVersion(int major, int minor, int build)
	{
		m_data = (ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32));
	}

	public UnityVersion(int major, int minor, int build, UnityVersionType type)
	{
		m_data = (ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32) | ((long)(type & (UnityVersionType)255) << 24));
	}

	public UnityVersion(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		m_data = (ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32) | ((long)(type & (UnityVersionType)255) << 24) | ((long)(typeNumber & 0xFF) << 16));
	}

	private UnityVersion(ulong data)
	{
		m_data = data;
	}

	public int[] ToArray()
	{
		return new int[3] { Major, Minor, Build };
	}

	public override string ToString()
	{
		return $"{Major}.{Minor}.{Build}{Type.ToLiteral()}{TypeNumber}";
	}

	public static UnityVersion Max(UnityVersion left, UnityVersion right)
	{
		if (left > right)
		{
			return left;
		}
		return right;
	}

	public static UnityVersion Min(UnityVersion left, UnityVersion right)
	{
		if (left < right)
		{
			return left;
		}
		return right;
	}

	public static UnityVersion Parse(string version)
	{
		if (string.IsNullOrEmpty(version))
		{
			throw new Exception("Invalid version number " + version);
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		UnityVersionType type = UnityVersionType.Final;
		int num4 = 0;
		using (StringReader stringReader = new StringReader(version))
		{
			while (true)
			{
				int num5 = stringReader.Read();
				if (num5 == -1)
				{
					throw new Exception("Invalid version format");
				}
				char c = (char)num5;
				if (c == '.')
				{
					break;
				}
				num = num * 10 + c.ParseDigit();
			}
			while (true)
			{
				int num6 = stringReader.Read();
				if (num6 == -1)
				{
					break;
				}
				char c2 = (char)num6;
				if (c2 == '.')
				{
					break;
				}
				num2 = num2 * 10 + c2.ParseDigit();
			}
			while (true)
			{
				int num7 = stringReader.Read();
				if (num7 == -1)
				{
					break;
				}
				char c3 = (char)num7;
				if (char.IsDigit(c3))
				{
					num3 = num3 * 10 + c3.ParseDigit();
					continue;
				}
				type = c3 switch
				{
					'a' => UnityVersionType.Alpha, 
					'b' => UnityVersionType.Beta, 
					'p' => UnityVersionType.Patch, 
					'f' => UnityVersionType.Final, 
					_ => throw new Exception($"Unsupported version type {c3} for version '{version}'"), 
				};
				break;
			}
			while (true)
			{
				int num8 = stringReader.Read();
				if (num8 == -1)
				{
					break;
				}
				char c4 = (char)num8;
				num4 = num4 * 10 + c4.ParseDigit();
			}
		}
		return new UnityVersion(num, num2, num3, type, num4);
	}

	public static bool operator ==(UnityVersion left, UnityVersion right)
	{
		return left.m_data == right.m_data;
	}

	public static bool operator !=(UnityVersion left, UnityVersion right)
	{
		return left.m_data != right.m_data;
	}

	public static bool operator >(UnityVersion left, UnityVersion right)
	{
		return left.m_data > right.m_data;
	}

	public static bool operator >=(UnityVersion left, UnityVersion right)
	{
		return left.m_data >= right.m_data;
	}

	public static bool operator <(UnityVersion left, UnityVersion right)
	{
		return left.m_data < right.m_data;
	}

	public static bool operator <=(UnityVersion left, UnityVersion right)
	{
		return left.m_data <= right.m_data;
	}

	public int CompareTo(object obj)
	{
		if (obj is UnityVersion other)
		{
			return CompareTo(other);
		}
		return 1;
	}

	public int CompareTo(UnityVersion other)
	{
		if (this > other)
		{
			return 1;
		}
		if (this < other)
		{
			return -1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (obj is UnityVersion unityVersion)
		{
			return this == unityVersion;
		}
		return false;
	}

	public bool Equals(UnityVersion other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		ulong data = m_data;
		return 827 + 911 * data.GetHashCode();
	}

	public bool IsEqual(int major)
	{
		return this == From(major);
	}

	public bool IsEqual(int major, int minor)
	{
		return this == From(major, minor);
	}

	public bool IsEqual(int major, int minor, int build)
	{
		return this == From(major, minor, build);
	}

	public bool IsEqual(int major, int minor, int build, UnityVersionType type)
	{
		return this == From(major, minor, build, type);
	}

	public bool IsEqual(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return this == new UnityVersion(major, minor, build, type, typeNumber);
	}

	public bool IsEqual(string version)
	{
		return this == Parse(version);
	}

	public bool IsLess(int major)
	{
		return this < From(major);
	}

	public bool IsLess(int major, int minor)
	{
		return this < From(major, minor);
	}

	public bool IsLess(int major, int minor, int build)
	{
		return this < From(major, minor, build);
	}

	public bool IsLess(int major, int minor, int build, UnityVersionType type)
	{
		return this < From(major, minor, build, type);
	}

	public bool IsLess(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return this < new UnityVersion(major, minor, build, type, typeNumber);
	}

	public bool IsLess(string version)
	{
		return this < Parse(version);
	}

	public bool IsLessEqual(int major)
	{
		return this <= From(major);
	}

	public bool IsLessEqual(int major, int minor)
	{
		return this <= From(major, minor);
	}

	public bool IsLessEqual(int major, int minor, int build)
	{
		return this <= From(major, minor, build);
	}

	public bool IsLessEqual(int major, int minor, int build, UnityVersionType type)
	{
		return this <= From(major, minor, build, type);
	}

	public bool IsLessEqual(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return this <= new UnityVersion(major, minor, build, type, typeNumber);
	}

	public bool IsLessEqual(string version)
	{
		return this <= Parse(version);
	}

	public bool IsGreater(int major)
	{
		return this > From(major);
	}

	public bool IsGreater(int major, int minor)
	{
		return this > From(major, minor);
	}

	public bool IsGreater(int major, int minor, int build)
	{
		return this > From(major, minor, build);
	}

	public bool IsGreater(int major, int minor, int build, UnityVersionType type)
	{
		return this > From(major, minor, build, type);
	}

	public bool IsGreater(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return this > new UnityVersion(major, minor, build, type, typeNumber);
	}

	public bool IsGreater(string version)
	{
		return this > Parse(version);
	}

	public bool IsGreaterEqual(int major)
	{
		return this >= From(major);
	}

	public bool IsGreaterEqual(int major, int minor)
	{
		return this >= From(major, minor);
	}

	public bool IsGreaterEqual(int major, int minor, int build)
	{
		return this >= From(major, minor, build);
	}

	public bool IsGreaterEqual(int major, int minor, int build, UnityVersionType type)
	{
		return this >= From(major, minor, build, type);
	}

	public bool IsGreaterEqual(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return this >= new UnityVersion(major, minor, build, type, typeNumber);
	}

	public bool IsGreaterEqual(string version)
	{
		return this >= Parse(version);
	}

	private UnityVersion From(int major)
	{
		return new UnityVersion((ulong)((long)(major & 0xFFFF) << 48) | (0xFFFFFFFFFFFFL & m_data));
	}

	private UnityVersion From(int major, int minor)
	{
		return new UnityVersion((ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40)) | (0xFFFFFFFFFFL & m_data));
	}

	private UnityVersion From(int major, int minor, int build)
	{
		return new UnityVersion((ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32)) | (0xFFFFFFFFu & m_data));
	}

	private UnityVersion From(int major, int minor, int build, UnityVersionType type)
	{
		return new UnityVersion((ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32) | ((long)(type & (UnityVersionType)255) << 24)) | (0xFFFFFF & m_data));
	}

	private UnityVersion From(int major, int minor, int build, UnityVersionType type, int typeNumber)
	{
		return new UnityVersion((ulong)(((long)(major & 0xFFFF) << 48) | ((long)(minor & 0xFF) << 40) | ((long)(build & 0xFF) << 32) | ((long)(type & (UnityVersionType)255) << 24) | ((long)(typeNumber & 0xFF) << 16)) | (0xFFFF & m_data));
	}
}
