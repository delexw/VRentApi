using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public interface IKemasUserAPI
    {
        findUser2Response findUser2(string UserID, string SessionID);
        getUsers2Response getUsers2(getUsers2Request getUsers2Request);
        updateUser2Response updateUser2(updateUser2Request updateUser2Request);
    }

    public class KemasUserAPIProxy : IKemasUserAPI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public findUser2Response findUser2(string UserID, string SessionID)
        {
            var kemasObj = new KemasUserAPI().findUser2(UserID, SessionID);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<findUser2Response, Error>();
            validator.Validate(kemasObj);

            return kemasObj;
        }

        public getUsers2Response getUsers2(getUsers2Request getUsers2Request)
        {
            var kemasObj = new KemasUserAPI().getUsers2(getUsers2Request);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getUsers2Response, Error>();
            validator.Validate(kemasObj);

            return kemasObj;
        }

        public getRolesResponse getRoles(string SessionID)
        {
            var kemasObj = new KemasUserAPI().getRoles(SessionID);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getRolesResponse, Error>();
            validator.Validate(kemasObj);

            return kemasObj;
        }

        public updateUser2Response updateUser2(updateUser2Request updateUser2Request)
        {
            var kemasObj = new KemasUserAPI().updateUser2(updateUser2Request);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<updateUser2Response, Error>();
            validator.Validate(kemasObj);

            return kemasObj;
        }

        /// <summary>
        /// kemas error
        /// </summary>
        /// <param name="kemasObj"></param>
        /// <param name="UserID"></param>
        private void _validate(object kemasObj)
        {
            var Error = kemasObj.GetType().GetProperty("Error");
            if (Error != null)
            {
                var ErrorValue = Error.GetValue(kemasObj, null) as Error;
                if (ErrorValue != null && ErrorValue.ErrorCode != "E0000")
                {
                    throw new WebFaultException<ReturnResult>(new ReturnResult()
                    {
                        Code = ErrorValue.ErrorCode,
                        Message = ErrorValue.ErrorMessage,
                        Type = ResultType.KEMAS
                    }, System.Net.HttpStatusCode.BadRequest);
                }
            }
        }
    }
}
