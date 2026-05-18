namespace WildcatHub
{
    public static class SessionManager
    {
        // COMMON
        public static bool IsLoggedIn { get; set; } = false;
        public static string CurrentRole { get; set; } = "";

        // STUDENT SESSION
        public static int UserID { get; set; } = 0;
        public static string StudentName { get; set; } = "";
        public static string SchoolID { get; set; } = "";
        public static string SchoolEmail { get; set; } = "";

        // OLD CODE COMPATIBILITY
        public static string FullName
        {
            get => StudentName;
            set => StudentName = value;
        }

        public static string VerificationStatus { get; set; } = "Verified";

        // ADMIN / NAS SESSION
        public static int AdminID { get; set; } = 0;
        public static string AdminFullName { get; set; } = "";
        public static string OfficeEmail { get; set; } = "";
        public static string AdminRole { get; set; } = "";
        public static int LabID { get; set; } = 0;
        public static string LabName { get; set; } = "";
        public static string LabCode { get; set; } = "";

        public static void SetStudentSession(int userId, string fullName, string schoolId, string schoolEmail)
        {
            Clear();

            IsLoggedIn = true;
            CurrentRole = "Student";

            UserID = userId;
            StudentName = fullName;
            SchoolID = schoolId;
            SchoolEmail = schoolEmail;
            VerificationStatus = "Verified";
        }

        public static void SetAdminSession(
            int adminId,
            string adminFullName,
            string officeEmail,
            string adminRole,
            int labId,
            string labName,
            string labCode)
        {
            Clear();

            IsLoggedIn = true;
            CurrentRole = "Admin";

            AdminID = adminId;
            AdminFullName = adminFullName;
            OfficeEmail = officeEmail;
            AdminRole = adminRole;
            LabID = labId;
            LabName = labName;
            LabCode = labCode;
        }

        public static void Clear()
        {
            IsLoggedIn = false;
            CurrentRole = "";

            UserID = 0;
            StudentName = "";
            SchoolID = "";
            SchoolEmail = "";
            VerificationStatus = "Verified";

            AdminID = 0;
            AdminFullName = "";
            OfficeEmail = "";
            AdminRole = "";
            LabID = 0;
            LabName = "";
            LabCode = "";
        }
    }
}