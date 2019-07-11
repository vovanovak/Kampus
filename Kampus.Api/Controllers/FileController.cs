using Kampus.Application.Extensions;
using Kampus.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Api.Controllers
{
    // TODO: Move logic to dedicated service
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Download(string path, string fileName)
        {
            try
            {
                var absolutePath = _hostingEnvironment.WebRootPath + "/Files/" + path;
                var bytes = System.IO.File.ReadAllBytes(absolutePath);
                return File(bytes, "application/zip", fileName);
            }
            catch
            {
                return new NotFoundObjectResult("Couldn't find " + path);
            }
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
            string absolutePath = _hostingEnvironment.WebRootPath + "/Images/" + fileName;

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

        public List<FileModel> UploadFilesToServer(HttpContext context)
        {
            return context.Request.Form.Files.Select(f => SaveFile(f)).ToList();
        }
    }
}
