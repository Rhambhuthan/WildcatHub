using System.Windows.Forms.DataVisualization.Charting;

namespace WildcatHub
{
    partial class frmUserDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUserDashboard));
            txtUserEquipmentSearch = new TextBox();
            sidebarPanel = new Panel();
            btnLogout = new Button();
            btnNavProfile = new Button();
            btnNavHistory = new Button();
            btnNavBorrowed = new Button();
            btnNavEquipment = new Button();
            btnNavDashboard = new Button();
            topPanel = new Panel();
            lblPageTitle = new Label();
            contentPanel = new Panel();
            panelProfile = new Panel();
            pnlProfileCard = new Panel();
            linkChangePassword = new LinkLabel();
            lblProfileStatusValue = new Label();
            lblProfileStatusTitle = new Label();
            lblProfileEmailValue = new Label();
            lblProfileEmailTitle = new Label();
            lblProfileSchoolIdValue = new Label();
            lblProfileSchoolIdTitle = new Label();
            lblProfileNameValue = new Label();
            lblProfileNameTitle = new Label();
            panelHistory = new Panel();
            pnlHistoryCard = new Panel();
            dgvHistory = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            lblHistoryTitle = new Label();
            panelBorrowed = new Panel();
            pnlBorrowedPopup = new Panel();
            btnCloseBorrowedPopup = new Button();
            lblPopupPenaltyValue = new Label();
            lblPopupPenaltyTitle = new Label();
            lblPopupDueDateValue = new Label();
            lblPopupDueDateTitle = new Label();
            lblPopupBorrowedDateValue = new Label();
            lblPopupBorrowedDateTitle = new Label();
            lblPopupQuantityValue = new Label();
            lblPopupQuantityTitle = new Label();
            lblPopupItemName = new Label();
            pnlBorrowedEmptyState = new Panel();
            lblBorrowedEmptySub = new Label();
            lblBorrowedEmptyTitle = new Label();
            pnlBorrowedHeader = new Panel();
            lblBorrowedHeaderSub = new Label();
            lblBorrowedHeaderTitle = new Label();
            flowBorrowedItems = new FlowLayoutPanel();
            borrowedItem1 = new Panel();
            lblBorrowedItem1 = new Label();
            picBorrowed1 = new PictureBox();
            borrowedItem2 = new Panel();
            lblBorrowedItem2 = new Label();
            picBorrowed2 = new PictureBox();
            borrowedItem3 = new Panel();
            lblBorrowedItem3 = new Label();
            picBorrowed3 = new PictureBox();
            borrowedItem4 = new Panel();
            lblBorrowedItem4 = new Label();
            picBorrowed4 = new PictureBox();
            borrowedItem5 = new Panel();
            lblBorrowedItem5 = new Label();
            picBorrowed5 = new PictureBox();
            panelEquipment = new Panel();
            pnlEquipmentHeader = new Panel();
            btnCatGeneral = new Button();
            btnCatSports = new Button();
            btnCatScience = new Button();
            btnCatTechnical = new Button();
            btnCatAll = new Button();
            flowEquipmentCards = new FlowLayoutPanel();
            panelDashboard = new Panel();
            pnlStatistics = new Panel();
            lblStatisticsSub = new Label();
            lblStatisticsTitle = new Label();
            chartPlaceholder = new Panel();
            lblWeek4 = new Label();
            lblWeek3 = new Label();
            lblWeek2 = new Label();
            lblWeek1 = new Label();
            pnlReminders = new Panel();
            lblReminder3 = new Label();
            lblReminder2 = new Label();
            lblReminder1 = new Label();
            flowReminderCards = new FlowLayoutPanel();
            lblRemindersTitle = new Label();
            cardHistory = new Panel();
            lblHistorySubCard = new Label();
            lblHistoryValue = new Label();
            lblHistoryCardTitle = new Label();
            cardOverdue = new Panel();
            lblOverdueSubCard = new Label();
            lblOverdueValue = new Label();
            lblOverdueTitle = new Label();
            cardDueSoon = new Panel();
            lblDueSoonSubCard = new Label();
            lblDueSoonValue = new Label();
            lblDueSoonTitle = new Label();
            cardBorrowed = new Panel();
            lblBorrowedSubCard = new Label();
            lblBorrowedCardValue = new Label();
            lblBorrowedCardTitle = new Label();
            pnlWelcome = new Panel();
            lblWelcomeSub = new Label();
            lblWelcomeTitle = new Label();
            sidebarPanel.SuspendLayout();
            topPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            panelProfile.SuspendLayout();
            pnlProfileCard.SuspendLayout();
            panelHistory.SuspendLayout();
            pnlHistoryCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            panelBorrowed.SuspendLayout();
            pnlBorrowedPopup.SuspendLayout();
            pnlBorrowedEmptyState.SuspendLayout();
            pnlBorrowedHeader.SuspendLayout();
            flowBorrowedItems.SuspendLayout();
            borrowedItem1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed1).BeginInit();
            borrowedItem2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed2).BeginInit();
            borrowedItem3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed3).BeginInit();
            borrowedItem4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed4).BeginInit();
            borrowedItem5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed5).BeginInit();
            panelEquipment.SuspendLayout();
            pnlEquipmentHeader.SuspendLayout();
            panelDashboard.SuspendLayout();
            pnlStatistics.SuspendLayout();
            chartPlaceholder.SuspendLayout();
            pnlReminders.SuspendLayout();
            cardHistory.SuspendLayout();
            cardOverdue.SuspendLayout();
            cardDueSoon.SuspendLayout();
            cardBorrowed.SuspendLayout();
            pnlWelcome.SuspendLayout();
            SuspendLayout();
            // 
            // txtUserEquipmentSearch
            // 
            txtUserEquipmentSearch.BorderStyle = BorderStyle.FixedSingle;
            txtUserEquipmentSearch.Font = new Font("Segoe UI", 10F);
            txtUserEquipmentSearch.Location = new Point(24, 62);
            txtUserEquipmentSearch.Name = "txtUserEquipmentSearch";
            txtUserEquipmentSearch.PlaceholderText = "Search equipment...";
            txtUserEquipmentSearch.Size = new Size(360, 25);
            txtUserEquipmentSearch.TabIndex = 5;
            // 
            // sidebarPanel
            // 
            sidebarPanel.BackColor = Color.FromArgb(153, 0, 0); // deep red
            sidebarPanel.BackgroundImage = (Image)resources.GetObject("sidebarPanel.BackgroundImage");
            sidebarPanel.Controls.Add(btnLogout);
            sidebarPanel.Controls.Add(btnNavProfile);
            sidebarPanel.Controls.Add(btnNavHistory);
            sidebarPanel.Controls.Add(btnNavBorrowed);
            sidebarPanel.Controls.Add(btnNavEquipment);
            sidebarPanel.Controls.Add(btnNavDashboard);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(248, 749);
            sidebarPanel.TabIndex = 0;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.FromArgb(212, 168, 45);
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseDownBackColor = Color.FromArgb(214, 197, 224);
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 209, 220);
            btnLogout.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(24, 661);
            btnLogout.Name = "btnLogout";
            btnLogout.Padding = new Padding(18, 0, 0, 0);
            btnLogout.Size = new Size(211, 49);
            btnLogout.TabIndex = 7;
            btnLogout.Text = "↩  Logout";
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnNavProfile
            // 
            btnNavProfile.BackColor = Color.FromArgb(229, 212, 236);
            btnNavProfile.FlatAppearance.BorderSize = 0;
            btnNavProfile.FlatAppearance.MouseDownBackColor = Color.FromArgb(156, 119, 181);
            btnNavProfile.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 200, 229);
            btnNavProfile.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavProfile.ForeColor = Color.FromArgb(87, 60, 99);
            btnNavProfile.Location = new Point(24, 456);
            btnNavProfile.Name = "btnNavProfile";
            btnNavProfile.Padding = new Padding(18, 0, 0, 0);
            btnNavProfile.Size = new Size(211, 49);
            btnNavProfile.TabIndex = 6;
            btnNavProfile.Text = "👤  Profile";
            btnNavProfile.TextAlign = ContentAlignment.MiddleLeft;
            btnNavProfile.UseVisualStyleBackColor = false;
            btnNavProfile.Click += btnNavProfile_Click;
            // 
            // btnNavHistory
            // 
            btnNavHistory.BackColor = Color.FromArgb(229, 212, 236);
            btnNavHistory.FlatAppearance.BorderSize = 0;
            btnNavHistory.FlatAppearance.MouseDownBackColor = Color.FromArgb(156, 119, 181);
            btnNavHistory.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 200, 229);
            btnNavHistory.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavHistory.ForeColor = Color.FromArgb(87, 60, 99);
            btnNavHistory.Location = new Point(24, 380);
            btnNavHistory.Name = "btnNavHistory";
            btnNavHistory.Padding = new Padding(18, 0, 0, 0);
            btnNavHistory.Size = new Size(211, 49);
            btnNavHistory.TabIndex = 5;
            btnNavHistory.Text = "🕘  History";
            btnNavHistory.TextAlign = ContentAlignment.MiddleLeft;
            btnNavHistory.UseVisualStyleBackColor = false;
            btnNavHistory.Click += btnNavHistory_Click;
            // 
            // btnNavBorrowed
            // 
            btnNavBorrowed.BackColor = Color.FromArgb(229, 212, 236);
            btnNavBorrowed.FlatAppearance.BorderSize = 0;
            btnNavBorrowed.FlatAppearance.MouseDownBackColor = Color.FromArgb(156, 119, 181);
            btnNavBorrowed.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 200, 229);
            btnNavBorrowed.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavBorrowed.ForeColor = Color.FromArgb(87, 60, 99);
            btnNavBorrowed.Location = new Point(24, 307);
            btnNavBorrowed.Name = "btnNavBorrowed";
            btnNavBorrowed.Padding = new Padding(18, 0, 0, 0);
            btnNavBorrowed.Size = new Size(211, 49);
            btnNavBorrowed.TabIndex = 4;
            btnNavBorrowed.Text = "📚  Borrowing";
            btnNavBorrowed.TextAlign = ContentAlignment.MiddleLeft;
            btnNavBorrowed.UseVisualStyleBackColor = false;
            btnNavBorrowed.Click += btnNavBorrowed_Click;
            // 
            // btnNavEquipment
            // 
            btnNavEquipment.BackColor = Color.FromArgb(229, 212, 236);
            btnNavEquipment.FlatAppearance.BorderSize = 0;
            btnNavEquipment.FlatAppearance.MouseDownBackColor = Color.FromArgb(156, 119, 181);
            btnNavEquipment.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 200, 229);
            btnNavEquipment.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavEquipment.ForeColor = Color.FromArgb(87, 60, 99);
            btnNavEquipment.Location = new Point(24, 236);
            btnNavEquipment.Name = "btnNavEquipment";
            btnNavEquipment.Padding = new Padding(18, 0, 0, 0);
            btnNavEquipment.Size = new Size(211, 49);
            btnNavEquipment.TabIndex = 3;
            btnNavEquipment.Text = "📦  Equipment";
            btnNavEquipment.TextAlign = ContentAlignment.MiddleLeft;
            btnNavEquipment.UseVisualStyleBackColor = false;
            btnNavEquipment.Click += btnNavEquipment_Click;
            // 
            // btnNavDashboard
            // 
            btnNavDashboard.BackColor = Color.FromArgb(156, 119, 181);
            btnNavDashboard.FlatAppearance.BorderSize = 0;
            btnNavDashboard.FlatAppearance.MouseDownBackColor = Color.FromArgb(156, 119, 181);
            btnNavDashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(156, 119, 181);
            btnNavDashboard.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavDashboard.ForeColor = Color.White;
            btnNavDashboard.Location = new Point(24, 162);
            btnNavDashboard.Name = "btnNavDashboard";
            btnNavDashboard.Padding = new Padding(18, 0, 0, 0);
            btnNavDashboard.Size = new Size(211, 49);
            btnNavDashboard.TabIndex = 2;
            btnNavDashboard.Text = "🏠  Dashboard";
            btnNavDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnNavDashboard.UseVisualStyleBackColor = false;
            btnNavDashboard.Click += btnNavDashboard_Click;
            // 
            // topPanel
            // 
            topPanel.BackColor = Color.FromArgb(250, 245, 247);
            topPanel.Controls.Add(lblPageTitle);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(248, 0);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(1112, 84);
            topPanel.TabIndex = 1;
            // 
            // lblPageTitle
            // 
            lblPageTitle.AutoSize = true;
            lblPageTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblPageTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblPageTitle.Location = new Point(34, 20);
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Size = new Size(184, 45);
            lblPageTitle.TabIndex = 0;
            lblPageTitle.Text = "Dashboard";
            // 
            // contentPanel
            // 
            contentPanel.BackColor = Color.FromArgb(250, 245, 247);
            contentPanel.Controls.Add(panelProfile);
            contentPanel.Controls.Add(panelHistory);
            contentPanel.Controls.Add(panelBorrowed);
            contentPanel.Controls.Add(panelEquipment);
            contentPanel.Controls.Add(panelDashboard);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(248, 84);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1112, 665);
            contentPanel.TabIndex = 2;
            // 
            // panelProfile
            // 
            panelProfile.BackColor = Color.FromArgb(250, 245, 247);
            panelProfile.Controls.Add(pnlProfileCard);
            panelProfile.Dock = DockStyle.Fill;
            panelProfile.Location = new Point(0, 0);
            panelProfile.Name = "panelProfile";
            panelProfile.Size = new Size(1112, 665);
            panelProfile.TabIndex = 4;
            panelProfile.Visible = false;
            // 
            // pnlProfileCard
            // 
            pnlProfileCard.BackColor = Color.FromArgb(255, 251, 252);
            pnlProfileCard.Controls.Add(linkChangePassword);
            pnlProfileCard.Controls.Add(lblProfileStatusValue);
            pnlProfileCard.Controls.Add(lblProfileStatusTitle);
            pnlProfileCard.Controls.Add(lblProfileEmailValue);
            pnlProfileCard.Controls.Add(lblProfileEmailTitle);
            pnlProfileCard.Controls.Add(lblProfileSchoolIdValue);
            pnlProfileCard.Controls.Add(lblProfileSchoolIdTitle);
            pnlProfileCard.Controls.Add(lblProfileNameValue);
            pnlProfileCard.Controls.Add(lblProfileNameTitle);
            pnlProfileCard.Location = new Point(40, 30);
            pnlProfileCard.Name = "pnlProfileCard";
            pnlProfileCard.Size = new Size(520, 360);
            pnlProfileCard.TabIndex = 0;
            // 
            // linkChangePassword
            // 
            linkChangePassword.ActiveLinkColor = Color.FromArgb(156, 119, 181);
            linkChangePassword.AutoSize = true;
            linkChangePassword.Font = new Font("Segoe UI", 10F, FontStyle.Underline);
            linkChangePassword.LinkColor = Color.FromArgb(72, 53, 84);
            linkChangePassword.Location = new Point(30, 311);
            linkChangePassword.Name = "linkChangePassword";
            linkChangePassword.Size = new Size(118, 19);
            linkChangePassword.TabIndex = 8;
            linkChangePassword.TabStop = true;
            linkChangePassword.Text = "Change Password";
            linkChangePassword.VisitedLinkColor = Color.FromArgb(72, 53, 84);
            linkChangePassword.LinkClicked += linkChangePassword_LinkClicked;
            // 
            // lblProfileStatusValue
            // 
            lblProfileStatusValue.AutoSize = true;
            lblProfileStatusValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblProfileStatusValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblProfileStatusValue.Location = new Point(30, 245);
            lblProfileStatusValue.Name = "lblProfileStatusValue";
            lblProfileStatusValue.Size = new Size(70, 21);
            lblProfileStatusValue.TabIndex = 7;
            lblProfileStatusValue.Text = "Verified";
            // 
            // lblProfileStatusTitle
            // 
            lblProfileStatusTitle.AutoSize = true;
            lblProfileStatusTitle.Font = new Font("Segoe UI", 10.5F);
            lblProfileStatusTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblProfileStatusTitle.Location = new Point(30, 221);
            lblProfileStatusTitle.Name = "lblProfileStatusTitle";
            lblProfileStatusTitle.Size = new Size(47, 19);
            lblProfileStatusTitle.TabIndex = 6;
            lblProfileStatusTitle.Text = "Status";
            // 
            // lblProfileEmailValue
            // 
            lblProfileEmailValue.AutoSize = true;
            lblProfileEmailValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblProfileEmailValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblProfileEmailValue.Location = new Point(30, 176);
            lblProfileEmailValue.Name = "lblProfileEmailValue";
            lblProfileEmailValue.Size = new Size(183, 21);
            lblProfileEmailValue.TabIndex = 5;
            lblProfileEmailValue.Text = "student@email.edu.ph";
            // 
            // lblProfileEmailTitle
            // 
            lblProfileEmailTitle.AutoSize = true;
            lblProfileEmailTitle.Font = new Font("Segoe UI", 10.5F);
            lblProfileEmailTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblProfileEmailTitle.Location = new Point(30, 152);
            lblProfileEmailTitle.Name = "lblProfileEmailTitle";
            lblProfileEmailTitle.Size = new Size(41, 19);
            lblProfileEmailTitle.TabIndex = 4;
            lblProfileEmailTitle.Text = "Email";
            // 
            // lblProfileSchoolIdValue
            // 
            lblProfileSchoolIdValue.AutoSize = true;
            lblProfileSchoolIdValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblProfileSchoolIdValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblProfileSchoolIdValue.Location = new Point(30, 107);
            lblProfileSchoolIdValue.Name = "lblProfileSchoolIdValue";
            lblProfileSchoolIdValue.Size = new Size(103, 21);
            lblProfileSchoolIdValue.TabIndex = 3;
            lblProfileSchoolIdValue.Text = "24-0000-000";
            // 
            // lblProfileSchoolIdTitle
            // 
            lblProfileSchoolIdTitle.AutoSize = true;
            lblProfileSchoolIdTitle.Font = new Font("Segoe UI", 10.5F);
            lblProfileSchoolIdTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblProfileSchoolIdTitle.Location = new Point(30, 83);
            lblProfileSchoolIdTitle.Name = "lblProfileSchoolIdTitle";
            lblProfileSchoolIdTitle.Size = new Size(67, 19);
            lblProfileSchoolIdTitle.TabIndex = 2;
            lblProfileSchoolIdTitle.Text = "School ID";
            // 
            // lblProfileNameValue
            // 
            lblProfileNameValue.AutoSize = true;
            lblProfileNameValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblProfileNameValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblProfileNameValue.Location = new Point(30, 39);
            lblProfileNameValue.Name = "lblProfileNameValue";
            lblProfileNameValue.Size = new Size(152, 21);
            lblProfileNameValue.TabIndex = 1;
            lblProfileNameValue.Text = "Student Full Name";
            // 
            // lblProfileNameTitle
            // 
            lblProfileNameTitle.AutoSize = true;
            lblProfileNameTitle.Font = new Font("Segoe UI", 10.5F);
            lblProfileNameTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblProfileNameTitle.Location = new Point(30, 15);
            lblProfileNameTitle.Name = "lblProfileNameTitle";
            lblProfileNameTitle.Size = new Size(45, 19);
            lblProfileNameTitle.TabIndex = 0;
            lblProfileNameTitle.Text = "Name";
            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.FromArgb(250, 245, 247);
            panelHistory.Controls.Add(pnlHistoryCard);
            panelHistory.Dock = DockStyle.Fill;
            panelHistory.Location = new Point(0, 0);
            panelHistory.Name = "panelHistory";
            panelHistory.Size = new Size(1112, 665);
            panelHistory.TabIndex = 3;
            panelHistory.Visible = false;
            // 
            // pnlHistoryCard
            // 
            pnlHistoryCard.BackColor = Color.FromArgb(255, 251, 252);
            pnlHistoryCard.Controls.Add(dgvHistory);
            pnlHistoryCard.Controls.Add(lblHistoryTitle);
            pnlHistoryCard.Location = new Point(40, 30);
            pnlHistoryCard.Name = "pnlHistoryCard";
            pnlHistoryCard.Size = new Size(1030, 590);
            pnlHistoryCard.TabIndex = 0;
            // 
            // dgvHistory
            // 
            dgvHistory.BackgroundColor = Color.FromArgb(255, 251, 252);
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6 });
            dgvHistory.Location = new Point(28, 78);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.Size = new Size(972, 480);
            dgvHistory.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // lblHistoryTitle
            // 
            lblHistoryTitle.AutoSize = true;
            lblHistoryTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblHistoryTitle.Location = new Point(28, 26);
            lblHistoryTitle.Name = "lblHistoryTitle";
            lblHistoryTitle.Size = new Size(98, 32);
            lblHistoryTitle.TabIndex = 0;
            lblHistoryTitle.Text = "History";
            // 
            // panelBorrowed
            // 
            panelBorrowed.BackColor = Color.FromArgb(250, 245, 247);
            panelBorrowed.Controls.Add(pnlBorrowedPopup);
            panelBorrowed.Controls.Add(pnlBorrowedEmptyState);
            panelBorrowed.Controls.Add(pnlBorrowedHeader);
            panelBorrowed.Controls.Add(flowBorrowedItems);
            panelBorrowed.Dock = DockStyle.Fill;
            panelBorrowed.Location = new Point(0, 0);
            panelBorrowed.Name = "panelBorrowed";
            panelBorrowed.Size = new Size(1112, 665);
            panelBorrowed.TabIndex = 2;
            panelBorrowed.Visible = false;
            // 
            // pnlBorrowedPopup
            // 
            pnlBorrowedPopup.BackColor = Color.FromArgb(255, 251, 252);
            pnlBorrowedPopup.Controls.Add(btnCloseBorrowedPopup);
            pnlBorrowedPopup.Controls.Add(lblPopupPenaltyValue);
            pnlBorrowedPopup.Controls.Add(lblPopupPenaltyTitle);
            pnlBorrowedPopup.Controls.Add(lblPopupDueDateValue);
            pnlBorrowedPopup.Controls.Add(lblPopupDueDateTitle);
            pnlBorrowedPopup.Controls.Add(lblPopupBorrowedDateValue);
            pnlBorrowedPopup.Controls.Add(lblPopupBorrowedDateTitle);
            pnlBorrowedPopup.Controls.Add(lblPopupQuantityValue);
            pnlBorrowedPopup.Controls.Add(lblPopupQuantityTitle);
            pnlBorrowedPopup.Controls.Add(lblPopupItemName);
            pnlBorrowedPopup.Location = new Point(316, 188);
            pnlBorrowedPopup.Name = "pnlBorrowedPopup";
            pnlBorrowedPopup.Size = new Size(480, 270);
            pnlBorrowedPopup.TabIndex = 3;
            pnlBorrowedPopup.Visible = false;
            // 
            // btnCloseBorrowedPopup
            // 
            btnCloseBorrowedPopup.BackColor = Color.Transparent;
            btnCloseBorrowedPopup.FlatAppearance.BorderSize = 0;
            btnCloseBorrowedPopup.FlatStyle = FlatStyle.Flat;
            btnCloseBorrowedPopup.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            btnCloseBorrowedPopup.ForeColor = Color.FromArgb(220, 95, 107);
            btnCloseBorrowedPopup.Location = new Point(383, 8);
            btnCloseBorrowedPopup.Name = "btnCloseBorrowedPopup";
            btnCloseBorrowedPopup.Size = new Size(34, 34);
            btnCloseBorrowedPopup.TabIndex = 9;
            btnCloseBorrowedPopup.Text = "×";
            btnCloseBorrowedPopup.UseVisualStyleBackColor = false;
            btnCloseBorrowedPopup.Click += btnCloseBorrowedPopup_Click;
            // 
            // lblPopupPenaltyValue
            // 
            lblPopupPenaltyValue.AutoSize = true;
            lblPopupPenaltyValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPopupPenaltyValue.ForeColor = Color.FromArgb(220, 140, 92);
            lblPopupPenaltyValue.Location = new Point(34, 198);
            lblPopupPenaltyValue.Name = "lblPopupPenaltyValue";
            lblPopupPenaltyValue.Size = new Size(33, 21);
            lblPopupPenaltyValue.TabIndex = 8;
            lblPopupPenaltyValue.Text = "₱ 0";
            // 
            // lblPopupPenaltyTitle
            // 
            lblPopupPenaltyTitle.AutoSize = true;
            lblPopupPenaltyTitle.Font = new Font("Segoe UI", 10F);
            lblPopupPenaltyTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblPopupPenaltyTitle.Location = new Point(34, 176);
            lblPopupPenaltyTitle.Name = "lblPopupPenaltyTitle";
            lblPopupPenaltyTitle.Size = new Size(86, 19);
            lblPopupPenaltyTitle.TabIndex = 7;
            lblPopupPenaltyTitle.Text = "Total Penalty";
            // 
            // lblPopupDueDateValue
            // 
            lblPopupDueDateValue.AutoSize = true;
            lblPopupDueDateValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPopupDueDateValue.ForeColor = Color.FromArgb(220, 95, 107);
            lblPopupDueDateValue.Location = new Point(224, 130);
            lblPopupDueDateValue.Name = "lblPopupDueDateValue";
            lblPopupDueDateValue.Size = new Size(109, 21);
            lblPopupDueDateValue.TabIndex = 6;
            lblPopupDueDateValue.Text = "May 15, 2026";
            // 
            // lblPopupDueDateTitle
            // 
            lblPopupDueDateTitle.AutoSize = true;
            lblPopupDueDateTitle.Font = new Font("Segoe UI", 10F);
            lblPopupDueDateTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblPopupDueDateTitle.Location = new Point(224, 108);
            lblPopupDueDateTitle.Name = "lblPopupDueDateTitle";
            lblPopupDueDateTitle.Size = new Size(67, 19);
            lblPopupDueDateTitle.TabIndex = 5;
            lblPopupDueDateTitle.Text = "Due Date";
            // 
            // lblPopupBorrowedDateValue
            // 
            lblPopupBorrowedDateValue.AutoSize = true;
            lblPopupBorrowedDateValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPopupBorrowedDateValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblPopupBorrowedDateValue.Location = new Point(34, 130);
            lblPopupBorrowedDateValue.Name = "lblPopupBorrowedDateValue";
            lblPopupBorrowedDateValue.Size = new Size(109, 21);
            lblPopupBorrowedDateValue.TabIndex = 4;
            lblPopupBorrowedDateValue.Text = "May 08, 2026";
            // 
            // lblPopupBorrowedDateTitle
            // 
            lblPopupBorrowedDateTitle.AutoSize = true;
            lblPopupBorrowedDateTitle.Font = new Font("Segoe UI", 10F);
            lblPopupBorrowedDateTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblPopupBorrowedDateTitle.Location = new Point(34, 108);
            lblPopupBorrowedDateTitle.Name = "lblPopupBorrowedDateTitle";
            lblPopupBorrowedDateTitle.Size = new Size(101, 19);
            lblPopupBorrowedDateTitle.TabIndex = 3;
            lblPopupBorrowedDateTitle.Text = "Date Borrowed";
            // 
            // lblPopupQuantityValue
            // 
            lblPopupQuantityValue.AutoSize = true;
            lblPopupQuantityValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPopupQuantityValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblPopupQuantityValue.Location = new Point(34, 74);
            lblPopupQuantityValue.Name = "lblPopupQuantityValue";
            lblPopupQuantityValue.Size = new Size(19, 21);
            lblPopupQuantityValue.TabIndex = 2;
            lblPopupQuantityValue.Text = "1";
            // 
            // lblPopupQuantityTitle
            // 
            lblPopupQuantityTitle.AutoSize = true;
            lblPopupQuantityTitle.Font = new Font("Segoe UI", 10F);
            lblPopupQuantityTitle.ForeColor = Color.FromArgb(126, 105, 136);
            lblPopupQuantityTitle.Location = new Point(34, 52);
            lblPopupQuantityTitle.Name = "lblPopupQuantityTitle";
            lblPopupQuantityTitle.Size = new Size(63, 19);
            lblPopupQuantityTitle.TabIndex = 1;
            lblPopupQuantityTitle.Text = "Quantity";
            // 
            // lblPopupItemName
            // 
            lblPopupItemName.AutoSize = true;
            lblPopupItemName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblPopupItemName.ForeColor = Color.FromArgb(72, 53, 84);
            lblPopupItemName.Location = new Point(32, 16);
            lblPopupItemName.Name = "lblPopupItemName";
            lblPopupItemName.Size = new Size(61, 30);
            lblPopupItemName.TabIndex = 0;
            lblPopupItemName.Text = "Item";
            // 
            // pnlBorrowedEmptyState
            // 
            pnlBorrowedEmptyState.BackColor = Color.FromArgb(255, 251, 252);
            pnlBorrowedEmptyState.Controls.Add(lblBorrowedEmptySub);
            pnlBorrowedEmptyState.Controls.Add(lblBorrowedEmptyTitle);
            pnlBorrowedEmptyState.Location = new Point(40, 138);
            pnlBorrowedEmptyState.Name = "pnlBorrowedEmptyState";
            pnlBorrowedEmptyState.Size = new Size(1030, 470);
            pnlBorrowedEmptyState.TabIndex = 2;
            // 
            // lblBorrowedEmptySub
            // 
            lblBorrowedEmptySub.AutoSize = true;
            lblBorrowedEmptySub.Font = new Font("Segoe UI", 12F);
            lblBorrowedEmptySub.ForeColor = Color.FromArgb(126, 105, 136);
            lblBorrowedEmptySub.Location = new Point(40, 98);
            lblBorrowedEmptySub.Name = "lblBorrowedEmptySub";
            lblBorrowedEmptySub.Size = new Size(238, 21);
            lblBorrowedEmptySub.TabIndex = 1;
            lblBorrowedEmptySub.Text = "Borrowing items will appear here.";
            // 
            // lblBorrowedEmptyTitle
            // 
            lblBorrowedEmptyTitle.AutoSize = true;
            lblBorrowedEmptyTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblBorrowedEmptyTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedEmptyTitle.Location = new Point(40, 58);
            lblBorrowedEmptyTitle.Name = "lblBorrowedEmptyTitle";
            lblBorrowedEmptyTitle.Size = new Size(236, 32);
            lblBorrowedEmptyTitle.TabIndex = 0;
            lblBorrowedEmptyTitle.Text = "No borrowing items";
            // 
            // pnlBorrowedHeader
            // 
            pnlBorrowedHeader.BackColor = Color.FromArgb(255, 251, 252);
            pnlBorrowedHeader.Controls.Add(lblBorrowedHeaderSub);
            pnlBorrowedHeader.Controls.Add(lblBorrowedHeaderTitle);
            pnlBorrowedHeader.Location = new Point(40, 26);
            pnlBorrowedHeader.Name = "pnlBorrowedHeader";
            pnlBorrowedHeader.Size = new Size(1030, 82);
            pnlBorrowedHeader.TabIndex = 1;
            // 
            // lblBorrowedHeaderSub
            // 
            lblBorrowedHeaderSub.AutoSize = true;
            lblBorrowedHeaderSub.Font = new Font("Segoe UI", 11F);
            lblBorrowedHeaderSub.ForeColor = Color.FromArgb(126, 105, 136);
            lblBorrowedHeaderSub.Location = new Point(26, 46);
            lblBorrowedHeaderSub.Name = "lblBorrowedHeaderSub";
            lblBorrowedHeaderSub.Size = new Size(291, 20);
            lblBorrowedHeaderSub.TabIndex = 1;
            lblBorrowedHeaderSub.Text = "Click an item card to see borrowing details.";
            // 
            // lblBorrowedHeaderTitle
            // 
            lblBorrowedHeaderTitle.AutoSize = true;
            lblBorrowedHeaderTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblBorrowedHeaderTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedHeaderTitle.Location = new Point(26, 14);
            lblBorrowedHeaderTitle.Name = "lblBorrowedHeaderTitle";
            lblBorrowedHeaderTitle.Size = new Size(126, 32);
            lblBorrowedHeaderTitle.TabIndex = 0;
            lblBorrowedHeaderTitle.Text = "Borrowing";
            // 
            // flowBorrowedItems
            // 
            flowBorrowedItems.AutoScroll = true;
            flowBorrowedItems.Controls.Add(borrowedItem1);
            flowBorrowedItems.Controls.Add(borrowedItem2);
            flowBorrowedItems.Controls.Add(borrowedItem3);
            flowBorrowedItems.Controls.Add(borrowedItem4);
            flowBorrowedItems.Controls.Add(borrowedItem5);
            flowBorrowedItems.Location = new Point(40, 138);
            flowBorrowedItems.Name = "flowBorrowedItems";
            flowBorrowedItems.Padding = new Padding(10);
            flowBorrowedItems.Size = new Size(1030, 470);
            flowBorrowedItems.TabIndex = 0;
            flowBorrowedItems.Visible = false;
            // 
            // borrowedItem1
            // 
            borrowedItem1.BackColor = Color.FromArgb(255, 251, 252);
            borrowedItem1.Controls.Add(lblBorrowedItem1);
            borrowedItem1.Controls.Add(picBorrowed1);
            borrowedItem1.Cursor = Cursors.Hand;
            borrowedItem1.Location = new Point(20, 20);
            borrowedItem1.Margin = new Padding(10);
            borrowedItem1.Name = "borrowedItem1";
            borrowedItem1.Size = new Size(220, 240);
            borrowedItem1.TabIndex = 0;
            borrowedItem1.Click += BorrowedItem_Click;
            // 
            // lblBorrowedItem1
            // 
            lblBorrowedItem1.AutoSize = true;
            lblBorrowedItem1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedItem1.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedItem1.Location = new Point(20, 194);
            lblBorrowedItem1.Name = "lblBorrowedItem1";
            lblBorrowedItem1.Size = new Size(56, 20);
            lblBorrowedItem1.TabIndex = 1;
            lblBorrowedItem1.Text = "Mouse";
            lblBorrowedItem1.Click += BorrowedItem_Click;
            // 
            // picBorrowed1
            // 
            picBorrowed1.BackColor = Color.FromArgb(243, 236, 245);
            picBorrowed1.Location = new Point(20, 18);
            picBorrowed1.Name = "picBorrowed1";
            picBorrowed1.Size = new Size(180, 160);
            picBorrowed1.SizeMode = PictureBoxSizeMode.Zoom;
            picBorrowed1.TabIndex = 0;
            picBorrowed1.TabStop = false;
            picBorrowed1.Click += BorrowedItem_Click;
            // 
            // borrowedItem2
            // 
            borrowedItem2.BackColor = Color.FromArgb(255, 251, 252);
            borrowedItem2.Controls.Add(lblBorrowedItem2);
            borrowedItem2.Controls.Add(picBorrowed2);
            borrowedItem2.Cursor = Cursors.Hand;
            borrowedItem2.Location = new Point(260, 20);
            borrowedItem2.Margin = new Padding(10);
            borrowedItem2.Name = "borrowedItem2";
            borrowedItem2.Size = new Size(220, 240);
            borrowedItem2.TabIndex = 1;
            borrowedItem2.Click += BorrowedItem_Click;
            // 
            // lblBorrowedItem2
            // 
            lblBorrowedItem2.AutoSize = true;
            lblBorrowedItem2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedItem2.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedItem2.Location = new Point(20, 194);
            lblBorrowedItem2.Name = "lblBorrowedItem2";
            lblBorrowedItem2.Size = new Size(58, 20);
            lblBorrowedItem2.TabIndex = 1;
            lblBorrowedItem2.Text = "Laptop";
            lblBorrowedItem2.Click += BorrowedItem_Click;
            // 
            // picBorrowed2
            // 
            picBorrowed2.BackColor = Color.FromArgb(243, 236, 245);
            picBorrowed2.Location = new Point(20, 18);
            picBorrowed2.Name = "picBorrowed2";
            picBorrowed2.Size = new Size(180, 160);
            picBorrowed2.SizeMode = PictureBoxSizeMode.Zoom;
            picBorrowed2.TabIndex = 0;
            picBorrowed2.TabStop = false;
            picBorrowed2.Click += BorrowedItem_Click;
            // 
            // borrowedItem3
            // 
            borrowedItem3.BackColor = Color.FromArgb(255, 251, 252);
            borrowedItem3.Controls.Add(lblBorrowedItem3);
            borrowedItem3.Controls.Add(picBorrowed3);
            borrowedItem3.Cursor = Cursors.Hand;
            borrowedItem3.Location = new Point(500, 20);
            borrowedItem3.Margin = new Padding(10);
            borrowedItem3.Name = "borrowedItem3";
            borrowedItem3.Size = new Size(220, 240);
            borrowedItem3.TabIndex = 2;
            borrowedItem3.Click += BorrowedItem_Click;
            // 
            // lblBorrowedItem3
            // 
            lblBorrowedItem3.AutoSize = true;
            lblBorrowedItem3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedItem3.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedItem3.Location = new Point(20, 194);
            lblBorrowedItem3.Name = "lblBorrowedItem3";
            lblBorrowedItem3.Size = new Size(56, 20);
            lblBorrowedItem3.TabIndex = 1;
            lblBorrowedItem3.Text = "Sensor";
            lblBorrowedItem3.Click += BorrowedItem_Click;
            // 
            // picBorrowed3
            // 
            picBorrowed3.BackColor = Color.FromArgb(243, 236, 245);
            picBorrowed3.Location = new Point(20, 18);
            picBorrowed3.Name = "picBorrowed3";
            picBorrowed3.Size = new Size(180, 160);
            picBorrowed3.SizeMode = PictureBoxSizeMode.Zoom;
            picBorrowed3.TabIndex = 0;
            picBorrowed3.TabStop = false;
            picBorrowed3.Click += BorrowedItem_Click;
            // 
            // borrowedItem4
            // 
            borrowedItem4.BackColor = Color.FromArgb(255, 251, 252);
            borrowedItem4.Controls.Add(lblBorrowedItem4);
            borrowedItem4.Controls.Add(picBorrowed4);
            borrowedItem4.Cursor = Cursors.Hand;
            borrowedItem4.Location = new Point(740, 20);
            borrowedItem4.Margin = new Padding(10);
            borrowedItem4.Name = "borrowedItem4";
            borrowedItem4.Size = new Size(220, 240);
            borrowedItem4.TabIndex = 3;
            borrowedItem4.Click += BorrowedItem_Click;
            // 
            // lblBorrowedItem4
            // 
            lblBorrowedItem4.AutoSize = true;
            lblBorrowedItem4.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedItem4.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedItem4.Location = new Point(20, 194);
            lblBorrowedItem4.Name = "lblBorrowedItem4";
            lblBorrowedItem4.Size = new Size(81, 20);
            lblBorrowedItem4.TabIndex = 1;
            lblBorrowedItem4.Text = "Basketball";
            lblBorrowedItem4.Click += BorrowedItem_Click;
            // 
            // picBorrowed4
            // 
            picBorrowed4.BackColor = Color.FromArgb(243, 236, 245);
            picBorrowed4.Location = new Point(20, 18);
            picBorrowed4.Name = "picBorrowed4";
            picBorrowed4.Size = new Size(180, 160);
            picBorrowed4.SizeMode = PictureBoxSizeMode.Zoom;
            picBorrowed4.TabIndex = 0;
            picBorrowed4.TabStop = false;
            picBorrowed4.Click += BorrowedItem_Click;
            // 
            // borrowedItem5
            // 
            borrowedItem5.BackColor = Color.FromArgb(255, 251, 252);
            borrowedItem5.Controls.Add(lblBorrowedItem5);
            borrowedItem5.Controls.Add(picBorrowed5);
            borrowedItem5.Cursor = Cursors.Hand;
            borrowedItem5.Location = new Point(20, 280);
            borrowedItem5.Margin = new Padding(10);
            borrowedItem5.Name = "borrowedItem5";
            borrowedItem5.Size = new Size(220, 240);
            borrowedItem5.TabIndex = 4;
            borrowedItem5.Click += BorrowedItem_Click;
            // 
            // lblBorrowedItem5
            // 
            lblBorrowedItem5.AutoSize = true;
            lblBorrowedItem5.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedItem5.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedItem5.Location = new Point(20, 194);
            lblBorrowedItem5.Name = "lblBorrowedItem5";
            lblBorrowedItem5.Size = new Size(54, 20);
            lblBorrowedItem5.TabIndex = 1;
            lblBorrowedItem5.Text = "Tripod";
            lblBorrowedItem5.Click += BorrowedItem_Click;
            // 
            // picBorrowed5
            // 
            picBorrowed5.BackColor = Color.FromArgb(243, 236, 245);
            picBorrowed5.Location = new Point(20, 18);
            picBorrowed5.Name = "picBorrowed5";
            picBorrowed5.Size = new Size(180, 160);
            picBorrowed5.SizeMode = PictureBoxSizeMode.Zoom;
            picBorrowed5.TabIndex = 0;
            picBorrowed5.TabStop = false;
            picBorrowed5.Click += BorrowedItem_Click;
            // 
            // panelEquipment
            // 
            panelEquipment.BackColor = Color.FromArgb(250, 245, 247);
            panelEquipment.Controls.Add(pnlEquipmentHeader);
            panelEquipment.Controls.Add(flowEquipmentCards);
            panelEquipment.Dock = DockStyle.Fill;
            panelEquipment.Location = new Point(0, 0);
            panelEquipment.Name = "panelEquipment";
            panelEquipment.Size = new Size(1112, 665);
            panelEquipment.TabIndex = 1;
            panelEquipment.Visible = false;
            // 
            // pnlEquipmentHeader
            // 
            pnlEquipmentHeader.BackColor = Color.FromArgb(255, 251, 252);
            pnlEquipmentHeader.Controls.Add(btnCatGeneral);
            pnlEquipmentHeader.Controls.Add(btnCatSports);
            pnlEquipmentHeader.Controls.Add(btnCatScience);
            pnlEquipmentHeader.Controls.Add(btnCatTechnical);
            pnlEquipmentHeader.Controls.Add(btnCatAll);
            pnlEquipmentHeader.Controls.Add(txtUserEquipmentSearch);
            pnlEquipmentHeader.Location = new Point(40, 24);
            pnlEquipmentHeader.Name = "pnlEquipmentHeader";
            pnlEquipmentHeader.Size = new Size(1030, 118);
            pnlEquipmentHeader.TabIndex = 1;
            // 
            // btnCatGeneral
            // 
            btnCatGeneral.BackColor = Color.FromArgb(241, 233, 245);
            btnCatGeneral.FlatAppearance.BorderSize = 0;
            btnCatGeneral.FlatStyle = FlatStyle.Flat;
            btnCatGeneral.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCatGeneral.ForeColor = Color.FromArgb(87, 60, 99);
            btnCatGeneral.Location = new Point(810, 16);
            btnCatGeneral.Name = "btnCatGeneral";
            btnCatGeneral.Size = new Size(192, 36);
            btnCatGeneral.TabIndex = 4;
            btnCatGeneral.Text = "General Equipment";
            btnCatGeneral.UseVisualStyleBackColor = false;
            // 
            // btnCatSports
            // 
            btnCatSports.BackColor = Color.FromArgb(241, 233, 245);
            btnCatSports.FlatAppearance.BorderSize = 0;
            btnCatSports.FlatStyle = FlatStyle.Flat;
            btnCatSports.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCatSports.ForeColor = Color.FromArgb(87, 60, 99);
            btnCatSports.Location = new Point(612, 16);
            btnCatSports.Name = "btnCatSports";
            btnCatSports.Size = new Size(182, 36);
            btnCatSports.TabIndex = 3;
            btnCatSports.Text = "Sports Equipment";
            btnCatSports.UseVisualStyleBackColor = false;
            // 
            // btnCatScience
            // 
            btnCatScience.BackColor = Color.FromArgb(241, 233, 245);
            btnCatScience.FlatAppearance.BorderSize = 0;
            btnCatScience.FlatStyle = FlatStyle.Flat;
            btnCatScience.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCatScience.ForeColor = Color.FromArgb(87, 60, 99);
            btnCatScience.Location = new Point(412, 16);
            btnCatScience.Name = "btnCatScience";
            btnCatScience.Size = new Size(184, 36);
            btnCatScience.TabIndex = 2;
            btnCatScience.Text = "Science Laboratory";
            btnCatScience.UseVisualStyleBackColor = false;
            // 
            // btnCatTechnical
            // 
            btnCatTechnical.BackColor = Color.FromArgb(241, 233, 245);
            btnCatTechnical.FlatAppearance.BorderSize = 0;
            btnCatTechnical.FlatStyle = FlatStyle.Flat;
            btnCatTechnical.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCatTechnical.ForeColor = Color.FromArgb(87, 60, 99);
            btnCatTechnical.Location = new Point(210, 16);
            btnCatTechnical.Name = "btnCatTechnical";
            btnCatTechnical.Size = new Size(186, 36);
            btnCatTechnical.TabIndex = 1;
            btnCatTechnical.Text = "Technical Laboratory";
            btnCatTechnical.UseVisualStyleBackColor = false;
            // 
            // btnCatAll
            // 
            btnCatAll.BackColor = Color.FromArgb(169, 215, 159);
            btnCatAll.FlatAppearance.BorderSize = 0;
            btnCatAll.FlatStyle = FlatStyle.Flat;
            btnCatAll.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCatAll.ForeColor = Color.White;
            btnCatAll.Location = new Point(24, 16);
            btnCatAll.Name = "btnCatAll";
            btnCatAll.Size = new Size(170, 36);
            btnCatAll.TabIndex = 0;
            btnCatAll.Text = "All";
            btnCatAll.UseVisualStyleBackColor = false;
            // 
            // flowEquipmentCards
            // 
            flowEquipmentCards.AutoScroll = true;
            flowEquipmentCards.Location = new Point(40, 162);
            flowEquipmentCards.Name = "flowEquipmentCards";
            flowEquipmentCards.Size = new Size(1030, 464);
            flowEquipmentCards.TabIndex = 0;
            // 
            // panelDashboard
            // 
            panelDashboard.BackColor = Color.FromArgb(250, 245, 247);
            panelDashboard.Controls.Add(pnlStatistics);
            panelDashboard.Controls.Add(pnlReminders);
            panelDashboard.Controls.Add(cardHistory);
            panelDashboard.Controls.Add(cardOverdue);
            panelDashboard.Controls.Add(cardDueSoon);
            panelDashboard.Controls.Add(cardBorrowed);
            panelDashboard.Controls.Add(pnlWelcome);
            panelDashboard.Dock = DockStyle.Fill;
            panelDashboard.Location = new Point(0, 0);
            panelDashboard.Name = "panelDashboard";
            panelDashboard.Size = new Size(1112, 665);
            panelDashboard.TabIndex = 0;
            // 
            // pnlStatistics
            // 
            pnlStatistics.BackColor = Color.FromArgb(255, 251, 252);
            pnlStatistics.Controls.Add(lblStatisticsSub);
            pnlStatistics.Controls.Add(lblStatisticsTitle);
            pnlStatistics.Controls.Add(chartPlaceholder);
            pnlStatistics.Location = new Point(490, 304);
            pnlStatistics.Name = "pnlStatistics";
            pnlStatistics.Size = new Size(580, 330);
            pnlStatistics.TabIndex = 6;
            // 
            // lblStatisticsSub
            // 
            lblStatisticsSub.AutoSize = true;
            lblStatisticsSub.Font = new Font("Segoe UI", 10F);
            lblStatisticsSub.ForeColor = Color.FromArgb(126, 105, 136);
            lblStatisticsSub.Location = new Point(24, 54);
            lblStatisticsSub.Name = "lblStatisticsSub";
            lblStatisticsSub.Size = new Size(346, 19);
            lblStatisticsSub.TabIndex = 1;
            lblStatisticsSub.Text = "Top returned equipment by total quantity";
            // 
            // lblStatisticsTitle
            // 
            lblStatisticsTitle.AutoSize = true;
            lblStatisticsTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblStatisticsTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblStatisticsTitle.Location = new Point(24, 20);
            lblStatisticsTitle.Name = "lblStatisticsTitle";
            lblStatisticsTitle.Size = new Size(308, 31);
            lblStatisticsTitle.TabIndex = 0;
            lblStatisticsTitle.Text = "Equipment Usage Overview";
            // 
            // chartPlaceholder
            // 
            chartPlaceholder.BackColor = Color.FromArgb(246, 239, 248);
            chartPlaceholder.Controls.Add(lblWeek4);
            chartPlaceholder.Controls.Add(lblWeek3);
            chartPlaceholder.Controls.Add(lblWeek2);
            chartPlaceholder.Controls.Add(lblWeek1);
            chartPlaceholder.Location = new Point(24, 78);
            chartPlaceholder.Name = "chartPlaceholder";
            chartPlaceholder.Size = new Size(540, 148);
            chartPlaceholder.TabIndex = 0;
            // 
            // lblWeek4
            // 
            lblWeek4.AutoSize = true;
            lblWeek4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWeek4.ForeColor = Color.FromArgb(126, 105, 136);
            lblWeek4.Location = new Point(446, 116);
            lblWeek4.Name = "lblWeek4";
            lblWeek4.Size = new Size(59, 19);
            lblWeek4.TabIndex = 3;
            lblWeek4.Text = "Week 4";
            // 
            // lblWeek3
            // 
            lblWeek3.AutoSize = true;
            lblWeek3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWeek3.ForeColor = Color.FromArgb(126, 105, 136);
            lblWeek3.Location = new Point(316, 116);
            lblWeek3.Name = "lblWeek3";
            lblWeek3.Size = new Size(59, 19);
            lblWeek3.TabIndex = 2;
            lblWeek3.Text = "Week 3";
            // 
            // lblWeek2
            // 
            lblWeek2.AutoSize = true;
            lblWeek2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWeek2.ForeColor = Color.FromArgb(126, 105, 136);
            lblWeek2.Location = new Point(182, 116);
            lblWeek2.Name = "lblWeek2";
            lblWeek2.Size = new Size(59, 19);
            lblWeek2.TabIndex = 1;
            lblWeek2.Text = "Week 2";
            // 
            // lblWeek1
            // 
            lblWeek1.AutoSize = true;
            lblWeek1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWeek1.ForeColor = Color.FromArgb(126, 105, 136);
            lblWeek1.Location = new Point(50, 116);
            lblWeek1.Name = "lblWeek1";
            lblWeek1.Size = new Size(59, 19);
            lblWeek1.TabIndex = 0;
            lblWeek1.Text = "Week 1";
            // 
            // pnlReminders
            // 
            pnlReminders.BackColor = Color.FromArgb(255, 251, 252);
            pnlReminders.Controls.Add(lblReminder3);
            pnlReminders.Controls.Add(lblReminder2);
            pnlReminders.Controls.Add(lblReminder1);
            pnlReminders.Controls.Add(flowReminderCards);
            pnlReminders.Controls.Add(lblRemindersTitle);
            pnlReminders.Location = new Point(40, 304);
            pnlReminders.Name = "pnlReminders";
            pnlReminders.Size = new Size(430, 330);
            pnlReminders.TabIndex = 5;
            // 
            // lblReminder3
            // 
            lblReminder3.AutoSize = true;
            lblReminder3.Location = new Point(24, 134);
            lblReminder3.Name = "lblReminder3";
            lblReminder3.Size = new Size(104, 15);
            lblReminder3.TabIndex = 3;
            lblReminder3.Text = "• Overdue items: 0";
            lblReminder3.Visible = false;
            // 
            // lblReminder2
            // 
            lblReminder2.AutoSize = true;
            lblReminder2.Location = new Point(24, 104);
            lblReminder2.Name = "lblReminder2";
            lblReminder2.Size = new Size(109, 15);
            lblReminder2.TabIndex = 2;
            lblReminder2.Text = "• Due soon items: 0";
            lblReminder2.Visible = false;
            // 
            // lblReminder1
            // 
            lblReminder1.AutoSize = true;
            lblReminder1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblReminder1.ForeColor = Color.FromArgb(126, 105, 136);
            lblReminder1.Location = new Point(24, 74);
            lblReminder1.Name = "lblReminder1";
            lblReminder1.Size = new Size(184, 20);
            lblReminder1.TabIndex = 1;
            lblReminder1.Text = "• Pending reservations: 0";
            lblReminder1.TextAlign = ContentAlignment.MiddleCenter;
            lblReminder1.Visible = false;
            // 
            // flowReminderCards
            // 
            flowReminderCards.AutoScroll = true;
            flowReminderCards.BackColor = Color.Transparent;
            flowReminderCards.FlowDirection = FlowDirection.TopDown;
            flowReminderCards.Location = new Point(24, 54);
            flowReminderCards.Name = "flowReminderCards";
            flowReminderCards.Size = new Size(382, 250);
            flowReminderCards.TabIndex = 4;
            flowReminderCards.WrapContents = false;
            // 
            // lblRemindersTitle
            // 
            lblRemindersTitle.AutoSize = true;
            lblRemindersTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRemindersTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblRemindersTitle.Location = new Point(24, 22);
            lblRemindersTitle.Name = "lblRemindersTitle";
            lblRemindersTitle.Size = new Size(106, 25);
            lblRemindersTitle.TabIndex = 0;
            lblRemindersTitle.Text = "Reminders";
            // 
            // cardHistory
            // 
            cardHistory.BackColor = Color.FromArgb(255, 251, 252);
            cardHistory.Controls.Add(lblHistorySubCard);
            cardHistory.Controls.Add(lblHistoryValue);
            cardHistory.Controls.Add(lblHistoryCardTitle);
            cardHistory.Location = new Point(829, 176);
            cardHistory.Name = "cardHistory";
            cardHistory.Size = new Size(241, 110);
            cardHistory.TabIndex = 4;
            // 
            // lblHistorySubCard
            // 
            lblHistorySubCard.AutoSize = true;
            lblHistorySubCard.Font = new Font("Segoe UI", 10F);
            lblHistorySubCard.ForeColor = Color.FromArgb(126, 105, 136);
            lblHistorySubCard.Location = new Point(18, 72);
            lblHistorySubCard.Name = "lblHistorySubCard";
            lblHistorySubCard.Size = new Size(107, 19);
            lblHistorySubCard.TabIndex = 2;
            lblHistorySubCard.Text = "Pending records";
            // 
            // lblHistoryValue
            // 
            lblHistoryValue.AutoSize = true;
            lblHistoryValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblHistoryValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblHistoryValue.Location = new Point(18, 34);
            lblHistoryValue.Name = "lblHistoryValue";
            lblHistoryValue.Size = new Size(56, 45);
            lblHistoryValue.TabIndex = 1;
            lblHistoryValue.Text = "00";
            // 
            // lblHistoryCardTitle
            // 
            lblHistoryCardTitle.AutoSize = true;
            lblHistoryCardTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblHistoryCardTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblHistoryCardTitle.Location = new Point(18, 14);
            lblHistoryCardTitle.Name = "lblHistoryCardTitle";
            lblHistoryCardTitle.Size = new Size(100, 20);
            lblHistoryCardTitle.TabIndex = 0;
            lblHistoryCardTitle.Text = "Reservations";
            // 
            // cardOverdue
            // 
            cardOverdue.BackColor = Color.FromArgb(232, 158, 163);
            cardOverdue.Controls.Add(lblOverdueSubCard);
            cardOverdue.Controls.Add(lblOverdueValue);
            cardOverdue.Controls.Add(lblOverdueTitle);
            cardOverdue.Location = new Point(566, 176);
            cardOverdue.Name = "cardOverdue";
            cardOverdue.Size = new Size(245, 110);
            cardOverdue.TabIndex = 3;
            // 
            // lblOverdueSubCard
            // 
            lblOverdueSubCard.AutoSize = true;
            lblOverdueSubCard.Font = new Font("Segoe UI", 10F);
            lblOverdueSubCard.ForeColor = Color.White;
            lblOverdueSubCard.Location = new Point(18, 72);
            lblOverdueSubCard.Name = "lblOverdueSubCard";
            lblOverdueSubCard.Size = new Size(107, 19);
            lblOverdueSubCard.TabIndex = 2;
            lblOverdueSubCard.Text = "Needs attention";
            // 
            // lblOverdueValue
            // 
            lblOverdueValue.AutoSize = true;
            lblOverdueValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblOverdueValue.ForeColor = Color.White;
            lblOverdueValue.Location = new Point(18, 34);
            lblOverdueValue.Name = "lblOverdueValue";
            lblOverdueValue.Size = new Size(56, 45);
            lblOverdueValue.TabIndex = 1;
            lblOverdueValue.Text = "00";
            // 
            // lblOverdueTitle
            // 
            lblOverdueTitle.AutoSize = true;
            lblOverdueTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblOverdueTitle.ForeColor = Color.White;
            lblOverdueTitle.Location = new Point(18, 14);
            lblOverdueTitle.Name = "lblOverdueTitle";
            lblOverdueTitle.Size = new Size(68, 20);
            lblOverdueTitle.TabIndex = 0;
            lblOverdueTitle.Text = "Overdue";
            // 
            // cardDueSoon
            // 
            cardDueSoon.BackColor = Color.FromArgb(255, 251, 252);
            cardDueSoon.Controls.Add(lblDueSoonSubCard);
            cardDueSoon.Controls.Add(lblDueSoonValue);
            cardDueSoon.Controls.Add(lblDueSoonTitle);
            cardDueSoon.Location = new Point(303, 176);
            cardDueSoon.Name = "cardDueSoon";
            cardDueSoon.Size = new Size(245, 110);
            cardDueSoon.TabIndex = 2;
            // 
            // lblDueSoonSubCard
            // 
            lblDueSoonSubCard.AutoSize = true;
            lblDueSoonSubCard.Font = new Font("Segoe UI", 10F);
            lblDueSoonSubCard.ForeColor = Color.FromArgb(126, 105, 136);
            lblDueSoonSubCard.Location = new Point(18, 72);
            lblDueSoonSubCard.Name = "lblDueSoonSubCard";
            lblDueSoonSubCard.Size = new Size(80, 19);
            lblDueSoonSubCard.TabIndex = 2;
            lblDueSoonSubCard.Text = "Due shortly";
            // 
            // lblDueSoonValue
            // 
            lblDueSoonValue.AutoSize = true;
            lblDueSoonValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblDueSoonValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblDueSoonValue.Location = new Point(18, 34);
            lblDueSoonValue.Name = "lblDueSoonValue";
            lblDueSoonValue.Size = new Size(56, 45);
            lblDueSoonValue.TabIndex = 1;
            lblDueSoonValue.Text = "00";
            // 
            // lblDueSoonTitle
            // 
            lblDueSoonTitle.AutoSize = true;
            lblDueSoonTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDueSoonTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblDueSoonTitle.Location = new Point(18, 14);
            lblDueSoonTitle.Name = "lblDueSoonTitle";
            lblDueSoonTitle.Size = new Size(76, 20);
            lblDueSoonTitle.TabIndex = 0;
            lblDueSoonTitle.Text = "Due Soon";
            // 
            // cardBorrowed
            // 
            cardBorrowed.BackColor = Color.FromArgb(255, 251, 252);
            cardBorrowed.Controls.Add(lblBorrowedSubCard);
            cardBorrowed.Controls.Add(lblBorrowedCardValue);
            cardBorrowed.Controls.Add(lblBorrowedCardTitle);
            cardBorrowed.Location = new Point(40, 176);
            cardBorrowed.Name = "cardBorrowed";
            cardBorrowed.Size = new Size(245, 110);
            cardBorrowed.TabIndex = 1;
            // 
            // lblBorrowedSubCard
            // 
            lblBorrowedSubCard.AutoSize = true;
            lblBorrowedSubCard.Font = new Font("Segoe UI", 10F);
            lblBorrowedSubCard.ForeColor = Color.FromArgb(126, 105, 136);
            lblBorrowedSubCard.Location = new Point(18, 72);
            lblBorrowedSubCard.Name = "lblBorrowedSubCard";
            lblBorrowedSubCard.Size = new Size(133, 19);
            lblBorrowedSubCard.TabIndex = 2;
            lblBorrowedSubCard.Text = "Items you have now";
            // 
            // lblBorrowedCardValue
            // 
            lblBorrowedCardValue.AutoSize = true;
            lblBorrowedCardValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblBorrowedCardValue.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedCardValue.Location = new Point(18, 34);
            lblBorrowedCardValue.Name = "lblBorrowedCardValue";
            lblBorrowedCardValue.Size = new Size(56, 45);
            lblBorrowedCardValue.TabIndex = 1;
            lblBorrowedCardValue.Text = "00";
            // 
            // lblBorrowedCardTitle
            // 
            lblBorrowedCardTitle.AutoSize = true;
            lblBorrowedCardTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBorrowedCardTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblBorrowedCardTitle.Location = new Point(18, 14);
            lblBorrowedCardTitle.Name = "lblBorrowedCardTitle";
            lblBorrowedCardTitle.Size = new Size(78, 20);
            lblBorrowedCardTitle.TabIndex = 0;
            lblBorrowedCardTitle.Text = "Borrowing";
            // 
            // pnlWelcome
            // 
            pnlWelcome.BackColor = Color.FromArgb(255, 251, 252);
            pnlWelcome.Controls.Add(lblWelcomeSub);
            pnlWelcome.Controls.Add(lblWelcomeTitle);
            pnlWelcome.Location = new Point(40, 52);
            pnlWelcome.Name = "pnlWelcome";
            pnlWelcome.Size = new Size(1030, 92);
            pnlWelcome.TabIndex = 0;
            // 
            // lblWelcomeSub
            // 
            lblWelcomeSub.AutoSize = true;
            lblWelcomeSub.Font = new Font("Segoe UI", 10.5F);
            lblWelcomeSub.ForeColor = Color.FromArgb(126, 105, 136);
            lblWelcomeSub.Location = new Point(30, 50);
            lblWelcomeSub.Name = "lblWelcomeSub";
            lblWelcomeSub.Size = new Size(315, 19);
            lblWelcomeSub.TabIndex = 1;
            lblWelcomeSub.Text = "Borrow, track, and manage your equipment easily.";
            // 
            // lblWelcomeTitle
            // 
            lblWelcomeTitle.AutoSize = true;
            lblWelcomeTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblWelcomeTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblWelcomeTitle.Location = new Point(24, 16);
            lblWelcomeTitle.Name = "lblWelcomeTitle";
            lblWelcomeTitle.Size = new Size(229, 32);
            lblWelcomeTitle.TabIndex = 0;
            lblWelcomeTitle.Text = "Welcome, Student!";
            // 
            // frmUserDashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 245, 247);
            ClientSize = new Size(1360, 749);
            Controls.Add(contentPanel);
            Controls.Add(topPanel);
            Controls.Add(sidebarPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmUserDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WildcatHub - User Dashboard";
            Load += frmUserDashboard_Load;
            sidebarPanel.ResumeLayout(false);
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            contentPanel.ResumeLayout(false);
            panelProfile.ResumeLayout(false);
            pnlProfileCard.ResumeLayout(false);
            pnlProfileCard.PerformLayout();
            panelHistory.ResumeLayout(false);
            pnlHistoryCard.ResumeLayout(false);
            pnlHistoryCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            panelBorrowed.ResumeLayout(false);
            pnlBorrowedPopup.ResumeLayout(false);
            pnlBorrowedPopup.PerformLayout();
            pnlBorrowedEmptyState.ResumeLayout(false);
            pnlBorrowedEmptyState.PerformLayout();
            pnlBorrowedHeader.ResumeLayout(false);
            pnlBorrowedHeader.PerformLayout();
            flowBorrowedItems.ResumeLayout(false);
            borrowedItem1.ResumeLayout(false);
            borrowedItem1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed1).EndInit();
            borrowedItem2.ResumeLayout(false);
            borrowedItem2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed2).EndInit();
            borrowedItem3.ResumeLayout(false);
            borrowedItem3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed3).EndInit();
            borrowedItem4.ResumeLayout(false);
            borrowedItem4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed4).EndInit();
            borrowedItem5.ResumeLayout(false);
            borrowedItem5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBorrowed5).EndInit();
            panelEquipment.ResumeLayout(false);
            pnlEquipmentHeader.ResumeLayout(false);
            pnlEquipmentHeader.PerformLayout();
            panelDashboard.ResumeLayout(false);
            pnlStatistics.ResumeLayout(false);
            pnlStatistics.PerformLayout();
            chartPlaceholder.ResumeLayout(false);
            chartPlaceholder.PerformLayout();
            pnlReminders.ResumeLayout(false);
            pnlReminders.PerformLayout();
            cardHistory.ResumeLayout(false);
            cardHistory.PerformLayout();
            cardOverdue.ResumeLayout(false);
            cardOverdue.PerformLayout();
            cardDueSoon.ResumeLayout(false);
            cardDueSoon.PerformLayout();
            cardBorrowed.ResumeLayout(false);
            cardBorrowed.PerformLayout();
            pnlWelcome.ResumeLayout(false);
            pnlWelcome.PerformLayout();
            ResumeLayout(false);
        }
        #endregion
        private Chart chartEquipmentUsage;
        private Panel sidebarPanel;
        private Button btnLogout;
        private Button btnNavProfile;
        private Button btnNavHistory;
        private Button btnNavBorrowed;
        private Button btnNavEquipment;
        private Button btnNavDashboard;
        private Panel topPanel;
        private Label lblPageTitle;
        private Panel contentPanel;
        private Panel panelProfile;
        private Panel pnlProfileCard;
        private LinkLabel linkChangePassword;
        private Label lblProfileStatusValue;
        private Label lblProfileStatusTitle;
        private Label lblProfileEmailValue;
        private Label lblProfileEmailTitle;
        private Label lblProfileSchoolIdValue;
        private Label lblProfileSchoolIdTitle;
        private Label lblProfileNameValue;
        private Label lblProfileNameTitle;
        private Panel panelHistory;
        private Panel pnlHistoryCard;
        private DataGridView dgvHistory;
        private Label lblHistoryTitle;
        private Panel panelBorrowed;
        private Panel pnlBorrowedPopup;
        private Button btnCloseBorrowedPopup;
        private Label lblPopupPenaltyValue;
        private Label lblPopupPenaltyTitle;
        private Label lblPopupDueDateValue;
        private Label lblPopupDueDateTitle;
        private Label lblPopupBorrowedDateValue;
        private Label lblPopupBorrowedDateTitle;
        private Label lblPopupQuantityValue;
        private Label lblPopupQuantityTitle;
        private Label lblPopupItemName;
        private Panel pnlBorrowedHeader;
        private Label lblBorrowedHeaderSub;
        private Label lblBorrowedHeaderTitle;
        private Panel pnlBorrowedEmptyState;
        private Label lblBorrowedEmptySub;
        private Label lblBorrowedEmptyTitle;
        private FlowLayoutPanel flowBorrowedItems;
        private Panel borrowedItem1;
        private Label lblBorrowedItem1;
        private PictureBox picBorrowed1;
        private Panel borrowedItem2;
        private Label lblBorrowedItem2;
        private PictureBox picBorrowed2;
        private Panel borrowedItem3;
        private Label lblBorrowedItem3;
        private PictureBox picBorrowed3;
        private Panel borrowedItem4;
        private Label lblBorrowedItem4;
        private PictureBox picBorrowed4;
        private Panel borrowedItem5;
        private Label lblBorrowedItem5;
        private PictureBox picBorrowed5;
        private Panel panelEquipment;
        private Panel pnlEquipmentHeader;
        private Button btnCatGeneral;
        private Button btnCatSports;
        private Button btnCatScience;
        private Button btnCatTechnical;
        private Button btnCatAll;
        private FlowLayoutPanel flowEquipmentCards;
        private Panel panelDashboard;
        private Panel pnlStatistics;
        private Label lblStatisticsSub;
        private Label lblStatisticsTitle;
        private Panel chartPlaceholder;
        private Label lblWeek4;
        private Label lblWeek3;
        private Label lblWeek2;
        private Label lblWeek1;
        private Panel pnlReminders;
        private Label lblReminder3;
        private Label lblReminder2;
        private Label lblReminder1;
        private Label lblRemindersTitle;
        private Panel cardHistory;
        private Label lblHistorySubCard;
        private Label lblHistoryValue;
        private Label lblHistoryCardTitle;
        private Panel cardOverdue;
        private Label lblOverdueSubCard;
        private Label lblOverdueValue;
        private Label lblOverdueTitle;
        private Panel cardDueSoon;
        private Label lblDueSoonSubCard;
        private Label lblDueSoonValue;
        private Label lblDueSoonTitle;
        private Panel cardBorrowed;
        private Label lblBorrowedSubCard;
        private Label lblBorrowedCardValue;
        private Label lblBorrowedCardTitle;
        private Panel pnlWelcome;
        private Label lblWelcomeSub;
        private Label lblWelcomeTitle;
        private FlowLayoutPanel flowReminderCards;
        private TextBox txtUserEquipmentSearch;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}
