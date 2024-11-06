using Amazon.Lambda.Core;
using AWSCostReport.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSCostReport;

public class Function
{

	/// <summary>
	/// A simple function that takes a string and returns both the upper and lower case version of the string.
	/// </summary>
	/// <param name="input">The event for the Lambda function handler to process.</param>
	/// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
	/// <returns></returns>
	public async ValueTask<string> FunctionHandler(string input, ILambdaContext context)
	{
		//var dailySummaryByResourceSpan = int.Parse(Environment.GetEnvironmentVariable("DailySummaryByResourceSpan")!);
		//var webhookUrl = Environment.GetEnvironmentVariable("SlackWebhookUrl")!;

		var dailySummaryByResourceSpan = 10;
		var webhookUrl = "https://hooks.slack.com/services/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";

		var today = DateTime.Today;
		var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

		var response = await CostReportService.GetByDateAsync(firstDayOfMonth, today);
		var costDetails = CostReportService.ConvertToCostDetail(response);
		await NotifyService.NotifyDailySummaryAsync(webhookUrl, costDetails);
		await NotifyService.NotifyDailySummaryByResource(webhookUrl, costDetails, dailySummaryByResourceSpan);

		return "OK";
	}
}