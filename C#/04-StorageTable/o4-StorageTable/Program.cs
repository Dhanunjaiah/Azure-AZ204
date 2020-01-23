using System;
using Microsoft.Azure.Cosmos.Table;

namespace _04_StorageTable
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sonu-tableapi;AccountKey=CW954GOsoT0ofhnba4ZK3H7fkDKQBcr85E6XnjXSrzCgSYa0os7b4nJ5OxL8QkG6bI7YLTjPHKedxIgKSzzotA==;TableEndpoint=https://sonu-tableapi.table.cosmos.azure.com:443/";
            var table = GetTable(storageConnectionString, "sales");

            //Insert
            var saleData = new SaleData("102", "West")
            {
                Name = "Sekhar",
                Email = "sekhar@gmail.com",
                Amount = 129000,
                ReportDate = new DateTime(2020, 1, 20)
            };
            var result = InsertEntity(table, saleData);
            Console.WriteLine($"Sales details of {result.Name} inserted successfully");

            //Read
            //var data = GetSaleData(table, "101", "South");
            //if (data != null)
            //{
            //    Console.WriteLine($"Name: {data.Name}");
            //    Console.WriteLine($"Email: {data.Email}");
            //    Console.WriteLine($"Amount: {data.Amount}");
            //    Console.WriteLine($"ReportDate: {data.ReportDate}");
            //}
            //else
            //{
            //    Console.WriteLine("No entities found");
            //}

            ////Delete
            //var res=DeleteSaleData(table, "101", "South");
            //if (res != null)
            //{
            //    Console.WriteLine($"Entity of { res.Name } deleted");
            //}
            //else
            //{
            //    Console.WriteLine("No entities found");
            //}
            Console.ReadLine();
        }

        static CloudTable GetTable(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient=storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        static SaleData InsertEntity(CloudTable table, SaleData data )
        {
            TableOperation operation = TableOperation.InsertOrReplace(data);
            var result= table.Execute(operation);
            Console.WriteLine("A new sale data entity inserted...");
            return result.Result as SaleData;
        }

        static SaleData GetSaleData(CloudTable table,string rowKey, string partitionKey)
        {
            TableOperation operation = TableOperation.Retrieve<SaleData>(partitionKey, rowKey);
            var tableResult = table.Execute(operation);
            if (tableResult.Result != null)
                return tableResult.Result as SaleData;
            else
                return null;
        }

        static SaleData DeleteSaleData(CloudTable table, string rowKey, string partitionKey)
        {
            TableOperation operation = TableOperation.Retrieve<SaleData>(partitionKey, rowKey);
            var tableResult = table.Execute(operation);
            if (tableResult.Result != null) {
                var saleData=tableResult.Result as SaleData;
                TableOperation deleteOperation = TableOperation.Delete(saleData);
                var result = table.Execute(deleteOperation);
                return result.Result as SaleData;
            }
            else
                return null;
        }
    }
}
