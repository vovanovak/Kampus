using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class TaskCategory : DbEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public virtual List<TaskSubcat> Subcategories { get; set; }
    }
}
