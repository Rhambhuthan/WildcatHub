using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WildcatHub
{
    public partial class frmAdminSetup : Form
    {
        private string _officeEmail = "";
        private string _generatedOtp = "";
        private DateTime _otpExpiry;
        private System.Windows.Forms.Timer _countdownTimer = null!;
        private int _timeLeft = 120;

        public frmAdminSetup()
        {
            InitializeComponent();
            SetupTimer();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplyRoundedCorners(glassPanel, 28);
            LoadOfficeEmail();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (glassPanel.Width > 0 && glassPanel.Height > 0)
                ApplyRoundedCorners(glassPanel, 28);
        }




        private void SetupTimer()
        {
            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000;
            _countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            _timeLeft--;
            lblTimer.Text = $"Time left: {_timeLeft}s";

            if (_timeLeft <= 0)
            {
                _countdownTimer.Stop();
                lblTimer.Text = "Code expired";
                lblResend.Visible = true;
            }
        }

        private void LoadOfficeEmail()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                // Fetch the pre-seeded office email (password will be empty)
                string query = "SELECT TOP 1 OfficeEmail FROM AdminCredentials WHERE ([Password] IS NULL OR [Password] = '')";
                using OleDbCommand cmd = new OleDbCommand(query, conn);
                object? result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value || string.IsNullOrWhiteSpace(result.ToString()))
                {
                    MessageBox.Show(
                        "No admin office email found in the database.\n\nPlease contact your system administrator to seed the AdminCredentials table with the office email.",
                        "Setup Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    Application.Exit();
                    return;
                }

                _officeEmail = result.ToString()!.Trim();

                // Show masked email for security (e.g. et***@cit.edu)
                lblEmailHint.Text = $"OTP will be sent to: {MaskEmail(_officeEmail)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading office email:\n" + ex.Message, "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private string MaskEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex <= 2) return email;

            string local = email[..2] + new string('*', Math.Max(0, atIndex - 2));
            string domain = email[atIndex..];
            return local + domain;
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
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



        private void btnSendOtp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_officeEmail))
            {
                MessageBox.Show("Office email not loaded. Please restart the application.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _generatedOtp = new Random().Next(100000, 999999).ToString();
                _otpExpiry = DateTime.Now.AddSeconds(120);

                EmailService.SendVerificationCode(_officeEmail, "WildcatHub Admin Office", _generatedOtp);

                MessageBox.Show(
                    $"A verification code has been sent to the office email.",
                    "OTP Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Show verify + password panel
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;

                txtOtp.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
                _timeLeft = 120;
                lblTimer.Text = "Time left: 120s";
                lblResend.Visible = false;
                _countdownTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send OTP:\n\n" + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnSetupAccount_Click(object sender, EventArgs e)
        {
            string inputOtp = txtOtp.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputOtp))
            {
                MessageBox.Show("Please enter the verification code.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.Equals(inputOtp, _generatedOtp, StringComparison.Ordinal))
            {
                MessageBox.Show("Invalid verification code.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DateTime.Now > _otpExpiry)
            {
                _countdownTimer.Stop();
                lblTimer.Text = "Code expired";
                lblResend.Visible = true;
                MessageBox.Show("The verification code has expired. Please resend.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please enter and confirm your password.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string updateQuery = @"
UPDATE AdminCredentials
SET [Password] = ?
WHERE OfficeEmail = ?";

                using OleDbCommand cmd = new OleDbCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@p1", newPassword);
                cmd.Parameters.AddWithValue("@p2", _officeEmail);
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    MessageBox.Show("Setup failed. Admin record not found.", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _countdownTimer.Stop();

                MessageBox.Show(
                    "Admin account created successfully!\n\nYou can now log in using the office email and your new password.",
                    "Setup Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Show login BEFORE closing so Application.Run keeps running
                frmLogin loginForm = new frmLogin();
                loginForm.Show();

                // Hide instead of Close so the app doesn't exit
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving admin account:\n" + ex.Message, "Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblResend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                _generatedOtp = new Random().Next(100000, 999999).ToString();
                _otpExpiry = DateTime.Now.AddSeconds(120);

                EmailService.SendVerificationCode(_officeEmail, "WildcatHub Admin Office", _generatedOtp);

                txtOtp.Clear();
                _timeLeft = 120;
                lblTimer.Text = "Time left: 120s";
                lblResend.Visible = false;
                _countdownTimer.Start();

                MessageBox.Show("A new verification code has been sent.", "Resend OTP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to resend OTP:\n\n" + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackToStep1_Click(object sender, EventArgs e)
        {
            _countdownTimer.Stop();
            pnlStep2.Visible = false;
            pnlStep1.Visible = true;
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool hide = !chkShowPassword.Checked;
            txtNewPassword.UseSystemPasswordChar = hide;
            txtConfirmPassword.UseSystemPasswordChar = hide;
        }
    }
}