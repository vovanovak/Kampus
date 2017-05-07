using Kampus.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kampus.DAL
{
    public class UnitOfWorkResolver
    {
        private static IUnitOfWork _work;
        private static object _lockObj = new object();
        public static IUnitOfWork UnitOfWork
        {
            get
            {
                if (_work != null)
                    return _work;

                lock (_lockObj)
                {
                    if (_work == null)
                    {
                        Volatile.Write(ref _work, new UnitOfWork());
                    }
                }

                return _work;
            }
        }
    }
}
