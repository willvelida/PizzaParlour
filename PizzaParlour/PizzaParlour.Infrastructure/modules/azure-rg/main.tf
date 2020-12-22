resource "azurerm_resource_group" "azure-rg" {
  name = var.resource_group_name
  location = var.location
  tags = {
      Terraform = "true"
      AppName = "PizzaParlor"
  }
}