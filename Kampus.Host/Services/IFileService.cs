using System.Collections.Generic;
using Kampus.Models;
using Microsoft.AspNetCore.Http;

namespace Kampus.Host.Services
{
    public interface IFileService
    {
        byte[] Download(string path);
        List<FileModel> UploadFilesToServer(HttpContext context);
        FileModel SaveFile(IFormFile file);
        string SaveImage(HttpContext context, IFormFile file);
    }
}
