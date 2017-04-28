using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UPSDK.Entities
{
    public class UnionPayState
    {
        public int[] TimeSpan { get; set; }

        public UnionPayState()
            : this("")
        { }

        public UnionPayState(string resCode)
        {
            if (resCode == "05")
            {
                TimeSpan = new int[] { 300000, 600000, 1800000, 3600000, 7200000 };
            }
            else
            {
                TimeSpan = new int[] { 0, 1000, 2000, 4000, 8000, 16000, 32000, 64000 };
            }
        }
    }
}
