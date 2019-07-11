using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Models
{
    public class UniversityModel: Entity
    {
        public string Name { get; set; }

        public List<UniversityFacultyModel> Faculties { get; set; }
    }
}
