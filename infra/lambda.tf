resource "aws_lambda_function" "cost_report_notify_dotnet" {
  function_name                  = "cost-report-notify-dotnet" 
  handler                        = "AWSCostReportLambda::AWSCostReportLambda.Function::FunctionHandler" 
  layers                         = [] 
  memory_size                    = 512 
  package_type                   = "Zip" 
  filename                       = "AWSCostReportLambda.zip"
  reserved_concurrent_executions = -1 
  role                           = aws_iam_role.cost_report_notify_dotnet_role.arn
  runtime                        = "dotnetcore3.1" 
  tags                           = {

  } 
  timeout                        = 15 
  environment {
    variables = {
      "SlackIncomingWebhookUrl" = var.slack_incoming_webhook_url
    } 
  }
  timeouts {}
  tracing_config {
    mode = "PassThrough" 
  }
}

resource "aws_iam_role" "cost_report_notify_dotnet_role" {
  assume_role_policy    = jsonencode(
    {
      Statement = [
        {
          Action    = "sts:AssumeRole"
          Effect    = "Allow"
          Principal = {
            Service = "lambda.amazonaws.com"
          }
        },
      ]
      Version   = "2012-10-17"
    }
  ) 
  managed_policy_arns   = [
    aws_iam_policy.cost_report_notify_dotnet_policy.arn,
  ]
  max_session_duration  = 3600 
  name                  = "cost-report-notify-role" 
  path                  = "/service-role/"
}

resource "aws_iam_policy" "cost_report_notify_dotnet_policy" {
  name   = "AWSLambdaBasicExecutionRole-for-cost-report-notify"
  path   = "/service-role/"
  policy = jsonencode(
      {
        Statement = [
          {
            Action   = "ce:GetCostAndUsage"
            Effect   = "Allow"
            Resource = "*"
          },
          {
            Action   = [
              "logs:CreateLogStream",
              "logs:PutLogEvents",
            ]
            Effect   = "Allow"
            Resource = format("%s:*",aws_cloudwatch_log_group.cost_report_notify_dotnet.arn)
          },
        ]
        Version   = "2012-10-17"
      }
  ) 
}

resource "aws_cloudwatch_log_group" "cost_report_notify_dotnet" {
  name              = "/aws/lambda/cost-report-notify-dotnet"
  retention_in_days = 0 
  tags              = {

  } 
}

