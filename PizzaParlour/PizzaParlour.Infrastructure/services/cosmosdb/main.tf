provider "azurerm" {
  features {}
}

terraform {
  backend "azurerm" {

  }
}

module "azure-rg" {
  source = "../../modules/azure-rg"
}

resource "azurerm_resource_group" "rg" {
  name = module.azure-rg.rg_name
  location = module.azure-rg.rg_location
}

resource "azurerm_cosmosdb_account" "account" {
  name = var.cosmosdb_account_name
  location = azurerm_resource_group.rg.location 
  resource_group_name = azurerm_resource_group.rg.name
  offer_type = "Standard"
  kind = "GlobalDocumentDB"

  capabilities {
    name = "EnableServerless"
  }

  consistency_policy {
    consistency_level = "Session"
  }

  depends_on = [ azurerm_resource_group.rg ]
}
