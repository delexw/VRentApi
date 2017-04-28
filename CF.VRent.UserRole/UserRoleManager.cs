using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CF.VRent.Common;
using Microsoft.Practices.Unity;
using CF.VRent.UserRole.Interfaces;
using CF.VRent.Common.Entities.UserExt;

namespace CF.VRent.UserRole
{
    /// <summary>
    /// Manage user role
    /// </summary>
    public class UserRoleManager : IUserRoleManager
    {
        private UserRoleEntityCollection _roles;
        public UserRoleEntityCollection Roles
        {
            get
            {
                return _roles;
            }

        }

        [InjectionConstructor]
        public UserRoleManager()
        {
            _init();
        }

        private void _init()
        {
            //var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Entities\UserExt\UserRole.xml";
            //XmlDocument document = new XmlDocument();
            //document.Load(filePath);
            var roles = UserRoleContext.UserRoleDocument.SelectNodes("//UserRole/Role");

            List<UserRoleEntity> entities = new List<UserRoleEntity>();

            foreach (XmlNode node in roles)
            {
                if (entities.FirstOrDefault(r => r.Key.Trim() == node.Attributes["key"].Value.Trim()) != null)
                {
                    throw new Exception("Multiple keys in " + node.Attributes["key"].Value);
                }

                //Associated Kemas roles
                List<KemasRoleEntity> kemsRole = new List<KemasRoleEntity>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element && child.Attributes["enabled"].Value.ToBool())
                    {
                        kemsRole.Add(new KemasRoleEntity()
                        {
                            Enable = child.Attributes["enabled"].Value.ToBool(),
                            Name = child.Attributes["name"].Value.ToStr(),
                            ID = child.Attributes["id"].Value.ToStr(),
                            IsDefault = child.Attributes["isDefault"].Value.ToBool()
                        });
                    }
                }

                if (node.Attributes["enabled"].Value.ToBool())
                {
                    entities.Add(new UserRoleEntity()
                    {
                        Enable = node.Attributes["enabled"].Value.ToBool(),
                        Key = node.Attributes["key"].Value.ToStr(),
                        KemasRole = kemsRole.ToArray()
                    });
                }
            }

            _roles = new UserRoleEntityCollection(entities);
        }
    }
}
