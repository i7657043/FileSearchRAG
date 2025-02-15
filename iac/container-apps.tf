resource "azurerm_container_app_environment" "this" {
  name                = "filesearch-rag-cont-env"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
}

resource "azurerm_container_app" "this" {
  name                         = "filesearch-rag-ui"
  container_app_environment_id = azurerm_container_app_environment.this.id
  resource_group_name          = azurerm_resource_group.this.name
  revision_mode                = "Single"

  secret {
    name  = "acrpassword"
    value = var.acrpassword
  }
  template {
    container {
      name   = "azurermcontainer"
      image  = "contdevreg.azurecr.io/filesearchrag-web-ui:${var.imageTag}"
      cpu    = 0.25
      memory = "0.5Gi"
      env {
        name  = "NODE_ENV"
        value = "production"
      }
    }
    min_replicas = 1
    max_replicas = 1
  }

  registry {
    server               = "contdevreg.azurecr.io"
    username             = "contdevreg"
    password_secret_name = "acrpassword"
  }

  ingress {
    external_enabled = true
    target_port      = 80

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}

variable "imageTag" {
  type      = string
  sensitive = true
}

variable "acrpassword" {
  type      = string
  sensitive = true
}
