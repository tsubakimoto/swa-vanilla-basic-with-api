using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text;

namespace Api;

public class Storage
{
    private readonly ILogger<Storage> _logger;

    public Storage(ILogger<Storage> logger)
    {
        _logger = logger;
    }

    [Function("storage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function 'storage' processed a request.");

        string accountName = "styutaswamid";
        string containerName = "files";
        string containerEndpoint = $"https://{accountName}.blob.core.windows.net/{containerName}";
        BlobContainerClient containerClient = new (new Uri(containerEndpoint), new DefaultAzureCredential());

        await containerClient.CreateIfNotExistsAsync();
        List<string> blobNames = new List<string>();

        try
        {
            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
                blobNames.Add(blob.Name);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return new OkObjectResult($"Files: {string.Join(",", blobNames)}");
    }
}
