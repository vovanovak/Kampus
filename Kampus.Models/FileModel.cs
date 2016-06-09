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
        public string Extension { get; set; }
        public bool IsImage { get; set; }
    }
}
