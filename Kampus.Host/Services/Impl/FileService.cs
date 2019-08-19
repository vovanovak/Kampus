using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Extensions;
using Kampus.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kampus.Host.Services.Impl
{
    internal class FileService : IFileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IReadOnlyList<FileModel>> UploadFilesToServer(HttpContext context)
        {
            var files = new List<FileModel>(context.Request.Form.Files.Count);

            foreach (var file in context.Request.Form.Files)
            {
                files.Add(await SaveFile(file));
            }

            return files;
        }

        public async Task<string> SaveImage(HttpContext context, IFormFile file)
        {
            var fileName = DateTime.Now.Ticks.ToString()
                .GetEncodedHash()
                .Replace("\\", "a")
                .Replace("/", "a")
                .Replace("+", "b");

            var ext = file.FileName.Substring(file.FileName.LastIndexOf("."));
            var absolutePath = _hostingEnvironment.WebRootPath + "/Images/" + fileName + ext;

            await SaveFile(file, absolutePath);

            var relativePath = "/Images/" + fileName + ext;
            return relativePath;
        }

        // TODO: Implement the method
        public Task RemoveFilesAsync(IReadOnlyCollection<int> fileIds)
        {
            return Task.CompletedTask;
        }

        private async Task<FileModel> SaveFile(IFormFile file)
        {
            var fileName = Convert.ToString(DateTime.Now.Ticks) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            var absolutePath = _hostingEnvironment.WebRootPath + "/Files/" + fileName;

            await SaveFile(file, absolutePath);

            return new FileModel()
            {
                RealFileName = fileName,
                FileName = file.FileName
            };
        }

        private static async Task SaveFile(IFormFile file, string absolutePath)
        {
            var content = new byte[file.Length];

            using (var fs = file.OpenReadStream())
            {
                await fs.ReadAsync(content, 0, (int)file.Length);
            }

            System.IO.File.WriteAllBytes(absolutePath, content);
        }

        public async Task<byte[]> Download(string path)
        {
            var absolutePath = _hostingEnvironment.WebRootPath + "/Files/" + path;
            return await System.IO.File.ReadAllBytesAsync(absolutePath);
        }
    }
}
