resource "azurerm_service_plan" "this" {
  name                = "filesearch-rag-asp"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "this" {
  name                = "filesearch-rag-webapi"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_service_plan.this.location
  service_plan_id     = azurerm_service_plan.this.id

  app_settings = {
    apiKeys__pinecone = data.azurerm_key_vault_secret.pineconeapikey.value
    apiKeys__openAi   = data.azurerm_key_vault_secret.openaiapikey.value
    pinecone__index   = "fs-live-index"
  }

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
  }
}

# resource "azurerm_linux_web_app" "ui" {
#   name                = "filesearch-rag-ui"
#   resource_group_name = data.azurerm_resource_group.this.name
#   location            = azurerm_service_plan.this.location
#   service_plan_id     = azurerm_service_plan.this.id

#   app_settings = {
#     NODE_ENV               = "production"
#     REACT_APP_API_BASE_URL = "https://${azurerm_linux_web_app.this.default_hostname}" # Reference the API URL
#   }

#   site_config {
#     application_stack {
#       node_version = "18" # Specify the Node.js version
#     }

#     always_on = true # Ensure the app is always running
#   }
# }

output "web_app_url" {
  value = azurerm_linux_web_app.this.default_hostname
}
