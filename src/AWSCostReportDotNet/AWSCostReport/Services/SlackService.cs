using Slack.Webhooks;

namespace AWSCostReport.Services
{
	public class SlackService
	{
		public static async Task PostMessageAsync(string webhookUrl, IEnumerable<string> postMessage)
		{
			var client = new SlackClient(webhookUrl);

			var slackMessage = new SlackMessage()
			{
				Text = string.Join("\r\n", postMessage),
			};
			_ = await client.PostAsync(slackMessage);

		}

	}
}
