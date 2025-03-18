using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=tcp:htc-az204.database.windows.net,1433;Initial Catalog=contoso;Persist Security Info=False;User ID=contosoadmin1;Password=Sigurnipass123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    static async Task Main()
    {
        string sasUrl = "https://sasmartwebshopteam1.blob.core.windows.net/products?sp=racwdl&st=2025-03-17T16:10:29Z&se=2025-03-27T00:10:29Z&sv=2022-11-02&sr=c&sig=gkzRA1LG%2BpUJvApEWaYoCv3Wle90uefn3T%2BrnfKx%2Fcc%3D";

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Upload product");
            Console.WriteLine("2. Retrieve all products");
            Console.WriteLine("3. Delete product");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter product name: ");
                    string productName = Console.ReadLine();
                    Console.Write("Enter category: ");
                    string category = Console.ReadLine();
                    Console.Write("Enter description: ");
                    string description = Console.ReadLine();
                    Console.Write("Enter price: ");
                    decimal price = decimal.Parse(Console.ReadLine());
                    Console.Write("Enter image file path: ");
                    string filePath = Console.ReadLine();
                    string fileName = Path.GetFileName(filePath);
                    await UploadProduct(sasUrl, filePath, fileName, productName, category, description, price);
                    break;

                case "2":
                    Console.WriteLine("Retrieving all products...");
                    await GetAllProducts();
                    break;

                case "3":
                    Console.Write("Enter the product name to delete: ");
                    string productToDelete = Console.ReadLine();
                    await DeleteProduct(sasUrl, productToDelete);
                    break;

                case "4":
                    Console.WriteLine("Exiting program...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 4.");
                    break;
            }
        }
    }

    static async Task UploadProduct(string sasUrl, string filePath, string fileName, string name, string category, string description, decimal price)
    {
        try
        {
            BlobClient blobClient = new BlobClient(new Uri(sasUrl + "/" + fileName));
            using FileStream uploadFileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            string imageUrl = sasUrl.Split('?')[0] + "/" + fileName;
            await SaveProductToDB(name, category, description, price, imageUrl);
            Console.WriteLine($"Product '{name}' uploaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task GetAllProducts()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            string query = "SELECT Name, Category, Description, Price, ImageUrl, CreatedAt FROM Products";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"Name: {reader["Name"]}, Category: {reader["Category"]}, Description: {reader["Description"]}, Price: {reader["Price"]}, Image URL: {reader["ImageUrl"]}, Created At: {reader["CreatedAt"]}");
                }
            }
        }
    }

    static async Task SaveProductToDB(string name, string category, string description, decimal price, string imageUrl)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            string query = "INSERT INTO Products (Name, Category, Description, Price, ImageUrl, CreatedAt) VALUES (@Name, @Category, @Description, @Price, @ImageUrl, GETDATE())";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    static async Task DeleteProduct(string sasUrl, string productName)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            string query = "SELECT ImageUrl FROM Products WHERE Name = @Name";
            string imageUrl = "";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", productName);
                object result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    imageUrl = result.ToString();
                }
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                BlobClient blobClient = new BlobClient(new Uri(imageUrl));
                await blobClient.DeleteIfExistsAsync();
                query = "DELETE FROM Products WHERE Name = @Name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", productName);
                    await cmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine($"Product '{productName}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }
    }
}

