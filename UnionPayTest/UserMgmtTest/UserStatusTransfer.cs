using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionPayTest.TestHeaders;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common;
using Microsoft.Practices.Unity;
using CF.VRent.UserStatus.Interfaces;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class UserStatusTransfer : TestHeader
    {
        [TestMethod]
        public void FormatUserStatusTo00000000()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });

            base.OutputMessage(manager.Status);
            base.OutputMessage(manager.Status.Extensions);

            Assert.IsNotNull(manager.Status);
        }

        [TestMethod]
        public void BinaryUserStatusEqual00000000()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });

            base.OutputMessage(manager.Status);

            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "00000000");
        }

        [TestMethod]
        public void UpdateUserStatusTo10000000()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });
            manager.Status["1"].Value = 1;
            base.OutputMessage(manager.Status);
            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "10000000");
        }

        [TestMethod]
        public void UpdateUserStatusViaIndexerTo10000000()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });
            var s = manager.Status["1"];
            s.Value = 1;
            base.OutputMessage(manager.Status);
            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "10000000");
        }

        [TestMethod]
        public void ConvertBinaryPatter10000001ToEntities()
        {
            //UserStatusManager manager = new UserStatusManager("10000001");
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer")
                .Resolve<IUserStatusManager>(new ParameterOverride("binaryPattern", "10000008"));

            base.OutputMessage(manager.Status);
            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "10000008");
        }

        [TestMethod]
        public void ConvertBinaryPatter100000011ToEntitiesMuchFlag()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer")
                .Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "100000078" } });
            base.OutputMessage(manager.Status);
            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "10000008");
        }

        [TestMethod]
        public void ConvertBinaryPattern10001ToEntitiesLessFlag()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer")
                .Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "10005" } });
            base.OutputMessage(manager.Status);
            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "10001000");
        }

        [TestMethod]
        public void AfterChangeFlagToDifferentValue()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });

            manager.Status["1"].Value = 1;
            manager.Status["2"].Value = 1;

            try
            {
                manager.Status["2"].Flag = "8";
                Assert.Fail("No exception to validate flag");
            }
            catch
            {
                Assert.IsTrue(true);
            }
            finally
            {
                base.OutputMessage(manager.Status);
            }
        }
        [TestMethod]
        public void AfterChangeFlag11000000ToSameValue()
        {
            IUserStatusManager manager = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverrides { { "binaryPattern", "" } });

            manager.Status["1"].Value = 1;
            manager.Status["2"].Value = 1;

            base.OutputMessage(manager.Status);

            manager.Status["2"].Flag = "2";

            Assert.AreEqual(manager.Status.BinaryPattern.Substring(0, 8), "12000000");
        }
    }
}
