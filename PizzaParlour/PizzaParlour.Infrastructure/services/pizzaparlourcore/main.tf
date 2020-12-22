provider "azurerm" {
  features {}
}

terraform {
  backend "azurerm" {
      resource_group_name = ""
      storage_account_name = ""
      container_name = ""
      key = ""
  }
}

module "azure-rg" {
  source = "../../modules/azure-rg"
}

## Create storage account

## Create Key vault