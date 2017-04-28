using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionPayTest.TestHeaders.AggregateEntity
{
    public class KemasError : IAggregateRoot
    {
        public string errorcode { get; set; }
        public string msg { get; set; }
    }
}
