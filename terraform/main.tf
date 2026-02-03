resource "azurerm_resource_group" "streakly" {
  name     = var.resource_group_name
  location = var.location
}