using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;

namespace Playground
{

    public static class Workspace1
    {
        public static async Task Run()
        {
            var st = Stopwatch.GetTimestamp();

            var blobEndpoint = new Uri("https://stppemdscontrolplane.blob.core.windows.net/");
            var blobUriBuilder = new BlobUriBuilder(blobEndpoint)
            {
                BlobContainerName = "artifacts"
            };
            var blobContainerClient = new BlobContainerClient(blobUriBuilder.ToUri(), new DefaultAzureCredential());

            /*await blobContainerClient.CreateIfNotExistsAsync();
            using var fileStream = File.OpenRead("C:\\StorageAccountSDK\\Playground\\new.txt");

            // await blobContainerClient.UploadBlobAsync("0/0/0/new.txt", fileStream);
            await blobContainerClient.UploadBlobAsync("0/0/new.txt", fileStream);*/

            
            var blobClient = blobContainerClient.GetBlobClient("1.0.0/ingestion.zip");


            var destBlobEndpoint = new Uri("https://mci4mrpcontrolpane.blob.core.windows.net/");
            var destBlobUriBuilder = new BlobUriBuilder(destBlobEndpoint)
            {
                BlobContainerName = "artifacts2"
            };
            
            var destBlobContainerClient = new BlobContainerClient(destBlobUriBuilder.ToUri(), new DefaultAzureCredential());
            var destBlobClient = destBlobContainerClient.GetBlobClient("ingestion.zip");

            var blobRequestConditions = new BlobRequestConditions()
            {
                
            };
            /*
            using var blobStream = await blobClient.OpenReadAsync(
                position: 0,
                bufferSize: 60 * 1024 * 1024,
                // bufferSize: null,
                conditions: null);            // await destBlobContainerClient.UploadBlobAsync("ingestion.zip", blobStream);
            await destBlobClient.UploadAsync(content: blobStream, conditions: null, transferOptions: new Azure.Storage.StorageTransferOptions()
            {
                InitialTransferSize = 1 * 1024 * 1024,
                MaximumTransferSize = 10 * 1024 * 1024,
                MaximumConcurrency = 10
            });
            */
            // var blob1 = await blobClient.DownloadContentAsync();
            /*
            var blob1 = await blobClient.DownloadStreamingAsync(options: new BlobDownloadOptions()
            {
            });
            */
            ThreadPool.SetMinThreads(1500, 4);
            ServicePointManager.DefaultConnectionLimit = 1000;
            /*using var blob1 = new MemoryStream()
            {
                Capacity = 60 * 1024 * 1024
            };*/


            /*
                        Console.WriteLine(Directory.GetCurrentDirectory().ToString());
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "file.zip");
                        await blobClient.DownloadToAsync(filePath, default, transferOptions: new Azure.Storage.StorageTransferOptions()
                        {
                            MaximumConcurrency = 8,
                            MaximumTransferSize = 1 * 1024 * 1024
                        });
            */

            /*
            var blobServiceClient = new BlobServiceClient(blobEndpoint, new DefaultAzureCredential());
            var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1));
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(1)
            };
            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
            var blobSasUriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(
                    userDelegationKey,
                    blobClient
                    .GetParentBlobContainerClient()
                    .GetParentBlobServiceClient().AccountName)
            };
            var temp = blobSasUriBuilder.ToUri();
            await destBlobClient.SyncCopyFromUriAsync(blobSasUriBuilder.ToUri());
            */

            var token = await (new DefaultAzureCredential()).GetTokenAsync(new Azure.Core.TokenRequestContext(scopes: new string[] { "https://storage.azure.com/.default" }));

            await destBlobClient.SyncCopyFromUriAsync(blobClient.Uri, options: new BlobCopyFromUriOptions()
            {
                SourceAuthentication = new Azure.HttpAuthorization("Bearer", token.Token)
            });

            /*await blobClient.DownloadToAsync(blob1, transferOptions: new Azure.Storage.StorageTransferOptions()
            {
                MaximumConcurrency = 8,
                MaximumTransferSize = 8 * 1024 * 1024
            });*/
            /*await destBlobClient.UploadAsync(blob1, transferOptions: new Azure.Storage.StorageTransferOptions()
            {
                MaximumConcurrency = 8,
                MaximumTransferSize = 50 * 1024 * 1024
            });*/
            // await destBlobClient.UploadAsync(blobStream, conditions: null);
            Console.WriteLine(Stopwatch.GetElapsedTime(st));
            Console.ReadKey();
        }
    }
}