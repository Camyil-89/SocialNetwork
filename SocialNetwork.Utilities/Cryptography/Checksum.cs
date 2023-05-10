using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Utilities.Cryptography
{
	public static class Checksum
	{
		public static string Compute(string data)
		{
			if (string.IsNullOrEmpty(data))
				return "ErrorHASH";
			return Compute(Encoding.UTF8.GetBytes(data));
		}
		public static string Compute(byte[] data)
		{
			return BitConverter.ToString(SHA256.HashData(data)).Replace("-","").ToLowerInvariant();
		}
	}
}
