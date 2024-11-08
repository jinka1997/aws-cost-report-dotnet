﻿using System;
using System.Collections.Generic;

namespace AWSCostReportLambda.Models
{
	public class CostReportDetail
	{
		private readonly Dictionary<string, string> _simpleDic;

		public CostReportDetail(string serviceName, decimal amount)
		{
			_simpleDic = new Dictionary<string, string>
			{
				{ "AWS Glue", "Glue" },
				{ "Amazon Relational Database Service", "RDS" },
				{ "Amazon Simple Storage Service", "S3" },
				{ "AmazonCloudWatch", "CloudWatch" },
				{ "AWS Key Management Service", "KMS" },
				{ "Amazon Elastic Compute Cloud - Compute", "EC2" },
				{ "AWS Lambda", "Lambda" },
				{ "AWS Cost Explorer", "CostExplorer" },
				{ "AWS CloudTrail", "CloudTrail" },
				{ "EC2 - Other", "EC2-Other" },
				{ "AWS Secrets Manager", "ASM" },
				{ "Amazon Elastic Container Service", "ECS" },
				{ "Amazon Elastic Load Balancing", "ELB" },
				{ "Amazon Simple Notification Service", "SNS" },
				{ "Amazon Simple Queue Service", "SQS" },
				{ "Amazon EC2 Container Registry (ECR)", "ECR" },
				{ "Amazon Elastic File System", "EFS" },
				{ "Amazon DynamoDB", "DynamoDB" },
				{ "AWS App Runner", "AppRunner" },
			};

			ServiceName = serviceName;
			Amount = amount;
		}

		public string ServiceName { get; private set; }
		public string ServiceNameSimple
		{
			get
			{
				if (_simpleDic.ContainsKey(ServiceName))
				{
					return _simpleDic[ServiceName];
				}
				return ServiceName;
			}
		}
		public decimal Amount { get; private set; }
	}

	public class CostDetail
	{
		public decimal PreTaxCost { get; set; }
		public string UsageDateString { get; set; }

		public DateTime UsageDate
		{
			get
			{
				var year = int.Parse(UsageDateString[..4]);
				var month = int.Parse(UsageDateString.Substring(4, 2));
				var day = int.Parse(UsageDateString.Substring(6, 2));
				return new DateTime(year, month, day);
			}
		}

		public string ResourceType { get; set; }
		public string Currency { get; set; }
	}
}
