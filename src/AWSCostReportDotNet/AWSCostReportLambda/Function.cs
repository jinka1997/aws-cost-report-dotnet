using Amazon.Lambda.Core;
using AWSCostReportLambda.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSCostReportLambda
{
	public class Function
	{

		/// <summary>
		/// A simple function that takes a string and does a ToUpper
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> FunctionHandler(ILambdaContext context)
		{
			//var today = DateTime.Today;
			//var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
			//var service = new CostReportService();
			//var response = await service.GetByDate(firstDayOfMonth, today);
			//var details = service.EditDetailsByResponse(response);
			//var messages = service.EditMessage(details);

			//var webhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");
			//await service.NotifyAsync(webhookUrl, messages);

			//return messages;



			context.Logger.LogInformation($"start");
			var today = DateTime.Today;
			var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
			var dailySummaryByResourceSpan = int.Parse(Environment.GetEnvironmentVariable("DailySummaryByResourceSpan"));

			var webhookUrl = Environment.GetEnvironmentVariable("SlackWebhookUrl");
			var response = await CostReportService.GetByDate(firstDayOfMonth, today);

			//List<CostReportDetail> costDetails;
			try
			{

				//costDetails = (await CostReportService.GetAsync(subscriptionId, accessToken)).ToList();
			}
			catch (Exception ex)
			{
				context.Logger.LogError($"{ex.Message}");
				context.Logger.LogError($"{ex.StackTrace}");
				return new string[] { };
			}
			//await NotifyService.NotifyDailySummaryAsync(webhookUrl, costDetails);
			//await NotifyService.NotifyDailySummaryByResource(webhookUrl, costDetails, dailySummaryByResourceSpan);

			context.Logger.LogInformation($"end");
			return new string[] { };
		}
	}
}
