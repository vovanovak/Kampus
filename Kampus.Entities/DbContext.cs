using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class DbContextSingleton
    {
        private static KampusContext context = null;

        public static KampusContext Context
        {
            get
            {
                if (context == null)
                    context = new KampusContext();
                return context;
            }
        }
    }
}
