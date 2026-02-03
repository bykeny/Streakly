output "container_app_url" {
  description = "Public URL of the Streakly Container App"
  value       = azurerm_container_app.streakly.latest_revision_fqdn
}

output "app_insights_url" {
  value = azurerm_application_insights.streakly.app_id
}