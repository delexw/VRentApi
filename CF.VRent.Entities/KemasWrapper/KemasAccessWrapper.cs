using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Practices.Unity;

namespace CF.VRent.Entities.KemasWrapper
{
    public class KemasAccessWrapper
    {
        public static string CommonKemasErrorCode = "10000";
        public static string CommonKemasErrorMsg = "Kemas returns a null object";
        public static string UnexpectedKemasErrorCode = "10001";
        public static string UnexpectedKemasErrorMsg = "Kemas throws an unexpected exception.Msg:{0}-StackTrace:{1}";
        public static string KemasTimeoutErrorCode = "10002";
        public static string KemasTimeoutErrorMsg = "Kemas Timeout Exception. Msg:{0}-StackTrace:{1}";
        public static string KemasCommunicationErrorCode = "10003";
        public static string KemasCommunicationErrorMsg = "Kemas Communication Exception. Msg:{0}-StackTrace:{1}";

        public static string KemasLogPattern = "ErrorCode:{0},ErrorMessage:{1},Json:{2},Operation:{3}";
        #region helper Method
        public static T InnerTryCatchInvoker<T>(Func<T> function, object client, string methodName)
        {
            try
            {
                LogInfor.WriteInfo("Kemas Server Initialized", "InvokeMethod:" + methodName, "KemasAccessWrapper");
                T r = function();
                KemasNullGuard<T>(r, methodName);
                return r;
            }
            catch (TimeoutException exception)
            {
                ((ICommunicationObject)client).Abort();
                VrentApplicationException vae = new VrentApplicationException(KemasTimeoutErrorCode, string.Format(KemasTimeoutErrorMsg, exception.Message, exception.StackTrace), ResultType.KEMAS, exception);
                LogInfor.WriteInfo(vae.ErrorCode, vae.ErrorMessage, "KemasAccessWrapper");
                throw vae;
            }
            catch (CommunicationException exception)
            {
                ((ICommunicationObject)client).Abort();
                VrentApplicationException vae = new VrentApplicationException(KemasCommunicationErrorCode, string.Format(KemasCommunicationErrorMsg, exception.Message, exception.StackTrace), ResultType.KEMAS, exception);
                LogInfor.WriteInfo(vae.ErrorCode, vae.ErrorMessage, "KemasAccessWrapper");
                throw vae;
            }
            catch (VrentApplicationException vae)
            {
                ((ICommunicationObject)client).Abort();
                LogInfor.WriteInfo(vae.ErrorCode, vae.ErrorMessage, "KemasAccessWrapper");
                throw new VrentApplicationException(vae.ErrorCode, vae.ErrorMessage, vae.Category, vae);
            }
            catch (Exception ex)
            {
                VrentApplicationException vae = new VrentApplicationException(UnexpectedKemasErrorCode, string.Format(UnexpectedKemasErrorMsg, ex.Message, ex.StackTrace), ResultType.KEMAS, ex);
                LogInfor.WriteInfo(vae.ErrorCode, vae.ErrorMessage, "KemasAccessWrapper");
                throw vae;
            }
            finally
            {
                if (client != null)
                {
                    if (((ICommunicationObject)client).State != CommunicationState.Faulted)
                    {
                        ((ICommunicationObject)client).Close();
                    }
                    ((IDisposable)client).Dispose();
                    client = null;
                }
                LogInfor.WriteInfo("Kemas Server Disposed", "InvokeMethod:" + methodName, "KemasAccessWrapper");
            }
        }

        public static void KemasNullGuard<T>(T kemasObj, string methodName)
        {

            string jsonObj = SerializedHelper.JsonSerialize<T>(kemasObj);
            LogInfor.WriteInfo(Constants.KemasDebuggingInfoTitle + "--" + methodName, jsonObj, methodName);

            if (kemasObj == null)
            {
                string errorMsg = string.Format(KemasLogPattern, CommonKemasErrorCode, CommonKemasErrorMsg, jsonObj, methodName);
                throw new VrentApplicationException(CommonKemasErrorCode, errorMsg, ResultType.KEMAS, new Exception(methodName));
            }
        }
        #endregion

        /// <summary>
        /// Kemas user2 api proxy with error handling
        /// </summary>
        /// <returns></returns>
        public static IKemasUserAPI CreateKemasUserAPI2Instance()
        {
            return UnityHelper.GetUnityContainer(UnityHelper.KemasApiWapperContainer).Resolve<IKemasUserAPI>();
        }

        /// <summary>
        /// Kemas config api proxy with error handling
        /// </summary>
        /// <returns></returns>
        public static IKemasConfigsAPI CreateKemasConfigAPIInstance()
        {
            return UnityHelper.GetUnityContainer(UnityHelper.KemasApiWapperContainer).Resolve<IKemasConfigsAPI>();
        }

        /// <summary>
        /// Create a validator
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <returns></returns>
        public static IKemasResponseValidator<TResponse, TError> CreateKemasValidatorInstance<TResponse, TError>()
        {
            return new KemasResponseValidator<TResponse, TError>();
        }

        /// <summary>
        /// Create a extension api instance
        /// </summary>
        /// <returns></returns>
        public static IKemasExtensionAPI CreateKemasExtensionAPIInstance()
        {
            return UnityHelper.GetUnityContainer(UnityHelper.KemasApiWapperContainer).Resolve<IKemasExtensionAPI>();
        }

        /// <summary>
        /// Create a reservation api instance
        /// </summary>
        /// <returns></returns>
        public static IKemasReservation CreateKemasReservationAPIInstance()
        {
            return UnityHelper.GetUnityContainer(UnityHelper.KemasApiWapperContainer).Resolve<IKemasReservation>();
        }
    }


}
