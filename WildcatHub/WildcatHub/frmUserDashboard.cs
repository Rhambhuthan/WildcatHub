using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;

namespace WildcatHub
{
    public partial class frmUserDashboard : Form
    {
        private string currentEquipmentCategory = "All";
        private int currentUserId => SessionManager.UserID;
        private int selectedBorrowId = 0;

        private readonly HashSet<Button> pressedSidebarButtons = new();
        private readonly HashSet<Button> styledSidebarButtons = new();

        private readonly HashSet<Button> pressedUiButtons = new();
        private readonly HashSet<Button> styledUiButtons = new();

        private readonly HashSet<Panel> styledUserPanels = new();
        private readonly HashSet<Panel> styledEquipmentCards = new();
        private readonly HashSet<Panel> pressedEquipmentCards = new();

        private Panel pnlExperimentDrawer;
        private FlowLayoutPanel flowExperimentList;
        private Button btnCloseExperimentDrawer;
        private bool isExperimentDrawerOpen = false;
        private ComboBox cmbCategoryFilter = null!;
        private ComboBox cmbCurrentSubjectFilter = null!;
        private Button btnSearchEquipment = null!;
        private Button btnBorrowCart = null!;
        private Button btnPendingBorrowSlips = null!;
        private List<BorrowCartItem> borrowCart = new List<BorrowCartItem>();
        private bool isRefreshingCategoryFilter = false;
        private bool isRefreshingSubjectFilter = false;
        private int selectedWholeDayScheduleId = 0;
        private int editingPendingSlipId = 0;
        private string editingPendingGroupNumber = "";
        private List<MemberEntry> editingPendingMembers = new List<MemberEntry>();

        private bool demoModeEnabled = false;


        private class ManualEquipmentItem
        {
            public string EquipmentName { get; set; } = "";
            public int Quantity { get; set; }
        }


        private class BorrowCartItem
        {
            public int EquipmentID { get; set; }

            public string EquipmentName { get; set; } = "";

            public int Quantity { get; set; }
        }

        private class MemberEntry
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
            public string SchoolID { get; set; } = "";

            public string DisplayText => FullName + " [" + SchoolID + "]";
        }

        private class CurrentLabContext
        {
            public int SubjectID { get; set; }
            public int ScheduleID { get; set; }
            public int LabID { get; set; }
            public string LabCode { get; set; } = "";
            public string SubjectCode { get; set; } = "";
            public string Section { get; set; } = "";
            public string DisplayText => LabCode + " - " + SubjectCode;
        }


        public frmUserDashboard()
        {
            InitializeComponent();

            ApplyRoundedUi();
            InitializeEquipmentChart();

            ApplyButtonStyle(btnLogout);
            ApplyButtonStyle(btnNavDashboard);
            ApplyButtonStyle(btnNavEquipment);
            ApplyButtonStyle(btnNavBorrowed);
            ApplyButtonStyle(btnNavHistory);
            ApplyButtonStyle(btnNavProfile);

            
            ConfigureHistoryGrid();
            InitializeHistorySearch();
            LoadUserProfileInfo();
            CreateEquipmentFilters();
            CreateBorrowCartButton();
            EnableEquipmentRefreshSmoothing();
            LoadEquipmentCards("All");
            ShowBorrowedEmptyState();

            ShowDashboardPanel();

            // SIDEBAR BUTTONS
            ApplySidebarNeumorphismButton(btnNavDashboard);
            ApplySidebarNeumorphismButton(btnNavEquipment);
            ApplySidebarNeumorphismButton(btnNavBorrowed);
            ApplySidebarNeumorphismButton(btnNavHistory);
            ApplySidebarNeumorphismButton(btnNavProfile);
            ApplySidebarNeumorphismButton(btnLogout);

            // DASHBOARD PANELS
            ApplyDashboardNeumorphism(cardBorrowed);
            ApplyDashboardNeumorphism(cardDueSoon);
            ApplyDashboardNeumorphism(cardOverdue);
            ApplyDashboardNeumorphism(cardHistory);
            ApplyDashboardNeumorphism(pnlWelcome);
            ApplyDashboardNeumorphism(pnlStatistics);

            // KEEP REMINDERS FLAT
            // ApplyDashboardNeumorphism(pnlReminders);

            // CATEGORY 

            // BORROWED POPUP IMPROVEMENT
            ApplyDashboardNeumorphism(pnlBorrowedPopup);

            // PROFILE CARD
            ApplyDashboardNeumorphism(pnlProfileCard);
        }

        private void EnableEquipmentRefreshSmoothing()
        {
            SetDoubleBuffered(flowEquipmentCards);
            SetDoubleBuffered(panelEquipment);
        }

