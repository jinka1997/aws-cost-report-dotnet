namespace AWSCostReport.Models
{
	public class CostDetail
	{
		public decimal Amount { get; set; }

		public DateTime UsageDate { get; set; }
		public string ServiceName { get; set; } = "";
		public string Currency { get; set; } = "";

	}
}
