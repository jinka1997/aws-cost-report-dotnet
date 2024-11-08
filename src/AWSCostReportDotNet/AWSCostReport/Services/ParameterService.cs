using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AWSCostReport.Services
{
	public class ParameterService
	{
		public static async ValueTask<string> GetParameterStoreValue(string parameterStoreKey)
		{
			var client = new AmazonSimpleSystemsManagementClient();
			var parameter = await client.GetParameterAsync(new GetParameterRequest()
			{
				Name = parameterStoreKey,
				WithDecryption = true,
			});

			return parameter.Parameter.Value;
		}
	}
}
