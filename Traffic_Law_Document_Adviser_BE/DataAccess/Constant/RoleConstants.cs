using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Constant
{
    public static class RoleConstants
    {
        public const int UserValue = 1;
        public const int ExpertValue = 2;
        public const int AdminValue = 3;

        public const string User = "User";
        public const string Expert = "Expert";
        public const string Admin = "Admin";

        public static string ToRoleName(int v) =>
            v switch
            {
                AdminValue => Admin,
                ExpertValue => Expert,
                _ => User,   
            };

        public static int ToRoleValue(string name)
        {
            return name switch
            {
                Admin => AdminValue,
                Expert => ExpertValue,
                _ => UserValue
            };
        }
    }
}
