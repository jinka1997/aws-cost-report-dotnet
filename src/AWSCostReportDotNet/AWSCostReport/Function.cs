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
	public async ValueTask<string> FunctionHandler(ILambdaContext context)
	{
		var dailySummaryByResourceSpan = int.Parse(Environment.GetEnvironmentVariable("DailySummaryByResourceSpan")!);
		var jpyUsdRate = decimal.Parse(Environment.GetEnvironmentVariable("JpyUsdRate")!);
		var webhookUrlParameterStoreKey = Environment.GetEnvironmentVariable("SlackWebhookUrl_ParameterStoreKey")!;


		var today = DateTime.Today;
		var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

		var response = await CostReportService.GetByDateAsync(firstDayOfMonth, today);
		var costDetails = CostReportService.ConvertToCostDetail(response);
		var webhookUrl = await ParameterService.GetParameterStoreValue(webhookUrlParameterStoreKey);
		await NotifyService.NotifyDailySummaryAsync(webhookUrl, costDetails, jpyUsdRate);
		await NotifyService.NotifyDailySummaryByResource(webhookUrl, costDetails, dailySummaryByResourceSpan);

		return "OK";
	}
}