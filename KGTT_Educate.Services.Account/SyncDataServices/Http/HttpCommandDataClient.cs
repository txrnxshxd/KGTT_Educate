using KGTT_Educate.Services.Account.Models.Dto;
using System.Net.Http.Headers;
using System.Text.Unicode;

namespace KGTT_Educate.Services.Account.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> SendFile(IFormFile entity, string section)
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(entity.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(entity.ContentType);
            content.Add(fileContent, "file", entity.FileName);
            content.Add(new StringContent(section), "section");


            return await _httpClient.PostAsync($"{_configuration["FilesAPIUpload"]}", content);
        }

        public async Task<HttpResponseMessage> DeleteFile(string path)
        {
            return await _httpClient.DeleteAsync($"{_configuration["FilesAPIDelete"]}/{Uri.EscapeDataString(path)}");
        }
    }
}
