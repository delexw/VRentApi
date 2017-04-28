using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace CF.VRent.Common
{
    public class VrentApplicationException : ApplicationException
    {
        public const string LoggingTitle = Constants.KemasExceptionTitle;

        private string _errorCode;
        private string _errorMessage;
        private ResultType _type;
        private int _retCode = 255;

        private Guid _id;
        private string _stackTrace;

        public VrentApplicationException(ReturnResult ret)
            : this(ret.Code,ret.Message,ret.Type,ret.Success)
        {
        }

        public VrentApplicationException(string errorCode, string errorMessage, ResultType category)
            : base()
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _type = category;
            _id = Guid.NewGuid();
            _stackTrace = Environment.StackTrace;
        }



        public VrentApplicationException(string errorCode, string errorMessage, ResultType category,int retCode)
            : base()
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _type = category;
            _retCode = retCode;
            _id = Guid.NewGuid();
            _stackTrace = Environment.StackTrace;
        }

        [SecuritySafeCritical]
        protected VrentApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _id = Guid.NewGuid();
        }

        public VrentApplicationException(string errorCode, string errorMessage, ResultType category, int retCode, Exception innerException)
            : base(errorMessage, innerException)
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _type = category;
            _retCode = retCode;

            _id = Guid.NewGuid();
            _stackTrace = Environment.StackTrace;
        }


        public VrentApplicationException(string errorCode, string errorMessage, ResultType category, Exception innerException)
            : base(errorMessage, innerException)
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _type = category;

            _id = Guid.NewGuid();
            _stackTrace = Environment.StackTrace;
        }

        public string ErrorCode
        {
            get { return _errorCode; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public ResultType Category
        {
            get { return _type; }
        }

        public int ReturnCode
        {
            get { return _retCode; }
        }

        public string StackTrace {
            get { return _stackTrace; }
        }

        public Guid ID
        {
            get { return _id; }
        }


    }
}
