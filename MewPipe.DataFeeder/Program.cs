using System;

namespace MewPipe.DataFeeder
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				ShowUsage();
			}
		}

		private static void ShowUsage()
		{
			Console.WriteLine("USAGE: DataFeeder.exe <pathToCsvFile>");
		}
	}
}