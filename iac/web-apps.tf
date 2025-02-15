resource "azurerm_linux_web_app" "portalui" {
  name                = ""
  resource_group_name = data.azurerm_resource_group.this.name
  location            = data.azurerm_service_plan.this.location
  service_plan_id     = data.azurerm_service_plan.this.id

  app_settings = {
    apiKeys__pinecone = data.azurerm_key_vault_secret.pineconeapikey.value
    apiKeys__opneAi   = data.azurerm_key_vault_secret.openaiapikey.value
    pinecone__index   = "fs-live-index"
  }

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
  }
}
