data "azurerm_key_vault" "secrets" {
  name                = "tf-filesearch-kv"
  resource_group_name = "tf-filesearch-rag"
}

data "azurerm_key_vault_secret" "pineconeapikey" {
  name         = "pinecone-api-key"
  key_vault_id = data.azurerm_key_vault.secrets.id
}

data "azurerm_key_vault_secret" "openaiapikey" {
  name         = "open-ai-api-key"
  key_vault_id = data.azurerm_key_vault.secrets.id
}
