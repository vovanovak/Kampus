using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class DbEntity
    {
        public int Id { get; set; }

        public DbEntity()
        {
            Id = -1;
        }
    }
}
