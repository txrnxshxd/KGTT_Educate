using KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.FilesAPI.Controllers
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

        [HttpGet("Download/{path}")]
        public async Task<ActionResult> DownloadFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return BadRequest();

            try
            {
                // ПРОБУЕМ СКАЧАТЬ ФАЙЛ
                // TRY TO DOWNLOAD FILE
                await _fileService.DownloadFileAsync(path, HttpContext.Response);
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
        public async Task<ActionResult> UploadFile(IFormFile file, [FromForm] string section)
        {
            try
            {
                string filePath = await _fileService.UploadFileAsync(file, section);

                string fileName = Path.GetFileName(filePath);

                string wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                string fullPath = Path.GetFullPath(wwwrootPath);

                return Ok(new { FileName = fileName, OriginalName = file.FileName, LocalFilePath = wwwrootPath, FullFilePath = fullPath });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete/{path}")]
        public async Task<ActionResult> DeleteFile(string path)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);

            await _fileService.DeleteFileAsync(fullPath);

            return Ok();
        }
    }
}
