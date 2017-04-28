using CF.VRent.UserStatus.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CF.VRent.Common;
using CF.VRent.Common.Entities.UserExt;

namespace CF.VRent.UserStatus
{
    public class UserStatusManager : IUserStatusManager
    {
        private UserStatusEntityCollection _status;
        public UserStatusEntityCollection Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
            }
        }

        
        public UserStatusManager()
            : this(null)
        {

        }

        [InjectionConstructor]
        public UserStatusManager(string binaryPattern)
        {
            _formatUserStatusFromFile();
            if (!String.IsNullOrWhiteSpace(binaryPattern))
            {
                string[] statusAndException = binaryPattern.Split('.');

                _convertToStatus(statusAndException[0]);

                if (statusAndException.Length > 1)
                {
                    _convertToStatusExtension(statusAndException[1]);
                }
            }
        }

        /// <summary>
        /// Extension binary pattern to entities
        /// </summary>
        /// <param name="extensionBinaryPattern"></param>
        private void _convertToStatusExtension(string extensionBinaryPattern)
        {
            var ca = extensionBinaryPattern.ToCharArray();
            var count = _status.Extensions.Count <= ca.Length ? _status.Extensions.Count : ca.Length;
            for (int i = 0; i < count; i++)
            {
                _status.Extensions[i].Value = ca[i].ToString().ToInt();
            }
        }

        /// <summary>
        /// Binary pattern to entities
        /// </summary>
        private void _convertToStatus(string binaryPattern)
        {
            var ca = binaryPattern.ToCharArray();
            var count = _status.Count <= ca.Length ? _status.Count : ca.Length;
            for (int i = 0; i < count; i++)
            {
                var val = ca[i].ToString();
                if (val == "0")
                {
                    _status[i].Value = val.ToInt();
                }
                else
                {
                    _status[i].Value = 1;
                }
            }
        }

        /// <summary>
        /// Get user status entities
        /// </summary>
        private void _formatUserStatusFromFile()
        {
            //var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Entities\UserExt\UserStatus.xml";
            //if (document == null)
            //{
            //    document = new XmlDocument();
            //    document.Load(filePath);
            //}
            var status = UserStatusContext.UserStatusDocument.SelectNodes("//Root/Status");
            var extensions = UserStatusContext.UserStatusDocument.SelectNodes("//Root/Extension");

            _status = new UserStatusEntityCollection(this._instantiatedProperties<UserStatusEntity>(status), 
                this._instantiatedProperties<UserStatusExtensionEntity>(extensions));
        }

        private List<T> _instantiatedProperties<T>(XmlNodeList nodes) where T : UserStatusEntity, new()
        {
             List<T> entities = new List<T>();

            foreach (XmlNode node in nodes)
            {
                if (entities.FirstOrDefault(r => r.Flag.Trim() == node.Attributes["flag"].Value.Trim()) != null)
                {
                    throw new Exception("Multiple flags in " + node.Attributes["name"].Value);
                }
                entities.Add(new T()
                { 
                    Flag = node.Attributes["flag"].Value,
                    Value = node.Attributes["value"].Value.ToInt(),
                    Name = node.Attributes["name"].Value
                });
            }

            return entities;
        }
    }
}
