using AWSCostReport.Models;

namespace AWSCostReport.Services
{
	public class NotifyService
	{
		public static async Task NotifyDailySummaryAsync(string webhookUrl, IEnumerable<CostDetail> costDetails, decimal jpyUsdRate)
		{
			var postMessage = new List<string>
			{
				$"Monthly Total(USD): `{costDetails.Sum(x => x.Amount):#,##0.000}`",
				$"Monthly Total(JPY(1USD={jpyUsdRate}円)): `{costDetails.Sum(x => x.Amount * jpyUsdRate):#,##0.000}`",
				"```",
			};

			//日毎のPreTaxCostを合計し、日付昇順に並べる
			var dailySummary = costDetails.GroupBy(x => x.UsageDate)
										  .Select(x => new
										  {
											  UsageDate = x.Key,
											  Summary = x.Sum(x => x.Amount)
										  })
										  .OrderBy(x => x.UsageDate);
			postMessage.AddRange(dailySummary.Select(x => $"{x.UsageDate:M/dd(ddd)}: {x.Summary:#,##0.000}"));
			postMessage.Add("```");

			await SlackService.PostMessageAsync(webhookUrl, postMessage);
		}

		public static async Task NotifyDailySummaryByResource(string webhookUrl, IEnumerable<CostDetail> message, int dailySummaryByResourceSpan)
		{
			var filterdDetails = message.Where(x => x.UsageDate >= DateTime.Today.AddDays(-1 * dailySummaryByResourceSpan));
			var dateList = filterdDetails.Select(x => x.UsageDate)
										 .Distinct()
										 .OrderBy(x => x)
										 .ToList();

			var postMessage = new List<string>
			{
				$"直近{dailySummaryByResourceSpan}日間の日毎の明細(USD)",
				"```",
			};

			foreach (var date in dateList)
			{
				//消費した金額の昇順
				var details = filterdDetails.Where(x => x.UsageDate == date)
											.OrderByDescending(x => x.Amount);

				var text = $"{date:M/dd(ddd)}:{details.Sum(x => x.Amount):#,##0.000}({string.Join(",", details.Select(x => $"{x.ServiceName}={x.Amount:#,##0.000}"))} )";
				postMessage.Add(text + "\r\n");
			}
			postMessage.Add("```");

			await SlackService.PostMessageAsync(webhookUrl, postMessage);
		}

	}
}
