using CF.VRent.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.DataAccessProxyTest
{
    [TestClass]
    public class CommonUtilityUnitTest
    {
        [TestMethod]
        public void LoadProxyBookingPriceTestMethod()
        {
            string chinese = "翻译此页 翻译此页";
            string english = "Tom bob";
            string mixed = "翻译此页 Tom";

            bool test1 = BookingUtility.IsSingleByteString(chinese);
            bool test2 = BookingUtility.IsSingleByteString(english);
            bool test3 = BookingUtility.IsSingleByteString(mixed);

            Assert.IsTrue(test1 == false && test2 == true && test3 == false, "check single-byte and double-byte");

        }
    }
}
