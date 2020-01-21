using System;
using Microsoft.Azure.Management.Batch;
using Microsoft.Azure.Management.Batch.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace _02_AzureBatchDemo
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
                .FromServicePrincipal(clientId, clientSecret, azureTenantId, AzureEnvironment.AzureGlobalCloud);
            var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithSubscription(azureSubscriptionId);
            Console.WriteLine("Successfully logged in to Azure");

            //Create resource group
            Console.WriteLine("Creating resource group...");
            var resourceGroup = azure.ResourceGroups
                .Define("DemoGroup")
                .WithRegion(Region.AsiaSouthEast)
                .Create();
            Console.WriteLine($"Resource group '{resourceGroup.Name}' created");

            //
            BatchManagementClient batchManagementClient=new BatchManagementClient(credentials);
            // Create a new Batch account
            var batchAccCreateParameters = new BatchAccountCreateParameters() { Location = "West US" };
            batchManagementClient.BatchAccount.CreateAsync("DemoGroup", "sonubathaccount1", batchAccCreateParameters ).GetAwaiter().GetResult();
            
            // Get the new account from the Batch service
            BatchAccount account = batchManagementClient.BatchAccount.GetAsync("DemoGroup","sonubathaccount1").GetAwaiter().GetResult();
            
            
            // Delete the account
            batchManagementClient.BatchAccount.DeleteAsync("DemoGroup", account.Name);
        }
    }
}
