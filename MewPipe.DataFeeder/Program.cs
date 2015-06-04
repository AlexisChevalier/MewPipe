using System;
using MewPipe.DataFeeder.Utils;

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
			else
			{
				var xlsxPath = args[0];
				Console.WriteLine("Getting the videos from the excel file.");
				var excelVideos = ExcelManager.GetVideos(xlsxPath);
				Console.WriteLine("Found {0} videos in the excel file.", excelVideos.Count);

				Console.WriteLine("Starting the downloads (The first initialisation may take few minutes) ...");
				foreach (var excelVideo in excelVideos)
				{
					VideoManager.Download(excelVideo.Url);
				}
			}

			Console.Write("\nPress any key to continue ...");
			Console.ReadKey();
		}

		private static void ShowUsage()
		{
			Console.WriteLine("USAGE: DataFeeder.exe <pathToXlsxFile>");
		}
	}
}