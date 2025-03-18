using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

class Program
{
    static async Task Main()
    {
        // Replace with your Azure Storage SAS token URL
        string sasUrl = "https://sasmartwebshopteam1.blob.core.windows.net/products?sp=racwdl&st=2025-03-17T16:10:29Z&se=2025-03-27T00:10:29Z&sv=2022-11-02&sr=c&sig=gkzRA1LG%2BpUJvApEWaYoCv3Wle90uefn3T%2BrnfKx%2Fcc%3D";
        string containerName = "products";

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Upload files");
            Console.WriteLine("2. Retrieve all product details");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
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
                    break;

                case "2":
                    Console.WriteLine("Retrieving all product details...");
                    await GetAllProducts(sasUrl);
                    break;

                case "3":
                    Console.WriteLine("Exiting program...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                    break;
            }
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

    static async Task GetAllProducts(string sasUrl)
    {
        try
        {
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(sasUrl));
            List<string> productDetails = new List<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                string url = sasUrl.Split('?')[0] + "/" + blobItem.Name;
                string savedDate = blobItem.Properties.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown";
                productDetails.Add($"Name: {blobItem.Name}, URL: {url}, Date: {savedDate}");
            }

            Console.WriteLine("Product List:");
            productDetails.ForEach(Console.WriteLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving products: {ex.Message}");
        }
    }


}
