using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities
{
    public class BusinessArea
    {
        public string Branch { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// represent the area of corportate firm code
        /// </summary>
        public string CorporateArea { get; set; }

        /// <summary>
        /// represent the area of private firm code
        /// </summary>
        public string PrivateArea { get; set; }
    }
}
