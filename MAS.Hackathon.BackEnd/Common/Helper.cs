using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MAS.Hackathon.BackEnd.Common
{
    public static class Helper
    {
        public static async Task<string> SaveImage(string base64, string path, string extension)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var separator = ",";
            if (base64.Contains(separator))
                base64 = base64.Split(separator)[1];

            var fileName = $"{Guid.NewGuid()}-{DateTime.Now:yyyyMMddHHMMss}{extension}";

            byte[] fileBytes = Convert.FromBase64String(base64);
            await File.WriteAllBytesAsync(Path.Combine(path, fileName), fileBytes);

            return fileName;
        }

        public static async Task<string> CallEndpointPrediction(string content, string url, IHttpClientFactory httpClientFactory)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"),
                Headers =
                {
                    {"Accept", "*/*"}
                }
            };

            using var httpClient = httpClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }
    }
}
