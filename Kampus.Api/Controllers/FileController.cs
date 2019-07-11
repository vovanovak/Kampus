using Kampus.Application.Extensions;
using Kampus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kampus.Api.Controllers
{
    public class FileController : Controller
    {
        public ActionResult Download(string path, string fileName)
        {
            try
            {
                var fs = System.IO.File.OpenRead(this.Server.MapPath("~/Files/" + path));
                return File(fs, "application/zip", fileName);
            }
            catch
            {
                return new NotFoundObjectResult("Couldn't find " + path);
            }
        }

        public static string SaveImage(HttpContext context, IFormFile file)
        {
            string filename = DateTime.Now.Ticks.ToString().GetEncodedHash().
                Replace("\\", "a").Replace("/", "a").Replace("+", "b");
            string ext = file.FileName.Substring(file.FileName.LastIndexOf("."));

            file.SaveAs(context.Server.MapPath("/Images/")
                        + filename + ext);

            return "/Images/" + filename + ext;
        }

        public static FileModel SaveFile(HttpContext context, IFormFile file)
        {
            var fileName = Convert.ToString(DateTime.Now.Ticks) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            var path = Path.Combine(context.Server.MapPath("~/Files/"), fileName);
            file.SaveAs(path);

            return new FileModel()
            {
                RealFileName = fileName,
                FileName = file.FileName
            };
        }

        public static List<FileModel> UploadFilesToServer(HttpContext context)
        {
            List<FileModel> files = new List<FileModel>();

            for (int i = 0; i < .Count; i++)
            {
                files.Add(SaveFile(context, context.Request.Form.Files[i]));
            }

            return context.Request.Form.Files.Select(f => SaveFile(context, f)).ToList();
        }
    }
}
