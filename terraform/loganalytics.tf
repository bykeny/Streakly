resource "azurerm_log_analytics_workspace" "streakly" {
  name                = "law-streakly"
  location            = azurerm_resource_group.streakly.location
  resource_group_name = azurerm_resource_group.streakly.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}