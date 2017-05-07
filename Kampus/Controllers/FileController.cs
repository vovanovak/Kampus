using Kampus.DAL.Security;
using Kampus.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class FileController : Controller
    {
        public ActionResult Download(string path, string fileName)
        {
            try
            {
                var fs = System.IO.File.OpenRead(Server.MapPath("~/Files/" + path));
                return File(fs, "application/zip", fileName);
            }
            catch
            {
                throw new HttpException(404, "Couldn't find " + path);
            }
        }
        
        public static string SaveImage(HttpContextBase context, HttpPostedFileBase file)
        {
            string filename = DateTime.Now.Ticks.ToString().GetEncodedHash().
                Replace("\\", "a").Replace("/", "a").Replace("+", "b");
            string ext = file.FileName.Substring(file.FileName.LastIndexOf("."));

            file.SaveAs(context.Server.MapPath("/Images/")
                        + filename + ext);

            return "/Images/" + filename + ext;
        }

        public static FileModel SaveFile(HttpContextBase context, HttpPostedFileBase file)
        {
            var fileName = Convert.ToString(DateTime.Now.Ticks) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            var path = Path.Combine(context.Server.MapPath("~/Files/"), fileName);
            file.SaveAs(path);

            return new FileModel() { RealFileName = fileName,
                                     FileName = file.FileName  };
        }

        public static List<FileModel> UploadFilesToServer(HttpContextBase context)
        {
            List<FileModel> files = new List<FileModel>();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                files.Add(SaveFile(context, context.Request.Files[i]));
            }

            return files;
        }
    }
}
