
using CF.VRent.Email.EmailSender.Clients;
using CF.VRent.Email.EmailSender.DebitNote;
using CF.VRent.Email.EmailSender.Payment;
using CF.VRent.Email.EmailSender.UserMgmt;
using CF.VRent.UserStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender
{
    public class EmailSenderFactory
    {
        /// <summary>
        /// Create instance of email sender of fee deduction failed
        /// </summary>
        /// <returns></returns>
        public static IFeeDeductionFailedSender CreateFeeDeductionFailedSender()
        {
            return new FeeDeductionFailedSender();
        }

        /// <summary>
        ///  Create instance of email sender of fee deduction sucess
        /// </summary>
        /// <returns></returns>
        public static IFeeDeductionSuccessSender CreateFeeDeductionSuccessSender()
        {
            return new FeeDeductionSuccessSender();
        }

        /// <summary>
        ///  Create instance of email sender of preauth cancellation failed
        /// </summary>
        /// <returns></returns>
        public static IPreauthCancelFailedSender CreatePreauthCancelFailedSender()
        {
            return new PreauthCancelFailedSender();
        }

        /// <summary>
        ///  Create instance of email sender of preauth cancellation success
        /// </summary>
        /// <returns></returns>
        public static IPreauthCancelSuccessSender CreatePreauthCancelSuccessSender()
        {
            return new PreauthCancelSuccessSender();
        }

        /// <summary>
        ///  Create instance of email sender of preauth completion failed
        /// </summary>
        /// <returns></returns>
        public static IPreauthCompletionFailedSender CreatePreauthCompletionFailedSender()
        {
            return new PreauthCompletionFailedSender();
        }

        /// <summary>
        /// Create instance of email sender of preauth failed
        /// </summary>
        /// <returns></returns>
        public static IPreauthFailedSender CreatePreauthFailedSender()
        {
            return new PreauthFailedSender();
        }

        /// <summary>
        /// Create instance of email sender of preauth sucess
        /// </summary>
        /// <returns></returns>
        public static IPreauthSuccessSender CreatePreauthSuccessSender()
        {
            return new PreauthSuccessSender();
        }

        /// <summary>
        /// Create instance of email sender of user mgmt
        /// </summary>
        /// <param name="currentStatus"></param>
        /// <returns></returns>
        public static IUserMgmtSender CreateUserMgmtSuccessSender(Dictionary<string, string[]> inputStatus, string currentStatus, UserStatusEntityCollection originalStatus)
        {
            return new UserMgmtSender(inputStatus, currentStatus, originalStatus);
        }

        /// <summary>
        /// Create instance of email sender of indirect fee deduction failed
        /// </summary>
        /// <returns></returns>
        public static IIndirectFeeDeductionFailedSender CreateIndirectFeeDeductionFailedSender()
        {
            return new IndirectFeeDeductionFailedSender();
        }

        /// <summary>
        /// Create instance of email sender of indirect fee deduction success
        /// </summary>
        /// <returns></returns>
        public static IIndirectFeeDeductionSuccessSender CreateIndirectFeeDeductionSuccessSender()
        {
            return new IndirectFeeDeductionSuccessSender();
        }

        /// <summary>
        /// Create instance of email sender of preauth completion success
        /// </summary>
        /// <returns></returns>
        public static IPreauthCompletionSuccessSender CreatePreauthCompletionSuccessSender()
        {
            return new PreauthCompletionSuccessSender();
        }

        /// <summary>
        /// Create instance of email sender of UserRegistration
        /// </summary>
        /// <returns></returns>
        public static IUserRegistrationSender CreateUserRegistrationSender()
        {
            return new UserRegistrationSender();
        }

        /// <summary>
        /// Create instance of email sender of indirect fee remainder
        /// </summary>
        /// <returns></returns>
        public static IIndirectFeeRemainder CreateIndirectFeeRemainderSender()
        {
            return new IndirectFeeRemainder();
        }

        /// <summary>
        /// Create instance of email sender of user transfer
        /// </summary>
        /// <param name="inputUserTransfer"></param>
        /// <returns></returns>
        public static IUserTransferSender CreateUserTransferSender(Dictionary<int, string[]> inputUserTransfer)
        {
            return new UserTransferSender(inputUserTransfer);
        }

        /// <summary>
        /// Create instance of ClientTerminal of user transfer
        /// </summary>
        /// <returns></returns>
        public static IClientTerminalSender CreateClientTerminalSender()
        {
            return new ClientTerminalSender();
        }

        public static IClientCreatedSender CreateClientCreatedSender()
        {
            return new ClientCreatedSender();
        }

        public static IPortalUserCreatedSender CreatePortalUserCreatedSender()
        {
            return new PortalUserCreatedSender();
        }

        public static IDebitNoteCreatedSender CreateDebitNoteCreatedSender()
        {
            return new DebitNoteCreatedSender();
        }
    }
}
