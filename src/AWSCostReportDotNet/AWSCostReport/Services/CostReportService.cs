using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using AWSCostReport.Models;

namespace AWSCostReport.Services
{
	public class CostReportService
	{
		public static async ValueTask<GetCostAndUsageResponse> GetByDateAsync(DateTime start, DateTime end)
		{
			var client = new AmazonCostExplorerClient();
			var request = new GetCostAndUsageRequest
			{
				TimePeriod = new Amazon.CostExplorer.Model.DateInterval()
				{
					Start = $"{start:yyyy-MM-dd}",
					End = $"{end:yyyy-MM-dd}"
				},
				Granularity = Granularity.DAILY
			};
			request.Metrics.Add("BlendedCost");
			request.GroupBy =
			[
				new GroupDefinition() { Key = "SERVICE", Type = GroupDefinitionType.DIMENSION }
			];
			return await client.GetCostAndUsageAsync(request);

		}
		public static IEnumerable<CostDetail> ConvertToCostDetail(GetCostAndUsageResponse response)
		{
			foreach (var resultByTime in response.ResultsByTime)
			{
				foreach (var group in resultByTime.Groups)
				{
					var serviceName = group.Keys.First();
					var metricValue = group.Metrics.First().Value;
					yield return new CostDetail()
					{
						Amount = decimal.Parse(metricValue.Amount),
						Currency = metricValue.Unit,
						ServiceName = serviceName,
						UsageDate = DateTime.ParseExact(resultByTime.TimePeriod.Start, "yyyy-MM-dd", null)
					};
				}
			}
		}
	}
}
