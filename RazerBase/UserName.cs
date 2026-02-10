using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazerBase
{
    public static class UserName
    {
        public static string GetUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1].ToLower();

        public static string GetUserNameWithDomain = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();

    }
}
