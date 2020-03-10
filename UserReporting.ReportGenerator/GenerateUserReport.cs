using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

using UserReporting.ReportGenerator.Library;
using UserReporting.Shared.Events;
using UserReporting.Shared.Tables;

namespace UserReporting.ReportGenerator
{
    public static class GenerateUserReport
    {
        [FunctionName("GenerateUserReport")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("eventqueue", Connection = "AzureWebJobsStorage")]CreateReportRequested request,
            [Table("downloads", Connection = "AzureWebJobsStorage")] IAsyncCollector<UserReportRecord> download,
            [Blob("user-reports", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            if (await container.CreateIfNotExistsAsync())
            {
                var permissions = await container.GetPermissionsAsync();
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await container.SetPermissionsAsync(permissions);
            }

            var blob = container.GetBlockBlobReference($"{request.Id}.pdf");

            var pdfFile = PDFGenerator.GeneratePDF(request);

            await blob.UploadFromByteArrayAsync(pdfFile, 0, pdfFile.Length);

            var url = blob.Uri.AbsoluteUri;

            await download.AddAsync(new UserReportRecord
            {
                PartitionKey = "UserReport",
                RowKey = request.Id.ToString(),
                Name = $"{request.FirstName} {request.LastName}",
                Url = url
            });
        }
    }
}
