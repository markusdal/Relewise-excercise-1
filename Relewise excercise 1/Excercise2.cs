using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Relewise.Client.DataTypes;
using System.Text.Json;

namespace Relewise_excercise_1
{
    internal class Excercise2 : IJob
    {
        public async Task<string> Execute(JobArguments arguments, Func<string, Task> info, Func<string, Task> warn, CancellationToken token)
        {
            try
            {
                string rawData = await DownloadDataFromUrl("https://cdn.relewise.com/academy/productdata/customjsonfeed", token);

                List<Product> products = ParseAndMapData(rawData);

                string message = $"Mapped {products.Count} products";
                await info(message);
                return message;
                
            }
            catch (Exception ex)
            {

                await warn($"Error has occurred: {ex.Message}");
                return "Error has occurred during execution.";
            }
        }

        private async Task<string> DownloadDataFromUrl(string url, CancellationToken token)
        {

            using (HttpClient client = new HttpClient())
            {
                // Send HTTP GET request
                HttpResponseMessage responseMessage = await client.GetAsync(url, token);
                responseMessage.EnsureSuccessStatusCode();

                // return content as string
                return await responseMessage.Content.ReadAsStringAsync();
            }
        }

        private List<Product> ParseAndMapData(string rawData)
        {
    
            List<Product> products = new List<Product>();

            JsonDocument jsonDocument = JsonDocument.Parse(rawData);

            // Get the root element of the JSON document
            JsonElement root = jsonDocument.RootElement;

            // Check if the root element is an array
            if (root.ValueKind == JsonValueKind.Array)
            {
                // Iterate through each element in the array
                foreach (JsonElement productElement in root.EnumerateArray())
                {
                    string id = productElement.GetProperty("productId").GetString();
                    string displayName = productElement.GetProperty("productName").GetString();
                    string listPrice = productElement.GetProperty("listPrice").GetString();
                    string salesPrice = productElement.GetProperty("salesPrice").GetString();

                    // Map fields to Product properties
                    Product product = new Product(id);
                    product.DisplayName = new Multilingual("en", displayName);
                    product.ListPrice = new MultiCurrency(listPrice, 2);
                    product.SalesPrice = new MultiCurrency(salesPrice, 2);

                    // Add mapped product to the list
                    products.Add(product);
                }
            }

            // Dispose the JsonDocument
            jsonDocument.Dispose();

            return products;
        }
    }
}
