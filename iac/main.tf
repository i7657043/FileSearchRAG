terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.16"
    }
  }
  backend "azurerm" {
    resource_group_name  = "tf-filesearch-rag"
    storage_account_name = "filesearchragtfsto"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}
