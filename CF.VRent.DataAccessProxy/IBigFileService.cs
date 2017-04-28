using CF.VRent.Common.Entities;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBigFileService" in both code and config file together.
    [ServiceContract]
    public interface IBigFileService
    {
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void SendEmailWithAttachments(EmailProxyParameter paras);
    }
}
