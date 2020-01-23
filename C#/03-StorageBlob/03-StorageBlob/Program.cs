using System;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using System.IO;

namespace _03_StorageBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sonustorageaccount5;AccountKey=NCvlMtFrp78W7vb389eR3NGtGzQnNtIQDbcqNh9I0IRAGE5hU/H3P78hgVZBnKocljf5KUV2XRcCoeNMk1U6BQ==;EndpointSuffix=core.windows.net";
            var container = GetBlobContainer(storageConnectionString, "files");
            //UploadFileToContainer(container, @"C:\Users\sonus\Desktop\syllabus.txt");
            DownloadFileFromContainer(container, "syllabus.txt");

            Console.ReadLine();
        }

        static CloudBlobContainer GetBlobContainer(string storageConnectionString, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient= storageAccount.CreateCloudBlobClient();
            var container=blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists(); 
            BlobContainerPermissions permissions = new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            container.SetPermissions(permissions);            
            return container;
        }

        static string UploadFileToContainer(CloudBlobContainer container, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var blob = container.GetBlockBlobReference(fileName);
            Console.WriteLine("Uploading file to blob container...");
            blob.UploadFromFile(filePath);
            Console.WriteLine("File uploaded successfully");
            return blob.Uri.AbsoluteUri;
        }

        static void DownloadFileFromContainer(CloudBlobContainer container, string blobName)
        {
            var downloadFilePath = Path.Combine(Environment.CurrentDirectory, blobName);
            var blob=container.GetBlockBlobReference(blobName);
            Console.WriteLine("Downloading file...");
            blob.DownloadToFile(downloadFilePath, FileMode.Create);
            Console.WriteLine($"File downloaded to {downloadFilePath}");
        }
    }
}
