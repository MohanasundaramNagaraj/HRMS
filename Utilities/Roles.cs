namespace SparkHRMS.Utilities
{
    public static class Roles
    {
        public enum RoleType
        {
            SuperAdmin,
            Admin,
            Employee
        }

        public static List<RoleType> GetAllRoles { get; set; } = new List<RoleType>
        {
             RoleType.SuperAdmin,
             RoleType.Admin,
             RoleType.Employee
        };
    }
}
