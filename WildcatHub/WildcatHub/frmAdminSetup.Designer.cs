namespace WildcatHub
{
    partial class frmAdminSetup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdminSetup));
            glassPanel = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblEmailHint = new Label();
            pnlStep1 = new Panel();
            lblStep1Info = new Label();
            btnSendOtp = new Button();
            pnlStep2 = new Panel();
            lblOtpLabel = new Label();
            txtOtp = new TextBox();
            lblTimer = new Label();
            lblResend = new LinkLabel();
            lblNewPasswordLabel = new Label();
            txtNewPassword = new TextBox();
            lblConfirmPasswordLabel = new Label();
            txtConfirmPassword = new TextBox();
            chkShowPassword = new CheckBox();
            btnBackToStep1 = new Button();
            btnSetupAccount = new Button();
            glassPanel.SuspendLayout();
            pnlStep1.SuspendLayout();
            pnlStep2.SuspendLayout();
            SuspendLayout();
            // 
            // glassPanel
            // 
            glassPanel.BackColor = Color.FromArgb(80, 255, 255, 255);
            glassPanel.Controls.Add(lblTitle);
            glassPanel.Controls.Add(lblSubtitle);
            glassPanel.Controls.Add(lblEmailHint);
            glassPanel.Controls.Add(pnlStep1);
            glassPanel.Controls.Add(pnlStep2);
            glassPanel.Location = new Point(260, 80);
            glassPanel.Name = "glassPanel";
            glassPanel.Size = new Size(400, 440);
            glassPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(40, 26);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(202, 41);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Admin Setup";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.White;
            lblSubtitle.Location = new Point(40, 72);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(284, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "One-time setup for the admin office account.";
            // 
            // lblEmailHint
            // 
            lblEmailHint.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblEmailHint.ForeColor = Color.FromArgb(200, 255, 255, 255);
            lblEmailHint.Location = new Point(40, 100);
            lblEmailHint.Name = "lblEmailHint";
            lblEmailHint.Size = new Size(320, 20);
            lblEmailHint.TabIndex = 2;
            lblEmailHint.Text = "Loading office email...";
            // 
            // pnlStep1
            // 
            pnlStep1.BackColor = Color.Transparent;
            pnlStep1.Controls.Add(lblStep1Info);
            pnlStep1.Controls.Add(btnSendOtp);
            pnlStep1.Location = new Point(0, 132);
            pnlStep1.Name = "pnlStep1";
            pnlStep1.Size = new Size(400, 280);
            pnlStep1.TabIndex = 3;
            // 
            // lblStep1Info
            // 
            lblStep1Info.Font = new Font("Segoe UI", 10.5F);
            lblStep1Info.ForeColor = Color.White;
            lblStep1Info.Location = new Point(40, 24);
            lblStep1Info.Name = "lblStep1Info";
            lblStep1Info.Size = new Size(320, 80);
            lblStep1Info.TabIndex = 0;
            lblStep1Info.Text = "This is a one-time setup. A verification code will be sent to the registered office email to confirm your identity before setting a password.";
            // 
            // btnSendOtp
            // 
            btnSendOtp.BackColor = Color.FromArgb(156, 119, 181);
            btnSendOtp.FlatAppearance.BorderSize = 0;
            btnSendOtp.FlatStyle = FlatStyle.Flat;
            btnSendOtp.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            btnSendOtp.ForeColor = Color.White;
            btnSendOtp.Location = new Point(40, 136);
            btnSendOtp.Name = "btnSendOtp";
            btnSendOtp.Size = new Size(310, 42);
            btnSendOtp.TabIndex = 1;
            btnSendOtp.Text = "Send Verification Code";
            btnSendOtp.UseVisualStyleBackColor = false;
            btnSendOtp.Click += btnSendOtp_Click;
            // 
            // pnlStep2
            // 
            pnlStep2.BackColor = Color.Transparent;
            pnlStep2.Controls.Add(lblOtpLabel);
            pnlStep2.Controls.Add(txtOtp);
            pnlStep2.Controls.Add(lblTimer);
            pnlStep2.Controls.Add(lblResend);
            pnlStep2.Controls.Add(lblNewPasswordLabel);
            pnlStep2.Controls.Add(txtNewPassword);
            pnlStep2.Controls.Add(lblConfirmPasswordLabel);
            pnlStep2.Controls.Add(txtConfirmPassword);
            pnlStep2.Controls.Add(chkShowPassword);
            pnlStep2.Controls.Add(btnBackToStep1);
            pnlStep2.Controls.Add(btnSetupAccount);
            pnlStep2.Location = new Point(0, 132);
            pnlStep2.Name = "pnlStep2";
            pnlStep2.Size = new Size(400, 300);
            pnlStep2.TabIndex = 4;
            pnlStep2.Visible = false;
            // 
            // lblOtpLabel
            // 
            lblOtpLabel.AutoSize = true;
            lblOtpLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblOtpLabel.ForeColor = Color.White;
            lblOtpLabel.Location = new Point(40, 10);
            lblOtpLabel.Name = "lblOtpLabel";
            lblOtpLabel.Size = new Size(117, 19);
            lblOtpLabel.TabIndex = 0;
            lblOtpLabel.Text = "Verification Code";
            // 
            // txtOtp
            // 
            txtOtp.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtOtp.Location = new Point(40, 34);
            txtOtp.MaxLength = 6;
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(200, 32);
            txtOtp.TabIndex = 1;
            txtOtp.TextAlign = HorizontalAlignment.Center;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Font = new Font("Segoe UI", 9F);
            lblTimer.ForeColor = Color.White;
            lblTimer.Location = new Point(40, 74);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(77, 15);
            lblTimer.TabIndex = 2;
            lblTimer.Text = "Time left: 60s";
            // 
            // lblResend
            // 
            lblResend.ActiveLinkColor = Color.FromArgb(169, 215, 159);
            lblResend.AutoSize = true;
            lblResend.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            lblResend.LinkColor = Color.White;
            lblResend.Location = new Point(160, 74);
            lblResend.Name = "lblResend";
            lblResend.Size = new Size(76, 15);
            lblResend.TabIndex = 3;
            lblResend.TabStop = true;
            lblResend.Text = "Resend Code";
            lblResend.Visible = false;
            lblResend.LinkClicked += lblResend_LinkClicked;
            // 
            // lblNewPasswordLabel
            // 
            lblNewPasswordLabel.AutoSize = true;
            lblNewPasswordLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblNewPasswordLabel.ForeColor = Color.White;
            lblNewPasswordLabel.Location = new Point(40, 104);
            lblNewPasswordLabel.Name = "lblNewPasswordLabel";
            lblNewPasswordLabel.Size = new Size(101, 19);
            lblNewPasswordLabel.TabIndex = 4;
            lblNewPasswordLabel.Text = "New Password";
            // 
            // txtNewPassword
            // 
            txtNewPassword.Font = new Font("Segoe UI", 10.5F);
            txtNewPassword.Location = new Point(40, 128);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.Size = new Size(310, 26);
            txtNewPassword.TabIndex = 5;
            txtNewPassword.UseSystemPasswordChar = true;
            // 
            // lblConfirmPasswordLabel
            // 
            lblConfirmPasswordLabel.AutoSize = true;
            lblConfirmPasswordLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblConfirmPasswordLabel.ForeColor = Color.White;
            lblConfirmPasswordLabel.Location = new Point(40, 168);
            lblConfirmPasswordLabel.Name = "lblConfirmPasswordLabel";
            lblConfirmPasswordLabel.Size = new Size(123, 19);
            lblConfirmPasswordLabel.TabIndex = 6;
            lblConfirmPasswordLabel.Text = "Confirm Password";
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Font = new Font("Segoe UI", 10.5F);
            txtConfirmPassword.Location = new Point(40, 192);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(310, 26);
            txtConfirmPassword.TabIndex = 7;
            txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.BackColor = Color.Transparent;
            chkShowPassword.Font = new Font("Segoe UI", 9F);
            chkShowPassword.ForeColor = Color.White;
            chkShowPassword.Location = new Point(40, 228);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new Size(113, 19);
            chkShowPassword.TabIndex = 8;
            chkShowPassword.Text = "Show Passwords";
            chkShowPassword.UseVisualStyleBackColor = false;
            chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
            // 
            // btnBackToStep1
            // 
            btnBackToStep1.BackColor = Color.FromArgb(190, 170, 205);
            btnBackToStep1.FlatAppearance.BorderSize = 0;
            btnBackToStep1.FlatStyle = FlatStyle.Flat;
            btnBackToStep1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnBackToStep1.ForeColor = Color.White;
            btnBackToStep1.Location = new Point(40, 258);
            btnBackToStep1.Name = "btnBackToStep1";
            btnBackToStep1.Size = new Size(130, 38);
            btnBackToStep1.TabIndex = 9;
            btnBackToStep1.Text = "Back";
            btnBackToStep1.UseVisualStyleBackColor = false;
            btnBackToStep1.Click += btnBackToStep1_Click;
            // 
            // btnSetupAccount
            // 
            btnSetupAccount.BackColor = Color.FromArgb(156, 119, 181);
            btnSetupAccount.FlatAppearance.BorderSize = 0;
            btnSetupAccount.FlatStyle = FlatStyle.Flat;
            btnSetupAccount.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSetupAccount.ForeColor = Color.White;
            btnSetupAccount.Location = new Point(188, 258);
            btnSetupAccount.Name = "btnSetupAccount";
            btnSetupAccount.Size = new Size(162, 38);
            btnSetupAccount.TabIndex = 10;
            btnSetupAccount.Text = "Create Account";
            btnSetupAccount.UseVisualStyleBackColor = false;
            btnSetupAccount.Click += btnSetupAccount_Click;
            // 
            // frmAdminSetup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(900, 600);
            Controls.Add(glassPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmAdminSetup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WildcatHub - Admin Account Setup";
            glassPanel.ResumeLayout(false);
            glassPanel.PerformLayout();
            pnlStep1.ResumeLayout(false);
            pnlStep2.ResumeLayout(false);
            pnlStep2.PerformLayout();
            ResumeLayout(false);
        }

        // Controls
        private Panel glassPanel;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblEmailHint;

        private Panel pnlStep1;
        private Label lblStep1Info;
        private Button btnSendOtp;

        private Panel pnlStep2;
        private Label lblOtpLabel;
        private TextBox txtOtp;
        private Label lblTimer;
        private LinkLabel lblResend;
        private Label lblNewPasswordLabel;
        private TextBox txtNewPassword;
        private Label lblConfirmPasswordLabel;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPassword;
        private Button btnBackToStep1;
        private Button btnSetupAccount;
    }
}