using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class TypeofJourneyStrategy : ITypeofJourneyStrategy
    {
        /// <summary>
        /// Get the strategy value by orginal value
        /// </summary>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        public int GetValueFromKemasValue(int originalValue)
        {
            if (originalValue == 1 || originalValue == 2)
            {
                return 1;
            }
            else if (originalValue == 3)
            {
                return 0;
            }
            return originalValue;
        }

        /// <summary>
        /// Get the strategy value from api input value
        /// </summary>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        public int GetValueFromApiInputValue(int originalValue)
        {
            if (originalValue == 1)
            {
                return BookingType.Business_Private.GetValue();
            }
            return BookingType.Private.GetValue();
        }
    }
}
