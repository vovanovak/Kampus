using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<FileModel> UploadFilesToServer(HttpContext context)
        {
            return context.Request.Form.Files.Select(f => SaveFile(f)).ToList();
        }

        public string SaveImage(HttpContext context, IFormFile file)
        {
            string fileName = DateTime.Now.Ticks.ToString().GetEncodedHash().
                Replace("\\", "a").Replace("/", "a").Replace("+", "b");
            string ext = file.FileName.Substring(file.FileName.LastIndexOf("."));
            string relativePath = "/Images/" + fileName + ext;
            string absolutePath = _hostingEnvironment.WebRootPath + "/Images/" + fileName + ext;

            SaveFile(file, absolutePath);

            return relativePath;
        }

        public FileModel SaveFile(IFormFile file)
        {
            var fileName = Convert.ToString(DateTime.Now.Ticks) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string absolutePath = _hostingEnvironment.WebRootPath + "/Files/" + fileName;

            SaveFile(file, absolutePath);

            return new FileModel()
            {
                RealFileName = fileName,
                FileName = file.FileName
            };
        }

        private static void SaveFile(IFormFile file, string absolutePath)
        {
            var content = new byte[file.Length];
            using (var fs = file.OpenReadStream())
            {
                fs.Read(content, 0, (int)file.Length);
            }
            System.IO.File.WriteAllBytes(absolutePath, content);
        }

        public byte[] Download(string path)
        {
            var absolutePath = _hostingEnvironment.WebRootPath + "/Files/" + path;
            return System.IO.File.ReadAllBytes(absolutePath);
        }
    }
}