        private void SetDoubleBuffered(Control? control)
        {
            if (control == null)
                return;

            typeof(Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }

        private void CreateBorrowCartButton()
        {
            if (btnBorrowCart != null)
                return;

            btnBorrowCart = new Button();
            btnBorrowCart.Name = "btnBorrowCart";
            btnBorrowCart.Text = "🧺 Cart (0)";
            btnBorrowCart.Size = new Size(135, 36);
            btnBorrowCart.Location = new Point(pnlEquipmentHeader.Width - 160, 54);
            btnBorrowCart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBorrowCart.BackColor = Color.FromArgb(212, 168, 45);
            btnBorrowCart.ForeColor = Color.FromArgb(72, 53, 84);
            btnBorrowCart.FlatStyle = FlatStyle.Flat;
            btnBorrowCart.FlatAppearance.BorderSize = 0;
            btnBorrowCart.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            btnBorrowCart.Click += (s, e) =>
            {
                ShowBorrowCartDialog();
            };

            pnlEquipmentHeader.Controls.Add(btnBorrowCart);
            btnBorrowCart.BringToFront();

            ApplyActionButtonStyle(btnBorrowCart);
            RoundControl(btnBorrowCart, 18);

            btnPendingBorrowSlips = new Button();
            btnPendingBorrowSlips.Name = "btnPendingBorrowSlips";
            btnPendingBorrowSlips.Text = "Borrower's Slip";
            btnPendingBorrowSlips.Size = new Size(160, 36);
            btnPendingBorrowSlips.Location = new Point(pnlEquipmentHeader.Width - 335, 54);
            btnPendingBorrowSlips.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPendingBorrowSlips.BackColor = Color.FromArgb(212, 168, 45);
            btnPendingBorrowSlips.ForeColor = Color.FromArgb(72, 53, 84);
            btnPendingBorrowSlips.FlatStyle = FlatStyle.Flat;
            btnPendingBorrowSlips.FlatAppearance.BorderSize = 0;
            btnPendingBorrowSlips.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnPendingBorrowSlips.Click += (s, e) => ShowPendingBorrowerSlipsDialog();

            pnlEquipmentHeader.Controls.Add(btnPendingBorrowSlips);
            btnPendingBorrowSlips.BringToFront();
            ApplyActionButtonStyle(btnPendingBorrowSlips);
            RoundControl(btnPendingBorrowSlips, 18);
        }

        private void UpdateBorrowCartButtonText()
        {
            if (btnBorrowCart == null)
                return;

            int totalItems = borrowCart.Count;
            btnBorrowCart.Text = "🧺 Cart (" + totalItems + ")";
        }

        private void CreateEquipmentFilters()
        {
            btnCatAll.Visible = false;
            btnCatTechnical.Visible = false;
            btnCatScience.Visible = false;
            btnCatSports.Visible = false;
            btnCatGeneral.Visible = false;

            txtUserEquipmentSearch.Location = new Point(24, 64);
            txtUserEquipmentSearch.Size = new Size(260, 28);

            cmbCurrentSubjectFilter = new ComboBox();
            cmbCurrentSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrentSubjectFilter.Font = new Font("Segoe UI", 10F);
            cmbCurrentSubjectFilter.Size = new Size(205, 28);
            cmbCurrentSubjectFilter.Location = new Point(304, 64);
            cmbCurrentSubjectFilter.Visible = false;
            cmbCurrentSubjectFilter.SelectedIndexChanged += (s, e) =>
            {
                if (isRefreshingSubjectFilter)
                    return;

                if (cmbCurrentSubjectFilter.SelectedItem is CurrentLabContext context)
                    selectedWholeDayScheduleId = context.ScheduleID;

                currentEquipmentCategory = "All";
                LoadEquipmentCards(currentEquipmentCategory, txtUserEquipmentSearch.Text.Trim());
            };

            cmbCategoryFilter = new ComboBox();
            cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoryFilter.Font = new Font("Segoe UI", 10F);
            cmbCategoryFilter.Size = new Size(145, 28);
            cmbCategoryFilter.Location = new Point(524, 64);
            cmbCategoryFilter.Items.Add("All Categories");
            cmbCategoryFilter.SelectedIndex = 0;
            cmbCategoryFilter.SelectedIndexChanged += (s, e) =>
            {
                if (isRefreshingCategoryFilter)
                    return;

                currentEquipmentCategory =
                    cmbCategoryFilter.Text == "All Categories"
                    ? "All"
                    : cmbCategoryFilter.Text;
                LoadEquipmentCards(currentEquipmentCategory, txtUserEquipmentSearch.Text.Trim());
            };

            btnSearchEquipment = new Button();
            btnSearchEquipment.Text = "Search";
            btnSearchEquipment.Size = new Size(100, 34);
            btnSearchEquipment.Location = new Point(850, 72);
            btnSearchEquipment.BackColor = Color.FromArgb(128, 0, 0);
            btnSearchEquipment.ForeColor = Color.White;
            btnSearchEquipment.FlatStyle = FlatStyle.Flat;
            btnSearchEquipment.FlatAppearance.BorderSize = 0;
            btnSearchEquipment.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSearchEquipment.Click += (s, e) =>
            {
                LoadEquipmentCards(currentEquipmentCategory);
            };
            ApplyActionButtonStyle(btnSearchEquipment);
            btnSearchEquipment.Visible = false;

            pnlEquipmentHeader.Controls.Add(cmbCurrentSubjectFilter);
            pnlEquipmentHeader.Controls.Add(cmbCategoryFilter);
            pnlEquipmentHeader.Controls.Add(btnSearchEquipment);

            cmbCurrentSubjectFilter.BringToFront();
            cmbCategoryFilter.BringToFront();
            btnSearchEquipment.BringToFront();
        }



        private Dictionary<string, List<ManualEquipmentItem>> GetExperimentManualList()
        {
            return new Dictionary<string, List<ManualEquipmentItem>>
    {
        {
            "Experiment 1 - Basic Circuit Testing",
            new List<ManualEquipmentItem>
            {
                new ManualEquipmentItem { EquipmentName = "Digital Multimeter", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Breadboard", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Resistor", Quantity = 5 },
                new ManualEquipmentItem { EquipmentName = "Jumper Wires", Quantity = 10 }
            }
        },
        {
            "Experiment 2 - Diode Testing",
            new List<ManualEquipmentItem>
            {
                new ManualEquipmentItem { EquipmentName = "Digital Multimeter", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Breadboard", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Diode", Quantity = 3 },
                new ManualEquipmentItem { EquipmentName = "Resistor", Quantity = 3 },
                new ManualEquipmentItem { EquipmentName = "Jumper Wires", Quantity = 10 }
            }
        },
        {
            "Experiment 3 - Transistor Circuit",
            new List<ManualEquipmentItem>
            {
                new ManualEquipmentItem { EquipmentName = "Digital Multimeter", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Breadboard", Quantity = 1 },
                new ManualEquipmentItem { EquipmentName = "Transistor", Quantity = 2 },
                new ManualEquipmentItem { EquipmentName = "Resistor", Quantity = 5 },
                new ManualEquipmentItem { EquipmentName = "Jumper Wires", Quantity = 10 }
            }
        }
    };
        }



        private void AddExperimentManualButton()
        {
            Control[] existing = pnlEquipmentHeader.Controls.Find("btnOpenExperimentManual", false);
            if (existing.Length > 0)
                return;

            Button btnOpenExperimentManual = new Button();
            btnOpenExperimentManual.Name = "btnOpenExperimentManual";
            btnOpenExperimentManual.Text = "Experiment Manual";
            btnOpenExperimentManual.Size = new Size(180, 32);
            btnOpenExperimentManual.Location = new Point(790, 62);
            btnOpenExperimentManual.BackColor = Color.FromArgb(212, 168, 45);
            btnOpenExperimentManual.ForeColor = Color.White;
            btnOpenExperimentManual.FlatStyle = FlatStyle.Flat;
            btnOpenExperimentManual.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnOpenExperimentManual.FlatAppearance.BorderSize = 0;

            btnOpenExperimentManual.Click += (s, e) =>
            {
                ToggleExperimentDrawer();
            };

            pnlEquipmentHeader.Controls.Add(btnOpenExperimentManual);
            btnOpenExperimentManual.BringToFront();

            ApplyActionButtonStyle(btnOpenExperimentManual);
            RoundControl(btnOpenExperimentManual, 18);
        }



        private void SetupExperimentDrawer()
        {
            if (pnlExperimentDrawer != null)
                return;

            pnlExperimentDrawer = new Panel();
            pnlExperimentDrawer.Name = "pnlExperimentDrawer";
            pnlExperimentDrawer.Size = new Size(320, panelEquipment.Height);
            pnlExperimentDrawer.Location = new Point(panelEquipment.Width, 0);
            pnlExperimentDrawer.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            pnlExperimentDrawer.BackColor = Color.FromArgb(255, 251, 252);
            pnlExperimentDrawer.Visible = false;

            Label lblTitle = new Label();
            lblTitle.Text = "Experiment Manual";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblTitle.Location = new Point(24, 26);
            lblTitle.AutoSize = true;

            Label lblSub = new Label();
            lblSub.Text = "Choose an experiment to borrow its required equipment.";
            lblSub.Font = new Font("Segoe UI", 9.5F);
            lblSub.ForeColor = Color.FromArgb(126, 105, 136);
            lblSub.Location = new Point(24, 62);
            lblSub.Size = new Size(260, 40);

            btnCloseExperimentDrawer = new Button();
            btnCloseExperimentDrawer.Text = "×";
            btnCloseExperimentDrawer.Size = new Size(36, 36);
            btnCloseExperimentDrawer.Location = new Point(270, 18);
            btnCloseExperimentDrawer.FlatStyle = FlatStyle.Flat;
            btnCloseExperimentDrawer.FlatAppearance.BorderSize = 0;
            btnCloseExperimentDrawer.BackColor = Color.Transparent;
            btnCloseExperimentDrawer.ForeColor = Color.FromArgb(92, 45, 58);
            btnCloseExperimentDrawer.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            btnCloseExperimentDrawer.Click += (s, e) =>
            {
                CloseExperimentDrawer();
            };

            flowExperimentList = new FlowLayoutPanel();
            flowExperimentList.Name = "flowExperimentList";
            flowExperimentList.Location = new Point(22, 120);
            flowExperimentList.Size = new Size(276, 500);
            flowExperimentList.AutoScroll = true;
            flowExperimentList.FlowDirection = FlowDirection.TopDown;
            flowExperimentList.WrapContents = false;
            flowExperimentList.BackColor = Color.Transparent;

            pnlExperimentDrawer.Controls.Add(lblTitle);
            pnlExperimentDrawer.Controls.Add(lblSub);
            pnlExperimentDrawer.Controls.Add(btnCloseExperimentDrawer);
            pnlExperimentDrawer.Controls.Add(flowExperimentList);

            panelEquipment.Controls.Add(pnlExperimentDrawer);
            pnlExperimentDrawer.BringToFront();

            RoundControl(pnlExperimentDrawer, 28);
            LoadExperimentDrawerButtons();
        }




        private void LoadExperimentDrawerButtons()
        {
            flowExperimentList.Controls.Clear();

            Dictionary<string, List<ManualEquipmentItem>> experiments = GetExperimentManualList();

            foreach (string experimentName in experiments.Keys)
            {
                Button btnExperiment = new Button();
                btnExperiment.Text = experimentName;
                btnExperiment.Size = new Size(250, 52);
                btnExperiment.Margin = new Padding(0, 0, 0, 12);
                btnExperiment.BackColor = Color.FromArgb(241, 233, 245);
                btnExperiment.ForeColor = Color.FromArgb(72, 53, 84);
                btnExperiment.FlatStyle = FlatStyle.Flat;
                btnExperiment.FlatAppearance.BorderSize = 0;
                btnExperiment.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                btnExperiment.TextAlign = ContentAlignment.MiddleLeft;
                btnExperiment.Padding = new Padding(14, 0, 0, 0);

                btnExperiment.Click += (s, e) =>
                {
                    ShowExperimentBorrowSlipDialog(experimentName, experiments[experimentName]);
                };

                flowExperimentList.Controls.Add(btnExperiment);

                ApplyActionButtonStyle(btnExperiment);
                RoundControl(btnExperiment, 18);
            }
        }




        private void ToggleExperimentDrawer()
        {
            SetupExperimentDrawer();

            if (isExperimentDrawerOpen)
                CloseExperimentDrawer();
            else
                OpenExperimentDrawer();
        }

        private void OpenExperimentDrawer()
        {
            SetupExperimentDrawer();

            pnlExperimentDrawer.Visible = true;
            pnlExperimentDrawer.Location = new Point(panelEquipment.Width - pnlExperimentDrawer.Width - 10, 0);
            pnlExperimentDrawer.Height = panelEquipment.Height;
            pnlExperimentDrawer.BringToFront();

            isExperimentDrawerOpen = true;
        }

        private void CloseExperimentDrawer()
        {
            if (pnlExperimentDrawer == null)
                return;

            pnlExperimentDrawer.Visible = false;
            isExperimentDrawerOpen = false;
        }





        private void ShowExperimentBorrowSlipDialog(string experimentName, List<ManualEquipmentItem> items)
        {
            Form slipForm = new Form();
            slipForm.Text = "Experiment Borrow Slip";
            slipForm.StartPosition = FormStartPosition.CenterParent;
            slipForm.Size = new Size(540, 620);
            slipForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            slipForm.MaximizeBox = false;
            slipForm.MinimizeBox = false;
            slipForm.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label();
            lblTitle.Text = experimentName;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblTitle.Location = new Point(28, 22);
            lblTitle.Size = new Size(460, 34);

            Label lblDate = new Label();
            lblDate.Text = "Date: " + DateTime.Now.ToString("MMMM dd, yyyy");
            lblDate.Font = new Font("Segoe UI", 9.5F);
            lblDate.ForeColor = Color.FromArgb(126, 105, 136);
            lblDate.Location = new Point(30, 60);
            lblDate.AutoSize = true;

            Label lblGroup = new Label();
            lblGroup.Text = "Group Number";
            lblGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblGroup.Location = new Point(30, 96);
            lblGroup.AutoSize = true;

            TextBox txtGroup = new TextBox();
            txtGroup.Font = new Font("Segoe UI", 10F);
            txtGroup.Location = new Point(30, 122);
            txtGroup.Size = new Size(160, 28);

            Label lblLeader = new Label();
            lblLeader.Text = "Group Leader";
            lblLeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLeader.Location = new Point(30, 162);
            lblLeader.AutoSize = true;

            TextBox txtLeader = new TextBox();
            txtLeader.Font = new Font("Segoe UI", 10F);
            txtLeader.Location = new Point(30, 188);
            txtLeader.Size = new Size(430, 28);
            txtLeader.Text = SessionManager.FullName;

            Label lblMember = new Label();
            lblMember.Text = "Group Members";
            lblMember.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMember.Location = new Point(30, 228);
            lblMember.AutoSize = true;

            TextBox txtMember = new TextBox();
            txtMember.Font = new Font("Segoe UI", 10F);
            txtMember.Location = new Point(30, 254);
            txtMember.Size = new Size(300, 28);
            txtMember.PlaceholderText = "Enter member name...";

            Button btnAddMember = new Button();
            btnAddMember.Text = "+ Add Member";
            btnAddMember.Size = new Size(120, 30);
            btnAddMember.Location = new Point(340, 253);
            btnAddMember.BackColor = Color.FromArgb(212, 168, 45);
            btnAddMember.ForeColor = Color.White;
            btnAddMember.FlatStyle = FlatStyle.Flat;
            btnAddMember.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnAddMember.FlatAppearance.BorderSize = 0;

            FlowLayoutPanel flowMembers = new FlowLayoutPanel();
            flowMembers.Location = new Point(30, 292);
            flowMembers.Size = new Size(430, 80);
            flowMembers.AutoScroll = true;
            flowMembers.FlowDirection = FlowDirection.TopDown;
            flowMembers.WrapContents = false;
            flowMembers.BackColor = Color.FromArgb(255, 251, 252);
            flowMembers.BorderStyle = BorderStyle.FixedSingle;

            Label lblEquip = new Label();
            lblEquip.Text = "Equipment included";
            lblEquip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEquip.Location = new Point(30, 390);
            lblEquip.AutoSize = true;

            ListBox lstEquipment = new ListBox();
            lstEquipment.Font = new Font("Segoe UI", 9.5F);
            lstEquipment.Location = new Point(30, 416);
            lstEquipment.Size = new Size(430, 80);

            foreach (ManualEquipmentItem item in items)
            {
                lstEquipment.Items.Add(item.EquipmentName + " - Qty: " + item.Quantity);
            }

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(120, 38);
            btnCancel.Location = new Point(220, 520);
            btnCancel.BackColor = Color.FromArgb(214, 197, 224);
            btnCancel.ForeColor = Color.FromArgb(87, 60, 99);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnCancel.FlatAppearance.BorderSize = 0;

            Button btnSubmit = new Button();
            btnSubmit.Text = "Submit Slip";
            btnSubmit.Size = new Size(130, 38);
            btnSubmit.Location = new Point(350, 520);
            btnSubmit.BackColor = Color.FromArgb(169, 215, 159);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSubmit.FlatAppearance.BorderSize = 0;

            List<string> members = new List<string>();

            void RefreshLayout()
            {
                int memberHeight = Math.Min(120, Math.Max(80, flowMembers.Controls.Count * 34 + 10));
                flowMembers.Height = memberHeight;

                int nextY = flowMembers.Bottom + 18;

                lblEquip.Location = new Point(30, nextY);
                lstEquipment.Location = new Point(30, nextY + 26);

                btnCancel.Location = new Point(220, lstEquipment.Bottom + 20);
                btnSubmit.Location = new Point(350, lstEquipment.Bottom + 20);

                slipForm.Height = btnSubmit.Bottom + 80;
            }

            void AddMemberToList(string memberName)
            {
                Panel memberPanel = new Panel();
                memberPanel.Size = new Size(400, 28);
                memberPanel.BackColor = Color.FromArgb(241, 233, 245);
                memberPanel.Margin = new Padding(4);

                Label lblName = new Label();
                lblName.Text = memberName;
                lblName.Font = new Font("Segoe UI", 9.5F);
                lblName.ForeColor = Color.FromArgb(72, 53, 84);
                lblName.Location = new Point(10, 5);
                lblName.AutoSize = true;

                Button btnRemove = new Button();
                btnRemove.Text = "x";
                btnRemove.Size = new Size(28, 24);
                btnRemove.Location = new Point(365, 2);
                btnRemove.FlatStyle = FlatStyle.Flat;
                btnRemove.FlatAppearance.BorderSize = 0;
                btnRemove.BackColor = Color.Transparent;
                btnRemove.ForeColor = Color.FromArgb(180, 60, 70);
                btnRemove.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                btnRemove.Click += (s, e) =>
                {
                    members.Remove(memberName);
                    flowMembers.Controls.Remove(memberPanel);
                    RefreshLayout();
                };

                memberPanel.Controls.Add(lblName);
                memberPanel.Controls.Add(btnRemove);
                flowMembers.Controls.Add(memberPanel);

                RoundControl(memberPanel, 10);
                RefreshLayout();
            }

            btnAddMember.Click += (s, e) =>
            {
                string memberName = txtMember.Text.Trim();

                if (string.IsNullOrWhiteSpace(memberName))
                {
                    MessageBox.Show("Please enter a member name first.", "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (members.Contains(memberName))
                {
                    MessageBox.Show("This member is already added.", "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                members.Add(memberName);
                AddMemberToList(memberName);
                txtMember.Clear();
                txtMember.Focus();
            };

            btnCancel.Click += (s, e) =>
            {
                slipForm.Close();
            };

            btnSubmit.Click += (s, e) =>
            {
                string leaderName = txtLeader.Text.Trim();
                string groupNumber = txtGroup.Text.Trim();
                string memberText = string.Join(", ", members);

                if (string.IsNullOrWhiteSpace(groupNumber))
                {
                    MessageBox.Show("Please enter the group number.", "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(leaderName))
                {
                    MessageBox.Show("Please enter the group leader.", "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveExperimentManualSlip(experimentName, leaderName, groupNumber, memberText, items);
                slipForm.Close();
                CloseExperimentDrawer();
            };

            slipForm.Controls.Add(lblTitle);
            slipForm.Controls.Add(lblDate);
            slipForm.Controls.Add(lblGroup);
            slipForm.Controls.Add(txtGroup);
            slipForm.Controls.Add(lblLeader);
            slipForm.Controls.Add(txtLeader);
            slipForm.Controls.Add(lblMember);
            slipForm.Controls.Add(txtMember);
            slipForm.Controls.Add(btnAddMember);
            slipForm.Controls.Add(flowMembers);
            slipForm.Controls.Add(lblEquip);
            slipForm.Controls.Add(lstEquipment);
            slipForm.Controls.Add(btnCancel);
            slipForm.Controls.Add(btnSubmit);

            RefreshLayout();

            slipForm.ShowDialog(this);
        }



        private void SaveExperimentManualSlip(
    string experimentName,
    string leaderName,
    string groupNumber,
    string members,
    List<ManualEquipmentItem> items)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    int subjectId = 1;

                    string getSubjectQuery = @"
SELECT TOP 1 SubjectID
FROM LabSubjects
ORDER BY SubjectID";

                    using (OleDbCommand subjectCmd = new OleDbCommand(getSubjectQuery, conn, trans))
                    {
                        object result = subjectCmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                            subjectId = Convert.ToInt32(result);
                    }

                    string insertSlipQuery = @"
INSERT INTO BorrowSlips
(UserID, SubjectID, GroupNumber, LeaderName, SlipType, DateCreated, SlipStatus)
VALUES (?, ?, ?, ?, ?, ?, ?)";

                    int newSlipId = 0;

                    using (OleDbCommand cmd = new OleDbCommand(insertSlipQuery, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                        cmd.Parameters.AddWithValue("@p2", subjectId);
                        cmd.Parameters.AddWithValue("@p3", groupNumber);
                        cmd.Parameters.AddWithValue("@p4", leaderName);
                        cmd.Parameters.AddWithValue("@p5", experimentName);
                        cmd.Parameters.AddWithValue("@p6", DateTime.Now);
                        cmd.Parameters.AddWithValue("@p7", "Pending");
                        cmd.ExecuteNonQuery();
                    }

                    using (OleDbCommand idCmd = new OleDbCommand("SELECT @@IDENTITY", conn, trans))
                    {
                        newSlipId = Convert.ToInt32(idCmd.ExecuteScalar());
                    }

                    foreach (ManualEquipmentItem item in items)
                    {
                        int equipmentId = 0;

                        string findEquipmentQuery = @"
SELECT TOP 1 EquipmentID
FROM Equipment
WHERE EquipmentName = ?
AND IsArchived = False";

                        using (OleDbCommand eqCmd = new OleDbCommand(findEquipmentQuery, conn, trans))
                        {
                            eqCmd.Parameters.AddWithValue("@p1", item.EquipmentName);

                            object result = eqCmd.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                            {
                                MessageBox.Show("Equipment not found: " + item.EquipmentName,
                                    "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                trans.Rollback();
                                return;
                            }

                            equipmentId = Convert.ToInt32(result);
                        }

                        string insertItemQuery = @"
INSERT INTO BorrowSlipItems
(SlipID, EquipmentID, QuantityRequested, ItemReturnStatus)
VALUES (?, ?, ?, ?)";

                        using (OleDbCommand itemCmd = new OleDbCommand(insertItemQuery, conn, trans))
                        {
                            itemCmd.Parameters.AddWithValue("@p1", newSlipId);
                            itemCmd.Parameters.AddWithValue("@p2", equipmentId);
                            itemCmd.Parameters.AddWithValue("@p3", item.Quantity);
                            itemCmd.Parameters.AddWithValue("@p4", "Pending");
                            itemCmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();

                    MessageBox.Show("Experiment borrow slip submitted successfully.",
                        "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadUserDashboardData();
                    LoadEquipmentCards(currentEquipmentCategory);
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving experiment borrow slip:\n" + ex.Message,
                    "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void ApplySidebarNeumorphismButton(Button btn)
        {
            if (styledSidebarButtons.Contains(btn))
                return;

            styledSidebarButtons.Add(btn);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.ForeColor = Color.White;

            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedSidebarButtons.Add(btn);
                    btn.Invalidate();
                }
            };

            btn.MouseUp += (s, e) =>
            {
                pressedSidebarButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.MouseLeave += (s, e) =>
            {
                pressedSidebarButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.Paint += (s, e) =>
            {
                if (s is not Button b) return;

                bool isPressed = pressedSidebarButtons.Contains(b);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using GraphicsPath path = GetUiRoundedPath(rect, 20);

                b.Region = new Region(path);

                // ✅ YELLOW THEME ONLY
                Color baseColor = isPressed
                    ? Color.FromArgb(184, 140, 25)   // darker yellow (pressed)
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


        private void InitializeHistorySearch()
        {
            lblHistoryTitle.Visible = false;

            foreach (Control ctrl in pnlHistoryCard.Controls.OfType<TextBox>().ToList())
            {
                if (ctrl.Name == "txtHistorySearch")
                    pnlHistoryCard.Controls.Remove(ctrl);
            }

            TextBox txtHistorySearch = new TextBox
            {
                Name = "txtHistorySearch",
                PlaceholderText = "Search equipment or category...",
                Size = new Size(300, 30),
                Location = new Point(28, 24),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtHistorySearch.TextChanged += (s, e) =>
            {
                FilterUserHistory(txtHistorySearch.Text.Trim());
            };

            pnlHistoryCard.Controls.Add(txtHistorySearch);
            txtHistorySearch.BringToFront();
        }



        private void LoadEquipmentUsageChart()
        {
            if (chartEquipmentUsage == null)
                return;

            try
            {
                lblStatisticsSub.Text = "Top returned equipment by total quantity";
                lblStatisticsSub.ForeColor = Color.FromArgb(126, 105, 136);

                chartEquipmentUsage.Series.Clear();
                chartEquipmentUsage.ChartAreas.Clear();
                chartEquipmentUsage.Titles.Clear();
                chartEquipmentUsage.Legends.Clear();

                ChartArea area = new ChartArea("MainArea");
                area.AxisX.Interval = 1;
                area.AxisX.LabelStyle.Angle = -25;
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
                area.AxisX.LineColor = Color.FromArgb(160, 130, 155);
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.Minimum = 0;
                area.AxisY.Title = "Returned Quantity";
                area.AxisY.TitleFont = new Font("Segoe UI", 8F, FontStyle.Bold);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
                area.AxisY.LineColor = Color.FromArgb(160, 130, 155);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(225, 214, 229);
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                area.BackColor = Color.White;
                chartEquipmentUsage.ChartAreas.Add(area);

                Series series = new Series("Returned");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.IsXValueIndexed = true;
                series.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                series.LabelForeColor = Color.Black;
                series.Color = Color.FromArgb(155, 100, 180);
                series["PointWidth"] = "0.55";
                series["DrawingStyle"] = "Default";
                series["BarLabelStyle"] = "Center";
                series.Label = "#VALY";
                chartEquipmentUsage.Series.Add(series);

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT TOP 5
    TRIM(E.EquipmentName) AS EquipmentName,
    SUM(BSI.QuantityReturned) AS TotalReturned
FROM (((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Returned'
AND BSI.QuantityReturned > 0
AND BS.SubjectID IN
(
    SELECT SubjectID
    FROM StudentSubjectEnrollments
    WHERE UserID = ?
    AND IsActive = True
)
AND
(
    BS.ScheduleID IS NULL
    OR BS.ScheduleID IN
    (
        SELECT ScheduleID
        FROM StudentSubjectEnrollments
        WHERE UserID = ?
        AND IsActive = True
    )
)
GROUP BY TRIM(E.EquipmentName)
ORDER BY SUM(BSI.QuantityReturned) DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
                cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasData = false;

                while (reader != null && reader.Read())
                {
                    string name = reader["EquipmentName"]?.ToString() ?? "Unknown";

                    int total = reader["TotalReturned"] != DBNull.Value
                        ? Convert.ToInt32(reader["TotalReturned"])
                        : 0;

                    string displayName = name.Length > 14
                        ? name.Substring(0, 13) + "…"
                        : name;

                    int pointIndex = series.Points.AddXY(displayName, total);
                    series.Points[pointIndex].Tag = name;
                    series.Points[pointIndex].Label = total.ToString();
                    hasData = true;
                }

                if (!hasData)
                {
                    int pointIndex = series.Points.AddXY("No Data", 0);
                    series.Points[pointIndex].Label = "";
                }
                else
                {
                    double maxValue = series.Points.Max(p => p.YValues[0]);
                    area.AxisY.Maximum = Math.Ceiling(Math.Max(1, maxValue) * 1.15);
                }

                chartEquipmentUsage.MouseMove -= chartEquipmentUsage_MouseMove;
                chartEquipmentUsage.MouseLeave -= chartEquipmentUsage_MouseLeave;
                chartEquipmentUsage.MouseMove += chartEquipmentUsage_MouseMove;
                chartEquipmentUsage.MouseLeave += chartEquipmentUsage_MouseLeave;
                chartEquipmentUsage.Visible = true;
            }
            catch
            {
                chartEquipmentUsage.Visible = false;
            }
        }

        private void chartEquipmentUsage_MouseMove(object? sender, MouseEventArgs e)
        {
            if (chartEquipmentUsage.Series.Count == 0)
                return;

            HitTestResult hit = chartEquipmentUsage.HitTest(e.X, e.Y);
            Series series = chartEquipmentUsage.Series[0];

            for (int i = 0; i < series.Points.Count; i++)
            {
                DataPoint point = series.Points[i];
                point.Color = Color.FromArgb(155, 100, 180);
                point["PointWidth"] = "0.55";
                point.Label = point.YValues[0] > 0 ? ((int)point.YValues[0]).ToString() : "";
                point.IsValueShownAsLabel = point.YValues[0] > 0;
                point.LabelForeColor = Color.Black;
            }

            if (hit.ChartElementType == ChartElementType.DataPoint && hit.PointIndex >= 0)
            {
                DataPoint point = series.Points[hit.PointIndex];
                point.Color = Color.FromArgb(128, 0, 0);
                point["PointWidth"] = "0.78";
                point.Label = ((int)point.YValues[0]).ToString();
                point.IsValueShownAsLabel = true;
                point.LabelForeColor = Color.Black;

                lblStatisticsSub.Text = point.Tag?.ToString() ?? point.AxisLabel;
                lblStatisticsSub.ForeColor = Color.FromArgb(153, 0, 0);
            }
        }

        private void chartEquipmentUsage_MouseLeave(object? sender, EventArgs e)
        {
            if (chartEquipmentUsage.Series.Count == 0)
                return;

            foreach (DataPoint point in chartEquipmentUsage.Series[0].Points)
            {
                point.Color = Color.FromArgb(155, 100, 180);
                point["PointWidth"] = "0.55";
                point.Label = point.YValues[0] > 0 ? ((int)point.YValues[0]).ToString() : "";
                point.IsValueShownAsLabel = point.YValues[0] > 0;
                point.LabelForeColor = Color.Black;
            }

            lblStatisticsSub.Text = "Top returned equipment by total quantity";
            lblStatisticsSub.ForeColor = Color.FromArgb(126, 105, 136);
        }




        private void FilterUserHistory(string keyword)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    LS.SubjectCode,
    SS.Section,
    BS.LeaderName,
    U.SchoolID,
    SUM(BSI.QuantityRequested) AS TotalQuantity,
    BS.DateCreated AS BorrowDate,
    'Returned' AS HistoryStatus
FROM ((((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
WHERE
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
AND BS.SlipStatus = 'Approved'
GROUP BY BS.SlipID, BS.GroupNumber, LS.SubjectCode, SS.Section, BS.LeaderName, U.SchoolID, BS.DateCreated
HAVING (SUM(IIF(BSI.ItemReturnStatus = 'Borrowed', 1, 0)) = 0
AND SUM(IIF(BSI.ItemReturnStatus = 'Returned', 1, 0)) > 0)
OR BS.SlipID IN (SELECT SlipID FROM DamageReports)
ORDER BY BS.DateCreated DESC";

                using var cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                cmd.Parameters.AddWithValue("@p2", SessionManager.UserID);

                DataTable dt = new DataTable();
                using var da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string escaped = keyword
                        .Replace("'", "''")
                        .Replace("[", "[[]")
                        .Replace("%", "[%]")
                        .Replace("*", "[*]");

                    DataView view = new DataView(dt)
                    {
                        RowFilter =
                            "Convert(GroupNumber, 'System.String') LIKE '%" + escaped + "%' OR " +
                            "Convert(SubjectCode, 'System.String') LIKE '%" + escaped + "%' OR " +
                            "Convert(Section, 'System.String') LIKE '%" + escaped + "%' OR " +
                            "Convert(LeaderName, 'System.String') LIKE '%" + escaped + "%' OR " +
                            "Convert(SchoolID, 'System.String') LIKE '%" + escaped + "%' OR " +
                            "Convert(HistoryStatus, 'System.String') LIKE '%" + escaped + "%'"
                    };

                    dgvHistory.DataSource = view;
                }
                else
                {
                    dgvHistory.DataSource = dt;
                }

                dgvHistory.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering history:\n" + ex.Message,
                    "History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private GraphicsPath GetUiRoundedPath(Rectangle rect, int radius)
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



        private void RestoreReminderStyle()
        {
            pnlReminders.BackColor = Color.FromArgb(255, 251, 252); // original flat look
            pnlReminders.Invalidate();
        }


        private void ApplyActionButtonStyle(Button btn)
        {
            if (styledUiButtons.Contains(btn))
                return;

            styledUiButtons.Add(btn);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.ForeColor = Color.FromArgb(87, 60, 99);

            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedUiButtons.Add(btn);
                    btn.Invalidate();
                }
            };

            btn.MouseUp += (s, e) =>
            {
                pressedUiButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.MouseLeave += (s, e) =>
            {
                pressedUiButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.Paint += (s, e) =>
            {
                if (s is not Button b) return;

                bool isPressed = pressedUiButtons.Contains(b);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using GraphicsPath path = GetUiRoundedPath(rect, 18);

                b.Region = new Region(path);

                Color baseColor = b.BackColor;
                Color lightEdge = isPressed
                    ? Color.FromArgb(170, 150, 190)
                    : Color.FromArgb(245, 240, 247);
                Color darkEdge = isPressed
                    ? Color.FromArgb(245, 240, 247)
                    : Color.FromArgb(170, 150, 190);

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
                    ? new Rectangle(1, 1, b.Width - 2, b.Height - 2)
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



        private void ResetSidebarButtons()
        {
            Color baseYellow = Color.FromArgb(212, 168, 45);

            btnNavDashboard.BackColor = baseYellow;
            btnNavEquipment.BackColor = baseYellow;
            btnNavBorrowed.BackColor = baseYellow;
            btnNavHistory.BackColor = baseYellow;
            btnNavProfile.BackColor = baseYellow;

            btnNavDashboard.ForeColor = Color.White;
            btnNavEquipment.ForeColor = Color.White;
            btnNavBorrowed.ForeColor = Color.White;
            btnNavHistory.ForeColor = Color.White;
            btnNavProfile.ForeColor = Color.White;
        }


        private void ApplyEquipmentCardStyle(Panel card)
        {
            if (styledEquipmentCards.Contains(card))
                return;

            styledEquipmentCards.Add(card);

            void pressDown(object? s, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedEquipmentCards.Add(card);
                    card.Invalidate();
                }
            }

            void pressUp(object? s, EventArgs e)
            {
                pressedEquipmentCards.Remove(card);
                card.Invalidate();
            }

            card.MouseDown += pressDown;
            card.MouseUp += pressUp;
            card.MouseLeave += pressUp;

            card.Paint += (s, e) =>
            {
                if (s is not Panel p) return;

                bool isPressed = pressedEquipmentCards.Contains(p);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                using GraphicsPath path = GetUiRoundedPath(rect, 24);

                p.Region = new Region(path);

                Color baseColor = p.BackColor;
                Color lightEdge = isPressed
                    ? Color.FromArgb(205, 190, 215)
                    : Color.FromArgb(255, 255, 255);
                Color darkEdge = isPressed
                    ? Color.FromArgb(255, 255, 255)
                    : Color.FromArgb(205, 190, 215);

                using SolidBrush brush = new SolidBrush(baseColor);
                e.Graphics.FillPath(brush, path);

                using Pen lightPen = new Pen(lightEdge, 3);
                using Pen darkPen = new Pen(darkEdge, 3);

                e.Graphics.DrawArc(lightPen, 1, 1, 28, 28, 180, 90);
                e.Graphics.DrawLine(lightPen, 15, 1, rect.Width - 15, 1);
                e.Graphics.DrawLine(lightPen, 1, 15, 1, rect.Height - 15);

                e.Graphics.DrawArc(darkPen, rect.Width - 29, rect.Height - 29, 28, 28, 0, 90);
                e.Graphics.DrawLine(darkPen, 15, rect.Height - 1, rect.Width - 15, rect.Height - 1);
                e.Graphics.DrawLine(darkPen, rect.Width - 1, 15, rect.Width - 1, rect.Height - 15);
            };
        }



        private void SetActiveSidebarButton(Button btn)
        {
            ResetSidebarButtons();

            btn.BackColor = Color.FromArgb(212, 168, 45);
            btn.ForeColor = Color.White;
            btn.Invalidate();
        }


        private GraphicsPath GetUserSidebarRoundedPath(Rectangle rect, int radius)
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


        private void SetActiveButton(Button btn)
        {
            ResetSidebarButtons();

            btn.BackColor = Color.FromArgb(212, 168, 45); // GOLD
            btn.ForeColor = Color.White;
        }




        private void ShowBorrowedPanel()
        {
            panelDashboard.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = true;
            panelHistory.Visible = false;
            panelProfile.Visible = false;
            pnlBorrowedPopup.Visible = false;

            panelBorrowed.BringToFront();

            lblPageTitle.Text = "Borrowing";
            ResetSidebarButtons();
            SetActiveButton(btnNavBorrowed);

            FixApprovedSlipItemsToBorrowedForCurrentLab();

            flowBorrowedItems.Controls.Clear();
            LoadBorrowedItems();
            LoadUserPaymentNotices();

            if (flowBorrowedItems.Controls.Count > 0)
            {
                pnlBorrowedEmptyState.Visible = false;
                flowBorrowedItems.Visible = true;
            }
            else
            {
                ShowBorrowedEmptyState();
            }
        }



        private void FixApprovedSlipItemsToBorrowedForCurrentLab()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Borrowed'
WHERE SlipID IN
(
    SELECT BS.SlipID
    FROM BorrowSlips AS BS
    INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
    WHERE LS.LabID = ?
    AND BS.SlipStatus = 'Approved'
)
AND
(
    ItemReturnStatus IS NULL
    OR ItemReturnStatus = ''
    OR ItemReturnStatus = 'Pending'
)";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.LabID;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // silent fixer
            }
        }

        private void LoadUserPaymentNotices()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    DR.ReportID,
    DR.DamageType,
    DR.DamageQuantity,
    DR.CurrentReplacementCost,
    DR.IndividualShare,
    DR.ReportStatus,
    DR.DateReported,
    E.EquipmentName,
    LS.SubjectCode,
    BS.GroupNumber,
    BS.LeaderName
FROM (((((DamageReportMembers AS DRM
INNER JOIN DamageReports AS DR ON DRM.ReportID = DR.ReportID)
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN BorrowSlips AS BS ON DR.SlipID = BS.SlipID)
INNER JOIN SubjectSchedules AS SS ON DR.ScheduleID = SS.ScheduleID)
WHERE DRM.UserID = ?
AND DRM.HasPaid = False
AND DR.ReportStatus = 'For Payment'
AND SS.DayOfWeek = ?
AND SS.StartTime <= ?
AND SS.EndTime >= ?
ORDER BY DR.DateReported DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                cmd.Parameters.AddWithValue("@p2", DateTime.Now.DayOfWeek.ToString());
                cmd.Parameters.AddWithValue("@p3", DateTime.Now.ToString("HH:mm:ss"));
                cmd.Parameters.AddWithValue("@p4", DateTime.Now.ToString("HH:mm:ss"));

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasPaymentNotice = false;

                while (reader != null && reader.Read())
                {
                    hasPaymentNotice = true;

                    int reportId = Convert.ToInt32(reader["ReportID"]);
                    string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                    string damageType = reader["DamageType"]?.ToString() ?? "";
                    string subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    string groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    string leaderName = reader["LeaderName"]?.ToString() ?? "";

                    int quantity = reader["DamageQuantity"] != DBNull.Value
                        ? Convert.ToInt32(reader["DamageQuantity"])
                        : 1;

                    decimal totalCost = reader["CurrentReplacementCost"] != DBNull.Value
                        ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                        : 0;

                    decimal individualShare = reader["IndividualShare"] != DBNull.Value
                        ? Convert.ToDecimal(reader["IndividualShare"])
                        : 0;

                    Panel card = CreateUserPaymentNoticeCard(
                        reportId,
                        equipmentName,
                        damageType,
                        quantity,
                        subjectCode,
                        groupNumber,
                        leaderName,
                        totalCost,
                        individualShare);

                    flowBorrowedItems.Controls.Add(card);
                }

                if (hasPaymentNotice)
                {
                    pnlBorrowedEmptyState.Visible = false;
                    flowBorrowedItems.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading payment notices:\n" + ex.Message,
                    "Equipment Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private Panel CreateUserPaymentNoticeCard(
    int reportId,
    string equipmentName,
    string damageType,
    int quantity,
    string subjectCode,
    string groupNumber,
    string leaderName,
    decimal totalCost,
    decimal individualShare)
        {
            Panel card = new Panel();
            card.Width = 850;
            card.Height = 170;
            card.BackColor = Color.FromArgb(255, 246, 230);
            card.Margin = new Padding(0, 0, 0, 14);

            RoundControl(card, 18);

            Label lblTitle = new Label();
            lblTitle.Text = "Equipment Report";
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(160, 98, 27);
            lblTitle.Location = new Point(20, 14);
            lblTitle.Size = new Size(300, 25);

            Label lblInfo = new Label();
            lblInfo.Text =
                "Your group has a reported equipment item that must be paid before borrowing in this lab.\n" +
                equipmentName + " | " + damageType + " | Qty: " + quantity +
                "\nSubject: " + subjectCode + " | Group: " + groupNumber +
                "\nLeader: " + leaderName;
            lblInfo.Font = new Font("Segoe UI", 9.5F);
            lblInfo.ForeColor = Color.FromArgb(92, 45, 58);
            lblInfo.Location = new Point(20, 45);
            lblInfo.Size = new Size(520, 88);

            Label lblCost = new Label();
            lblCost.Text =
                "Total: ₱" + totalCost.ToString("N2") +
                "\nYour Share: ₱" + individualShare.ToString("N2");
            lblCost.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCost.ForeColor = Color.FromArgb(72, 53, 84);
            lblCost.Location = new Point(550, 45);
            lblCost.Size = new Size(250, 50);

            Button btnPrint = new Button();
            btnPrint.Text = "Receipt";
            btnPrint.Size = new Size(170, 36);
            btnPrint.Location = new Point(620, 112);
            btnPrint.BackColor = Color.FromArgb(212, 168, 45);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            btnPrint.Click += (s, e) =>
            {
                ShowPaymentSlipPopup(reportId);
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblInfo);
            card.Controls.Add(lblCost);
            card.Controls.Add(btnPrint);

            RoundControl(btnPrint, 14);

            return card;
        }



        private string BuildUserPaymentSlipText(
    int reportId,
    string equipmentName,
    string damageType,
    int quantity,
    string subjectCode,
    string groupNumber,
    string leaderName,
    decimal totalCost,
    decimal individualShare)
        {
            return
                "WILDCATHUB PAYMENT SLIP\n" +
                "Damage/Lost Equipment Payment\n\n" +
                "Report ID: " + reportId + "\n" +
                "Student: " + SessionManager.FullName + "\n" +
                "School ID: " + SessionManager.SchoolID + "\n\n" +
                "Subject: " + subjectCode + "\n" +
                "Group Number: " + groupNumber + "\n" +
                "Leader: " + leaderName + "\n\n" +
                "Equipment: " + equipmentName + "\n" +
                "Damage Type: " + damageType + "\n" +
                "Quantity: " + quantity + "\n\n" +
                "Total Group Payment: ₱" + totalCost.ToString("N2") + "\n" +
                "Individual Share: ₱" + individualShare.ToString("N2") + "\n\n" +
                "Instructions:\n" +
                "1. Print this slip.\n" +
                "2. Get Admin/NAS signature.\n" +
                "3. Proceed to cashier for payment.\n" +
                "4. Return official cashier receipt to the admin.\n\n" +
                "Admin/NAS Signature: ______________________\n\n" +
                "Cashier Receipt No.: ______________________\n" +
                "Date Paid: ________________________________";
        }



        private void PrintUserPaymentSlip(string slipText)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                printDoc.PrintPage += (s, e) =>
                {
                    Font titleFont =
                        new Font("Segoe UI", 18F, FontStyle.Bold);

                    Font subFont =
                        new Font("Segoe UI", 10F, FontStyle.Bold);

                    Font bodyFont =
                        new Font("Segoe UI", 10F);

                    int left = 80;
                    int top = 40;

                    e.Graphics.DrawString(
                        "WILDCATHUB",
                        titleFont,
                        Brushes.Maroon,
                        left,
                        top);

                    e.Graphics.DrawString(
                        "LABORATORY PAYMENT SLIP",
                        subFont,
                        Brushes.DarkGoldenrod,
                        left,
                        top + 40);

                    e.Graphics.DrawLine(
                        Pens.Maroon,
                        left,
                        top + 70,
                        700,
                        top + 70);

                    Rectangle border =
                        new Rectangle(60, 30, 680, 900);

                    e.Graphics.DrawRectangle(
                        Pens.Gray,
                        border);

                    e.Graphics.DrawString(
                        slipText,
                        bodyFont,
                        Brushes.Black,
                        new RectangleF(80, 130, 620, 760));
                };

                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = printDoc;
                preview.Width = 900;
                preview.Height = 700;
                preview.StartPosition = FormStartPosition.CenterScreen;

                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error printing payment slip:\n" + ex.Message,
                    "Print",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private void frmUserDashboard_Load(object sender, EventArgs e)
        {
            ApplyRoundedUi();

            btnCatTechnical.Text = "Trainers";
            btnCatScience.Text = "Testers";
            btnCatSports.Text = "Electronic Components";
            btnCatGeneral.Visible = false;
            btnCatAll.Visible = false;
            btnCatTechnical.Visible = false;
            btnCatScience.Visible = false;
            btnCatSports.Visible = false;
            btnCatGeneral.Visible = false;

            ConfigureHistoryGrid();
            LoadUserProfileInfo();
            LoadEquipmentCards(currentEquipmentCategory);

            RestoreReminderStyle();
        }


        private void ApplyButtonStyle(Button btn)
        {
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
        }

        private void ApplyRoundedUi()
        {
            RoundControl(cardBorrowed, 28);
            RoundControl(cardDueSoon, 28);
            RoundControl(cardOverdue, 28);
            RoundControl(cardHistory, 28);

            RoundControl(pnlWelcome, 28);
            RoundControl(pnlReminders, 28);
            RoundControl(pnlStatistics, 28);

            RoundControl(pnlEquipmentHeader, 26);
            RoundControl(btnCatAll, 18);
            RoundControl(btnCatTechnical, 18);
            RoundControl(btnCatScience, 18);
            RoundControl(btnCatSports, 18);
            RoundControl(btnCatGeneral, 18);

            RoundControl(pnlBorrowedHeader, 26);
            RoundControl(pnlBorrowedEmptyState, 28);
            RoundControl(pnlHistoryCard, 28);
            RoundControl(pnlProfileCard, 28);
            RoundControl(pnlBorrowedPopup, 24);

            RoundControl(pnlStatistics, 24);
            RoundControl(pnlReminders, 24);
        }



        private void RoundControl(Control control, int radius = 28)
        {
            if (control.Width <= 0 || control.Height <= 0)
                return;

            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d - 1, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d - 1, rect.Bottom - d - 1, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d - 1, d, d, 90, 90);
            path.CloseFigure();

            control.Region = new Region(path);
        }



        private void SetActiveCategoryButton(Button activeButton)
        {
            Button[] buttons = { btnCatAll, btnCatTechnical, btnCatScience, btnCatSports, btnCatGeneral };

            foreach (Button btn in buttons)
            {
                btn.BackColor = Color.FromArgb(241, 233, 245);
                btn.ForeColor = Color.FromArgb(87, 60, 99);
                btn.Invalidate();
            }

            activeButton.BackColor = Color.FromArgb(169, 215, 159);
            activeButton.ForeColor = Color.White;
            activeButton.Invalidate();

            txtUserEquipmentSearch.Clear();
        }




        private void ApplyDashboardNeumorphism(Panel panel, int radius = 28)
        {
            if (styledUserPanels.Contains(panel))
                return;

            styledUserPanels.Add(panel);

            panel.Paint += (s, e) =>
            {
                if (s is not Panel p) return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                using GraphicsPath path = GetDashboardRoundedPath(rect, radius);

                p.Region = new Region(path);

                Color baseColor = p.BackColor;
                Color lightEdge = Color.FromArgb(255, 255, 255);
                Color darkEdge = Color.FromArgb(200, 185, 205);

                using SolidBrush brush = new SolidBrush(baseColor);
                e.Graphics.FillPath(brush, path);

                using Pen lightPen = new Pen(lightEdge, 3);
                using Pen darkPen = new Pen(darkEdge, 3);

                // TOP-LEFT LIGHT
                e.Graphics.DrawArc(lightPen, 1, 1, 28, 28, 180, 90);
                e.Graphics.DrawLine(lightPen, 15, 1, rect.Width - 15, 1);
                e.Graphics.DrawLine(lightPen, 1, 15, 1, rect.Height - 15);

                // BOTTOM-RIGHT SHADOW
                e.Graphics.DrawArc(darkPen, rect.Width - 29, rect.Height - 29, 28, 28, 0, 90);
                e.Graphics.DrawLine(darkPen, 15, rect.Height - 1, rect.Width - 15, rect.Height - 1);
                e.Graphics.DrawLine(darkPen, rect.Width - 1, 15, rect.Width - 1, rect.Height - 15);
            };
        }



        private GraphicsPath GetDashboardRoundedPath(Rectangle rect, int radius)
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


        private void LoadUserProfileInfo()
        {
            lblProfileNameValue.Text = string.IsNullOrWhiteSpace(SessionManager.FullName)
                ? "Student Full Name"
                : SessionManager.FullName;

            lblProfileSchoolIdValue.Text = string.IsNullOrWhiteSpace(SessionManager.SchoolID)
                ? "24-0000-000"
                : SessionManager.SchoolID;

            lblProfileEmailValue.Text = string.IsNullOrWhiteSpace(SessionManager.SchoolEmail)
                ? "student@email.edu.ph"
                : SessionManager.SchoolEmail;

            lblProfileStatusValue.Text = string.IsNullOrWhiteSpace(SessionManager.VerificationStatus)
                ? "Verified"
                : SessionManager.VerificationStatus;

            string firstName = "Student";
            if (!string.IsNullOrWhiteSpace(SessionManager.FullName))
            {
                string[] parts = SessionManager.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                    firstName = parts[0];
            }

            lblWelcomeTitle.Text = $"Welcome, {firstName}!";
        }

        private void ShowBorrowedEmptyState()
        {
            flowBorrowedItems.Controls.Clear();
            flowBorrowedItems.Visible = false;
            pnlBorrowedPopup.Visible = false;
            pnlBorrowedEmptyState.Visible = true;
        }

        private void ShowDashboardPanel()
        {
            panelDashboard.Visible = true;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelHistory.Visible = false;
            panelProfile.Visible = false;

            lblPageTitle.Text = "Dashboard";

            ResetSidebarButtons();
            SetActiveSidebarButton(btnNavDashboard);

            LoadUserDashboardData();
        }

        private void ShowEquipmentPanel()
        {
            panelDashboard.Visible = false;
            panelEquipment.Visible = true;
            panelBorrowed.Visible = false;
            panelHistory.Visible = false;
            panelProfile.Visible = false;
            pnlBorrowedPopup.Visible = false;

            lblPageTitle.Text = "Equipment";

            ResetSidebarButtons();
            SetActiveSidebarButton(btnNavEquipment);

            LoadEquipmentCards(currentEquipmentCategory, txtUserEquipmentSearch.Text.Trim());
        }






        private void LoadBorrowedItems()
        {
            try
            {
                flowBorrowedItems.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BSI.SlipItemID,
    E.EquipmentName,
    BSI.QuantityRequested,
    BS.DateCreated,
    BSI.ItemReturnStatus
FROM ((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID)
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
ORDER BY BS.DateCreated DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
                cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasItems = false;

                while (reader != null && reader.Read())
                {
                    hasItems = true;

                    int slipItemId = Convert.ToInt32(reader["SlipItemID"]);
                    string equipmentName = reader["EquipmentName"]?.ToString() ?? "";

                    int quantity = reader["QuantityRequested"] != DBNull.Value
                        ? Convert.ToInt32(reader["QuantityRequested"])
                        : 0;

                    DateTime borrowDate = reader["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateCreated"])
                        : DateTime.Now;

                    string returnStatus = reader["ItemReturnStatus"]?.ToString() ?? "Borrowed";
                    string serialNumbers = GetBorrowedSerialNumbers(slipItemId);

                    Panel card = new Panel
                    {
                        Width = 850,
                        Height = 120,
                        BackColor = Color.FromArgb(255, 251, 252),
                        Margin = new Padding(0, 0, 0, 12)
                    };

                    RoundControl(card, 18);

                    Label lblName = new Label
                    {
                        Text = equipmentName,
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Location = new Point(20, 18),
                        Width = 500,
                        Height = 24
                    };

                    Label lblQty = new Label
                    {
                        Text = "Quantity: " + quantity,
                        Font = new Font("Segoe UI", 9.5F),
                        ForeColor = Color.FromArgb(126, 105, 136),
                        Location = new Point(20, 48),
                        Width = 200
                    };

                    Label lblDate = new Label
                    {
                        Text = "Borrowed: " + borrowDate.ToString("MMM dd, yyyy"),
                        Font = new Font("Segoe UI", 9.5F),
                        ForeColor = Color.FromArgb(126, 105, 136),
                        Location = new Point(230, 48),
                        Width = 250
                    };

                    Label lblSerials = new Label
                    {
                        Text = "Serial No.: " + serialNumbers,
                        Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(92, 45, 58),
                        Location = new Point(20, 74),
                        Width = 760,
                        Height = 20
                    };

                    Label lblStatus = new Label
                    {
                        Text = returnStatus,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                        ForeColor = Color.DarkOrange,
                        AutoSize = true,
                        Location = new Point(720, 20)
                    };

                    card.Controls.Add(lblName);
                    card.Controls.Add(lblQty);
                    card.Controls.Add(lblDate);
                    card.Controls.Add(lblSerials);
                    card.Controls.Add(lblStatus);

                    flowBorrowedItems.Controls.Add(card);
                }

                flowBorrowedItems.Visible = true;
                pnlBorrowedEmptyState.Visible = !hasItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowed items:\n" + ex.Message);
            }
        }




        private Panel CreateBorrowedItemCard(
    int borrowId,
    string equipmentName,
    string imagePath,
    int quantityBorrowed,
    DateTime borrowDate,
    DateTime dueDate,
    decimal penaltyAmount)
        {
            Panel card = new Panel
            {
                BackColor = Color.FromArgb(255, 251, 252),
                Width = 230,
                Height = 255,
                Margin = new Padding(12),
                Cursor = Cursors.Hand,
                Tag = borrowId
            };

            RoundControl(card, 24);
            ApplyEquipmentCardStyle(card);

            Panel imageHolder = new Panel
            {
                Location = new Point(18, 18),
                Size = new Size(194, 160),
                BackColor = Color.FromArgb(243, 236, 245)
            };
            RoundControl(imageHolder, 18);

            PictureBox pic = new PictureBox
            {
                Location = new Point(12, 10),
                Size = new Size(170, 140),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    using Image temp = Image.FromStream(fs);
                    pic.Image = new Bitmap(temp);
                }
                catch
                {
                    pic.Image = null;
                }
            }

            Label lblName = new Label
            {
                Text = equipmentName,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(18, 188),
                AutoSize = false,
                Width = 194,
                Height = 24,
                BackColor = Color.Transparent
            };

            Label lblQty = new Label
            {
                Text = $"Qty: {quantityBorrowed}",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(18, 214),
                AutoSize = false,
                Width = 194,
                Height = 20,
                BackColor = Color.Transparent
            };

            Label lblHint = new Label
            {
                Text = "Tap to view details",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 135, 160),
                Location = new Point(18, 233),
                AutoSize = false,
                Width = 194,
                Height = 16,
                BackColor = Color.Transparent
            };

            void openPopup(object? s, EventArgs e)
            {
                ShowBorrowedPopup(borrowId, equipmentName, quantityBorrowed, borrowDate, dueDate, penaltyAmount);
            }

            card.Click += openPopup;
            imageHolder.Click += openPopup;
            pic.Click += openPopup;
            lblName.Click += openPopup;
            lblQty.Click += openPopup;
            lblHint.Click += openPopup;

            imageHolder.Controls.Add(pic);

            card.Controls.Add(imageHolder);
            card.Controls.Add(lblName);
            card.Controls.Add(lblQty);
            card.Controls.Add(lblHint);

            return card;
        }





        private void SaveReservation(
    int equipmentId,
    int quantityReserved,
    DateTime reservationDate)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                int totalQty = 0;
                int maintenanceQty = 0;
                bool hasSerial = false;
                string equipmentType = "Reusable";
                string equipmentName = "";

                string equipmentQuery = @"
SELECT
    EquipmentName,
    QuantityTotal,
    QuantityMaintenance,
    HasSerial,
    EquipmentType
FROM Equipment
WHERE EquipmentID = ?
AND IsArchived = False
AND Status = 'Active'";

                using (var equipmentCmd = new OleDbCommand(equipmentQuery, conn))
                {
                    equipmentCmd.Parameters.Add("@p1", OleDbType.Integer).Value = equipmentId;

                    using var reader = equipmentCmd.ExecuteReader();

                    if (reader != null && reader.Read())
                    {
                        equipmentName = reader["EquipmentName"]?.ToString() ?? "";

                        totalQty = reader["QuantityTotal"] != DBNull.Value
                            ? Convert.ToInt32(reader["QuantityTotal"])
                            : 0;

                        maintenanceQty = reader["QuantityMaintenance"] != DBNull.Value
                            ? Convert.ToInt32(reader["QuantityMaintenance"])
                            : 0;

                        hasSerial = reader["HasSerial"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["HasSerial"]);

                        equipmentType = reader["EquipmentType"] != DBNull.Value
                            ? reader["EquipmentType"].ToString()
                            : "Reusable";
                    }
                    else
                    {
                        MessageBox.Show("Equipment not found.");
                        return;
                    }
                }

                int currentAvailable = GetCorrectAvailableQuantity(
                    conn,
                    equipmentId,
                    totalQty,
                    maintenanceQty,
                    hasSerial,
                    equipmentType);

                int alreadyInCart = borrowCart
                    .Where(x => x.EquipmentID == equipmentId)
                    .Sum(x => x.Quantity);

                if (currentAvailable < alreadyInCart + quantityReserved)
                {
                    MessageBox.Show(
                        "Not enough available quantity. Only " + currentAvailable + " available.",
                        "Borrow Cart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                BorrowCartItem existingItem =
                    borrowCart.FirstOrDefault(x => x.EquipmentID == equipmentId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantityReserved;
                }
                else
                {
                    borrowCart.Add(new BorrowCartItem
                    {
                        EquipmentID = equipmentId,
                        EquipmentName = equipmentName,
                        Quantity = quantityReserved
                    });
                }

                UpdateBorrowCartButtonText();

                MessageBox.Show(
                    "Added to borrow cart. Open the cart to submit your borrow slip.",
                    "Borrow Cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding to borrow cart:\n" + ex.Message,
                    "Borrow Cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private void ShowBorrowCartDialog()
        {
            if (borrowCart.Count == 0)
            {
                MessageBox.Show("Your borrow cart is empty.");
                return;
            }

            Form cartForm = new Form();
            cartForm.Text = "Borrower's Slip";
            cartForm.Size = new Size(620, 690);
            cartForm.StartPosition = FormStartPosition.CenterParent;
            cartForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            cartForm.MaximizeBox = false;
            cartForm.MinimizeBox = false;
            cartForm.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label();
            lblTitle.Text = "Borrower's Slip";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblTitle.Location = new Point(28, 22);
            lblTitle.AutoSize = true;

            FlowLayoutPanel flowItems = new FlowLayoutPanel();
            flowItems.Location = new Point(30, 65);
            flowItems.Size = new Size(540, 165);
            flowItems.AutoScroll = true;
            flowItems.FlowDirection = FlowDirection.TopDown;
            flowItems.WrapContents = false;
            flowItems.BackColor = Color.White;
            flowItems.BorderStyle = BorderStyle.FixedSingle;

            void RefreshCartItems()
            {
                flowItems.Controls.Clear();

                foreach (BorrowCartItem item in borrowCart.ToList())
                {
                    Panel row = new Panel();
                    row.Size = new Size(510, 46);
                    row.BackColor = Color.FromArgb(255, 251, 252);
                    row.Margin = new Padding(8, 6, 8, 0);

                    Label lblItem = new Label();
                    lblItem.Text = item.EquipmentName;
                    lblItem.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    lblItem.ForeColor = Color.FromArgb(72, 53, 84);
                    lblItem.Location = new Point(10, 11);
                    lblItem.Size = new Size(245, 22);

                    Label lblQty = new Label();
                    lblQty.Text = "Qty:";
                    lblQty.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    lblQty.ForeColor = Color.FromArgb(72, 53, 84);
                    lblQty.Location = new Point(265, 12);
                    lblQty.Size = new Size(34, 20);

                    NumericUpDown numQty = new NumericUpDown();
                    numQty.Minimum = 1;
                    numQty.Maximum = Math.Max(1, GetCartEditableAvailableQuantity(item.EquipmentID));
                    numQty.Value = Math.Min(item.Quantity, (int)numQty.Maximum);
                    numQty.Location = new Point(302, 9);
                    numQty.Size = new Size(58, 24);
                    numQty.Font = new Font("Segoe UI", 9F);
                    numQty.Tag = item;
                    numQty.ValueChanged += (s, e) =>
                    {
                        item.Quantity = (int)numQty.Value;
                        UpdateBorrowCartButtonText();
                    };

                    Button btnRemove = new Button();
                    btnRemove.Text = "Remove";
                    btnRemove.Size = new Size(80, 28);
                    btnRemove.Location = new Point(415, 8);
                    btnRemove.BackColor = Color.FromArgb(180, 60, 70);
                    btnRemove.ForeColor = Color.White;
                    btnRemove.FlatStyle = FlatStyle.Flat;
                    btnRemove.FlatAppearance.BorderSize = 0;

                    btnRemove.Click += (s, e) =>
                    {
                        borrowCart.Remove(item);
                        UpdateBorrowCartButtonText();
                        RefreshCartItems();

                        if (borrowCart.Count == 0)
                            cartForm.Close();
                    };

                    row.Controls.Add(lblItem);
                    row.Controls.Add(lblQty);
                    row.Controls.Add(numQty);
                    row.Controls.Add(btnRemove);
                    flowItems.Controls.Add(row);
                }
            }

            RefreshCartItems();

            Label lblGroup = new Label();
            lblGroup.Text = "Group #";
            lblGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblGroup.Location = new Point(30, 265);
            lblGroup.AutoSize = true;

            TextBox txtGroupNumber = new TextBox();
            txtGroupNumber.Font = new Font("Segoe UI", 10F);
            txtGroupNumber.Location = new Point(30, 290);
            txtGroupNumber.Size = new Size(160, 28);
            txtGroupNumber.Text = editingPendingGroupNumber;

            Label lblLeader = new Label();
            lblLeader.Text = "Leader";
            lblLeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLeader.Location = new Point(220, 265);
            lblLeader.AutoSize = true;

            TextBox txtLeader = new TextBox();
            txtLeader.Font = new Font("Segoe UI", 10F);
            txtLeader.Location = new Point(220, 290);
            txtLeader.Size = new Size(350, 28);
            txtLeader.Text = SessionManager.FullName;
            txtLeader.ReadOnly = true;

            Label lblDateBorrowed = new Label();
            lblDateBorrowed.Text = "Date Borrowed: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            lblDateBorrowed.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblDateBorrowed.ForeColor = Color.FromArgb(72, 53, 84);
            lblDateBorrowed.Location = new Point(30, 332);
            lblDateBorrowed.Size = new Size(540, 24);

            Label lblMember = new Label();
            lblMember.Text = "Add Member School ID";
            lblMember.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMember.Location = new Point(30, 365);
            lblMember.AutoSize = true;

            TextBox txtMemberSchoolId = new TextBox();
            txtMemberSchoolId.Font = new Font("Segoe UI", 10F);
            txtMemberSchoolId.Location = new Point(30, 390);
            txtMemberSchoolId.Size = new Size(250, 28);

            Button btnAddMember = new Button();
            btnAddMember.Text = "+ Add";
            btnAddMember.Size = new Size(90, 30);
            btnAddMember.Location = new Point(295, 388);
            btnAddMember.BackColor = Color.FromArgb(212, 168, 45);
            btnAddMember.ForeColor = Color.White;
            btnAddMember.FlatStyle = FlatStyle.Flat;
            btnAddMember.FlatAppearance.BorderSize = 0;

            FlowLayoutPanel flowMembers = new FlowLayoutPanel();
            flowMembers.Location = new Point(30, 432);
            flowMembers.Size = new Size(540, 105);
            flowMembers.AutoScroll = true;
            flowMembers.FlowDirection = FlowDirection.TopDown;
            flowMembers.WrapContents = false;
            flowMembers.BackColor = Color.White;
            flowMembers.BorderStyle = BorderStyle.FixedSingle;

            List<MemberEntry> members = editingPendingMembers
                .Select(x => new MemberEntry { UserID = x.UserID, FullName = x.FullName, SchoolID = x.SchoolID })
                .ToList();

            void RefreshMembers()
            {
                flowMembers.Controls.Clear();

                foreach (MemberEntry member in members)
                {
                    CheckBox chk = new CheckBox
                    {
                        Text = member.DisplayText,
                        Tag = member.UserID,
                        Font = new Font("Segoe UI", 9.5F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Size = new Size(500, 26),
                        Margin = new Padding(8, 4, 8, 0)
                    };

                    flowMembers.Controls.Add(chk);
                }
            }

            btnAddMember.Click += (s, e) =>
            {
                string schoolId = txtMemberSchoolId.Text.Trim();

                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    MessageBox.Show("Enter member School ID first.");
                    return;
                }

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                CurrentLabContext? labContext = GetCurrentLabContext(conn);
                if (labContext == null)
                {
                    MessageBox.Show("You can only add members during your active laboratory schedule.");
                    return;
                }

                string query = @"
SELECT U.UserID, U.FullName
FROM (Users AS U
INNER JOIN StudentSubjectEnrollments AS SSE ON U.UserID = SSE.UserID)
WHERE U.SchoolID = ?
AND U.IsActive = True
AND SSE.IsActive = True
AND SSE.SubjectID = ?
AND SSE.ScheduleID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = schoolId;
                cmd.Parameters.Add("@p2", OleDbType.Integer).Value = labContext.SubjectID;
                cmd.Parameters.Add("@p3", OleDbType.Integer).Value = labContext.ScheduleID;

                using OleDbDataReader reader = cmd.ExecuteReader();

                if (reader == null || !reader.Read())
                {
                    MessageBox.Show("Member is not enrolled in this same subject schedule.");
                    return;
                }

                int memberId = Convert.ToInt32(reader["UserID"]);
                string memberName = reader["FullName"]?.ToString() ?? "";

                if (memberId == SessionManager.UserID || members.Any(x => x.UserID == memberId))
                {
                    MessageBox.Show("Member already added.");
                    return;
                }

                members.Add(new MemberEntry
                {
                    UserID = memberId,
                    FullName = memberName,
                    SchoolID = schoolId
                });

                RefreshMembers();
                txtMemberSchoolId.Clear();
            };

            Button btnDeleteMember = new Button();
            btnDeleteMember.Text = "Delete";
            btnDeleteMember.Size = new Size(90, 30);
            btnDeleteMember.Location = new Point(480, 388);
            btnDeleteMember.BackColor = Color.FromArgb(180, 60, 70);
            btnDeleteMember.ForeColor = Color.White;
            btnDeleteMember.FlatStyle = FlatStyle.Flat;
            btnDeleteMember.FlatAppearance.BorderSize = 0;
            btnDeleteMember.Click += (s, e) =>
            {
                List<int> selectedIds = flowMembers.Controls
                    .OfType<CheckBox>()
                    .Where(x => x.Checked && x.Tag is int)
                    .Select(x => (int)x.Tag)
                    .ToList();

                if (selectedIds.Count == 0)
                {
                    MessageBox.Show("Check a member first.");
                    return;
                }

                members.RemoveAll(x => selectedIds.Contains(x.UserID));
                RefreshMembers();
            };

            Button btnSubmit = new Button();
            btnSubmit.Text = editingPendingSlipId > 0 ? "Save Slip" : "Submit Borrow Slip";
            btnSubmit.Size = new Size(170, 38);
            btnSubmit.Location = new Point(400, 575);
            btnSubmit.BackColor = Color.FromArgb(128, 0, 0);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            btnSubmit.Click += (s, e) =>
            {
                bool saved = SubmitBorrowSlipFromCart(
                    txtGroupNumber.Text.Trim(),
                    txtLeader.Text.Trim(),
                    members);

                if (saved)
                    cartForm.Close();
            };

            Button btnDeleteSlip = new Button();
            btnDeleteSlip.Text = "Delete Slip";
            btnDeleteSlip.Size = new Size(130, 38);
            btnDeleteSlip.Location = new Point(30, 575);
            btnDeleteSlip.BackColor = Color.FromArgb(180, 60, 70);
            btnDeleteSlip.ForeColor = Color.White;
            btnDeleteSlip.FlatStyle = FlatStyle.Flat;
            btnDeleteSlip.FlatAppearance.BorderSize = 0;
            btnDeleteSlip.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnDeleteSlip.Visible = editingPendingSlipId > 0;
            btnDeleteSlip.Click += (s, e) =>
            {
                if (DeletePendingBorrowSlip(editingPendingSlipId))
                    cartForm.Close();
            };

            RefreshMembers();

            cartForm.Controls.Add(lblTitle);
            cartForm.Controls.Add(flowItems);
            cartForm.Controls.Add(lblGroup);
            cartForm.Controls.Add(txtGroupNumber);
            cartForm.Controls.Add(lblLeader);
            cartForm.Controls.Add(txtLeader);
            cartForm.Controls.Add(lblDateBorrowed);
            cartForm.Controls.Add(lblMember);
            cartForm.Controls.Add(txtMemberSchoolId);
            cartForm.Controls.Add(btnAddMember);
            cartForm.Controls.Add(btnDeleteMember);
            cartForm.Controls.Add(flowMembers);
            cartForm.Controls.Add(btnDeleteSlip);
            cartForm.Controls.Add(btnSubmit);

            cartForm.ShowDialog(this);
        }

        private int GetCartEditableAvailableQuantity(int equipmentId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string equipmentQuery = @"
SELECT QuantityTotal, QuantityMaintenance, HasSerial, EquipmentType
FROM Equipment
WHERE EquipmentID = ?
AND IsArchived = False
AND Status = 'Active'";

                using OleDbCommand cmd = new OleDbCommand(equipmentQuery, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = equipmentId;

                using OleDbDataReader reader = cmd.ExecuteReader();
                if (reader == null || !reader.Read())
                    return 1;

                int totalQty = reader["QuantityTotal"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityTotal"])
                    : 0;
                int maintenanceQty = reader["QuantityMaintenance"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityMaintenance"])
                    : 0;
                bool hasSerial = reader["HasSerial"] != DBNull.Value &&
                                 Convert.ToBoolean(reader["HasSerial"]);
                string equipmentType = reader["EquipmentType"] != DBNull.Value
                    ? reader["EquipmentType"].ToString() ?? "Reusable"
                    : "Reusable";

                return Math.Max(1, GetCorrectAvailableQuantity(
                    conn,
                    equipmentId,
                    totalQty,
                    maintenanceQty,
                    hasSerial,
                    equipmentType));
            }
            catch
            {
                return 1;
            }
        }

        private bool DeletePendingBorrowSlip(int slipId)
        {
            if (slipId <= 0)
                return false;

            if (MessageBox.Show("Delete this pending borrower slip?", "Borrow Slip",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();
                using OleDbTransaction trans = conn.BeginTransaction();

                using (OleDbCommand cmd = new OleDbCommand("DELETE FROM BorrowSlipMembers WHERE SlipID = ?", conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    cmd.ExecuteNonQuery();
                }

                using (OleDbCommand cmd = new OleDbCommand("DELETE FROM BorrowSlipItems WHERE SlipID = ?", conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    cmd.ExecuteNonQuery();
                }

                using (OleDbCommand cmd = new OleDbCommand("DELETE FROM BorrowSlips WHERE SlipID = ? AND UserID = ? AND SlipStatus = 'Pending'", conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
                borrowCart.Clear();
                editingPendingSlipId = 0;
                editingPendingGroupNumber = "";
                editingPendingMembers.Clear();
                UpdateBorrowCartButtonText();
                MessageBox.Show("Pending borrower slip deleted.", "Borrow Slip",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting pending slip:\n" + ex.Message, "Borrow Slip",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ShowPendingBorrowerSlipsDialog()
        {
            try
            {
                Form slipForm = new Form
                {
                    Text = "Pending Borrower's Slips",
                    Size = new Size(620, 420),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(250, 245, 247)
                };

                Label lblTitle = new Label
                {
                    Text = "Pending Borrower's Slips",
                    Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(24, 22),
                    AutoSize = true
                };

                FlowLayoutPanel flow = new FlowLayoutPanel
                {
                    Location = new Point(24, 68),
                    Size = new Size(555, 285),
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    BackColor = Color.White
                };

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    BS.DateCreated,
    LS.SubjectCode,
    SS.Section
FROM ((BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
WHERE BS.UserID = ?
AND BS.SlipStatus = 'Pending'
ORDER BY BS.DateCreated DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;

                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    int slipId = Convert.ToInt32(reader["SlipID"]);
                    string groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    string subject = reader["SubjectCode"]?.ToString() ?? "";
                    string section = reader["Section"]?.ToString() ?? "";
                    DateTime dateCreated = reader["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateCreated"])
                        : DateTime.Now;

                    Panel row = new Panel
                    {
                        Width = 520,
                        Height = 78,
                        BackColor = Color.FromArgb(255, 251, 252),
                        Margin = new Padding(10, 8, 10, 0)
                    };
                    RoundControl(row, 14);

                    Label lblInfo = new Label
                    {
                        Text = "Group " + groupNumber + "  |  " + subject + "  " + section + "\n" +
                               dateCreated.ToString("MM/dd/yyyy hh:mm tt"),
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Location = new Point(14, 14),
                        Size = new Size(330, 48)
                    };

                    Button btnEdit = new Button
                    {
                        Text = "Edit",
                        Size = new Size(105, 34),
                        Location = new Point(390, 22),
                        BackColor = Color.FromArgb(212, 168, 45),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                        Tag = slipId
                    };
                    btnEdit.FlatAppearance.BorderSize = 0;
                    btnEdit.Click += (s, e) =>
                    {
                        LoadPendingSlipIntoCart((int)((Button)s!).Tag);
                        slipForm.Close();
                        ShowBorrowCartDialog();
                    };

                    row.Controls.Add(lblInfo);
                    row.Controls.Add(btnEdit);
                    flow.Controls.Add(row);
                }

                if (flow.Controls.Count == 0)
                {
                    Label lblEmpty = new Label
                    {
                        Text = "No pending borrower slips.",
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(126, 105, 136),
                        Size = new Size(520, 70),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    flow.Controls.Add(lblEmpty);
                }

                slipForm.Controls.Add(lblTitle);
                slipForm.Controls.Add(flow);
                slipForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending slips:\n" + ex.Message);
            }
        }

        private void LoadPendingSlipIntoCart(int slipId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string slipQuery = "SELECT GroupNumber FROM BorrowSlips WHERE SlipID = ? AND UserID = ? AND SlipStatus = 'Pending'";
            using (OleDbCommand cmd = new OleDbCommand(slipQuery, conn))
            {
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    MessageBox.Show("This slip can no longer be edited.");
                    return;
                }

                editingPendingGroupNumber = result.ToString() ?? "";
            }

            borrowCart.Clear();
            editingPendingMembers.Clear();
            editingPendingSlipId = slipId;

            string itemQuery = @"
SELECT BSI.EquipmentID, E.EquipmentName, BSI.QuantityRequested
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?
ORDER BY E.EquipmentName";

            using (OleDbCommand itemCmd = new OleDbCommand(itemQuery, conn))
            {
                itemCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                using OleDbDataReader reader = itemCmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    borrowCart.Add(new BorrowCartItem
                    {
                        EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                        EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                        Quantity = reader["QuantityRequested"] != DBNull.Value ? Convert.ToInt32(reader["QuantityRequested"]) : 1
                    });
                }
            }

            string memberQuery = @"
SELECT BSM.UserID, BSM.MemberName, U.SchoolID
FROM BorrowSlipMembers AS BSM
LEFT JOIN Users AS U ON BSM.UserID = U.UserID
WHERE BSM.SlipID = ?
ORDER BY BSM.MemberName";

            using (OleDbCommand memberCmd = new OleDbCommand(memberQuery, conn))
            {
                memberCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                using OleDbDataReader reader = memberCmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    editingPendingMembers.Add(new MemberEntry
                    {
                        UserID = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0,
                        FullName = reader["MemberName"]?.ToString() ?? "",
                        SchoolID = reader["SchoolID"]?.ToString() ?? ""
                    });
                }
            }

            UpdateBorrowCartButtonText();
        }




        private bool SubmitBorrowSlipFromCart(
    string groupNumber,
    string leaderName,
    List<MemberEntry> members)
        {
            if (borrowCart.Count == 0)
            {
                MessageBox.Show("Your borrow cart is empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(groupNumber))
            {
                MessageBox.Show("Please enter your group number.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(leaderName))
            {
                MessageBox.Show("Please enter the leader name.");
                return false;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                CurrentLabContext? labContext = GetCurrentLabContext(conn);
                if (labContext == null)
                {
                    MessageBox.Show("You do not have a laboratory schedule right now.");
                    return false;
                }

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {

                    int newSlipId = editingPendingSlipId;

                    if (editingPendingSlipId > 0)
                    {
                        string updateSlipQuery = @"
UPDATE BorrowSlips
SET GroupNumber = ?, LeaderName = ?, DateCreated = ?
WHERE SlipID = ? AND UserID = ? AND SlipStatus = 'Pending'";

                        using OleDbCommand updateCmd = new OleDbCommand(updateSlipQuery, conn, trans);
                        updateCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = groupNumber;
                        updateCmd.Parameters.Add("@p2", OleDbType.VarWChar).Value = leaderName;
                        updateCmd.Parameters.Add("@p3", OleDbType.Date).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@p4", OleDbType.Integer).Value = editingPendingSlipId;
                        updateCmd.Parameters.Add("@p5", OleDbType.Integer).Value = SessionManager.UserID;

                        if (updateCmd.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("This slip can no longer be edited.");
                            trans.Rollback();
                            return false;
                        }

                        using OleDbCommand deleteItemsCmd = new OleDbCommand("DELETE FROM BorrowSlipItems WHERE SlipID = ?", conn, trans);
                        deleteItemsCmd.Parameters.Add("@p1", OleDbType.Integer).Value = editingPendingSlipId;
                        deleteItemsCmd.ExecuteNonQuery();

                        using OleDbCommand deleteMembersCmd = new OleDbCommand("DELETE FROM BorrowSlipMembers WHERE SlipID = ?", conn, trans);
                        deleteMembersCmd.Parameters.Add("@p1", OleDbType.Integer).Value = editingPendingSlipId;
                        deleteMembersCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertSlipQuery = @"
INSERT INTO BorrowSlips
(UserID, SubjectID, ScheduleID, GroupNumber, LeaderName, SlipType, DateCreated, SlipStatus)
VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                        using (OleDbCommand cmd = new OleDbCommand(insertSlipQuery, conn, trans))
                        {
                            cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
                            cmd.Parameters.Add("@p2", OleDbType.Integer).Value = labContext.SubjectID;
                            cmd.Parameters.Add("@p3", OleDbType.Integer).Value = labContext.ScheduleID;
                            cmd.Parameters.Add("@p4", OleDbType.VarWChar).Value = groupNumber;
                            cmd.Parameters.Add("@p5", OleDbType.VarWChar).Value = leaderName;
                            cmd.Parameters.Add("@p6", OleDbType.VarWChar).Value = "Group";
                            cmd.Parameters.Add("@p7", OleDbType.Date).Value = DateTime.Now;
                            cmd.Parameters.Add("@p8", OleDbType.VarWChar).Value = "Pending";

                            cmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand idCmd = new OleDbCommand("SELECT @@IDENTITY", conn, trans))
                        {
                            newSlipId = Convert.ToInt32(idCmd.ExecuteScalar());
                        }
                    }

                    foreach (BorrowCartItem item in borrowCart)
                    {
                        string insertItemQuery = @"
INSERT INTO BorrowSlipItems
(SlipID, EquipmentID, QuantityRequested, ItemReturnStatus)
VALUES (?, ?, ?, ?)";

                        using OleDbCommand itemCmd = new OleDbCommand(insertItemQuery, conn, trans);
                        itemCmd.Parameters.Add("@p1", OleDbType.Integer).Value = newSlipId;
                        itemCmd.Parameters.Add("@p2", OleDbType.Integer).Value = item.EquipmentID;
                        itemCmd.Parameters.Add("@p3", OleDbType.Integer).Value = item.Quantity;
                        itemCmd.Parameters.Add("@p4", OleDbType.VarWChar).Value = "Pending";
                        itemCmd.ExecuteNonQuery();
                    }

                    foreach (MemberEntry member in members)
                    {
                        if (string.IsNullOrWhiteSpace(member.FullName))
                            continue;

                        string insertMemberQuery = @"
INSERT INTO BorrowSlipMembers
(SlipID, UserID, MemberName)
VALUES (?, ?, ?)";

                        using OleDbCommand memberCmd =
                            new OleDbCommand(insertMemberQuery, conn, trans);

                        memberCmd.Parameters.Add("@p1", OleDbType.Integer).Value = newSlipId;
                        memberCmd.Parameters.Add("@p2", OleDbType.Integer).Value = member.UserID;
                        memberCmd.Parameters.Add("@p3", OleDbType.VarWChar).Value = member.FullName;

                        memberCmd.ExecuteNonQuery();
                    }

                    trans.Commit();

                    borrowCart.Clear();
                    editingPendingSlipId = 0;
                    editingPendingGroupNumber = "";
                    editingPendingMembers.Clear();
                    UpdateBorrowCartButtonText();

                    MessageBox.Show(
                        "Borrow slip saved successfully.",
                        "Borrow Slip",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadEquipmentCards(currentEquipmentCategory);
                    LoadUserDashboardData();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error submitting borrow slip:\n" + ex.Message,
                    "Borrow Slip",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }




        private void CancelReservation(int reservationId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbTransaction trans = conn.BeginTransaction();

            try
            {
                string status = "";

                string getQuery = @"
SELECT ReservationStatus
FROM Reservations
WHERE ReservationID = ? AND UserID = ?";

                using (OleDbCommand cmd = new OleDbCommand(getQuery, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@p1", reservationId);
                    cmd.Parameters.AddWithValue("@p2", SessionManager.UserID);

                    using OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.Read())
                    {
                        status = reader["ReservationStatus"]?.ToString() ?? "";
                    }
                    else
                    {
                        MessageBox.Show("Reservation not found.",
                            "Cancel Reservation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Only pending reservations can be cancelled.",
                        "Cancel Reservation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string cancelQuery = @"
UPDATE Reservations
SET ReservationStatus = 'Cancelled'
WHERE ReservationID = ?";

                using (OleDbCommand cmd = new OleDbCommand(cancelQuery, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@p1", reservationId);
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();

                MessageBox.Show("Reservation cancelled successfully.",
                    "Cancel Reservation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadUserDashboardData();
                LoadEquipmentCards(currentEquipmentCategory);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("Error cancelling reservation:\n" + ex.Message,
                    "Cancel Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowHistoryPanel()
        {
            panelDashboard.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelHistory.Visible = true;
            panelProfile.Visible = false;
            pnlBorrowedPopup.Visible = false;

            lblPageTitle.Text = "History";

            ResetSidebarButtons();
            SetActiveSidebarButton(btnNavHistory);

            InitializeHistorySearch();
            LoadUserHistory();
        }

        private int GetComputedAvailable(int total, int maintenance, int borrowed, int reserved)
        {
            int available = total - maintenance - borrowed - reserved;
            return available < 0 ? 0 : available;
        }

        private void ShowProfilePanel()
        {
            panelDashboard.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelHistory.Visible = false;
            panelProfile.Visible = true;
            pnlBorrowedPopup.Visible = false;

            lblPageTitle.Text = "Profile";

            ResetSidebarButtons();
            SetActiveSidebarButton(btnNavProfile);

            LoadUserProfileInfo();
        }




        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            ShowDashboardPanel();
        }

        private void btnNavEquipment_Click(object sender, EventArgs e)
        {
            ShowEquipmentPanel();
        }

        private void btnNavBorrowed_Click(object sender, EventArgs e)
        {
            ShowBorrowedPanel();
        }

        private void btnNavHistory_Click(object sender, EventArgs e)
        {
            ShowHistoryPanel();
        }

        private void btnNavProfile_Click(object sender, EventArgs e)
        {
            ShowProfilePanel();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
            Close();
        }




        private void BorrowedItem_Click(object sender, EventArgs e)
        {
            // Replaced by dynamic card click logic
        }




        private void btnCloseBorrowedPopup_Click(object sender, EventArgs e)
        {
            pnlBorrowedPopup.Visible = false;
        }




        private void WireEquipmentCategoryEvents()
        {
            btnCatAll.Click += btnCatAll_Click;
            btnCatTechnical.Click += btnCatTechnical_Click;
            btnCatScience.Click += btnCatScience_Click;
            btnCatSports.Click += btnCatSports_Click;
            btnCatGeneral.Click += btnCatGeneral_Click;
            txtUserEquipmentSearch.TextChanged += txtUserEquipmentSearch_TextChanged;
        }



        private void txtUserEquipmentSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEquipmentCards(currentEquipmentCategory, txtUserEquipmentSearch.Text.Trim());
        }


        private void btnCatAll_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "All";
            SetActiveCategoryButton(btnCatAll);
            LoadEquipmentCards("All");
        }


        private void btnCatTechnical_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Technical Laboratory";
            SetActiveCategoryButton(btnCatTechnical);
            LoadEquipmentCards(currentEquipmentCategory);
        }


        private void btnCatScience_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Science Laboratory";
            SetActiveCategoryButton(btnCatScience);
            LoadEquipmentCards(currentEquipmentCategory);
        }


        private void btnCatSports_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Sports Equipment";
            SetActiveCategoryButton(btnCatSports);
            LoadEquipmentCards(currentEquipmentCategory);
        }

        private void btnCatGeneral_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "General Equipment";
            SetActiveCategoryButton(btnCatGeneral);
            LoadEquipmentCards(currentEquipmentCategory);
        }


        private void LoadEquipmentCards(string categoryFilter, string keyword = "")
        {
            flowEquipmentCards.SuspendLayout();
            try
            {
                flowEquipmentCards.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                CurrentLabContext? labContext = GetCurrentLabContext(conn);

                RefreshUserEquipmentCategories(conn, labContext);

                if (labContext == null)
                {
                    ShowNoScheduleEquipmentMessage();
                    return;
                }

                if (HasCurrentLabRestriction(conn, labContext.LabID))
                {
                    ShowRestrictedEquipmentMessage();
                    return;
                }

                string query = @"
SELECT DISTINCT
    E.EquipmentID,
    E.EquipmentName,
    E.Category,
    E.QuantityTotal,
    E.QuantityMaintenance,
    E.Status,
    E.ImagePath,
    E.EquipmentType,
    E.HasSerial
FROM ((Equipment AS E
INNER JOIN SubjectEquipments AS SE ON E.EquipmentID = SE.EquipmentID)
INNER JOIN StudentSubjectEnrollments AS SSE ON SE.SubjectID = SSE.SubjectID)
WHERE SSE.UserID = ?
AND SSE.IsActive = True
AND SE.SubjectID = ?
AND E.IsArchived = False
AND E.Status = 'Active'
AND SSE.ScheduleID = ?";

                if (categoryFilter != "All")
                    query += " AND E.Category = ?";

                if (!string.IsNullOrWhiteSpace(keyword))
                    query += " AND E.EquipmentName LIKE ?";

                query += " ORDER BY E.EquipmentName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                cmd.Parameters.AddWithValue("@p2", labContext.SubjectID);
                cmd.Parameters.AddWithValue("@p3", labContext.ScheduleID);

                if (categoryFilter != "All")
                    cmd.Parameters.AddWithValue("@p4", categoryFilter);

                if (!string.IsNullOrWhiteSpace(keyword))
                    cmd.Parameters.AddWithValue("@p5", "%" + keyword + "%");

                using OleDbDataReader reader = cmd.ExecuteReader();
                RenderEquipmentCards(conn, reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading equipment:\n" + ex.Message,
                    "Equipment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                flowEquipmentCards.ResumeLayout(true);
                flowEquipmentCards.Invalidate();
            }
        }

        private CurrentLabContext? GetCurrentLabContext(OleDbConnection conn)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            string currentDay = DateTime.Now.DayOfWeek.ToString();

            string query = @"
SELECT TOP 1
    LS.SubjectID,
    SSE.ScheduleID,
    LS.LabID,
    LS.SubjectCode,
    SS.Section
FROM ((StudentSubjectEnrollments AS SSE
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID)
INNER JOIN SubjectSchedules AS SS ON SSE.ScheduleID = SS.ScheduleID)
WHERE SSE.UserID = ?
AND SSE.IsActive = True
AND LS.IsActive = True
AND SS.DayOfWeek = ?
AND SS.StartTime <= ?
AND SS.EndTime >= ?
ORDER BY SS.StartTime";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
            cmd.Parameters.AddWithValue("@p2", currentDay);
            cmd.Parameters.AddWithValue("@p3", currentTime);
            cmd.Parameters.AddWithValue("@p4", currentTime);

            using OleDbDataReader reader = cmd.ExecuteReader();
            if (reader == null || !reader.Read())
                return null;

            return new CurrentLabContext
            {
                SubjectID = Convert.ToInt32(reader["SubjectID"]),
                ScheduleID = reader["ScheduleID"] != DBNull.Value ? Convert.ToInt32(reader["ScheduleID"]) : 0,
                LabID = reader["LabID"] != DBNull.Value ? Convert.ToInt32(reader["LabID"]) : 0,
                SubjectCode = reader["SubjectCode"]?.ToString() ?? "",
                Section = reader["Section"]?.ToString() ?? ""
            };
        }

        private void RefreshUserEquipmentCategories(OleDbConnection conn, CurrentLabContext? labContext)
        {
            if (cmbCategoryFilter == null)
                return;

            string previous = cmbCategoryFilter.Text;
            List<string> categories = new List<string>();

            if (labContext != null)
            {
                string query = @"
SELECT DISTINCT E.Category
FROM ((Equipment AS E
INNER JOIN SubjectEquipments AS SE ON E.EquipmentID = SE.EquipmentID)
INNER JOIN StudentSubjectEnrollments AS SSE ON SE.SubjectID = SSE.SubjectID)
WHERE SSE.UserID = ?
AND SSE.IsActive = True
AND SE.SubjectID = ?
AND SSE.ScheduleID = ?
AND E.IsArchived = False
AND E.Status = 'Active'
AND E.Category IS NOT NULL
ORDER BY E.Category";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                cmd.Parameters.AddWithValue("@p2", labContext.SubjectID);
                cmd.Parameters.AddWithValue("@p3", labContext.ScheduleID);

                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    string category = reader["Category"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(category) && !categories.Contains(category))
                        categories.Add(category);
                }
            }

            isRefreshingCategoryFilter = true;
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add("All Categories");
            foreach (string category in categories)
                cmbCategoryFilter.Items.Add(category);

            if (!string.IsNullOrWhiteSpace(previous) && cmbCategoryFilter.Items.Contains(previous))
                cmbCategoryFilter.SelectedItem = previous;
            else
            {
                cmbCategoryFilter.SelectedIndex = 0;
                currentEquipmentCategory = "All";
            }

            isRefreshingCategoryFilter = false;
        }

        private void ShowNoScheduleEquipmentMessage()
        {
            flowEquipmentCards.Controls.Clear();

            Label lblNoSchedule = new Label
            {
                Text = "You do not have a laboratory schedule right now.",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(126, 105, 136),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 90)
            };

            flowEquipmentCards.Controls.Add(lblNoSchedule);
        }


        private void ShowRestrictedEquipmentMessage()
        {
            flowEquipmentCards.Controls.Clear();

            Label lblRestricted = new Label
            {
                Text = "You are Restricted",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(153, 0, 0),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 90)
            };

            flowEquipmentCards.Controls.Add(lblRestricted);
        }


        private bool HasCurrentLabRestriction(OleDbConnection conn, int currentLabId)
        {
            string query = @"
SELECT COUNT(*)
FROM ((DamageReportMembers AS DRM
INNER JOIN DamageReports AS DR ON DRM.ReportID = DR.ReportID)
INNER JOIN LabSubjects AS RestrictedSubject ON DR.SubjectID = RestrictedSubject.SubjectID)
WHERE DRM.UserID = ?
AND DRM.IsRestricted = True
AND DR.ReportStatus = 'For Payment'
AND RestrictedSubject.LabID = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
            cmd.Parameters.AddWithValue("@p2", currentLabId);

            object result = cmd.ExecuteScalar();
            int count = result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;

            return count > 0;
        }


        private void RenderEquipmentCards(OleDbConnection conn, OleDbDataReader reader)
        {
            bool hasCards = false;

            while (reader != null && reader.Read())
            {
                int equipmentId = Convert.ToInt32(reader["EquipmentID"]);
                string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                string category = reader["Category"]?.ToString() ?? "";

                int total = reader["QuantityTotal"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityTotal"])
                    : 0;

                int maintenance = reader["QuantityMaintenance"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityMaintenance"])
                    : 0;

                string status = reader["Status"]?.ToString() ?? "";
                string imagePath = reader["ImagePath"]?.ToString() ?? "";

                string equipmentType = reader["EquipmentType"] != DBNull.Value
                    ? reader["EquipmentType"].ToString()
                    : "Reusable";

                bool hasSerial = reader["HasSerial"] != DBNull.Value &&
                                 Convert.ToBoolean(reader["HasSerial"]);

                int available = GetCorrectAvailableQuantity(
                    conn,
                    equipmentId,
                    total,
                    maintenance,
                    hasSerial,
                    equipmentType);

                Panel card = CreateEquipmentCard(
                    equipmentId,
                    equipmentName,
                    category,
                    total,
                    available,
                    status,
                    imagePath);

                flowEquipmentCards.Controls.Add(card);
                hasCards = true;
            }

            if (!hasCards)
            {
                Label lblEmpty = new Label
                {
                    Text = demoModeEnabled
                        ? "No equipment available for your enrolled subjects."
                        : "No equipment available right now.\nEquipment is only visible during your scheduled lab time.",
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(900, 60)
                };

                flowEquipmentCards.Controls.Add(lblEmpty);
            }
        }




        private int GetCorrectAvailableQuantity(
    OleDbConnection conn,
    int equipmentId,
    int total,
    int maintenance,
    bool hasSerial,
    string equipmentType)
        {
            if (hasSerial)
            {
                return GetAvailableSerialUnitCount(conn, equipmentId);
            }

            int borrowedQty = GetEquipmentBorrowedQuantity(conn, equipmentId);
            int usedUpQty = GetEquipmentUsedUpQuantity(conn, equipmentId);
            int reportedQty = GetEquipmentReportedQuantity(conn, equipmentId);

            int available = total - maintenance - borrowedQty - usedUpQty - reportedQty;

            if (available < 0)
                available = 0;

            return available;
        }


        private int GetAvailableSerialUnitCount(OleDbConnection conn, int equipmentId)
        {
            string query = @"
SELECT COUNT(*)
FROM EquipmentUnits
WHERE EquipmentID = ?
AND UnitStatus = 'Available'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }


        private int GetEquipmentPendingQuantity(OleDbConnection conn, int equipmentId)
        {
            string query = @"
SELECT SUM(
    BSI.QuantityRequested -
    IIF(
        (SELECT SUM(DR.DamageQuantity) FROM DamageReports AS DR WHERE DR.SlipItemID = BSI.SlipItemID) IS NULL,
        0,
        (SELECT SUM(DR2.DamageQuantity) FROM DamageReports AS DR2 WHERE DR2.SlipItemID = BSI.SlipItemID)
    )
)
FROM BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID
WHERE BSI.EquipmentID = ?
AND BS.SlipStatus = 'Pending'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }


        private int GetEquipmentBorrowedQuantity(OleDbConnection conn, int equipmentId)
        {
            string query = @"
SELECT SUM(BSI.QuantityRequested)
FROM BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID
WHERE BSI.EquipmentID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }


        private int GetEquipmentUsedUpQuantity(OleDbConnection conn, int equipmentId)
{
    string query = @"
SELECT SUM(
    IIF(
        BSI.QuantityRequested - BSI.QuantityReturned -
        IIF(
            (SELECT SUM(DR.DamageQuantity) FROM DamageReports AS DR WHERE DR.SlipItemID = BSI.SlipItemID) IS NULL,
            0,
            (SELECT SUM(DR2.DamageQuantity) FROM DamageReports AS DR2 WHERE DR2.SlipItemID = BSI.SlipItemID)
        ) < 0,
        0,
        BSI.QuantityRequested - BSI.QuantityReturned -
        IIF(
            (SELECT SUM(DR3.DamageQuantity) FROM DamageReports AS DR3 WHERE DR3.SlipItemID = BSI.SlipItemID) IS NULL,
            0,
            (SELECT SUM(DR4.DamageQuantity) FROM DamageReports AS DR4 WHERE DR4.SlipItemID = BSI.SlipItemID)
        )
    )
)
FROM (BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.EquipmentID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Returned'
AND (E.EquipmentType = 'Consumable' OR E.EquipmentType = 'One Time Use' OR E.EquipmentType = 'Limited Use')";

    using OleDbCommand cmd = new OleDbCommand(query, conn);
    cmd.Parameters.AddWithValue("@p1", equipmentId);

    object result = cmd.ExecuteScalar();

    return result != null && result != DBNull.Value
        ? Convert.ToInt32(result)
        : 0;
}

        private int GetEquipmentReportedQuantity(OleDbConnection conn, int equipmentId)
        {
            string query = @"
SELECT SUM(DamageQuantity)
FROM DamageReports
WHERE EquipmentID = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }



        private void ShowReserveDialog(int equipmentId, string equipmentName, int availableQty)
        {
            Form reserveForm = new Form
            {
                Text = "Add Equipment",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(300, 265),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(250, 245, 247)
            };

            Label lblItem = new Label
            {
                Text = equipmentName,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(20, 24),
                Size = new Size(240, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblAvailable = new Label
            {
                Text = "Available: " + availableQty,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(34, 82),
                AutoSize = true
            };

            Label lblQty = new Label
            {
                Text = "Quantity:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(34, 116),
                AutoSize = true
            };

            Button btnMinus = new Button
            {
                Text = "-",
                Size = new Size(28, 26),
                Location = new Point(116, 112),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            btnMinus.FlatAppearance.BorderSize = 1;

            TextBox txtQty = new TextBox
            {
                Text = "1",
                Location = new Point(146, 113),
                Size = new Size(38, 26),
                Font = new Font("Segoe UI", 10F),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true
            };

            Button btnPlus = new Button
            {
                Text = "+",
                Size = new Size(28, 26),
                Location = new Point(186, 112),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            btnPlus.FlatAppearance.BorderSize = 1;

            Button btnConfirm = new Button
            {
                Text = "Add",
                Size = new Size(90, 34),
                Location = new Point(96, 166),
                BackColor = Color.FromArgb(169, 215, 159),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold)
            };
            btnConfirm.FlatAppearance.BorderSize = 0;

            int selectedQuantity = 1;

            void refreshQty()
            {
                txtQty.Text = selectedQuantity.ToString();
                btnMinus.Enabled = selectedQuantity > 1;
                btnPlus.Enabled = selectedQuantity < availableQty;
            }

            btnMinus.Click += (s, e) =>
            {
                if (selectedQuantity > 1)
                    selectedQuantity--;
                refreshQty();
            };

            btnPlus.Click += (s, e) =>
            {
                if (selectedQuantity < availableQty)
                    selectedQuantity++;
                refreshQty();
            };

            btnConfirm.Click += (s, e) =>
            {
                if (selectedQuantity <= 0)
                {
                    MessageBox.Show(
                        "Quantity must be at least 1.",
                        "Invalid Quantity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Prevent exceeding available
                if (selectedQuantity > availableQty)
                {
                    MessageBox.Show(
                        $"Only {availableQty} unit(s) available. Please reduce your quantity.",
                        "Invalid Quantity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                SaveReservation(equipmentId, selectedQuantity, DateTime.Now);
                reserveForm.Close();
            };

            refreshQty();

            reserveForm.Controls.Add(lblItem);
            reserveForm.Controls.Add(lblAvailable);
            reserveForm.Controls.Add(lblQty);
            reserveForm.Controls.Add(btnMinus);
            reserveForm.Controls.Add(txtQty);
            reserveForm.Controls.Add(btnPlus);
            reserveForm.Controls.Add(btnConfirm);

            reserveForm.ShowDialog(this);
        }



        private DateTime GetNextValidReservationDate(DateTime startDate)
        {
            DateTime date = startDate;

            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);

            return date;
        }




        private void ConfigureHistoryGrid()
        {
            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.Columns.Clear();
            dgvHistory.CellClick -= dgvHistory_CellClick;
            dgvHistory.CellClick += dgvHistory_CellClick;
            dgvHistory.DataBindingComplete -= dgvHistory_DataBindingComplete;
            dgvHistory.DataBindingComplete += dgvHistory_DataBindingComplete;
            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.AllowUserToResizeRows = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.MultiSelect = false;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.BackgroundColor = Color.FromArgb(255, 251, 252);
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 250);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(90, 60, 100);
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgvHistory.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 240, 250);
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(90, 60, 100);
            dgvHistory.ColumnHeadersHeight = 42;
            dgvHistory.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvHistory.DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 80);
            dgvHistory.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 210, 240);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvHistory.RowTemplate.Height = 40;

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipID",
                HeaderText = "SlipID",
                DataPropertyName = "SlipID",
                Visible = false
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GroupNumber",
                HeaderText = "Group #",
                DataPropertyName = "GroupNumber",
                Width = 85
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectCode",
                HeaderText = "Subject",
                DataPropertyName = "SubjectCode",
                Width = 110
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Section",
                HeaderText = "Section",
                DataPropertyName = "Section",
                Width = 90
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LeaderName",
                HeaderText = "Leader",
                DataPropertyName = "LeaderName",
                Width = 170
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SchoolID",
                HeaderText = "School ID",
                DataPropertyName = "SchoolID",
                Width = 120
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalQuantity",
                HeaderText = "Total Qty",
                DataPropertyName = "TotalQuantity",
                Width = 90
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BorrowDate",
                HeaderText = "Date Borrowed",
                DataPropertyName = "BorrowDate",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HistoryStatus",
                HeaderText = "Status",
                DataPropertyName = "HistoryStatus",
                Width = 100
            });
        }

        private void dgvHistory_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvHistory.ClearSelection();
            dgvHistory.CurrentCell = null;
        }



        private void LoadUserHistory()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    LS.SubjectCode,
    SS.Section,
    BS.LeaderName,
    U.SchoolID,
    SUM(BSI.QuantityRequested) AS TotalQuantity,
    BS.DateCreated AS BorrowDate,
    'Returned' AS HistoryStatus
FROM ((((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
WHERE
(
    BS.UserID = ?
    OR BS.SlipID IN
(
    SELECT SlipID
    FROM BorrowSlipMembers
    WHERE UserID = ?
)
)
AND BS.SlipStatus = 'Approved'
GROUP BY BS.SlipID, BS.GroupNumber, LS.SubjectCode, SS.Section, BS.LeaderName, U.SchoolID, BS.DateCreated
HAVING (SUM(IIF(BSI.ItemReturnStatus = 'Borrowed', 1, 0)) = 0
AND SUM(IIF(BSI.ItemReturnStatus = 'Returned', 1, 0)) > 0)
OR BS.SlipID IN (SELECT SlipID FROM DamageReports)
ORDER BY BS.DateCreated DESC";

                using var cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                cmd.Parameters.AddWithValue("@p2", SessionManager.UserID);

                DataTable dt = new DataTable();
                using var da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                dgvHistory.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading history:\n" + ex.Message, "History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistory_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvHistory.Rows[e.RowIndex].IsNewRow)
                return;

            object? value = dgvHistory.Rows[e.RowIndex].Cells["SlipID"].Value;
            if (value == null || value == DBNull.Value)
                return;

            ShowUserBorrowerSlipDialog(Convert.ToInt32(value));
            dgvHistory.ClearSelection();
        }

        private int GetReportedQuantityForSlipItem(OleDbConnection conn, int slipItemId)
        {
            if (slipItemId <= 0)
                return 0;

            using OleDbCommand cmd = new OleDbCommand(@"
SELECT SUM(DamageQuantity)
FROM DamageReports
WHERE SlipItemID = ?", conn);
            cmd.Parameters.AddWithValue("@p1", slipItemId);

            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        private void ShowUserBorrowerSlipDialog(int slipId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string leaderName = "";
                string schoolId = "";
                string groupNumber = "";
                string section = "";
                string subjectCode = "";
                string slipStatus = "";
                DateTime borrowDate = DateTime.Now;
                DateTime dueDate = DateTime.Now;

                string slipQuery = @"
SELECT
    BS.LeaderName,
    BS.GroupNumber,
    BS.DateCreated,
    BS.SlipStatus,
    U.SchoolID,
    LS.SubjectCode,
    SS.Section,
    SS.EndTime
FROM ((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID
WHERE BS.SlipID = ?";

                using (OleDbCommand cmd = new OleDbCommand(slipQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);

                    using OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader == null || !reader.Read())
                    {
                        MessageBox.Show("Borrow slip not found for this account.", "History",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    leaderName = reader["LeaderName"]?.ToString() ?? "";
                    schoolId = reader["SchoolID"]?.ToString() ?? "";
                    groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    section = reader["Section"]?.ToString() ?? "";
                    subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    slipStatus = reader["SlipStatus"]?.ToString() ?? "";
                    borrowDate = reader["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateCreated"])
                        : DateTime.Now;
                    dueDate = BuildUserDueDate(borrowDate, reader["EndTime"]);
                }

                List<string> members = new List<string>();
                using (OleDbCommand cmd = new OleDbCommand(@"
SELECT BSM.MemberName, U.SchoolID
FROM BorrowSlipMembers AS BSM
LEFT JOIN Users AS U ON BSM.UserID = U.UserID
WHERE BSM.SlipID = ?
ORDER BY BSM.MemberName", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string member = reader["MemberName"]?.ToString() ?? "";
                        string memberSchoolId = reader["SchoolID"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(member))
                        {
                            members.Add(string.IsNullOrWhiteSpace(memberSchoolId)
                                ? member
                                : member + " [" + memberSchoolId + "]");
                        }
                    }
                }

                List<(string EquipmentName, int Quantity, string Status, int Reported, int Returned)> equipmentLines =
                    new List<(string EquipmentName, int Quantity, string Status, int Reported, int Returned)>();
                using (OleDbCommand cmd = new OleDbCommand(@"
SELECT BSI.SlipItemID, E.EquipmentName, BSI.QuantityRequested, BSI.QuantityReturned, BSI.ItemReturnStatus
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?
ORDER BY E.EquipmentName", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string equipment = reader["EquipmentName"]?.ToString() ?? "";
                        int slipItemId = reader["SlipItemID"] != DBNull.Value
                            ? Convert.ToInt32(reader["SlipItemID"])
                            : 0;
                        int requested = reader["QuantityRequested"] != DBNull.Value
                            ? Convert.ToInt32(reader["QuantityRequested"])
                            : 0;
                        int returned = reader["QuantityReturned"] != DBNull.Value
                            ? Convert.ToInt32(reader["QuantityReturned"])
                            : requested;
                        string itemStatus = reader["ItemReturnStatus"]?.ToString() ?? "";
                        int reported = GetReportedQuantityForSlipItem(conn, slipItemId);
                        int displayQuantity = Math.Max(0, requested - reported);

                        equipmentLines.Add((equipment, displayQuantity, itemStatus, reported, returned));
                    }
                }

                List<string> reportLines = new List<string>();
                using (OleDbCommand cmd = new OleDbCommand(@"
SELECT DR.ReportID, E.EquipmentName, DR.DamageType, DR.DamageQuantity, DR.ReportStatus, DR.CurrentReplacementCost, DR.IndividualShare
FROM (DamageReports AS DR
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
WHERE DR.SlipID = ?
ORDER BY DR.DateReported DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        decimal totalCost = reader["CurrentReplacementCost"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentReplacementCost"]) : 0;
                        decimal share = reader["IndividualShare"] != DBNull.Value ? Convert.ToDecimal(reader["IndividualShare"]) : 0;
                        reportLines.Add("Report #" + reader["ReportID"] + " - " + reader["EquipmentName"] +
                            " | " + reader["DamageType"] + " Qty: " + reader["DamageQuantity"] +
                            " | " + reader["ReportStatus"] +
                            (totalCost > 0 ? " | Cost: ₱" + totalCost.ToString("N2") + " | Share: ₱" + share.ToString("N2") : ""));
                    }
                }

                Form slipForm = new Form
                {
                    Text = "Borrower's Slip",
                    StartPosition = FormStartPosition.CenterParent,
                    AutoScaleMode = AutoScaleMode.None,
                    ClientSize = new Size(380, 660),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(250, 246, 238)
                };
                slipForm.MinimumSize = slipForm.Size;
                slipForm.MaximumSize = slipForm.Size;

                Label title = new Label
                {
                    Text = "Borrow Slip Details",
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(92, 45, 58),
                    Location = new Point(28, 24),
                    AutoSize = true
                };

                Label statusPill = new Label
                {
                    Text = "VIEW ONLY",
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(126, 105, 136),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(250, 30),
                    Size = new Size(110, 26)
                };
                RoundControl(statusPill, 13);

                Label slipInfo = new Label
                {
                    Text =
                        "Group #: " + groupNumber + "\n" +
                        "Date Borrowed: " + borrowDate.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                        "Subject: " + subjectCode + "\n" +
                        "Section: " + section + "\n" +
                        "Leader: " + leaderName + "\n" +
                        "Student: " + leaderName + " (" + schoolId + ")\n" +
                        "Expected Return: " + dueDate.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                        "Status: " + slipStatus,
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(14, 10),
                    Size = new Size(300, 180),
                    BackColor = Color.Transparent
                };

                Panel infoPanel = new Panel
                {
                    Location = new Point(28, 76),
                    Size = new Size(330, 205),
                    BackColor = Color.FromArgb(255, 253, 247)
                };
                RoundControl(infoPanel, 18);
                infoPanel.Controls.Add(slipInfo);

                Label membersTitle = new Label
                {
                    Text = "Members:",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(32, 300),
                    AutoSize = true
                };

                Panel membersPanel = CreateReadonlyHistoryPanel(
                    members.Count == 0 ? "No members listed." : string.Join(Environment.NewLine, members),
                    new Point(32, 325),
                    new Size(330, 92),
                    new Font("Segoe UI", 10F));

                Label equipmentTitle = new Label
                {
                    Text = "Equipments:",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(32, 435),
                    AutoSize = true
                };

                Panel equipmentPanel = CreateHistoryEquipmentPanel(
                    equipmentLines,
                    reportLines,
                    new Point(32, 460),
                    new Size(330, 135));

                Button btnClose = new Button
                {
                    Text = "Close",
                    Size = new Size(120, 34),
                    Location = new Point(128, 615),
                    BackColor = Color.FromArgb(212, 168, 45),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) => slipForm.Close();
                ApplyActionButtonStyle(btnClose);

                slipForm.Controls.Add(title);
                slipForm.Controls.Add(statusPill);
                slipForm.Controls.Add(infoPanel);
                slipForm.Controls.Add(membersTitle);
                slipForm.Controls.Add(membersPanel);
                slipForm.Controls.Add(equipmentTitle);
                slipForm.Controls.Add(equipmentPanel);
                slipForm.Controls.Add(btnClose);
                slipForm.Shown += (s, e) => slipForm.ActiveControl = btnClose;

                slipForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening borrower slip:\n" + ex.Message,
                    "History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateReadonlyHistoryPanel(string text, Point location, Size size, Font font)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                TabStop = false
            };

            Label label = new Label
            {
                Text = text,
                Location = new Point(6, 6),
                AutoSize = true,
                MaximumSize = new Size(size.Width - 28, 0),
                Font = font,
                ForeColor = Color.FromArgb(72, 53, 84),
                BackColor = Color.Transparent
            };

            panel.Controls.Add(label);
            return panel;
        }

        private Panel CreateHistoryEquipmentPanel(
            List<(string EquipmentName, int Quantity, string Status, int Reported, int Returned)> equipmentLines,
            List<string> reportLines,
            Point location,
            Size size)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                TabStop = false
            };

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(size.Width - 4, size.Height - 4),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White
            };

            if (equipmentLines.Count == 0)
            {
                Label empty = new Label
                {
                    Text = "No equipment listed.",
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(8, 8),
                    AutoSize = true
                };
                flow.Controls.Add(empty);
            }

            foreach (var item in equipmentLines)
            {
                Panel row = new Panel
                {
                    Size = new Size(size.Width - 28, 58),
                    Margin = new Padding(6, 6, 6, 0),
                    BackColor = Color.FromArgb(255, 251, 252)
                };
                RoundControl(row, 10);

                Label name = new Label
                {
                    Text = item.EquipmentName,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(10, 7),
                    Size = new Size(195, 22),
                    AutoEllipsis = true
                };

                Label qty = new Label
                {
                    Text = "Qty: " + item.Quantity,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(92, 45, 58),
                    Location = new Point(218, 7),
                    Size = new Size(70, 20),
                    TextAlign = ContentAlignment.TopRight
                };

                string statusText = string.IsNullOrWhiteSpace(item.Status) ? "Recorded" : item.Status;
                if (item.Reported > 0)
                    statusText += " | Reported: " + item.Reported;
                if (item.Returned != item.Quantity)
                    statusText += " | Returned: " + item.Returned;

                Label status = new Label
                {
                    Text = statusText,
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Location = new Point(10, 31),
                    Size = new Size(278, 20),
                    AutoEllipsis = true
                };

                row.Controls.Add(name);
                row.Controls.Add(qty);
                row.Controls.Add(status);
                flow.Controls.Add(row);
            }

            if (reportLines.Count > 0)
            {
                Label reportTitle = new Label
                {
                    Text = "Reports",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Margin = new Padding(8, 10, 8, 0),
                    AutoSize = true
                };
                flow.Controls.Add(reportTitle);

                foreach (string reportLine in reportLines)
                {
                    Label report = new Label
                    {
                        Text = reportLine,
                        Font = new Font("Segoe UI", 8.5F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        MaximumSize = new Size(size.Width - 34, 0),
                        AutoSize = true,
                        Margin = new Padding(8, 4, 8, 0)
                    };
                    flow.Controls.Add(report);
                }
            }

            panel.Controls.Add(flow);
            return panel;
        }



        private void LoadUserDashboardData()
        {
            if (SessionManager.UserID <= 0)
                return;

            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                CurrentLabContext? labContext = GetCurrentLabContext(conn);
                bool isRestricted = labContext != null && HasCurrentLabRestriction(conn, labContext.LabID);

                int historicalBorrowedQty = GetScalarCount(conn, @"
SELECT SUM(BSI.QuantityRequested)
FROM BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus <> 'Borrowed'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)", SessionManager.UserID, SessionManager.UserID);

                int currentlyBorrowingQty = GetScalarCount(conn, @"
SELECT SUM(BSI.QuantityRequested)
FROM BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)", SessionManager.UserID, SessionManager.UserID);

                int overdueSlipCount = GetUserOverdueSlipCount(conn);

                lblBorrowedCardValue.Text = labContext == null
                    ? "No Lab"
                    : (isRestricted ? "Restricted" : "Active");
                lblBorrowedCardValue.ForeColor = isRestricted
                    ? Color.FromArgb(153, 0, 0)
                    : Color.FromArgb(61, 132, 74);

                lblDueSoonValue.Text = historicalBorrowedQty.ToString("00");
                lblOverdueValue.Text = overdueSlipCount.ToString("00");
                lblHistoryValue.Text = currentlyBorrowingQty.ToString("00");

                lblBorrowedCardTitle.Text = "Status";
                lblDueSoonTitle.Text = "Borrowed";
                lblOverdueTitle.Text = "Overdue";
                lblHistoryCardTitle.Text = "Borrowing";
                lblDueSoonSubCard.Text = "Total quantity";
                lblOverdueSubCard.Text = "Needs attention";
                lblHistorySubCard.Text = "Items you have now";

                LoadUserReminders(conn);
                LoadEquipmentUsageChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard:\n" + ex.Message);
            }
        }



        private void LoadUserReminders(OleDbConnection conn)
        {
            try
            {
                if (SessionManager.UserID <= 0)
                    return;

                flowReminderCards.Controls.Clear();

                // 1. Pending borrow slips
                string borrowSlipQuery = @"
SELECT 
    BS.SlipID,
    E.EquipmentName,
    BSI.QuantityRequested AS Quantity,
    BS.DateCreated,
    BS.SlipStatus
FROM ((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID)
WHERE BS.UserID = ?
AND BS.SlipStatus = 'Pending'
ORDER BY BS.DateCreated DESC";

                using (OleDbCommand cmd = new OleDbCommand(borrowSlipQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", SessionManager.UserID);

                    using OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader != null && reader.Read())
                    {
                        int slipId = reader["SlipID"] != DBNull.Value ? Convert.ToInt32(reader["SlipID"]) : 0;
                        string itemName = reader["EquipmentName"]?.ToString() ?? "";
                        int qty = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0;
                        DateTime dateCreated = reader["DateCreated"] != DBNull.Value ? Convert.ToDateTime(reader["DateCreated"]) : DateTime.Today;
                        string status = reader["SlipStatus"]?.ToString() ?? "Pending";

                        Panel card = CreateReminderCard(
                            slipId,
                            "Borrow Slip",
                            itemName,
                            qty,
                            dateCreated,
                            status,
                            Color.FromArgb(255, 239, 213),
                            Color.FromArgb(160, 98, 27));

                        AddReminderCardNewestFirst(card, dateCreated);
                    }
                }

                string declinedSlipQuery = @"
SELECT
    BS.SlipID,
    BS.DateCreated,
    BS.GroupNumber,
    BS.DeclineReason,
    LS.SubjectCode
FROM (BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
WHERE BS.SlipStatus = 'Declined'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
ORDER BY BS.DateCreated DESC";

                using (OleDbCommand declinedCmd = new OleDbCommand(declinedSlipQuery, conn))
                {
                    declinedCmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
                    declinedCmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

                    using OleDbDataReader declinedReader = declinedCmd.ExecuteReader();
                    while (declinedReader != null && declinedReader.Read())
                    {
                        int slipId = declinedReader["SlipID"] != DBNull.Value ? Convert.ToInt32(declinedReader["SlipID"]) : 0;
                        string subject = declinedReader["SubjectCode"]?.ToString() ?? "";
                        string group = declinedReader["GroupNumber"]?.ToString() ?? "";
                        string declineReason = declinedReader["DeclineReason"]?.ToString() ?? "";
                        DateTime dateCreated = declinedReader["DateCreated"] != DBNull.Value ? Convert.ToDateTime(declinedReader["DateCreated"]) : DateTime.Today;

                        Panel card = CreateReminderCard(
                            slipId,
                            "Borrow Slip Declined",
                            "Group " + group + " - " + subject,
                            1,
                            dateCreated,
                            "Declined",
                            Color.FromArgb(255, 225, 225),
                            Color.FromArgb(153, 0, 0),
                            declineReason);

                        AddReminderCardNewestFirst(card, dateCreated);
                    }
                }

                string readyToClaimQuery = @"
SELECT
    BS.SlipID,
    BS.DateCreated,
    BS.ApprovedDate,
    BS.GroupNumber,
    LS.SubjectCode
FROM (BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
WHERE BS.SlipStatus = 'Approved'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
AND BS.SlipID IN
(
    SELECT SlipID
    FROM BorrowSlipItems
    WHERE ItemReturnStatus = 'Borrowed'
)
ORDER BY BS.ApprovedDate DESC";

                using (OleDbCommand readyCmd = new OleDbCommand(readyToClaimQuery, conn))
                {
                    readyCmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
                    readyCmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

                    using OleDbDataReader readyReader = readyCmd.ExecuteReader();
                    while (readyReader != null && readyReader.Read())
                    {
                        int slipId = readyReader["SlipID"] != DBNull.Value ? Convert.ToInt32(readyReader["SlipID"]) : 0;
                        string subject = readyReader["SubjectCode"]?.ToString() ?? "";
                        string group = readyReader["GroupNumber"]?.ToString() ?? "";
                        DateTime dateCreated = readyReader["ApprovedDate"] != DBNull.Value
                            ? Convert.ToDateTime(readyReader["ApprovedDate"])
                            : readyReader["DateCreated"] != DBNull.Value
                                ? Convert.ToDateTime(readyReader["DateCreated"])
                                : DateTime.Today;

                        Panel card = CreateReminderCard(
                            slipId,
                            "To Be Claimed",
                            "Group " + group + " - " + subject,
                            1,
                            dateCreated,
                            "Approved",
                            Color.FromArgb(238, 248, 235),
                            Color.FromArgb(61, 132, 74));

                        AddReminderCardNewestFirst(card, dateCreated);
                    }
                }

                LoadUserOverdueReminderCards(conn);

                // Damage/Lost payment notifications
                string reportQuery = @"
SELECT
    DR.ReportID,
    DR.DamageType,
    DR.CurrentReplacementCost,
    DR.IndividualShare,
    DR.ReportStatus,
    DR.DateCostSet,
    DR.DateResolved,
    DR.DateReported,
    E.EquipmentName,
    LS.SubjectCode
FROM ((((DamageReportMembers AS DRM
INNER JOIN DamageReports AS DR ON DRM.ReportID = DR.ReportID)
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN SubjectSchedules AS SS ON DR.ScheduleID = SS.ScheduleID)
WHERE DRM.UserID = ?
AND DR.ReportStatus IN ('For Payment', 'Paid')
AND SS.DayOfWeek = ?
AND SS.StartTime <= ?
AND SS.EndTime >= ?
ORDER BY IIF(DR.ReportStatus = 'Paid', 1, 0), DR.DateCostSet DESC, DR.DateResolved DESC";

                using (OleDbCommand cmdReport = new OleDbCommand(reportQuery, conn))
                {
                    cmdReport.Parameters.AddWithValue("@p1", SessionManager.UserID);
                    cmdReport.Parameters.AddWithValue("@p2", DateTime.Now.DayOfWeek.ToString());
                    cmdReport.Parameters.AddWithValue("@p3", DateTime.Now.ToString("HH:mm:ss"));
                    cmdReport.Parameters.AddWithValue("@p4", DateTime.Now.ToString("HH:mm:ss"));

                    using OleDbDataReader reader = cmdReport.ExecuteReader();

                    while (reader != null && reader.Read())
                    {
                        int reportId = reader["ReportID"] != DBNull.Value ? Convert.ToInt32(reader["ReportID"]) : 0;
                        string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                        string damageType = reader["DamageType"]?.ToString() ?? "";
                        string subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                        string reportStatus = reader["ReportStatus"]?.ToString() ?? "";
                        DateTime sortDate = reader["DateCostSet"] != DBNull.Value
                            ? Convert.ToDateTime(reader["DateCostSet"])
                            : reader["DateResolved"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DateResolved"])
                                : reader["DateReported"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["DateReported"])
                                    : DateTime.Today;

                        decimal totalCost = reader["CurrentReplacementCost"] != DBNull.Value
                            ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                            : 0;

                        decimal share = reader["IndividualShare"] != DBNull.Value
                            ? Convert.ToDecimal(reader["IndividualShare"])
                            : 0;

                        Panel card = CreateDamagePaymentReminderCard(
                            reportId,
                            equipmentName,
                            damageType,
                            subjectCode,
                            totalCost,
                            share,
                            reportStatus,
                            sortDate);

                        AddReminderCardNewestFirst(card, sortDate);
                    }
                }

                if (flowReminderCards.Controls.Count == 0)
                {
                    Panel emptyCard = new Panel
                    {
                        Width = 335,
                        Height = 60,
                        BackColor = Color.FromArgb(245, 240, 245),
                        Margin = new Padding(0, 0, 0, 10)
                    };

                    RoundControl(emptyCard, 18);

                    Label lblEmpty = new Label
                    {
                        Text = "No notifications right now.",
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(100, 82, 112),
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };

                    emptyCard.Controls.Add(lblEmpty);
                    flowReminderCards.Controls.Add(emptyCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Reminder error:\n" + ex.Message,
                    "Dashboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private void AddReminderCardNewestFirst(Panel card, DateTime sortDate)
        {
            card.AccessibleName = sortDate.Ticks.ToString();

            for (int i = 0; i < flowReminderCards.Controls.Count; i++)
            {
                Control existing = flowReminderCards.Controls[i];
                if (!long.TryParse(existing.AccessibleName, out long existingTicks))
                    continue;

                if (sortDate.Ticks > existingTicks)
                {
                    flowReminderCards.Controls.Add(card);
                    flowReminderCards.Controls.SetChildIndex(card, i);
                    return;
                }
            }

            flowReminderCards.Controls.Add(card);
        }





        private Panel CreateDamagePaymentReminderCard(
    int reportId,
    string equipmentName,
    string damageType,
    string subjectCode,
    decimal totalCost,
    decimal individualShare,
    string reportStatus,
    DateTime receivedAt)
        {
            bool isPaid = string.Equals(reportStatus, "Paid", StringComparison.OrdinalIgnoreCase);

            Panel card = new Panel
            {
                Width = 335,
                Height = 122,
                BackColor = isPaid ? Color.FromArgb(225, 246, 229) : Color.FromArgb(255, 225, 225),
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand,
                Tag = reportId
            };

            RoundControl(card, 18);

            Label lblTitle = new Label
            {
                Text = isPaid ? "Payment Required [paid]" : "Payment Required",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = isPaid ? Color.FromArgb(45, 120, 60) : Color.FromArgb(92, 45, 58),
                Location = new Point(14, 10),
                AutoSize = true
            };

            Label lblItem = new Label
            {
                Text = damageType + ": " + equipmentName,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(14, 35),
                Size = new Size(300, 20)
            };

            Label lblCost = new Label
            {
                Text = subjectCode + " | Group Total: ₱" + totalCost.ToString("N2"),
                Font = new Font("Segoe UI", 8.8F),
                ForeColor = isPaid ? Color.FromArgb(61, 132, 74) : Color.FromArgb(126, 75, 85),
                Location = new Point(14, 57),
                Size = new Size(305, 20)
            };

            Label lblShare = new Label
            {
                Text = (isPaid ? "Paid: " : "Your share: ₱") +
                    (isPaid ? "₱" : "") + individualShare.ToString("N2") + " | Click to view slip",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = isPaid ? Color.FromArgb(45, 120, 60) : Color.FromArgb(160, 55, 65),
                Location = new Point(14, 78),
                Size = new Size(305, 20)
            };

            Label lblReceived = new Label
            {
                Text = "Received: " + receivedAt.ToString("MM/dd/yyyy hh:mm tt"),
                Font = new Font("Segoe UI", 8.3F),
                ForeColor = Color.FromArgb(110, 90, 122),
                Location = new Point(14, 99),
                Size = new Size(305, 18)
            };

            void openSlip(object? s, EventArgs e)
            {
                ShowPaymentSlipPopup(reportId);
            }

            card.Click += openSlip;
            lblTitle.Click += openSlip;
            lblItem.Click += openSlip;
            lblCost.Click += openSlip;
            lblShare.Click += openSlip;
            lblReceived.Click += openSlip;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblItem);
            card.Controls.Add(lblCost);
            card.Controls.Add(lblShare);
            card.Controls.Add(lblReceived);
            AttachDismissNotificationMenu(card);

            return card;
        }

        private void LoadUserOverdueReminderCards(OleDbConnection conn)
        {
            string query = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    BS.DateCreated,
    SS.EndTime,
    LS.SubjectCode
FROM (((BorrowSlips AS BS
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
GROUP BY BS.SlipID, BS.GroupNumber, BS.DateCreated, SS.EndTime, LS.SubjectCode
ORDER BY BS.DateCreated DESC";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
            cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                DateTime borrowDate = reader["DateCreated"] != DBNull.Value
                    ? Convert.ToDateTime(reader["DateCreated"])
                    : DateTime.Now;

                DateTime dueDate = BuildUserDueDate(borrowDate, reader["EndTime"]);
                if (DateTime.Now <= dueDate)
                    continue;

                int slipId = reader["SlipID"] != DBNull.Value ? Convert.ToInt32(reader["SlipID"]) : 0;
                string subject = reader["SubjectCode"]?.ToString() ?? "";
                string group = reader["GroupNumber"]?.ToString() ?? "";

                Panel card = CreateReminderCard(
                    slipId,
                    "Overdue Borrowed Equipment",
                    "Group " + group + " - " + subject,
                    1,
                    dueDate,
                    "Overdue",
                    Color.FromArgb(255, 225, 225),
                    Color.FromArgb(153, 0, 0));

                AddReminderCardNewestFirst(card, dueDate);
            }
        }



        private void ShowPaymentSlipPopup(int reportId)
        {
            try
            {
                string slipText = "";

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    DR.ReportID,
    DR.SlipID,
    DR.DamageType,
    DR.DamageQuantity,
    DR.Description,
    DR.DateReported,
    DR.CurrentReplacementCost,
    DR.IndividualShare,
    DR.ReportStatus,
    E.EquipmentName,
    LS.SubjectCode,
    LS.SubjectName,
    BS.GroupNumber,
    BS.LeaderName
FROM (((DamageReports AS DR
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN BorrowSlips AS BS ON DR.SlipID = BS.SlipID)
WHERE DR.ReportID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", reportId);

                using OleDbDataReader reader = cmd.ExecuteReader();

                if (reader == null || !reader.Read())
                {
                    MessageBox.Show("Payment slip not found.",
                        "Payment Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal totalCost = reader["CurrentReplacementCost"] != DBNull.Value
                    ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                    : 0;

                decimal individualShare = reader["IndividualShare"] != DBNull.Value
                    ? Convert.ToDecimal(reader["IndividualShare"])
                    : 0;

                slipText =
     "WILDCATHUB PAYMENT SLIP\r\n" +
     "----------------------------------------\r\n\r\n" +
     "Report ID:        " + reader["ReportID"] + "\r\n" +
     "Slip ID:          " + reader["SlipID"] + "\r\n" +
     "Status:           " + reader["ReportStatus"] + "\r\n\r\n" +

     "STUDENT INFORMATION\r\n" +
     "Student Name:     " + SessionManager.FullName + "\r\n" +
     "School ID:        " + SessionManager.SchoolID + "\r\n\r\n" +

     "BORROWING INFORMATION\r\n" +
     "Subject:          " + reader["SubjectCode"] + " - " + reader["SubjectName"] + "\r\n" +
     "Group Number:     " + reader["GroupNumber"] + "\r\n" +
     "Leader:           " + reader["LeaderName"] + "\r\n\r\n" +

     "REPORT INFORMATION\r\n" +
     "Equipment:        " + reader["EquipmentName"] + "\r\n" +
     "Damage Type:      " + reader["DamageType"] + "\r\n" +
     "Quantity:         " + reader["DamageQuantity"] + "\r\n" +
     "Date Reported:    " + Convert.ToDateTime(reader["DateReported"]).ToString("MMMM dd, yyyy") + "\r\n" +
     "Details:          " + reader["Description"] + "\r\n\r\n" +

     "PAYMENT INFORMATION\r\n" +
     "Total Group Cost: PHP " + totalCost.ToString("N2") + "\r\n" +
     "Individual Share: PHP " + individualShare.ToString("N2") + "\r\n\r\n" +

     "NOTE:\r\n" +
     "Please bring this slip to the laboratory\r\n" +
     "admin/NAS for validation/signature, then\r\n" +
     "proceed to the cashier for payment.\r\n\r\n" +
     "________________________________\r\n" +
     "Admin's Signature over Full Name\r\n\r\n" +
     "Cashier Receipt No.: ______________________\r\n" +
     "Date Paid:           ______________________";

                Form slipForm = new Form();
                slipForm.Text = "Payment Slip";
                slipForm.StartPosition = FormStartPosition.CenterParent;
                slipForm.AutoScaleMode = AutoScaleMode.None;
                slipForm.ClientSize = new Size(430, 650);
                slipForm.MinimumSize = slipForm.Size;
                slipForm.MaximumSize = slipForm.Size;
                slipForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                slipForm.MaximizeBox = false;
                slipForm.MinimizeBox = false;
                slipForm.BackColor = Color.FromArgb(250, 245, 247);

                Label lblTitle = new Label();
                lblTitle.Text = "Payment Slip";
                lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
                lblTitle.Location = new Point(25, 20);
                lblTitle.AutoSize = true;

                Panel pnlSlip = CreatePaymentSlipDisplayPanel(
                    slipText,
                    new Point(25, 65),
                    new Size(380, 510));

                Button btnDownload = new Button();
                btnDownload.Text = "Download";
                btnDownload.Size = new Size(130, 38);
                btnDownload.Location = new Point(135, 592);
                btnDownload.BackColor = Color.FromArgb(212, 168, 45);
                btnDownload.ForeColor = Color.White;
                btnDownload.FlatStyle = FlatStyle.Flat;
                btnDownload.FlatAppearance.BorderSize = 0;
                btnDownload.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

                Button btnClose = new Button();
                btnClose.Text = "Close";
                btnClose.Size = new Size(130, 38);
                btnClose.Location = new Point(275, 592);
                btnClose.BackColor = Color.FromArgb(214, 197, 224);
                btnClose.ForeColor = Color.FromArgb(87, 60, 99);
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

                btnDownload.Click += (s, e) =>
                {
                    DownloadPaymentSlipPdf(slipText, reportId);

                    MessageBox.Show(
                        "Receipt Downloaded.",
                        "Receipt",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                };

                btnClose.Click += (s, e) =>
                {
                    slipForm.Close();
                };

                slipForm.Controls.Add(lblTitle);
                slipForm.Controls.Add(pnlSlip);
                slipForm.Controls.Add(btnDownload);
                slipForm.Controls.Add(btnClose);
                slipForm.Shown += (s, e) => slipForm.ActiveControl = btnClose;

                RoundControl(btnDownload, 16);
                RoundControl(btnClose, 16);

                slipForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening payment slip:\n" + ex.Message,
                    "Payment Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Panel CreatePaymentSlipDisplayPanel(string slipText, Point location, Size size)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                TabStop = false
            };

            int y = 12;
            foreach (string rawLine in slipText.Replace("\r", "").Split('\n'))
            {
                string line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("---"))
                    continue;

                bool isHeading = line == line.ToUpperInvariant() && !line.Contains(":");

                if (isHeading)
                {
                    Label heading = new Label
                    {
                        Text = line.Replace("WILDCATHUB ", ""),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(69, 45, 96),
                        Location = new Point(14, y),
                        AutoSize = true
                    };
                    panel.Controls.Add(heading);
                    y += 28;
                    continue;
                }

                int colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {
                    string labelText = line.Substring(0, colonIndex + 1);
                    string valueText = line.Substring(colonIndex + 1).Trim();

                    Label label = new Label
                    {
                        Text = labelText,
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(92, 45, 58),
                        Location = new Point(18, y),
                        Size = new Size(128, 22)
                    };

                    Label value = new Label
                    {
                        Text = valueText,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Location = new Point(150, y),
                        MaximumSize = new Size(size.Width - 180, 0),
                        AutoSize = true
                    };

                    panel.Controls.Add(label);
                    panel.Controls.Add(value);
                    y += Math.Max(24, value.Height + 6);
                }
                else
                {
                    Label note = new Label
                    {
                        Text = line,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Location = new Point(18, y),
                        MaximumSize = new Size(size.Width - 46, 0),
                        AutoSize = true
                    };

                    panel.Controls.Add(note);
                    y += Math.Max(24, note.Height + 6);
                }
            }

            return panel;
        }


        private void DownloadPaymentSlipPdf(string slipText, int reportId)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string receiptFolder = System.IO.Path.Combine(documentsPath, "WildcatHub Receipts");
            System.IO.Directory.CreateDirectory(receiptFolder);

            string fileName = "WildcatHub_Receipt_" + reportId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
            string filePath = System.IO.Path.Combine(receiptFolder, fileName);

            WriteSimpleTextPdf(filePath, slipText);
        }


        private void WriteSimpleTextPdf(string filePath, string text)
        {
            List<string> lines = new List<string>();

            foreach (string sourceLine in text.Replace("\r", "").Split('\n'))
            {
                string line = sourceLine;

                while (line.Length > 88)
                {
                    lines.Add(line.Substring(0, 88));
                    line = line.Substring(88);
                }

                lines.Add(line);
            }

            StringBuilder content = new StringBuilder();
            content.AppendLine("BT");
            content.AppendLine("/F1 10 Tf");
            content.AppendLine("50 790 Td");
            content.AppendLine("14 TL");

            foreach (string line in lines.Take(52))
            {
                content.Append("(");
                content.Append(EscapePdfText(line));
                content.AppendLine(") Tj");
                content.AppendLine("T*");
            }

            content.AppendLine("ET");

            string stream = content.ToString();
            List<string> objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>",
                "<< /Length " + Encoding.ASCII.GetByteCount(stream) + " >>\nstream\n" + stream + "endstream"
            };

            StringBuilder pdf = new StringBuilder();
            List<int> offsets = new List<int>();

            pdf.Append("%PDF-1.4\n");

            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.Append(i + 1);
                pdf.Append(" 0 obj\n");
                pdf.Append(objects[i]);
                pdf.Append("\nendobj\n");
            }

            int xrefOffset = Encoding.ASCII.GetByteCount(pdf.ToString());
            pdf.Append("xref\n");
            pdf.Append("0 ");
            pdf.Append(objects.Count + 1);
            pdf.Append("\n");
            pdf.Append("0000000000 65535 f \n");

            foreach (int offset in offsets)
            {
                pdf.Append(offset.ToString("D10"));
                pdf.Append(" 00000 n \n");
            }

            pdf.Append("trailer\n");
            pdf.Append("<< /Size ");
            pdf.Append(objects.Count + 1);
            pdf.Append(" /Root 1 0 R >>\n");
            pdf.Append("startxref\n");
            pdf.Append(xrefOffset);
            pdf.Append("\n%%EOF");

            System.IO.File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(pdf.ToString()));
        }


        private string EscapePdfText(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }



        private void PrintPaymentSlip(string slipText)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                printDoc.PrintPage += (s, e) =>
                {
                    Font printFont = new Font("Consolas", 10F);
                    RectangleF printArea = e.MarginBounds;

                    e.Graphics.DrawString(
                        slipText,
                        printFont,
                        Brushes.Black,
                        printArea);
                };

                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = printDoc;
                preview.Width = 900;
                preview.Height = 700;
                preview.StartPosition = FormStartPosition.CenterScreen;

                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing payment slip:\n" + ex.Message,
                    "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private Panel CreateReminderCard(
    int reservationId,
    string type,
    string itemName,
    int quantity,
    DateTime targetDate,
    string status,
    Color backColor,
    Color accentColor,
    string details = "")
        {
            Panel card = new Panel
            {
                Width = 335,
                Height = 92,
                BackColor = backColor,
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand,
                Tag = reservationId
            };
            RoundControl(card, 18);

            Label lblTitle = new Label
            {
                Text = itemName,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(14, 12),
                AutoSize = true
            };

            string dateLabel = type == "Reservation"
                ? "Reservation Date"
                : type == "Overdue Borrowed Equipment"
                    ? "Due"
                    : "Received";

            Label lblSub = new Label
            {
                Text = type == "Reservation" || type == "Overdue Borrowed Equipment"
                    ? $"Qty: {quantity} • {dateLabel}: {targetDate:MM/dd/yyyy hh:mm tt}"
                    : $"Qty: {quantity}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(110, 90, 122),
                Location = new Point(14, 38),
                AutoSize = true
            };

            Label lblReceived = new Label
            {
                Text = "Received: " + targetDate.ToString("MM/dd/yyyy hh:mm tt"),
                Font = new Font("Segoe UI", 8.3F),
                ForeColor = Color.FromArgb(110, 90, 122),
                Location = new Point(14, 60),
                AutoSize = true
            };

            Label lblBadge = new Label
            {
                Text = status,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = accentColor,
                BackColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(88, 24),
                Location = new Point(235, 31)
            };
            RoundControl(lblBadge, 12);

            void openDetails(object? sender, EventArgs e)
            {
                ShowReminderDetails(reservationId, type, itemName, quantity, targetDate, status, details);
            }

            card.Click += openDetails;
            lblTitle.Click += openDetails;
            lblSub.Click += openDetails;
            lblReceived.Click += openDetails;
            lblBadge.Click += openDetails;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(lblReceived);
            card.Controls.Add(lblBadge);
            AttachDismissNotificationMenu(card);

            return card;
        }

        private void AttachDismissNotificationMenu(Control card)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete notification");
            deleteItem.Click += (s, e) =>
            {
                Control? parent = card.Parent;
                parent?.Controls.Remove(card);
                card.Dispose();
            };
            menu.Items.Add(deleteItem);
            card.ContextMenuStrip = menu;
            foreach (Control child in card.Controls)
                child.ContextMenuStrip = menu;
        }




        private void ShowReminderDetails(int reservationId, string type, string itemName, int quantity, DateTime targetDate, string status, string details = "")
        {
            string dateLabel = type == "Reservation"
                ? "Reservation Date"
                : type == "Overdue Borrowed Equipment"
                    ? "Due Date"
                    : "Received";

            if (type == "Reservation" && status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                DialogResult result = MessageBox.Show(
                    $"Type: {type}\n" +
                    $"Item: {itemName}\n" +
                    $"Quantity: {quantity}\n" +
                    $"{dateLabel}: {targetDate:MMMM dd, yyyy hh:mm tt}\n" +
                    $"Status: {status}\n\n" +
                    $"Do you want to cancel this reservation?",
                    "Notification Details",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CancelReservation(reservationId);
                }

                return;
            }

            string detailsText = string.IsNullOrWhiteSpace(details)
                ? ""
                : "\nAdmin Message: " + details.Trim();

            MessageBox.Show(
                $"Type: {type}\n" +
                $"Item: {itemName}\n" +
                $"Quantity: {quantity}\n" +
                $"{dateLabel}: {targetDate:MMMM dd, yyyy hh:mm tt}\n" +
                $"Status: {status}" +
                detailsText,
                "Notification Details",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }






        private int GetScalarCount(OleDbConnection conn, string query, params object[] parameters)
        {
            using var cmd = new OleDbCommand(query, conn);

            foreach (var p in parameters)
                cmd.Parameters.AddWithValue("@p", p);

            object? result = cmd.ExecuteScalar();

            if (result == DBNull.Value || result == null)
                return 0;

            return Convert.ToInt32(result);
        }

        private int GetUserOverdueSlipCount(OleDbConnection conn)
        {
            string query = @"
SELECT
    BS.SlipID,
    BS.DateCreated,
    SS.EndTime
FROM ((BorrowSlips AS BS
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
WHERE BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'
AND
(
    BS.UserID = ?
    OR BS.SlipID IN
    (
        SELECT SlipID
        FROM BorrowSlipMembers
        WHERE UserID = ?
    )
)
GROUP BY BS.SlipID, BS.DateCreated, SS.EndTime";

            int count = 0;
            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.UserID;
            cmd.Parameters.Add("@p2", OleDbType.Integer).Value = SessionManager.UserID;

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                DateTime borrowDate = reader["DateCreated"] != DBNull.Value
                    ? Convert.ToDateTime(reader["DateCreated"])
                    : DateTime.Now;

                DateTime dueDate = BuildUserDueDate(borrowDate, reader["EndTime"]);
                if (DateTime.Now > dueDate)
                    count++;
            }

            return count;
        }

        private DateTime BuildUserDueDate(DateTime borrowDate, object endTimeValue)
        {
            if (endTimeValue == DBNull.Value || endTimeValue == null)
                return borrowDate.Date.AddHours(23).AddMinutes(59);

            DateTime endTime = Convert.ToDateTime(endTimeValue);
            return borrowDate.Date.Add(endTime.TimeOfDay);
        }


        private void linkChangePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form changeForm = new Form
            {
                Text = "Change Password",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(460, 430),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(250, 245, 247)
            };

            Label lblTitle = new Label
            {
                Text = "Change Password",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(28, 20),
                AutoSize = true
            };

            Label lblCurrent = new Label
            {
                Text = "Current Password",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 72),
                AutoSize = true
            };

            TextBox txtCurrent = new TextBox
            {
                Location = new Point(30, 98),
                Size = new Size(370, 28),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true
            };

            Label lblNew = new Label
            {
                Text = "New Password",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 140),
                AutoSize = true
            };

            TextBox txtNew = new TextBox
            {
                Location = new Point(30, 166),
                Size = new Size(370, 28),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true
            };

            Label lblConfirm = new Label
            {
                Text = "Confirm New Password",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 208),
                AutoSize = true
            };

            TextBox txtConfirm = new TextBox
            {
                Location = new Point(30, 234),
                Size = new Size(370, 28),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true
            };

            CheckBox chkShowPasswords = new CheckBox
            {
                Text = "Show Passwords",
                Location = new Point(30, 275),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(72, 53, 84),
                BackColor = Color.Transparent
            };

            chkShowPasswords.CheckedChanged += (s, ev) =>
            {
                bool hide = !chkShowPasswords.Checked;
                txtCurrent.UseSystemPasswordChar = hide;
                txtNew.UseSystemPasswordChar = hide;
                txtConfirm.UseSystemPasswordChar = hide;
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(120, 40),
                Location = new Point(150, 320),
                BackColor = Color.FromArgb(214, 197, 224),
                ForeColor = Color.FromArgb(87, 60, 99),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            Button btnSave = new Button
            {
                Text = "Save",
                Size = new Size(120, 40),
                Location = new Point(280, 320),
                BackColor = Color.FromArgb(156, 119, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel.Click += (s, ev) => changeForm.Close();

            btnSave.Click += (s, ev) =>
            {
                string currentPassword = txtCurrent.Text.Trim();
                string newPassword = txtNew.Text.Trim();
                string confirmPassword = txtConfirm.Text.Trim();

                if (string.IsNullOrWhiteSpace(currentPassword) ||
                    string.IsNullOrWhiteSpace(newPassword) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Please complete all fields.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("New password and confirm password do not match.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newPassword == currentPassword)
                {
                    MessageBox.Show("New password must be different from the current password.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool changed = ChangeCurrentUserPassword(currentPassword, newPassword);

                if (changed)
                {
                    MessageBox.Show("Password changed successfully.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    changeForm.Close();
                }
            };

            changeForm.Controls.Add(lblTitle);
            changeForm.Controls.Add(lblCurrent);
            changeForm.Controls.Add(txtCurrent);
            changeForm.Controls.Add(lblNew);
            changeForm.Controls.Add(txtNew);
            changeForm.Controls.Add(lblConfirm);
            changeForm.Controls.Add(txtConfirm);
            changeForm.Controls.Add(chkShowPasswords);
            changeForm.Controls.Add(btnCancel);
            changeForm.Controls.Add(btnSave);

            changeForm.ShowDialog(this);
        }


        private bool ChangeCurrentUserPassword(string currentPassword, string newPassword)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE UserID = ? AND [Password] = ?";
                using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@p1", SessionManager.UserID);
                    checkCmd.Parameters.AddWithValue("@p2", currentPassword);

                    object? result = checkCmd.ExecuteScalar();
                    int count = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;

                    if (count == 0)
                    {
                        MessageBox.Show("Current password is incorrect.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                string updateQuery = "UPDATE Users SET [Password] = ? WHERE UserID = ?";
                using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@p1", newPassword);
                    updateCmd.Parameters.AddWithValue("@p2", SessionManager.UserID);

                    int rows = updateCmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing password:\n" + ex.Message, "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        private void InitializeEquipmentChart()
        {
            chartPlaceholder.Visible = false;

            chartEquipmentUsage = new Chart();
            chartEquipmentUsage.Name = "chartEquipmentUsage";
            chartEquipmentUsage.Location = new Point(22, 86);
            chartEquipmentUsage.Size = new Size(536, 220);
            chartEquipmentUsage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chartEquipmentUsage.BackColor = Color.FromArgb(241, 233, 245);

            pnlStatistics.Controls.Add(chartEquipmentUsage);
            chartEquipmentUsage.BringToFront();
        }


        private Panel CreateEquipmentCard(
    int equipmentId,
    string equipmentName,
    string category,
    int totalQuantity,
    int availableQuantity,
    string status,
    string imagePath)
        {
            Panel card = new Panel
            {
                BackColor = Color.FromArgb(255, 251, 252),
                Width = 238,
                Height = 292,
                Margin = new Padding(18, 14, 18, 18),
                Cursor = Cursors.Hand
            };

            RoundControl(card, 18);
            ApplyEquipmentCardStyle(card);

            PictureBox pic = new PictureBox
            {
                Location = new Point(20, 18),
                Size = new Size(198, 150),
                BackColor = Color.FromArgb(243, 236, 245),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    using Image temp = Image.FromStream(fs);
                    pic.Image = new Bitmap(temp);
                }
                catch
                {
                    pic.Image = null;
                }
            }

            Label lblName = new Label
            {
                Text = equipmentName,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(20, 178),
                AutoSize = false,
                Width = 198,
                Height = 22,
                BackColor = Color.Transparent
            };

            Label lblCategory = new Label
            {
                Text = category,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(20, 202),
                AutoSize = false,
                Width = 198,
                Height = 20,
                BackColor = Color.Transparent
            };

            Label lblAvailable = new Label
            {
                Text = $"Available: {availableQuantity}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(20, 225),
                AutoSize = false,
                Width = 198,
                Height = 20,
                BackColor = Color.Transparent
            };

            Button btnReserve = new Button
            {
                Text = availableQuantity > 0 ? "Add" : "Unavailable",
                Size = new Size(198, 32),
                Location = new Point(20, 250),
                BackColor = availableQuantity > 0
                    ? Color.FromArgb(169, 215, 159)
                    : Color.FromArgb(190, 190, 190),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Enabled = availableQuantity > 0
            };

            btnReserve.FlatAppearance.BorderSize = 0;
            ApplyActionButtonStyle(btnReserve);

            btnReserve.Click += (s, e) =>
            {
                ShowReserveDialog(equipmentId, equipmentName, availableQuantity);
            };

            void openDetails(object? s, EventArgs e)
            {
                ShowUserEquipmentDetails(equipmentId, availableQuantity);
            }

            void cardMouseDown(object? s, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedEquipmentCards.Add(card);
                    card.Invalidate();
                }
            }

            void cardMouseUp(object? s, EventArgs e)
            {
                pressedEquipmentCards.Remove(card);
                card.Invalidate();
            }

            pic.MouseDown += cardMouseDown;
            lblName.MouseDown += cardMouseDown;
            lblCategory.MouseDown += cardMouseDown;
            lblAvailable.MouseDown += cardMouseDown;

            pic.MouseUp += cardMouseUp;
            lblName.MouseUp += cardMouseUp;
            lblCategory.MouseUp += cardMouseUp;
            lblAvailable.MouseUp += cardMouseUp;

            pic.MouseLeave += cardMouseUp;
            lblName.MouseLeave += cardMouseUp;
            lblCategory.MouseLeave += cardMouseUp;
            lblAvailable.MouseLeave += cardMouseUp;

            card.Click += openDetails;
            pic.Click += openDetails;
            lblName.Click += openDetails;
            lblCategory.Click += openDetails;
            lblAvailable.Click += openDetails;

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblCategory);
            card.Controls.Add(lblAvailable);
            card.Controls.Add(btnReserve);

            return card;
        }

        private void ShowUserEquipmentDetails(int equipmentId, int availableQuantity)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT EquipmentName, Brand, Category, Description, ImagePath
FROM Equipment
WHERE EquipmentID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", equipmentId);

                using OleDbDataReader reader = cmd.ExecuteReader();
                if (reader == null || !reader.Read())
                    return;

                Form detailsForm = new Form
                {
                    Text = "Equipment Details",
                    StartPosition = FormStartPosition.CenterParent,
                    Size = new Size(440, 560),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(250, 245, 247)
                };

                string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                string brand = reader["Brand"]?.ToString() ?? "";
                string category = reader["Category"]?.ToString() ?? "";
                string description = reader["Description"]?.ToString() ?? "";
                string imagePath = reader["ImagePath"]?.ToString() ?? "";

                PictureBox pic = new PictureBox
                {
                    Location = new Point(145, 28),
                    Size = new Size(150, 150),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    using FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    using Image temp = Image.FromStream(fs);
                    pic.Image = new Bitmap(temp);
                }

                Label lblName = new Label
                {
                    Text = equipmentName,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(55, 184),
                    Size = new Size(330, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label lblInfo = new Label
                {
                    Text = "Brand: " + (string.IsNullOrWhiteSpace(brand) ? "N/A" : brand) + "\n" +
                           "Category: " + (string.IsNullOrWhiteSpace(category) ? "N/A" : category) + "\n" +
                           "Available: " + availableQuantity,
                    Font = new Font("Segoe UI", 10.5F),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(55, 226),
                    Size = new Size(330, 78)
                };

                Label lblDescTitle = new Label
                {
                    Text = "Details",
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(55, 322),
                    AutoSize = true
                };

                TextBox txtDescription = new TextBox
                {
                    Text = string.IsNullOrWhiteSpace(description) ? "No description added." : description,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(55, 350),
                    Size = new Size(330, 86),
                    Multiline = true,
                    ReadOnly = true,
                    TabStop = false,
                    HideSelection = true,
                    BackColor = Color.White
                };

                Button btnClose = new Button
                {
                    Text = "Close",
                    Size = new Size(120, 36),
                    Location = new Point(265, 455),
                    BackColor = Color.FromArgb(212, 168, 45),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) => detailsForm.Close();

                detailsForm.Controls.Add(pic);
                detailsForm.Controls.Add(lblName);
                detailsForm.Controls.Add(lblInfo);
                detailsForm.Controls.Add(lblDescTitle);
                detailsForm.Controls.Add(txtDescription);
                detailsForm.Controls.Add(btnClose);
                detailsForm.Shown += (s, e) =>
                {
                    txtDescription.SelectionStart = 0;
                    txtDescription.SelectionLength = 0;
                    detailsForm.ActiveControl = btnClose;
                };

                detailsForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading equipment details:\n" + ex.Message);
            }
        }


        private void ShowBorrowedPopup(
    int borrowId,
    string equipmentName,
    int quantityBorrowed,
    DateTime borrowDate,
    DateTime dueDate,
    decimal penaltyAmount)
        {
            selectedBorrowId = borrowId;

            lblPopupItemName.Text = equipmentName;

            lblPopupQuantityTitle.Text = "Borrowed Quantity";
            lblPopupBorrowedDateTitle.Text = "Borrowed On";
            lblPopupDueDateTitle.Text = "Due Back";
            lblPopupPenaltyTitle.Text = "Current Penalty";

            lblPopupQuantityValue.Text = quantityBorrowed.ToString();
            lblPopupBorrowedDateValue.Text = borrowDate.ToString("MMM dd, yyyy");
            lblPopupDueDateValue.Text = dueDate.ToString("MMM dd, yyyy");

            decimal displayPenalty = penaltyAmount;

            if (DateTime.Now.Date > dueDate.Date)
            {
                int overdueDays = (DateTime.Now.Date - dueDate.Date).Days;
                if (overdueDays < 0) overdueDays = 0;
                displayPenalty = overdueDays * 10m;
            }

            lblPopupPenaltyValue.Text = $"₱ {displayPenalty:0.00}";

            if (DateTime.Now.Date > dueDate.Date)
                lblPopupPenaltyValue.ForeColor = Color.FromArgb(220, 95, 107);
            else
                lblPopupPenaltyValue.ForeColor = Color.FromArgb(220, 140, 92);

            lblPopupDueDateValue.ForeColor = DateTime.Now.Date > dueDate.Date
                ? Color.FromArgb(220, 95, 107)
                : Color.FromArgb(72, 53, 84);

            pnlBorrowedEmptyState.Visible = false;
            flowBorrowedItems.Visible = true;
            pnlBorrowedPopup.Visible = true;
            pnlBorrowedPopup.BringToFront();
        }



        private bool HasUnpaidDamageRestriction(int userId, int subjectId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int labId = 0;

                string getLabQuery = @"
SELECT LabID
FROM LabSubjects
WHERE SubjectID = ?";

                using (OleDbCommand labCmd = new OleDbCommand(getLabQuery, conn))
                {
                    labCmd.Parameters.AddWithValue("@p1", subjectId);

                    object result = labCmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return false;

                    labId = Convert.ToInt32(result);
                }

                string query = @"
SELECT COUNT(*)
FROM ((DamageReportMembers AS DRM
INNER JOIN DamageReports AS DR ON DRM.ReportID = DR.ReportID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
WHERE DRM.UserID = ?
AND DR.ReportStatus = 'For Payment'
AND LS.LabID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", userId);
                cmd.Parameters.AddWithValue("@p2", labId);

                object countResult = cmd.ExecuteScalar();

                int count = countResult != null && countResult != DBNull.Value
                    ? Convert.ToInt32(countResult)
                    : 0;

                return count > 0;
            }
            catch
            {
                return false;
            }
        }



        private string GetBorrowedSerialNumbers(int slipItemId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT EU.SerialNumber
FROM BorrowSlipUnits AS BSU
INNER JOIN EquipmentUnits AS EU
ON BSU.UnitID = EU.UnitID
WHERE BSU.SlipItemID = ?
ORDER BY EU.SerialNumber";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", slipItemId);

                List<string> serials = new List<string>();

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    string serial =
                        reader["SerialNumber"]?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(serial))
                    {
                        serials.Add(serial);
                    }
                }

                if (serials.Count == 0)
                    return "N/A";

                return string.Join(", ", serials);
            }
            catch
            {
                return "N/A";
            }
        }

    }
}
