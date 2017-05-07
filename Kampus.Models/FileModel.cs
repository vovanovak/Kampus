using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Models
{
    public class FileModel:Entity
    {
        public string RealFileName { get; set; }
        public string FileName { get; set; }
        
        public string GetExtension()
        {
            return RealFileName.Substring(RealFileName.LastIndexOf('.'));
        }

        public bool IsImage()
        {
            var ext = GetExtension();

            return ext == ".jpg" || ext == ".png";
        }
    }
}
