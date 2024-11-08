# % terraform import aws_lambda_function.cost_report_notify_dotnet cost-report-f

resource "aws_lambda_function" "cost_report_notify_dotnet" {
  architectures = [
    "x86_64",
  ]
  #arn                            = "arn:aws:lambda:ap-northeast-1:381492137918:function:cost-report-f"
  #code_sha256                    = "9feb586fcae1a9a8cb931d4385cd06ccf23f3cbf8e5afe45c5d072339aa588ff"
  function_name = "cost-report-f"
  #id                             = "cost-report-f"
  image_uri = "381492137918.dkr.ecr.ap-northeast-1.amazonaws.com/cost-report:latest"
  #invoke_arn = "arn:aws:apigateway:ap-northeast-1:lambda:path/2015-03-31/functions/arn:aws:lambda:ap-northeast-1:381492137918:function:cost-report-f/invocations"
  #last_modified                  = "2024-11-06T08:13:27.000+0000"
  layers       = []
  memory_size  = 512
  package_type = "Image"
  #qualified_arn                  = "arn:aws:lambda:ap-northeast-1:381492137918:function:cost-report-f:$LATEST"
  #qualified_invoke_arn           = "arn:aws:apigateway:ap-northeast-1:lambda:path/2015-03-31/functions/arn:aws:lambda:ap-northeast-1:381492137918:function:cost-report-f:$LATEST/invocations"
  #reserved_concurrent_executions = -1
  role = "arn:aws:iam::381492137918:role/service-role/cost-report-role"
  #skip_destroy                   = false
  #source_code_size               = 0
  tags = {}
  #tags_all                       = {}
  timeout = 30
  #version                        = "$LATEST"

  environment {
    variables = {
      "DailySummaryByResourceSpan" = "10"
      "SlackWebhookUrl"            = "https://hooks.slack.com/services/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"
    }
  }

  ephemeral_storage {
    size = 512
  }

  image_config {
    command = [
      "AWSCostReport::AWSCostReport.Function::FunctionHandler",
    ]
    entry_point = []
  }

  logging_config {
    log_format = "Text"
    log_group  = "/aws/lambda/cost-report-f"
  }

  tracing_config {
    mode = "PassThrough"
  }
}

# terraform import aws_iam_role.cost_report_notify_dotnet_role  arn:aws:iam::381492137918:role/service-role/cost-report-role
# terraform import aws_iam_role.cost_report_notify_dotnet_role  cost-report-role
resource "aws_iam_role" "cost_report_notify_dotnet_role" {
  #arn = "arn:aws:iam::381492137918:role/service-role/cost-report-role"
  assume_role_policy = jsonencode(
    {
      Statement = [
        {
          Action = "sts:AssumeRole"
          Effect = "Allow"
          Principal = {
            Service = "lambda.amazonaws.com"
          }
        },
      ]
      Version = "2012-10-17"
    }
  )
  #create_date           = "2024-11-06T08:09:56Z"
  #force_detach_policies = false
  #id                    = "cost-report-role"
  managed_policy_arns = [
    "arn:aws:iam::381492137918:policy/service-role/AWSLambdaBasicExecutionRole-b268d950-3ab8-41b3-ad0b-115481e2d094",
  ]
  max_session_duration = 3600
  name                 = "cost-report-role"
  path                 = "/service-role/"
  tags                 = {}
  #tags_all             = {}
  #unique_id            = "AROAVRUVUB67LP7JYX44D"

  inline_policy {
    name = "inline-1"
    policy = jsonencode(
      {
        Statement = [
          {
            Action   = "ce:GetCostAndUsage"
            Effect   = "Allow"
            Resource = "*"
            Sid      = "VisualEditor0"
          },
        ]
        Version = "2012-10-17"
      }
    )
  }
}

