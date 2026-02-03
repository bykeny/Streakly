resource "azurerm_container_app_environment" "streakly" {
  name                       = var.environment_name
  location                   = azurerm_resource_group.streakly.location
  resource_group_name        = azurerm_resource_group.streakly.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.streakly.id
}