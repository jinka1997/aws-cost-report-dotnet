resource "aws_lambda_function" "cost_report" {
  architectures = [
    "x86_64",
  ]
  function_name = "cost-report-f"
  image_uri     = "${aws_ecr_repository.repository.repository_url}:latest"
  memory_size   = 512
  package_type  = "Image"
  role          = aws_iam_role.cost_report_lambda.arn
  tags          = {}
  timeout       = 30

  environment {
    variables = {
      "DailySummaryByResourceSpan"        = "10"
      "JpyUsdRate"                        = "160"
      "SlackWebhookUrl_ParameterStoreKey" = aws_ssm_parameter.slack_url.name
    }
  }

  ephemeral_storage {
    size = 512
  }

  image_config {
    command = [
      "AWSCostReport::AWSCostReport.Function::FunctionHandler",
    ]
  }

  logging_config {
    log_format = "Text"
    log_group  = "/aws/lambda/cost-report-f"
  }

  tracing_config {
    mode = "PassThrough"
  }
}


resource "aws_ecr_repository" "repository" {
  image_tag_mutability = "MUTABLE"
  name                 = "cost-report"
  tags                 = {}

  encryption_configuration {
    encryption_type = "AES256"
  }

  image_scanning_configuration {
    scan_on_push = false
  }
}

resource "aws_ssm_parameter" "slack_url" {
  data_type = "text"
  name      = "/SlackWebhookUrl/CostReport"
  tags      = {}
  tier      = "Standard"
  type      = "String"
  value     = var.slack_incoming_webhook_url
}

resource "aws_iam_role" "cost_report_lambda" {
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
  name = "cost-report-role"
  path = "/service-role/"
  tags = {}
}



resource "aws_iam_role_policy_attachments_exclusive" "lambda_basic" {
  role_name = aws_iam_role.cost_report_lambda.name
  policy_arns = [
    "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  ]
}


resource "aws_iam_role_policy" "inline_1" {
  name = "inline-1"
  policy = jsonencode(
    {
      Statement = [
        {
          Action = [
            "ce:GetCostAndUsage",
          ]
          Effect   = "Allow"
          Resource = "*"
          Sid      = "VisualEditor0"
        },
        {
          Action = [
            "ssm:GetParameter",
          ]
          Effect   = "Allow"
          Resource = aws_ssm_parameter.slack_url.arn
          Sid      = "VisualEditor1"
        },
      ]
      Version = "2012-10-17"
    }
  )
  role = aws_iam_role.cost_report_lambda.name
}

