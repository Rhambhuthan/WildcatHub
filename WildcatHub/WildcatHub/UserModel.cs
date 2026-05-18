namespace WildcatHub
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string SchoolID { get; set; } = "";
        public string FullName { get; set; } = "";
        public string SchoolEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
        public string VerificationStatus { get; set; } = "";
        public string ProofImagePath { get; set; } = "";
        public string RejectReasons { get; set; } = "";
        public string RejectCustomMessage { get; set; } = "";
    }
}