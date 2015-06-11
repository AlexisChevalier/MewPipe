using System;
using System.Collections.Generic;
using System.Configuration;
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
				var author = sheet.Cells[row, 3].Text;

				if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(category)) break; // End of the videos
				if (!IsUrlValid(url)) continue; // This will skip any extra rows like the header

				videos.Add(new ExcelVideo
				{
					Index = row,
					Url = url,
					Category = category,
					Author = author
				});
			}

			return videos;
		}

		public static List<ExcelUser> GetUsers(string xlsxPath)
		{
			var users = new List<ExcelUser>();

			FileInfo fInfo = new FileInfo(xlsxPath);
			ExcelPackage package = new ExcelPackage(fInfo);
			ExcelWorksheet sheet = package.Workbook.Worksheets[1];

			var row = 0;
			while (true)
			{
				row++;
				if (row == 1) continue; // This will skip the header row

				var fullname = sheet.Cells[row, 1].Text;
				var videoGameInterestStr = sheet.Cells[row, 2].Text;
				var sportInterestStr = sheet.Cells[row, 3].Text;
				var musicInterestStr = sheet.Cells[row, 4].Text;

				if (string.IsNullOrEmpty(fullname)
					&& string.IsNullOrEmpty(videoGameInterestStr)
					&& string.IsNullOrEmpty(sportInterestStr)
					&& string.IsNullOrEmpty(musicInterestStr))
					break; // End of the videos

				var videoGameInterest = int.Parse(videoGameInterestStr);
				var sportInterest = int.Parse(sportInterestStr);
				var musicInterest = int.Parse(musicInterestStr);

				users.Add(new ExcelUser
				{
					Index = row,
					FullName = fullname,
					VideoGameInterest = videoGameInterest,
					SportInterest = sportInterest,
					MusicInterest = musicInterest
				});
			}

			return users;
		}

		public static void SaveExcelImpressionsReport(ExcelImpressionsReport report)
		{
			var folder = ConfigurationManager.ConnectionStrings["DataFeederCacheFolder"].ConnectionString;

			var newFile = new FileInfo(string.Format(@"{0}\{1}.xlsx", folder, report.Name));
			using (var pack = new ExcelPackage(newFile))
			{
				var workSheet = pack.Workbook.Worksheets.Add("Impressions");

				// Headers:
				int column = 1;
				foreach (var header in report.Headers)
					workSheet.Cells[1, column++].Value = header;

				// Rows:
				int row = 2; // 1 = headers
				foreach (var excelRow in report.Rows)
				{
					column = 1;
					workSheet.Cells[row, column++].Value = excelRow.VideoId;
					workSheet.Cells[row, column++].Value = excelRow.VideoCategory;

					foreach (var header in report.Headers)
					{
						if (!excelRow.RateByUsers.ContainsKey(header)) continue;

						var excelRate = excelRow.RateByUsers[header];
						workSheet.Cells[row, column++].Value = excelRate;
					}
					row++;
				}
				pack.Save();
			}
		}
	}
}