using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDAL
{
    public class ShardedResults<T> where T : IComparable<T>
    {
        public List<T> Results;
        public List<string> ErrorConnectionStrings=new List<string>();
    }
}
