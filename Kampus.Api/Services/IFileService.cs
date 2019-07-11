using Kampus.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Kampus.Api.Services
{
    public interface IFileService
    {
        byte[] Download(string path);
        List<FileModel> UploadFilesToServer(HttpContext context);
        FileModel SaveFile(IFormFile file);
        string SaveImage(HttpContext context, IFormFile file);
    }
}
