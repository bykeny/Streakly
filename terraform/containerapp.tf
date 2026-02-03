resource "azurerm_container_app" "streakly" {
  name                         = var.containerapp_name
  resource_group_name          = azurerm_resource_group.streakly.name
  container_app_environment_id = azurerm_container_app_environment.streakly.id
  revision_mode                = "Single"

  template {
    container {
      name   = "streakly"
      image  = var.image_name

      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }

      env {
        name        = "EmailSettings__SmtpHost"
        secret_name = "smtp-host"
      }

      env {
        name        = "EmailSettings__SmtpPort"
        secret_name = "smtp-port"
      }

      env {
        name        = "EmailSettings__SmtpUsername"
        secret_name = "smtp-username"
      }

      env {
        name        = "EmailSettings__SmtpPassword"
        secret_name = "smtp-password"
      }

      env {
        name        = "EmailSettings__FromEmail"
        secret_name = "smtp-from-email"
      }

      env {
        name        = "EmailSettings__FromName"
        secret_name = "smtp-from-name"
      }

      env {
        name        = "EmailSettings__UseSsl"
        secret_name = "smtp-use-ssl"
      }
    }

    min_replicas = 0
    max_replicas = 1
  }

  ingress {
    external_enabled = true
    target_port      = 8080

    traffic_weight {
      percentage       = 100
      latest_revision  = true
    }
  }

  secret {
    name  = "smtp-host"
    value = var.smtp_host
  }

  secret {
    name  = "smtp-port"
    value = var.smtp_port
  }

  secret {
    name  = "smtp-username"
    value = var.smtp_username
  }

  secret {
    name  = "smtp-password"
    value = var.smtp_password
  }

  secret {
    name  = "smtp-from-email"
    value = var.smtp_from_email
  }

  secret {
    name  = "smtp-from-name"
    value = var.smtp_from_name
  }

  secret {
    name  = "smtp-use-ssl"
    value = var.smtp_use_ssl
  }
}