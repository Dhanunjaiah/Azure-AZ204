
New-AzResourceGroup -Name "DemoGroup" -Location "Southeastasia"

New-AzKeyVault -Name 'sonuKeyVault' `
    -ResourceGroupName 'DemoGroup' `
    -Location 'Southeastasia' `
    -EnabledForDiskEncryption

    #Update existing Keyvault for DiskEncryption
Set-AzureRmKeyVaultAccessPolicy -VaultName 'sonuKeyVault' 
    -ResourceGroupName 'DemoGroup' 
    -EnabledForDiskEncryption

Get-AzKeyVault -VaultName 'sonuKeyVault' -ResourceGroupName 'DemoGroup' | Select-Object VaultUri, ResourceId

# Enable disk encryption for existing VM
Set-AzureRmVmDiskEncryptionExtension `
    -ResourceGroupName 'DemoGroup' `
    -VMName 'samplevm' `
    -VolumeType All  #[All | OS | Data]
    -DiskEncryptionKeyVaultId <keyVault.ResourceId> `
    -DiskEncryptionKeyVaultUrl <keyVault.VaultUri> `
    -SkipVmBackup

# Get status of VM Disk encryption
Get-AzureRmVmDiskEncryptionStatus -ResourceGroupName <resource-group> -VMName <vm-name>

# Disable Encryption (Decrypt)
Disable-AzureRmVMDiskEncryption -ResourceGroupName <resource-group> -VMName <vm-name>