resource "azurerm_application_insights" "streakly" {
  name                = var.appinsights_name
  location            = azurerm_resource_group.streakly.location
  resource_group_name = azurerm_resource_group.streakly.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.streakly.id
}