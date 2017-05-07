using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class UniversityFaculty : DbEntity
    {
        
        public string Name { get; set; }

        public int? UniversityId { get; set; }
        public virtual University University { get; set; }
    }
}
