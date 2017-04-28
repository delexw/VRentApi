using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UPSDK
{
    public interface IUnionPayUtilsMethod
    {
        /// <summary>
        /// refund
        /// </summary>
        /// <returns></returns>
        ReturnResult ReturnGoodsConsume();
        /// <summary>
        /// Bind card
        /// </summary>
        /// <returns></returns>
        ReturnResult OpenUnionPayCard();
        /// <summary>
        /// Cancel card
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ReturnResult CancelUnionPayCardBinding(string token);
        /// <summary>
        /// Send binding sms
        /// </summary>
        /// <returns></returns>
        ReturnResult SendVerificationSMS();
        /// <summary>
        /// Send preauth sms
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ReturnResult SendPreauthorizaitonSMS(string token);
        /// <summary>
        /// preauth
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ReturnResult PreAuthorize(string token);
        /// <summary>
        /// cancel preauth
        /// </summary>
        /// <returns></returns>
        ReturnResult CancelPreAuthorization();
        /// <summary>
        /// finish preauth
        /// </summary>
        /// <returns></returns>
        ReturnResult FinishPreAuthorization();
        /// <summary>
        /// cancel fee
        /// </summary>
        /// <returns></returns>
        ReturnResult CancelConsume();
        /// <summary>
        /// direct fee
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ReturnResult Consume(string token);
        /// <summary>
        /// check card status
        /// </summary>
        /// <returns></returns>
        ReturnResult CheckUnionPayCardStatus();
        /// <summary>
        /// check payment status
        /// </summary>
        /// <returns></returns>
        ReturnResult CheckPaymentStatus();
        /// <summary>
        /// send request to union pay
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ReturnResult SendRequest(Dictionary<string, string> param);
    }
}
