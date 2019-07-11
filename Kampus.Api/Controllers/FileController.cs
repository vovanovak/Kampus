using Kampus.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Api.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public ActionResult Download(string path, string fileName)
        {
            try
            {
                var bytes = _fileService.Download(path);
                return File(bytes, "application/zip", fileName);
            }
            catch
            {
                return new NotFoundObjectResult("Couldn't find " + path);
            }
        }
    }
}
