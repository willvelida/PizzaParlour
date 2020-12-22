variable "key_vault_id" {}

variable "resource_group_name" {}

variable "tenant_id" {}

variable "object_id" {}

variable "key_permissions" {
  type = list(string)
  description = "List of key permissions"
}

variable "secret_permissions" {
  type = list(string)
  description = "List of secret permissions"
}

variable "certificate_permissions" {
  type = list(string)
  description = "List of certificate permissions"
}