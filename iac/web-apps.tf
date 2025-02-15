resource "azurerm_service_plan" "this" {
  name                = "filesearch-rag-asp"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "this" {
  name                = var.webApiAppName
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_service_plan.this.location
  service_plan_id     = azurerm_service_plan.this.id

  app_settings = {
    apiKeys__pinecone = var.pineconeApiKey
    apiKeys__openAi   = var.openAiApiKey
    pinecone__index   = "fs-live-index"
  }

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
  }
}

