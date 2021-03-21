resource "aws_cloudwatch_event_rule" "cost_report_notify_dotnet" {
  description         = "コストレポート通知のdotnet版" 
  event_bus_name      = "default" 
  is_enabled          = true 
  name                = "exec_cost_report_notify_dotnet" 
  schedule_expression = "cron(0 11 * * ? *)" # 日本時間の20時
  tags                = {

  } 
}

resource "aws_cloudwatch_event_target" "cost_report_notify_dotnet" {
  arn            = aws_lambda_function.cost_report_notify_dotnet.arn
  event_bus_name = "default" 
  rule           = "exec_cost_report_notify_dotnet" 
  depends_on = [
    aws_cloudwatch_event_rule.cost_report_notify_dotnet
  ]
}