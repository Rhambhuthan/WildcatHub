using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Collections.Generic;


namespace WildcatHub
{
    public partial class frmLogin : Form
    {
        private string pendingSchoolID = "";
        private string pendingEmail = "";
        private int pendingUserID = 0;
        private string pendingFirstName = "";
        private string pendingLastName = "";
        private string pendingMI = "";
        private string pendingPassword = "";

        private string currentVerificationCode = "";
        private DateTime currentVerificationExpiry;

        private string resetSchoolID = "";
        private string resetEmail = "";
        private string currentResetCode = "";
        private DateTime currentResetExpiry;

        private System.Windows.Forms.Timer countdownTimer = null!;
        private int timeLeft = 120;
        private readonly HashSet<Button> pressedNeoButtons = new();
        private readonly HashSet<Button> styledNeoButtons = new();

        public frmLogin()
        {
            InitializeComponent();

            txtPassword_Login.UseSystemPasswordChar = true;
            txtPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            SetupTimer();
            SetupInitialState();

            ApplyRoundedCorners(glassLogin, 25);
            ApplyRoundedCorners(glassCreate, 25);
            ApplyRoundedCorners(glassVerify, 25);
            ApplyRoundedCorners(glassForgot, 25);
            ApplyRoundedCorners(glassResetVerify, 25);
            ApplyRoundedCorners(glassNewPassword, 25);

            ApplyNeumorphismButton(btnLogin);
            ApplyNeumorphismButton(btnCreateAccount);
            ApplyNeumorphismButton(btnVerify);
            ApplyNeumorphismButton(btnSendResetCode);
            ApplyNeumorphismButton(btnVerifyResetCode);
            ApplyNeumorphismButton(btnSaveNewPassword);

            ApplyNeumorphismButton(btnBackFromForgot);
            ApplyNeumorphismButton(btnBackToForgot);
            ApplyNeumorphismButton(btnBackToResetVerify);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            ApplyRoundedCorners(glassLogin, 25);
            ApplyRoundedCorners(glassCreate, 25);
            ApplyRoundedCorners(glassVerify, 25);
            ApplyRoundedCorners(glassForgot, 25);
            ApplyRoundedCorners(glassResetVerify, 25);
            ApplyRoundedCorners(glassNewPassword, 25);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            if (glassLogin.Width > 0) ApplyRoundedCorners(glassLogin, 25);
            if (glassCreate.Width > 0) ApplyRoundedCorners(glassCreate, 25);
            if (glassVerify.Width > 0) ApplyRoundedCorners(glassVerify, 25);
            if (glassForgot.Width > 0) ApplyRoundedCorners(glassForgot, 25);
            if (glassResetVerify.Width > 0) ApplyRoundedCorners(glassResetVerify, 25);
            if (glassNewPassword.Width > 0) ApplyRoundedCorners(glassNewPassword, 25);
        }


        private void linkReturnVerify_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            glassVerify.Visible = false;
            glassCreate.Visible = true;
        }

        private void SetupTimer()
        {
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
        }



