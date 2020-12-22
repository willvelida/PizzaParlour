resource "azurerm_key_vault_secret" "this" {
  name = var.name
  value = var.value
  key_vault_id = var.key_vault_id
}