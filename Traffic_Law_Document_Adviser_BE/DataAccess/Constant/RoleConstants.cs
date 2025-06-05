using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Constant
{
    public static class RoleConstants
    {
        public const int AdminValue = 2;
        public const int UserValue = 1;

        public const string Admin = "Admin";
        public const string User = "User";

        public static string ToRoleName(int v) =>
            v == AdminValue ? Admin : User;

        public static int ToRoleValue(string name) =>
            name.Equals(Admin, System.StringComparison.OrdinalIgnoreCase)
                ? AdminValue
                : UserValue;
    }
}
