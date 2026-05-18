namespace WildcatHub
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            glassLogin = new Panel();
            btnGoToCreate = new Button();
            btnLogin = new Button();
            txtPassword_Login = new TextBox();
            lblPasswordLogin = new Label();
            txtSchoolID_Login = new TextBox();
            lblSchoolIDLogin = new Label();
            lblLoginTitle = new Label();
            linkCreateAccount = new LinkLabel();
            lblForgotPassword = new LinkLabel();
            chkShowPasswordLogin = new CheckBox();
            lblNoAccount = new Label();
            glassCreate = new Panel();
            lblHasAccount = new Label();
            chkShowPasswordCreate = new CheckBox();
            btnBackToLogin = new Button();
            linkBackToLogin = new LinkLabel();
            btnCreateAccount = new Button();
            txtConfirmPassword = new TextBox();
            lblConfirmPassword = new Label();
            txtPassword = new TextBox();
            lblPassword = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            txtSchoolID = new TextBox();
            lblSchoolID = new Label();
            lblCreateTitle = new Label();
            btnBackToCreate = new Button();
            btnVerify = new Button();
            lblResend = new LinkLabel();
            lblTimer = new Label();
            txtCode = new TextBox();
            lblCode = new Label();
            lblVerifyGuide = new Label();
            lblVerifyTitle = new Label();
            glassForgot = new Panel();
            btnBackFromForgot = new Button();
            btnSendResetCode = new Button();
            txtForgotEmail = new TextBox();
            lblForgotEmail = new Label();
            txtForgotSchoolID = new TextBox();
            lblForgotSchoolID = new Label();
            lblForgotTitle = new Label();
            glassResetVerify = new Panel();
            btnBackToForgot = new Button();
            btnVerifyResetCode = new Button();
            lblResetResend = new LinkLabel();
            lblResetTimer = new Label();
            txtResetCode = new TextBox();
            lblResetCode = new Label();
            lblResetGuide = new Label();
            lblResetVerifyTitle = new Label();
            glassNewPassword = new Panel();
            btnBackToResetVerify = new Button();
            btnSaveNewPassword = new Button();
            txtConfirmNewPassword = new TextBox();
            lblConfirmNewPassword = new Label();
            txtNewPassword = new TextBox();
            lblNewPassword = new Label();
            lblNewPasswordTitle = new Label();
            glassVerify = new Panel();
            linkReturnVerify = new LinkLabel();
            glassLogin.SuspendLayout();
            glassCreate.SuspendLayout();
            glassForgot.SuspendLayout();
            glassResetVerify.SuspendLayout();
            glassNewPassword.SuspendLayout();
            glassVerify.SuspendLayout();
            SuspendLayout();
            // 
            // glassLogin
            // 
            glassLogin.BackColor = Color.FromArgb(165, 255, 251, 252);
            glassLogin.Controls.Add(btnGoToCreate);
            glassLogin.Controls.Add(btnLogin);
            glassLogin.Controls.Add(txtPassword_Login);
            glassLogin.Controls.Add(lblPasswordLogin);
            glassLogin.Controls.Add(txtSchoolID_Login);
            glassLogin.Controls.Add(lblSchoolIDLogin);
            glassLogin.Controls.Add(lblLoginTitle);
            glassLogin.Controls.Add(linkCreateAccount);
            glassLogin.Controls.Add(lblForgotPassword);
            glassLogin.Controls.Add(chkShowPasswordLogin);
            glassLogin.Controls.Add(lblNoAccount);
            glassLogin.Location = new Point(703, 100);
            glassLogin.Name = "glassLogin";
            glassLogin.Size = new Size(380, 430);
            glassLogin.TabIndex = 0;
            // 
            // btnGoToCreate
            // 
            btnGoToCreate.BackColor = Color.FromArgb(212, 168, 45);
            btnGoToCreate.FlatAppearance.BorderSize = 0;
            btnGoToCreate.FlatStyle = FlatStyle.Flat;
            btnGoToCreate.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            btnGoToCreate.ForeColor = Color.White;
            btnGoToCreate.Location = new Point(43, 310);
            btnGoToCreate.Name = "btnGoToCreate";
            btnGoToCreate.Size = new Size(294, 40);
            btnGoToCreate.TabIndex = 6;
            btnGoToCreate.Text = "Create Account";
            btnGoToCreate.UseVisualStyleBackColor = false;
            btnGoToCreate.Visible = false;
            btnGoToCreate.Click += btnGoToCreate_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(212, 168, 45);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(43, 248);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(294, 42);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword_Login
            // 
            txtPassword_Login.BackColor = Color.White;
            txtPassword_Login.BorderStyle = BorderStyle.FixedSingle;
            txtPassword_Login.Font = new Font("Segoe UI", 11F);
            txtPassword_Login.Location = new Point(43, 183);
            txtPassword_Login.Name = "txtPassword_Login";
            txtPassword_Login.Size = new Size(294, 27);
            txtPassword_Login.TabIndex = 4;
            txtPassword_Login.UseSystemPasswordChar = true;
            // 
            // lblPasswordLogin
            // 
            lblPasswordLogin.AutoSize = true;
            lblPasswordLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordLogin.ForeColor = Color.FromArgb(92, 45, 58);
            lblPasswordLogin.Location = new Point(43, 161);
            lblPasswordLogin.Name = "lblPasswordLogin";
            lblPasswordLogin.Size = new Size(73, 19);
            lblPasswordLogin.TabIndex = 3;
            lblPasswordLogin.Text = "Password";
            // 
            // txtSchoolID_Login
            // 
            txtSchoolID_Login.BackColor = Color.White;
            txtSchoolID_Login.BorderStyle = BorderStyle.FixedSingle;
            txtSchoolID_Login.Font = new Font("Segoe UI", 11F);
            txtSchoolID_Login.Location = new Point(43, 118);
            txtSchoolID_Login.Name = "txtSchoolID_Login";
            txtSchoolID_Login.Size = new Size(294, 27);
            txtSchoolID_Login.TabIndex = 2;
            // 
            // lblSchoolIDLogin
            // 
            lblSchoolIDLogin.AutoSize = true;
            lblSchoolIDLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSchoolIDLogin.ForeColor = Color.FromArgb(92, 45, 58);
            lblSchoolIDLogin.Location = new Point(43, 96);
            lblSchoolIDLogin.Name = "lblSchoolIDLogin";
            lblSchoolIDLogin.Size = new Size(72, 19);
            lblSchoolIDLogin.TabIndex = 1;
            lblSchoolIDLogin.Text = "School ID";
            // 
            // lblLoginTitle
            // 
            lblLoginTitle.AutoSize = true;
            lblLoginTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblLoginTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblLoginTitle.Location = new Point(138, 32);
            lblLoginTitle.Name = "lblLoginTitle";
            lblLoginTitle.Size = new Size(89, 37);
            lblLoginTitle.TabIndex = 0;
            lblLoginTitle.Text = "Login";
            // 
            // linkCreateAccount
            // 
            linkCreateAccount.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            linkCreateAccount.AutoSize = true;
            linkCreateAccount.BackColor = Color.Transparent;
            linkCreateAccount.Font = new Font("Segoe UI", 9.5F);
            linkCreateAccount.LinkColor = Color.FromArgb(92, 45, 58);
            linkCreateAccount.Location = new Point(215, 305);
            linkCreateAccount.Name = "linkCreateAccount";
            linkCreateAccount.Size = new Size(96, 17);
            linkCreateAccount.TabIndex = 9;
            linkCreateAccount.TabStop = true;
            linkCreateAccount.Text = "Create Account";
            linkCreateAccount.VisitedLinkColor = Color.FromArgb(92, 45, 58);
            linkCreateAccount.LinkClicked += linkCreateAccount_LinkClicked;
            // 
            // lblForgotPassword
            // 
            lblForgotPassword.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            lblForgotPassword.AutoSize = true;
            lblForgotPassword.BackColor = Color.Transparent;
            lblForgotPassword.Font = new Font("Segoe UI", 9F);
            lblForgotPassword.LinkColor = Color.FromArgb(92, 45, 58);
            lblForgotPassword.Location = new Point(140, 334);
            lblForgotPassword.Name = "lblForgotPassword";
            lblForgotPassword.Size = new Size(100, 15);
            lblForgotPassword.TabIndex = 7;
            lblForgotPassword.TabStop = true;
            lblForgotPassword.Text = "Forgot Password?";
            lblForgotPassword.VisitedLinkColor = Color.FromArgb(92, 45, 58);
            lblForgotPassword.LinkClicked += lblForgotPassword_LinkClicked;
            // 
            // chkShowPasswordLogin
            // 
            chkShowPasswordLogin.AutoSize = true;
            chkShowPasswordLogin.BackColor = Color.Transparent;
            chkShowPasswordLogin.Font = new Font("Segoe UI", 9F);
            chkShowPasswordLogin.ForeColor = Color.FromArgb(120, 90, 108);
            chkShowPasswordLogin.Location = new Point(43, 223);
            chkShowPasswordLogin.Name = "chkShowPasswordLogin";
            chkShowPasswordLogin.Size = new Size(108, 19);
            chkShowPasswordLogin.TabIndex = 8;
            chkShowPasswordLogin.Text = "Show Password";
            chkShowPasswordLogin.UseVisualStyleBackColor = false;
            chkShowPasswordLogin.CheckedChanged += chkShowPasswordLogin_CheckedChanged;
            // 
            // lblNoAccount
            // 
            lblNoAccount.AutoSize = true;
            lblNoAccount.BackColor = Color.Transparent;
            lblNoAccount.Font = new Font("Segoe UI", 9.5F);
            lblNoAccount.ForeColor = Color.FromArgb(92, 45, 58);
            lblNoAccount.Location = new Point(70, 305);
            lblNoAccount.Name = "lblNoAccount";
            lblNoAccount.Size = new Size(143, 17);
            lblNoAccount.TabIndex = 21;
            lblNoAccount.Text = "Don't have an account?";
            // 
            // glassCreate
            // 
            glassCreate.BackColor = Color.FromArgb(165, 255, 251, 252);
            glassCreate.Controls.Add(lblHasAccount);
            glassCreate.Controls.Add(chkShowPasswordCreate);
            glassCreate.Controls.Add(btnBackToLogin);
            glassCreate.Controls.Add(linkBackToLogin);
            glassCreate.Controls.Add(btnCreateAccount);
            glassCreate.Controls.Add(txtConfirmPassword);
            glassCreate.Controls.Add(lblConfirmPassword);
            glassCreate.Controls.Add(txtPassword);
            glassCreate.Controls.Add(lblPassword);
            glassCreate.Controls.Add(txtEmail);
            glassCreate.Controls.Add(lblEmail);
            glassCreate.Controls.Add(txtSchoolID);
            glassCreate.Controls.Add(lblSchoolID);
            glassCreate.Controls.Add(lblCreateTitle);
            glassCreate.Location = new Point(703, 100);
            glassCreate.Name = "glassCreate";
            glassCreate.Size = new Size(395, 470);
            glassCreate.TabIndex = 1;
            glassCreate.Visible = false;
            // 
            // lblHasAccount
            // 
            lblHasAccount.AutoSize = true;
            lblHasAccount.BackColor = Color.Transparent;
            lblHasAccount.Font = new Font("Segoe UI", 9.5F);
            lblHasAccount.ForeColor = Color.FromArgb(92, 45, 58);
            lblHasAccount.Location = new Point(90, 432);
            lblHasAccount.Name = "lblHasAccount";
            lblHasAccount.Size = new Size(156, 17);
            lblHasAccount.TabIndex = 22;
            lblHasAccount.Text = "Already have an account?";
            // 
            // chkShowPasswordCreate
            // 
            chkShowPasswordCreate.AutoSize = true;
            chkShowPasswordCreate.BackColor = Color.Transparent;
            chkShowPasswordCreate.Font = new Font("Segoe UI", 9F);
            chkShowPasswordCreate.ForeColor = Color.FromArgb(120, 90, 108);
            chkShowPasswordCreate.Location = new Point(46, 322);
            chkShowPasswordCreate.Name = "chkShowPasswordCreate";
            chkShowPasswordCreate.Size = new Size(108, 19);
            chkShowPasswordCreate.TabIndex = 9;
            chkShowPasswordCreate.Text = "Show Password";
            chkShowPasswordCreate.UseVisualStyleBackColor = false;
            chkShowPasswordCreate.CheckedChanged += chkShowPasswordCreate_CheckedChanged;
            // 
            // btnBackToLogin
            // 
            btnBackToLogin.BackColor = Color.FromArgb(190, 170, 205);
            btnBackToLogin.FlatAppearance.BorderSize = 0;
            btnBackToLogin.FlatStyle = FlatStyle.Flat;
            btnBackToLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBackToLogin.ForeColor = Color.White;
            btnBackToLogin.Location = new Point(46, 374);
            btnBackToLogin.Name = "btnBackToLogin";
            btnBackToLogin.Size = new Size(140, 38);
            btnBackToLogin.TabIndex = 10;
            btnBackToLogin.Text = "Back";
            btnBackToLogin.UseVisualStyleBackColor = false;
            btnBackToLogin.Visible = false;
            btnBackToLogin.Click += btnBackToLogin_Click;
            // 
            // linkBackToLogin
            // 
            linkBackToLogin.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            linkBackToLogin.AutoSize = true;
            linkBackToLogin.BackColor = Color.Transparent;
            linkBackToLogin.Font = new Font("Segoe UI", 9.5F);
            linkBackToLogin.LinkColor = Color.FromArgb(92, 45, 58);
            linkBackToLogin.Location = new Point(245, 432);
            linkBackToLogin.Name = "linkBackToLogin";
            linkBackToLogin.Size = new Size(40, 17);
            linkBackToLogin.TabIndex = 12;
            linkBackToLogin.TabStop = true;
            linkBackToLogin.Text = "Login";
            linkBackToLogin.VisitedLinkColor = Color.FromArgb(92, 45, 58);
            linkBackToLogin.LinkClicked += linkBackToLogin_LinkClicked;
            // 
            // btnCreateAccount
            // 
            btnCreateAccount.BackColor = Color.FromArgb(212, 168, 45);
            btnCreateAccount.FlatAppearance.BorderSize = 0;
            btnCreateAccount.FlatStyle = FlatStyle.Flat;
            btnCreateAccount.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnCreateAccount.ForeColor = Color.White;
            btnCreateAccount.Location = new Point(43, 352);
            btnCreateAccount.Name = "btnCreateAccount";
            btnCreateAccount.Size = new Size(294, 42);
            btnCreateAccount.TabIndex = 10;
            btnCreateAccount.Text = "Create Account";
            btnCreateAccount.UseVisualStyleBackColor = false;
            btnCreateAccount.Click += btnCreateAccount_Click;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.BackColor = Color.White;
            txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtConfirmPassword.Font = new Font("Segoe UI", 10.5F);
            txtConfirmPassword.Location = new Point(46, 288);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(302, 26);
            txtConfirmPassword.TabIndex = 8;
            txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblConfirmPassword.ForeColor = Color.FromArgb(92, 45, 58);
            lblConfirmPassword.Location = new Point(46, 266);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(131, 19);
            lblConfirmPassword.TabIndex = 7;
            lblConfirmPassword.Text = "Confirm Password";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 10.5F);
            txtPassword.Location = new Point(46, 228);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(302, 26);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(92, 45, 58);
            lblPassword.Location = new Point(46, 206);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(73, 19);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password";
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.White;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Font = new Font("Segoe UI", 10.5F);
            txtEmail.Location = new Point(46, 168);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(302, 26);
            txtEmail.TabIndex = 4;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(92, 45, 58);
            lblEmail.Location = new Point(46, 146);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(94, 19);
            lblEmail.TabIndex = 3;
            lblEmail.Text = "School Email";
            // 
            // txtSchoolID
            // 
            txtSchoolID.BackColor = Color.White;
            txtSchoolID.BorderStyle = BorderStyle.FixedSingle;
            txtSchoolID.Font = new Font("Segoe UI", 10.5F);
            txtSchoolID.Location = new Point(46, 108);
            txtSchoolID.Name = "txtSchoolID";
            txtSchoolID.Size = new Size(302, 26);
            txtSchoolID.TabIndex = 2;
            // 
            // lblSchoolID
            // 
            lblSchoolID.AutoSize = true;
            lblSchoolID.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSchoolID.ForeColor = Color.FromArgb(92, 45, 58);
            lblSchoolID.Location = new Point(46, 86);
            lblSchoolID.Name = "lblSchoolID";
            lblSchoolID.Size = new Size(170, 19);
            lblSchoolID.TabIndex = 1;
            lblSchoolID.Text = "School ID (24-####-###)";
            // 
            // lblCreateTitle
            // 
            lblCreateTitle.AutoSize = true;
            lblCreateTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblCreateTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblCreateTitle.Location = new Point(86, 28);
            lblCreateTitle.Name = "lblCreateTitle";
            lblCreateTitle.Size = new Size(213, 37);
            lblCreateTitle.TabIndex = 0;
            lblCreateTitle.Text = "Create Account";
            // 
            // btnBackToCreate
            // 
            btnBackToCreate.BackColor = Color.FromArgb(190, 170, 205);
            btnBackToCreate.FlatAppearance.BorderSize = 0;
            btnBackToCreate.FlatStyle = FlatStyle.Flat;
            btnBackToCreate.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnBackToCreate.ForeColor = Color.White;
            btnBackToCreate.Location = new Point(142, 305);
            btnBackToCreate.Name = "btnBackToCreate";
            btnBackToCreate.Size = new Size(110, 30);
            btnBackToCreate.TabIndex = 7;
            btnBackToCreate.Text = "Back";
            btnBackToCreate.UseVisualStyleBackColor = false;
            btnBackToCreate.Visible = false;
            btnBackToCreate.Click += btnBackToCreate_Click;
            // 
            // btnVerify
            // 
            btnVerify.BackColor = Color.FromArgb(212, 168, 45);
            btnVerify.FlatAppearance.BorderSize = 0;
            btnVerify.FlatStyle = FlatStyle.Flat;
            btnVerify.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnVerify.ForeColor = Color.White;
            btnVerify.Location = new Point(43, 244);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new Size(294, 42);
            btnVerify.TabIndex = 4;
            btnVerify.Text = "Verify";
            btnVerify.UseVisualStyleBackColor = false;
            btnVerify.Click += btnVerify_Click;
            // 
            // lblResend
            // 
            lblResend.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            lblResend.BackColor = Color.Transparent;
            lblResend.Font = new Font("Segoe UI", 9F);
            lblResend.LinkColor = Color.FromArgb(92, 45, 58);
            lblResend.Location = new Point(0, 214);
            lblResend.Name = "lblResend";
            lblResend.Size = new Size(395, 20);
            lblResend.TabIndex = 5;
            lblResend.TabStop = true;
            lblResend.Text = "Resend Code";
            lblResend.TextAlign = ContentAlignment.MiddleCenter;
            lblResend.Visible = false;
            lblResend.VisitedLinkColor = Color.FromArgb(92, 45, 58);
            lblResend.LinkClicked += lblResend_LinkClicked;
            // 
            // lblTimer
            // 
            lblTimer.Font = new Font("Segoe UI", 9.5F);
            lblTimer.ForeColor = Color.FromArgb(120, 90, 108);
            lblTimer.Location = new Point(0, 214);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(395, 20);
            lblTimer.TabIndex = 4;
            lblTimer.Text = "Time left: 120s";
            lblTimer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtCode
            // 
            txtCode.BackColor = Color.White;
            txtCode.BorderStyle = BorderStyle.FixedSingle;
            txtCode.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtCode.Location = new Point(67, 164);
            txtCode.Name = "txtCode";
            txtCode.Size = new Size(260, 36);
            txtCode.TabIndex = 3;
            txtCode.TextAlign = HorizontalAlignment.Center;
            // 
            // lblCode
            // 
            lblCode.AutoSize = true;
            lblCode.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCode.ForeColor = Color.FromArgb(92, 45, 58);
            lblCode.Location = new Point(67, 138);
            lblCode.Name = "lblCode";
            lblCode.Size = new Size(128, 20);
            lblCode.TabIndex = 2;
            lblCode.Text = "Verification Code";
            // 
            // lblVerifyGuide
            // 
            lblVerifyGuide.Font = new Font("Segoe UI", 10.5F);
            lblVerifyGuide.ForeColor = Color.FromArgb(120, 90, 108);
            lblVerifyGuide.Location = new Point(40, 82);
            lblVerifyGuide.Name = "lblVerifyGuide";
            lblVerifyGuide.Size = new Size(315, 44);
            lblVerifyGuide.TabIndex = 1;
            lblVerifyGuide.Text = "Enter the 6-digit verification code sent to your school email.";
            lblVerifyGuide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblVerifyTitle
            // 
            lblVerifyTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblVerifyTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblVerifyTitle.Location = new Point(0, 28);
            lblVerifyTitle.Name = "lblVerifyTitle";
            lblVerifyTitle.Size = new Size(395, 42);
            lblVerifyTitle.TabIndex = 0;
            lblVerifyTitle.Text = "Verify";
            lblVerifyTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // glassForgot
            // 
            glassForgot.BackColor = Color.FromArgb(80, 255, 255, 255);
            glassForgot.Controls.Add(btnBackFromForgot);
            glassForgot.Controls.Add(btnSendResetCode);
            glassForgot.Controls.Add(txtForgotEmail);
            glassForgot.Controls.Add(lblForgotEmail);
            glassForgot.Controls.Add(txtForgotSchoolID);
            glassForgot.Controls.Add(lblForgotSchoolID);
            glassForgot.Controls.Add(lblForgotTitle);
            glassForgot.Location = new Point(703, 120);
            glassForgot.Name = "glassForgot";
            glassForgot.Size = new Size(380, 330);
            glassForgot.TabIndex = 3;
            glassForgot.Visible = false;
            // 
            // btnBackFromForgot
            // 
            btnBackFromForgot.BackColor = Color.FromArgb(190, 170, 205);
            btnBackFromForgot.FlatAppearance.BorderSize = 0;
            btnBackFromForgot.FlatStyle = FlatStyle.Flat;
            btnBackFromForgot.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnBackFromForgot.ForeColor = Color.White;
            btnBackFromForgot.Location = new Point(43, 238);
            btnBackFromForgot.Name = "btnBackFromForgot";
            btnBackFromForgot.Size = new Size(120, 40);
            btnBackFromForgot.TabIndex = 5;
            btnBackFromForgot.Text = "Back";
            btnBackFromForgot.UseVisualStyleBackColor = false;
            btnBackFromForgot.Click += btnBackFromForgot_Click;
            // 
            // btnSendResetCode
            // 
            btnSendResetCode.BackColor = Color.FromArgb(212, 168, 45);
            btnSendResetCode.FlatAppearance.BorderSize = 0;
            btnSendResetCode.FlatStyle = FlatStyle.Flat;
            btnSendResetCode.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnSendResetCode.ForeColor = Color.White;
            btnSendResetCode.Location = new Point(177, 238);
            btnSendResetCode.Name = "btnSendResetCode";
            btnSendResetCode.Size = new Size(160, 40);
            btnSendResetCode.TabIndex = 5;
            btnSendResetCode.Text = "Send Reset Code";
            btnSendResetCode.UseVisualStyleBackColor = false;
            btnSendResetCode.Click += btnSendResetCode_Click;
            // 
            // txtForgotEmail
            // 
            txtForgotEmail.Font = new Font("Segoe UI", 10.5F);
            txtForgotEmail.Location = new Point(43, 174);
            txtForgotEmail.Name = "txtForgotEmail";
            txtForgotEmail.Size = new Size(294, 26);
            txtForgotEmail.TabIndex = 4;
            // 
            // lblForgotEmail
            // 
            lblForgotEmail.AutoSize = true;
            lblForgotEmail.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblForgotEmail.ForeColor = Color.FromArgb(92, 45, 58);
            lblForgotEmail.Location = new Point(43, 150);
            lblForgotEmail.Name = "lblForgotEmail";
            lblForgotEmail.Size = new Size(90, 19);
            lblForgotEmail.TabIndex = 3;
            lblForgotEmail.Text = "School Email";
            // 
            // txtForgotSchoolID
            // 
            txtForgotSchoolID.Font = new Font("Segoe UI", 10.5F);
            txtForgotSchoolID.Location = new Point(43, 108);
            txtForgotSchoolID.Name = "txtForgotSchoolID";
            txtForgotSchoolID.Size = new Size(294, 26);
            txtForgotSchoolID.TabIndex = 2;
            // 
            // lblForgotSchoolID
            // 
            lblForgotSchoolID.AutoSize = true;
            lblForgotSchoolID.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblForgotSchoolID.ForeColor = Color.FromArgb(92, 45, 58);
            lblForgotSchoolID.Location = new Point(43, 84);
            lblForgotSchoolID.Name = "lblForgotSchoolID";
            lblForgotSchoolID.Size = new Size(70, 19);
            lblForgotSchoolID.TabIndex = 1;
            lblForgotSchoolID.Text = "School ID";
            // 
            // lblForgotTitle
            // 
            lblForgotTitle.AutoSize = false;
            lblForgotTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblForgotTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblForgotTitle.Location = new Point(0, 24);
            lblForgotTitle.Name = "lblForgotTitle";
            lblForgotTitle.Size = new Size(380, 46);
            lblForgotTitle.TabIndex = 0;
            lblForgotTitle.Text = "Forgot Password";
            lblForgotTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // glassResetVerify
            // 
            glassResetVerify.BackColor = Color.FromArgb(80, 255, 255, 255);
            glassResetVerify.Controls.Add(btnBackToForgot);
            glassResetVerify.Controls.Add(btnVerifyResetCode);
            glassResetVerify.Controls.Add(lblResetResend);
            glassResetVerify.Controls.Add(lblResetTimer);
            glassResetVerify.Controls.Add(txtResetCode);
            glassResetVerify.Controls.Add(lblResetCode);
            glassResetVerify.Controls.Add(lblResetGuide);
            glassResetVerify.Controls.Add(lblResetVerifyTitle);
            glassResetVerify.Location = new Point(703, 100);
            glassResetVerify.Name = "glassResetVerify";
            glassResetVerify.Size = new Size(380, 390);
            glassResetVerify.TabIndex = 4;
            glassResetVerify.Visible = false;
            // 
            // btnBackToForgot
            // 
            btnBackToForgot.BackColor = Color.FromArgb(190, 170, 205);
            btnBackToForgot.FlatAppearance.BorderSize = 0;
            btnBackToForgot.FlatStyle = FlatStyle.Flat;
            btnBackToForgot.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnBackToForgot.ForeColor = Color.White;
            btnBackToForgot.Location = new Point(43, 296);
            btnBackToForgot.Name = "btnBackToForgot";
            btnBackToForgot.Size = new Size(120, 38);
            btnBackToForgot.TabIndex = 7;
            btnBackToForgot.Text = "Back";
            btnBackToForgot.UseVisualStyleBackColor = false;
            btnBackToForgot.Click += btnBackToForgot_Click;
            // 
            // btnVerifyResetCode
            // 
            btnVerifyResetCode.BackColor = Color.FromArgb(212, 168, 45);
            btnVerifyResetCode.FlatAppearance.BorderSize = 0;
            btnVerifyResetCode.FlatStyle = FlatStyle.Flat;
            btnVerifyResetCode.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnVerifyResetCode.ForeColor = Color.White;
            btnVerifyResetCode.Location = new Point(43, 242);
            btnVerifyResetCode.Name = "btnVerifyResetCode";
            btnVerifyResetCode.Size = new Size(294, 42);
            btnVerifyResetCode.TabIndex = 4;
            btnVerifyResetCode.Text = "Verify";
            btnVerifyResetCode.UseVisualStyleBackColor = false;
            btnVerifyResetCode.Click += btnVerifyResetCode_Click;
            // 
            // lblResetResend
            // 
            lblResetResend.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            lblResetResend.AutoSize = false;
            lblResetResend.LinkColor = Color.FromArgb(92, 45, 58);
            lblResetResend.Location = new Point(0, 214);
            lblResetResend.Name = "lblResetResend";
            lblResetResend.Size = new Size(380, 20);
            lblResetResend.TabIndex = 6;
            lblResetResend.TabStop = true;
            lblResetResend.Text = "Resend Code";
            lblResetResend.TextAlign = ContentAlignment.MiddleCenter;
            lblResetResend.Visible = false;
            lblResetResend.LinkClicked += lblResetResend_LinkClicked;
            // 
            // lblResetTimer
            // 
            lblResetTimer.AutoSize = false;
            lblResetTimer.Font = new Font("Segoe UI", 9F);
            lblResetTimer.ForeColor = Color.FromArgb(120, 90, 108);
            lblResetTimer.Location = new Point(0, 214);
            lblResetTimer.Name = "lblResetTimer";
            lblResetTimer.Size = new Size(380, 20);
            lblResetTimer.TabIndex = 5;
            lblResetTimer.Text = "Time left: 60s";
            lblResetTimer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtResetCode
            // 
            txtResetCode.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtResetCode.Location = new Point(43, 166);
            txtResetCode.MaxLength = 6;
            txtResetCode.Name = "txtResetCode";
            txtResetCode.Size = new Size(294, 36);
            txtResetCode.TabIndex = 4;
            txtResetCode.TextAlign = HorizontalAlignment.Center;
            // 
            // lblResetCode
            // 
            lblResetCode.AutoSize = true;
            lblResetCode.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblResetCode.ForeColor = Color.FromArgb(92, 45, 58);
            lblResetCode.Location = new Point(43, 144);
            lblResetCode.Name = "lblResetCode";
            lblResetCode.Size = new Size(117, 19);
            lblResetCode.TabIndex = 3;
            lblResetCode.Text = "Verification Code";
            // 
            // lblResetGuide
            // 
            lblResetGuide.ForeColor = Color.FromArgb(120, 90, 108);
            lblResetGuide.Location = new Point(43, 80);
            lblResetGuide.Name = "lblResetGuide";
            lblResetGuide.Size = new Size(294, 44);
            lblResetGuide.TabIndex = 2;
            lblResetGuide.Text = "Enter the 6-digit code sent to your school email to continue resetting your password.";
            // 
            // lblResetVerifyTitle
            // 
            lblResetVerifyTitle.AutoSize = false;
            lblResetVerifyTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblResetVerifyTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblResetVerifyTitle.Location = new Point(0, 20);
            lblResetVerifyTitle.Name = "lblResetVerifyTitle";
            lblResetVerifyTitle.Size = new Size(380, 45);
            lblResetVerifyTitle.TabIndex = 0;
            lblResetVerifyTitle.Text = "Verify Reset";
            lblResetVerifyTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // glassNewPassword
            // 
            glassNewPassword.BackColor = Color.FromArgb(80, 255, 255, 255);
            glassNewPassword.Controls.Add(btnBackToResetVerify);
            glassNewPassword.Controls.Add(btnSaveNewPassword);
            glassNewPassword.Controls.Add(txtConfirmNewPassword);
            glassNewPassword.Controls.Add(lblConfirmNewPassword);
            glassNewPassword.Controls.Add(txtNewPassword);
            glassNewPassword.Controls.Add(lblNewPassword);
            glassNewPassword.Controls.Add(lblNewPasswordTitle);
            glassNewPassword.Location = new Point(703, 120);
            glassNewPassword.Name = "glassNewPassword";
            glassNewPassword.Size = new Size(380, 350);
            glassNewPassword.TabIndex = 5;
            glassNewPassword.Visible = false;
            // 
            // btnBackToResetVerify
            // 
            btnBackToResetVerify.BackColor = Color.FromArgb(190, 170, 205);
            btnBackToResetVerify.FlatAppearance.BorderSize = 0;
            btnBackToResetVerify.FlatStyle = FlatStyle.Flat;
            btnBackToResetVerify.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnBackToResetVerify.ForeColor = Color.White;
            btnBackToResetVerify.Location = new Point(43, 256);
            btnBackToResetVerify.Name = "btnBackToResetVerify";
            btnBackToResetVerify.Size = new Size(120, 40);
            btnBackToResetVerify.TabIndex = 5;
            btnBackToResetVerify.Text = "Back";
            btnBackToResetVerify.UseVisualStyleBackColor = false;
            btnBackToResetVerify.Click += btnBackToResetVerify_Click;
            // 
            // btnSaveNewPassword
            // 
            btnSaveNewPassword.BackColor = Color.FromArgb(212, 168, 45);
            btnSaveNewPassword.FlatAppearance.BorderSize = 0;
            btnSaveNewPassword.FlatStyle = FlatStyle.Flat;
            btnSaveNewPassword.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnSaveNewPassword.ForeColor = Color.White;

            btnSaveNewPassword.Location = new Point(177, 256);
            btnSaveNewPassword.Name = "btnSaveNewPassword";
            btnSaveNewPassword.Size = new Size(160, 40);
            btnSaveNewPassword.TabIndex = 4;
            btnSaveNewPassword.Text = "Save New Password";
            btnSaveNewPassword.UseVisualStyleBackColor = false;
            btnSaveNewPassword.Click += btnSaveNewPassword_Click;
            // 
            // txtConfirmNewPassword
            // 
            txtConfirmNewPassword.Font = new Font("Segoe UI", 10.5F);
            txtConfirmNewPassword.Location = new Point(43, 188);
            txtConfirmNewPassword.Name = "txtConfirmNewPassword";
            txtConfirmNewPassword.Size = new Size(294, 26);
            txtConfirmNewPassword.TabIndex = 4;
            txtConfirmNewPassword.UseSystemPasswordChar = true;
            // 
            // lblConfirmNewPassword
            // 
            lblConfirmNewPassword.AutoSize = true;
            lblConfirmNewPassword.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblConfirmNewPassword.ForeColor = Color.FromArgb(92, 45, 58);
            lblConfirmNewPassword.Location = new Point(43, 164);
            lblConfirmNewPassword.Name = "lblConfirmNewPassword";
            lblConfirmNewPassword.Size = new Size(123, 19);
            lblConfirmNewPassword.TabIndex = 3;
            lblConfirmNewPassword.Text = "Confirm Password";
            // 
            // txtNewPassword
            // 
            txtNewPassword.Font = new Font("Segoe UI", 10.5F);
            txtNewPassword.Location = new Point(43, 120);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.Size = new Size(294, 26);
            txtNewPassword.TabIndex = 2;
            txtNewPassword.UseSystemPasswordChar = true;
            // 
            // lblNewPassword
            // 
            lblNewPassword.AutoSize = true;
            lblNewPassword.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblNewPassword.ForeColor = Color.FromArgb(92, 45, 58);
            lblNewPassword.Location = new Point(43, 96);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(101, 19);
            lblNewPassword.TabIndex = 1;
            lblNewPassword.Text = "New Password";
            // 
            // lblNewPasswordTitle
            // 
            lblNewPasswordTitle.AutoSize = false;
            lblNewPasswordTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblNewPasswordTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblNewPasswordTitle.Location = new Point(0, 26);
            lblNewPasswordTitle.Name = "lblNewPasswordTitle";
            lblNewPasswordTitle.Size = new Size(380, 45);
            lblNewPasswordTitle.TabIndex = 0;
            lblNewPasswordTitle.Text = "Set New Password";
            lblNewPasswordTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // glassVerify
            // 
            glassVerify.BackColor = Color.FromArgb(165, 255, 251, 252);
            glassVerify.Controls.Add(btnVerify);
            glassVerify.Controls.Add(lblResend);
            glassVerify.Controls.Add(lblTimer);
            glassVerify.Controls.Add(txtCode);
            glassVerify.Controls.Add(lblCode);
            glassVerify.Controls.Add(lblVerifyGuide);
            glassVerify.Controls.Add(lblVerifyTitle);
            glassVerify.Controls.Add(linkReturnVerify);
            glassVerify.Location = new Point(703, 100);
            glassVerify.Name = "glassVerify";
            glassVerify.Size = new Size(395, 380);
            glassVerify.TabIndex = 2;
            glassVerify.Visible = false;
            // 
            // linkReturnVerify
            // 
            linkReturnVerify.ActiveLinkColor = Color.FromArgb(130, 70, 88);
            linkReturnVerify.BackColor = Color.Transparent;
            linkReturnVerify.Font = new Font("Segoe UI", 9.5F);
            linkReturnVerify.LinkColor = Color.FromArgb(92, 45, 58);
            linkReturnVerify.Location = new Point(0, 325);
            linkReturnVerify.Name = "linkReturnVerify";
            linkReturnVerify.Size = new Size(395, 20);
            linkReturnVerify.TabIndex = 20;
            linkReturnVerify.TabStop = true;
            linkReturnVerify.Text = "Return";
            linkReturnVerify.TextAlign = ContentAlignment.MiddleCenter;
            linkReturnVerify.VisitedLinkColor = Color.FromArgb(92, 45, 58);
            linkReturnVerify.LinkClicked += linkReturnVerify_LinkClicked;
            // 
            // frmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1150, 600);
            Controls.Add(glassNewPassword);
            Controls.Add(glassResetVerify);
            Controls.Add(glassForgot);
            Controls.Add(glassVerify);
            Controls.Add(glassCreate);
            Controls.Add(glassLogin);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WildcatHub - Login";
            glassLogin.ResumeLayout(false);
            glassLogin.PerformLayout();
            glassCreate.ResumeLayout(false);
            glassCreate.PerformLayout();
            glassForgot.ResumeLayout(false);
            glassForgot.PerformLayout();
            glassResetVerify.ResumeLayout(false);
            glassResetVerify.PerformLayout();
            glassNewPassword.ResumeLayout(false);
            glassNewPassword.PerformLayout();
            glassVerify.ResumeLayout(false);
            glassVerify.PerformLayout();
            ResumeLayout(false);
        }

        #endregion


        private Panel glassLogin;
        private Button btnGoToCreate;
        private Button btnLogin;
        private TextBox txtPassword_Login;
        private Label lblPasswordLogin;
        private TextBox txtSchoolID_Login;
        private Label lblSchoolIDLogin;
        private Label lblLoginTitle;
        private Panel glassCreate;
        private Button btnBackToLogin;
        private Button btnCreateAccount;
        private TextBox txtConfirmPassword;
        private Label lblConfirmPassword;
        private TextBox txtPassword;
        private Label lblPassword;
        private TextBox txtEmail;
        private Label lblEmail;
        private TextBox txtSchoolID;
        private Label lblSchoolID;
        private Label lblCreateTitle;
        private Button btnBackToCreate;
        private Button btnVerify;
        private LinkLabel lblResend;
        private Label lblTimer;
        private TextBox txtCode;
        private Label lblCode;
        private Label lblVerifyGuide;
        private Label lblVerifyTitle;
        private CheckBox chkShowPasswordLogin;
        private CheckBox chkShowPasswordCreate;
        private Panel glassForgot;
        private Button btnBackFromForgot;
        private Button btnSendResetCode;
        private TextBox txtForgotEmail;
        private Label lblForgotEmail;
        private TextBox txtForgotSchoolID;
        private Label lblForgotSchoolID;
        private Label lblForgotTitle;
        private Panel glassResetVerify;
        private Button btnBackToForgot;
        private Button btnVerifyResetCode;
        private LinkLabel lblResetResend;
        private Label lblResetTimer;
        private TextBox txtResetCode;
        private Label lblResetCode;
        private Label lblResetGuide;
        private Label lblResetVerifyTitle;
        private Panel glassNewPassword;
        private Button btnBackToResetVerify;
        private Button btnSaveNewPassword;
        private TextBox txtNewPassword;
        private Label lblNewPassword;
        private TextBox txtConfirmNewPassword;
        private Label lblConfirmNewPassword;
        private Label lblNewPasswordTitle;
        private LinkLabel lblForgotPassword;
        private Panel glassVerify;
        private LinkLabel linkCreateAccount;
        private LinkLabel linkBackToLogin;
        private LinkLabel linkReturnVerify;
        private Label lblNoAccount;
        private Label lblHasAccount;
    }
}
