using CF.VRent.Common.Entities.UserExt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class UserUtility
    {
        /// <summary>
        /// As the name mentioned
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RadomPassword(int length)
        {
            var generator = GetGenerator();

            if (generator.Enabled)
            {
                string[] token = new string[] { 
                "A","B","C","D","E","F","G","H","I","G","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                "a","b","c","p","e","f","g","s","i","u","k","z","x","n","o","d","q","r","h","t","g","v","w","m","y","l",
                "1","2","4","7","5","6","3","8","9","0"
            };

                var radom = new Random();
                StringBuilder pwd = new StringBuilder();

                for (int i = 0; i < generator.Length; i++)
                {
                    var t = radom.Next(token.Length);
                    pwd.Append(token[t]);
                }

                return pwd.ToString();
            }

            return null;
        }

        public static UserPasswordGenerator GetGenerator()
        {
            UserPasswordGenerator up = ConfigurationManager.GetSection("UserPasswordGenerator") as UserPasswordGenerator;
            return up;
        }
    }
}
