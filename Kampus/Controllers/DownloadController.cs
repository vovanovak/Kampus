using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Download(string path, string fileName)
        {
            try
            {
                //string path = Environment.CurrentDirectory;
                //path = path.Remove(path.LastIndexOf('\\'));
                //path += realName;


                var fs = System.IO.File.OpenRead(Server.MapPath("~/Files/" + path));
                return File(fs, "application/zip", fileName);
            }
            catch
            {
                throw new HttpException(404, "Couldn't find " + path);
            }
        }
    }
}