        private GraphicsPath GetNeoRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }



        private void SetupInitialState()
        {
            glassLogin.Visible = true;
            glassCreate.Visible = false;
            glassVerify.Visible = false;
            glassForgot.Visible = false;
            glassResetVerify.Visible = false;
            glassNewPassword.Visible = false;

            lblResend.Visible = false;
            lblTimer.Text = "Time left: 120s";
            lblResetResend.Visible = false;
            lblResetTimer.Text = "Time left: 120s";
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, d, d), 180, 90);
            path.AddArc(new Rectangle(control.Width - d, 0, d, d), 270, 90);
            path.AddArc(new Rectangle(control.Width - d, control.Height - d, d, d), 0, 90);
            path.AddArc(new Rectangle(0, control.Height - d, d, d), 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            timeLeft--;

            if (glassVerify.Visible)
            {
                lblTimer.Text = $"Time left: {timeLeft}s";
                if (timeLeft <= 0)
                {
                    countdownTimer.Stop();
                    lblTimer.Text = "Code expired";
                    lblResend.Visible = true;
                }
            }
            else if (glassResetVerify.Visible)
            {
                lblResetTimer.Text = $"Time left: {timeLeft}s";
                if (timeLeft <= 0)
                {
                    countdownTimer.Stop();
                    lblResetTimer.Text = "Code expired";
                    lblResetResend.Visible = true;
                }
            }
        }



        private void ClearVerifyFields()
        {
            txtCode.Clear();
            lblTimer.Text = "Time left: 120s";
            lblResend.Visible = false;
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            string enteredID = txtSchoolID_Login.Text.Trim();
            string enteredPass = txtPassword_Login.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredID) || string.IsNullOrWhiteSpace(enteredPass))
            {
                MessageBox.Show("Please enter your School ID/Office Email and Password.",
                    "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                // CHECK ADMIN / NAS FIRST
                string adminQuery = @"
SELECT 
    A.AdminID,
    A.AdminFullName,
    A.OfficeEmail,
    A.Role,
    A.LabID,
    A.IsActive,
    L.LabName,
    L.LabCode
FROM AdminCredentials AS A
LEFT JOIN Laboratories AS L ON A.LabID = L.LabID
WHERE A.OfficeEmail = ? AND A.[Password] = ?";

                using (OleDbCommand adminCmd = new OleDbCommand(adminQuery, conn))
                {
                    adminCmd.Parameters.AddWithValue("@p1", enteredID);
                    adminCmd.Parameters.AddWithValue("@p2", enteredPass);

                    using OleDbDataReader adminReader = adminCmd.ExecuteReader();

                    if (adminReader != null && adminReader.Read())
                    {
                        bool isActive = adminReader["IsActive"] != DBNull.Value &&
                                        Convert.ToBoolean(adminReader["IsActive"]);

                        if (!isActive)
                        {
                            MessageBox.Show("Admin account is deactivated. Please contact the system administrator.",
                                "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int adminId = Convert.ToInt32(adminReader["AdminID"]);
                        string adminFullName = adminReader["AdminFullName"]?.ToString() ?? "Admin";
                        string officeEmail = adminReader["OfficeEmail"]?.ToString() ?? enteredID;
                        string adminRole = adminReader["Role"]?.ToString() ?? "NAS";
                        int labId = adminReader["LabID"] != DBNull.Value ? Convert.ToInt32(adminReader["LabID"]) : 0;
                        string labName = adminReader["LabName"]?.ToString() ?? "";
                        string labCode = adminReader["LabCode"]?.ToString() ?? "";

                        SessionManager.SetAdminSession(
                            adminId,
                            adminFullName,
                            officeEmail,
                            adminRole,
                            labId,
                            labName,
                            labCode
                        );

                        frmAdminDashboard admin = new frmAdminDashboard();
                        admin.Show();
                        Hide();
                        return;
                    }
                }

                // CHECK STUDENT
                string userQuery = @"
SELECT 
    UserID,
    SchoolID,
    FullName,
    SchoolEmail,
    IsActive
FROM Users
WHERE SchoolID = ? AND [Password] = ?";

                using (OleDbCommand userCmd = new OleDbCommand(userQuery, conn))
                {
                    userCmd.Parameters.AddWithValue("@p1", enteredID);
                    userCmd.Parameters.AddWithValue("@p2", enteredPass);

                    using OleDbDataReader userReader = userCmd.ExecuteReader();

                    if (userReader != null && userReader.Read())
                    {
                        bool isActive = userReader["IsActive"] != DBNull.Value &&
                                        Convert.ToBoolean(userReader["IsActive"]);

                        if (!isActive)
                        {
                            MessageBox.Show("Account deactivated. Please approach the admin.",
                                "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int userId = Convert.ToInt32(userReader["UserID"]);
                        string fullName = userReader["FullName"]?.ToString() ?? "";
                        string schoolId = userReader["SchoolID"]?.ToString() ?? "";
                        string schoolEmail = userReader["SchoolEmail"]?.ToString() ?? "";

                        SessionManager.SetStudentSession(
                            userId,
                            fullName,
                            schoolId,
                            schoolEmail
                        );

                        frmUserDashboard user = new frmUserDashboard();
                        user.Show();
                        Hide();
                        return;
                    }
                }

                MessageBox.Show("Incorrect School ID/Office Email or Password.",
                    "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during login:\n\n" + ex.Message,
                    "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        



        private void btnGoToCreate_Click(object sender, EventArgs e)
        {
            ClearLoginFields();
            glassLogin.Visible = false;
            glassCreate.Visible = true;
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            ClearCreateFields();
            ClearVerifyFields();
            glassLogin.Visible = true;
            glassCreate.Visible = false;
            glassVerify.Visible = false;
        }



        private void btnBackToCreate_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            ClearVerifyFields();
            glassLogin.Visible = false;
            glassCreate.Visible = true;
            glassVerify.Visible = false;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            string schoolID = txtSchoolID.Text.Trim();
            string schoolEmail = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (!Regex.IsMatch(schoolID, @"^24-\d{4}-\d{3}$"))
            {
                MessageBox.Show("Invalid School ID format.\nFormat must be 24-####-###",
                    "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(schoolEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please complete all required fields.",
                    "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.",
                    "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Make sure that the credentials inputted are accurate.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string recordQuery = @"
SELECT UserID, FullName, SchoolEmail, IsActive
FROM Users
WHERE SchoolID = ?";

                int userId = 0;
                string recordFullName = "";
                string recordEmail = "";
                bool isActive = false;

                using (OleDbCommand recordCmd = new OleDbCommand(recordQuery, conn))
                {
                    recordCmd.Parameters.AddWithValue("@p1", schoolID);
                    using OleDbDataReader reader = recordCmd.ExecuteReader();

                    if (reader == null || !reader.Read())
                    {
                        MessageBox.Show("School ID is not enrolled in the system.",
                            "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    userId = reader["UserID"] != DBNull.Value
                        ? Convert.ToInt32(reader["UserID"])
                        : 0;
                    recordFullName = reader["FullName"]?.ToString() ?? "";
                    recordEmail = reader["SchoolEmail"]?.ToString() ?? "";
                    isActive = reader["IsActive"] != DBNull.Value &&
                               Convert.ToBoolean(reader["IsActive"]);
                }

                if (isActive)
                {
                    MessageBox.Show("An account with this School ID already exists.",
                        "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!string.Equals(schoolEmail, recordEmail, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("The School Email entered does not match the enrolled student record.",
                        "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                pendingUserID = userId;
                pendingSchoolID = schoolID;
                pendingEmail = schoolEmail;
                pendingFirstName = recordFullName;
                pendingLastName = "";
                pendingMI = "";
                pendingPassword = password;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating account:\n\n" + ex.Message,
                    "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                currentVerificationCode = new Random().Next(100000, 999999).ToString();
                currentVerificationExpiry = DateTime.Now.AddSeconds(120);

                string displayName = pendingFirstName;

                EmailService.SendVerificationCode(pendingEmail, displayName, currentVerificationCode);

                MessageBox.Show("Verification code sent to your school email.",
                    "Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);

                glassCreate.Visible = false;
                glassVerify.Visible = true;

                txtCode.Clear();
                timeLeft = 120;
                lblTimer.Text = "Time left: 120s";
                lblResend.Visible = false;
                countdownTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email sending failed.\n\n" + ex.Message,
                    "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void btnVerify_Click(object sender, EventArgs e)
        {
            string inputCode = txtCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputCode))
            {
                MessageBox.Show("Please enter the verification code.", "Verify Code",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DateTime.Now > currentVerificationExpiry)
            {
                countdownTimer.Stop();
                lblTimer.Text = "Code expired";
                lblResend.Visible = true;
                MessageBox.Show("The verification code has expired. Please request a new code.", "Verify Code",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.Equals(inputCode, currentVerificationCode, StringComparison.Ordinal))
            {
                MessageBox.Show("Invalid verification code.", "Verify Code",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string updateQuery = @"
UPDATE Users
SET [Password] = ?, IsActive = True
WHERE UserID = ? AND SchoolID = ? AND SchoolEmail = ? AND IsActive = False";

                using OleDbCommand cmd = new OleDbCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@p1", pendingPassword);
                cmd.Parameters.AddWithValue("@p2", pendingUserID);
                cmd.Parameters.AddWithValue("@p3", pendingSchoolID);
                cmd.Parameters.AddWithValue("@p4", pendingEmail);
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                {
                    MessageBox.Show("Unable to activate account. The student may already be active or the enrolled record changed.",
                        "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                countdownTimer.Stop();
                MessageBox.Show("Account created successfully!", "Create Account",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                pendingUserID = 0;
                pendingSchoolID = pendingEmail = pendingFirstName = "";
                pendingLastName = pendingMI = pendingPassword = "";
                currentVerificationCode = "";
                currentVerificationExpiry = DateTime.MinValue;

                ClearCreateFields();
                ClearVerifyFields();
                ClearLoginFields();

                glassVerify.Visible = false;
                glassCreate.Visible = false;
                glassLogin.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating account:\n\n" + ex.Message,
                    "Verify Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void lblResend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pendingEmail)) return;

            try
            {
                currentVerificationCode = new Random().Next(100000, 999999).ToString();
                currentVerificationExpiry = DateTime.Now.AddSeconds(120);

                string displayName = pendingFirstName;

                EmailService.SendVerificationCode(pendingEmail, displayName, currentVerificationCode);

                txtCode.Clear();
                timeLeft = 120;
                lblTimer.Text = "Time left: 120s";
                lblResend.Visible = false;
                countdownTimer.Start();

                MessageBox.Show("A new verification code has been sent.",
                    "Resend Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resending code:\n\n" + ex.Message,
                    "Resend Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void lblForgotPassword_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearLoginFields();
            glassLogin.Visible = false;
            glassForgot.Visible = true;
        }

        private void btnBackFromForgot_Click(object sender, EventArgs e)
        {
            txtForgotSchoolID.Clear();
            txtForgotEmail.Clear();
            glassForgot.Visible = false;
            glassLogin.Visible = true;
        }


        private void btnSendResetCode_Click(object sender, EventArgs e)
        {
            string schoolID = txtForgotSchoolID.Text.Trim();
            string schoolEmail = txtForgotEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(schoolID) || string.IsNullOrWhiteSpace(schoolEmail))
            {
                MessageBox.Show("Please enter your School ID and School Email.",
                    "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string displayName = "";

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE SchoolID = ? AND SchoolEmail = ?";
                using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@p1", schoolID);
                    checkCmd.Parameters.AddWithValue("@p2", schoolEmail);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar() ?? 0);

                    if (count == 0)
                    {
                        MessageBox.Show("No account found matching that School ID and Email.",
                            "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string nameQuery = "SELECT FullName FROM Users WHERE SchoolID = ? AND SchoolEmail = ?";
                using (OleDbCommand nameCmd = new OleDbCommand(nameQuery, conn))
                {
                    nameCmd.Parameters.AddWithValue("@p1", schoolID);
                    nameCmd.Parameters.AddWithValue("@p2", schoolEmail);
                    displayName = nameCmd.ExecuteScalar()?.ToString() ?? schoolID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message);
                return;
            }

            try
            {
                resetSchoolID = schoolID;
                resetEmail = schoolEmail;
                currentResetCode = new Random().Next(100000, 999999).ToString();
                currentResetExpiry = DateTime.Now.AddSeconds(120);

                EmailService.SendVerificationCode(resetEmail, displayName, currentResetCode);

                MessageBox.Show("A reset code has been sent to your school email.",
                    "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Information);

                glassForgot.Visible = false;
                glassResetVerify.Visible = true;

                txtResetCode.Clear();
                timeLeft = 120;
                lblResetTimer.Text = "Time left: 120s";
                lblResetResend.Visible = false;
                countdownTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email sending failed.\n\n" + ex.Message,
                    "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnVerifyResetCode_Click(object sender, EventArgs e)
        {
            string inputCode = txtResetCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputCode))
            {
                MessageBox.Show("Please enter the reset code.", "Verify Reset",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.Equals(inputCode, currentResetCode, StringComparison.Ordinal))
            {
                MessageBox.Show("Invalid reset code.", "Verify Reset",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DateTime.Now > currentResetExpiry)
            {
                countdownTimer.Stop();
                lblResetTimer.Text = "Code expired";
                lblResetResend.Visible = true;
                MessageBox.Show("The reset code has expired.",
                    "Verify Reset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            countdownTimer.Stop();
            glassResetVerify.Visible = false;
            glassNewPassword.Visible = true;
        }

        private void btnBackToForgot_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            txtResetCode.Clear();
            glassResetVerify.Visible = false;
            glassForgot.Visible = true;
        }

        private void lblResetResend_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(resetEmail)) return;

            try
            {
                currentResetCode = new Random().Next(100000, 999999).ToString();
                currentResetExpiry = DateTime.Now.AddSeconds(120);

                EmailService.SendVerificationCode(resetEmail, resetSchoolID, currentResetCode);

                txtResetCode.Clear();
                timeLeft = 120;
                lblResetTimer.Text = "Time left: 120s";
                lblResetResend.Visible = false;
                countdownTimer.Start();

                MessageBox.Show("A new reset code has been sent.",
                    "Resend Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resending code:\n\n" + ex.Message,
                    "Resend Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackToResetVerify_Click(object sender, EventArgs e)
        {
            txtNewPassword.Clear();
            txtConfirmNewPassword.Clear();
            glassNewPassword.Visible = false;
            glassResetVerify.Visible = true;
        }

        private void btnSaveNewPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text.Trim();
            string confirmNewPassword = txtConfirmNewPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                MessageBox.Show("Please enter and confirm your new password.",
                    "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Passwords do not match.",
                    "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string updateQuery = "UPDATE Users SET [Password] = ? WHERE SchoolID = ? AND SchoolEmail = ?";
                using OleDbCommand cmd = new OleDbCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@p1", newPassword);
                cmd.Parameters.AddWithValue("@p2", resetSchoolID);
                cmd.Parameters.AddWithValue("@p3", resetEmail);
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    MessageBox.Show("Password reset failed. Account not found.",
                        "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Password reset successfully!",
                    "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Information);

                resetSchoolID = resetEmail = currentResetCode = "";
                currentResetExpiry = DateTime.MinValue;

                txtForgotSchoolID.Clear();
                txtForgotEmail.Clear();
                txtResetCode.Clear();
                txtNewPassword.Clear();
                txtConfirmNewPassword.Clear();

                glassNewPassword.Visible = false;
                glassLogin.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password:\n\n" + ex.Message,
                    "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void chkShowPasswordLogin_CheckedChanged(object? sender, EventArgs e)
        {
            txtPassword_Login.UseSystemPasswordChar = !chkShowPasswordLogin.Checked;
        }

        private void chkShowPasswordCreate_CheckedChanged(object? sender, EventArgs e)
        {
            bool hide = !chkShowPasswordCreate.Checked;
            txtPassword.UseSystemPasswordChar = hide;
            txtConfirmPassword.UseSystemPasswordChar = hide;
        }


        private void ClearLoginFields()
        {
            txtSchoolID_Login.Clear();
            txtPassword_Login.Clear();
            chkShowPasswordLogin.Checked = false;
            txtPassword_Login.UseSystemPasswordChar = true;
        }

        private void ClearCreateFields()
        {
            txtSchoolID.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void linkCreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearLoginFields();
            glassLogin.Visible = false;
            glassCreate.Visible = true;
            }


        private void linkBackToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            countdownTimer.Stop();
            ClearCreateFields();
            ClearVerifyFields();
            glassCreate.Visible = false;
            glassVerify.Visible = false;
            glassForgot.Visible = false;
            glassResetVerify.Visible = false;
            glassNewPassword.Visible = false;
            glassLogin.Visible = true;
        }


        private void ApplyNeumorphismButton(Button btn)
        {
            if (styledNeoButtons.Contains(btn))
                return;

            styledNeoButtons.Add(btn);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.ForeColor = Color.White;

            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedNeoButtons.Add(btn);
                    btn.Invalidate();
                }
            };

            btn.MouseUp += (s, e) =>
            {
                pressedNeoButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.MouseLeave += (s, e) =>
            {
                pressedNeoButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.Paint += (s, e) =>
            {
                if (s is not Button b) return;

                bool isPressed = pressedNeoButtons.Contains(b);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using GraphicsPath path = GetNeoRoundedPath(rect, 18);

                b.Region = new Region(path);

                Color baseColor = isPressed
                    ? Color.FromArgb(184, 140, 25)   // darker yellow when clicked
                    : Color.FromArgb(212, 168, 45);  // normal yellow

                Color lightEdge = isPressed
                    ? Color.FromArgb(240, 210, 120)
                    : Color.FromArgb(255, 235, 170);

                Color darkEdge = isPressed
                    ? Color.FromArgb(120, 85, 10)
                    : Color.FromArgb(160, 120, 20);

                using SolidBrush fillBrush = new SolidBrush(baseColor);
                e.Graphics.FillPath(fillBrush, path);

                using Pen lightPen = new Pen(lightEdge, 2);
                using Pen darkPen = new Pen(darkEdge, 2);

                e.Graphics.DrawArc(lightPen, 1, 1, 20, 20, 180, 90);
                e.Graphics.DrawLine(lightPen, 11, 1, rect.Width - 12, 1);
                e.Graphics.DrawLine(lightPen, 1, 11, 1, rect.Height - 12);

                e.Graphics.DrawArc(darkPen, rect.Width - 21, rect.Height - 21, 20, 20, 0, 90);
                e.Graphics.DrawLine(darkPen, 11, rect.Height - 1, rect.Width - 12, rect.Height - 1);
                e.Graphics.DrawLine(darkPen, rect.Width - 1, 11, rect.Width - 1, rect.Height - 12);

                Rectangle drawRect = isPressed
                    ? new Rectangle(1, 1, b.Width - 1, b.Height - 1)
                    : new Rectangle(0, 0, b.Width, b.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    b.Text,
                    b.Font,
                    drawRect,
                    b.ForeColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);
            };
        }
    }
}
