using KGTT_Educate.Services.Courses.Data;
using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Common;
using System.Reflection.Emit;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("Download/{fileName}")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(Path.GetExtension(fileName));

            string directory = isMedia ? "Media" : "Files";
            // ПОЛНЫЙ ПУТЬ
            // FULL PATH
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directory, fileName);

            try
            {
                // ПРОБУЕМ СКАЧАТЬ ФАЙЛ
                // TRY TO DOWNLOAD FILE
                await _fileService.DownloadFileAsync(fullPath, HttpContext.Response);
                Console.WriteLine(fullPath);
                return new EmptyResult();
            }
            catch (FileNotFoundException)
            {
                // ЕСЛИ НЕ НАШЛИ, КИДАЕМ NF
                // IF FILE WASN'T FOUND, THROW NOT FOUND
                return NotFound();
            }
        }

        [HttpPost("Upload")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest();

            try
            {
                // ПОЛУЧАЕМ РАСШИРЕНИЕ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                // GET PROVIDED FILE EXTENSION
                string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedFileExtensions.fileExtensions.Contains(fileExt) && !AllowedFileExtensions.mediaExtensions.Contains(fileExt))
                    return BadRequest(new { Message = $"Вы не можете загрузить файл с расширением {fileExt}" });

                bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(fileExt);

                string filePath = await _fileService.UploadFileAsync(file, isMedia);

                string fileName = Path.GetFileName(filePath);

                // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                // RELATIVE PATH
                var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                return Ok(new { WwwrootPath = wwwrootPath, FileName = fileName, IsMedia = isMedia });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete/{fileName}")]
        public async Task<ActionResult> Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(Path.GetExtension(fileName));

            string directory = isMedia ? "Media" : "Files";

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directory, fileName);

            await _fileService.DeleteFileAsync(fullPath);

            return Ok(new {FilePath = fullPath, FileName = fileName });
        }
    }
}
