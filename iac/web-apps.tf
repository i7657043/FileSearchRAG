resource "azurerm_linux_web_app" "portalui" {
  name                = ""
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_service_plan.this.location
  service_plan_id     = azurerm_service_plan.this.id

  depends_on = [azurerm_app_service_certificate.this]

  app_settings = {
    apiKeys__pinecone = var.pineconeApiKey
    apiKeys__opneAi   = var.openAiApiKey
    pinecone__index   = "fs-live-index"
  }

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }
  }
}

variable "pineconeApiKey" {
  type      = string
  sensitive = true
}

variable "openAiApiKey" {
  type      = string
  sensitive = true
}
