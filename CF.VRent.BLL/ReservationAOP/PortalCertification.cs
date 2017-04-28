using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common;
using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using CF.VRent.Entities.DataAccessProxy;
using System.Web;
using Microsoft.Practices.Unity;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL.ReservationAOP
{
    /// <summary>
    /// Identity emnu
    /// </summary>
    public enum PortalIdentity
    {
        Login,
        UserMgmt,
        Booking,
        TermsCondition,
        Transaction
    }

    /// <summary>
    /// Interface for certification
    /// </summary>
    public interface IPortalCertification
    {
        IMethodInvocation Input { get; }
        UserExtension User { get; }
        IPortalCertificationValidator Validator { get; }

        bool Certificate(PortalIdentity type);
        IMethodReturn ReturnException();
    }

    /// <summary>
    /// Implementation for certification
    /// </summary>
    public class PortalCertification : IPortalCertification
    {
        public PortalCertification(IMethodInvocation input, IPortalCertificationValidator validator)
        {
            _input = input;
            _validator = validator;
        }

        /// <summary>
        /// Validate the permission of current user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Certificate(PortalIdentity type)
        {
            //Portal Authentication
            //Ignore property set method
            if (!_input.MethodBase.IsSpecialName)
            {
                var userProperty = _input.Target.GetType().GetProperty("UserInfo");
                ProxyUserSetting loginUser = new ProxyUserSetting();
                if (HttpContext.Current != null &&
                    HttpContext.Current.Session != null &&
                    HttpContext.Current.Session["UserSetting"] != null &&
                    type != PortalIdentity.Login)
                {
                    //Get user from session
                    loginUser = HttpContext.Current.Session["UserSetting"] as ProxyUserSetting;
                    if (userProperty != null)
                    {
                        userProperty.SetValue(_input.Target, loginUser, null);
                    }
                }
                else
                {
                    if (userProperty != null)
                    {
                        loginUser = userProperty.GetValue(_input.Target, null) as ProxyUserSetting;
                    }
                }

                if (loginUser != null || (loginUser == null && type == PortalIdentity.TermsCondition))
                {
                    var loginUserExtension = new UserFactory().CreateEntity(loginUser);
                    _user = loginUserExtension;
                    //Validate permission
                    if (Validator != null)
                    {
                        return Validator.Validate(_input, type, _user);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Return the method exception
        /// </summary>
        /// <param name="input"></param>
        /// <param name="loginUserExtension"></param>
        /// <returns></returns>
        public virtual IMethodReturn ReturnException()
        {
            return _input.CreateExceptionMethodReturn(new WebFaultException<ReturnResult>(new Common.Entities.ReturnResult()
            {
                Code = MessageCode.CVCE000007.ToString(),
                Message = MessageCode.CVCE000007.GetDescription(),
                Type = MessageCode.CVCE000007.GetMessageType()
            }, System.Net.HttpStatusCode.BadRequest));
        }

        private IMethodInvocation _input;
        public IMethodInvocation Input
        {
            get { return _input; }
        }

        private UserExtension _user;
        public UserExtension User
        {
            get { return _user; }
        }

        private IPortalCertificationValidator _validator;
        public IPortalCertificationValidator Validator
        {
            get { return _validator; }
        }
    }

    /// <summary>
    /// Context for certification
    /// </summary>
    public class PortalCertificationContext
    {
        public static IPortalCertification GetCertification(IMethodInvocation input, IPortalCertificationValidator validator)
        {
            IPortalCertification cer = UnityHelper.GetUnityContainer("PortalCertificationContainer").Resolve<IPortalCertification>(new ParameterOverride("input", input), new ParameterOverride("validator", validator));
            return cer;
        }
    }

    /// <summary>
    /// ProtalAuthentication
    /// </summary>
    public abstract class ProtalAuthentication
    {
        public IMethodReturn Authenticate(IMethodInvocation input, IPortalCertificationValidator validator, PortalIdentity identity, IPortalAuthenticationValidator authValidator)
        {
            var certification = PortalCertificationContext.GetCertification(input, validator);
            var ret = certification.Certificate(
                identity);
            //Certification result
            if (!ret)
            {
                return certification.ReturnException();
            }
            else
            {
                if (authValidator != null)
                {
                    //Authentication result
                    if (!authValidator.Validate(input, certification))
                    {
                        return certification.ReturnException();
                    }
                }
                return null;
            }
        }
    }
}
