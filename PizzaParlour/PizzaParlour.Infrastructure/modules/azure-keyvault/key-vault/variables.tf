variable "name" {}

variable "resource_group_name" {}

variable "location" {}

variable "sku" {
  default = "standard"
  description = "The name of the SKU used for this key vault"
}

variable "tenant_id" {}