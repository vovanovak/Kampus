using System.Threading.Tasks;
using Kampus.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<IActionResult> Download(string path, string fileName)
        {
            try
            {
                var bytes = await _fileService.Download(path);
                return File(bytes, "application/zip", fileName);
            }
            catch
            {
                return new NotFoundObjectResult("Couldn't find " + path);
            }
        }
    }
}
