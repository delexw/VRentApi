using CF.VRent.Log;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class IntercepterUtility
    {
        /// <summary>
        /// Log the method in and input arguments
        /// </summary>
        /// <param name="input"></param>
        public static void MethodIn(IMethodInvocation input)
        {
            try
            {
                LogInfor.WriteInfo(input.MethodBase.DeclaringType.FullName + "." + input.MethodBase.Name + " In", input.Arguments.ObjectToJson(), "");
            }
            catch (Exception ex)
            {
                LogInfor.WriteError("", ex.ToStr(), "");
            }
        }

        /// <summary>
        /// Log the method out and return value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void MethodOut(IMethodInvocation input, IMethodReturn output)
        {
            try
            {
                LogInfor.WriteInfo(input.MethodBase.DeclaringType.FullName + "." + input.MethodBase.Name + " Out", output.ReturnValue.ObjectToJson(), "");
            }
            catch (Exception ex)
            {
                LogInfor.WriteError("", ex.ToStr(), "");
            }
        }
    }
}
