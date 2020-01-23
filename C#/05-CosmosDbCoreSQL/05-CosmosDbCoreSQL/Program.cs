
using _05_CosmosDbCoreSQL.Models;
using Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace _05_CosmosDbCoreSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            string docuemntUri = "https://sonu-coresql.documents.azure.com:443/";
            string key = "iV7BwV7rhYVIQ4kdHsFYkKQume7E33aCtr6LqGqm6Xq0EiGGC38LzPjO5o97invxxHyA84qswrLfztZphc0aHg==";
            string databaseName = "shopdb";
            string collectionName = "sales";

            var container = GetDocumentContainer(docuemntUri, key, databaseName, collectionName, "/location").GetAwaiter().GetResult();
            //SaleData data = new SaleData()
            //{
            //    Id = "1",
            //    Name = "Saroj kumar",
            //    Amount = 125000,
            //    Location = "Mumbai",
            //    ReportDate = new DateTime(2020, 1, 20)
            //};
            //var result= InsertSaleDataAsync(container, data).GetAwaiter().GetResult();
            //var docText = JsonConvert.SerializeObject(result, Formatting.Indented);
            //Console.WriteLine("New document created:");
            //Console.WriteLine(docText);

            //var result = GetSaleDataAsync(container, "1", "Mumbai").GetAwaiter().GetResult();
            //if (result != null)
            //{
            //    var docText = JsonConvert.SerializeObject(result, Formatting.Indented);
            //    Console.WriteLine(docText);
            //}
            //else
            //{
            //    Console.WriteLine("Document not found");
            //}

            var sales=GetSalesDataByLocationAsync(container, "Mumbai").GetAwaiter().GetResult();
            foreach(var saleData in sales)
            {
                var docText = JsonConvert.SerializeObject(saleData, Formatting.Indented);
                Console.WriteLine(docText);
            }

            Console.ReadLine();
        }

        static async Task<CosmosContainer> GetDocumentContainer(string documentUri, string key, string dbName, string containerName, string partitionKey)
        {
            CosmosClient client = new CosmosClient(documentUri, key);
            await client.CreateDatabaseIfNotExistsAsync(dbName);
            var db = client.GetDatabase(dbName);
            await db.CreateContainerIfNotExistsAsync(containerName, partitionKey);
            var container = db.GetContainer(containerName);
            return container;
        }

        static async Task<SaleData> InsertSaleDataAsync(CosmosContainer container, SaleData saleData)
        {
            try
            {
                ItemResponse<SaleData> result = await container.ReadItemAsync<SaleData>(saleData.Id, new PartitionKey(saleData.Location));
                Console.WriteLine("Item already exists for document with id " + result.Value.Id);
                return null;
            } catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                ItemResponse<SaleData> result = await container.CreateItemAsync<SaleData>(saleData, new PartitionKey(saleData.Location));
                Console.WriteLine("New document inserted with id " + result.Value.Id);
                return result.Value;
            }
        }

        static async Task<SaleData> GetSaleDataAsync(CosmosContainer container, string docId, string location)
        {
            try
            {
                ItemResponse<SaleData> result = await container.ReadItemAsync<SaleData>(docId, new PartitionKey(location));
                return result.Value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static async Task<List<SaleData>> GetSalesDataByLocationAsync(CosmosContainer container, string location)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.location = '{location}'";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            List<SaleData> sales = new List<SaleData>();
            var queryResult = container.GetItemQueryIterator<SaleData>(queryDefinition);

            await foreach (SaleData saleData in queryResult)
            {
                sales.Add(saleData);
            }
            return sales;
        }

    }
}
