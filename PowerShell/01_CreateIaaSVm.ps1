Connect-AzAccount

Select-AzSubscription -Subscription "MSDN Platforms"

$resoruceGroup = "DemoGroup"
$vmName = "test-sea1-vm";
$location = "Southeast asia"
$vnetName = "test-vnet"
$subnetName = "Frontend"
$securityGroupName = "test-nsg"
$pipName = "test-sea-pubip"

New-AzVM `
    -ResourceGroupName $resoruceGroup `
    -Name $vmName `
    -Location $location `
    -VirtualNetworkName  $vnetName `
    -SubnetName $subnetName `
    -SecurityGroupName $securityGroupName `
    -PublicIpAddressName $pipName `
    -OpenPorts 80, 3389