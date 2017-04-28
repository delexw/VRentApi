using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class OrderItemConstant
    {
        private List<string> _groups;

        public OrderItemConstant()
        {
            _groups = new List<string>();

            _groups.Add("RENTALFEE");
            _groups.Add("FINEFEE");
            _groups.Add("INDIRECTFEE");
        }


        public List<String> Groups 
        { 
            get
            {
                return _groups;
            }
        }

    }
}
