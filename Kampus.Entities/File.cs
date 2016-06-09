using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class File: DbEntity
    {
        public string FileName { get; set; }
        public string RealFileName { get; set; }
    }
}
