variable "resource_group_name" {}

variable "location" {}

variable "name" {}

variable "account_tier" {
  default = "Standard"
  description = "Defines the tier to use for this storage account"
}

variable "account_kind" {
  default = "StorageV2"
  description = "Defines the kind of account"
}

variable "account_replication_type" {
  default = "GRS"
  description = "Defines the type of replication for this storage account"
}