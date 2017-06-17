using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Libooru
{
	public static class Tools
	{
		/// <summary>
		/// Compute MD5 hash from a given file.
		/// </summary>
		/// <param name="path">Path of the file to compute.</param>
		/// <returns>Hash in hexadecimal form.</returns>
		public static string GetMd5FromFile(string path)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(path))
				{
					return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", ""); ;
				}
			}
		}
	}
}
