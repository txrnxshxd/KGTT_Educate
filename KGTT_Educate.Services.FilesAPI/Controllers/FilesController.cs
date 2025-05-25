using KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace KGTT_Educate.Services.FilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IContentTypeProvider _contentTypeProvider;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpGet("Get/{path}")]
        public IActionResult GetFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return BadRequest();

            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);

            if (_contentTypeProvider.TryGetContentType(wwwrootPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(wwwrootPath, contentType, enableRangeProcessing: true);
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
