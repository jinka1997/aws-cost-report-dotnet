﻿using Amazon;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using AWSCostReportLambda.Models;
using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSCostReportLambda.Services
{
    public class CostReportService
    {
        public CostReportService()
        {
        }

        public async ValueTask<GetCostAndUsageResponse> GetByDate(DateTime start, DateTime end)
        {
            var client = new AmazonCostExplorerClient(RegionEndpoint.APNortheast1);
            var request = new GetCostAndUsageRequest
            {
                TimePeriod = new DateInterval()
                {
                    Start = $"{start:yyyy-MM-dd}",
                    End = $"{end:yyyy-MM-dd}"
                },
                Granularity = Granularity.DAILY
            };
            request.Metrics.Add("BlendedCost");
            request.GroupBy = new List<GroupDefinition>()
            {
                new GroupDefinition() { Key = "SERVICE", Type = GroupDefinitionType.DIMENSION }
            };
            return  await client.GetCostAndUsageAsync(request);

        }

        public IEnumerable<string> EditMessage(Dictionary<DateTime, IEnumerable<CostReportDetail>> details)
        {
            var messages = details.Select(x =>
            {
                var displayServices = x.Value.Where(x => x.Amount > 0).OrderByDescending(x => x.Amount).Select(x => $"{x.ServiceNameSimple}={x.Amount:0.000}");
                return $"{x.Key:MM/dd(ddd)}=${x.Value.Sum(x => x.Amount):0.000}({string.Join(",", displayServices)})";
            }).ToList();
            messages.Add($"MonthlyTotal=${details.SelectMany(x => x.Value).Sum(x => x.Amount)}");
            return messages;
        }

        public Dictionary<DateTime, IEnumerable<CostReportDetail>> EditDetailsByResponse(GetCostAndUsageResponse response)
        {
            return response.ResultsByTime.ToDictionary(
                byDay => DateTime.ParseExact(byDay.TimePeriod.Start, "yyyy-MM-dd", null),
                byDay => byDay.Groups.Select(x => new CostReportDetail(x.Keys.First(), decimal.Parse(x.Metrics.First().Value.Amount))));
        }
        public async Task NotifyAsync(string webhookUrl, IEnumerable<string> messages)
        {
            var client = new SlackClient(webhookUrl);
            var postMessage = new string[] {
                "ボンビ～",
                "リソース立ち上げてるとお金がかかっちゃうのねん！",
                "社長さんに使用状況を教えてあげるのねん！",
                "",
                "```"
            }.Concat(messages)
            .Concat(new string[] { "```" });
            var slackMessage = new SlackMessage() 
            {  
                Text = string.Join("\r\n", postMessage) ,
            };
            _ = await client.PostAsync(slackMessage);
        }
    }
}