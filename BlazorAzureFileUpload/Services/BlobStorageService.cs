using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlazorAzureFileUpload.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlobStorageService> _logger;
        string blobStorageConnection = string.Empty;
        private string blobContainerName = "blazorfilestorage";

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            blobStorageConnection = _configuration.GetConnectionString("AzureStorageAccount");
        }
        
        public async Task<bool> DeleteFileToBlobAsync(string strFileName)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                
                var createResponse = await container.CreateIfNotExistsAsync();
                
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                
                var blob = container.GetBlobClient(strFileName);
                
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                var createRespone = await container.CreateIfNotExistsAsync();
                
                if(createRespone != null && createRespone.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                var blob = container.GetBlobClient(strFileName);

                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contecntType });

                var urlString = blob.Uri.ToString();

                return urlString;

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;

                throw;
            }
        }
    }
}
