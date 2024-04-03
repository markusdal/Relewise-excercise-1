using Relewise.Client.DataTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Relewise_excercise_1
{
    internal class Excercise3 : IJob
    {
        public async Task<string> Execute(JobArguments arguments, Func<string, Task> info, Func<string, Task> warn, CancellationToken token)
        {
            try
            {
                string rawData = await DownloadDataFromUrl("https://cdn.relewise.com/academy/productdata/googleshoppingfeed", token);

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

            // Parse XML data
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawData);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("g", "http://base.google.com/ns/1.0");

            // Extract product information
            XmlNodeList productNodes = xmlDoc.SelectNodes("//item");
            foreach (XmlNode productNode in productNodes)
            {
                string id = productNode.SelectSingleNode("g:id", nsManager).InnerText;
                string name = productNode.SelectSingleNode("title").InnerText;
                string listPrice = productNode.SelectSingleNode("g:price", nsManager).InnerText;
                string salesPrice = productNode.SelectSingleNode("g:sale_price", nsManager).InnerText;

                // Map fields to Product properties
                Product product = new Product(id);
                product.DisplayName = new Multilingual("en", name);
                product.ListPrice = new MultiCurrency(listPrice, 2);
                product.SalesPrice = new MultiCurrency(salesPrice, 2);

                // Add mapped product to the list
                products.Add(product);
            }

            return products;
        }

    }
}

