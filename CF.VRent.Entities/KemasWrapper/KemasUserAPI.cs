using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public class KemasUserAPI : IDisposable
    {
        public findUser_Response findUser(string userId)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<findUser_Response>(() =>
            {
                return client.findUser(userId);

            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public findUser2Response findUser2(string userId, string sessionId)
        {
            WSKemasUserPortTypeClient client = new WSKemasUserPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<findUser2Response>(() =>
            {
                return client.findUser2(userId, sessionId);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public string updateUser(string userId, UserData userData)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<string>(() =>
            {
                return client.updateUser(userId, userData);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public updateUser2Response updateUser2(updateUser2Request request)
        {
            WSKemasUserPortTypeClient client = new WSKemasUserPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<updateUser2Response>(() =>
            {

                return client.updateUser2(request);

            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public StdResponse forgotPassword(string mail, string lang)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<StdResponse>(() =>
            {
                return client.forgotPassword(mail, lang);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public Right[] getRights(string userId)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<Right[]>(() =>
            {
                return client.getRights(userId);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public Driver[] findAllDrivers(string userId)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<Driver[]>(() =>
            {
                return client.findAllDrivers(userId);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public UserList getUsers(string userId)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<UserList>(() =>
            {
                return client.getUsers(userId);
            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public getUsers2Response getUsers2(getUsers2Request request)
        {
            WSKemasUserPortTypeClient client = new WSKemasUserPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<getUsers2Response>(() =>
            {
                return client.getUsers2(request);
            }, client, MethodInfo.GetCurrentMethod().Name);

        }

        public getRolesResponse getRoles(string sessionID)
        {
            WSKemasUserPortTypeClient client = new WSKemasUserPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker<getRolesResponse>(() =>
            {

                return client.getRoles(sessionID);

            }, client, MethodInfo.GetCurrentMethod().Name);
        }

        public void Dispose()
        {
            //Nothing
        }
    }
}