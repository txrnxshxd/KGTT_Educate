﻿namespace KGTT_Educate.Services.Courses.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        Task<HttpResponseMessage> SendFile(IFormFile file, string section);
        Task<HttpResponseMessage> DeleteFile(string path);
    }
}
