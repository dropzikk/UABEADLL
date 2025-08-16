using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Metsys.Bson;

internal static class ObjectIdGenerator
{
	private static readonly object _inclock = new object();

	private static int _counter;

	private static readonly byte[] _machineHash = GenerateHostHash();

	private static readonly byte[] _processId = BitConverter.GetBytes(GenerateProcId());

	public static byte[] Generate()
	{
		byte[] array = new byte[12];
		int num = 0;
		Array.Copy(BitConverter.GetBytes(GenerateTime()), 0, array, num, 4);
		num += 4;
		Array.Copy(_machineHash, 0, array, num, 3);
		num += 3;
		Array.Copy(_processId, 0, array, num, 2);
		num += 2;
		Array.Copy(BitConverter.GetBytes(GenerateInc()), 0, array, num, 3);
		return array;
	}

	private static int GenerateTime()
	{
		DateTime dateTime = DateTime.Now.ToUniversalTime();
		return Convert.ToInt32(Math.Floor((new DateTime(Helper.Epoch.Year, Helper.Epoch.Month, Helper.Epoch.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond) - Helper.Epoch).TotalMilliseconds));
	}

	private static int GenerateInc()
	{
		lock (_inclock)
		{
			return _counter++;
		}
	}

	private static byte[] GenerateHostHash()
	{
		using MD5 mD = MD5.Create();
		string hostName = Dns.GetHostName();
		return mD.ComputeHash(Encoding.Default.GetBytes(hostName));
	}

	private static int GenerateProcId()
	{
		return Process.GetCurrentProcess().Id;
	}
}
