using System.IO;

namespace Castle.Core.Tests
{
	public class FindPeVerify
	{
		private static readonly string[] PeVerifyProbingPaths =
		{
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1\Bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bin",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin",
			@"C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v6.0A\Bin",
			@"C:\Program Files (x86)\Microsoft Visual Studio 8\SDK\v2.0\bin"
		};

		private static string peVerifyPath;

		public static string PeVerifyPath
		{
			get { return peVerifyPath ?? (peVerifyPath = FindPeVerifyPath()); }
		}

		private static string FindPeVerifyPath()
		{
			var peVerifyProbingPaths = PeVerifyProbingPaths;
			foreach (var path in peVerifyProbingPaths)
			{
				var file = Path.Combine(path, "peverify.exe");
				if (File.Exists(file))
					return file;
			}
			throw new FileNotFoundException(
				"Please check the PeVerifyProbingPaths configuration setting and set it to the folder where peverify.exe is located");
		}
	}
}