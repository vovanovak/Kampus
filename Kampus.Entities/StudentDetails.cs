using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class StudentDetails : DbEntity
    {
        public int? UniversityId { get; set; }
        public virtual University University { get; set; }

        public int? FacultyId { get; set; }
        public virtual UniversityFaculty Faculty { get; set; }

        public int Course { get; set; }
    }
}
