using System;
using System.Collections.Generic;
using System.IO;
using MewPipe.DataFeeder.Entities;
using OfficeOpenXml;

namespace MewPipe.DataFeeder.Utils
{
	public static class ExcelManager
	{
		private static bool IsUrlValid(string url)
		{
			Uri uriResult;
			return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
					&& (uriResult.Scheme == Uri.UriSchemeHttp
						|| uriResult.Scheme == Uri.UriSchemeHttps);
		}

		public static List<ExcelVideo> GetVideos(string xlsxPath)
		{
			var videos = new List<ExcelVideo>();

			FileInfo fInfo = new FileInfo(xlsxPath);
			ExcelPackage package = new ExcelPackage(fInfo);
			ExcelWorksheet sheet = package.Workbook.Worksheets[1];

			var row = 0;
			while (true)
			{
				row++;
				var url = sheet.Cells[row, 1].Text;
				var category = sheet.Cells[row, 2].Text;

				if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(category)) break; // End of the videos
				if (!IsUrlValid(url)) continue; // This will skip any extra rows like the header

				videos.Add(new ExcelVideo
				{
					Index = row,
					Url = url,
					Category = category
				});
			}

			return videos;
		}
	}
}