using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Playground
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            await Workspace1.Run();
            Environment.Exit(exitCode: 0);
            var cred = new DefaultAzureCredential();
            var blobEndpoint = new Uri("https://stppemdscontrolplane.blob.core.windows.net/");
            var blobServiceClient = new BlobServiceClient(blobEndpoint, cred);
            var resp = await blobServiceClient.GetPropertiesAsync();
            var prop = resp.Value;

            var blobContainerEnpoint = new Uri("https://stppemdscontrolplane.blob.core.windows.net/");


            var blobUriBuilder = new BlobUriBuilder(blobEndpoint)
            {
                BlobContainerName = "fromvs"
            };

            var blobContainerClient = new BlobContainerClient(blobUriBuilder.ToUri(), cred);
            /*var contResp = await blobContainerClient.CreateIfNotExistsAsync(
                PublicAccessType.None,
                cancellationToken: CancellationToken.None);
            var sampleJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "sample.json");
            var fileStream = File.OpenRead(sampleJsonPath);
            await blobContainerClient.UploadBlobAsync("vsblob", fileStream, CancellationToken.None);*/

            var blobClient = blobContainerClient.GetBlobClient("Ingestion.zip");
            using var blobReadStream = await blobClient.OpenReadAsync();

            var blockBlobClient = blobContainerClient.GetBlockBlobClient("vsblob");

            using (var towrite = File.OpenWrite("C:\\StorageAccountSDK\\Playground\\new.txt"))
            {
                // await blobReadStream.ReadExactlyAsync();
                // await towrite.WriteAsync();

                byte[] buffer = new byte[1024 * 1024 * 10];

                while (true)
                {
                    int bytesRead = await blobReadStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    await towrite.WriteAsync(buffer, 0, bytesRead);
                }
            }
            // await blobContainerClient.UploadBlobAsync("vsblobupload", blobReadStream, CancellationToken.None);
            // blockBlobClient.
            Console.ReadKey();
        }
    }
}