using System;
using System.Collections.Generic;

namespace MewPipe.DataFeeder.Entities
{
	public class ExcelImpressionsReport
	{
		public string Name { get; set; }
		public List<string> Headers { get; set; }
		public List<ExcelImpressionsReportRow> Rows { get; set; }

		public ExcelImpressionsReport(IEnumerable<ExcelUser> users)
		{
			var dateStr = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
			Name = string.Format("ImpressionReport_{0}", dateStr);
			Headers = new List<string> {"VideoId", "VideoCategory"};

			foreach (var excelUser in users)
			{
				Headers.Add(excelUser.UserName);
			}

			Rows = new List<ExcelImpressionsReportRow>();
		}

		/*
		public void AddRow(string videoId, string videoCategory, string userNick)
		{
			Rows.Add(new ExcelImpressionsReportRow
			{
				VideoId = videoId,
				VideoCategory = videoCategory
			});
		}
		*/

		public void AddRow(string videoId, string videoCategory, Dictionary<string, int> rateByUsers)
		{
			Rows.Add(new ExcelImpressionsReportRow
			{
				VideoId = videoId,
				VideoCategory = videoCategory,
				RateByUsers = rateByUsers
			});
		}
	}

	public class ExcelImpressionsReportRow
	{
		public string VideoId { get; set; }
		public string VideoCategory { get; set; }
		public Dictionary<string, int> RateByUsers { get; set; }
	}
}