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
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var service = new CostReportService();
            var response = await service.GetByDate(firstDayOfMonth, today);
            var details = service.EditDetailsByResponse(response);
            var messages = service.EditMessage(details);

            var webhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");
            await service.NotifyAsync(webhookUrl, messages);

            return messages;
        }
    }
}
