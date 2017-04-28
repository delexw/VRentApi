using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using System.ServiceModel.Web;
using CF.VRent.Common.Entities;
using System.Diagnostics;

namespace CF.VRent.Entities.KemasWrapper
{
    public interface IKemasResponseValidator<TResponse,TError>
    {
        void Validate(TResponse kemasObj);
    }

    public class KemasResponseValidator<TResponse, TError> : IKemasResponseValidator<TResponse, TError>
    {
        private Type _error;

        public KemasResponseValidator()
        {
            _error = typeof(TError);
        }

        public void Validate(TResponse kemasObj)
        {
            var Error = kemasObj.GetType().GetProperty("Error");
            if (Error != null)
            {
                var ErrorValue = Convert.ChangeType(Error.GetValue(kemasObj, null), _error);

                if (ErrorValue != null)
                {
                    var errorType = ErrorValue.GetType();
                    if (errorType.GetProperty("ErrorCode").GetValue(ErrorValue, null).ToStr() != "E0000")
                    {
                        throw new WebFaultException<ReturnResult>(new ReturnResult()
                        {
                            Code = errorType.GetProperty("ErrorCode").GetValue(ErrorValue, null).ToStr(),
                            Message = String.Format("({0}){1}", new StackTrace().GetFrame(1).GetMethod().ToString(),
                                                    errorType.GetProperty("ErrorMessage").GetValue(ErrorValue, null).ToStr()),
                            Type = ResultType.KEMAS
                        }, System.Net.HttpStatusCode.BadRequest);
                    }
                }
            }
        }
    }
}
