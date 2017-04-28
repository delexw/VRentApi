using CF.VRent.Contract;
using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    public class FapiaoPreferenceImpl : AbstractBLL,IFapiaoPreference
    {
        public FapiaoPreferenceImpl(ProxyUserSetting userInfo)
            : base(userInfo) 
        {
        }

        public ProxyFapiaoPreference[] GetAllFapiaoPreference(string uid)
        {
            FapiaoPreferenceServiceClient FaPiaoPreference = new FapiaoPreferenceServiceClient();
            return FaPiaoPreference.GetAllFapiaoPreference(uid).ToArray();
        }

        public ProxyFapiaoPreference GetFapiaoPreferenceDetail(string uuid)
        {
            FapiaoPreferenceServiceClient FaPiaoPreference = new FapiaoPreferenceServiceClient();
            return FaPiaoPreference.GetFapiaoPreferenceDetail(uuid);
        }

        public void DeleteFapiaoPreference(string uuid)
        {
            FapiaoPreferenceServiceClient FaPiaoPreference = new FapiaoPreferenceServiceClient();
            FaPiaoPreference.DeleteFapiaoPreference(uuid);
        }

        public ProxyFapiaoPreference UpdateFapiaoPreference(ProxyFapiaoPreference fp)
        {
            //vrent bug201 in case when booking user has changed a fapiao preference, create a new one fapiao preference.
            fp.State = (int)CommonState.Deleted;


            ProxyFapiaoPreference newFP = new ProxyFapiaoPreference();
            newFP.ID = Guid.NewGuid().ToString();
            newFP.UserID = UserInfo.ID;
            newFP.FapiaoType = (int)FapiaoType.RentalFee;

            //from FE
            newFP.AddresseeName = fp.AddresseeName;
            newFP.CustomerName = fp.CustomerName;
            newFP.MailAddress = fp.MailAddress;
            newFP.MailPhone = fp.MailPhone;
            newFP.MailType = FapiaoDeliverType.Express.ToString();

            newFP.State = (int)CommonState.Active;
            newFP.CreatedBy = Guid.Parse(UserInfo.ID);
            newFP.CreatedOn = DateTime.Now;


            FapiaoPreferenceServiceClient FaPiaoPreference = new FapiaoPreferenceServiceClient();
            return FaPiaoPreference.UpdateFapiaoPreference(fp, newFP);
        }


        public ProxyFapiaoPreference SaveFapiaoPreference(ProxyFapiaoPreference fp)
        {
            fp.ID = Guid.NewGuid().ToString();
            fp.UserID = UserInfo.ID;
            fp.FapiaoType = (int)FapiaoType.RentalFee;
            fp.State = (int)CommonState.Active; //0:active, 1:delete
            fp.CreatedOn = DateTime.Now;
            fp.CreatedBy = Guid.Parse(UserInfo.ID);
            fp.MailType = FapiaoDeliverType.Express.ToString();

            FapiaoPreferenceServiceClient FaPiaoPreference = new FapiaoPreferenceServiceClient();
            return FaPiaoPreference.CreateFapiaoPreference(fp);
        }
    }
}
