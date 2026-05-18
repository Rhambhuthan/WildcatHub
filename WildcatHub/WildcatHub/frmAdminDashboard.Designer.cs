namespace WildcatHub
{
    partial class frmAdminDashboard
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdminDashboard));
            sidebarPanel = new Panel();
            btnLogout = new Button();
            btnNavHistory = new Button();
            btnNavExperimentManuals = new Button();
            btnNavReservations = new Button();
            btnNavBorrowed = new Button();
            btnNavEquipment = new Button();
            btnNavVerification = new Button();
            btnNavDashboard = new Button();
            lblAdminTitle = new Label();
            topPanel = new Panel();
            lblWelcome = new Label();
            contentPanel = new Panel();
            panelHistory = new Panel();
            pnlHistoryMain = new Panel();
            dgvHistory = new DataGridView();
            dataGridViewTextBoxColumn19 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn20 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn21 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn22 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn23 = new DataGridViewTextBoxColumn();
            lblHistoryHeader = new Label();
            panelReservations = new Panel();
            pnlReservationsMain = new Panel();
            pnlReservationStats = new Panel();
            cardResUnclaimed = new Panel();
            lblResUnclaimedTitle = new Label();
            lblResUnclaimedCount = new Label();
            cardResClaimed = new Panel();
            lblResClaimedTitle = new Label();
            lblResClaimedCount = new Label();
            cardResPending = new Panel();
            lblResPendingTitle = new Label();
            lblResPendingCount = new Label();
            btnUnclaimed = new Button();
            btnClaim = new Button();
            lblResShowAll = new Label();
            lblReservationsHeader = new Label();
            flowPendingCards = new FlowLayoutPanel();
            panelBorrowed = new Panel();
            pnlBorrowedMain = new Panel();
            dgvBorrowed = new DataGridView();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn11 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn12 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn13 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn14 = new DataGridViewTextBoxColumn();
            btnReturn = new Button();
            lblBorrowedHeader = new Label();
            panelEquipment = new Panel();
            pnlEquipmentMain = new Panel();
            flowEquipmentCards = new FlowLayoutPanel();
            pnlEqFilters = new Panel();
            btnEqGeneral = new Button();
            btnEqSports = new Button();
            btnEqScience = new Button();
            btnEqTechnical = new Button();
            btnEqAll = new Button();
            lblEquipmentHeader = new Label();
            txtEquipmentAdminSearch = new TextBox();
            cmbEquipmentSubjectFilter = new ComboBox();
            panelVerification = new Panel();
            pnlVerifiedSearch = new Panel();
            pnlStudentReservationsCard = new Panel();
            lblStudentReservationsTitle = new Label();
            lblStudentReservationsCount = new Label();
            pnlStudentReturnedCard = new Panel();
            lblStudentReturnedTitle = new Label();
            lblStudentReturnedCount = new Label();
            pnlStudentBorrowedCard = new Panel();
            lblStudentBorrowedTitle = new Label();
            lblStudentBorrowedCount = new Label();
            lblStudentEmail = new Label();
            lblStudentSchoolID = new Label();
            lblStudentName = new Label();
            txtVerifiedSearch = new TextBox();
            lblVerifiedSearchHeader = new Label();
            pnlPendingList = new Panel();
            dgvPendingUsers = new DataGridView();
            dataGridViewTextBoxColumn15 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn16 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn17 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn18 = new DataGridViewTextBoxColumn();
            lblPendingHeader = new Label();
            txtStudentSearch = new TextBox();
            cmbSubjectFilter = new ComboBox();
            cmbSectionFilter = new ComboBox();
            panelDashboard = new Panel();
            pnlClaimableToday = new Panel();
            lblClaimableSub = new Label();
            lblClaimableValue = new Label();
            lblClaimableHeader = new Label();
            pnlRecentActivity = new Panel();
            flowRecentActivity = new FlowLayoutPanel();
            lblRecentActivityEmpty = new Label();
            lblRecentActivityHeader = new Label();
            pnlStatistics = new Panel();
            chartStats = new System.Windows.Forms.DataVisualization.Charting.Chart();
            lblStatisticsSub = new Label();
            lblStatisticsHeader = new Label();
            cardEquipment = new Panel();
            lblCardEquipmentText = new Label();
            lblCardEquipmentCount = new Label();
            cardOverdue = new Panel();
            lblCardOverdueText = new Label();
            lblCardOverdueCount = new Label();
            cardBorrowed = new Panel();
            lblCardBorrowedText = new Label();
            lblCardBorrowedCount = new Label();
            cardPending = new Panel();
            lblCardPendingText = new Label();
            lblCardPendingCount = new Label();
            cardVerified = new Panel();
            lblCardVerifiedText = new Label();
            lblCardVerifiedCount = new Label();
            dgvReservations = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            vCardRejected = new Panel();
            lblVRejectedText = new Label();
            lblVRejectedCount = new Label();
            vCardPending = new Panel();
            lblVPendingText = new Label();
            lblVPendingCount = new Label();
            vCardVerified = new Panel();
            lblVVerifiedText = new Label();
            lblVVerifiedCount = new Label();
            sidebarPanel.SuspendLayout();
            topPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            panelHistory.SuspendLayout();
            pnlHistoryMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            panelReservations.SuspendLayout();
            pnlReservationsMain.SuspendLayout();
            pnlReservationStats.SuspendLayout();
            cardResUnclaimed.SuspendLayout();
            cardResClaimed.SuspendLayout();
            cardResPending.SuspendLayout();
            panelBorrowed.SuspendLayout();
            pnlBorrowedMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBorrowed).BeginInit();
            panelEquipment.SuspendLayout();
            pnlEquipmentMain.SuspendLayout();
            pnlEqFilters.SuspendLayout();
            panelVerification.SuspendLayout();
            pnlVerifiedSearch.SuspendLayout();
            pnlStudentReservationsCard.SuspendLayout();
            pnlStudentReturnedCard.SuspendLayout();
            pnlStudentBorrowedCard.SuspendLayout();
            pnlPendingList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).BeginInit();
            panelDashboard.SuspendLayout();
            pnlClaimableToday.SuspendLayout();
            pnlRecentActivity.SuspendLayout();
            pnlStatistics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartStats).BeginInit();
            cardEquipment.SuspendLayout();
            cardOverdue.SuspendLayout();
            cardBorrowed.SuspendLayout();
            cardPending.SuspendLayout();
            cardVerified.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReservations).BeginInit();
            vCardRejected.SuspendLayout();
            vCardPending.SuspendLayout();
            vCardVerified.SuspendLayout();
            SuspendLayout();
            // 
            // sidebarPanel
            // 
            sidebarPanel.BackColor = Color.FromArgb(223, 208, 226);
            sidebarPanel.BackgroundImage = (Image)resources.GetObject("sidebarPanel.BackgroundImage");
            sidebarPanel.Controls.Add(btnLogout);
            sidebarPanel.Controls.Add(btnNavHistory);
            sidebarPanel.Controls.Add(btnNavExperimentManuals);
            sidebarPanel.Controls.Add(btnNavReservations);
            sidebarPanel.Controls.Add(btnNavBorrowed);
            sidebarPanel.Controls.Add(btnNavEquipment);
            sidebarPanel.Controls.Add(btnNavVerification);
            sidebarPanel.Controls.Add(btnNavDashboard);
            sidebarPanel.Controls.Add(lblAdminTitle);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(252, 749);
            sidebarPanel.TabIndex = 0;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.FromArgb(212, 168, 45);
            btnLogout.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(33, 693);
            btnLogout.Name = "btnLogout";
            btnLogout.Padding = new Padding(18, 0, 0, 0);
            btnLogout.Size = new Size(190, 44);
            btnLogout.TabIndex = 8;
            btnLogout.Text = "↩  Logout";
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnNavHistory
            // 
            btnNavHistory.BackColor = Color.FromArgb(212, 168, 45);
            btnNavHistory.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavHistory.ForeColor = Color.White;
            btnNavHistory.Location = new Point(24, 483);
            btnNavHistory.Name = "btnNavHistory";
            btnNavHistory.Padding = new Padding(18, 0, 0, 0);
            btnNavHistory.Size = new Size(211, 49);
            btnNavHistory.TabIndex = 7;
            btnNavHistory.Text = "🕘  History";
            btnNavHistory.TextAlign = ContentAlignment.MiddleLeft;
            btnNavHistory.UseVisualStyleBackColor = false;
            btnNavHistory.Click += btnNavHistory_Click;
            // 
            // btnNavExperimentManuals
            // 
            btnNavExperimentManuals.BackColor = Color.FromArgb(212, 168, 45);
            btnNavExperimentManuals.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavExperimentManuals.ForeColor = Color.White;
            btnNavExperimentManuals.Location = new Point(24, 548);
            btnNavExperimentManuals.Name = "btnNavExperimentManuals";
            btnNavExperimentManuals.Padding = new Padding(18, 0, 0, 0);
            btnNavExperimentManuals.Size = new Size(211, 49);
            btnNavExperimentManuals.TabIndex = 20;
            btnNavExperimentManuals.Text = "\U0001f9ea  Manuals";
            btnNavExperimentManuals.TextAlign = ContentAlignment.MiddleLeft;
            btnNavExperimentManuals.UseVisualStyleBackColor = false;
            btnNavExperimentManuals.Click += btnNavExperimentManuals_Click;
            // 
            // btnNavReservations
            // 
            btnNavReservations.BackColor = Color.FromArgb(212, 168, 45);
            btnNavReservations.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavReservations.ForeColor = Color.White;
            btnNavReservations.Location = new Point(24, 414);
            btnNavReservations.Name = "btnNavReservations";
            btnNavReservations.Padding = new Padding(18, 0, 0, 0);
            btnNavReservations.Size = new Size(211, 49);
            btnNavReservations.TabIndex = 6;
            btnNavReservations.Text = "📝  Reservations";
            btnNavReservations.TextAlign = ContentAlignment.MiddleLeft;
            btnNavReservations.UseVisualStyleBackColor = false;
            btnNavReservations.Click += btnNavReservations_Click;
            // 
            // btnNavBorrowed
            // 
            btnNavBorrowed.BackColor = Color.FromArgb(212, 168, 45);
            btnNavBorrowed.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavBorrowed.ForeColor = Color.White;
            btnNavBorrowed.Location = new Point(24, 345);
            btnNavBorrowed.Name = "btnNavBorrowed";
            btnNavBorrowed.Padding = new Padding(18, 0, 0, 0);
            btnNavBorrowed.Size = new Size(211, 49);
            btnNavBorrowed.TabIndex = 5;
            btnNavBorrowed.Text = "📚  Borrowing";
            btnNavBorrowed.TextAlign = ContentAlignment.MiddleLeft;
            btnNavBorrowed.UseVisualStyleBackColor = false;
            btnNavBorrowed.Click += btnNavBorrowed_Click;
            // 
            // btnNavEquipment
            // 
            btnNavEquipment.BackColor = Color.FromArgb(212, 168, 45);
            btnNavEquipment.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavEquipment.ForeColor = Color.White;
            btnNavEquipment.Location = new Point(24, 280);
            btnNavEquipment.Name = "btnNavEquipment";
            btnNavEquipment.Padding = new Padding(18, 0, 0, 0);
            btnNavEquipment.Size = new Size(211, 49);
            btnNavEquipment.TabIndex = 4;
            btnNavEquipment.Text = "📦  Equipment";
            btnNavEquipment.TextAlign = ContentAlignment.MiddleLeft;
            btnNavEquipment.UseVisualStyleBackColor = false;
            btnNavEquipment.Click += btnNavEquipment_Click;
            // 
            // btnNavVerification
            // 
            btnNavVerification.BackColor = Color.FromArgb(212, 168, 45);
            btnNavVerification.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavVerification.ForeColor = Color.White;
            btnNavVerification.Location = new Point(24, 214);
            btnNavVerification.Name = "btnNavVerification";
            btnNavVerification.Padding = new Padding(18, 0, 0, 0);
            btnNavVerification.Size = new Size(211, 49);
            btnNavVerification.TabIndex = 3;
            btnNavVerification.Text = "👥  Accounts";
            btnNavVerification.TextAlign = ContentAlignment.MiddleLeft;
            btnNavVerification.UseVisualStyleBackColor = false;
            btnNavVerification.Click += btnNavVerification_Click;
            // 
            // btnNavDashboard
            // 
            btnNavDashboard.BackColor = Color.Goldenrod;
            btnNavDashboard.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavDashboard.ForeColor = Color.White;
            btnNavDashboard.Location = new Point(24, 149);
            btnNavDashboard.Name = "btnNavDashboard";
            btnNavDashboard.Padding = new Padding(18, 0, 0, 0);
            btnNavDashboard.Size = new Size(211, 49);
            btnNavDashboard.TabIndex = 2;
            btnNavDashboard.Text = "🏠  Dashboard";
            btnNavDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnNavDashboard.UseVisualStyleBackColor = false;
            btnNavDashboard.Click += btnNavDashboard_Click;
            // 
            // lblAdminTitle
            // 
            lblAdminTitle.AutoSize = true;
            lblAdminTitle.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            lblAdminTitle.ForeColor = Color.FromArgb(92, 45, 58);
            lblAdminTitle.Location = new Point(88, 38);
            lblAdminTitle.Name = "lblAdminTitle";
            lblAdminTitle.Size = new Size(0, 36);
            lblAdminTitle.TabIndex = 0;
            lblAdminTitle.Visible = false;
            // 
            // topPanel
            // 
            topPanel.BackColor = Color.FromArgb(245, 240, 245);
            topPanel.Controls.Add(lblWelcome);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(252, 0);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(1108, 84);
            topPanel.TabIndex = 1;
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(69, 45, 96);
            lblWelcome.Location = new Point(36, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(179, 45);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Dashboard";
            // 
            // contentPanel
            // 
            contentPanel.BackColor = Color.FromArgb(245, 240, 245);
            contentPanel.Controls.Add(panelHistory);
            contentPanel.Controls.Add(panelReservations);
            contentPanel.Controls.Add(panelBorrowed);
            contentPanel.Controls.Add(panelEquipment);
            contentPanel.Controls.Add(panelVerification);
            contentPanel.Controls.Add(panelDashboard);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(252, 84);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1108, 665);
            contentPanel.TabIndex = 2;
            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.FromArgb(245, 240, 245);
            panelHistory.Controls.Add(pnlHistoryMain);
            panelHistory.Dock = DockStyle.Fill;
            panelHistory.Location = new Point(0, 0);
            panelHistory.Name = "panelHistory";
            panelHistory.Size = new Size(1108, 665);
            panelHistory.TabIndex = 5;
            panelHistory.Visible = false;
            // 
            // pnlHistoryMain
            // 
            pnlHistoryMain.BackColor = Color.WhiteSmoke;
            pnlHistoryMain.Controls.Add(dgvHistory);
            pnlHistoryMain.Controls.Add(lblHistoryHeader);
            pnlHistoryMain.Location = new Point(34, 27);
            pnlHistoryMain.Name = "pnlHistoryMain";
            pnlHistoryMain.Size = new Size(1040, 600);
            pnlHistoryMain.TabIndex = 0;
            // 
            // dgvHistory
            // 
            dgvHistory.BackgroundColor = Color.WhiteSmoke;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn19, dataGridViewTextBoxColumn20, dataGridViewTextBoxColumn21, dataGridViewTextBoxColumn22, dataGridViewTextBoxColumn23 });
            dgvHistory.Location = new Point(28, 76);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.Size = new Size(980, 490);
            dgvHistory.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn19
            // 
            dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
            // 
            // dataGridViewTextBoxColumn20
            // 
            dataGridViewTextBoxColumn20.Name = "dataGridViewTextBoxColumn20";
            // 
            // dataGridViewTextBoxColumn21
            // 
            dataGridViewTextBoxColumn21.Name = "dataGridViewTextBoxColumn21";
            // 
            // dataGridViewTextBoxColumn22
            // 
            dataGridViewTextBoxColumn22.Name = "dataGridViewTextBoxColumn22";
            // 
            // dataGridViewTextBoxColumn23
            // 
            dataGridViewTextBoxColumn23.Name = "dataGridViewTextBoxColumn23";
            // 
            // lblHistoryHeader
            // 
            lblHistoryHeader.AutoSize = true;
            lblHistoryHeader.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHistoryHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblHistoryHeader.Location = new Point(28, 25);
            lblHistoryHeader.Name = "lblHistoryHeader";
            lblHistoryHeader.Size = new Size(98, 32);
            lblHistoryHeader.TabIndex = 0;
            lblHistoryHeader.Text = "History";
            // 
            // panelReservations
            // 
            panelReservations.BackColor = Color.FromArgb(245, 240, 245);
            panelReservations.Controls.Add(pnlReservationsMain);
            panelReservations.Dock = DockStyle.Fill;
            panelReservations.Location = new Point(0, 0);
            panelReservations.Name = "panelReservations";
            panelReservations.Size = new Size(1108, 665);
            panelReservations.TabIndex = 4;
            panelReservations.Visible = false;
            // 
            // pnlReservationsMain
            // 
            pnlReservationsMain.BackColor = Color.WhiteSmoke;
            pnlReservationsMain.Controls.Add(pnlReservationStats);
            pnlReservationsMain.Controls.Add(btnUnclaimed);
            pnlReservationsMain.Controls.Add(btnClaim);
            pnlReservationsMain.Controls.Add(lblResShowAll);
            pnlReservationsMain.Controls.Add(lblReservationsHeader);
            pnlReservationsMain.Controls.Add(flowPendingCards);
            pnlReservationsMain.Location = new Point(34, 27);
            pnlReservationsMain.Name = "pnlReservationsMain";
            pnlReservationsMain.Size = new Size(1040, 600);
            pnlReservationsMain.TabIndex = 0;
            // 
            // pnlReservationStats
            // 
            pnlReservationStats.BackColor = Color.Transparent;
            pnlReservationStats.Controls.Add(cardResUnclaimed);
            pnlReservationStats.Controls.Add(cardResClaimed);
            pnlReservationStats.Controls.Add(cardResPending);
            pnlReservationStats.Location = new Point(28, 72);
            pnlReservationStats.Name = "pnlReservationStats";
            pnlReservationStats.Size = new Size(980, 74);
            pnlReservationStats.TabIndex = 4;
            // 
            // cardResUnclaimed
            // 
            cardResUnclaimed.BackColor = Color.FromArgb(255, 225, 225);
            cardResUnclaimed.Controls.Add(lblResUnclaimedTitle);
            cardResUnclaimed.Controls.Add(lblResUnclaimedCount);
            cardResUnclaimed.Cursor = Cursors.Hand;
            cardResUnclaimed.Location = new Point(680, 6);
            cardResUnclaimed.Name = "cardResUnclaimed";
            cardResUnclaimed.Size = new Size(300, 62);
            cardResUnclaimed.TabIndex = 2;
            // 
            // lblResUnclaimedTitle
            // 
            lblResUnclaimedTitle.AutoSize = true;
            lblResUnclaimedTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblResUnclaimedTitle.ForeColor = Color.FromArgb(150, 55, 55);
            lblResUnclaimedTitle.Location = new Point(14, 10);
            lblResUnclaimedTitle.Name = "lblResUnclaimedTitle";
            lblResUnclaimedTitle.Size = new Size(80, 19);
            lblResUnclaimedTitle.TabIndex = 0;
            lblResUnclaimedTitle.Text = "Unclaimed";
            // 
            // lblResUnclaimedCount
            // 
            lblResUnclaimedCount.AutoSize = true;
            lblResUnclaimedCount.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblResUnclaimedCount.ForeColor = Color.FromArgb(135, 45, 45);
            lblResUnclaimedCount.Location = new Point(14, 28);
            lblResUnclaimedCount.Name = "lblResUnclaimedCount";
            lblResUnclaimedCount.Size = new Size(26, 30);
            lblResUnclaimedCount.TabIndex = 1;
            lblResUnclaimedCount.Text = "0";
            // 
            // cardResClaimed
            // 
            cardResClaimed.BackColor = Color.FromArgb(220, 245, 224);
            cardResClaimed.Controls.Add(lblResClaimedTitle);
            cardResClaimed.Controls.Add(lblResClaimedCount);
            cardResClaimed.Cursor = Cursors.Hand;
            cardResClaimed.Location = new Point(340, 6);
            cardResClaimed.Name = "cardResClaimed";
            cardResClaimed.Size = new Size(300, 62);
            cardResClaimed.TabIndex = 1;
            // 
            // lblResClaimedTitle
            // 
            lblResClaimedTitle.AutoSize = true;
            lblResClaimedTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblResClaimedTitle.ForeColor = Color.FromArgb(45, 110, 60);
            lblResClaimedTitle.Location = new Point(14, 10);
            lblResClaimedTitle.Name = "lblResClaimedTitle";
            lblResClaimedTitle.Size = new Size(64, 19);
            lblResClaimedTitle.TabIndex = 0;
            lblResClaimedTitle.Text = "Claimed";
            // 
            // lblResClaimedCount
            // 
            lblResClaimedCount.AutoSize = true;
            lblResClaimedCount.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblResClaimedCount.ForeColor = Color.FromArgb(38, 97, 53);
            lblResClaimedCount.Location = new Point(14, 28);
            lblResClaimedCount.Name = "lblResClaimedCount";
            lblResClaimedCount.Size = new Size(26, 30);
            lblResClaimedCount.TabIndex = 1;
            lblResClaimedCount.Text = "0";
            // 
            // cardResPending
            // 
            cardResPending.BackColor = Color.FromArgb(255, 240, 224);
            cardResPending.Controls.Add(lblResPendingTitle);
            cardResPending.Controls.Add(lblResPendingCount);
            cardResPending.Cursor = Cursors.Hand;
            cardResPending.Location = new Point(0, 6);
            cardResPending.Name = "cardResPending";
            cardResPending.Size = new Size(300, 62);
            cardResPending.TabIndex = 0;
            // 
            // lblResPendingTitle
            // 
            lblResPendingTitle.AutoSize = true;
            lblResPendingTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblResPendingTitle.ForeColor = Color.FromArgb(160, 98, 27);
            lblResPendingTitle.Location = new Point(14, 10);
            lblResPendingTitle.Name = "lblResPendingTitle";
            lblResPendingTitle.Size = new Size(64, 19);
            lblResPendingTitle.TabIndex = 0;
            lblResPendingTitle.Text = "Pending";
            // 
            // lblResPendingCount
            // 
            lblResPendingCount.AutoSize = true;
            lblResPendingCount.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblResPendingCount.ForeColor = Color.FromArgb(140, 80, 20);
            lblResPendingCount.Location = new Point(14, 28);
            lblResPendingCount.Name = "lblResPendingCount";
            lblResPendingCount.Size = new Size(26, 30);
            lblResPendingCount.TabIndex = 1;
            lblResPendingCount.Text = "0";
            // 
            // btnUnclaimed
            // 
            btnUnclaimed.BackColor = Color.FromArgb(229, 123, 123);
            btnUnclaimed.FlatStyle = FlatStyle.Flat;
            btnUnclaimed.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnUnclaimed.ForeColor = Color.White;
            btnUnclaimed.Location = new Point(864, 548);
            btnUnclaimed.Name = "btnUnclaimed";
            btnUnclaimed.Size = new Size(144, 38);
            btnUnclaimed.TabIndex = 2;
            btnUnclaimed.Text = "Unclaimed";
            btnUnclaimed.UseVisualStyleBackColor = false;
            // 
            // btnClaim
            // 
            btnClaim.BackColor = Color.FromArgb(125, 204, 126);
            btnClaim.FlatStyle = FlatStyle.Flat;
            btnClaim.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnClaim.ForeColor = Color.White;
            btnClaim.Location = new Point(709, 548);
            btnClaim.Name = "btnClaim";
            btnClaim.Size = new Size(144, 38);
            btnClaim.TabIndex = 1;
            btnClaim.Text = "Claim";
            btnClaim.UseVisualStyleBackColor = false;
            // 
            // lblResShowAll
            // 
            lblResShowAll.AutoSize = true;
            lblResShowAll.Cursor = Cursors.Hand;
            lblResShowAll.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Underline);
            lblResShowAll.ForeColor = Color.FromArgb(69, 45, 96);
            lblResShowAll.Location = new Point(936, 38);
            lblResShowAll.Name = "lblResShowAll";
            lblResShowAll.Size = new Size(54, 15);
            lblResShowAll.TabIndex = 5;
            lblResShowAll.Text = "Show All";
            // 
            // lblReservationsHeader
            // 
            lblReservationsHeader.AutoSize = true;
            lblReservationsHeader.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblReservationsHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblReservationsHeader.Location = new Point(28, 25);
            lblReservationsHeader.Name = "lblReservationsHeader";
            lblReservationsHeader.Size = new Size(160, 32);
            lblReservationsHeader.TabIndex = 0;
            lblReservationsHeader.Text = "Reservations";
            // 
            // flowPendingCards
            // 
            flowPendingCards.AutoScroll = true;
            flowPendingCards.BackColor = Color.Transparent;
            flowPendingCards.Location = new Point(28, 160);
            flowPendingCards.Name = "flowPendingCards";
            flowPendingCards.Size = new Size(980, 370);
            flowPendingCards.TabIndex = 6;
            // 
            // panelBorrowed
            // 
            panelBorrowed.BackColor = Color.FromArgb(245, 240, 245);
            panelBorrowed.Controls.Add(pnlBorrowedMain);
            panelBorrowed.Dock = DockStyle.Fill;
            panelBorrowed.Location = new Point(0, 0);
            panelBorrowed.Name = "panelBorrowed";
            panelBorrowed.Size = new Size(1108, 665);
            panelBorrowed.TabIndex = 3;
            panelBorrowed.Visible = false;
            // 
            // pnlBorrowedMain
            // 
            pnlBorrowedMain.BackColor = Color.WhiteSmoke;
            pnlBorrowedMain.Controls.Add(dgvBorrowed);
            pnlBorrowedMain.Controls.Add(btnReturn);
            pnlBorrowedMain.Controls.Add(lblBorrowedHeader);
            pnlBorrowedMain.Location = new Point(34, 27);
            pnlBorrowedMain.Name = "pnlBorrowedMain";
            pnlBorrowedMain.Size = new Size(1040, 600);
            pnlBorrowedMain.TabIndex = 0;
            // 
            // dgvBorrowed
            // 
            dgvBorrowed.BackgroundColor = Color.WhiteSmoke;
            dgvBorrowed.BorderStyle = BorderStyle.None;
            dgvBorrowed.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBorrowed.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10, dataGridViewTextBoxColumn11, dataGridViewTextBoxColumn12, dataGridViewTextBoxColumn13, dataGridViewTextBoxColumn14 });
            dgvBorrowed.Location = new Point(28, 110);
            dgvBorrowed.Name = "dgvBorrowed";
            dgvBorrowed.RowHeadersVisible = false;
            dgvBorrowed.Size = new Size(980, 440);
            dgvBorrowed.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            // 
            // dataGridViewTextBoxColumn11
            // 
            dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            // 
            // dataGridViewTextBoxColumn12
            // 
            dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            // 
            // dataGridViewTextBoxColumn13
            // 
            dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            // 
            // dataGridViewTextBoxColumn14
            // 
            dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            // 
            // btnReturn
            // 
            btnReturn.BackColor = Color.FromArgb(125, 204, 126);
            btnReturn.FlatStyle = FlatStyle.Flat;
            btnReturn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnReturn.ForeColor = Color.White;
            btnReturn.Location = new Point(864, 530);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(144, 38);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "Return";
            btnReturn.UseVisualStyleBackColor = false;
            btnReturn.Click += btnReturn_Click;
            // 
            // lblBorrowedHeader
            // 
            lblBorrowedHeader.AutoSize = true;
            lblBorrowedHeader.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblBorrowedHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblBorrowedHeader.Location = new Point(28, 25);
            lblBorrowedHeader.Name = "lblBorrowedHeader";
            lblBorrowedHeader.Size = new Size(0, 32);
            lblBorrowedHeader.TabIndex = 0;
            // 
            // panelEquipment
            // 
            panelEquipment.BackColor = Color.FromArgb(245, 240, 245);
            panelEquipment.Controls.Add(pnlEquipmentMain);
            panelEquipment.Dock = DockStyle.Fill;
            panelEquipment.Location = new Point(0, 0);
            panelEquipment.Name = "panelEquipment";
            panelEquipment.Size = new Size(1108, 665);
            panelEquipment.TabIndex = 2;
            panelEquipment.Visible = false;
            // 
            // pnlEquipmentMain
            // 
            pnlEquipmentMain.BackColor = Color.WhiteSmoke;
            pnlEquipmentMain.Controls.Add(flowEquipmentCards);
            pnlEquipmentMain.Controls.Add(pnlEqFilters);
            pnlEquipmentMain.Controls.Add(lblEquipmentHeader);
            pnlEquipmentMain.Controls.Add(txtEquipmentAdminSearch);
            pnlEquipmentMain.Controls.Add(cmbEquipmentSubjectFilter);
            pnlEquipmentMain.Location = new Point(34, 27);
            pnlEquipmentMain.Name = "pnlEquipmentMain";
            pnlEquipmentMain.Size = new Size(1040, 600);
            pnlEquipmentMain.TabIndex = 0;
            // 
            // flowEquipmentCards
            // 
            flowEquipmentCards.AutoScroll = true;
            flowEquipmentCards.Location = new Point(28, 150);
            flowEquipmentCards.Name = "flowEquipmentCards";
            flowEquipmentCards.Size = new Size(980, 430);
            flowEquipmentCards.TabIndex = 2;
            // 
            // pnlEqFilters
            // 
            pnlEqFilters.Controls.Add(btnEqGeneral);
            pnlEqFilters.Controls.Add(btnEqSports);
            pnlEqFilters.Controls.Add(btnEqScience);
            pnlEqFilters.Controls.Add(btnEqTechnical);
            pnlEqFilters.Controls.Add(btnEqAll);
            pnlEqFilters.Location = new Point(28, 105);
            pnlEqFilters.Name = "pnlEqFilters";
            pnlEqFilters.Size = new Size(980, 40);
            pnlEqFilters.TabIndex = 1;
            // 
            // btnEqGeneral
            // 
            btnEqGeneral.BackColor = Color.FromArgb(228, 218, 236);
            btnEqGeneral.FlatStyle = FlatStyle.Flat;
            btnEqGeneral.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEqGeneral.ForeColor = Color.FromArgb(69, 45, 96);
            btnEqGeneral.Location = new Point(783, 2);
            btnEqGeneral.Name = "btnEqGeneral";
            btnEqGeneral.Size = new Size(180, 34);
            btnEqGeneral.TabIndex = 4;
            btnEqGeneral.Text = "General Equipment";
            btnEqGeneral.UseVisualStyleBackColor = false;
            // 
            // btnEqSports
            // 
            btnEqSports.BackColor = Color.FromArgb(228, 218, 236);
            btnEqSports.FlatStyle = FlatStyle.Flat;
            btnEqSports.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEqSports.ForeColor = Color.FromArgb(69, 45, 96);
            btnEqSports.Location = new Point(592, 2);
            btnEqSports.Name = "btnEqSports";
            btnEqSports.Size = new Size(180, 34);
            btnEqSports.TabIndex = 3;
            btnEqSports.Text = "Sports Equipment";
            btnEqSports.UseVisualStyleBackColor = false;
            // 
            // btnEqScience
            // 
            btnEqScience.BackColor = Color.FromArgb(228, 218, 236);
            btnEqScience.FlatStyle = FlatStyle.Flat;
            btnEqScience.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEqScience.ForeColor = Color.FromArgb(69, 45, 96);
            btnEqScience.Location = new Point(401, 2);
            btnEqScience.Name = "btnEqScience";
            btnEqScience.Size = new Size(180, 34);
            btnEqScience.TabIndex = 2;
            btnEqScience.Text = "Science Laboratory";
            btnEqScience.UseVisualStyleBackColor = false;
            // 
            // btnEqTechnical
            // 
            btnEqTechnical.BackColor = Color.FromArgb(228, 218, 236);
            btnEqTechnical.FlatStyle = FlatStyle.Flat;
            btnEqTechnical.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEqTechnical.ForeColor = Color.FromArgb(69, 45, 96);
            btnEqTechnical.Location = new Point(210, 2);
            btnEqTechnical.Name = "btnEqTechnical";
            btnEqTechnical.Size = new Size(180, 34);
            btnEqTechnical.TabIndex = 1;
            btnEqTechnical.Text = "Technical Laboratory";
            btnEqTechnical.UseVisualStyleBackColor = false;
            // 
            // btnEqAll
            // 
            btnEqAll.BackColor = Color.FromArgb(125, 204, 126);
            btnEqAll.FlatStyle = FlatStyle.Flat;
            btnEqAll.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEqAll.ForeColor = Color.White;
            btnEqAll.Location = new Point(19, 2);
            btnEqAll.Name = "btnEqAll";
            btnEqAll.Size = new Size(180, 34);
            btnEqAll.TabIndex = 0;
            btnEqAll.Text = "All";
            btnEqAll.UseVisualStyleBackColor = false;
            // 
            // lblEquipmentHeader
            // 
            lblEquipmentHeader.AutoSize = true;
            lblEquipmentHeader.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblEquipmentHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblEquipmentHeader.Location = new Point(28, 25);
            lblEquipmentHeader.Name = "lblEquipmentHeader";
            lblEquipmentHeader.Size = new Size(138, 32);
            lblEquipmentHeader.TabIndex = 0;
            lblEquipmentHeader.Text = "Equipment";
            // 
            // txtEquipmentAdminSearch
            // 
            txtEquipmentAdminSearch.Font = new Font("Segoe UI", 9.5F);
            txtEquipmentAdminSearch.Location = new Point(28, 66);
            txtEquipmentAdminSearch.Name = "txtEquipmentAdminSearch";
            txtEquipmentAdminSearch.PlaceholderText = "Search equipment...";
            txtEquipmentAdminSearch.Size = new Size(250, 24);
            txtEquipmentAdminSearch.TabIndex = 9;
            txtEquipmentAdminSearch.TextChanged += txtEquipmentAdminSearch_TextChanged;
            // 
            // cmbEquipmentSubjectFilter
            // 
            cmbEquipmentSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEquipmentSubjectFilter.Font = new Font("Segoe UI", 9.5F);
            cmbEquipmentSubjectFilter.Location = new Point(295, 66);
            cmbEquipmentSubjectFilter.Name = "cmbEquipmentSubjectFilter";
            cmbEquipmentSubjectFilter.Size = new Size(250, 25);
            cmbEquipmentSubjectFilter.TabIndex = 10;
            cmbEquipmentSubjectFilter.SelectedIndexChanged += cmbEquipmentSubjectFilter_SelectedIndexChanged;
            // 
            // panelVerification
            // 
            panelVerification.BackColor = Color.FromArgb(245, 240, 245);
            panelVerification.Controls.Add(pnlVerifiedSearch);
            panelVerification.Controls.Add(pnlPendingList);
            panelVerification.Dock = DockStyle.Fill;
            panelVerification.Location = new Point(0, 0);
            panelVerification.Name = "panelVerification";
            panelVerification.Size = new Size(1108, 665);
            panelVerification.TabIndex = 1;
            panelVerification.Visible = false;
            // 
            // pnlVerifiedSearch
            // 
            pnlVerifiedSearch.BackColor = Color.WhiteSmoke;
            pnlVerifiedSearch.Controls.Add(pnlStudentReservationsCard);
            pnlVerifiedSearch.Controls.Add(pnlStudentReturnedCard);
            pnlVerifiedSearch.Controls.Add(pnlStudentBorrowedCard);
            pnlVerifiedSearch.Controls.Add(lblStudentEmail);
            pnlVerifiedSearch.Controls.Add(lblStudentSchoolID);
            pnlVerifiedSearch.Controls.Add(lblStudentName);
            pnlVerifiedSearch.Controls.Add(txtVerifiedSearch);
            pnlVerifiedSearch.Controls.Add(lblVerifiedSearchHeader);
            pnlVerifiedSearch.Location = new Point(802, 32);
            pnlVerifiedSearch.Name = "pnlVerifiedSearch";
            pnlVerifiedSearch.Size = new Size(272, 550);
            pnlVerifiedSearch.TabIndex = 4;
            // 
            // pnlStudentReservationsCard
            // 
            pnlStudentReservationsCard.BackColor = Color.FromArgb(255, 240, 224);
            pnlStudentReservationsCard.Controls.Add(lblStudentReservationsTitle);
            pnlStudentReservationsCard.Controls.Add(lblStudentReservationsCount);
            pnlStudentReservationsCard.Location = new Point(24, 354);
            pnlStudentReservationsCard.Name = "pnlStudentReservationsCard";
            pnlStudentReservationsCard.Size = new Size(224, 62);
            pnlStudentReservationsCard.TabIndex = 8;
            // 
            // lblStudentReservationsTitle
            // 
            lblStudentReservationsTitle.AutoSize = true;
            lblStudentReservationsTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStudentReservationsTitle.ForeColor = Color.FromArgb(160, 98, 27);
            lblStudentReservationsTitle.Location = new Point(12, 8);
            lblStudentReservationsTitle.Name = "lblStudentReservationsTitle";
            lblStudentReservationsTitle.Size = new Size(53, 15);
            lblStudentReservationsTitle.TabIndex = 0;
            lblStudentReservationsTitle.Text = "Overdue";
            // 
            // lblStudentReservationsCount
            // 
            lblStudentReservationsCount.AutoSize = true;
            lblStudentReservationsCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblStudentReservationsCount.ForeColor = Color.FromArgb(140, 80, 20);
            lblStudentReservationsCount.Location = new Point(12, 26);
            lblStudentReservationsCount.Name = "lblStudentReservationsCount";
            lblStudentReservationsCount.Size = new Size(24, 28);
            lblStudentReservationsCount.TabIndex = 1;
            lblStudentReservationsCount.Text = "0";
            // 
            // pnlStudentReturnedCard
            // 
            pnlStudentReturnedCard.BackColor = Color.FromArgb(235, 246, 236);
            pnlStudentReturnedCard.Controls.Add(lblStudentReturnedTitle);
            pnlStudentReturnedCard.Controls.Add(lblStudentReturnedCount);
            pnlStudentReturnedCard.Location = new Point(24, 280);
            pnlStudentReturnedCard.Name = "pnlStudentReturnedCard";
            pnlStudentReturnedCard.Size = new Size(224, 62);
            pnlStudentReturnedCard.TabIndex = 7;
            // 
            // lblStudentReturnedTitle
            // 
            lblStudentReturnedTitle.AutoSize = true;
            lblStudentReturnedTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStudentReturnedTitle.ForeColor = Color.FromArgb(62, 98, 68);
            lblStudentReturnedTitle.Location = new Point(12, 8);
            lblStudentReturnedTitle.Name = "lblStudentReturnedTitle";
            lblStudentReturnedTitle.Size = new Size(59, 15);
            lblStudentReturnedTitle.TabIndex = 0;
            lblStudentReturnedTitle.Text = "Returned";
            // 
            // lblStudentReturnedCount
            // 
            lblStudentReturnedCount.AutoSize = true;
            lblStudentReturnedCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblStudentReturnedCount.ForeColor = Color.FromArgb(50, 90, 58);
            lblStudentReturnedCount.Location = new Point(12, 26);
            lblStudentReturnedCount.Name = "lblStudentReturnedCount";
            lblStudentReturnedCount.Size = new Size(24, 28);
            lblStudentReturnedCount.TabIndex = 1;
            lblStudentReturnedCount.Text = "0";
            // 
            // pnlStudentBorrowedCard
            // 
            pnlStudentBorrowedCard.BackColor = Color.FromArgb(243, 236, 247);
            pnlStudentBorrowedCard.Controls.Add(lblStudentBorrowedTitle);
            pnlStudentBorrowedCard.Controls.Add(lblStudentBorrowedCount);
            pnlStudentBorrowedCard.Location = new Point(24, 206);
            pnlStudentBorrowedCard.Name = "pnlStudentBorrowedCard";
            pnlStudentBorrowedCard.Size = new Size(224, 62);
            pnlStudentBorrowedCard.TabIndex = 6;
            // 
            // lblStudentBorrowedTitle
            // 
            lblStudentBorrowedTitle.AutoSize = true;
            lblStudentBorrowedTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStudentBorrowedTitle.ForeColor = Color.FromArgb(87, 60, 99);
            lblStudentBorrowedTitle.Location = new Point(12, 8);
            lblStudentBorrowedTitle.Name = "lblStudentBorrowedTitle";
            lblStudentBorrowedTitle.Size = new Size(67, 15);
            lblStudentBorrowedTitle.TabIndex = 0;
            lblStudentBorrowedTitle.Text = "Borrowing";
            // 
            // lblStudentBorrowedCount
            // 
            lblStudentBorrowedCount.AutoSize = true;
            lblStudentBorrowedCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblStudentBorrowedCount.ForeColor = Color.FromArgb(72, 53, 84);
            lblStudentBorrowedCount.Location = new Point(12, 26);
            lblStudentBorrowedCount.Name = "lblStudentBorrowedCount";
            lblStudentBorrowedCount.Size = new Size(24, 28);
            lblStudentBorrowedCount.TabIndex = 1;
            lblStudentBorrowedCount.Text = "0";
            // 
            // lblStudentEmail
            // 
            lblStudentEmail.Font = new Font("Segoe UI", 10F);
            lblStudentEmail.ForeColor = Color.FromArgb(105, 85, 118);
            lblStudentEmail.Location = new Point(24, 146);
            lblStudentEmail.Name = "lblStudentEmail";
            lblStudentEmail.Size = new Size(224, 42);
            lblStudentEmail.TabIndex = 5;
            lblStudentEmail.Text = "Email: ---";
            // 
            // lblStudentSchoolID
            // 
            lblStudentSchoolID.Font = new Font("Segoe UI", 10F);
            lblStudentSchoolID.ForeColor = Color.FromArgb(105, 85, 118);
            lblStudentSchoolID.Location = new Point(24, 118);
            lblStudentSchoolID.Name = "lblStudentSchoolID";
            lblStudentSchoolID.Size = new Size(224, 24);
            lblStudentSchoolID.TabIndex = 4;
            lblStudentSchoolID.Text = "School ID: ---";
            // 
            // lblStudentName
            // 
            lblStudentName.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblStudentName.ForeColor = Color.FromArgb(72, 53, 84);
            lblStudentName.Location = new Point(24, 66);
            lblStudentName.Name = "lblStudentName";
            lblStudentName.Size = new Size(224, 48);
            lblStudentName.TabIndex = 3;
            lblStudentName.Text = "No account selected";
            // 
            // txtVerifiedSearch
            // 
            txtVerifiedSearch.Font = new Font("Segoe UI", 10F);
            txtVerifiedSearch.Location = new Point(24, 66);
            txtVerifiedSearch.Name = "txtVerifiedSearch";
            txtVerifiedSearch.Size = new Size(224, 25);
            txtVerifiedSearch.TabIndex = 1;
            txtVerifiedSearch.Visible = false;
            // 
            // lblVerifiedSearchHeader
            // 
            lblVerifiedSearchHeader.AutoSize = true;
            lblVerifiedSearchHeader.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblVerifiedSearchHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblVerifiedSearchHeader.Location = new Point(24, 24);
            lblVerifiedSearchHeader.Name = "lblVerifiedSearchHeader";
            lblVerifiedSearchHeader.Size = new Size(158, 28);
            lblVerifiedSearchHeader.TabIndex = 0;
            lblVerifiedSearchHeader.Text = "Student Details";
            // 
            // pnlPendingList
            // 
            pnlPendingList.BackColor = Color.WhiteSmoke;
            pnlPendingList.Controls.Add(dgvPendingUsers);
            pnlPendingList.Controls.Add(lblPendingHeader);
            pnlPendingList.Controls.Add(txtStudentSearch);
            pnlPendingList.Controls.Add(cmbSubjectFilter);
            pnlPendingList.Controls.Add(cmbSectionFilter);
            pnlPendingList.Location = new Point(34, 32);
            pnlPendingList.Name = "pnlPendingList";
            pnlPendingList.Size = new Size(744, 550);
            pnlPendingList.TabIndex = 3;
            // 
            // dgvPendingUsers
            // 
            dgvPendingUsers.BackgroundColor = Color.WhiteSmoke;
            dgvPendingUsers.BorderStyle = BorderStyle.None;
            dgvPendingUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPendingUsers.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn15, dataGridViewTextBoxColumn16, dataGridViewTextBoxColumn17, dataGridViewTextBoxColumn18 });
            dgvPendingUsers.Location = new Point(28, 120);
            dgvPendingUsers.Name = "dgvPendingUsers";
            dgvPendingUsers.RowHeadersVisible = false;
            dgvPendingUsers.Size = new Size(660, 410);
            dgvPendingUsers.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn15
            // 
            dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            // 
            // dataGridViewTextBoxColumn16
            // 
            dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            // 
            // dataGridViewTextBoxColumn17
            // 
            dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
            // 
            // dataGridViewTextBoxColumn18
            // 
            dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
            // 
            // lblPendingHeader
            // 
            lblPendingHeader.AutoSize = true;
            lblPendingHeader.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblPendingHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblPendingHeader.Location = new Point(24, 24);
            lblPendingHeader.Name = "lblPendingHeader";
            lblPendingHeader.Size = new Size(256, 32);
            lblPendingHeader.TabIndex = 0;
            lblPendingHeader.Text = "STUDENT ACCOUNTS";
            // 
            // txtStudentSearch
            // 
            txtStudentSearch.Font = new Font("Segoe UI", 9.5F);
            txtStudentSearch.Location = new Point(28, 75);
            txtStudentSearch.Name = "txtStudentSearch";
            txtStudentSearch.PlaceholderText = "Search name, ID, or email...";
            txtStudentSearch.Size = new Size(250, 24);
            txtStudentSearch.TabIndex = 2;
            txtStudentSearch.TextChanged += txtStudentSearch_TextChanged;
            // 
            // cmbSubjectFilter
            // 
            cmbSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSubjectFilter.Font = new Font("Segoe UI", 9.5F);
            cmbSubjectFilter.Location = new Point(295, 75);
            cmbSubjectFilter.Name = "cmbSubjectFilter";
            cmbSubjectFilter.Size = new Size(180, 25);
            cmbSubjectFilter.TabIndex = 3;
            cmbSubjectFilter.SelectedIndexChanged += cmbSubjectFilter_SelectedIndexChanged;
            // 
            // cmbSectionFilter
            // 
            cmbSectionFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSectionFilter.Font = new Font("Segoe UI", 9.5F);
            cmbSectionFilter.Location = new Point(490, 75);
            cmbSectionFilter.Name = "cmbSectionFilter";
            cmbSectionFilter.Size = new Size(160, 25);
            cmbSectionFilter.TabIndex = 4;
            cmbSectionFilter.SelectedIndexChanged += cmbSectionFilter_SelectedIndexChanged;
            // 
            // panelDashboard
            // 
            panelDashboard.BackColor = Color.FromArgb(245, 240, 245);
            panelDashboard.Controls.Add(pnlClaimableToday);
            panelDashboard.Controls.Add(pnlRecentActivity);
            panelDashboard.Controls.Add(pnlStatistics);
            panelDashboard.Controls.Add(cardEquipment);
            panelDashboard.Controls.Add(cardOverdue);
            panelDashboard.Controls.Add(cardBorrowed);
            panelDashboard.Controls.Add(cardPending);
            panelDashboard.Controls.Add(cardVerified);
            panelDashboard.Dock = DockStyle.Fill;
            panelDashboard.Location = new Point(0, 0);
            panelDashboard.Name = "panelDashboard";
            panelDashboard.Size = new Size(1108, 665);
            panelDashboard.TabIndex = 0;
            // 
            // pnlClaimableToday
            // 
            pnlClaimableToday.BackColor = Color.FromArgb(250, 246, 248);
            pnlClaimableToday.Controls.Add(lblClaimableSub);
            pnlClaimableToday.Controls.Add(lblClaimableValue);
            pnlClaimableToday.Controls.Add(lblClaimableHeader);
            pnlClaimableToday.Location = new Point(520, 233);
            pnlClaimableToday.Name = "pnlClaimableToday";
            pnlClaimableToday.Size = new Size(554, 120);
            pnlClaimableToday.TabIndex = 8;
            // 
            // lblClaimableSub
            // 
            lblClaimableSub.AutoSize = true;
            lblClaimableSub.Font = new Font("Segoe UI", 9F);
            lblClaimableSub.ForeColor = Color.Gray;
            lblClaimableSub.Location = new Point(180, 66);
            lblClaimableSub.Name = "lblClaimableSub";
            lblClaimableSub.Size = new Size(184, 15);
            lblClaimableSub.TabIndex = 0;
            lblClaimableSub.Text = "Reservations ready to claim today";
            // 
            // lblClaimableValue
            // 
            lblClaimableValue.AutoSize = true;
            lblClaimableValue.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            lblClaimableValue.ForeColor = Color.FromArgb(45, 110, 60);
            lblClaimableValue.Location = new Point(24, 48);
            lblClaimableValue.Name = "lblClaimableValue";
            lblClaimableValue.Size = new Size(44, 51);
            lblClaimableValue.TabIndex = 1;
            lblClaimableValue.Text = "0";
            // 
            // lblClaimableHeader
            // 
            lblClaimableHeader.AutoSize = true;
            lblClaimableHeader.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblClaimableHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblClaimableHeader.Location = new Point(24, 18);
            lblClaimableHeader.Name = "lblClaimableHeader";
            lblClaimableHeader.Size = new Size(151, 25);
            lblClaimableHeader.TabIndex = 2;
            lblClaimableHeader.Text = "Claimable Today";
            // 
            // pnlRecentActivity
            // 
            pnlRecentActivity.BackColor = Color.FromArgb(250, 246, 248);
            pnlRecentActivity.Controls.Add(flowRecentActivity);
            pnlRecentActivity.Controls.Add(lblRecentActivityEmpty);
            pnlRecentActivity.Controls.Add(lblRecentActivityHeader);
            pnlRecentActivity.Location = new Point(34, 233);
            pnlRecentActivity.Name = "pnlRecentActivity";
            pnlRecentActivity.Size = new Size(460, 360);
            pnlRecentActivity.TabIndex = 7;
            // 
            // flowRecentActivity
            // 
            flowRecentActivity.AutoScroll = true;
            flowRecentActivity.BackColor = Color.Transparent;
            flowRecentActivity.FlowDirection = FlowDirection.TopDown;
            flowRecentActivity.Location = new Point(24, 58);
            flowRecentActivity.Name = "flowRecentActivity";
            flowRecentActivity.Size = new Size(412, 280);
            flowRecentActivity.TabIndex = 2;
            flowRecentActivity.WrapContents = false;
            // 
            // lblRecentActivityEmpty
            // 
            lblRecentActivityEmpty.AutoSize = true;
            lblRecentActivityEmpty.Font = new Font("Segoe UI", 10F);
            lblRecentActivityEmpty.ForeColor = Color.FromArgb(132, 108, 153);
            lblRecentActivityEmpty.Location = new Point(24, 58);
            lblRecentActivityEmpty.Name = "lblRecentActivityEmpty";
            lblRecentActivityEmpty.Size = new Size(181, 19);
            lblRecentActivityEmpty.TabIndex = 1;
            lblRecentActivityEmpty.Text = "No dashboard notifications.";
            lblRecentActivityEmpty.Visible = false;
            // 
            // lblRecentActivityHeader
            // 
            lblRecentActivityHeader.AutoSize = true;
            lblRecentActivityHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRecentActivityHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblRecentActivityHeader.Location = new Point(24, 18);
            lblRecentActivityHeader.Name = "lblRecentActivityHeader";
            lblRecentActivityHeader.Size = new Size(126, 25);
            lblRecentActivityHeader.TabIndex = 0;
            lblRecentActivityHeader.Text = "Notifications";
            // 
            // pnlStatistics
            // 
            pnlStatistics.BackColor = Color.FromArgb(250, 246, 248);
            pnlStatistics.Controls.Add(chartStats);
            pnlStatistics.Controls.Add(lblStatisticsSub);
            pnlStatistics.Controls.Add(lblStatisticsHeader);
            pnlStatistics.Location = new Point(520, 373);
            pnlStatistics.Name = "pnlStatistics";
            pnlStatistics.Size = new Size(554, 220);
            pnlStatistics.TabIndex = 6;
            // 
            // chartStats
            // 
            chartStats.BackColor = Color.FromArgb(241, 233, 245);
            chartStats.Location = new Point(18, 78);
            chartStats.Name = "chartStats";
            chartStats.Size = new Size(518, 132);
            chartStats.TabIndex = 2;
            chartStats.Text = "chartStats";
            // 
            // lblStatisticsSub
            // 
            lblStatisticsSub.AutoSize = true;
            lblStatisticsSub.Font = new Font("Segoe UI", 10F);
            lblStatisticsSub.ForeColor = Color.FromArgb(132, 108, 153);
            lblStatisticsSub.Location = new Point(24, 50);
            lblStatisticsSub.Name = "lblStatisticsSub";
            lblStatisticsSub.Size = new Size(202, 19);
            lblStatisticsSub.TabIndex = 1;
            lblStatisticsSub.Text = "By returned equipment quantity";
            // 
            // lblStatisticsHeader
            // 
            lblStatisticsHeader.AutoSize = true;
            lblStatisticsHeader.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblStatisticsHeader.ForeColor = Color.FromArgb(69, 45, 96);
            lblStatisticsHeader.Location = new Point(24, 18);
            lblStatisticsHeader.Name = "lblStatisticsHeader";
            lblStatisticsHeader.Size = new Size(244, 30);
            lblStatisticsHeader.TabIndex = 0;
            lblStatisticsHeader.Text = "Top 5 Most Borrowed";
            // 
            // cardEquipment
            // 
            cardEquipment.BackColor = Color.FromArgb(142, 124, 176);
            cardEquipment.Controls.Add(lblCardEquipmentText);
            cardEquipment.Controls.Add(lblCardEquipmentCount);
            cardEquipment.Location = new Point(794, 42);
            cardEquipment.Name = "cardEquipment";
            cardEquipment.Size = new Size(170, 146);
            cardEquipment.TabIndex = 4;
            // 
            // lblCardEquipmentText
            // 
            lblCardEquipmentText.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblCardEquipmentText.ForeColor = Color.White;
            lblCardEquipmentText.Location = new Point(22, 88);
            lblCardEquipmentText.Name = "lblCardEquipmentText";
            lblCardEquipmentText.Size = new Size(126, 42);
            lblCardEquipmentText.TabIndex = 1;
            lblCardEquipmentText.Text = "Low Stock Items";
            // 
            // lblCardEquipmentCount
            // 
            lblCardEquipmentCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblCardEquipmentCount.ForeColor = Color.White;
            lblCardEquipmentCount.Location = new Point(22, 26);
            lblCardEquipmentCount.Name = "lblCardEquipmentCount";
            lblCardEquipmentCount.Size = new Size(126, 50);
            lblCardEquipmentCount.TabIndex = 0;
            lblCardEquipmentCount.Text = "00";
            lblCardEquipmentCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardOverdue
            // 
            cardOverdue.BackColor = Color.FromArgb(232, 158, 163);
            cardOverdue.Controls.Add(lblCardOverdueText);
            cardOverdue.Controls.Add(lblCardOverdueCount);
            cardOverdue.Location = new Point(604, 42);
            cardOverdue.Name = "cardOverdue";
            cardOverdue.Size = new Size(170, 146);
            cardOverdue.TabIndex = 3;
            // 
            // lblCardOverdueText
            // 
            lblCardOverdueText.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblCardOverdueText.ForeColor = Color.White;
            lblCardOverdueText.Location = new Point(22, 88);
            lblCardOverdueText.Name = "lblCardOverdueText";
            lblCardOverdueText.Size = new Size(126, 42);
            lblCardOverdueText.TabIndex = 1;
            lblCardOverdueText.Text = "Borrowing Items";
            // 
            // lblCardOverdueCount
            // 
            lblCardOverdueCount.AutoSize = true;
            lblCardOverdueCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblCardOverdueCount.ForeColor = Color.White;
            lblCardOverdueCount.Location = new Point(22, 32);
            lblCardOverdueCount.Name = "lblCardOverdueCount";
            lblCardOverdueCount.Size = new Size(56, 45);
            lblCardOverdueCount.TabIndex = 0;
            lblCardOverdueCount.Text = "00";
            // 
            // cardBorrowed
            // 
            cardBorrowed.BackColor = Color.FromArgb(250, 246, 248);
            cardBorrowed.Controls.Add(lblCardBorrowedText);
            cardBorrowed.Controls.Add(lblCardBorrowedCount);
            cardBorrowed.Location = new Point(414, 42);
            cardBorrowed.Name = "cardBorrowed";
            cardBorrowed.Size = new Size(170, 146);
            cardBorrowed.TabIndex = 2;
            // 
            // lblCardBorrowedText
            // 
            lblCardBorrowedText.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblCardBorrowedText.ForeColor = Color.FromArgb(69, 45, 96);
            lblCardBorrowedText.Location = new Point(22, 88);
            lblCardBorrowedText.Name = "lblCardBorrowedText";
            lblCardBorrowedText.Size = new Size(126, 42);
            lblCardBorrowedText.TabIndex = 1;
            lblCardBorrowedText.Text = "Total Students";
            // 
            // lblCardBorrowedCount
            // 
            lblCardBorrowedCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblCardBorrowedCount.ForeColor = Color.FromArgb(69, 45, 96);
            lblCardBorrowedCount.Location = new Point(22, 26);
            lblCardBorrowedCount.Name = "lblCardBorrowedCount";
            lblCardBorrowedCount.Size = new Size(126, 50);
            lblCardBorrowedCount.TabIndex = 0;
            lblCardBorrowedCount.Text = "00";
            lblCardBorrowedCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardPending
            // 
            cardPending.BackColor = Color.FromArgb(250, 246, 248);
            cardPending.Controls.Add(lblCardPendingText);
            cardPending.Controls.Add(lblCardPendingCount);
            cardPending.Location = new Point(224, 42);
            cardPending.Name = "cardPending";
            cardPending.Size = new Size(170, 146);
            cardPending.TabIndex = 1;
            // 
            // lblCardPendingText
            // 
            lblCardPendingText.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblCardPendingText.ForeColor = Color.FromArgb(69, 45, 96);
            lblCardPendingText.Location = new Point(22, 88);
            lblCardPendingText.Name = "lblCardPendingText";
            lblCardPendingText.Size = new Size(126, 42);
            lblCardPendingText.TabIndex = 1;
            lblCardPendingText.Text = "Total Subjects";
            // 
            // lblCardPendingCount
            // 
            lblCardPendingCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblCardPendingCount.ForeColor = Color.FromArgb(69, 45, 96);
            lblCardPendingCount.Location = new Point(22, 26);
            lblCardPendingCount.Name = "lblCardPendingCount";
            lblCardPendingCount.Size = new Size(126, 50);
            lblCardPendingCount.TabIndex = 0;
            lblCardPendingCount.Text = "02";
            lblCardPendingCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cardVerified
            // 
            cardVerified.BackColor = Color.FromArgb(154, 162, 228);
            cardVerified.Controls.Add(lblCardVerifiedText);
            cardVerified.Controls.Add(lblCardVerifiedCount);
            cardVerified.Location = new Point(34, 42);
            cardVerified.Name = "cardVerified";
            cardVerified.Size = new Size(170, 146);
            cardVerified.TabIndex = 0;
            // 
            // lblCardVerifiedText
            // 
            lblCardVerifiedText.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblCardVerifiedText.ForeColor = Color.White;
            lblCardVerifiedText.Location = new Point(22, 88);
            lblCardVerifiedText.Name = "lblCardVerifiedText";
            lblCardVerifiedText.Size = new Size(126, 42);
            lblCardVerifiedText.TabIndex = 1;
            lblCardVerifiedText.Text = "Classes Today";
            // 
            // lblCardVerifiedCount
            // 
            lblCardVerifiedCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblCardVerifiedCount.ForeColor = Color.White;
            lblCardVerifiedCount.Location = new Point(22, 26);
            lblCardVerifiedCount.Name = "lblCardVerifiedCount";
            lblCardVerifiedCount.Size = new Size(126, 50);
            lblCardVerifiedCount.TabIndex = 0;
            lblCardVerifiedCount.Text = "01";
            lblCardVerifiedCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dgvReservations
            // 
            dgvReservations.BackgroundColor = Color.WhiteSmoke;
            dgvReservations.BorderStyle = BorderStyle.None;
            dgvReservations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReservations.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7 });
            dgvReservations.Location = new Point(28, 160);
            dgvReservations.Name = "dgvReservations";
            dgvReservations.RowHeadersVisible = false;
            dgvReservations.Size = new Size(980, 370);
            dgvReservations.TabIndex = 3;
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
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            // 
            // vCardRejected
            // 
            vCardRejected.BackColor = Color.FromArgb(234, 128, 128);
            vCardRejected.Controls.Add(lblVRejectedText);
            vCardRejected.Controls.Add(lblVRejectedCount);
            vCardRejected.Location = new Point(372, 42);
            vCardRejected.Name = "vCardRejected";
            vCardRejected.Size = new Size(140, 96);
            vCardRejected.TabIndex = 2;
            // 
            // lblVRejectedText
            // 
            lblVRejectedText.AutoSize = true;
            lblVRejectedText.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblVRejectedText.ForeColor = Color.White;
            lblVRejectedText.Location = new Point(22, 54);
            lblVRejectedText.Name = "lblVRejectedText";
            lblVRejectedText.Size = new Size(86, 25);
            lblVRejectedText.TabIndex = 1;
            lblVRejectedText.Text = "Rejected";
            // 
            // lblVRejectedCount
            // 
            lblVRejectedCount.AutoSize = true;
            lblVRejectedCount.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblVRejectedCount.ForeColor = Color.White;
            lblVRejectedCount.Location = new Point(53, 20);
            lblVRejectedCount.Name = "lblVRejectedCount";
            lblVRejectedCount.Size = new Size(27, 31);
            lblVRejectedCount.TabIndex = 0;
            lblVRejectedCount.Text = "0";
            // 
            // vCardPending
            // 
            vCardPending.BackColor = Color.FromArgb(250, 246, 248);
            vCardPending.Controls.Add(lblVPendingText);
            vCardPending.Controls.Add(lblVPendingCount);
            vCardPending.Location = new Point(204, 42);
            vCardPending.Name = "vCardPending";
            vCardPending.Size = new Size(140, 96);
            vCardPending.TabIndex = 1;
            // 
            // lblVPendingText
            // 
            lblVPendingText.AutoSize = true;
            lblVPendingText.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblVPendingText.ForeColor = Color.FromArgb(69, 45, 96);
            lblVPendingText.Location = new Point(28, 54);
            lblVPendingText.Name = "lblVPendingText";
            lblVPendingText.Size = new Size(81, 25);
            lblVPendingText.TabIndex = 1;
            lblVPendingText.Text = "Pending";
            // 
            // lblVPendingCount
            // 
            lblVPendingCount.AutoSize = true;
            lblVPendingCount.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblVPendingCount.ForeColor = Color.FromArgb(69, 45, 96);
            lblVPendingCount.Location = new Point(53, 20);
            lblVPendingCount.Name = "lblVPendingCount";
            lblVPendingCount.Size = new Size(27, 31);
            lblVPendingCount.TabIndex = 0;
            lblVPendingCount.Text = "2";
            // 
            // vCardVerified
            // 
            vCardVerified.BackColor = Color.FromArgb(125, 204, 126);
            vCardVerified.Controls.Add(lblVVerifiedText);
            vCardVerified.Controls.Add(lblVVerifiedCount);
            vCardVerified.Location = new Point(34, 42);
            vCardVerified.Name = "vCardVerified";
            vCardVerified.Size = new Size(140, 96);
            vCardVerified.TabIndex = 0;
            // 
            // lblVVerifiedText
            // 
            lblVVerifiedText.AutoSize = true;
            lblVVerifiedText.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblVVerifiedText.ForeColor = Color.White;
            lblVVerifiedText.Location = new Point(28, 54);
            lblVVerifiedText.Name = "lblVVerifiedText";
            lblVVerifiedText.Size = new Size(78, 25);
            lblVVerifiedText.TabIndex = 1;
            lblVVerifiedText.Text = "Verified";
            // 
            // lblVVerifiedCount
            // 
            lblVVerifiedCount.AutoSize = true;
            lblVVerifiedCount.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblVVerifiedCount.ForeColor = Color.White;
            lblVVerifiedCount.Location = new Point(53, 20);
            lblVVerifiedCount.Name = "lblVVerifiedCount";
            lblVVerifiedCount.Size = new Size(27, 31);
            lblVVerifiedCount.TabIndex = 0;
            lblVVerifiedCount.Text = "1";
            // 
            // frmAdminDashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 240, 245);
            ClientSize = new Size(1360, 749);
            Controls.Add(contentPanel);
            Controls.Add(topPanel);
            Controls.Add(sidebarPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmAdminDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WildcatHub - Admin Dashboard";
            Load += frmAdminDashboard_Load;
            Resize += frmAdminDashboard_Resize;
            sidebarPanel.ResumeLayout(false);
            sidebarPanel.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            contentPanel.ResumeLayout(false);
            panelHistory.ResumeLayout(false);
            pnlHistoryMain.ResumeLayout(false);
            pnlHistoryMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            panelReservations.ResumeLayout(false);
            pnlReservationsMain.ResumeLayout(false);
            pnlReservationsMain.PerformLayout();
            pnlReservationStats.ResumeLayout(false);
            cardResUnclaimed.ResumeLayout(false);
            cardResUnclaimed.PerformLayout();
            cardResClaimed.ResumeLayout(false);
            cardResClaimed.PerformLayout();
            cardResPending.ResumeLayout(false);
            cardResPending.PerformLayout();
            panelBorrowed.ResumeLayout(false);
            pnlBorrowedMain.ResumeLayout(false);
            pnlBorrowedMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBorrowed).EndInit();
            panelEquipment.ResumeLayout(false);
            pnlEquipmentMain.ResumeLayout(false);
            pnlEquipmentMain.PerformLayout();
            pnlEqFilters.ResumeLayout(false);
            panelVerification.ResumeLayout(false);
            pnlVerifiedSearch.ResumeLayout(false);
            pnlVerifiedSearch.PerformLayout();
            pnlStudentReservationsCard.ResumeLayout(false);
            pnlStudentReservationsCard.PerformLayout();
            pnlStudentReturnedCard.ResumeLayout(false);
            pnlStudentReturnedCard.PerformLayout();
            pnlStudentBorrowedCard.ResumeLayout(false);
            pnlStudentBorrowedCard.PerformLayout();
            pnlPendingList.ResumeLayout(false);
            pnlPendingList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).EndInit();
            panelDashboard.ResumeLayout(false);
            pnlClaimableToday.ResumeLayout(false);
            pnlClaimableToday.PerformLayout();
            pnlRecentActivity.ResumeLayout(false);
            pnlRecentActivity.PerformLayout();
            pnlStatistics.ResumeLayout(false);
            pnlStatistics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chartStats).EndInit();
            cardEquipment.ResumeLayout(false);
            cardOverdue.ResumeLayout(false);
            cardOverdue.PerformLayout();
            cardBorrowed.ResumeLayout(false);
            cardPending.ResumeLayout(false);
            cardVerified.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReservations).EndInit();
            vCardRejected.ResumeLayout(false);
            vCardRejected.PerformLayout();
            vCardPending.ResumeLayout(false);
            vCardPending.PerformLayout();
            vCardVerified.ResumeLayout(false);
            vCardVerified.PerformLayout();
            ResumeLayout(false);
        }
        private Label lblResShowAll;
        private Panel sidebarPanel;
        private Button btnLogout;
        private Button btnNavHistory;
        private Button btnNavReservations;
        private Button btnNavBorrowed;
        private Button btnNavEquipment;
        private Button btnNavVerification;
        private Button btnNavDashboard;
        private Label lblAdminTitle;
        private Panel topPanel;
        private Label lblWelcome;
        private Panel contentPanel;
        private Panel panelDashboard;
        private Panel cardVerified;
        private Label lblCardVerifiedText;
        private Label lblCardVerifiedCount;
        private Panel cardPending;
        private Label lblCardPendingText;
        private Label lblCardPendingCount;
        private Panel cardBorrowed;
        private Label lblCardBorrowedText;
        private Label lblCardBorrowedCount;
        private Panel cardOverdue;
        private Label lblCardOverdueText;
        private Label lblCardOverdueCount;
        private Panel cardEquipment;
        private Label lblCardEquipmentText;
        private Label lblCardEquipmentCount;
        private Panel pnlStatistics;
        private Label lblStatisticsSub;
        private Label lblStatisticsHeader;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartStats;
        private Panel panelVerification;
        private Panel vCardVerified;
        private Label lblVVerifiedText;
        private Label lblVVerifiedCount;
        private Panel vCardPending;
        private Label lblVPendingText;
        private Label lblVPendingCount;
        private Panel vCardRejected;
        private Label lblVRejectedText;
        private Label lblVRejectedCount;
        private Panel pnlPendingList;
        private DataGridView dgvPendingUsers;
        private Label lblPendingHeader;
        private Panel pnlVerifiedSearch;
        private Label lblVerifiedSearchHeader;
        private TextBox txtVerifiedSearch;
        private Panel panelEquipment;
        private Panel pnlEquipmentMain;
        private FlowLayoutPanel flowEquipmentCards;
        private Panel pnlEqFilters;
        private Button btnEqGeneral;
        private Button btnEqSports;
        private Button btnEqScience;
        private Button btnEqTechnical;
        private Button btnEqAll;
        private Label lblEquipmentHeader;
        private Panel panelBorrowed;
        private Panel pnlBorrowedMain;
        private DataGridView dgvBorrowed;
        private Label lblBorrowedHeader;
        private Panel panelReservations;
        private Panel pnlReservationsMain;
        private DataGridView dgvReservations;
        private Button btnUnclaimed;
        private Button btnClaim;
        private Label lblReservationsHeader;
        private Panel panelHistory;
        private Panel pnlHistoryMain;
        private DataGridView dgvHistory;
        private Label lblHistoryHeader;
        private Button btnReturn;
        private Panel pnlReservationStats;
        private Panel cardResPending;
        private Label lblResPendingTitle;
        private Label lblResPendingCount;
        private Panel cardResClaimed;
        private Label lblResClaimedTitle;
        private Label lblResClaimedCount;
        private Panel cardResUnclaimed;
        private Label lblResUnclaimedTitle;
        private Label lblResUnclaimedCount;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn20;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn21;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn22;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn23;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private Label lblStudentName;
        private Label lblStudentSchoolID;
        private Label lblStudentEmail;
        private Panel pnlStudentBorrowedCard;
        private Panel pnlStudentReturnedCard;
        private Panel pnlStudentReservationsCard;
        private Label lblStudentBorrowedTitle;
        private Label lblStudentReturnedTitle;
        private Label lblStudentReservationsTitle;
        private Label lblStudentBorrowedCount;
        private Label lblStudentReturnedCount;
        private Label lblStudentReservationsCount;
        private Panel pnlRecentActivity;
        private Label lblRecentActivityHeader;
        private Label lblRecentActivityEmpty;
        private Panel pnlClaimableToday;
        private Label lblClaimableHeader;
        private Label lblClaimableValue;
        private Label lblClaimableSub;
        private FlowLayoutPanel flowRecentActivity;
        private TextBox txtEquipmentAdminSearch;
        private TextBox txtStudentSearch;
        private ComboBox cmbSubjectFilter;
        private ComboBox cmbSectionFilter;
        private ComboBox cmbEquipmentSubjectFilter;
        private FlowLayoutPanel flowPendingCards;
        private Button btnNavExperimentManuals;
    }
}
