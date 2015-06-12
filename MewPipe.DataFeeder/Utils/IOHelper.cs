using System.IO;

namespace MewPipe.DataFeeder.Utils
{
	public static class IOHelper
	{
		public static void CreateFolder(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
	}
}