using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace CF.VRent.DataAccessProxy.Payment.UnionPayProxy
{
    public class TransactionUPManager
    {
        public static void Run(Action action)
        {
            //Transcation
            TransactionOptions to = new TransactionOptions();
            to.IsolationLevel = IsolationLevel.ReadCommitted;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, to))
            {
                try
                {
                    action();
                    ts.Complete();
                }
                catch
                {
                    ts.Dispose();
                    throw;
                }
                finally
                {
                    ts.Dispose();
                }
            }
        }

        /// <summary>
        /// Run a part of db transaction
        /// </summary>
        /// <param name="action"></param>
        public static void RunPart(Action action)
        {
            //Transcation
            TransactionOptions to = new TransactionOptions();
            to.IsolationLevel = IsolationLevel.ReadCommitted;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, to))
            {
                try
                {
                    action();
                    ts.Complete();
                }
                catch
                {
                    ts.Dispose();
                    throw;
                }
            }
        }
    }
}