using System;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace _01_CreateIaaSVm
{
    class Program
    {
        private static string clientId = "91b7abc3-8f9c-43e5-b936-d2801fd69861";
        private static string clientSecret = "0165d053-27b6-4b44-aabe-d7663c6b8dfb";
        private static string azureTenantId = "fc73ed10-905f-4b66-85d6-ae10c14623d7";
        private static string azureSubscriptionId = "31455576-2160-407a-8363-0b842921ef7f";

        static void Main(string[] args)
        {
            Console.WriteLine("Logging in to Azure...");
            AzureCredentials credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(clientId, clientSecret, azureTenantId, AzureEnvironment.AzureGlobalCloud );
            var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithSubscription(azureSubscriptionId);
            Console.WriteLine("Successfully logged in to Azure");

            //Create resource group
            Console.WriteLine("Creating resource group...");
            var resourceGroup= azure.ResourceGroups
                .Define("DemoGroup")
                .WithRegion(Region.AsiaSouthEast)
                .Create();
            Console.WriteLine($"Resource group '{resourceGroup.Name}' created");

            //Create VNET
            Console.WriteLine("Creating Virtual Network...");
            var vnet = azure.Networks
                .Define("test-sea-vnet")
                .WithRegion(Region.AsiaSouthEast)
                .WithExistingResourceGroup(resourceGroup)
                .WithAddressSpace("10.5.0.0/16")
                .WithSubnet("Frontend", "10.5.1.0/24")
                .Create();
            Console.WriteLine($"Virtual Network '{vnet.Name}' created");

            //Create network interface
            Console.WriteLine("Creating NIC ...");
            var nic= azure.NetworkInterfaces
                .Define("test-sea-01-demo-nic")
                .WithRegion(Region.AsiaSouthEast)
                .WithExistingResourceGroup(resourceGroup)
                .WithExistingPrimaryNetwork(vnet)
                .WithSubnet("Frontend")
                .WithPrimaryPrivateIPAddressDynamic()
                .WithNewPrimaryPublicIPAddress()
                .Create();
            Console.WriteLine($"NIC '{nic.Name}' created");

            //Create VM
            Console.WriteLine("Creating VM ...");
            var vmName = "test-sea1-demo";
            var vm=azure.VirtualMachines
                .Define(vmName)
                .WithRegion(Region.AsiaSouthEast)
                .WithExistingResourceGroup(resourceGroup.Name)
                .WithExistingPrimaryNetworkInterface(nic)             
                .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2016-Datacenter")
                .WithAdminUsername("labuser")
                .WithAdminPassword("Password@123")
                .WithComputerName(vmName)
                .WithSize(VirtualMachineSizeTypes.StandardDS1)
                .Create();
            Console.WriteLine($"Virtual Machine '{vm.Name}' created in VNET '{vnet.Name}");
        }

    }
}
