using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class University : DbEntity
    {
       
        public string Name { get; set; }

        public virtual List<UniversityFaculty> Faculties { get; set; }
    }
}
