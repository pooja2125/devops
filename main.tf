# We strongly recommend using the required_providers block to set the
# Azure Provider source and version being used
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "2.65.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
}
# Create Resource Group
resource "azurerm_resource_group" "state-demo-rg" {
  name     = var.resource_group_name
  location = var.location
}

# Create Storage Account
resource "azurerm_storage_account" "state-demo-secure" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.state-demo-rg.name
  location                 = azurerm_resource_group.state-demo-rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

# Create virtual network
resource "azurerm_virtual_network" "state-demo-TFNet" {
  name                = var.virtual_network_name
  address_space       = var.virtual_network_cidr
  location            = var.location
  resource_group_name = azurerm_resource_group.state-demo-rg.name

}

# Create subnet
resource "azurerm_subnet" "state-demo-subnet" {
  name                 = var.subnet_name
  resource_group_name  = azurerm_resource_group.state-demo-rg.name
  address_prefixes     = var.subnet_cidr
  virtual_network_name = azurerm_virtual_network.state-demo-TFNet.name
}

# Deploy Public IP
resource "azurerm_public_ip" "state-demo-PBIP" {
  name                = var.public_ip_name
  resource_group_name = azurerm_resource_group.state-demo-rg.name
  location            = var.location
  allocation_method   = var.allocation_method_type
  sku                 = var.sku_type

}

#Create NIC
resource "azurerm_network_interface" "state-demo-NIC" {
  name                = var.network_interface_name
  location            = var.location
  resource_group_name = azurerm_resource_group.state-demo-rg.name

  ip_configuration {
    name                          = var.ipconfig1
    subnet_id                     = azurerm_subnet.state-demo-subnet.id
    private_ip_address_allocation = var.allocation_method_type
    public_ip_address_id          = azurerm_public_ip.state-demo-PBIP.id
  }
}

# Create Virtual Machine
resource "azurerm_virtual_machine" "state-demo-VM1" {
  name                             = var.virtual_machine_name
  location                         = var.location
  resource_group_name              = azurerm_resource_group.state-demo-rg.name
  network_interface_ids            = [azurerm_network_interface.state-demo-NIC.id]
  vm_size                          = var.vm_size
  delete_os_disk_on_termination    = var.delete_os_disk_on_termination
  delete_data_disks_on_termination = var.delete_data_disks_on_termination

  storage_image_reference {
    publisher = var.vm_os_publisher
    offer     = var.vm_os_offer
    sku       = var.vm_os_sku
    version   = var.vm_os_version
  }

  storage_os_disk {
    name              = var.vm_hostname
    disk_size_gb      = var.vm_disk_size_gb
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = var.managed_disk_type
  }

  os_profile {
    computer_name  = var.vm_computer_name
    admin_username = var.vm_admin_username
    admin_password = var.vm_admin_password
  }

  os_profile_linux_config {
    disable_password_authentication = false
  }

  boot_diagnostics {
    enabled     = var.vm_boot_diagnostics
    storage_uri = azurerm_storage_account.state-demo-secure.primary_blob_endpoint
  }
}

# Create MySQL DB
resource "azurerm_sql_server" "state-demo-sql-server" {
  name                         = var.sql_server_name
  resource_group_name          = azurerm_resource_group.state-demo-rg.name
  location                     = var.location
  version                      = var.sql_version
  administrator_login          = var.administrator_login
  administrator_login_password = var.administrator_login_password
}

resource "azurerm_sql_database" "state-demo-sql-db" {
  name                = var.sql_db
  resource_group_name = azurerm_resource_group.state-demo-rg.name
  location            = var.location
  server_name         = azurerm_sql_server.state-demo-sql-server.name
  extended_auditing_policy {
    storage_endpoint                        = azurerm_storage_account.state-demo-secure.primary_blob_endpoint
    storage_account_access_key              = azurerm_storage_account.state-demo-secure.primary_access_key
    storage_account_access_key_is_secondary = true
    retention_in_days                       = 6
  }
}

