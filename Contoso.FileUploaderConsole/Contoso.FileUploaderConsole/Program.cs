using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

class Program
{
    static async Task Main()
    {
        // Replace with your Azure Storage SAS token URL
        string sasUrl = "https://sasmartwebshopteam1.blob.core.windows.net/products?sp=racwdl&st=2025-03-17T16:10:29Z&se=2025-03-27T00:10:29Z&sv=2022-11-02&sr=c&sig=gkzRA1LG%2BpUJvApEWaYoCv3Wle90uefn3T%2BrnfKx%2Fcc%3D";
        string containerName = "products";

        Console.Write("Enter the folder path to upload: ");
        string folderPath = Console.ReadLine();

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                await UploadToBlobStorage(sasUrl, containerName, filePath, fileName);
            }
        }
        else
        {
            Console.WriteLine("Folder not found. Please check the path and try again.");
        }
    }

    static async Task UploadToBlobStorage(string sasUrl, string containerName, string filePath, string fileName)
    {
        try
        {
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(sasUrl));
            // await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            using FileStream uploadFileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            Console.WriteLine($"File '{fileName}' uploaded successfully to container '{containerName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
