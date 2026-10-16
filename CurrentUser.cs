using System;

namespace Inventory_Management_System
{
    public static class CurrentUser
    {
        public static string Username { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public static bool IsAdmin => string.Equals(Role, "Yönetici (Admin)", StringComparison.OrdinalIgnoreCase) ||
                                      string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);

        public static bool IsCashier => string.Equals(Role, "Kasiyer", StringComparison.OrdinalIgnoreCase);

        public static bool IsWarehouse => string.Equals(Role, "Depo Görevlisi", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(Role, "Depocu", StringComparison.OrdinalIgnoreCase);

        public static void Clear()
        {
            Username = string.Empty;
            FullName = string.Empty;
            Role = string.Empty;
        }
    }
}
