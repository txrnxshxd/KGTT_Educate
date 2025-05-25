using System.Net.Http;

namespace KGTT_Educate.Services.Courses.SyncDataServices.Http
{
    public class HttpReadOnlyDataClient : IReadOnlyDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpReadOnlyDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(Stream FileStream, string ContentType)> GetFile(string path)
        {
            var response = await _httpClient.GetAsync($"{_configuration["FilesAPIGet"]}/{Uri.EscapeDataString(path)}");

            response.EnsureSuccessStatusCode();

            var fileStream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            return (fileStream, contentType);
        }
    }
}
