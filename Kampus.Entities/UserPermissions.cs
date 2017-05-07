using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class UserPermissions: DbEntity
    {
        public bool AllowToWriteOnMyWall { get; set; }
        public bool AllowToWriteComments { get; set; }
        public bool AllowToSendMeAMessage { get; set; }
    }
}
