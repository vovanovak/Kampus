using System.Collections.Generic;
using System.Threading.Tasks;
using Kampus.Models;
using Microsoft.AspNetCore.Http;

namespace Kampus.Host.Services
{
    public interface IFileService
    {
        Task<byte[]> Download(string path);
        Task<IReadOnlyList<FileModel>> UploadFilesToServer(HttpContext context);
        Task<string> SaveImage(HttpContext context, IFormFile file);
        Task RemoveFilesAsync(IReadOnlyCollection<int> fileIds);
    }
}
