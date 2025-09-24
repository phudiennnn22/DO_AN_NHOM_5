using DOAN.Models;

namespace DOAN.Helpers
{
    public static class AdminHelper
    {
        public static bool IsAdmin(this ISession session)
        {
            var role = session.GetInt32("UserRole");
            return role == (int)UserRole.Admin;
        }

        public static bool IsManager(this ISession session)
        {
            var role = session.GetInt32("UserRole");
            return role == (int)UserRole.Manager || role == (int)UserRole.Admin;
        }

        public static bool IsCustomer(this ISession session)
        {
            var role = session.GetInt32("UserRole");
            return role == (int)UserRole.Customer;
        }

        public static UserRole GetUserRole(this ISession session)
        {
            var role = session.GetInt32("UserRole");
            return role.HasValue ? (UserRole)role.Value : UserRole.Customer;
        }
    }
}
