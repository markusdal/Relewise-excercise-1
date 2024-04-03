using Relewise.Client.DataTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Relewise_excercise_1
{
    internal class Excercise1 : IJob
    {
        public async Task<string> Execute(JobArguments arguments, Func<string, Task> info, Func<string, Task> warn, CancellationToken token)
        {
            try
            {
                string rawData = await DownloadDataFromUrl("https://cdn.relewise.com/academy/productdata/raw", token);

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

            // Split rows
            var rows = rawData.Split('\n');

            // Skip header row and seperator
            for (int i = 2; i < rows.Length; i++)
            {
                var row = rows[i];

                // Split columns
                var columns = Regex.Split(row.Trim(), @"\s*\|\s*");

                if (columns.Length >= 10)
                {
                    string id = columns[1].Trim();
                    string displayName = columns[2].Trim();
                    string listPrice = columns[4].Trim();
                    string salesPrice = columns[3].Trim();

                    // Map fields to Product properties
                    var product = new Product(id);
                    product.DisplayName = new Multilingual("en", displayName);
                    product.ListPrice = new MultiCurrency(listPrice, 2);
                    product.SalesPrice = new MultiCurrency(salesPrice, 2);

                    // Add mapped product to the list
                    products.Add(product);
                }
            }

            return products;
        }
    }
}
