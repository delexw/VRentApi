using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.Entities
{
    /////web user and kemas user share the same User Entity
    ///// <summary>
    ///// User Entity
    ///// </summary>
    //[Serializable]
    //[DataContract]
    //public class ProxyUser
    //{
    //    [DataMember]
    //    public bool IsWebUser { get; set; }
    //    /// <summary>
    //    /// if the user is enabled the value should be 1
    //    /// </summary>
    //    [DataMember]
    //    public string Enabled { get; set; }

    //    /// <summary>
    //    /// id of current user
    //    /// </summary>
    //    [DataMember]
    //    public string ID { get; set; }

    //    /// <summary>
    //    /// person number of the user
    //    /// </summary>
    //    [DataMember]
    //    public string PNo { get; set; }

    //    /// <summary>
    //    /// last name of user
    //    /// </summary>
    //    [DataMember]
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// firstName of the user
    //    /// </summary>
    //    [DataMember]
    //    public string VName { get; set; }

    //    /// <summary>
    //    /// Department of user
    //    /// </summary>
    //    [DataMember]
    //    public string Department { get; set; }

    //    /// <summary>
    //    /// phone number of the user
    //    /// </summary>
    //    [DataMember]
    //    public string Phone { get; set; }

    //    /// <summary>
    //    /// business mail address
    //    /// </summary>
    //    [DataMember]
    //    public string Mail { get; set; }

    //    /// <summary>
    //    /// date of creation of user data
    //    /// </summary>
    //    [DataMember]
    //    public string CreateDate { get; set; }

    //    /// <summary>
    //    /// company of user
    //    /// </summary>
    //    [DataMember]
    //    public string Company { get; set; }

    //    /// <summary>
    //    /// responsible person of user, only information no reference to other user
    //    /// </summary>
    //    [DataMember]
    //    public string PersonInCharge { get; set; }

    //    /// <summary>
    //    /// mobile number of user
    //    /// </summary>
    //    [DataMember]
    //    public string PrivateMobileNumber { get; set; }

    //    /// <summary>
    //    /// bank account of user
    //    /// </summary>
    //    [DataMember]
    //    public string PrivateBankAccount { get; set; }

    //    /// <summary>
    //    /// private mail address of user
    //    /// </summary>
    //    [DataMember]
    //    public string PrivateEmail { get; set; }

    //    /// <summary>
    //    /// private address of user
    //    /// </summary>
    //    [DataMember]
    //    public string PrivateAddress { get; set; }

    //    //if the user is a web user, use this address as OfficeLocation field
    //    /// <summary>
    //    /// business address of user
    //    /// </summary>
    //    [DataMember]
    //    public string BusinessAddress { get; set; }

    //    /// <summary>
    //    /// Password of user
    //    /// </summary>
    //    [DataMember]
    //    public string Password { get; set; }

    //    /// <summary>
    //    /// ChangePwd
    //    /// </summary>
    //    [DataMember]
    //    public string ChangePwd { get; set; }

    //    /// <summary>
    //    /// User setting
    //    /// </summary>
    //    [DataMember]
    //    public ProxyUserSetting UserSetting { get; set; }

    //    /// <summary>
    //    /// User valid to date
    //    /// </summary>
    //    [DataMember]
    //    public string ValidTo { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [DataMember]
    //    public string Lic { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [DataMember]
    //    public string LicExpireDate { get; set; }

    //    /// <summary>
    //    /// int value if the lic is expired (0 or 1)
    //    /// </summary>
    //    [DataMember]
    //    public string LicExpired { get; set; }

    //    /// <summary>
    //    ///ture for false 
    //    /// </summary>
    //    [DataMember]
    //    public string LicExpiredSpecified { get; set; }

    //    /// <summary>
    //    /// Driver license Number
    //    /// </summary>
    //    [DataMember]
    //    public string LicenseNumber { get; set; }

    //    [DataMember]
    //    public string CurrentPassword { get; set; }

    //    [DataMember]
    //    public string UserName { get; set; }

    //    //newly added fields vr compaign
    //    //[User_JobTitle] nvarchar(50) NULL,
    //    //[User_OfficeLocation] nvarchar(100) NULL,
    //    //[UserLeadSource] navarchar(10) NOT NULL,
    //    //[User_IsAcceptAppointment] tinyint NULL,
    //    //[User_IsShareInfo] tinyint NULL,
    //    //[User_IsNoImmediateNeed] tinyint NULL,
    //    //--newly added fields
    //    [DataMember]
    //    public string UserJobTitle { get; set; }

    //    [DataMember]
    //    public string UserOfficeLocation { get; set; }

    //    [DataMember]
    //    public string UserLeadSource { get; set; }
        

    //    [DataMember]
    //    public string UserIsAcceptAppointment { get; set; }

    //    [DataMember]
    //    public string UserIsShareInfo { get; set; }

    //    [DataMember]
    //    public string UserIsNoImmediateNeed { get; set; }
    //}
}
