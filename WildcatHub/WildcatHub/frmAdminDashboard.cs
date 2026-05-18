using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WildcatHub;
using System.Drawing.Printing;

namespace WildcatHub
{
    public partial class frmAdminDashboard : Form
    {
        private Button btnAddEquipmentDynamic = null!;
        private Button btnRefreshEquipmentDynamic = null!;
        private string currentEquipmentCategory = "All";
        private string currentReservationFilter = "Pending";
        private string currentReservationSearch = "";
        private string currentReservationSubjectFilter = "All Subjects";
        private string currentReservationSectionFilter = "All Sections";
        private bool loadingReservationFilters = false;
        private string currentEquipmentSubject = "All Subjects";
        private bool equipmentAvailableExpanded = true;
        private bool equipmentArchivedExpanded = true;
        private Chart chartSlipStatus = null!;
        private Label lblUsageHoverValue = null!;
        private int hoveredWorkloadSlice = -1;
        private int hoveredUsageBar = -1;
        private readonly HashSet<Button> pressedButtons = new();
        private readonly HashSet<Button> styledButtons = new();
        private readonly HashSet<Panel> styledPanels = new();
        private Panel pnlExperimentManualAdmin;
        private ComboBox cmbManualSubject;
        private TextBox txtManualExperimentName;
        private ComboBox cmbManualEquipment;
        private NumericUpDown numManualQty;
        private Button btnManualAddEquipment;
        private Button btnManualSave;
        private FlowLayoutPanel flowManualItems;
        private FlowLayoutPanel flowManualList;
        private bool experimentManualInitialized = false;
        private FlowLayoutPanel flowManualEquipmentCards;
        private Button btnReportDamageDynamic = null!;
        private Button btnNavReportsDynamic = null!;
        private Panel pnlDamageReportsAdmin = null!;
        private FlowLayoutPanel flowDamageReportList = null!;
        private Panel pnlBorrowedReportsDrawer = null!;
        private FlowLayoutPanel flowBorrowedReportsList = null!;
        private ComboBox cmbBorrowedReportFilter = null!;
        private Button btnOpenBorrowedReportsDrawer = null!;
        private Button btnCloseBorrowedReportsDrawer = null!;
        private bool isBorrowedReportsDrawerOpen = false;
        private string selectedReportEvidenceImage = "";
        private string currentBorrowedSearch = "";
        private string currentBorrowedFilter = "All";

        private List<ManualAdminItem> manualItems = new List<ManualAdminItem>();

        private class ManualAdminItem
        {
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public int QuantityNeeded { get; set; }
        }

        private class DashboardStockItem
        {
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public int QuantityTotal { get; set; }
            public int QuantityMaintenance { get; set; }
            public int LowStockThreshold { get; set; }
            public string EquipmentType { get; set; } = "Reusable";
            public bool HasSerial { get; set; }
        }

        private class EquipmentCardItem
        {
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public string Category { get; set; } = "";
            public int QuantityTotal { get; set; }
            public int QuantityMaintenance { get; set; }
            public string Status { get; set; } = "";
            public string ImagePath { get; set; } = "";
            public string EquipmentType { get; set; } = "Reusable";
            public bool HasSerial { get; set; }
            public bool IsArchived { get; set; }
        }

        public frmAdminDashboard()
        {
            InitializeComponent();
            EnableEquipmentRefreshSmoothing();
            LoadEquipmentCards(currentEquipmentCategory);
            ApplyButtonStyle(btnNavDashboard);
            ApplyButtonStyle(btnNavVerification);
            ApplyButtonStyle(btnNavEquipment);
            ApplyButtonStyle(btnNavBorrowed);
            ApplyButtonStyle(btnNavReservations);
            ApplyButtonStyle(btnNavHistory);
            ApplyButtonStyle(btnNavExperimentManuals);
            ApplyButtonStyle(btnLogout);

            ApplyButtonStyle(btnEqAll);
            ApplyButtonStyle(btnEqTechnical);
            ApplyButtonStyle(btnEqScience);
            ApplyButtonStyle(btnEqSports);
            ApplyButtonStyle(btnEqGeneral);

            ApplyButtonStyle(btnClaim);
            ApplyButtonStyle(btnUnclaimed);
            ApplyButtonStyle(btnReturn);

            WireEvents();
            ConfigureGrids();
            ConfigureAccountsDetailsList();
            ConfigureStudentDetailCards();

            dgvPendingUsers.CellPainting += dgvPendingUsers_CellPainting;
            dgvBorrowed.CellPainting += dgvBorrowed_CellPainting;

            ShowDashboardPanel();
            ApplyRoundedUi();
            InitializeEquipmentAdminButtons();
            InitializeBorrowedAdminButtons();
            // InitializeReportsNavButton();
            InitializeReservationsSearch();

            RoundControl(cardVerified, 24);
            RoundControl(cardPending, 24);
            RoundControl(cardBorrowed, 24);
            RoundControl(cardOverdue, 24);
            RoundControl(cardEquipment, 24);
            RoundControl(pnlClaimableToday, 24);
            RoundControl(pnlRecentActivity, 24);
            RoundControl(pnlStatistics, 24);

            ApplyNeumorphismPanel(cardVerified, 24);
            ApplyNeumorphismPanel(cardPending, 24);
            ApplyNeumorphismPanel(cardBorrowed, 24);
            ApplyNeumorphismPanel(cardOverdue, 24);
            ApplyNeumorphismPanel(cardEquipment, 24);
            ApplyNeumorphismPanel(pnlClaimableToday, 24);
            ApplyNeumorphismPanel(pnlRecentActivity, 24);
            ApplyNeumorphismPanel(pnlStatistics, 24);

            ApplyNeumorphismPanel(pnlStudentBorrowedCard, 20);
            ApplyNeumorphismPanel(pnlStudentReturnedCard, 20);
            ApplyNeumorphismPanel(pnlStudentReservationsCard, 20);

            ApplyNeumorphismPanel(cardResPending, 18);
            ApplyNeumorphismPanel(cardResClaimed, 18);
            ApplyNeumorphismPanel(cardResUnclaimed, 18);
        }

        private void ConfigureStudentDetailCards()
        {
            lblStudentBorrowedTitle.Text = "Borrowing";
            lblStudentReturnedTitle.Text = "Returned";
            lblStudentReservationsTitle.Text = "Due";
        }

        private void EnableEquipmentRefreshSmoothing()
        {
            SetDoubleBuffered(flowEquipmentCards);
            SetDoubleBuffered(pnlEquipmentMain);
        }

        private void SetDoubleBuffered(Control? control)
        {
            if (control == null)
                return;

            typeof(Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }





        private class DamageReportItemChoice
        {
            public int SlipItemID { get; set; }
            public int SlipID { get; set; }
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public int QuantityBorrowed { get; set; }
            public int ReportQuantity { get; set; } = 1;
            public bool HasSerial { get; set; }
            public bool IsQuantityLocked { get; set; }

            public RadioButton Radio { get; set; } = null!;
            public Label QtyLabel { get; set; } = null!;

            public List<int> SelectedUnitIDs { get; set; } = new List<int>();
        }



        private class DamageUnitChoice
        {
            public int UnitID { get; set; }
            public string SerialNumber { get; set; } = "";
            public CheckBox Check { get; set; } = null!;
        }


        private class SerialUnitChoice
        {
            public int UnitID { get; set; }
            public string SerialNumber { get; set; } = "";
            public CheckBox Check { get; set; } = null!;
        }



        private class PendingSlipItemForApproval
        {
            public int SlipItemID { get; set; }
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public int QuantityRequested { get; set; }
            public bool HasSerial { get; set; }
            public List<int> SelectedUnitIDs { get; set; } = new List<int>();
        }

        private static bool CanRequireSerialAssignment(string equipmentType, bool hasSerial)
        {
            if (!hasSerial)
                return false;

            string normalizedType = (equipmentType ?? "").Trim();
            return normalizedType != "Consumable" &&
                   normalizedType != "One Time Use";
        }


        private void LoadEquipmentCards(string categoryFilter, string keyword = "")
        {
            flowEquipmentCards.SuspendLayout();
            try
            {
                flowEquipmentCards.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query;

                bool hasSubjectFilter =
                    currentEquipmentSubject != "All" &&
                    currentEquipmentSubject != "All Subjects";

                if (hasSubjectFilter)
                {
                    query = @"
SELECT DISTINCT
    E.EquipmentID,
    E.EquipmentName,
    E.Category,
    E.QuantityTotal,
    E.QuantityMaintenance,
    E.Status,
    E.ImagePath,
    E.EquipmentType,
    E.HasSerial,
    E.IsArchived
FROM ((Equipment AS E
INNER JOIN SubjectEquipments AS SE ON E.EquipmentID = SE.EquipmentID)
INNER JOIN LabSubjects AS LS ON SE.SubjectID = LS.SubjectID)
WHERE E.LabID = ?
AND E.Status = 'Active'
AND LS.SubjectCode = ?";
                }
                else
                {
                    query = @"
SELECT DISTINCT
    E.EquipmentID,
    E.EquipmentName,
    E.Category,
    E.QuantityTotal,
    E.QuantityMaintenance,
    E.Status,
    E.ImagePath,
    E.EquipmentType,
    E.HasSerial,
    E.IsArchived
FROM Equipment AS E
WHERE E.LabID = ?
AND E.Status = 'Active'";
                }

                if (categoryFilter != "All")
                    query += " AND E.Category = ?";

                if (!string.IsNullOrWhiteSpace(keyword))
                    query += " AND E.EquipmentName LIKE ?";

                query += " ORDER BY E.EquipmentName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);

                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                if (hasSubjectFilter)
                    cmd.Parameters.AddWithValue("@p2", currentEquipmentSubject);

                if (categoryFilter != "All")
                    cmd.Parameters.AddWithValue("@p3", categoryFilter);

                if (!string.IsNullOrWhiteSpace(keyword))
                    cmd.Parameters.AddWithValue("@p4", "%" + keyword + "%");

                List<EquipmentCardItem> equipmentItems = new List<EquipmentCardItem>();

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        equipmentItems.Add(new EquipmentCardItem
                        {
                            EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                            EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                            Category = reader["Category"]?.ToString() ?? "",
                            QuantityTotal = reader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(reader["QuantityTotal"]) : 0,
                            QuantityMaintenance = reader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(reader["QuantityMaintenance"]) : 0,
                            Status = reader["Status"]?.ToString() ?? "",
                            ImagePath = reader["ImagePath"]?.ToString() ?? "",
                            EquipmentType = reader["EquipmentType"] != DBNull.Value ? reader["EquipmentType"].ToString() ?? "Reusable" : "Reusable",
                            HasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"]),
                            IsArchived = reader["IsArchived"] != DBNull.Value && Convert.ToBoolean(reader["IsArchived"])
                        });
                    }
                }

                AddEquipmentSectionHeader("Available", equipmentAvailableExpanded, ToggleAvailableEquipmentSection);

                List<EquipmentCardItem> availableItems = equipmentItems
                    .Where(x => !x.IsArchived)
                    .ToList();

                if (equipmentAvailableExpanded)
                {
                    foreach (EquipmentCardItem item in availableItems)
                    {
                        int available = GetCorrectAvailableQuantity(
                            conn,
                            item.EquipmentID,
                            item.QuantityTotal,
                            item.QuantityMaintenance,
                            item.HasSerial,
                            item.EquipmentType);

                        Panel card = CreateEquipmentCard(
                            item.EquipmentID,
                            item.EquipmentName,
                            item.Category,
                            item.QuantityTotal,
                            available,
                            item.Status,
                            item.ImagePath,
                            false
                        );

                        flowEquipmentCards.Controls.Add(card);
                    }

                    if (availableItems.Count == 0)
                        flowEquipmentCards.Controls.Add(CreateEquipmentEmptyLabel("No available equipment found."));
                }

                AddEquipmentSectionHeader("Archived", equipmentArchivedExpanded, ToggleArchivedEquipmentSection);

                List<EquipmentCardItem> archivedItems = equipmentItems
                    .Where(x => x.IsArchived)
                    .ToList();

                if (equipmentArchivedExpanded)
                {
                    foreach (EquipmentCardItem item in archivedItems)
                    {
                        int available = GetCorrectAvailableQuantity(
                            conn,
                            item.EquipmentID,
                            item.QuantityTotal,
                            item.QuantityMaintenance,
                            item.HasSerial,
                            item.EquipmentType);

                        Panel card = CreateEquipmentCard(
                            item.EquipmentID,
                            item.EquipmentName,
                            item.Category,
                            item.QuantityTotal,
                            available,
                            item.Status,
                            item.ImagePath,
                            true
                        );

                        flowEquipmentCards.Controls.Add(card);
                    }

                    if (archivedItems.Count == 0)
                        flowEquipmentCards.Controls.Add(CreateEquipmentEmptyLabel("No archived equipment."));
                }
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

        private void LoadAdminDashboardNew()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int labId = SessionManager.LabID;
                string todayName = DateTime.Now.DayOfWeek.ToString();

                lblCardVerifiedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM SubjectSchedules AS SS
INNER JOIN LabSubjects AS LS ON SS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND LS.IsActive = True
AND SS.DayOfWeek = ?", labId, todayName).ToString("00");

                lblCardPendingCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM LabSubjects
WHERE LabID = ?
AND IsActive = True", labId).ToString("00");

                lblCardBorrowedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM Users AS U
WHERE U.IsActive = True
AND U.UserID IN
(
    SELECT DISTINCT SSE.UserID
    FROM StudentSubjectEnrollments AS SSE
    INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID
    WHERE SSE.IsActive = True
    AND LS.LabID = ?
)", labId).ToString("00");

                lblCardOverdueCount.Text = GetScalarCount(conn, @"
SELECT SUM(BSI.QuantityRequested - BSI.QuantityReturned)
FROM (BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'", labId).ToString("00");

                int lowStockCount = 0;
                List<DashboardStockItem> stockItems = new List<DashboardStockItem>();
                string lowStockQuery = @"
SELECT EquipmentID, QuantityTotal, QuantityMaintenance,
       LowStockThreshold, EquipmentType, HasSerial, EquipmentName
FROM Equipment
WHERE LabID = ?
AND IsArchived = False
AND Status = 'Active'";

                using (OleDbCommand lowCmd = new OleDbCommand(lowStockQuery, conn))
                {
                    lowCmd.Parameters.AddWithValue("@p1", labId);
                    using OleDbDataReader lowReader = lowCmd.ExecuteReader();

                    while (lowReader != null && lowReader.Read())
                    {
                        stockItems.Add(new DashboardStockItem
                        {
                            EquipmentID = Convert.ToInt32(lowReader["EquipmentID"]),
                            EquipmentName = lowReader["EquipmentName"]?.ToString() ?? "",
                            QuantityTotal = lowReader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(lowReader["QuantityTotal"]) : 0,
                            QuantityMaintenance = lowReader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(lowReader["QuantityMaintenance"]) : 0,
                            LowStockThreshold = lowReader["LowStockThreshold"] != DBNull.Value ? Convert.ToInt32(lowReader["LowStockThreshold"]) : 3,
                            EquipmentType = lowReader["EquipmentType"] != DBNull.Value ? lowReader["EquipmentType"].ToString() ?? "Reusable" : "Reusable",
                            HasSerial = lowReader["HasSerial"] != DBNull.Value && Convert.ToBoolean(lowReader["HasSerial"])
                        });
                    }
                }

                foreach (DashboardStockItem item in stockItems)
                {
                    int available = GetCorrectAvailableQuantity(
                        conn,
                        item.EquipmentID,
                        item.QuantityTotal,
                        item.QuantityMaintenance,
                        item.HasSerial,
                        item.EquipmentType);

                    if (available <= item.LowStockThreshold)
                        lowStockCount++;
                }

                lblCardEquipmentCount.Text = lowStockCount.ToString("00");

                int pendingSlipCount = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Pending'", labId);

                lblResPendingCount.Text = pendingSlipCount.ToString("00");
                lblClaimableValue.Text = pendingSlipCount.ToString("00");

                lblResClaimedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'", labId).ToString("00");

                lblResUnclaimedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Declined'", labId).ToString("00");

                LoadLowStockAlerts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading dashboard:\n" + ex.Message,
                    "Dashboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private void LoadAllAdminData()
        {
            LoadAdminDashboardNew();
        }

        private class DeletedReservationNotice
        {
            public string FullName { get; set; } = "";
            public string SchoolEmail { get; set; } = "";
            public string EquipmentName { get; set; } = "";
            public int QuantityReserved { get; set; }
            public DateTime ReservationDate { get; set; }
        }


        private List<int>? ShowSerialSelectionDialog(
    int equipmentId,
    string equipmentName,
    int quantityNeeded)
        {
            using Form popup = new Form();

            popup.Text = "Assign Serial Numbers";
            popup.Size = new Size(440, 540);
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;
            popup.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label();
            lblTitle.Text = equipmentName;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(24, 20);
            lblTitle.Size = new Size(370, 32);

            Label lblQty = new Label();
            lblQty.Text = "Quantity Needed: " + quantityNeeded;
            lblQty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblQty.ForeColor = Color.FromArgb(92, 45, 58);
            lblQty.Location = new Point(26, 58);
            lblQty.AutoSize = true;

            Label lblGuide = new Label();
            lblGuide.Text = "Select the actual serial/unit number(s) you physically issued.";
            lblGuide.Font = new Font("Segoe UI", 9F);
            lblGuide.ForeColor = Color.FromArgb(126, 105, 136);
            lblGuide.Location = new Point(26, 82);
            lblGuide.Size = new Size(360, 34);

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Location = new Point(24, 125);
            flow.Size = new Size(370, 300);
            flow.FlowDirection = FlowDirection.TopDown;
            flow.WrapContents = false;
            flow.AutoScroll = true;
            flow.BackColor = Color.White;
            flow.BorderStyle = BorderStyle.FixedSingle;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(110, 38);
            btnCancel.Location = new Point(165, 445);
            btnCancel.BackColor = Color.FromArgb(214, 197, 224);
            btnCancel.ForeColor = Color.FromArgb(87, 60, 99);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            Button btnAssign = new Button();
            btnAssign.Text = "Assign";
            btnAssign.Size = new Size(110, 38);
            btnAssign.Location = new Point(285, 445);
            btnAssign.BackColor = Color.FromArgb(212, 168, 45);
            btnAssign.ForeColor = Color.White;
            btnAssign.FlatStyle = FlatStyle.Flat;
            btnAssign.FlatAppearance.BorderSize = 0;
            btnAssign.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            List<SerialUnitChoice> choices = new List<SerialUnitChoice>();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
SELECT UnitID, SerialNumber
FROM EquipmentUnits
WHERE EquipmentID = ?
AND UnitStatus = 'Available'
ORDER BY SerialNumber";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            using OleDbDataReader reader = cmd.ExecuteReader();

            while (reader != null && reader.Read())
            {
                int unitId = Convert.ToInt32(reader["UnitID"]);
                string serial = reader["SerialNumber"]?.ToString() ?? "";

                CheckBox chk = new CheckBox();
                chk.Text = serial;
                chk.Font = new Font("Segoe UI", 10F);
                chk.AutoSize = true;
                chk.Margin = new Padding(10, 8, 8, 0);

                flow.Controls.Add(chk);

                choices.Add(new SerialUnitChoice
                {
                    UnitID = unitId,
                    SerialNumber = serial,
                    Check = chk
                });
            }

            List<int>? selectedUnitIds = null;

            btnAssign.Click += (s, e) =>
            {
                selectedUnitIds = choices
                    .Where(x => x.Check.Checked)
                    .Select(x => x.UnitID)
                    .ToList();

                if (selectedUnitIds.Count != quantityNeeded)
                {
                    MessageBox.Show(
                        "Please select exactly " + quantityNeeded + " serial number(s).",
                        "Serial Assignment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                popup.DialogResult = DialogResult.OK;
                popup.Close();
            };

            btnCancel.Click += (s, e) =>
            {
                selectedUnitIds = null;
                popup.DialogResult = DialogResult.Cancel;
                popup.Close();
            };

            popup.Controls.Add(lblTitle);
            popup.Controls.Add(lblQty);
            popup.Controls.Add(lblGuide);
            popup.Controls.Add(flow);
            popup.Controls.Add(btnCancel);
            popup.Controls.Add(btnAssign);

            RoundControl(btnCancel, 14);
            RoundControl(btnAssign, 14);

            return popup.ShowDialog(this) == DialogResult.OK
                ? selectedUnitIds
                : null;
        }



        private int CountAvailableSerialUnits(int equipmentId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

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


        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            lblAdminTitle.Text = "WildcatHub";
            lblWelcome.Text = "Dashboard";

            btnNavVerification.Text = "👥  Students";
            btnNavReservations.Text = "📝  Slips";
            btnReturn.Text = "Return";
            btnNavExperimentManuals.Visible = false;
            if (btnNavReportsDynamic != null)
            {
                btnNavReportsDynamic.Visible = false;
            }

            btnEqTechnical.Text = "Chemical";
            btnEqScience.Text = "Mechanical";
            btnEqSports.Text = "Civil";
            btnEqGeneral.Text = "General";
            btnEqGeneral.Visible = true;

            lblRecentActivityHeader.Text = "Low Stock Alerts";
            lblRecentActivityEmpty.Text = "No low-stock equipment.";
            btnClaim.Text = "Approve";
            btnUnclaimed.Text = "Decline";
            lblCardVerifiedText.Text = "Classes Today";
            lblCardPendingText.Text = "Total Subjects";
            lblCardBorrowedText.Text = "Total Students";
            lblCardOverdueText.Text = "Borrowing Items";
            lblCardEquipmentText.Text = "Low Stock Items";

            lblStatisticsHeader.Text = "Dashboard Charts";
            lblStatisticsSub.Text = "Workload share and top returned equipment";
            lblRecentActivityHeader.Text = "Notifications";
            lblRecentActivityEmpty.Text = "No dashboard notifications.";

            lblStudentBorrowedTitle.Text = "Borrowing";
            lblStudentReturnedTitle.Text = "Returned";
            lblStudentReservationsTitle.Text = "Due";


            lblClaimableHeader.Text = "Pending Slip Requests";
            lblClaimableSub.Text = "Borrower slips waiting for approval";

            Label lblStudentSubjectFilter = new Label();
            lblStudentSubjectFilter.Name = "lblStudentSubjectFilter";
            lblStudentSubjectFilter.Text = "Subject";
            lblStudentSubjectFilter.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStudentSubjectFilter.ForeColor = Color.FromArgb(69, 45, 96);
            lblStudentSubjectFilter.Location = new Point(cmbSubjectFilter.Left, cmbSubjectFilter.Top - 20);
            lblStudentSubjectFilter.AutoSize = true;

            Label lblStudentSectionFilter = new Label();
            lblStudentSectionFilter.Name = "lblStudentSectionFilter";
            lblStudentSectionFilter.Text = "Schedule / Section";
            lblStudentSectionFilter.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStudentSectionFilter.ForeColor = Color.FromArgb(69, 45, 96);
            lblStudentSectionFilter.Location = new Point(cmbSectionFilter.Left, cmbSectionFilter.Top - 20);
            lblStudentSectionFilter.AutoSize = true;

            if (!pnlPendingList.Controls.ContainsKey("lblStudentSubjectFilter"))
                pnlPendingList.Controls.Add(lblStudentSubjectFilter);

            if (!pnlPendingList.Controls.ContainsKey("lblStudentSectionFilter"))
                pnlPendingList.Controls.Add(lblStudentSectionFilter);
            LoadAdminDashboardNew();
            LoadSubjectFilter();
            LoadEquipmentSubjectFilter();
        }



        private void LoadSubjectFilter()
        {
            try
            {
                cmbSubjectFilter.Items.Clear();
                cmbSubjectFilter.Items.Add("All");

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT SubjectCode
FROM LabSubjects
WHERE LabID = ?
AND IsActive = True
ORDER BY SubjectCode";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    cmbSubjectFilter.Items.Add(reader["SubjectCode"]?.ToString() ?? "");
                }

                cmbSubjectFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subjects:\n" + ex.Message);
            }
        }



        private void frmAdminDashboard_Resize(object sender, EventArgs e)
        {
            cardVerified.Invalidate();
            cardPending.Invalidate();
            cardBorrowed.Invalidate();
            cardOverdue.Invalidate();
            cardEquipment.Invalidate();
            pnlClaimableToday.Invalidate();
            pnlRecentActivity.Invalidate();
            pnlStatistics.Invalidate();
        }

        

        private void WireEvents()
        {
            txtVerifiedSearch.TextChanged += txtVerifiedSearch_TextChanged;
            dgvPendingUsers.CellContentClick += dgvPendingUsers_CellContentClick;

            btnClaim.Click += btnClaim_Click;
            btnUnclaimed.Click += btnUnclaimed_Click;
            btnNavReservations.Text = "📝  Slips";

            btnEqAll.Click += btnEqAll_Click;
            btnEqTechnical.Click += btnEqTechnical_Click;
            btnEqScience.Click += btnEqScience_Click;
            btnEqSports.Click += btnEqSports_Click;
            btnEqGeneral.Click += btnEqGeneral_Click;

            dgvBorrowed.CellDoubleClick += dgvBorrowed_CellDoubleClick;
            dgvBorrowed.CellClick += dgvBorrowed_CellClick;
            dgvBorrowed.RowPrePaint += dgvBorrowed_RowPrePaint;
            dgvBorrowed.SelectionChanged += dgvBorrowed_SelectionChanged;
            dgvHistory.CellClick += dgvHistory_CellClick;
            dgvHistory.RowPrePaint += dgvHistory_RowPrePaint;
            dgvPendingUsers.SelectionChanged += dgvPendingUsers_SelectionChanged;
            dgvPendingUsers.CellFormatting += dgvPendingUsers_CellFormatting;
            cardResPending.Click += cardResPending_Click;
            lblResPendingTitle.Click += cardResPending_Click;
            lblResPendingCount.Click += cardResPending_Click;

            cardResClaimed.Click += cardResClaimed_Click;
            lblResClaimedTitle.Click += cardResClaimed_Click;
            lblResClaimedCount.Click += cardResClaimed_Click;
            lblResShowAll.Click += lblResShowAll_Click;
            cardResUnclaimed.Click += cardResUnclaimed_Click;
            lblResUnclaimedTitle.Click += cardResUnclaimed_Click;
            lblResUnclaimedCount.Click += cardResUnclaimed_Click;
        }



        private void txtStudentSearch_TextChanged(object sender, EventArgs e)
        {
            LoadAccountsData();
        }

        private void cmbSubjectFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSectionsForSelectedSubject();
            LoadAccountsData();
        }

        private void cmbSectionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAccountsData();
        }


        private void LoadSectionsForSelectedSubject()
        {
            try
            {
                cmbSectionFilter.Items.Clear();
                cmbSectionFilter.Items.Add(new ScheduleFilterItem
                {
                    ScheduleID = 0,
                    DisplayText = "All"
                });

                if (cmbSubjectFilter.SelectedItem == null ||
                    cmbSubjectFilter.SelectedItem.ToString() == "All")
                {
                    cmbSectionFilter.SelectedIndex = 0;
                    return;
                }

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT 
    SS.ScheduleID,
    SS.Section,
    SS.DayOfWeek,
    SS.StartTime,
    SS.EndTime
FROM SubjectSchedules AS SS
INNER JOIN LabSubjects AS LS ON SS.SubjectID = LS.SubjectID
WHERE LS.SubjectCode = ?
AND LS.LabID = ?
ORDER BY SS.Section";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", cmbSubjectFilter.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@p2", SessionManager.LabID);

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    int scheduleId = Convert.ToInt32(reader["ScheduleID"]);
                    string section = reader["Section"]?.ToString() ?? "";

                    string day = reader["DayOfWeek"]?.ToString() ?? "";
                    string dayShort = day.Length >= 3 ? day.Substring(0, 3) : day;

                    DateTime startTime = Convert.ToDateTime(reader["StartTime"]);
                    DateTime endTime = Convert.ToDateTime(reader["EndTime"]);

                    string display = section + " [" +
                                     dayShort + " " +
                                     startTime.ToString("h:mm tt") + "-" +
                                     endTime.ToString("h:mm tt") + "]";

                    cmbSectionFilter.Items.Add(new ScheduleFilterItem
                    {
                        ScheduleID = scheduleId,
                        DisplayText = display
                    });
                }

                cmbSectionFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sections:\n" + ex.Message);
            }
        }


        private void dgvBorrowed_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                using (Pen headerPen = new Pen(Color.FromArgb(215, 205, 230), 1))
                {
                    e.Graphics.DrawLine(
                        headerPen,
                        e.CellBounds.Left,
                        e.CellBounds.Bottom - 1,
                        e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }

                e.Handled = true;
                return;
            }

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            using (Pen rowPen = new Pen(Color.FromArgb(228, 220, 238), 1))
            {
                e.Graphics.DrawLine(
                    rowPen,
                    e.CellBounds.Left + 6,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right - 6,
                    e.CellBounds.Bottom - 1);
            }

            e.Handled = true;
        }

        

        private void ApplyNeumorphismPanel(Panel panel, int radius = 24)
        {
            if (styledPanels.Contains(panel))
                return;

            styledPanels.Add(panel);

            panel.Paint += (s, e) =>
            {
                if (s is not Panel p) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                using GraphicsPath path = GetRoundedPath(rect, radius);

                using SolidBrush fillBrush = new SolidBrush(p.BackColor);
                e.Graphics.FillPath(fillBrush, path);

                using Pen lightPen = new Pen(Color.FromArgb(255, 255, 255), 3);
                using Pen darkPen = new Pen(Color.FromArgb(150, 120, 170), 3);  

                // top-left highlight
                e.Graphics.DrawArc(lightPen, 1, 1, 20, 20, 180, 90);
                e.Graphics.DrawLine(lightPen, 11, 1, rect.Width - 12, 1);
                e.Graphics.DrawLine(lightPen, 1, 11, 1, rect.Height - 12);

                // bottom-right shadow
                e.Graphics.DrawArc(darkPen, rect.Width - 21, rect.Height - 21, 20, 20, 0, 90);
                e.Graphics.DrawLine(darkPen, 11, rect.Height - 1, rect.Width - 12, rect.Height - 1);
                e.Graphics.DrawLine(darkPen, rect.Width - 1, 11, rect.Width - 1, rect.Height - 12);
            };
        }






        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
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


        private void ApplyRoundedUi()
        {
            RoundControl(btnNavDashboard, 18);
            RoundControl(btnNavVerification, 18);
            RoundControl(btnNavEquipment, 18);
            RoundControl(btnNavBorrowed, 18);
            RoundControl(btnNavReservations, 18);
            RoundControl(btnNavHistory, 18);
            RoundControl(btnNavExperimentManuals, 18);
            RoundControl(btnLogout, 18);

            RoundControl(cardVerified, 28);
            RoundControl(cardPending, 28);
            RoundControl(cardBorrowed, 28);
            RoundControl(cardOverdue, 28);
            RoundControl(cardEquipment, 28);

            RoundControl(pnlStatistics, 28);
            RoundControl(pnlRecentActivity, 28);

            RoundControl(vCardVerified, 28);
            RoundControl(vCardPending, 28);
            RoundControl(vCardRejected, 28);
            RoundControl(pnlPendingList, 28);
            RoundControl(pnlVerifiedSearch, 28);

            RoundControl(pnlEquipmentMain, 28);
            RoundControl(pnlBorrowedMain, 28);
            RoundControl(pnlReservationsMain, 28);
            RoundControl(pnlHistoryMain, 28);

            RoundControl(cardResPending, 18);
            RoundControl(cardResClaimed, 18);
            RoundControl(cardResUnclaimed, 18);

            RoundControl(btnEqAll, 18);
            RoundControl(btnEqTechnical, 18);
            RoundControl(btnEqScience, 18);
            RoundControl(btnEqSports, 18);
            RoundControl(btnEqGeneral, 18);
            RoundControl(pnlClaimableToday, 28);
            RoundControl(btnClaim, 18);
            RoundControl(btnUnclaimed, 18);

            RoundControl(pnlStudentBorrowedCard, 20);
            RoundControl(pnlStudentReturnedCard, 20);
            RoundControl(pnlStudentReservationsCard, 20);
        }



        private void RoundControl(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

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


        private void lblResShowAll_Click(object? sender, EventArgs e)
        {
            currentReservationFilter = "All";
            LoadReservationsData();
        }



        private void ShowDashboardPanel()
        {
            LoadAllAdminData();
            LoadBorrowedChart();

            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            panelDashboard.Visible = true;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            panelDashboard.BringToFront();

            lblWelcome.Text = "Dashboard";
            ResetSidebarButtons();
            SetActiveButton(btnNavDashboard);
        }



        private void ShowVerificationPanel()
        {
            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            panelDashboard.Visible = false;
            panelVerification.Visible = true;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            panelVerification.BringToFront();

            lblWelcome.Text = "Students";

            ResetSidebarButtons();
            SetActiveButton(btnNavVerification);

            LoadAccountsData();
        }




        private void LoadEquipmentSubjectFilter()
        {
            try
            {
                cmbEquipmentSubjectFilter.Items.Clear();
                cmbEquipmentSubjectFilter.Items.Add("All Subjects");

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT SubjectCode
FROM LabSubjects
WHERE LabID = ?
AND IsActive = True
ORDER BY SubjectCode";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    cmbEquipmentSubjectFilter.Items.Add(reader["SubjectCode"].ToString());
                }

                cmbEquipmentSubjectFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading equipment subject filter:\n" + ex.Message);
            }
        }



        private void cmbEquipmentSubjectFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentEquipmentSubject = cmbEquipmentSubjectFilter.SelectedItem?.ToString() ?? "All Subjects";
            LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
        }



        private void ShowEquipmentPanel()
        {
            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = true;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            panelEquipment.BringToFront();

            lblWelcome.Text = "Equipment";
            ResetSidebarButtons();
            SetActiveButton(btnNavEquipment);

            UpdateEquipmentSectionTitle();
            CreateAdminEquipmentCategoryDropdown();
            InitializeEquipmentAdminButtons();
        }



        private void ShowBorrowedPanel()
        {
            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            if (pnlDamageReportsAdmin != null)
                pnlDamageReportsAdmin.Visible = false;

            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = true;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            panelBorrowed.BringToFront();

            lblWelcome.Text = "Borrowing";
            ResetSidebarButtons();
            SetActiveButton(btnNavBorrowed);

            btnReturn.Visible = false;

            if (btnReportDamageDynamic != null)
                btnReportDamageDynamic.Visible = false;

            if (pnlBorrowedReportsDrawer != null)
            {
                pnlBorrowedReportsDrawer.Visible = false;
                isBorrowedReportsDrawerOpen = false;
            }

            dgvBorrowed.Visible = true;
            dgvBorrowed.BringToFront();

            FlowLayoutPanel? oldCards = pnlBorrowedMain.Controls
                .OfType<FlowLayoutPanel>()
                .FirstOrDefault(f => f.Name == "flowAdminBorrowedCards");
            if (oldCards != null)
                oldCards.Visible = false;

            if (pnlBorrowedMain.Controls.Find("txtSearchBorrowedCards", false).Length == 0)
            {
                TextBox txtSearch = new TextBox
                {
                    Name = "txtSearchBorrowedCards",
                    PlaceholderText = "Search leader, member, or student ID...",
                    Size = new Size(320, 30),
                    Location = new Point(20, 18),
                    Font = new Font("Segoe UI", 9.5F),
                    BorderStyle = BorderStyle.FixedSingle
                };

                txtSearch.TextChanged += (s, e) =>
                {
                    currentBorrowedSearch = txtSearch.Text.Trim();
                    LoadBorrowedData(currentBorrowedSearch);
                };

                pnlBorrowedMain.Controls.Add(txtSearch);
                txtSearch.BringToFront();
            }

            if (pnlBorrowedMain.Controls.Find("cmbBorrowedStatusFilter", false).Length == 0)
            {
                ComboBox cmbFilter = new ComboBox
                {
                    Name = "cmbBorrowedStatusFilter",
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Size = new Size(160, 30),
                    Location = new Point(355, 18),
                    Font = new Font("Segoe UI", 9.5F)
                };
                cmbFilter.Items.AddRange(new object[] { "All", "Borrowed", "Overdue" });
                cmbFilter.SelectedItem = currentBorrowedFilter;
                cmbFilter.SelectedIndexChanged += (s, e) =>
                {
                    currentBorrowedFilter = cmbFilter.Text;
                    LoadBorrowedData(currentBorrowedSearch);
                };

                pnlBorrowedMain.Controls.Add(cmbFilter);
                cmbFilter.BringToFront();
            }

            LoadBorrowedData();
            InitializeBorrowedReportsDrawerButton();
        }




        private void dgvBorrowed_SelectionChanged(object sender, EventArgs e)
        {
            btnReturn.Visible = false;

            if (btnReportDamageDynamic != null)
                btnReportDamageDynamic.Visible = false;
        }



        private void ShowReservationsPanel()
        {
            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = true;
            panelHistory.Visible = false;

            panelReservations.BringToFront();

            currentReservationFilter = "Pending";
            lblWelcome.Text = "Slips";

            ResetSidebarButtons();
            SetActiveButton(btnNavReservations);

            dgvReservations.Visible = false;
            btnClaim.Visible = false;
            btnUnclaimed.Visible = false;

            InitializeReservationsSearch();
            LoadReservationsData();
        }



        private void ShowHistoryPanel()
        {
            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = true;

            panelHistory.BringToFront();

            lblWelcome.Text = "History";

            ResetSidebarButtons();
            SetActiveButton(btnNavHistory);

            LoadHistoryData();
        }



        private void btnNavExperimentManuals_Click(object sender, EventArgs e)
        {
            ShowExperimentManualAdminPanel();
        }



        private void ShowExperimentManualAdminPanel()
        {
            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            lblWelcome.Text = "Experiment Manuals";

            ResetSidebarButtons();
            SetActiveButton(btnNavExperimentManuals);

            if (pnlExperimentManualAdmin != null)
            {
                pnlExperimentManualAdmin.Visible = true;
                pnlExperimentManualAdmin.BringToFront();

                LoadManualSubjects();
                LoadManualEquipmentCards();
                LoadExperimentManualList();
                return;
            }

            pnlExperimentManualAdmin = new Panel();
            pnlExperimentManualAdmin.Dock = DockStyle.Fill;
            pnlExperimentManualAdmin.BackColor = Color.FromArgb(245, 240, 245);

            contentPanel.Controls.Add(pnlExperimentManualAdmin);
            pnlExperimentManualAdmin.BringToFront();

            Panel mainCard = new Panel();
            mainCard.BackColor = Color.WhiteSmoke;
            mainCard.Location = new Point(34, 27);
            mainCard.Size = new Size(1040, 600);
            pnlExperimentManualAdmin.Controls.Add(mainCard);

            Label lblTitle = new Label();
            lblTitle.Text = "CREATE EXPERIMENT MANUAL";
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(32, 26);
            lblTitle.AutoSize = true;

            Label lblSubject = new Label();
            lblSubject.Text = "Subject";
            lblSubject.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSubject.Location = new Point(35, 78);
            lblSubject.AutoSize = true;

            cmbManualSubject = new ComboBox();
            cmbManualSubject.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualSubject.Font = new Font("Segoe UI", 9.5F);
            cmbManualSubject.Location = new Point(35, 104);
            cmbManualSubject.Size = new Size(250, 25);
            cmbManualSubject.SelectedIndexChanged += (s, e) =>
            {
                LoadManualEquipmentCards();
            };

            Label lblExpName = new Label();
            lblExpName.Text = "Experiment Name";
            lblExpName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblExpName.Location = new Point(310, 78);
            lblExpName.AutoSize = true;

            txtManualExperimentName = new TextBox();
            txtManualExperimentName.Font = new Font("Segoe UI", 9.5F);
            txtManualExperimentName.Location = new Point(310, 104);
            txtManualExperimentName.Size = new Size(330, 25);
            txtManualExperimentName.PlaceholderText = "Example: Experiment 1 - Basic Circuit Testing";

            Label lblEquipment = new Label();
            lblEquipment.Text = "Equipment under selected subject";
            lblEquipment.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEquipment.Location = new Point(35, 150);
            lblEquipment.AutoSize = true;

            flowManualEquipmentCards = new FlowLayoutPanel();
            flowManualEquipmentCards.Location = new Point(35, 180);
            flowManualEquipmentCards.Size = new Size(605, 180);
            flowManualEquipmentCards.AutoScroll = true;
            flowManualEquipmentCards.FlowDirection = FlowDirection.LeftToRight;
            flowManualEquipmentCards.WrapContents = true;
            flowManualEquipmentCards.BackColor = Color.FromArgb(245, 240, 247);

            Label lblAdded = new Label();
            lblAdded.Text = "Added Equipment";
            lblAdded.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAdded.Location = new Point(35, 378);
            lblAdded.AutoSize = true;

            flowManualItems = new FlowLayoutPanel();
            flowManualItems.Location = new Point(35, 405);
            flowManualItems.Size = new Size(605, 120);
            flowManualItems.AutoScroll = true;
            flowManualItems.FlowDirection = FlowDirection.TopDown;
            flowManualItems.WrapContents = false;
            flowManualItems.BackColor = Color.FromArgb(245, 240, 247);
            flowManualItems.BorderStyle = BorderStyle.FixedSingle;

            btnManualSave = new Button();
            btnManualSave.Text = "Save Manual";
            btnManualSave.Location = new Point(490, 540);
            btnManualSave.Size = new Size(150, 38);
            btnManualSave.BackColor = Color.FromArgb(169, 215, 159);
            btnManualSave.ForeColor = Color.White;
            btnManualSave.FlatStyle = FlatStyle.Flat;
            btnManualSave.FlatAppearance.BorderSize = 0;
            btnManualSave.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnManualSave.Click += btnManualSave_Click;

            Label lblListTitle = new Label();
            lblListTitle.Text = "SAVED MANUALS";
            lblListTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblListTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblListTitle.Location = new Point(690, 26);
            lblListTitle.AutoSize = true;

            flowManualList = new FlowLayoutPanel();
            flowManualList.Location = new Point(690, 70);
            flowManualList.Size = new Size(310, 500);
            flowManualList.AutoScroll = true;
            flowManualList.FlowDirection = FlowDirection.TopDown;
            flowManualList.WrapContents = false;
            flowManualList.BackColor = Color.Transparent;

            mainCard.Controls.Add(lblTitle);
            mainCard.Controls.Add(lblSubject);
            mainCard.Controls.Add(cmbManualSubject);
            mainCard.Controls.Add(lblExpName);
            mainCard.Controls.Add(txtManualExperimentName);
            mainCard.Controls.Add(lblEquipment);
            mainCard.Controls.Add(flowManualEquipmentCards);
            mainCard.Controls.Add(lblAdded);
            mainCard.Controls.Add(flowManualItems);
            mainCard.Controls.Add(btnManualSave);
            mainCard.Controls.Add(lblListTitle);
            mainCard.Controls.Add(flowManualList);

            RoundControl(mainCard, 28);
            RoundControl(btnManualSave, 16);

            LoadManualSubjects();
            LoadManualEquipmentCards();
            LoadExperimentManualList();
        }






        private void LoadManualEquipmentCards()
        {
            if (flowManualEquipmentCards == null)
                return;

            flowManualEquipmentCards.Controls.Clear();

            if (cmbManualSubject == null || cmbManualSubject.SelectedItem == null)
                return;

            int subjectId = GetManualSelectedSubjectId();

            if (subjectId <= 0)
                return;

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    E.EquipmentID,
    E.EquipmentName,
    E.Category,
    E.QuantityTotal,
    E.QuantityMaintenance,
    E.ImagePath
FROM ((Equipment AS E
INNER JOIN SubjectEquipments AS SE ON E.EquipmentID = SE.EquipmentID)
INNER JOIN LabSubjects AS LS ON SE.SubjectID = LS.SubjectID)
WHERE SE.SubjectID = ?
AND E.IsArchived = False
AND E.Status = 'Active'
ORDER BY E.EquipmentName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", subjectId);

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasCards = false;

                while (reader != null && reader.Read())
                {
                    int equipmentId = Convert.ToInt32(reader["EquipmentID"]);
                    string equipmentName = reader["EquipmentName"] != DBNull.Value ? reader["EquipmentName"].ToString() : "";
                    string category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "";

                    int total = reader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(reader["QuantityTotal"]) : 0;
                    int maintenance = reader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(reader["QuantityMaintenance"]) : 0;
                    int available = total - maintenance;

                    if (available < 0)
                        available = 0;

                    string imagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";

                    Panel card = CreateManualEquipmentCard(equipmentId, equipmentName, category, available, imagePath);
                    flowManualEquipmentCards.Controls.Add(card);

                    hasCards = true;
                }

                if (!hasCards)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No equipment linked to this subject yet.";
                    lblEmpty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    lblEmpty.ForeColor = Color.FromArgb(126, 105, 136);
                    lblEmpty.AutoSize = false;
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
                    lblEmpty.Size = new Size(560, 50);

                    flowManualEquipmentCards.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subject equipment:\n" + ex.Message);
            }
        }

        private int GetManualSelectedSubjectId()
        {
            if (cmbManualSubject == null || cmbManualSubject.SelectedItem == null)
                return 0;

            string selected = cmbManualSubject.SelectedItem.ToString();
            int dashIndex = selected.IndexOf(" - ");

            if (dashIndex <= 0)
                return 0;

            return Convert.ToInt32(selected.Substring(0, dashIndex));
        }

        private Panel CreateManualEquipmentCard(int equipmentId, string equipmentName, string category, int available, string imagePath)
        {
            Panel card = new Panel();
            card.Size = new Size(150, 145);
            card.BackColor = Color.FromArgb(255, 251, 252);
            card.Margin = new Padding(8);
            card.Cursor = Cursors.Hand;

            PictureBox pic = new PictureBox();
            pic.Location = new Point(15, 10);
            pic.Size = new Size(120, 50);
            pic.BackColor = Color.FromArgb(243, 236, 245);
            pic.SizeMode = PictureBoxSizeMode.Zoom;

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

            Label lblName = new Label();
            lblName.Text = equipmentName;
            lblName.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(69, 45, 96);
            lblName.Location = new Point(10, 65);
            lblName.Size = new Size(130, 22);
            lblName.TextAlign = ContentAlignment.MiddleCenter;

            Label lblAvail = new Label();
            lblAvail.Text = "Available: " + available;
            lblAvail.Font = new Font("Segoe UI", 8F);
            lblAvail.ForeColor = Color.FromArgb(126, 105, 136);
            lblAvail.Location = new Point(10, 88);
            lblAvail.Size = new Size(130, 18);
            lblAvail.TextAlign = ContentAlignment.MiddleCenter;

            Button btnAdd = new Button();
            btnAdd.Text = "Add";
            btnAdd.Size = new Size(75, 26);
            btnAdd.Location = new Point(38, 112);
            btnAdd.BackColor = Color.FromArgb(212, 168, 45);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnAdd.FlatAppearance.BorderSize = 0;

            btnAdd.Click += (s, e) =>
            {
                ShowManualEquipmentQuantityDialog(equipmentId, equipmentName, available);
            };

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblAvail);
            card.Controls.Add(btnAdd);

            RoundControl(card, 18);
            RoundControl(btnAdd, 10);

            return card;
        }

        private void ShowManualEquipmentQuantityDialog(int equipmentId, string equipmentName, int available)
        {
            Form qtyForm = new Form();
            qtyForm.Text = "Add Equipment";
            qtyForm.StartPosition = FormStartPosition.CenterParent;
            qtyForm.Size = new Size(360, 260);
            qtyForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            qtyForm.MaximizeBox = false;
            qtyForm.MinimizeBox = false;
            qtyForm.BackColor = Color.FromArgb(250, 245, 247);

            Label lblName = new Label();
            lblName.Text = equipmentName;
            lblName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(69, 45, 96);
            lblName.Location = new Point(24, 22);
            lblName.Size = new Size(300, 32);
            lblName.TextAlign = ContentAlignment.MiddleCenter;

            Label lblAvailable = new Label();
            lblAvailable.Text = "Available: " + available;
            lblAvailable.Font = new Font("Segoe UI", 9.5F);
            lblAvailable.ForeColor = Color.FromArgb(126, 105, 136);
            lblAvailable.Location = new Point(24, 58);
            lblAvailable.Size = new Size(300, 22);
            lblAvailable.TextAlign = ContentAlignment.MiddleCenter;

            Button btnMinus = new Button();
            btnMinus.Text = "-";
            btnMinus.Size = new Size(42, 36);
            btnMinus.Location = new Point(92, 105);
            btnMinus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnMinus.BackColor = Color.FromArgb(214, 197, 224);
            btnMinus.ForeColor = Color.FromArgb(87, 60, 99);
            btnMinus.FlatStyle = FlatStyle.Flat;
            btnMinus.FlatAppearance.BorderSize = 0;

            Label lblQty = new Label();
            lblQty.Text = "1";
            lblQty.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblQty.ForeColor = Color.FromArgb(69, 45, 96);
            lblQty.Location = new Point(145, 107);
            lblQty.Size = new Size(60, 32);
            lblQty.TextAlign = ContentAlignment.MiddleCenter;

            Button btnPlus = new Button();
            btnPlus.Text = "+";
            btnPlus.Size = new Size(42, 36);
            btnPlus.Location = new Point(215, 105);
            btnPlus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnPlus.BackColor = Color.FromArgb(214, 197, 224);
            btnPlus.ForeColor = Color.FromArgb(87, 60, 99);
            btnPlus.FlatStyle = FlatStyle.Flat;
            btnPlus.FlatAppearance.BorderSize = 0;

            int qty = 1;

            btnMinus.Click += (s, e) =>
            {
                if (qty > 1)
                    qty--;

                lblQty.Text = qty.ToString();
            };

            btnPlus.Click += (s, e) =>
            {
                if (qty < available)
                    qty++;

                lblQty.Text = qty.ToString();
            };

            Button btnConfirm = new Button();
            btnConfirm.Text = "Add to Manual";
            btnConfirm.Size = new Size(150, 38);
            btnConfirm.Location = new Point(105, 168);
            btnConfirm.BackColor = Color.FromArgb(169, 215, 159);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnConfirm.FlatAppearance.BorderSize = 0;

            btnConfirm.Click += (s, e) =>
            {
                AddEquipmentToManualList(equipmentId, equipmentName, qty);
                qtyForm.Close();
            };

            qtyForm.Controls.Add(lblName);
            qtyForm.Controls.Add(lblAvailable);
            qtyForm.Controls.Add(btnMinus);
            qtyForm.Controls.Add(lblQty);
            qtyForm.Controls.Add(btnPlus);
            qtyForm.Controls.Add(btnConfirm);

            RoundControl(btnMinus, 12);
            RoundControl(btnPlus, 12);
            RoundControl(btnConfirm, 16);

            qtyForm.ShowDialog(this);
        }

        private void AddEquipmentToManualList(int equipmentId, string equipmentName, int qty)
        {
            foreach (ManualAdminItem item in manualItems)
            {
                if (item.EquipmentID == equipmentId)
                {
                    item.QuantityNeeded += qty;
                    RefreshManualItemList();
                    return;
                }
            }

            manualItems.Add(new ManualAdminItem
            {
                EquipmentID = equipmentId,
                EquipmentName = equipmentName,
                QuantityNeeded = qty
            });

            RefreshManualItemList();
        }

        private void RefreshManualItemList()
        {
            flowManualItems.Controls.Clear();

            foreach (ManualAdminItem item in manualItems)
            {
                Panel row = new Panel();
                row.Size = new Size(560, 38);
                row.BackColor = Color.FromArgb(255, 251, 252);
                row.Margin = new Padding(6);

                Label lblText = new Label();
                lblText.Text = item.EquipmentName + "   Qty: " + item.QuantityNeeded;
                lblText.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                lblText.ForeColor = Color.FromArgb(69, 45, 96);
                lblText.Location = new Point(12, 9);
                lblText.Size = new Size(420, 22);

                Button btnRemove = new Button();
                btnRemove.Text = "Remove";
                btnRemove.Size = new Size(90, 26);
                btnRemove.Location = new Point(455, 6);
                btnRemove.BackColor = Color.FromArgb(220, 95, 107);
                btnRemove.ForeColor = Color.White;
                btnRemove.FlatStyle = FlatStyle.Flat;
                btnRemove.FlatAppearance.BorderSize = 0;

                btnRemove.Click += (s, e) =>
                {
                    manualItems.Remove(item);
                    RefreshManualItemList();
                };

                row.Controls.Add(lblText);
                row.Controls.Add(btnRemove);

                flowManualItems.Controls.Add(row);
                RoundControl(row, 12);
            }
        }





        private void LoadManualSubjects()
        {
            cmbManualSubject.Items.Clear();

            using var conn = DbHelper.GetConnection();
            conn.Open();

            string query = "SELECT SubjectCode FROM LabSubjects ORDER BY SubjectCode";

            using var cmd = new OleDbCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cmbManualSubject.Items.Add(reader["SubjectCode"].ToString());
            }

            if (cmbManualSubject.Items.Count > 0)
                cmbManualSubject.SelectedIndex = 0;
        }

        private void LoadManualEquipment()
        {
            cmbManualEquipment.Items.Clear();

            using var conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
SELECT EquipmentID, EquipmentName
FROM Equipment
WHERE IsArchived = False
ORDER BY EquipmentName";

            using var cmd = new OleDbCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            cmbManualEquipment.DataSource = dt;
            cmbManualEquipment.DisplayMember = "EquipmentName";
            cmbManualEquipment.ValueMember = "EquipmentID";
        }

        private void btnManualAddEquipment_Click(object sender, EventArgs e)
        {
            string equipmentName = cmbManualEquipment.Text;
            int equipmentId = Convert.ToInt32(cmbManualEquipment.SelectedValue);
            int qty = (int)numManualQty.Value;

            Label lbl = new Label();

            lbl.Text = equipmentName + " - Qty: " + qty;
            lbl.Font = new Font("Segoe UI", 10F);
            lbl.AutoSize = false;
            lbl.Width = 650;
            lbl.Height = 35;
            lbl.Padding = new Padding(10, 8, 0, 0);
            lbl.BackColor = Color.FromArgb(240, 235, 245);

            flowManualItems.Controls.Add(lbl);

            manualItems.Add(new ManualAdminItem
            {
                EquipmentID = equipmentId,
                EquipmentName = equipmentName,
                QuantityNeeded = qty
            });
        }

        private void btnManualSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Experiment Manual Saved!\n\nDatabase saving will be added next.",
                "Manual",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }




        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            ShowDashboardPanel();
        }


        private void btnNavVerification_Click(object sender, EventArgs e)
        {
            ShowVerificationPanel();
        }


        private void btnNavEquipment_Click(object sender, EventArgs e)
        {
            ShowEquipmentPanel();
        }

        private void btnNavBorrowed_Click(object sender, EventArgs e)
        {
            ShowBorrowedPanel();
        }


        private void btnNavReservations_Click(object sender, EventArgs e)
        {
            ShowReservationsPanel();
        }
        

        private void btnNavHistory_Click(object sender, EventArgs e)
        {
            ShowHistoryPanel();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();
            Close();
        }



        private void ConfigureGrids()
        {
            ConfigurePendingGrid();
            ConfigureBorrowedGrid();
            ConfigureReservationsGrid();
            ConfigureHistoryGrid();
        }


        private void ConfigurePendingGrid()
        {
            dgvPendingUsers.AutoGenerateColumns = false;
            dgvPendingUsers.Columns.Clear();
            dgvPendingUsers.ReadOnly = true;
            dgvPendingUsers.AllowUserToAddRows = false;
            dgvPendingUsers.AllowUserToDeleteRows = false;
            dgvPendingUsers.AllowUserToResizeRows = false;
            dgvPendingUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPendingUsers.MultiSelect = false;
            dgvPendingUsers.RowHeadersVisible = false;

            // CLEAN BASE
            dgvPendingUsers.BorderStyle = BorderStyle.None;
            dgvPendingUsers.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvPendingUsers.GridColor = Color.FromArgb(230, 220, 240);
            dgvPendingUsers.BackgroundColor = Color.WhiteSmoke;
            dgvPendingUsers.EnableHeadersVisualStyles = false;
            dgvPendingUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // HEADER STYLE
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 250);
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(90, 60, 100);
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // 🔥 FIX BLUE HEADER (IMPORTANT)
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 240, 250);
            dgvPendingUsers.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(90, 60, 100);

            dgvPendingUsers.ColumnHeadersHeight = 42;

            // CELL STYLE
            dgvPendingUsers.DefaultCellStyle.BackColor = Color.White;
            dgvPendingUsers.DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 80);
            dgvPendingUsers.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvPendingUsers.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);

            // SOFT SELECTION (no harsh purple block)
            dgvPendingUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 210, 240);
            dgvPendingUsers.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvPendingUsers.RowTemplate.Height = 38;

            // COLUMNS
            dgvPendingUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UserID",
                DataPropertyName = "UserID",
                Visible = false
            });

            dgvPendingUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Name",
                DataPropertyName = "FullName",
                Width = 220
            });

            dgvPendingUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SchoolID",
                HeaderText = "School ID",
                DataPropertyName = "SchoolID",
                Width = 140
            });

            dgvPendingUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SchoolEmail",
                HeaderText = "School Email",
                DataPropertyName = "SchoolEmail",
                Width = 205
            });

            dgvPendingUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccountStatus",
                HeaderText = "Status",
                DataPropertyName = "AccountStatus",
                Width = 90
            });

            dgvPendingUsers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "AccountAction",
                HeaderText = "Action",
                DataPropertyName = "AccountAction",
                UseColumnTextForButtonValue = false,
                Width = 92
            });

            dgvPendingUsers.Columns["AccountStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPendingUsers.Columns["AccountAction"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 🚫 PREVENT COLUMN HIGHLIGHT (THIS FIXES YOUR BLUE NAME)
            dgvPendingUsers.ClearSelection();
            dgvPendingUsers.CurrentCell = null;
        }



        
        private void ConfigureAccountsDetailsList()
        {
            txtVerifiedSearch.Visible = false;
        }


        private void ConfigureBorrowedGrid()
        {
            dgvBorrowed.AutoGenerateColumns = false;
            dgvBorrowed.Columns.Clear();
            dgvBorrowed.ReadOnly = true;
            dgvBorrowed.AllowUserToAddRows = false;
            dgvBorrowed.AllowUserToDeleteRows = false;
            dgvBorrowed.AllowUserToResizeRows = false;
            dgvBorrowed.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBorrowed.MultiSelect = false;
            dgvBorrowed.RowHeadersVisible = false;

            dgvBorrowed.BorderStyle = BorderStyle.None;
            dgvBorrowed.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvBorrowed.GridColor = Color.FromArgb(230, 220, 240);
            dgvBorrowed.BackgroundColor = Color.WhiteSmoke;
            dgvBorrowed.EnableHeadersVisualStyles = false;
            dgvBorrowed.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvBorrowed.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 250);
            dgvBorrowed.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(90, 60, 100);
            dgvBorrowed.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvBorrowed.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dgvBorrowed.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvBorrowed.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 240, 250);
            dgvBorrowed.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(90, 60, 100);
            dgvBorrowed.ColumnHeadersHeight = 42;

            dgvBorrowed.DefaultCellStyle.BackColor = Color.White;
            dgvBorrowed.DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 80);
            dgvBorrowed.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvBorrowed.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            dgvBorrowed.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 210, 240);
            dgvBorrowed.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvBorrowed.RowTemplate.Height = 38;

            dgvBorrowed.Columns.Clear();

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipID",
                DataPropertyName = "SlipID",
                Visible = false
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LeaderName",
                HeaderText = "Leader",
                DataPropertyName = "LeaderName",
                Width = 145
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SchoolID",
                HeaderText = "School ID",
                DataPropertyName = "SchoolID",
                Width = 120
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GroupNumber",
                HeaderText = "Group #",
                DataPropertyName = "GroupNumber",
                Width = 85
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Section",
                HeaderText = "Section",
                DataPropertyName = "Section",
                Width = 95
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectCode",
                HeaderText = "Subject",
                DataPropertyName = "SubjectCode",
                Width = 95
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateCreated",
                HeaderText = "Borrow Date",
                DataPropertyName = "DateCreated",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DueDate",
                HeaderText = "Expected Return",
                DataPropertyName = "DueDate",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvBorrowed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemReturnStatus",
                HeaderText = "Status",
                DataPropertyName = "DisplayStatus",
                Width = 110
            });
        }




        private void ConfigureHistoryGrid()
        {
            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.Columns.Clear();
            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.MultiSelect = false;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.BackgroundColor = Color.WhiteSmoke;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(228, 218, 236);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(69, 45, 96);
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgvHistory.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 190, 225);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(69, 45, 96);

            dgvHistory.Columns.Clear();

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipID",
                DataPropertyName = "SlipID",
                Visible = false
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LeaderName",
                HeaderText = "Leader",
                DataPropertyName = "LeaderName",
                Width = 145
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
                Name = "GroupNumber",
                HeaderText = "Group #",
                DataPropertyName = "GroupNumber",
                Width = 85
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Section",
                HeaderText = "Section",
                DataPropertyName = "Section",
                Width = 95
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectCode",
                HeaderText = "Subject",
                DataPropertyName = "SubjectCode",
                Width = 95
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateCreated",
                HeaderText = "Borrow Date",
                DataPropertyName = "DateCreated",
                Width = 145,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DueDate",
                HeaderText = "Expected Return",
                DataPropertyName = "DueDate",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemReturnStatus",
                HeaderText = "Status",
                DataPropertyName = "DisplayStatus",
                Width = 110
            });
        }



        private void UpdateEquipmentSectionTitle()
        {
            if (currentEquipmentCategory == "All")
                lblEquipmentHeader.Text = "All Equipment";
            else
                lblEquipmentHeader.Text = currentEquipmentCategory;
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (dgvBorrowed.CurrentRow == null)
            {
                MessageBox.Show("Select a borrowed item first.");
                return;
            }

            int slipItemId = Convert.ToInt32(dgvBorrowed.CurrentRow.Cells["ItemID"].Value);

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int equipmentId = 0;
                int quantityRequested = 0;
                string equipmentName = "";
                string equipmentType = "Reusable";
                bool hasSerial = false;

                string infoQuery = @"
SELECT
    BSI.SlipItemID,
    BSI.EquipmentID,
    BSI.QuantityRequested,
    E.EquipmentName,
    E.EquipmentType,
    E.HasSerial
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipItemID = ?";

                using (OleDbCommand cmd = new OleDbCommand(infoQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipItemId);

                    using OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader == null || !reader.Read())
                    {
                        MessageBox.Show("Borrowed item not found.");
                        return;
                    }

                    equipmentId = Convert.ToInt32(reader["EquipmentID"]);
                    quantityRequested = Convert.ToInt32(reader["QuantityRequested"]);
                    equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                    equipmentType = reader["EquipmentType"]?.ToString() ?? "Reusable";

                    hasSerial = reader["HasSerial"] != DBNull.Value &&
                                Convert.ToBoolean(reader["HasSerial"]);
                }

                string finalUnitStatus = "Available";
                int quantityReturned = quantityRequested;

                if (equipmentType == "Consumable" || equipmentType == "One Time Use")
                {
                    DialogResult confirm = MessageBox.Show(
                        equipmentName + " is One Time Use.\n\nIt will NOT return to available stock.\n\nContinue?",
                        "Return One Time Use",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm != DialogResult.Yes)
                        return;

                    finalUnitStatus = "Consumed";
                    quantityReturned = 0;
                }

                if (equipmentType == "Limited Use")
                {
                    DialogResult result = MessageBox.Show(
                        equipmentName + " is Limited Use.\n\nYES = Still usable, return to stock\nNO = Used up, do not return to stock\nCANCEL = stop return",
                        "Limited Use Return",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        return;

                    if (result == DialogResult.Yes)
                    {
                        finalUnitStatus = "Available";
                        quantityReturned = quantityRequested;
                    }
                    else
                    {
                        finalUnitStatus = "Consumed";
                        quantityReturned = 0;
                    }
                }

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    string updateItemQuery = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Returned',
    QuantityReturned = ?
WHERE SlipItemID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(updateItemQuery, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@p1", quantityReturned);
                        cmd.Parameters.AddWithValue("@p2", slipItemId);
                        cmd.ExecuteNonQuery();
                    }

                    if (hasSerial)
                    {
                        string updateUnitsQuery = @"
UPDATE EquipmentUnits
SET UnitStatus = ?
WHERE UnitID IN
(
    SELECT UnitID
    FROM BorrowSlipUnits
    WHERE SlipItemID = ?
)";

                        using (OleDbCommand cmd = new OleDbCommand(updateUnitsQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@p1", finalUnitStatus);
                            cmd.Parameters.AddWithValue("@p2", slipItemId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();

                    MessageBox.Show(
                        "Return processed successfully.",
                        "Return",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadBorrowedData();
                    LoadAdminDashboardNew();
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
                MessageBox.Show(
                    "Error processing return:\n" + ex.Message,
                    "Return",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private void StyleReservationsGrid()
        {
            // Trigger RowPrePaint for all visible rows
            dgvReservations.Invalidate();
        }

        private void dgvReservations_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvReservations.Rows[e.RowIndex];
            string status = row.Cells["ReservationStatus"].Value?.ToString() ?? "";

            switch (status)
            {
                case "Pending":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 235);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(130, 80, 10);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 225, 170);
                    row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 60, 5);
                    break;
                case "Claimed":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 250, 237);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(38, 97, 53);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(195, 235, 202);
                    row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(28, 77, 43);
                    break;
                case "Unclaimed":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(140, 40, 40);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 200, 200);
                    row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(110, 30, 30);
                    break;
                default:
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
            }
        }




        private void LoadDashboardCounts()
        {
            LoadAdminDashboardNew();
        }




        private int GetComputedAvailable(int total, int maintenance, int borrowed, int reserved)
        {
            int available = total - maintenance - borrowed - reserved;
            return available < 0 ? 0 : available;
        }

        private void LogActivity(string actionType, string description)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
INSERT INTO ActivityLogs (ActionType, Description, DateCreated)
VALUES (?, ?, ?)";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", actionType);
                cmd.Parameters.AddWithValue("@p2", description);
                cmd.Parameters.AddWithValue("@p3", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Silent fail so activity logging never breaks the main action
            }
        }



        private void LoadReservationNotifications()
        {
            LoadLowStockAlerts();
        }



        private void ConfigureReservationsGrid()
        {
            dgvReservations.AutoGenerateColumns = false;
            dgvReservations.Columns.Clear();
            dgvReservations.ReadOnly = true;
            dgvReservations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReservations.MultiSelect = false;
            dgvReservations.RowHeadersVisible = false;
            dgvReservations.AllowUserToAddRows = false;
            dgvReservations.AllowUserToDeleteRows = false;
            dgvReservations.AllowUserToResizeRows = false;

            dgvReservations.BackgroundColor = Color.WhiteSmoke;
            dgvReservations.BorderStyle = BorderStyle.None;
            dgvReservations.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvReservations.EnableHeadersVisualStyles = false;
            dgvReservations.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvReservations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(228, 218, 236);
            dgvReservations.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(69, 45, 96);
            dgvReservations.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgvReservations.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvReservations.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(228, 218, 236);
            dgvReservations.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(69, 45, 96);
            dgvReservations.ColumnHeadersHeight = 40;

            dgvReservations.DefaultCellStyle.BackColor = Color.White;
            dgvReservations.DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 80);
            dgvReservations.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvReservations.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvReservations.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 190, 225);
            dgvReservations.DefaultCellStyle.SelectionForeColor = Color.FromArgb(69, 45, 96);

            dgvReservations.RowTemplate.Height = 44;

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipID",
                DataPropertyName = "SlipID",
                Visible = false
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Borrower",
                DataPropertyName = "FullName",
                Width = 160
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SchoolID",
                HeaderText = "School ID",
                DataPropertyName = "SchoolID",
                Width = 110
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubjectCode",
                HeaderText = "Subject",
                DataPropertyName = "SubjectCode",
                Width = 90
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GroupNumber",
                HeaderText = "Group #",
                DataPropertyName = "GroupNumber",
                Width = 80
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipType",
                HeaderText = "Type",
                DataPropertyName = "SlipType",
                Width = 130
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateCreated",
                HeaderText = "Date Submitted",
                DataPropertyName = "DateCreated",
                Width = 145,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy hh:mm tt" }
            });

            dgvReservations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SlipStatus",
                HeaderText = "Status",
                DataPropertyName = "SlipStatus",
                Width = 110
            });

            dgvReservations.DataBindingComplete -= dgvReservations_DataBindingComplete;
            dgvReservations.DataBindingComplete += dgvReservations_DataBindingComplete;
        }


        private void dgvReservations_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvReservations.ClearSelection();
            dgvReservations.CurrentCell = null;
        }


        private Panel CreateRecentActivityCard(string actionType, string description, DateTime dateCreated)
        {
            Color backColor = Color.FromArgb(247, 241, 244);
            Color badgeFore = Color.FromArgb(87, 60, 99);

            if (actionType.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                backColor = Color.FromArgb(255, 225, 225);
                badgeFore = Color.FromArgb(150, 55, 55);
            }
            else if (actionType.Equals("Claim", StringComparison.OrdinalIgnoreCase))
            {
                backColor = Color.FromArgb(220, 245, 224);
                badgeFore = Color.FromArgb(45, 110, 60);
            }
            else if (actionType.Equals("Reservation", StringComparison.OrdinalIgnoreCase))
            {
                backColor = Color.FromArgb(255, 239, 213);
                badgeFore = Color.FromArgb(160, 98, 27);
            }
            else if (actionType.Equals("Return", StringComparison.OrdinalIgnoreCase))
            {
                backColor = Color.FromArgb(235, 246, 236);
                badgeFore = Color.FromArgb(50, 90, 58);
            }

            Panel card = new Panel
            {
                Width = 390,
                Height = 74,
                BackColor = backColor,
                Margin = new Padding(0, 0, 0, 10)
            };
            RoundControl(card, 18);

            Label lblDesc = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(14, 12),
                Size = new Size(250, 20)
            };

            Label lblDate = new Label
            {
                Text = dateCreated.ToString("MMM dd, yyyy hh:mm tt"),
                Font = new Font("Segoe UI", 8.8F),
                ForeColor = Color.FromArgb(110, 90, 122),
                Location = new Point(14, 40),
                Size = new Size(180, 18)
            };

            Label lblBadge = new Label
            {
                Text = actionType,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = badgeFore,
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(286, 24),
                Size = new Size(88, 24)
            };
            RoundControl(lblBadge, 12);

            card.Controls.Add(lblDesc);
            card.Controls.Add(lblDate);
            card.Controls.Add(lblBadge);

            return card;
        }



        private void LoadBorrowedChart()
        {
            try
            {
                EnsureDashboardPieChart();
                EnsureUsageHoverLabel();
                LoadWorkloadPieChart();

                pnlClaimableToday.Size = new Size(554, 86);
                lblClaimableHeader.Location = new Point(24, 14);
                lblClaimableValue.Location = new Point(24, 36);
                lblClaimableValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
                lblClaimableSub.Location = new Point(150, 45);

                pnlStatistics.Location = new Point(520, 333);
                pnlStatistics.Size = new Size(554, 260);
                chartStats.Location = new Point(288, 74);
                chartStats.Size = new Size(248, 174);
                chartStats.Series.Clear();
                chartStats.ChartAreas.Clear();
                chartStats.Titles.Clear();
                chartStats.Legends.Clear();

                ChartArea area = new ChartArea("MainArea");
                area.AxisX.Interval = 1;
                area.AxisX.LabelStyle.Angle = -45;
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7F);
                area.AxisX.LineColor = Color.FromArgb(160, 130, 155);
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.Minimum = 0;
                area.AxisY.Interval = 10;
                area.AxisY.Title = "Returned Quantity";
                area.AxisY.TitleFont = new Font("Segoe UI", 7F, FontStyle.Bold);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7F);
                area.AxisY.LineColor = Color.FromArgb(160, 130, 155);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(225, 214, 229);
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                area.BackColor = Color.White;
                area.Position = new ElementPosition(4, 4, 92, 88);
                area.InnerPlotPosition = new ElementPosition(13, 4, 80, 69);
                chartStats.ChartAreas.Add(area);

                Series series = new Series("Returned");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.IsXValueIndexed = true;
                series.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
                series.LabelForeColor = Color.Black;
                series.Color = Color.FromArgb(179, 126, 35);
                series.BackGradientStyle = GradientStyle.None;
                series.ShadowOffset = 0;
                series["PointWidth"] = "0.55";
                series["DrawingStyle"] = "Default";
                series["BarLabelStyle"] = "Center";
                series.Label = "#VALY";
                chartStats.Series.Add(series);

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
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Returned'
AND BSI.QuantityReturned > 0
GROUP BY TRIM(E.EquipmentName)
ORDER BY SUM(BSI.QuantityReturned) DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.LabID;

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasData = false;

                while (reader != null && reader.Read())
                {
                    string name = reader["EquipmentName"]?.ToString() ?? "Unknown";

                    int total = reader["TotalReturned"] != DBNull.Value
                        ? Convert.ToInt32(reader["TotalReturned"])
                        : 0;

                    string displayName = name.Length > 12
                        ? name.Substring(0, 11) + "…"
                        : name;

                    int pointIndex = series.Points.AddXY(displayName, total);
                    series.Points[pointIndex].Tag = name;
                    series.Points[pointIndex].Label = total.ToString();
                    series.Points[pointIndex].ToolTip = "";
                    hasData = true;
                }

                if (!hasData)
                    series.Points.AddXY("No Data", 0);
                else
                {
                    double maxValue = series.Points.Max(p => p.YValues[0]);
                    area.AxisY.Maximum = Math.Ceiling(Math.Max(1, maxValue) * 1.15);
                }

                WireDashboardChartHoverEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowed chart:\n" + ex.Message);
            }
        }


        private void EnsureUsageHoverLabel()
        {
            if (lblUsageHoverValue != null)
                return;

            lblUsageHoverValue = new Label
            {
                Name = "lblUsageHoverValue",
                Text = "",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(153, 0, 0),
                BackColor = Color.Transparent,
                Location = new Point(305, 50),
                Size = new Size(220, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Visible = false
            };

            pnlStatistics.Controls.Add(lblUsageHoverValue);
            lblUsageHoverValue.BringToFront();
        }


        private void EnsureDashboardPieChart()
        {
            if (chartSlipStatus != null)
                return;

            chartSlipStatus = new Chart
            {
                Name = "chartSlipStatus",
                BackColor = Color.White,
                Location = new Point(18, 74),
                Size = new Size(252, 174)
            };

            pnlStatistics.Controls.Add(chartSlipStatus);
            chartSlipStatus.BringToFront();
        }


        private void LoadWorkloadPieChart()
        {
            chartSlipStatus.Series.Clear();
            chartSlipStatus.ChartAreas.Clear();
            chartSlipStatus.Titles.Clear();
            chartSlipStatus.Legends.Clear();
            hoveredWorkloadSlice = -1;

            ChartArea area = new ChartArea("SlipStatusArea");
            area.Position = new ElementPosition(0, 0, 100, 100);
            area.InnerPlotPosition = new ElementPosition(15, 2, 69, 96);
            area.BackColor = Color.White;
            chartSlipStatus.ChartAreas.Add(area);

            Series series = new Series("Slips")
            {
                ChartType = SeriesChartType.Pie,
                ChartArea = "SlipStatusArea",
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                Label = "#AXISLABEL",
                LabelForeColor = Color.White,
                BorderColor = Color.FromArgb(250, 246, 248),
                BorderWidth = 2
            };
            series["PieLabelStyle"] = "Inside";
            series["PieStartAngle"] = "270";

            chartSlipStatus.Series.Add(series);

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            int pendingSlips = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ? AND BS.SlipStatus = 'Pending'", SessionManager.LabID);

            int overdueSlips = GetDashboardOverdueSlipCount(conn, SessionManager.LabID);
            int lowStockEquipment = GetDashboardLowStockCount(conn, SessionManager.LabID);

            int unpaidReports = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM DamageReports AS DR
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND DR.ReportStatus IN ('Pending Cost', 'For Payment')", SessionManager.LabID);

            AddSlipPiePoint(series, "Pending", pendingSlips, Color.FromArgb(212, 168, 45));
            AddSlipPiePoint(series, "Overdue", overdueSlips, Color.FromArgb(153, 0, 0));
            AddSlipPiePoint(series, "Low Stock", lowStockEquipment, Color.FromArgb(185, 134, 40));
            AddSlipPiePoint(series, "Reports", unpaidReports, Color.FromArgb(108, 25, 37));

            if (series.Points.Count == 0)
                AddSlipPiePoint(series, "No Data", 1, Color.FromArgb(190, 160, 95));

            if (series.Points.Count == 1 && series.Points[0].AxisLabel == "No Data")
            {
                series.Points[0].Label = "No Data";
                series.Points[0].Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            }
        }


        private void AddSlipPiePoint(Series series, string label, int value, Color color)
        {
            if (value <= 0)
                return;

            int index = series.Points.AddXY(label, value);
            series.Points[index].Color = color;
            series.Points[index].Tag = color;
            series.Points[index].ToolTip = label + ": " + value;
        }


        private void WireDashboardChartHoverEvents()
        {
            chartSlipStatus.MouseMove -= chartSlipStatus_MouseMove;
            chartSlipStatus.MouseLeave -= chartSlipStatus_MouseLeave;
            chartStats.MouseMove -= chartStats_MouseMove;
            chartStats.MouseLeave -= chartStats_MouseLeave;

            chartSlipStatus.MouseMove += chartSlipStatus_MouseMove;
            chartSlipStatus.MouseLeave += chartSlipStatus_MouseLeave;
            chartStats.MouseMove += chartStats_MouseMove;
            chartStats.MouseLeave += chartStats_MouseLeave;
        }


        private void chartSlipStatus_MouseMove(object? sender, MouseEventArgs e)
        {
            HitTestResult hit = chartSlipStatus.HitTest(e.X, e.Y);
            int pointIndex = hit.ChartElementType == ChartElementType.DataPoint
                ? hit.PointIndex
                : -1;

            if (pointIndex == hoveredWorkloadSlice)
                return;

            hoveredWorkloadSlice = pointIndex;
            ApplyWorkloadPieHover(pointIndex);
        }


        private void chartSlipStatus_MouseLeave(object? sender, EventArgs e)
        {
            if (hoveredWorkloadSlice == -1)
                return;

            hoveredWorkloadSlice = -1;
            ApplyWorkloadPieHover(-1);
        }


        private void ApplyWorkloadPieHover(int pointIndex)
        {
            if (chartSlipStatus.Series.Count == 0)
                return;

            Series series = chartSlipStatus.Series[0];

            for (int i = 0; i < series.Points.Count; i++)
            {
                DataPoint point = series.Points[i];
                Color baseColor = point.Tag is Color c ? c : point.Color;
                bool isNoDataSlice = point.AxisLabel.Equals("No Data", StringComparison.OrdinalIgnoreCase);

                point.Color = i == pointIndex
                    ? ControlPaint.Light(baseColor, 0.25f)
                    : baseColor;

                point["Exploded"] = i == pointIndex && !isNoDataSlice ? "true" : "false";
                point.Font = i == pointIndex
                    ? new Font("Segoe UI", 8.5F, FontStyle.Bold)
                    : new Font("Segoe UI", 7.5F, FontStyle.Bold);
                point.Label = isNoDataSlice
                    ? "No Data"
                    : i == pointIndex
                    ? "#AXISLABEL\n#PERCENT{P0}"
                    : "#AXISLABEL";
            }

            chartSlipStatus.Invalidate();
        }


        private void chartStats_MouseMove(object? sender, MouseEventArgs e)
        {
            HitTestResult hit = chartStats.HitTest(e.X, e.Y);
            int pointIndex = hit.ChartElementType == ChartElementType.DataPoint
                ? hit.PointIndex
                : -1;

            if (pointIndex == hoveredUsageBar)
                return;

            hoveredUsageBar = pointIndex;
            ApplyUsageBarHover(pointIndex);
        }


        private void chartStats_MouseLeave(object? sender, EventArgs e)
        {
            if (hoveredUsageBar == -1)
                return;

            hoveredUsageBar = -1;
            ApplyUsageBarHover(-1);
        }


        private void ApplyUsageBarHover(int pointIndex)
        {
            if (chartStats.Series.Count == 0)
                return;

            Series series = chartStats.Series[0];
            bool hasHover = pointIndex >= 0 && pointIndex < series.Points.Count;
            series["PointWidth"] = hasHover ? "0.78" : "0.55";

            for (int i = 0; i < series.Points.Count; i++)
            {
                DataPoint point = series.Points[i];

                point.Color = Color.FromArgb(179, 126, 35);
                point.BackGradientStyle = GradientStyle.None;
                point.BackHatchStyle = ChartHatchStyle.None;
                point.IsValueShownAsLabel = point.YValues[0] > 0;
                point.Label = point.YValues[0] > 0 ? ((int)point.YValues[0]).ToString() : "";
                point.LabelForeColor = Color.Black;
                point.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
                point.BorderWidth = 0;
                point.BorderColor = Color.Transparent;
            }

            if (lblUsageHoverValue != null)
            {
                if (pointIndex >= 0 && pointIndex < series.Points.Count)
                {
                    lblUsageHoverValue.Text = series.Points[pointIndex].Tag?.ToString()
                        ?? series.Points[pointIndex].AxisLabel;
                    lblUsageHoverValue.Visible = true;
                }
                else
                {
                    lblUsageHoverValue.Visible = false;
                    lblUsageHoverValue.Text = "";
                }
            }

            chartStats.Invalidate();
        }


        private int GetDashboardOverdueSlipCount(OleDbConnection conn, int labId)
        {
            int count = 0;

            string query = @"
SELECT
    BS.SlipID,
    BS.DateCreated,
    SS.EndTime,
    SUM(IIF(BSI.ItemReturnStatus = 'Borrowed', 1, 0)) AS BorrowedItemCount
FROM (((BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
GROUP BY BS.SlipID, BS.DateCreated, SS.EndTime";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", labId);

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                int borrowedItemCount = reader["BorrowedItemCount"] != DBNull.Value
                    ? Convert.ToInt32(reader["BorrowedItemCount"])
                    : 0;

                if (borrowedItemCount == 0)
                    continue;

                DateTime borrowDate = reader["DateCreated"] != DBNull.Value
                    ? Convert.ToDateTime(reader["DateCreated"])
                    : DateTime.Now;

                DateTime dueDate = BuildDueDate(borrowDate, reader["EndTime"]);
                if (DateTime.Now > dueDate)
                    count++;
            }

            return count;
        }


        private int GetDashboardLowStockCount(OleDbConnection conn, int labId)
        {
            int count = 0;

            string query = @"
SELECT EquipmentID, QuantityTotal, QuantityMaintenance,
       LowStockThreshold, EquipmentType, HasSerial
FROM Equipment
WHERE LabID = ?
AND IsArchived = False
AND Status = 'Active'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", labId);

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                int equipmentId = Convert.ToInt32(reader["EquipmentID"]);
                int total = reader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(reader["QuantityTotal"]) : 0;
                int maintenance = reader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(reader["QuantityMaintenance"]) : 0;
                int threshold = reader["LowStockThreshold"] != DBNull.Value ? Convert.ToInt32(reader["LowStockThreshold"]) : 3;
                string equipmentType = reader["EquipmentType"] != DBNull.Value ? reader["EquipmentType"].ToString() ?? "Reusable" : "Reusable";
                bool hasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"]);

                int available = GetCorrectAvailableQuantity(
                    conn,
                    equipmentId,
                    total,
                    maintenance,
                    hasSerial,
                    equipmentType);

                if (available <= threshold)
                    count++;
            }

            return count;
        }




        private int GetScalarCount(OleDbConnection conn, string query, params object[] parameters)
        {
            object? value = ExecuteScalar(conn, query, parameters);
            if (value == null || value == DBNull.Value) return 0;
            return Convert.ToInt32(value);
        }




        private object? ExecuteScalar(OleDbConnection conn, string query, params object[] parameters)
        {
            using OleDbCommand cmd = new OleDbCommand(query, conn);
            foreach (object p in parameters)
                cmd.Parameters.AddWithValue("@p", p);

            return cmd.ExecuteScalar();
        }


        


        private void txtEquipmentAdminSearch_TextChanged(object? sender, EventArgs e)
        {
            LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
        }



        private void btnRefreshEquipmentDynamic_Click(object? sender, EventArgs e)
        {
            txtEquipmentAdminSearch.Clear();
            LoadEquipmentCards(currentEquipmentCategory);
        }


        private void DeactivateUser(int userId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            try
            {
                string deactivateQuery = "UPDATE Users SET IsActive = False WHERE UserID = ?";
                using (OleDbCommand cmd = new OleDbCommand(deactivateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", userId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User deactivated successfully.",
                    "Account Action", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during deactivation:\n" + ex.Message,
                    "Account Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ActivateUser(int userId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string activateQuery = "UPDATE Users SET IsActive = True WHERE UserID = ?";

            using OleDbCommand cmd = new OleDbCommand(activateQuery, conn);
            cmd.Parameters.AddWithValue("@p1", userId);
            cmd.ExecuteNonQuery();

            MessageBox.Show("User activated successfully.",
                "Account Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtVerifiedSearch_TextChanged(object? sender, EventArgs e)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string keyword = txtVerifiedSearch.Text.Trim();
                string likeValue = "%" + keyword + "%";

                string query = @"
SELECT DISTINCT
    U.UserID,
    U.FullName,
    U.SchoolID,
    U.SchoolEmail,
    U.IsActive,
    IIF(U.IsActive = True, 'Active', 'Inactive') AS AccountStatus,
    IIF(U.IsActive = True, 'Deactivate', 'Activate') AS AccountAction
FROM (Users AS U
INNER JOIN StudentSubjectEnrollments AS SSE ON U.UserID = SSE.UserID)
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND SSE.IsActive = True
AND
(
    U.FullName LIKE ?
    OR U.SchoolID LIKE ?
    OR U.SchoolEmail LIKE ?
)
ORDER BY U.FullName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                cmd.Parameters.AddWithValue("@p2", likeValue);
                cmd.Parameters.AddWithValue("@p3", likeValue);
                cmd.Parameters.AddWithValue("@p4", likeValue);

                DataTable dt = new DataTable();
                using OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                dgvPendingUsers.DataSource = dt;

                if (dt.Rows.Count > 0)
                {
                    int firstUserId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    LoadAccountDetails(firstUserId);
                }
                else
                {
                    lblStudentName.Text = "No student selected";
                    lblStudentSchoolID.Text = "School ID: ---";
                    lblStudentEmail.Text = "Email: ---";
                    lblStudentBorrowedCount.Text = "0";
                    lblStudentReturnedCount.Text = "0";
                    lblStudentReservationsCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching students:\n" + ex.Message,
                    "Students Enrolled", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        private void dgvPendingUsers_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPendingUsers.Columns[e.ColumnIndex].Name != "AccountAction") return;

            int userId = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["UserID"].Value);
            ToggleStudentAccountStatus(userId);
        }



        private void dgvPendingUsers_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvPendingUsers.CurrentRow == null) return;

            if (dgvPendingUsers.CurrentRow.Cells["UserID"].Value == null ||
                dgvPendingUsers.CurrentRow.Cells["UserID"].Value == DBNull.Value)
                return;

            int userId = Convert.ToInt32(dgvPendingUsers.CurrentRow.Cells["UserID"].Value);
            if (userId > 0)
                LoadAccountDetails(userId);
        }

        private void dgvPendingUsers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvPendingUsers.Rows[e.RowIndex].Cells["AccountStatus"].Value == null)
                return;

            string status = dgvPendingUsers.Rows[e.RowIndex].Cells["AccountStatus"].Value?.ToString() ?? "";

            if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
            {
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(145, 75, 75);
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 244);
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 210, 215);
            }
            else
            {
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 80);
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                dgvPendingUsers.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 210, 240);
            }
        }




        private void ToggleStudentAccountStatus(int userId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT FullName, SchoolID, SchoolEmail, IsActive
FROM Users
WHERE UserID = ?";

                string fullName = "";
                string schoolID = "";
                string schoolEmail = "";
                bool isActive = true;

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", userId);

                    using OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.Read())
                    {
                        fullName = reader["FullName"]?.ToString() ?? "";
                        schoolID = reader["SchoolID"]?.ToString() ?? "";
                        schoolEmail = reader["SchoolEmail"]?.ToString() ?? "";
                        isActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);
                    }
                }

                string actionText = isActive ? "Deactivate this user?" : "Activate this user?";

                DialogResult result = MessageBox.Show(
                    $"Name: {fullName}\nSchool ID: {schoolID}\nEmail: {schoolEmail}\n\n{actionText}",
                    "Account Action",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                if (isActive)
                    DeactivateUser(userId);
                else
                    ActivateUser(userId);

                LoadAccountsData();
                LoadDashboardCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening account details:\n" + ex.Message,
                    "Accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadBorrowedData(string keyword = "")
        {
            try
            {
                dgvBorrowed.Visible = true;
                btnReturn.Visible = false;

                if (btnReportDamageDynamic != null)
                    btnReportDamageDynamic.Visible = false;

                FlowLayoutPanel flowBorrowed = pnlBorrowedMain.Controls
                    .OfType<FlowLayoutPanel>()
                    .FirstOrDefault(f => f.Name == "flowAdminBorrowedCards");
                if (flowBorrowed != null)
                    flowBorrowed.Visible = false;

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string likeValue = "%" + keyword + "%";

                string query = @"
SELECT
    BS.SlipID,
    BS.LeaderName,
    BS.GroupNumber,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    SS.Section,
    BS.DateCreated,
    SS.EndTime,
    COUNT(BSI.SlipItemID) AS ItemCount,
    SUM(IIF(BSI.ItemReturnStatus = 'Borrowed', 1, 0)) AS BorrowedItemCount,
    (SELECT COUNT(*) FROM DamageReports AS DR WHERE DR.SlipID = BS.SlipID) AS ReportCount
FROM (((((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
LEFT JOIN BorrowSlipMembers AS BSM ON BS.SlipID = BSM.SlipID)
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND
(
    U.FullName LIKE ?
    OR BS.LeaderName LIKE ?
    OR U.SchoolID LIKE ?
    OR BSM.MemberName LIKE ?
)
GROUP BY
    BS.SlipID,
    BS.LeaderName,
    BS.GroupNumber,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    SS.Section,
    BS.DateCreated,
    SS.EndTime
ORDER BY BS.DateCreated DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = SessionManager.LabID;
                cmd.Parameters.Add("@p2", OleDbType.VarWChar).Value = likeValue;
                cmd.Parameters.Add("@p3", OleDbType.VarWChar).Value = likeValue;
                cmd.Parameters.Add("@p4", OleDbType.VarWChar).Value = likeValue;
                cmd.Parameters.Add("@p5", OleDbType.VarWChar).Value = likeValue;

                DataTable raw = new DataTable();
                using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    da.Fill(raw);

                DataTable table = CreateBorrowedSlipTable();

                foreach (DataRow row in raw.Rows)
                {
                    DateTime borrowDate = row["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(row["DateCreated"])
                        : DateTime.Now;
                    DateTime dueDate = BuildDueDate(borrowDate, row["EndTime"]);
                    int borrowedItemCount = row["BorrowedItemCount"] != DBNull.Value
                        ? Convert.ToInt32(row["BorrowedItemCount"])
                        : 0;

                    if (borrowedItemCount == 0)
                        continue;

                    string status = DateTime.Now > dueDate ? "Overdue" : "Borrowed";

                    if (currentBorrowedFilter != "All" &&
                        !status.Equals(currentBorrowedFilter, StringComparison.OrdinalIgnoreCase))
                        continue;

                    table.Rows.Add(
                        row["SlipID"],
                        row["LeaderName"]?.ToString() ?? row["FullName"]?.ToString() ?? "",
                        row["SchoolID"]?.ToString() ?? "",
                        row["GroupNumber"]?.ToString() ?? "",
                        row["Section"]?.ToString() ?? "",
                        row["SubjectCode"]?.ToString() ?? "",
                        borrowDate,
                        dueDate,
                        status);
                }

                dgvBorrowed.DataSource = table;
                dgvBorrowed.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowed data:\n" + ex.Message);
            }
        }

        private DataTable CreateBorrowedSlipTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("SlipID", typeof(int));
            table.Columns.Add("LeaderName", typeof(string));
            table.Columns.Add("SchoolID", typeof(string));
            table.Columns.Add("GroupNumber", typeof(string));
            table.Columns.Add("Section", typeof(string));
            table.Columns.Add("SubjectCode", typeof(string));
            table.Columns.Add("DateCreated", typeof(DateTime));
            table.Columns.Add("DueDate", typeof(DateTime));
            table.Columns.Add("DisplayStatus", typeof(string));
            return table;
        }

        private DateTime BuildDueDate(DateTime borrowDate, object endTimeValue)
        {
            if (endTimeValue == DBNull.Value || endTimeValue == null)
                return borrowDate.Date.AddHours(23).AddMinutes(59);

            DateTime endTime = Convert.ToDateTime(endTimeValue);
            return borrowDate.Date.Add(endTime.TimeOfDay);
        }



        private Panel CreateAdminBorrowedCard(
    int slipItemId,
    int slipId,
    string fullName,
    string schoolId,
    string equipmentName,
    int quantity,
    DateTime borrowDate,
    string serialNumbers,
    int equipmentId,
    int cardWidth)
        {
            Panel card = new Panel
            {
                Width = cardWidth,
                Height = 130,
                BackColor = Color.FromArgb(255, 251, 252),
                Margin = new Padding(0, 0, 0, 12)
            };

            RoundControl(card, 18);

            Label lblName = new Label
            {
                Text = fullName + " (" + schoolId + ")",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(20, 14),
                Size = new Size(cardWidth - 280, 24)
            };

            Label lblEquipment = new Label
            {
                Text = "Equipment: " + equipmentName + "  ×  " + quantity,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(20, 42),
                Size = new Size(cardWidth - 280, 22)
            };

            Label lblDate = new Label
            {
                Text = "Borrowed: " + borrowDate.ToString("MMM dd, yyyy  hh:mm tt"),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(20, 66),
                Size = new Size(cardWidth - 280, 20)
            };

            Label lblSerials = new Label
            {
                Text = "Serial No.: " + serialNumbers,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(92, 45, 58),
                Location = new Point(20, 88),
                Size = new Size(cardWidth - 280, 20)
            };

            Button btnReturnCard = new Button
            {
                Text = "Return",
                Size = new Size(110, 36),
                Location = new Point(cardWidth - 240, 44),
                BackColor = Color.FromArgb(169, 215, 159),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReturnCard.FlatAppearance.BorderSize = 0;
            btnReturnCard.Click += (s, e) =>
            {
                ProcessReturnForCard(slipItemId, equipmentName, equipmentId);
            };

            Button btnReportCard = new Button
            {
                Text = "Report",
                Size = new Size(110, 36),
                Location = new Point(cardWidth - 120, 44),
                BackColor = Color.FromArgb(220, 95, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReportCard.FlatAppearance.BorderSize = 0;
            btnReportCard.Click += (s, e) =>
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int slipId = GetSlipIdForBorrowSlipItem(conn, slipItemId);
                List<BorrowSlipReturnItem> reportItems = LoadBorrowSlipReturnItems(conn, slipId)
                    .Where(x => x.SlipItemID == slipItemId && x.ItemReturnStatus == "Borrowed")
                    .ToList();

                List<(int SlipItemID, int ReportQuantity)> reportSelections =
                    ShowReportQuantitySelectionDialog(slipId, reportItems);

                foreach ((int selectedSlipItemId, int reportQuantity) in reportSelections)
                    ShowDamageReportForm(selectedSlipItemId, reportQuantity);
            };

            RoundControl(btnReturnCard, 14);
            RoundControl(btnReportCard, 14);

            card.Controls.Add(lblName);
            card.Controls.Add(lblEquipment);
            card.Controls.Add(lblDate);
            card.Controls.Add(lblSerials);
            card.Controls.Add(btnReturnCard);
            card.Controls.Add(btnReportCard);

            return card;
        }





        private void ProcessReturnForCard(int slipItemId, string equipmentName, int equipmentId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int quantityRequested = 0;
                string equipmentType = "Reusable";
                bool hasSerial = false;

                string infoQuery = @"
SELECT BSI.QuantityRequested, E.EquipmentType, E.HasSerial
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipItemID = ?";

                using (OleDbCommand cmd = new OleDbCommand(infoQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipItemId);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader == null || !reader.Read()) { MessageBox.Show("Item not found."); return; }
                    quantityRequested = Convert.ToInt32(reader["QuantityRequested"]);
                    equipmentType = reader["EquipmentType"]?.ToString() ?? "Reusable";
                    hasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"]);
                }

                string finalUnitStatus = "Available";
                int quantityReturned = quantityRequested;

                if (equipmentType == "Consumable" || equipmentType == "One Time Use")
                {
                    DialogResult confirm = MessageBox.Show(
                        equipmentName + " is One Time Use — it will NOT return to stock.\n\nContinue?",
                        "Return One Time Use", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes) return;
                    finalUnitStatus = "Consumed";
                    quantityReturned = 0;
                }
                else if (equipmentType == "Limited Use")
                {
                    DialogResult result = MessageBox.Show(
                        equipmentName + " is Limited Use.\n\nYES = still usable (return to stock)\nNO = used up (do not return)\nCANCEL = abort",
                        "Limited Use Return", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return;
                    if (result == DialogResult.No) { finalUnitStatus = "Consumed"; quantityReturned = 0; }
                }

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    string updateItem = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Returned', QuantityReturned = ?
WHERE SlipItemID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(updateItem, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@p1", quantityReturned);
                        cmd.Parameters.AddWithValue("@p2", slipItemId);
                        cmd.ExecuteNonQuery();
                    }

                    if (hasSerial)
                    {
                        string updateUnits = @"
UPDATE EquipmentUnits SET UnitStatus = ?
WHERE UnitID IN (SELECT UnitID FROM BorrowSlipUnits WHERE SlipItemID = ?)";

                        using (OleDbCommand cmd = new OleDbCommand(updateUnits, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@p1", finalUnitStatus);
                            cmd.Parameters.AddWithValue("@p2", slipItemId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();

                    MessageBox.Show("Item returned successfully.",
                        "Return", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadBorrowedData();
                    LoadAdminDashboardNew();
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
                MessageBox.Show("Error processing return:\n" + ex.Message,
                    "Return", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void UpdateReservationCardStyles()
        {
            cardResPending.BorderStyle = BorderStyle.None;
            cardResClaimed.BorderStyle = BorderStyle.None;
            cardResUnclaimed.BorderStyle = BorderStyle.None;

            lblResShowAll.Font =
                new Font("Segoe UI Semibold", 9F, FontStyle.Regular);

            if (currentReservationFilter == "Pending")
                cardResPending.BorderStyle = BorderStyle.FixedSingle;
            else if (currentReservationFilter == "Approved")
                cardResClaimed.BorderStyle = BorderStyle.FixedSingle;
            else if (currentReservationFilter == "Declined")
                cardResUnclaimed.BorderStyle = BorderStyle.FixedSingle;
            else
                lblResShowAll.Font =
                    new Font("Segoe UI Semibold", 9F, FontStyle.Underline);
        }



        private void cardResPending_Click(object? sender, EventArgs e)
        {
            currentReservationFilter =
                currentReservationFilter == "Pending"
                ? "All"
                : "Pending";

            LoadReservationsData();
        }

        private void cardResClaimed_Click(object? sender, EventArgs e)
        {
            currentReservationFilter =
                currentReservationFilter == "Approved"
                ? "All"
                : "Approved";

            LoadReservationsData();
        }

        private void cardResUnclaimed_Click(object? sender, EventArgs e)
        {
            currentReservationFilter =
                currentReservationFilter == "Declined"
                ? "All"
                : "Declined";

            LoadReservationsData();
        }

        private void btnClaim_Click(object? sender, EventArgs e)
        {
            if (dgvReservations.CurrentRow == null)
            {
                MessageBox.Show("Select a pending slip first.");
                return;
            }

            int slipId = Convert.ToInt32(
                dgvReservations.CurrentRow.Cells["SlipID"].Value);

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string checkQuery = @"
SELECT SlipStatus
FROM BorrowSlips
WHERE SlipID = ?";

                using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;

                    string status = checkCmd.ExecuteScalar()?.ToString() ?? "";

                    if (status != "Pending")
                    {
                        MessageBox.Show("Only pending slips can be approved.");
                        return;
                    }
                }

                DialogResult confirm = MessageBox.Show(
                    "Approve this borrower slip?",
                    "Approve Slip",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                List<PendingSlipItemForApproval> items =
                    new List<PendingSlipItemForApproval>();

                string itemQuery = @"
SELECT
    BSI.SlipItemID,
    BSI.EquipmentID,
    BSI.QuantityRequested,
    E.EquipmentName,
    E.EquipmentType,
    E.HasSerial
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E
ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?";

                using (OleDbCommand itemLoadCmd = new OleDbCommand(itemQuery, conn))
                {
                    itemLoadCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;

                    using OleDbDataReader reader = itemLoadCmd.ExecuteReader();

                    while (reader != null && reader.Read())
                    {
                        items.Add(new PendingSlipItemForApproval
                        {
                            SlipItemID = Convert.ToInt32(reader["SlipItemID"]),
                            EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                            EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                            QuantityRequested = Convert.ToInt32(reader["QuantityRequested"]),
                            HasSerial = CanRequireSerialAssignment(
                                reader["EquipmentType"]?.ToString() ?? "",
                                reader["HasSerial"] != DBNull.Value &&
                                Convert.ToBoolean(reader["HasSerial"]))
                        });
                    }
                }

                foreach (PendingSlipItemForApproval item in items)
                {
                    if (!item.HasSerial)
                        continue;

                    int availableSerials = CountAvailableSerialUnits(item.EquipmentID);

                    if (availableSerials < item.QuantityRequested)
                    {
                        MessageBox.Show(item.EquipmentName + " does not have enough serial units.");
                        return;
                    }

                    List<int>? selectedUnits = ShowSerialSelectionDialog(
                        item.EquipmentID,
                        item.EquipmentName,
                        item.QuantityRequested);

                    if (selectedUnits == null ||
                        selectedUnits.Count != item.QuantityRequested)
                    {
                        MessageBox.Show("Approval cancelled.");
                        return;
                    }

                    item.SelectedUnitIDs = selectedUnits;
                }

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    foreach (PendingSlipItemForApproval item in items)
                    {
                        if (!item.HasSerial)
                            continue;

                        foreach (int unitId in item.SelectedUnitIDs)
                        {
                            string insertUnitQuery = @"
INSERT INTO BorrowSlipUnits
(SlipItemID, UnitID, DateAssigned)
VALUES (?, ?, ?)";

                            using (OleDbCommand unitCmd =
                                new OleDbCommand(insertUnitQuery, conn, trans))
                            {
                                unitCmd.Parameters.Add("@p1", OleDbType.Integer).Value = item.SlipItemID;
                                unitCmd.Parameters.Add("@p2", OleDbType.Integer).Value = unitId;
                                unitCmd.Parameters.Add("@p3", OleDbType.DBTimeStamp).Value = DateTime.Now;
                                unitCmd.ExecuteNonQuery();
                            }

                            string updateUnitQuery = @"
UPDATE EquipmentUnits
SET UnitStatus = ?
WHERE UnitID = ?";

                            using (OleDbCommand unitStatusCmd =
                                new OleDbCommand(updateUnitQuery, conn, trans))
                            {
                                unitStatusCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = "Borrowed";
                                unitStatusCmd.Parameters.Add("@p2", OleDbType.Integer).Value = unitId;
                                unitStatusCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    string approveQuery = "UPDATE BorrowSlips SET SlipStatus = 'Approved' WHERE SlipID = ?";
                    using (OleDbCommand approveCmd = new OleDbCommand(approveQuery, conn, trans))
                    {
                        approveCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                        approveCmd.ExecuteNonQuery();
                    }

                    string updateSlipItemsQuery = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = ?
WHERE SlipID = ?";

                    using (OleDbCommand slipItemCmd =
                        new OleDbCommand(updateSlipItemsQuery, conn, trans))
                    {
                        slipItemCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = "Borrowed";
                        slipItemCmd.Parameters.Add("@p2", OleDbType.Integer).Value = slipId;
                        slipItemCmd.ExecuteNonQuery();
                    }

                    trans.Commit();

                    MessageBox.Show("Borrower slip approved successfully.");

                    LoadReservationsData();
                    LoadBorrowedData();
                    LoadEquipmentCards(currentEquipmentCategory);
                    LoadAdminDashboardNew();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving slip:\n" + ex.Message);
            }
        }




        private void SendOverdueReminders()
        {
            MessageBox.Show(
                "Overdue reminders are disabled in the new lab-based system because borrowing is handled during laboratory time.",
                "Overdue Reminder",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }




        private void InitializeBorrowedAdminButtons()
        {
            btnReturn.Visible = false;

            if (btnReportDamageDynamic == null)
            {
                btnReportDamageDynamic = new Button();
                btnReportDamageDynamic.Name = "btnReportDamageDynamic";
                btnReportDamageDynamic.Text = "Report";
                btnReportDamageDynamic.Size = new Size(140, 38);
                btnReportDamageDynamic.BackColor = Color.FromArgb(220, 95, 107);
                btnReportDamageDynamic.ForeColor = Color.White;
                btnReportDamageDynamic.FlatStyle = FlatStyle.Flat;
                btnReportDamageDynamic.FlatAppearance.BorderSize = 0;
                btnReportDamageDynamic.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                btnReportDamageDynamic.Click += btnReportDamageDynamic_Click;

                pnlBorrowedMain.Controls.Add(btnReportDamageDynamic);

                ApplyButtonStyle(btnReportDamageDynamic);
                RoundControl(btnReportDamageDynamic, 16);
                InitializeBorrowedReportsDrawerButton();
            }

            btnReportDamageDynamic.Visible = false;
        }




        private void InitializeBorrowedReportsDrawerButton()
        {
            if (btnOpenBorrowedReportsDrawer != null)
                return;

            btnOpenBorrowedReportsDrawer = new Button();
            btnOpenBorrowedReportsDrawer.Name = "btnOpenBorrowedReportsDrawer";
            btnOpenBorrowedReportsDrawer.Text = "Reports";
            btnOpenBorrowedReportsDrawer.Size = new Size(130, 34);
            btnOpenBorrowedReportsDrawer.Location = new Point(850, 55);
            btnOpenBorrowedReportsDrawer.BackColor = Color.FromArgb(212, 168, 45);
            btnOpenBorrowedReportsDrawer.ForeColor = Color.White;
            btnOpenBorrowedReportsDrawer.FlatStyle = FlatStyle.Flat;
            btnOpenBorrowedReportsDrawer.FlatAppearance.BorderSize = 0;
            btnOpenBorrowedReportsDrawer.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);

            btnOpenBorrowedReportsDrawer.Click += (s, e) =>
            {
                ToggleBorrowedReportsDrawer();
            };

            pnlBorrowedMain.Controls.Add(btnOpenBorrowedReportsDrawer);
            btnOpenBorrowedReportsDrawer.BringToFront();

            ApplyButtonStyle(btnOpenBorrowedReportsDrawer);
            RoundControl(btnOpenBorrowedReportsDrawer, 16);
        }



        private void SetupBorrowedReportsDrawer()
        {
            if (pnlBorrowedReportsDrawer != null)
                return;

            pnlBorrowedReportsDrawer = new Panel();
            pnlBorrowedReportsDrawer.Name = "pnlBorrowedReportsDrawer";
            pnlBorrowedReportsDrawer.Size = new Size(390, pnlBorrowedMain.Height - 20);
            pnlBorrowedReportsDrawer.Location = new Point(pnlBorrowedMain.Width, 10);
            pnlBorrowedReportsDrawer.BackColor = Color.FromArgb(255, 251, 252);
            pnlBorrowedReportsDrawer.Visible = false;
            pnlBorrowedReportsDrawer.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            Label lblTitle = new Label();
            lblTitle.Text = "Reports";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblTitle.Location = new Point(22, 20);
            lblTitle.AutoSize = true;

            btnCloseBorrowedReportsDrawer = new Button();
            btnCloseBorrowedReportsDrawer.Text = "×";
            btnCloseBorrowedReportsDrawer.Size = new Size(35, 35);
            btnCloseBorrowedReportsDrawer.Location = new Point(340, 16);
            btnCloseBorrowedReportsDrawer.FlatStyle = FlatStyle.Flat;
            btnCloseBorrowedReportsDrawer.FlatAppearance.BorderSize = 0;
            btnCloseBorrowedReportsDrawer.BackColor = Color.Transparent;
            btnCloseBorrowedReportsDrawer.ForeColor = Color.FromArgb(92, 45, 58);
            btnCloseBorrowedReportsDrawer.Font = new Font("Segoe UI", 16F, FontStyle.Bold);

            btnCloseBorrowedReportsDrawer.Click += (s, e) =>
            {
                CloseBorrowedReportsDrawer();
            };

            Label lblFilter = new Label();
            lblFilter.Text = "Filter reports";
            lblFilter.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblFilter.ForeColor = Color.FromArgb(72, 53, 84);
            lblFilter.Location = new Point(24, 70);
            lblFilter.AutoSize = true;

            cmbBorrowedReportFilter = new ComboBox();
            cmbBorrowedReportFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBorrowedReportFilter.Font = new Font("Segoe UI", 9.5F);
            cmbBorrowedReportFilter.Location = new Point(24, 96);
            cmbBorrowedReportFilter.Size = new Size(335, 26);

            cmbBorrowedReportFilter.Items.Add("View All");
            cmbBorrowedReportFilter.Items.Add("Pending Cost");
            cmbBorrowedReportFilter.Items.Add("For Payment");
            cmbBorrowedReportFilter.Items.Add("Paid");
            cmbBorrowedReportFilter.SelectedIndex = 0;

            cmbBorrowedReportFilter.SelectedIndexChanged += (s, e) =>
            {
                LoadBorrowedReportsDrawerList();
            };

            flowBorrowedReportsList = new FlowLayoutPanel();
            flowBorrowedReportsList.Location = new Point(22, 140);
            flowBorrowedReportsList.Size = new Size(345, pnlBorrowedMain.Height - 175);
            flowBorrowedReportsList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowBorrowedReportsList.AutoScroll = true;
            flowBorrowedReportsList.FlowDirection = FlowDirection.TopDown;
            flowBorrowedReportsList.WrapContents = false;
            flowBorrowedReportsList.BackColor = Color.Transparent;

            pnlBorrowedReportsDrawer.Controls.Add(lblTitle);
            pnlBorrowedReportsDrawer.Controls.Add(btnCloseBorrowedReportsDrawer);
            pnlBorrowedReportsDrawer.Controls.Add(lblFilter);
            pnlBorrowedReportsDrawer.Controls.Add(cmbBorrowedReportFilter);
            pnlBorrowedReportsDrawer.Controls.Add(flowBorrowedReportsList);

            pnlBorrowedMain.Controls.Add(pnlBorrowedReportsDrawer);
            pnlBorrowedReportsDrawer.BringToFront();

            RoundControl(pnlBorrowedReportsDrawer, 28);
        }



        private void ToggleBorrowedReportsDrawer()
        {
            SetupBorrowedReportsDrawer();

            if (isBorrowedReportsDrawerOpen)
                CloseBorrowedReportsDrawer();
            else
                OpenBorrowedReportsDrawer();
        }

        private void OpenBorrowedReportsDrawer()
        {
            SetupBorrowedReportsDrawer();

            pnlBorrowedReportsDrawer.Visible = true;
            pnlBorrowedReportsDrawer.Location = new Point(
                pnlBorrowedMain.Width - pnlBorrowedReportsDrawer.Width - 10,
                10
            );

            pnlBorrowedReportsDrawer.Height = pnlBorrowedMain.Height - 20;
            flowBorrowedReportsList.Height = pnlBorrowedReportsDrawer.Height - 175;

            pnlBorrowedReportsDrawer.BringToFront();

            isBorrowedReportsDrawerOpen = true;

            LoadBorrowedReportsDrawerList();
        }

        private void CloseBorrowedReportsDrawer()
        {
            if (pnlBorrowedReportsDrawer == null)
                return;

            pnlBorrowedReportsDrawer.Visible = false;
            isBorrowedReportsDrawerOpen = false;
        }




        private void LoadBorrowedReportsDrawerList()
        {
            if (flowBorrowedReportsList == null)
                return;

            flowBorrowedReportsList.Controls.Clear();

            try
            {
                string selectedFilter =
                    cmbBorrowedReportFilter?.SelectedItem?.ToString() ?? "View All";

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    DR.ReportID,
    DR.SlipID,
    DR.DamageType,
    DR.DamageQuantity,
    DR.DateReported,
    DR.CurrentReplacementCost,
    DR.IndividualShare,
    DR.ReportStatus,
    E.EquipmentName,
    LS.SubjectCode,
    BS.GroupNumber,
    BS.LeaderName
FROM (((DamageReports AS DR
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN BorrowSlips AS BS ON DR.SlipID = BS.SlipID)
WHERE 1 = 1";

                if (selectedFilter != "View All")
                    query += " AND DR.ReportStatus = ?";

                query += selectedFilter == "View All"
                    ? " ORDER BY IIF(DR.ReportStatus = 'Paid', 1, 0), DR.DateReported DESC"
                    : " ORDER BY DR.DateReported DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);

                if (selectedFilter != "View All")
                    cmd.Parameters.AddWithValue("@p1", selectedFilter);

                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasReports = false;

                while (reader != null && reader.Read())
                {
                    int reportId = Convert.ToInt32(reader["ReportID"]);

                    string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                    string damageType = reader["DamageType"]?.ToString() ?? "";
                    string subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    string groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    string leaderName = reader["LeaderName"]?.ToString() ?? "";
                    string status = reader["ReportStatus"]?.ToString() ?? "";

                    int qty = reader["DamageQuantity"] != DBNull.Value
                        ? Convert.ToInt32(reader["DamageQuantity"])
                        : 1;

                    decimal cost = reader["CurrentReplacementCost"] != DBNull.Value
                        ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                        : 0;

                    Panel card = CreateBorrowedReportDrawerCard(
                        reportId,
                        equipmentName,
                        damageType,
                        qty,
                        subjectCode,
                        groupNumber,
                        leaderName,
                        status,
                        cost
                    );

                    flowBorrowedReportsList.Controls.Add(card);
                    hasReports = true;
                }

                if (!hasReports)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No reports found.";
                    lblEmpty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    lblEmpty.ForeColor = Color.FromArgb(126, 105, 136);
                    lblEmpty.Size = new Size(260, 60);
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;

                    flowBorrowedReportsList.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading reports:\n" + ex.Message,
                    "Reports",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private Panel CreateBorrowedReportDrawerCard(
    int reportId,
    string equipmentName,
    string damageType,
    int quantity,
    string subjectCode,
    string groupNumber,
    string leaderName,
    string status,
    decimal replacementCost)
        {
            string serialNumbers = GetReportSerialNumbers(reportId);

            Panel card = new Panel
            {
                Width = 350,
                Height = 165,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12),
                Cursor = Cursors.Hand
            };

            RoundControl(card, 18);

            Label lblEquipment = new Label
            {
                Text = equipmentName,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(16, 14),
                Size = new Size(250, 24)
            };

            Label lblSubject = new Label
            {
                Text = subjectCode + " • Group " + groupNumber,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(16, 40),
                Size = new Size(220, 18)
            };

            Label lblLeader = new Label
            {
                Text = "Leader: " + leaderName,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(16, 60),
                Size = new Size(250, 18)
            };

            Label lblDamage = new Label
            {
                Text = damageType + " • Qty: " + quantity,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = damageType == "Lost" ? Color.Firebrick : Color.DarkOrange,
                Location = new Point(16, 84),
                Size = new Size(160, 20)
            };

            Label lblSerials = new Label
            {
                Text = "Serial No.: " + serialNumbers,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(92, 45, 58),
                Location = new Point(16, 106),
                Size = new Size(310, 18)
            };

            Label lblCost = new Label
            {
                Text = "₱ " + replacementCost.ToString("N2"),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                AutoSize = true,
                Location = new Point(16, 130)
            };

            Label lblStatus = new Label
            {
                Text = status,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                ForeColor = GetReportStatusColor(status),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(208, 130),
                Size = new Size(120, 22)
            };

            card.Controls.Add(lblEquipment);
            card.Controls.Add(lblSubject);
            card.Controls.Add(lblLeader);
            card.Controls.Add(lblDamage);
            card.Controls.Add(lblSerials);
            card.Controls.Add(lblCost);
            card.Controls.Add(lblStatus);

            card.Click += (s, e) =>
            {
                ShowDamageReportDetails(reportId);
            };

            foreach (Control ctrl in card.Controls)
            {
                ctrl.Click += (s, e) =>
                {
                    ShowDamageReportDetails(reportId);
                };
            }

            return card;
        }



        private void MarkReportAsPaid(int reportId)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "Mark this report as PAID?",
                    "Confirm Payment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    string updateMembers = @"
UPDATE DamageReportMembers
SET HasPaid = True,
    IsRestricted = False
WHERE ReportID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(updateMembers, conn, trans))
                    {
                        cmd.Parameters.Add("@p1", OleDbType.Integer).Value = reportId;
                        cmd.ExecuteNonQuery();
                    }

                    string updateReport = @"
UPDATE DamageReports
SET ReportStatus = 'Paid',
    DateResolved = ?
WHERE ReportID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(updateReport, conn, trans))
                    {
                        cmd.Parameters.Add("@p1", OleDbType.Date).Value = DateTime.Now.Date;
                        cmd.Parameters.Add("@p2", OleDbType.Integer).Value = reportId;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();

                    MessageBox.Show(
                        "Report marked as PAID.",
                        "Payment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadBorrowedReportsDrawerList();
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
                    "Error marking report as paid:\n" + ex.Message,
                    "Payment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private Color GetReportStatusColor(string status)
        {
            if (status == "Paid")
                return Color.FromArgb(45, 120, 60);

            if (status == "For Payment")
                return Color.FromArgb(180, 110, 20);

            if (status == "Pending Cost")
                return Color.FromArgb(150, 90, 140);

            if (status == "Reported")
                return Color.FromArgb(180, 70, 80);

            return Color.FromArgb(92, 45, 58);
        }



        private void InitializeReportsNavButton()
        {
            if (btnNavReportsDynamic != null)
                return;

            btnNavReportsDynamic = new Button();
            btnNavReportsDynamic.Name = "btnNavReportsDynamic";
            btnNavReportsDynamic.Text = "🧾  Reports";
            btnNavReportsDynamic.Size = new Size(211, 49);
            btnNavReportsDynamic.Location = new Point(24, 613);
            btnNavReportsDynamic.BackColor = Color.FromArgb(212, 168, 45);
            btnNavReportsDynamic.ForeColor = Color.White;
            btnNavReportsDynamic.FlatStyle = FlatStyle.Flat;
            btnNavReportsDynamic.FlatAppearance.BorderSize = 0;
            btnNavReportsDynamic.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNavReportsDynamic.Padding = new Padding(18, 0, 0, 0);
            btnNavReportsDynamic.TextAlign = ContentAlignment.MiddleLeft;
            btnNavReportsDynamic.Click += btnNavReportsDynamic_Click;

            sidebarPanel.Controls.Add(btnNavReportsDynamic);
            btnNavReportsDynamic.BringToFront();

            ApplyButtonStyle(btnNavReportsDynamic);
            RoundControl(btnNavReportsDynamic, 18);

            btnLogout.Location = new Point(33, 693);
        }


        private void btnNavReportsDynamic_Click(object sender, EventArgs e)
        {
            ShowDamageReportsPanel();
        }



        private void ShowDamageReportsPanel()
        {
            panelDashboard.Visible = false;
            panelVerification.Visible = false;
            panelEquipment.Visible = false;
            panelBorrowed.Visible = false;
            panelReservations.Visible = false;
            panelHistory.Visible = false;

            if (pnlExperimentManualAdmin != null)
                pnlExperimentManualAdmin.Visible = false;

            lblWelcome.Text = "Damage Reports";

            ResetSidebarButtons();
            SetActiveButton(btnNavReportsDynamic);

            if (pnlDamageReportsAdmin != null)
            {
                pnlDamageReportsAdmin.Visible = true;
                pnlDamageReportsAdmin.BringToFront();
                LoadDamageReportsList();
                return;
            }

            pnlDamageReportsAdmin = new Panel();
            pnlDamageReportsAdmin.Dock = DockStyle.Fill;
            pnlDamageReportsAdmin.BackColor = Color.FromArgb(245, 240, 245);

            contentPanel.Controls.Add(pnlDamageReportsAdmin);
            pnlDamageReportsAdmin.BringToFront();

            Panel mainCard = new Panel();
            mainCard.BackColor = Color.WhiteSmoke;
            mainCard.Location = new Point(34, 27);
            mainCard.Size = new Size(1040, 600);

            Label lblTitle = new Label();
            lblTitle.Text = "DAMAGE / LOST REPORTS";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(30, 25);
            lblTitle.AutoSize = true;

            flowDamageReportList = new FlowLayoutPanel();
            flowDamageReportList.Location = new Point(30, 82);
            flowDamageReportList.Size = new Size(980, 485);
            flowDamageReportList.AutoScroll = true;
            flowDamageReportList.FlowDirection = FlowDirection.TopDown;
            flowDamageReportList.WrapContents = false;
            flowDamageReportList.BackColor = Color.Transparent;

            mainCard.Controls.Add(lblTitle);
            mainCard.Controls.Add(flowDamageReportList);

            pnlDamageReportsAdmin.Controls.Add(mainCard);

            RoundControl(mainCard, 28);

            LoadDamageReportsList();
        }



        private void LoadDamageReportsList()
        {
            if (flowDamageReportList == null)
                return;

            flowDamageReportList.Controls.Clear();

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    DR.ReportID,
    DR.SlipID,
    DR.DamageType,
    DR.DamageQuantity,
    DR.DateReported,
    DR.CurrentReplacementCost,
    DR.IndividualShare,
    DR.ReportStatus,
    E.EquipmentName,
    LS.SubjectCode,
    BS.GroupNumber,
    BS.LeaderName
FROM (((DamageReports AS DR
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN BorrowSlips AS BS ON DR.SlipID = BS.SlipID)
ORDER BY IIF(DR.ReportStatus = 'Paid', 1, 0), DR.DateReported DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                using OleDbDataReader reader = cmd.ExecuteReader();

                bool hasReports = false;

                while (reader != null && reader.Read())
                {
                    int reportId = Convert.ToInt32(reader["ReportID"]);
                    string equipmentName = reader["EquipmentName"]?.ToString() ?? "";
                    string damageType = reader["DamageType"]?.ToString() ?? "";
                    string subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    string groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    string leaderName = reader["LeaderName"]?.ToString() ?? "";
                    string status = reader["ReportStatus"]?.ToString() ?? "";
                    int qty = reader["DamageQuantity"] != DBNull.Value ? Convert.ToInt32(reader["DamageQuantity"]) : 1;

                    decimal cost = reader["CurrentReplacementCost"] != DBNull.Value
                        ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                        : 0;

                    Panel card = CreateDamageReportCard(
                        reportId,
                        equipmentName,
                        damageType,
                        qty,
                        subjectCode,
                        groupNumber,
                        leaderName,
                        status,
                        cost);

                    flowDamageReportList.Controls.Add(card);
                    hasReports = true;
                }

                if (!hasReports)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No damage/lost reports yet.";
                    lblEmpty.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                    lblEmpty.ForeColor = Color.FromArgb(126, 105, 136);
                    lblEmpty.Size = new Size(900, 60);
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;

                    flowDamageReportList.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reports:\n" + ex.Message);
            }
        }




        private Panel CreateDamageReportCard(
    int reportId,
    string equipmentName,
    string damageType,
    int qty,
    string subjectCode,
    string groupNumber,
    string leaderName,
    string status,
    decimal cost)
        {
            Panel card = new Panel();
            card.Size = new Size(940, 92);
            card.BackColor = Color.FromArgb(255, 251, 252);
            card.Margin = new Padding(0, 0, 0, 14);
            card.Cursor = Cursors.Hand;

            Label lblTitle = new Label();
            lblTitle.Text = equipmentName + " (" + damageType + ") x" + qty;
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(18, 12);
            lblTitle.Size = new Size(420, 24);

            Label lblInfo = new Label();
            lblInfo.Text = subjectCode + " | Group " + groupNumber + " | Leader: " + leaderName;
            lblInfo.Font = new Font("Segoe UI", 9.5F);
            lblInfo.ForeColor = Color.FromArgb(126, 105, 136);
            lblInfo.Location = new Point(18, 42);
            lblInfo.Size = new Size(520, 22);

            Label lblStatus = new Label();
            lblStatus.Text = "Status: " + status;
            lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblStatus.ForeColor = status == "Paid" || status == "Resolved"
                ? Color.FromArgb(45, 120, 60)
                : Color.FromArgb(160, 98, 27);
            lblStatus.Location = new Point(620, 18);
            lblStatus.Size = new Size(250, 22);

            Label lblCost = new Label();
            lblCost.Text = cost > 0 ? "Cost: ₱" + cost.ToString("N2") : "Cost: Not set";
            lblCost.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCost.ForeColor = Color.FromArgb(92, 45, 58);
            lblCost.Location = new Point(620, 45);
            lblCost.Size = new Size(250, 22);

            card.Click += (s, e) =>
            {
                ShowDamageReportDetails(reportId);
            };

            foreach (Control ctrl in new Control[] { lblTitle, lblInfo, lblStatus, lblCost })
            {
                ctrl.Click += (s, e) =>
                {
                    ShowDamageReportDetails(reportId);
                };
            }

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblInfo);
            card.Controls.Add(lblStatus);
            card.Controls.Add(lblCost);

            RoundControl(card, 18);

            return card;
        }



        private void ShowDamageReportDetails(int reportId)
        {
            try
            {
                int totalMembers = 0;
                decimal currentCost = 0;
                string status = "";
                string evidenceImagePath = "";
                string serialNumbers = GetReportSerialNumbers(reportId);
                string slipIdText = "";
                string subjectCodeText = "";
                string groupNumberText = "";
                string leaderNameText = "";
                string equipmentNameText = "";
                string damageTypeText = "";
                string damageQuantityText = "";
                string dateReportedText = "";
                string descriptionText = "";

                using (OleDbConnection conn = DbHelper.GetConnection())
                {
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
    DR.EvidenceImagePath,
    E.EquipmentName,
    LS.SubjectCode,
    BS.GroupNumber,
    BS.LeaderName
FROM (((DamageReports AS DR
INNER JOIN Equipment AS E ON DR.EquipmentID = E.EquipmentID)
INNER JOIN LabSubjects AS LS ON DR.SubjectID = LS.SubjectID)
INNER JOIN BorrowSlips AS BS ON DR.SlipID = BS.SlipID)
WHERE DR.ReportID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", reportId);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader == null || !reader.Read())
                            {
                                MessageBox.Show("Report not found.");
                                return;
                            }

                            currentCost = reader["CurrentReplacementCost"] != DBNull.Value
                                ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                                : 0;

                            status = reader["ReportStatus"]?.ToString() ?? "";

                            evidenceImagePath =
                                reader["EvidenceImagePath"] != DBNull.Value
                                ? reader["EvidenceImagePath"].ToString(): "";

                            if (status == "Reported")
                            {
                                MarkDamageReportPendingCost(reportId);
                                status = "Pending Cost";
                            }

                            slipIdText = reader["SlipID"]?.ToString() ?? "";
                            subjectCodeText = reader["SubjectCode"]?.ToString() ?? "";
                            groupNumberText = reader["GroupNumber"]?.ToString() ?? "";
                            leaderNameText = reader["LeaderName"]?.ToString() ?? "";
                            equipmentNameText = reader["EquipmentName"]?.ToString() ?? "";
                            damageTypeText = reader["DamageType"]?.ToString() ?? "";
                            damageQuantityText = reader["DamageQuantity"]?.ToString() ?? "";
                            dateReportedText = Convert.ToDateTime(reader["DateReported"]).ToString("MMMM dd, yyyy");
                            descriptionText = reader["Description"]?.ToString() ?? "";
                        }
                    }

                    using (OleDbCommand countCmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM DamageReportMembers WHERE ReportID = ?", conn))
                    {
                        countCmd.Parameters.AddWithValue("@p1", reportId);

                        object countResult = countCmd.ExecuteScalar();
                        totalMembers = countResult != null && countResult != DBNull.Value
                            ? Convert.ToInt32(countResult)
                            : 0;
                    }
                }

                Form detailsForm = new Form();
                detailsForm.Text = "Damage Report Details";
                detailsForm.StartPosition = FormStartPosition.CenterParent;
                detailsForm.Size = new Size(650, 700);
                detailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                detailsForm.MaximizeBox = false;
                detailsForm.MinimizeBox = false;
                detailsForm.BackColor = Color.FromArgb(250, 245, 247);

                Label lblTitle = new Label();
                lblTitle.Text = "Damage Report Details";
                lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
                lblTitle.Location = new Point(28, 22);
                lblTitle.AutoSize = true;

                Panel pnlReportInfo = new Panel
                {
                    Location = new Point(30, 68),
                    Size = new Size(570, 120),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Panel pnlEquipmentInfo = new Panel
                {
                    Location = new Point(30, 198),
                    Size = new Size(570, 145),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                void AddInfoRow(Panel panel, string labelText, string valueText, int y)
                {
                    Label label = new Label
                    {
                        Text = labelText,
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(69, 45, 96),
                        Location = new Point(14, y),
                        Size = new Size(112, 22),
                        TextAlign = ContentAlignment.TopLeft
                    };

                    Label value = new Label
                    {
                        Text = string.IsNullOrWhiteSpace(valueText) ? "N/A" : valueText,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Location = new Point(136, y),
                        Size = new Size(410, labelText == "Serial No." ? 42 : 22),
                        AutoEllipsis = true
                    };

                    panel.Controls.Add(label);
                    panel.Controls.Add(value);
                }

                AddInfoRow(pnlReportInfo, "Report ID", reportId.ToString(), 12);
                AddInfoRow(pnlReportInfo, "Slip ID", slipIdText, 38);
                AddInfoRow(pnlReportInfo, "Subject", subjectCodeText, 64);
                AddInfoRow(pnlReportInfo, "Group", groupNumberText, 90);

                AddInfoRow(pnlEquipmentInfo, "Leader", leaderNameText, 12);
                AddInfoRow(pnlEquipmentInfo, "Equipment", equipmentNameText, 38);
                AddInfoRow(pnlEquipmentInfo, "Serial No.", string.IsNullOrWhiteSpace(serialNumbers) ? "N/A" : serialNumbers, 64);
                AddInfoRow(pnlEquipmentInfo, "Damage Type", damageTypeText, 104);
                AddInfoRow(pnlEquipmentInfo, "Quantity", damageQuantityText, 128);

                Label lblStatus = new Label
                {
                    Text = status,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = status == "Paid"
                        ? Color.FromArgb(90, 158, 106)
                        : status == "For Payment"
                            ? Color.FromArgb(192, 57, 75)
                            : Color.FromArgb(212, 168, 45),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(475, 23),
                    Size = new Size(125, 28)
                };
                RoundControl(lblStatus, 14);

                Label lblDate = new Label
                {
                    Text = "Reported: " + dateReportedText,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Location = new Point(365, 54),
                    Size = new Size(230, 22),
                    TextAlign = ContentAlignment.TopRight
                };

                Label lblDetailsTitle = new Label
                {
                    Text = "Details",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(30, 356),
                    AutoSize = true
                };

                TextBox txtDetails = new TextBox
                {
                    Text = descriptionText,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(30, 382),
                    Size = new Size(360, 92),
                    BackColor = Color.White,
                    HideSelection = true,
                    TabStop = false
                };

                Label lblCost = new Label();
                lblCost.Text = "Replacement Cost:";
                lblCost.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblCost.ForeColor = Color.FromArgb(69, 45, 96);
                lblCost.Location = new Point(30, 502);
                lblCost.AutoSize = true;

                NumericUpDown numCost = new NumericUpDown();
                numCost.DecimalPlaces = 2;
                numCost.Maximum = 1000000;
                numCost.Minimum = 0;
                numCost.Value = currentCost;
                numCost.Font = new Font("Segoe UI", 10F);
                numCost.Location = new Point(170, 498);
                numCost.Size = new Size(160, 30);
                numCost.Enabled = status != "Paid";

                Label lblMembers = new Label();
                lblMembers.Text = "Members: " + totalMembers;
                lblMembers.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblMembers.ForeColor = Color.FromArgb(92, 45, 58);
                lblMembers.Location = new Point(30, 542);
                lblMembers.AutoSize = true;

                Label lblShare = new Label();
                lblShare.Text = totalMembers > 0
                    ? "Individual Share: ₱" + (numCost.Value / totalMembers).ToString("N2")
                    : "Individual Share: ₱0.00";
                lblShare.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblShare.ForeColor = Color.FromArgb(92, 45, 58);
                lblShare.Location = new Point(170, 542);
                lblShare.AutoSize = true;

                numCost.ValueChanged += (s, e) =>
                {
                    lblShare.Text = totalMembers > 0
                        ? "Individual Share: ₱" + (numCost.Value / totalMembers).ToString("N2")
                        : "Individual Share: ₱0.00";
                };

                Button btnSaveCost = new Button();
                btnSaveCost.Text = "Send Cost";
                btnSaveCost.Size = new Size(130, 38);
                btnSaveCost.Location = new Point(30, 592);
                btnSaveCost.BackColor = Color.FromArgb(212, 168, 45);
                btnSaveCost.ForeColor = Color.White;
                btnSaveCost.FlatStyle = FlatStyle.Flat;
                btnSaveCost.FlatAppearance.BorderSize = 0;
                btnSaveCost.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                btnSaveCost.Visible = status != "Paid" && status != "For Payment";

                btnSaveCost.Click += (s, e) =>
                {
                    SaveDamageReportCost(reportId, numCost.Value, totalMembers);
                    detailsForm.Close();
                };

                Button btnPaid = new Button();
                btnPaid.Text = "Mark as Paid";
                btnPaid.Size = new Size(130, 38);
                btnPaid.Location = new Point(170, 592);
                btnPaid.BackColor = Color.FromArgb(120, 190, 120);
                btnPaid.ForeColor = Color.White;
                btnPaid.FlatStyle = FlatStyle.Flat;
                btnPaid.FlatAppearance.BorderSize = 0;
                btnPaid.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                btnPaid.Visible = status == "For Payment";

                btnPaid.Click += (s, e) =>
                {
                    MarkReportAsPaid(reportId);
                    detailsForm.Close();
                };

                detailsForm.Controls.Add(lblTitle);
                detailsForm.Controls.Add(lblStatus);
                detailsForm.Controls.Add(lblDate);
                detailsForm.Controls.Add(pnlReportInfo);
                detailsForm.Controls.Add(pnlEquipmentInfo);
                detailsForm.Controls.Add(lblDetailsTitle);
                detailsForm.Controls.Add(txtDetails);
                detailsForm.Controls.Add(lblCost);
                detailsForm.Controls.Add(numCost);
                Button btnChooseEvidence = new Button();
                btnChooseEvidence.Text = "Choose Evidence Image";
                btnChooseEvidence.Size = new Size(190, 36);
                btnChooseEvidence.Location = new Point(410, 494);
                btnChooseEvidence.BackColor = Color.FromArgb(214, 197, 224);
                btnChooseEvidence.ForeColor = Color.FromArgb(87, 60, 99);
                btnChooseEvidence.FlatStyle = FlatStyle.Flat;
                btnChooseEvidence.FlatAppearance.BorderSize = 0;
                btnChooseEvidence.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
                btnChooseEvidence.Visible = status != "Paid";

                btnChooseEvidence.Click += (s, e) =>
                {
                    using OpenFileDialog ofd = new OpenFileDialog();

                    ofd.Filter =
                        "Image Files|*.jpg;*.jpeg;*.png";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        selectedReportEvidenceImage =
                            ofd.FileName;

                        MessageBox.Show(
                            "Evidence image selected.");
                    }
                };

                detailsForm.Controls.Add(btnChooseEvidence);

                RoundControl(btnChooseEvidence, 12);
                detailsForm.Controls.Add(lblMembers);
                detailsForm.Controls.Add(lblShare);
                detailsForm.Controls.Add(btnSaveCost);
                detailsForm.Controls.Add(btnPaid);

                RoundControl(btnSaveCost, 14);
                RoundControl(btnPaid, 14);

                if (!string.IsNullOrWhiteSpace(evidenceImagePath)
    && System.IO.File.Exists(evidenceImagePath))
                {
                    PictureBox picEvidence = new PictureBox();

                    picEvidence.Location = new Point(410, 382);
                    picEvidence.Size = new Size(190, 92);

                    picEvidence.SizeMode = PictureBoxSizeMode.Zoom;
                    picEvidence.BorderStyle = BorderStyle.FixedSingle;

                    picEvidence.Image =
                        Image.FromFile(evidenceImagePath);

                    detailsForm.Controls.Add(picEvidence);
                }

                detailsForm.Shown += (s, e) =>
                {
                    txtDetails.SelectionLength = 0;
                    detailsForm.ActiveControl = lblTitle;
                };
                detailsForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error opening report details:\n" + ex.Message,
                    "Report Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private void ShowAdminPaymentSlipPopup(int reportId)
        {
            try
            {
                string slipText = BuildAdminPaymentSlipText(reportId);

                if (string.IsNullOrWhiteSpace(slipText))
                    return;

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

                Button btnPrint = new Button();
                btnPrint.Text = "Print";
                btnPrint.Size = new Size(130, 38);
                btnPrint.Location = new Point(135, 592);
                btnPrint.BackColor = Color.FromArgb(212, 168, 45);
                btnPrint.ForeColor = Color.White;
                btnPrint.FlatStyle = FlatStyle.Flat;
                btnPrint.FlatAppearance.BorderSize = 0;
                btnPrint.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

                Button btnClose = new Button();
                btnClose.Text = "Close";
                btnClose.Size = new Size(130, 38);
                btnClose.Location = new Point(275, 592);
                btnClose.BackColor = Color.FromArgb(214, 197, 224);
                btnClose.ForeColor = Color.FromArgb(87, 60, 99);
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

                btnPrint.Click += (s, e) =>
                {
                    PrintAdminPaymentSlip(slipText);
                };

                btnClose.Click += (s, e) =>
                {
                    slipForm.Close();
                };

                slipForm.Controls.Add(lblTitle);
                slipForm.Controls.Add(pnlSlip);
                slipForm.Controls.Add(btnPrint);
                slipForm.Controls.Add(btnClose);
                slipForm.Shown += (s, e) => slipForm.ActiveControl = btnClose;

                RoundControl(btnPrint, 16);
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



        private string BuildAdminPaymentSlipText(int reportId)
        {
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
                return "";
            }

            decimal totalCost = reader["CurrentReplacementCost"] != DBNull.Value
                ? Convert.ToDecimal(reader["CurrentReplacementCost"])
                : 0;

            decimal individualShare = reader["IndividualShare"] != DBNull.Value
                ? Convert.ToDecimal(reader["IndividualShare"])
                : 0;

            if (totalCost <= 0)
            {
                MessageBox.Show(
                    "Please send the cost first before printing the payment slip.",
                    "Payment Slip",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return "";
            }

            return
                "WILDCATHUB PAYMENT SLIP\n" +
                "----------------------------------------\n\n" +
                "Report ID: " + reader["ReportID"] + "\n" +
                "Slip ID: " + reader["SlipID"] + "\n" +
                "Status: " + reader["ReportStatus"] + "\n\n" +

                "BORROWING INFORMATION\n" +
                "Leader: " + reader["LeaderName"] + "\n" +
                "Group Number: " + reader["GroupNumber"] + "\n" +
                "Subject: " + reader["SubjectCode"] + " - " + reader["SubjectName"] + "\n\n" +

                "REPORT INFORMATION\n" +
                "Equipment: " + reader["EquipmentName"] + "\n" +
                "Damage Type: " + reader["DamageType"] + "\n" +
                "Quantity: " + reader["DamageQuantity"] + "\n" +
                "Date Reported: " + Convert.ToDateTime(reader["DateReported"]).ToString("MMMM dd, yyyy") + "\n" +
                "Details: " + reader["Description"] + "\n\n" +

                "PAYMENT INFORMATION\n" +
                "Total Group Cost: PHP " + totalCost.ToString("N2") + "\n" +
                "Individual Share: PHP " + individualShare.ToString("N2") + "\n\n" +

                "NOTE:\n" +
                "Please bring this slip to the laboratory admin/NAS for validation/signature,\n" +
                "then proceed to the cashier for payment.\n\n" +

                "Admin/NAS Signature: ______________________\n\n" +
                "Cashier Receipt No.: ______________________\n" +
                "Date Paid: ________________________________";
        }


        private void PrintAdminPaymentSlip(string slipText)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                printDoc.PrintPage += (s, e) =>
                {
                    using Font printFont = new Font("Consolas", 10F);
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

        private void SaveDamageReportCost(int reportId, decimal totalCost, int totalMembers)
        {
            try
            {
                decimal individualShare = 0;

                if (totalMembers > 0)
                    individualShare = totalCost / totalMembers;

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
UPDATE DamageReports
SET
    CurrentReplacementCost = ?,
    IndividualShare = ?,
    EvidenceImagePath = ?,
    DateCostSet = ?,
    ReportStatus = 'For Payment'
WHERE ReportID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);

                cmd.Parameters.Add("@p1", OleDbType.Currency).Value = totalCost;
                cmd.Parameters.Add("@p2", OleDbType.Currency).Value = individualShare;
                string safeEvidence = (selectedReportEvidenceImage ?? "");
                if (safeEvidence.Length > 255) safeEvidence = safeEvidence.Substring(0, 255);
                cmd.Parameters.Add("@p3", OleDbType.VarWChar).Value = safeEvidence;
                cmd.Parameters.Add("@p4", OleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("@p5", OleDbType.Integer).Value = reportId;

                cmd.ExecuteNonQuery();

                string updateMembers = @"
UPDATE DamageReportMembers
SET AmountShare = ?,
    IsRestricted = True,
    HasPaid = False
WHERE ReportID = ?";

                using OleDbCommand memberCmd = new OleDbCommand(updateMembers, conn);
                memberCmd.Parameters.Add("@p1", OleDbType.Currency).Value = individualShare;
                memberCmd.Parameters.Add("@p2", OleDbType.Integer).Value = reportId;
                memberCmd.ExecuteNonQuery();

                MessageBox.Show(
                    "Replacement cost successfully sent to borrowers.",
                    "Damage Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadBorrowedReportsDrawerList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving replacement cost:\n" + ex.Message,
                    "Damage Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private void MarkDamageReportPaid(int reportId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbTransaction trans = conn.BeginTransaction();

            try
            {
                string updateReportQuery = @"
UPDATE DamageReports
SET ReportStatus = 'Paid',
    DateResolved = ?
WHERE ReportID = ?";

                using (OleDbCommand cmd = new OleDbCommand(updateReportQuery, conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Date).Value = DateTime.Now.Date;
                    cmd.Parameters.Add("@p2", OleDbType.Integer).Value = reportId;
                    cmd.ExecuteNonQuery();
                }

                string updateMembersQuery = @"
UPDATE DamageReportMembers
SET IsRestricted = False,
    HasPaid = True
WHERE ReportID = ?";

                using (OleDbCommand cmd = new OleDbCommand(updateMembersQuery, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@p1", reportId);
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }




  
        private List<(int SlipItemID, int ReportQuantity)> ShowReportQuantitySelectionDialog(
            int slipId,
            List<BorrowSlipReturnItem> items)
        {
            List<(int SlipItemID, int ReportQuantity)> selections = new List<(int SlipItemID, int ReportQuantity)>();

            List<BorrowSlipReturnItem> borrowedItems = items
                .Where(x => x.ItemReturnStatus == "Borrowed" && x.QuantityRequested > 0)
                .ToList();

            if (borrowedItems.Count == 0)
            {
                MessageBox.Show("No borrowed equipment is available to report.",
                    "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return selections;
            }

            using Form selectForm = new Form();
            selectForm.Text = "Choose Report Quantity";
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;
            selectForm.ClientSize = new Size(520, 420);
            selectForm.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label
            {
                Text = "Report",
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(28, 22),
                Size = new Size(420, 36)
            };

            Label lblGuide = new Label
            {
                Text = "Enter only the quantity that is lost, broken, damaged, or disposed.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(30, 60),
                Size = new Size(450, 24)
            };

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Location = new Point(30, 96),
                Size = new Size(460, 240),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Dictionary<int, NumericUpDown> quantityPickers = new Dictionary<int, NumericUpDown>();

            foreach (BorrowSlipReturnItem item in borrowedItems)
            {
                Panel row = new Panel
                {
                    Size = new Size(432, 58),
                    Margin = new Padding(8, 8, 8, 0),
                    BackColor = Color.FromArgb(255, 251, 252)
                };
                RoundControl(row, 10);

                Label lblName = new Label
                {
                    Text = item.EquipmentName,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(12, 8),
                    Size = new Size(260, 22)
                };

                Label lblBorrowed = new Label
                {
                    Text = "Borrowed / remaining: " + item.QuantityRequested,
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Location = new Point(12, 31),
                    Size = new Size(240, 18)
                };

                Label lblQty = new Label
                {
                    Text = "Report:",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(282, 18),
                    Size = new Size(58, 22)
                };

                NumericUpDown numReport = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = Math.Max(1, item.QuantityRequested),
                    Value = 0,
                    Location = new Point(344, 16),
                    Size = new Size(58, 24),
                    Font = new Font("Segoe UI", 9F)
                };

                quantityPickers[item.SlipItemID] = numReport;

                row.Controls.Add(lblName);
                row.Controls.Add(lblBorrowed);
                row.Controls.Add(lblQty);
                row.Controls.Add(numReport);
                flow.Controls.Add(row);
            }

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(150, 38),
                Location = new Point(170, 360),
                BackColor = Color.FromArgb(214, 197, 224),
                ForeColor = Color.FromArgb(69, 45, 96),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => selectForm.Close();

            Button btnNext = new Button
            {
                Text = "Next",
                Size = new Size(150, 38),
                Location = new Point(340, 360),
                BackColor = Color.FromArgb(192, 57, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += (s, e) =>
            {
                selections.Clear();

                foreach (BorrowSlipReturnItem item in borrowedItems)
                {
                    int reportQuantity = (int)quantityPickers[item.SlipItemID].Value;
                    if (reportQuantity <= 0)
                        continue;

                    if (reportQuantity > item.QuantityRequested)
                    {
                        MessageBox.Show(
                            "Report quantity cannot be greater than the remaining borrowed quantity.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    selections.Add((item.SlipItemID, reportQuantity));
                }

                if (selections.Count == 0)
                {
                    MessageBox.Show("Enter a report quantity for at least one equipment.",
                        "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectForm.DialogResult = DialogResult.OK;
                selectForm.Close();
            };

            RoundControl(btnCancel, 14);
            RoundControl(btnNext, 14);

            selectForm.Controls.Add(lblTitle);
            selectForm.Controls.Add(lblGuide);
            selectForm.Controls.Add(flow);
            selectForm.Controls.Add(btnCancel);
            selectForm.Controls.Add(btnNext);

            DialogResult result = selectForm.ShowDialog(this);
            if (result != DialogResult.OK)
                selections.Clear();

            return selections;
        }

        private void ShowDamageReportForm(int selectedSlipItemId, int forcedReportQuantity = 0, int remainingReturnQuantity = -1)
        {
            try
            {
                int slipId = 0;
                int subjectId = 0;
                int scheduleId = 0;
                string leaderName = "";
                string groupNumber = "";
                string subjectCode = "";
                string section = "";
                string borrowedDate = "";
                List<string> members = new List<string>();
                List<DamageReportItemChoice> itemChoices = new List<DamageReportItemChoice>();

                using (OleDbConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string slipQuery = @"
SELECT
    BSI.SlipID,
    BS.SubjectID,
    BS.LeaderName,
    BS.GroupNumber,
    BS.DateCreated,
    LS.SubjectCode
FROM ((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
WHERE BSI.SlipItemID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(slipQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", selectedSlipItemId);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.Read())
                            {
                                slipId = Convert.ToInt32(reader["SlipID"]);
                                subjectId = Convert.ToInt32(reader["SubjectID"]);
                                leaderName = reader["LeaderName"]?.ToString() ?? "";
                                groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                                subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                                borrowedDate = reader["DateCreated"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["DateCreated"]).ToString("MMMM dd, yyyy")
                                    : "";
                            }
                        }
                    }

                    string sectionQuery = @"
SELECT TOP 1 SS.ScheduleID, SS.Section
FROM ((StudentSubjectEnrollments AS SSE
INNER JOIN SubjectSchedules AS SS ON SSE.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlips AS BS ON SSE.UserID = BS.UserID)
WHERE BS.SlipID = ?
AND SSE.SubjectID = BS.SubjectID
AND SSE.IsActive = True";

                    using (OleDbCommand cmd = new OleDbCommand(sectionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", slipId);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.Read())
                            {
                                scheduleId = reader["ScheduleID"] != DBNull.Value ? Convert.ToInt32(reader["ScheduleID"]) : 0;
                                section = reader["Section"]?.ToString() ?? "";
                            }
                        }
                    }

                    string membersQuery = @"
SELECT MemberName FROM BorrowSlipMembers WHERE SlipID = ? ORDER BY MemberName";

                    using (OleDbCommand cmd = new OleDbCommand(membersQuery, conn))
                    {
                        cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                        using OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader != null && reader.Read())
                            members.Add(reader["MemberName"]?.ToString() ?? "");
                    }

                    if (!members.Contains(leaderName))
                        members.Insert(0, leaderName);

                    string itemsQuery = @"
SELECT
    BSI.SlipItemID,
    BSI.SlipID,
    BSI.EquipmentID,
    E.EquipmentName,
    E.HasSerial,
    BSI.QuantityRequested
FROM (BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID)
WHERE BSI.SlipID = ?
AND BSI.ItemReturnStatus = 'Borrowed'
ORDER BY E.EquipmentName";

                    using (OleDbCommand cmd = new OleDbCommand(itemsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", slipId);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                int quantityBorrowed = reader["QuantityRequested"] != DBNull.Value
                                    ? Convert.ToInt32(reader["QuantityRequested"]) : 1;

                                DamageReportItemChoice choice = new DamageReportItemChoice
                                {
                                    SlipItemID = Convert.ToInt32(reader["SlipItemID"]),
                                    SlipID = Convert.ToInt32(reader["SlipID"]),
                                    EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                                    EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                                    QuantityBorrowed = quantityBorrowed,
                                    ReportQuantity = 1,
                                    HasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"]),
                                };

                                if (choice.SlipItemID == selectedSlipItemId && forcedReportQuantity > 0)
                                {
                                    choice.ReportQuantity = Math.Min(forcedReportQuantity, quantityBorrowed);
                                    choice.IsQuantityLocked = true;
                                }

                                itemChoices.Add(choice);
                            }
                        }
                    }
                }

                if (forcedReportQuantity > 0)
                    itemChoices = itemChoices.Where(x => x.SlipItemID == selectedSlipItemId).ToList();

                if (itemChoices.Count == 0)
                {
                    MessageBox.Show("No borrowed equipment found for this slip.",
                        "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ── FORM SETUP ──────────────────────────────────────────────
                Form reportForm = new Form();
                reportForm.Text = "Damage / Lost Report";
                reportForm.StartPosition = FormStartPosition.CenterParent;
                reportForm.Size = new Size(660, 560);
                reportForm.MinimumSize = new Size(660, 500);
                reportForm.FormBorderStyle = FormBorderStyle.Sizable;
                reportForm.MaximizeBox = false;
                reportForm.MinimizeBox = false;
                reportForm.BackColor = Color.FromArgb(250, 245, 247);
                reportForm.AutoScroll = true;

                int px = 30;

                // Title
                Label lblTitle = new Label
                {
                    Text = "Damage / Lost Equipment Report",
                    Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(px, 20),
                    Size = new Size(580, 32)
                };

                // Slip info block
                Label lblInfo = new Label
                {
                    Text =
                        "Slip ID: " + slipId +
                        "\nSubject: " + subjectCode +
                        "\nSection: " + section +
                        "\nGroup Number: " + groupNumber +
                        "\nLeader: " + leaderName +
                        "\nBorrowed Date: " + borrowedDate +
                        "\nDate Reported: " + DateTime.Now.ToString("MMMM dd, yyyy"),
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(px, 60),
                    Size = new Size(580, 138)
                };

                Label lblMembers = new Label
                {
                    Text = "Members: " + string.Join(", ", members),
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(px, 205),
                    Size = new Size(580, 40)
                };

                // Section: choose equipment
                Label lblChoose = new Label
                {
                    Text = "Select broken/lost equipment",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(px, 252),
                    AutoSize = true
                };

                // ── TIGHTENED: height based on item count (44px per non-serial row, 90px per serial row) ──
                FlowLayoutPanel flowItems = new FlowLayoutPanel
                {
                    Location = new Point(px, 275),
                    Size = new Size(580, 130),
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    BackColor = Color.FromArgb(255, 251, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                foreach (DamageReportItemChoice choice in itemChoices)
                {
                    Panel row = CreateDamageReportItemRow(choice, itemChoices);
                    flowItems.Controls.Add(row);
                }

                // Positions of everything below are now relative to bottom of flowItems
                int belowFlow = flowItems.Bottom + 16;

                // Left column X and right column X
                int leftX = px;
                int rightX = px + 310;

                // ── Damage Type (left) ──────────────────────────────────────────────
                Label lblDamageType = new Label
                {
                    Text = "Damage Type",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(leftX, belowFlow),
                    AutoSize = true
                };

                ComboBox cmbDamageType = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(leftX, belowFlow + 24),
                    Size = new Size(260, 28)
                };
                cmbDamageType.Items.Add("Broken");
                cmbDamageType.Items.Add("Lost");

                // ── Evidence (right, always same row as Damage Type) ────────────────
                Label lblEvidenceTitle = new Label
                {
                    Text = "Evidence",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(rightX, belowFlow),
                    AutoSize = true,
                    Visible = false
                };

                string selectedEvidencePath = "";

                Panel pnlEvidenceBox = new Panel
                {
                    Size = new Size(240, 130),
                    Location = new Point(rightX, belowFlow + 24),
                    BackColor = Color.FromArgb(241, 233, 245),
                    Cursor = Cursors.Hand,
                    Visible = false
                };
                RoundControl(pnlEvidenceBox, 12);

                Label lblPlusEvidence = new Label
                {
                    Text = "+",
                    Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Size = new Size(240, 80),
                    Location = new Point(0, 18),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };

                Label lblAddEvidence = new Label
                {
                    Text = "Choose Image",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Size = new Size(240, 24),
                    Location = new Point(0, 100),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };

                PictureBox picEvidence = new PictureBox
                {
                    Size = new Size(240, 130),
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Visible = false,
                    Cursor = Cursors.Hand
                };

                pnlEvidenceBox.Controls.Add(picEvidence);
                pnlEvidenceBox.Controls.Add(lblPlusEvidence);
                pnlEvidenceBox.Controls.Add(lblAddEvidence);

                EventHandler evidenceClickHandler = (s, e) =>
                {
                    using OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Choose evidence image";
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        selectedEvidencePath = ofd.FileName;
                        try
                        {
                            picEvidence.Image = Image.FromFile(selectedEvidencePath);
                            picEvidence.Visible = true;
                            lblPlusEvidence.Visible = false;
                            lblAddEvidence.Visible = false;
                        }
                        catch
                        {
                            MessageBox.Show("Could not load image.", "Evidence",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                };
                pnlEvidenceBox.Click += evidenceClickHandler;
                lblPlusEvidence.Click += evidenceClickHandler;
                lblAddEvidence.Click += evidenceClickHandler;
                picEvidence.Click += evidenceClickHandler;

                // ── Description (left, below damage type) ───────────────────────────
                int belowDamageLabel = belowFlow + 24 + 28 + 14;

                Label lblDescription = new Label
                {
                    Text = "Details / Description",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(leftX, belowDamageLabel),
                    AutoSize = true
                };

                TextBox txtDescription = new TextBox
                {
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(leftX, belowDamageLabel + 24),
                    Size = new Size(260, 106),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    PlaceholderText = "Explain what happened..."
                };

                // ── Report button (right column, below evidence box) ─────────────────
                int belowEvidence = belowFlow + 24 + 130 + 10;

                Button btnSend = new Button
                {
                    Text = "Send Report",
                    Size = new Size(240, 42),
                    Location = new Point(rightX, belowEvidence),
                    BackColor = Color.FromArgb(220, 95, 107),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold)
                };
                btnSend.FlatAppearance.BorderSize = 0;
                RoundControl(btnSend, 14);

                // Form height based on tallest column
                int leftBottom = belowDamageLabel + 24 + 106 + 16;
                int rightBottom = belowEvidence + 42 + 16;
                int belowDescription = Math.Max(leftBottom, rightBottom);

                cmbDamageType.SelectedIndexChanged += (s, e) =>
                {
                    bool isBroken = cmbDamageType.SelectedItem?.ToString() == "Broken";
                    lblEvidenceTitle.Visible = isBroken;
                    pnlEvidenceBox.Visible = isBroken;

                    if (!isBroken)
                    {
                        selectedEvidencePath = "";
                        picEvidence.Image = null;
                        picEvidence.Visible = false;
                        lblPlusEvidence.Visible = true;
                        lblAddEvidence.Visible = true;
                    }
                };

                btnSend.Click += (s, e) =>
                {
                    DamageReportItemChoice? selectedChoice = itemChoices.FirstOrDefault(x => x.Radio.Checked);

                    if (selectedChoice == null)
                    {
                        MessageBox.Show("Please select the equipment that was broken or lost.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cmbDamageType.SelectedItem == null)
                    {
                        MessageBox.Show("Please select Broken or Lost.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtDescription.Text))
                    {
                        MessageBox.Show("Please enter the report details.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string damageType = cmbDamageType.SelectedItem.ToString();

                    if (damageType == "Broken" && string.IsNullOrWhiteSpace(selectedEvidencePath))
                    {
                        MessageBox.Show("Please choose a photo evidence for broken equipment.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (selectedChoice.HasSerial && selectedChoice.SelectedUnitIDs.Count == 0)
                    {
                        MessageBox.Show("Please select the serial number/unit that was broken or lost.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (selectedChoice.HasSerial &&
                        forcedReportQuantity > 0 &&
                        selectedChoice.SelectedUnitIDs.Count != forcedReportQuantity)
                    {
                        MessageBox.Show(
                            "Please select exactly " + forcedReportQuantity + " serial number(s) for this report.",
                            "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SaveDamageReport(
    slipId,
    selectedChoice.SlipItemID,
    selectedChoice.EquipmentID,
    subjectId,
    scheduleId,
    damageType,
    selectedChoice.HasSerial
    ? selectedChoice.SelectedUnitIDs.Count
    : selectedChoice.ReportQuantity,
    selectedChoice.QuantityBorrowed,
    txtDescription.Text.Trim(),
    selectedEvidencePath,
    selectedChoice.SelectedUnitIDs);

                    reportForm.Close();
                };

                reportForm.Controls.Add(lblTitle);
                reportForm.Controls.Add(lblInfo);
                reportForm.Controls.Add(lblMembers);
                reportForm.Controls.Add(lblChoose);
                reportForm.Controls.Add(flowItems);
                reportForm.Controls.Add(lblDamageType);
                reportForm.Controls.Add(cmbDamageType);
                reportForm.Controls.Add(lblEvidenceTitle);
                reportForm.Controls.Add(pnlEvidenceBox);
                reportForm.Controls.Add(lblDescription);
                reportForm.Controls.Add(txtDescription);
                reportForm.Controls.Add(btnSend);

                int totalFormHeight = belowDescription + 42 + 24;
                reportForm.ClientSize = new Size(reportForm.ClientSize.Width,
                    Math.Max(totalFormHeight, 500));

                RoundControl(btnSend, 16);

                reportForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening report form:\n" + ex.Message,
                    "Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void CreateAdminEquipmentCategoryDropdown()
        {
            foreach (Control duplicate in panelEquipment.Controls.Find("cmbAdminCategoryFilter", true))
            {
                if (duplicate.Parent != pnlEquipmentMain)
                {
                    duplicate.Parent?.Controls.Remove(duplicate);
                    duplicate.Dispose();
                }
            }

            ComboBox? existing = pnlEquipmentMain.Controls.Find("cmbAdminCategoryFilter", true)
                .OfType<ComboBox>()
                .FirstOrDefault();

            if (existing != null)
            {
                existing.Location = new Point(562, 66);
                existing.Size = new Size(250, 25);
                existing.BringToFront();
                return;
            }

            btnEqAll.Visible = false;
            btnEqTechnical.Visible = false;
            btnEqScience.Visible = false;
            btnEqSports.Visible = false;
            btnEqGeneral.Visible = false;

            ComboBox cmbAdminCategoryFilter = new ComboBox();
            cmbAdminCategoryFilter.Name = "cmbAdminCategoryFilter";
            cmbAdminCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAdminCategoryFilter.Font = new Font("Segoe UI", 10F);
            cmbAdminCategoryFilter.Size = new Size(250, 25);
            cmbAdminCategoryFilter.Location = new Point(562, 66);

            cmbAdminCategoryFilter.Items.Add("All Categories");
            foreach (string category in EquipmentCategoryService.GetCategories())
                cmbAdminCategoryFilter.Items.Add(category);

            cmbAdminCategoryFilter.SelectedIndex = 0;

            cmbAdminCategoryFilter.SelectedIndexChanged += (s, e) =>
            {
                currentEquipmentCategory =
                    cmbAdminCategoryFilter.Text == "All Categories"
                    ? "All"
                    : cmbAdminCategoryFilter.Text;

                LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
            };

            cmbAdminCategoryFilter.MouseUp += (s, e) =>
            {
                if (e.Button != MouseButtons.Right)
                    return;

                using EquipmentCategoryManagerForm form =
                    new EquipmentCategoryManagerForm(EquipmentCategoryService.GetCategories());

                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                cmbAdminCategoryFilter.Items.Clear();
                cmbAdminCategoryFilter.Items.Add("All Categories");
                foreach (string category in EquipmentCategoryService.GetCategories())
                    cmbAdminCategoryFilter.Items.Add(category);

                if (currentEquipmentCategory != "All" &&
                    cmbAdminCategoryFilter.Items.Contains(currentEquipmentCategory))
                {
                    cmbAdminCategoryFilter.SelectedItem = currentEquipmentCategory;
                }
                else
                {
                    currentEquipmentCategory = "All";
                    cmbAdminCategoryFilter.SelectedIndex = 0;
                }

                LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
            };

            pnlEquipmentMain.Controls.Add(cmbAdminCategoryFilter);
            cmbAdminCategoryFilter.BringToFront();
        }


        private Panel CreateDamageReportItemRow(DamageReportItemChoice choice, List<DamageReportItemChoice> allChoices)
        {
            Panel row = new Panel
            {
                Size = choice.HasSerial ? new Size(560, 110) : new Size(560, 44),
                BackColor = Color.FromArgb(245, 240, 247),
                Margin = new Padding(4, 4, 4, 0)
            };

            RadioButton rb = new RadioButton
            {
                Location = new Point(10, 12),
                Size = new Size(22, 22),
                Checked = choice.IsQuantityLocked
            };
            choice.Radio = rb;

            Label lblName = new Label
            {
                Text = choice.EquipmentName + "  |  Borrowed: " + choice.QuantityBorrowed,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(38, 12),
                Size = new Size(280, 22)
            };

            rb.CheckedChanged += (s, e) =>
            {
                if (rb.Checked)
                {
                    foreach (DamageReportItemChoice item in allChoices)
                        if (item != choice) item.Radio.Checked = false;
                }
            };

            row.Controls.Add(rb);
            row.Controls.Add(lblName);

            if (!choice.HasSerial)
            {
                // Qty label (centered between buttons)
                Label lblQtyVal = new Label
                {
                    Text = choice.ReportQuantity.ToString(),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(382, 10),
                    Size = new Size(36, 24),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                choice.QtyLabel = lblQtyVal;

                Button btnMinus = new Button
                {
                    Text = "−",
                    Size = new Size(32, 26),
                    Location = new Point(344, 10),
                    BackColor = Color.FromArgb(214, 197, 224),
                    ForeColor = Color.FromArgb(87, 60, 99),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Enabled = !choice.IsQuantityLocked
                };
                btnMinus.FlatAppearance.BorderSize = 0;

                Button btnPlus = new Button
                {
                    Text = "+",
                    Size = new Size(32, 26),
                    Location = new Point(424, 10),
                    BackColor = Color.FromArgb(214, 197, 224),
                    ForeColor = Color.FromArgb(87, 60, 99),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Enabled = !choice.IsQuantityLocked
                };
                btnPlus.FlatAppearance.BorderSize = 0;

                Label lblQtyHint = new Label
                {
                    Text = "qty to report",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Location = new Point(344, 38),
                    Size = new Size(115, 16),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                btnMinus.Click += (s, e) =>
                {
                    if (choice.ReportQuantity > 1)
                        choice.ReportQuantity--;
                    choice.QtyLabel.Text = choice.ReportQuantity.ToString();
                };

                btnPlus.Click += (s, e) =>
                {
                    if (choice.ReportQuantity < choice.QuantityBorrowed)
                        choice.ReportQuantity++;
                    choice.QtyLabel.Text = choice.ReportQuantity.ToString();
                };

                RoundControl(btnMinus, 8);
                RoundControl(btnPlus, 8);

                row.Controls.Add(btnMinus);
                row.Controls.Add(lblQtyVal);
                row.Controls.Add(btnPlus);
                row.Controls.Add(lblQtyHint);
            }
            else
            {
                // Serial checkboxes
                Label lblSerialGuide = new Label
                {
                    Text = "Select affected serial/unit:",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(126, 105, 136),
                    Location = new Point(38, 36),
                    Size = new Size(200, 18)
                };

                FlowLayoutPanel flowSerials = new FlowLayoutPanel
                {
                    Location = new Point(38, 56),
                    Size = new Size(500, 40),
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    AutoScroll = true,
                    BackColor = Color.Transparent
                };

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string serialQuery = @"
SELECT EU.UnitID, EU.SerialNumber
FROM BorrowSlipUnits AS BSU
INNER JOIN EquipmentUnits AS EU ON BSU.UnitID = EU.UnitID
WHERE BSU.SlipItemID = ?
ORDER BY EU.SerialNumber";

                using OleDbCommand cmd = new OleDbCommand(serialQuery, conn);
                cmd.Parameters.AddWithValue("@p1", choice.SlipItemID);
                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    int unitId = Convert.ToInt32(reader["UnitID"]);
                    string serial = reader["SerialNumber"]?.ToString() ?? "";

                    CheckBox chk = new CheckBox
                    {
                        Text = serial,
                        AutoSize = true,
                        Font = new Font("Segoe UI", 8.8F),
                        Margin = new Padding(4)
                    };

                    chk.CheckedChanged += (s, e) =>
                    {
                        if (chk.Checked)
                        {
                            if (!choice.SelectedUnitIDs.Contains(unitId))
                                choice.SelectedUnitIDs.Add(unitId);
                        }
                        else
                        {
                            choice.SelectedUnitIDs.Remove(unitId);
                        }
                        choice.ReportQuantity = choice.SelectedUnitIDs.Count;
                    };

                    flowSerials.Controls.Add(chk);
                }

                row.Controls.Add(lblSerialGuide);
                row.Controls.Add(flowSerials);
            }

            RoundControl(row, 10);
            return row;
        }



        private int GetUserIdByFullName(OleDbConnection conn, OleDbTransaction trans, string fullName)
        {
            string query = @"
SELECT TOP 1 UserID
FROM Users
WHERE FullName = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn, trans);
            cmd.Parameters.AddWithValue("@p1", fullName);

            object result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }



        private void SaveDamageReport(
     int slipId,
     int slipItemId,
     int equipmentId,
     int subjectId,
     int scheduleId,
     string damageType,
     int damageQuantity,
     int quantityBorrowed,
     string description,
     string evidenceImagePath,
     List<int> selectedUnitIds)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbTransaction trans = conn.BeginTransaction();

            int reportId = 0;

            try
            {
                string insertReportQuery = @"
INSERT INTO DamageReports
(SlipID, SlipItemID, EquipmentID, SubjectID, ScheduleID, ReportedByAdminID,
DamageType, DamageQuantity, Description, DateReported, ReportStatus, EvidenceImagePath)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (OleDbCommand cmd = new OleDbCommand(insertReportQuery, conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    cmd.Parameters.Add("@p2", OleDbType.Integer).Value = slipItemId;
                    cmd.Parameters.Add("@p3", OleDbType.Integer).Value = equipmentId;
                    cmd.Parameters.Add("@p4", OleDbType.Integer).Value = subjectId;
                    cmd.Parameters.Add("@p5", OleDbType.Integer).Value = scheduleId > 0 ? (object)scheduleId : DBNull.Value;
                    cmd.Parameters.Add("@p6", OleDbType.Integer).Value = SessionManager.AdminID > 0 ? (object)SessionManager.AdminID : DBNull.Value;
                    cmd.Parameters.Add("@p7", OleDbType.VarWChar).Value = damageType;
                    cmd.Parameters.Add("@p8", OleDbType.Integer).Value = damageQuantity;
                    cmd.Parameters.Add("@p9", OleDbType.LongVarWChar).Value = description;
                    cmd.Parameters.Add("@p10", OleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add("@p11", OleDbType.VarWChar).Value = "Pending Cost";
                    string safePath = (evidenceImagePath ?? "");
                    if (safePath.Length > 255) safePath = safePath.Substring(0, 255);
                    cmd.Parameters.Add("@p12", OleDbType.VarWChar).Value = safePath;

                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            "FAILED AT: INSERT DamageReports\n" +
                            "p1 slipId=" + slipId + "\n" +
                            "p2 slipItemId=" + slipItemId + "\n" +
                            "p3 equipmentId=" + equipmentId + "\n" +
                            "p4 subjectId=" + subjectId + "\n" +
                            "p5 scheduleId=" + scheduleId + "\n" +
                            "p6 AdminID=" + SessionManager.AdminID + "\n" +
                            "p7 damageType=" + damageType + "\n" +
                            "p8 damageQuantity=" + damageQuantity + "\n" +
                            "p9 description=" + description + "\n" +
                            "p10 DateTime=" + DateTime.Now + "\n" +
                            "p11=Pending Cost\n" +
                            "p12 evidenceImagePath=" + (evidenceImagePath ?? "NULL") + "\n\n" +
                            ex.Message
                        );
                    }
                }

                using (OleDbCommand idCmd = new OleDbCommand("SELECT @@IDENTITY", conn, trans))
                {
                    reportId = Convert.ToInt32(idCmd.ExecuteScalar());
                }

                foreach (int unitId in selectedUnitIds)
                {
                    string insertUnitQuery = @"
INSERT INTO DamageReportUnits (ReportID, UnitID) VALUES (?, ?)";

                    using (OleDbCommand unitCmd = new OleDbCommand(insertUnitQuery, conn, trans))
                    {
                        unitCmd.Parameters.Add("@p1", OleDbType.Integer).Value = reportId;
                        unitCmd.Parameters.Add("@p2", OleDbType.Integer).Value = unitId;

                        try { unitCmd.ExecuteNonQuery(); }
                        catch (Exception ex) { throw new Exception("FAILED AT: INSERT DamageReportUnits\nReportID=" + reportId + " UnitID=" + unitId + "\n" + ex.Message); }
                    }

                    string updateUnitQuery = @"
UPDATE EquipmentUnits SET UnitStatus = ? WHERE UnitID = ?";

                    using (OleDbCommand updateUnitCmd = new OleDbCommand(updateUnitQuery, conn, trans))
                    {
                        updateUnitCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = damageType;
                        updateUnitCmd.Parameters.Add("@p2", OleDbType.Integer).Value = unitId;

                        try { updateUnitCmd.ExecuteNonQuery(); }
                        catch (Exception ex) { throw new Exception("FAILED AT: UPDATE EquipmentUnits\nUnitID=" + unitId + "\n" + ex.Message); }
                    }
                }

                // Only mark as Under Report if ALL quantity is reported
                // If partial, keep status as Borrowed and just reduce the quantity
                string updateReportedItemQuery;

                if (damageQuantity >= quantityBorrowed)
                {
                    // All units reported — mark full row as Under Report
                    updateReportedItemQuery = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Under Report', QuantityReturned = 0
WHERE SlipItemID = ?";
                }
                else
                {
                    // Partial report — keep as Borrowed, just note the reported quantity
                    // The remaining quantity stays borrowable
                    updateReportedItemQuery = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Borrowed'
WHERE SlipItemID = ?";
                }

                using (OleDbCommand cmd = new OleDbCommand(updateReportedItemQuery, conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipItemId;

                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception ex) { throw new Exception("FAILED AT: UPDATE BorrowSlipItems\nSlipItemID=" + slipItemId + "\n" + ex.Message); }
                }

                string leaderQuery = @"SELECT UserID FROM BorrowSlips WHERE SlipID = ?";
                List<int> memberUserIds = new List<int>();

                using (OleDbCommand cmd = new OleDbCommand(leaderQuery, conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        memberUserIds.Add(Convert.ToInt32(result));
                }

                string membersQuery = @"SELECT MemberName FROM BorrowSlipMembers WHERE SlipID = ?";

                using (OleDbCommand cmd = new OleDbCommand(membersQuery, conn, trans))
                {
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string memberName = reader["MemberName"]?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(memberName)) continue;
                        int userId = GetUserIdByFullName(conn, trans, memberName);
                        if (userId > 0 && !memberUserIds.Contains(userId))
                            memberUserIds.Add(userId);
                    }
                }

                foreach (int userId in memberUserIds)
                {
                    string insertMemberQuery = @"
INSERT INTO DamageReportMembers
(ReportID, UserID, AmountShare, IsRestricted, HasPaid)
VALUES (?, ?, ?, ?, ?)";

                    using OleDbCommand cmd = new OleDbCommand(insertMemberQuery, conn, trans);
                    cmd.Parameters.Add("@p1", OleDbType.Integer).Value = reportId;
                    cmd.Parameters.Add("@p2", OleDbType.Integer).Value = userId;
                    cmd.Parameters.Add("@p3", OleDbType.Decimal).Value = 0;
                    cmd.Parameters.Add("@p4", OleDbType.Boolean).Value = false;
                    cmd.Parameters.Add("@p5", OleDbType.Boolean).Value = false;

                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception ex) { throw new Exception("FAILED AT: INSERT DamageReportMembers\nUserID=" + userId + "\n" + ex.Message); }
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show(ex.Message, "Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }




        private void MarkDamageReportPendingCost(int reportId)
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
UPDATE DamageReports
SET ReportStatus = 'Pending Cost'
WHERE ReportID = ?
AND ReportStatus = 'Reported'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", reportId);
            cmd.ExecuteNonQuery();
        }


        private void btnUnclaimed_Click(object? sender, EventArgs e)
        {
            if (dgvReservations.CurrentRow == null)
            {
                MessageBox.Show("Select a pending slip first.", "Decline Slip",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int slipId = Convert.ToInt32(dgvReservations.CurrentRow.Cells["SlipID"].Value);

            string reason = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter reason for declining this borrower slip:",
                "Decline Slip",
                "Please approach the NAS/admin for clarification.");

            if (string.IsNullOrWhiteSpace(reason))
                return;

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string declineQuery = @"
UPDATE BorrowSlips
SET SlipStatus = 'Declined',
    DeclineReason = ?
WHERE SlipID = ?";

                using OleDbCommand cmd = new OleDbCommand(declineQuery, conn);
                cmd.Parameters.AddWithValue("@p1", reason);
                cmd.Parameters.AddWithValue("@p2", slipId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Borrower slip declined successfully.", "Decline Slip",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadReservationsData();
                LoadAdminDashboardNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error declining slip:\n" + ex.Message,
                    "Decline Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InitializeReservationsSearch()
        {
            lblReservationsHeader.Visible = false;
            cardResUnclaimed.Visible = false;
            lblResShowAll.Text = "Show All";

            foreach (Control ctrl in pnlReservationsMain.Controls.OfType<TextBox>().ToList())
            {
                if (ctrl.Name == "txtSearchReservations")
                    pnlReservationsMain.Controls.Remove(ctrl);
            }

            foreach (Control ctrl in pnlReservationStats.Controls.OfType<ComboBox>().ToList())
            {
                if (ctrl.Name == "cmbSlipSubjectFilter" || ctrl.Name == "cmbSlipSectionFilter")
                    pnlReservationStats.Controls.Remove(ctrl);
            }

            foreach (Control ctrl in pnlReservationStats.Controls.OfType<Label>().ToList())
            {
                if (ctrl.Name == "lblSlipSubjectFilter" || ctrl.Name == "lblSlipSectionFilter")
                    pnlReservationStats.Controls.Remove(ctrl);
            }

            TextBox txtSearchReservations = new TextBox
            {
                Name = "txtSearchReservations",
                PlaceholderText = "Search by student name or School ID...",
                Size = new Size(270, 30),
                Location = new Point(28, 25),
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtSearchReservations.TextChanged += (s, e) =>
            {
                currentReservationSearch = txtSearchReservations.Text.Trim();
                LoadReservationsData();
            };

            pnlReservationsMain.Controls.Add(txtSearchReservations);
            txtSearchReservations.BringToFront();

            CreateReservationDropdownFilters();
        }

        private void CreateReservationDropdownFilters()
        {
            loadingReservationFilters = true;

            Label lblSubject = new Label
            {
                Name = "lblSlipSubjectFilter",
                Text = "Subject",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(680, 4),
                Size = new Size(120, 18)
            };

            ComboBox cmbSubject = new ComboBox
            {
                Name = "cmbSlipSubjectFilter",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(680, 26),
                Size = new Size(140, 28)
            };

            Label lblSection = new Label
            {
                Name = "lblSlipSectionFilter",
                Text = "Section",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(836, 4),
                Size = new Size(120, 18)
            };

            ComboBox cmbSection = new ComboBox
            {
                Name = "cmbSlipSectionFilter",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(836, 26),
                Size = new Size(140, 28)
            };

            cmbSubject.Items.Add("All Subjects");
            cmbSection.Items.Add("All Sections");

            using (OleDbConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                using (OleDbCommand cmd = new OleDbCommand(
                    "SELECT SubjectCode FROM LabSubjects WHERE LabID = ? AND IsActive = True ORDER BY SubjectCode", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string subject = reader["SubjectCode"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(subject) && !cmbSubject.Items.Contains(subject))
                            cmbSubject.Items.Add(subject);
                    }
                }

                string sectionQuery = @"
SELECT DISTINCT SS.Section
FROM SubjectSchedules AS SS
INNER JOIN LabSubjects AS LS ON SS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND LS.IsActive = True
ORDER BY SS.Section";

                using (OleDbCommand cmd = new OleDbCommand(sectionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string section = reader["Section"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(section) && !cmbSection.Items.Contains(section))
                            cmbSection.Items.Add(section);
                    }
                }
            }

            cmbSubject.SelectedItem = cmbSubject.Items.Contains(currentReservationSubjectFilter)
                ? currentReservationSubjectFilter
                : "All Subjects";
            cmbSection.SelectedItem = cmbSection.Items.Contains(currentReservationSectionFilter)
                ? currentReservationSectionFilter
                : "All Sections";

            cmbSubject.SelectedIndexChanged += (s, e) =>
            {
                if (loadingReservationFilters) return;
                currentReservationSubjectFilter = cmbSubject.SelectedItem?.ToString() ?? "All Subjects";
                LoadReservationsData();
            };

            cmbSection.SelectedIndexChanged += (s, e) =>
            {
                if (loadingReservationFilters) return;
                currentReservationSectionFilter = cmbSection.SelectedItem?.ToString() ?? "All Sections";
                LoadReservationsData();
            };

            pnlReservationStats.Controls.Add(lblSubject);
            pnlReservationStats.Controls.Add(cmbSubject);
            pnlReservationStats.Controls.Add(lblSection);
            pnlReservationStats.Controls.Add(cmbSection);

            lblSubject.BringToFront();
            cmbSubject.BringToFront();
            lblSection.BringToFront();
            cmbSection.BringToFront();

            loadingReservationFilters = false;
        }

        private void FilterReservations(string keyword)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string likeValue = "%" + keyword + "%";

                string query = @"
SELECT
    BS.SlipID,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    LS.SubjectName,
    BS.GroupNumber,
    BS.LeaderName,
    BS.SlipType,
    BS.DateCreated,
    BS.SlipStatus
FROM (BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND
(
    U.FullName LIKE ?
    OR U.SchoolID LIKE ?
    OR LS.SubjectCode LIKE ?
)";

                if (currentReservationFilter != "All")
                    query += " AND BS.SlipStatus = ?";

                query += " ORDER BY BS.DateCreated DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                cmd.Parameters.AddWithValue("@p2", likeValue);
                cmd.Parameters.AddWithValue("@p3", likeValue);
                cmd.Parameters.AddWithValue("@p4", likeValue);

                if (currentReservationFilter != "All")
                    cmd.Parameters.AddWithValue("@p5", currentReservationFilter);

                DataTable dt = new DataTable();
                using OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                dgvReservations.DataSource = dt;
                dgvReservations.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering pending slips:\n" + ex.Message,
                    "Pending Slips", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHistoryData()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    BS.LeaderName,
    BS.GroupNumber,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    SS.Section,
    BS.DateCreated,
    SS.EndTime,
    COUNT(BSI.SlipItemID) AS ItemCount,
    SUM(IIF(BSI.ItemReturnStatus = 'Borrowed', 1, 0)) AS BorrowedItemCount,
    (SELECT COUNT(*) FROM DamageReports AS DR WHERE DR.SlipID = BS.SlipID) AS ReportCount
FROM ((((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN BorrowSlipItems AS BSI ON BS.SlipID = BSI.SlipID)
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
GROUP BY
    BS.SlipID,
    BS.LeaderName,
    BS.GroupNumber,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    SS.Section,
    BS.DateCreated,
    SS.EndTime
ORDER BY BS.DateCreated DESC";

                DataTable raw = new DataTable();
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                    using OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(raw);
                }

                DataTable dt = CreateBorrowedSlipTable();
                foreach (DataRow row in raw.Rows)
                {
                    int borrowedItemCount = row["BorrowedItemCount"] != DBNull.Value
                        ? Convert.ToInt32(row["BorrowedItemCount"])
                        : 0;
                    int reportCount = row["ReportCount"] != DBNull.Value
                        ? Convert.ToInt32(row["ReportCount"])
                        : 0;

                    if (borrowedItemCount > 0 && reportCount == 0)
                        continue;

                    DateTime borrowDate = row["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(row["DateCreated"])
                        : DateTime.Now;
                    DateTime dueDate = BuildDueDate(borrowDate, row["EndTime"]);

                    dt.Rows.Add(
                        row["SlipID"],
                        row["LeaderName"]?.ToString() ?? row["FullName"]?.ToString() ?? "",
                        row["SchoolID"]?.ToString() ?? "",
                        row["GroupNumber"]?.ToString() ?? "",
                        row["Section"]?.ToString() ?? "",
                        row["SubjectCode"]?.ToString() ?? "",
                        borrowDate,
                        dueDate,
                        reportCount > 0 ? "Returned / Reported" : "Returned");
                }

                dgvHistory.DataSource = dt;
                dgvHistory.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading history:\n" + ex.Message,
                    "History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnEqAll_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "All";
            SetEquipmentFilterButtons(btnEqAll);
            UpdateEquipmentSectionTitle();
            LoadEquipmentCards(currentEquipmentCategory);
        }

        private void btnEqTechnical_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Chemical Engineering";
            SetEquipmentFilterButtons(btnEqTechnical);
            UpdateEquipmentSectionTitle();
            LoadEquipmentCards(currentEquipmentCategory);
        }

        private void btnEqScience_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Mechanical Engineering";
            SetEquipmentFilterButtons(btnEqScience);
            UpdateEquipmentSectionTitle();
            LoadEquipmentCards(currentEquipmentCategory);
        }

        private void btnEqSports_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "Civil Engineering";
            SetEquipmentFilterButtons(btnEqSports);
            UpdateEquipmentSectionTitle();
            LoadEquipmentCards(currentEquipmentCategory);
        }

        private void btnEqGeneral_Click(object? sender, EventArgs e)
        {
            currentEquipmentCategory = "General Laboratory";
            SetEquipmentFilterButtons(btnEqGeneral);
            UpdateEquipmentSectionTitle();
            LoadEquipmentCards(currentEquipmentCategory);
        }


        private void AddEquipmentSectionHeader(string text, bool isExpanded, EventHandler clickHandler)
        {
            Label header = new Label
            {
                Text = text + (isExpanded ? " [v]" : " [^]"),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                AutoSize = false,
                Width = 960,
                Height = 34,
                Margin = new Padding(12, 12, 12, 4),
                Cursor = Cursors.Hand
            };

            header.Click += clickHandler;
            flowEquipmentCards.Controls.Add(header);
            flowEquipmentCards.SetFlowBreak(header, true);
        }

        private void ToggleAvailableEquipmentSection(object? sender, EventArgs e)
        {
            equipmentAvailableExpanded = !equipmentAvailableExpanded;
            LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
        }

        private void ToggleArchivedEquipmentSection(object? sender, EventArgs e)
        {
            equipmentArchivedExpanded = !equipmentArchivedExpanded;
            LoadEquipmentCards(currentEquipmentCategory, txtEquipmentAdminSearch.Text.Trim());
        }

        private Label CreateEquipmentEmptyLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(126, 105, 136),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(900, 42),
                Margin = new Padding(16, 0, 12, 12)
            };
        }

        private Panel CreateEquipmentCard(int equipmentId, string equipmentName, string category, int total, int available, string status, string imagePath, bool isArchived = false)
        {
            Panel card = new Panel
            {
                BackColor = isArchived ? Color.FromArgb(238, 234, 240) : Color.White,
                Width = 238,
                Height = 272,
                Margin = new Padding(18, 14, 18, 18),
                Cursor = Cursors.Hand,
                Tag = equipmentId
            };

            RoundControl(card, 18);
            ApplyNeumorphismPanel(card, 18);

            PictureBox pic = new PictureBox
            {
                Location = new Point(20, 18),
                Size = new Size(198, 130),
                BackColor = Color.FromArgb(240, 235, 245),
                BorderStyle = BorderStyle.FixedSingle,
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
                Location = new Point(20, 156),
                AutoSize = false,
                Width = 198,
                Height = 24
            };

            Label lblCategory = new Label
            {
                Text = category,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(126, 105, 136),
                Location = new Point(20, 182),
                AutoSize = false,
                Width = 198,
                Height = 20
            };

            Label lblAvailable = new Label
            {
                Text = $"Available: {available}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(20, 205),
                AutoSize = false,
                Width = 198,
                Height = 20
            };

            Label lblStatus = new Label
            {
                Text = isArchived ? "Archived" : $"Status: {status}",
                Font = new Font("Segoe UI", 9.2F),
                ForeColor = isArchived
                    ? Color.FromArgb(120, 90, 130)
                    : status.Equals("Available", StringComparison.OrdinalIgnoreCase)
                    || status.Equals("Active", StringComparison.OrdinalIgnoreCase)
                    ? Color.FromArgb(45, 132, 74)
                    : Color.Firebrick,
                Location = new Point(20, 226),
                AutoSize = false,
                Width = 198,
                Height = 20
            };


            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblCategory);
            card.Controls.Add(lblAvailable);
            card.Controls.Add(lblStatus);


            card.Click += (s, e) =>
            {
                using frmEquipmentDetails details = new frmEquipmentDetails(equipmentId);
                if (details.ShowDialog() == DialogResult.OK)
                {
                    LoadEquipmentCards(currentEquipmentCategory);
                    LoadAdminDashboardNew();
                }
            };

            foreach (Control ctrl in card.Controls)
            {
                ctrl.Click += (s, e) =>
                {
                    using frmEquipmentDetails details = new frmEquipmentDetails(equipmentId);
                    if (details.ShowDialog() == DialogResult.OK)
                    {
                        LoadEquipmentCards(currentEquipmentCategory);
                        LoadAdminDashboardNew();
                    }
                };
            }
            return card;
        }








        private void btnAddEquipment_Click(object sender, EventArgs e)
        {
            using frmAddEquipment addForm = new frmAddEquipment();

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadEquipmentCards(currentEquipmentCategory);
                LoadAdminDashboardNew();
            }
        }





        

    private void InitializeBorrowedSearch()
        {
            TextBox txtSearchBorrowed = new TextBox
            {
                Name = "txtSearchBorrowed",
                PlaceholderText = "Search by name or School ID...",
                Size = new Size(260, 30),
                Location = new Point(720, 20),
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtSearchBorrowed.TextChanged += (s, e) =>
            {
                FilterBorrowed(txtSearchBorrowed.Text);
            };

            pnlBorrowedMain.Controls.Add(txtSearchBorrowed);
            txtSearchBorrowed.BringToFront();
        }


        private void dgvReservations_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // RESET ALL CELLS TO CLEAN WHITE (removes colored rows)
            e.CellStyle.BackColor = Color.White;
            e.CellStyle.ForeColor = Color.FromArgb(70, 50, 80);

            // ONLY COLOR THE STATUS TEXT
            if (dgvReservations.Columns[e.ColumnIndex].Name == "ReservationStatus")
            {
                if (e.Value == null) return;

                string status = e.Value.ToString() ?? "";

                e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

                switch (status)
                {
                    case "Pending":
                        e.CellStyle.ForeColor = Color.FromArgb(180, 120, 20); // yellow
                        break;

                    case "Claimed":
                        e.CellStyle.ForeColor = Color.FromArgb(40, 140, 80); // green
                        break;

                    case "Unclaimed":
                        e.CellStyle.ForeColor = Color.FromArgb(170, 50, 50); // red
                        break;

                    case "Cancelled":
                        e.CellStyle.ForeColor = Color.Black;
                        break;
                }
            }
        }


        private void FilterBorrowed(string keyword)
        {
            try
            {
                using (OleDbConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    b.BorrowID,
                    u.FullName,
                    u.SchoolID,
                    e.EquipmentName,
                    b.QuantityBorrowed,
                    b.BorrowDate,
                    b.DueDate,
                    b.PenaltyAmount,
                    b.BorrowStatus
                FROM BorrowTransactions b
                INNER JOIN Users u ON b.UserID = u.UserID
                INNER JOIN Equipment e ON b.EquipmentID = e.EquipmentID
                WHERE u.FullName LIKE ? OR u.SchoolID LIKE ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", "%" + keyword + "%");
                        cmd.Parameters.AddWithValue("@p2", "%" + keyword + "%");

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvBorrowed.DataSource = dt;
                        }
                    }
                }

                dgvBorrowed.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
        }


        private void SaveNewEquipment(string name, string category, int totalQty, string status, string imagePath)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int maintenanceQty = status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase)
                    ? totalQty
                    : 0;

                string query = @"
INSERT INTO Equipment
(EquipmentName, Category, QuantityTotal, QuantityAvailable, QuantityMaintenance, Status, ImagePath, IsArchived)
VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", name);
                cmd.Parameters.AddWithValue("@p2", category);
                cmd.Parameters.AddWithValue("@p3", totalQty);
                cmd.Parameters.AddWithValue("@p4", 0); // kept only for compatibility, no longer used as truth
                cmd.Parameters.AddWithValue("@p5", maintenanceQty);
                cmd.Parameters.AddWithValue("@p6", status);
                cmd.Parameters.AddWithValue("@p7", imagePath);
                cmd.Parameters.AddWithValue("@p8", false);

                cmd.ExecuteNonQuery();

                LoadAllAdminData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding equipment:\n" + ex.Message, "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateEquipment(
    int equipmentId,
    string name,
    string category,
    int totalQty,
    int currentAvailableQty,
    string oldStatus,
    string newStatus,
    string imagePath)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                int maintenanceQty = newStatus.Equals("Maintenance", StringComparison.OrdinalIgnoreCase)
                    ? totalQty
                    : 0;

                string query = @"
UPDATE Equipment
SET EquipmentName = ?,
    Category = ?,
    QuantityTotal = ?,
    QuantityMaintenance = ?,
    Status = ?,
    ImagePath = ?
WHERE EquipmentID = ?";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", name);
                cmd.Parameters.AddWithValue("@p2", category);
                cmd.Parameters.AddWithValue("@p3", totalQty);
                cmd.Parameters.AddWithValue("@p4", maintenanceQty);
                cmd.Parameters.AddWithValue("@p5", newStatus);
                cmd.Parameters.AddWithValue("@p6", imagePath);
                cmd.Parameters.AddWithValue("@p7", equipmentId);

                cmd.ExecuteNonQuery();

                LoadAllAdminData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating equipment:\n" + ex.Message, "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private bool ArchiveEquipment(int equipmentId)
        {
            List<DeletedReservationNotice> affectedReservations = new List<DeletedReservationNotice>();

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                OleDbTransaction tx = conn.BeginTransaction();

                try
                {
                    string equipmentName = "";

                    // Get equipment name
                    string getEquipmentQuery = @"
SELECT EquipmentName
FROM Equipment
WHERE EquipmentID = ?";

                    using (OleDbCommand getEquipmentCmd = new OleDbCommand(getEquipmentQuery, conn, tx))
                    {
                        getEquipmentCmd.Parameters.AddWithValue("@p1", equipmentId);
                        object? result = getEquipmentCmd.ExecuteScalar();
                        equipmentName = result?.ToString() ?? "";
                    }

                    // Get ALL affected pending reservations (today AND future)
                    string getAffectedReservationsQuery = @"
SELECT
    U.FullName,
    U.SchoolEmail,
    E.EquipmentName,
    R.QuantityReserved,
    R.ReservationDate
FROM (Reservations AS R
INNER JOIN Users AS U ON R.UserID = U.UserID)
INNER JOIN Equipment AS E ON R.EquipmentID = E.EquipmentID
WHERE R.EquipmentID = ?
  AND R.ReservationStatus = 'Pending'";

                    using (OleDbCommand affectedCmd = new OleDbCommand(getAffectedReservationsQuery, conn, tx))
                    {
                        affectedCmd.Parameters.AddWithValue("@p1", equipmentId);

                        using (OleDbDataReader reader = affectedCmd.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                affectedReservations.Add(new DeletedReservationNotice
                                {
                                    FullName = reader["FullName"]?.ToString() ?? "",
                                    SchoolEmail = reader["SchoolEmail"]?.ToString() ?? "",
                                    EquipmentName = reader["EquipmentName"]?.ToString() ?? equipmentName,
                                    QuantityReserved = reader["QuantityReserved"] != DBNull.Value
                                        ? Convert.ToInt32(reader["QuantityReserved"])
                                        : 0,
                                    ReservationDate = reader["ReservationDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["ReservationDate"])
                                        : DateTime.Today
                                });
                            }
                        }
                    }

                    // Get total quantity reserved across all pending reservations
                    int totalReservedQty = 0;
                    string sumReservedQuery = @"
SELECT SUM(QuantityReserved)
FROM Reservations
WHERE EquipmentID = ?
  AND ReservationStatus = 'Pending'";

                    using (OleDbCommand sumCmd = new OleDbCommand(sumReservedQuery, conn, tx))
                    {
                        sumCmd.Parameters.AddWithValue("@p1", equipmentId);
                        object? sumResult = sumCmd.ExecuteScalar();
                        if (sumResult != null && sumResult != DBNull.Value)
                            totalReservedQty = Convert.ToInt32(sumResult);
                    }

                    // Restore QuantityAvailable using the fetched sum
                    if (totalReservedQty > 0)
                    {
                        string restoreQuantityQuery = @"
UPDATE Equipment
SET QuantityAvailable = QuantityAvailable + ?
WHERE EquipmentID = ?";

                        using (OleDbCommand restoreCmd = new OleDbCommand(restoreQuantityQuery, conn, tx))
                        {
                            restoreCmd.Parameters.AddWithValue("@p1", totalReservedQty);
                            restoreCmd.Parameters.AddWithValue("@p2", equipmentId);
                            restoreCmd.ExecuteNonQuery();
                        }
                    }

                    // Delete ALL pending reservations for this equipment
                    string deleteReservationsQuery = @"
DELETE FROM Reservations
WHERE EquipmentID = ?
  AND ReservationStatus = 'Pending'";

                    using (OleDbCommand deleteReservationsCmd = new OleDbCommand(deleteReservationsQuery, conn, tx))
                    {
                        deleteReservationsCmd.Parameters.AddWithValue("@p1", equipmentId);
                        deleteReservationsCmd.ExecuteNonQuery();
                    }

                    // Archive equipment
                    string archiveEquipmentQuery = @"
UPDATE Equipment
SET IsArchived = True
WHERE EquipmentID = ?";

                    using (OleDbCommand archiveCmd = new OleDbCommand(archiveEquipmentQuery, conn, tx))
                    {
                        archiveCmd.Parameters.AddWithValue("@p1", equipmentId);
                        int rows = archiveCmd.ExecuteNonQuery();

                        if (rows <= 0)
                        {
                            tx.Rollback();
                            MessageBox.Show("Equipment could not be archived.", "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting equipment:\n" + ex.Message, "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Send emails after successful commit
            foreach (DeletedReservationNotice notice in affectedReservations)
            {
                try
                {
                    EmailService.SendEquipmentDeletedReservationNotice(
                        notice.SchoolEmail,
                        notice.FullName,
                        notice.EquipmentName,
                        notice.QuantityReserved,
                        notice.ReservationDate);
                }
                catch
                {
                    // Do not stop delete if one email fails
                }
            }

            return true;
        }


        private bool SetEquipmentMaintenance(int equipmentId, string equipmentName)
        {
            List<MaintenanceReservationNotice> affectedReservations = new List<MaintenanceReservationNotice>();

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                using OleDbTransaction tx = conn.BeginTransaction();

                try
                {
                    string getAffectedQuery = @"
SELECT U.FullName, U.SchoolEmail, E.EquipmentName, R.QuantityReserved, R.ReservationDate
FROM (Reservations AS R
INNER JOIN Users AS U ON R.UserID = U.UserID)
INNER JOIN Equipment AS E ON R.EquipmentID = E.EquipmentID
WHERE R.EquipmentID = ?
  AND R.ReservationStatus = 'Pending'";

                    using (OleDbCommand affectedCmd = new OleDbCommand(getAffectedQuery, conn, tx))
                    {
                        affectedCmd.Parameters.AddWithValue("@p1", equipmentId);

                        using (OleDbDataReader reader = affectedCmd.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                affectedReservations.Add(new MaintenanceReservationNotice
                                {
                                    FullName = reader["FullName"]?.ToString() ?? "",
                                    SchoolEmail = reader["SchoolEmail"]?.ToString() ?? "",
                                    EquipmentName = reader["EquipmentName"]?.ToString() ?? equipmentName,
                                    QuantityReserved = reader["QuantityReserved"] != DBNull.Value
                                        ? Convert.ToInt32(reader["QuantityReserved"])
                                        : 0,
                                    ReservationDate = reader["ReservationDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["ReservationDate"])
                                        : DateTime.Today
                                });
                            }
                        }
                    }

                    string deleteReservationsQuery = @"
DELETE FROM Reservations
WHERE EquipmentID = ?
  AND ReservationStatus = 'Pending'";

                    using (OleDbCommand deleteCmd = new OleDbCommand(deleteReservationsQuery, conn, tx))
                    {
                        deleteCmd.Parameters.AddWithValue("@p1", equipmentId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    string setMaintenanceQuery = @"
UPDATE Equipment
SET Status = 'Maintenance',
    QuantityMaintenance = QuantityTotal
WHERE EquipmentID = ?";

                    using (OleDbCommand maintenanceCmd = new OleDbCommand(setMaintenanceQuery, conn, tx))
                    {
                        maintenanceCmd.Parameters.AddWithValue("@p1", equipmentId);
                        maintenanceCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting maintenance:\n" + ex.Message, "Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (MaintenanceReservationNotice notice in affectedReservations)
            {
                try
                {
                    EmailService.SendMaintenanceNotice(
                        notice.SchoolEmail,
                        notice.FullName,
                        notice.EquipmentName,
                        notice.QuantityReserved,
                        notice.ReservationDate);
                }
                catch
                {
                }
            }

            return true;
        }

        private void LoadReservationsData()
        {
            try
            {
                dgvReservations.Visible = false;
                flowPendingCards.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    LS.SubjectName,
    SS.Section,
    BS.GroupNumber,
    BS.LeaderName,
    BS.SlipType,
    BS.DateCreated,
    BS.SlipStatus
FROM ((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID
WHERE LS.LabID = ?";

                if (currentReservationFilter != "All")
                    query += " AND BS.SlipStatus = ?";

                if (currentReservationSubjectFilter != "All Subjects")
                    query += " AND LS.SubjectCode = ?";

                if (currentReservationSectionFilter != "All Sections")
                    query += " AND SS.Section = ?";

                if (!string.IsNullOrWhiteSpace(currentReservationSearch))
                    query += " AND (U.FullName LIKE ? OR U.SchoolID LIKE ? OR LS.SubjectCode LIKE ? OR BS.LeaderName LIKE ?)";

                query += " ORDER BY BS.DateCreated DESC";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                if (currentReservationFilter != "All")
                    cmd.Parameters.AddWithValue("@p2", currentReservationFilter);

                if (currentReservationSubjectFilter != "All Subjects")
                    cmd.Parameters.AddWithValue("@p3", currentReservationSubjectFilter);

                if (currentReservationSectionFilter != "All Sections")
                    cmd.Parameters.AddWithValue("@p4", currentReservationSectionFilter);

                if (!string.IsNullOrWhiteSpace(currentReservationSearch))
                {
                    string likeValue = "%" + currentReservationSearch + "%";
                    cmd.Parameters.AddWithValue("@p5", likeValue);
                    cmd.Parameters.AddWithValue("@p6", likeValue);
                    cmd.Parameters.AddWithValue("@p7", likeValue);
                    cmd.Parameters.AddWithValue("@p8", likeValue);
                }

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int slipId = Convert.ToInt32(reader["SlipID"]);
                    string groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    string subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    string slipType = reader["SlipType"]?.ToString() ?? "";
                    string status = reader["SlipStatus"]?.ToString() ?? "";
                    string leaderName = reader["LeaderName"]?.ToString() ?? "";
                    string section = reader["Section"]?.ToString() ?? "";
                    DateTime dateCreated = reader["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateCreated"])
                        : DateTime.Now;
                    string membersText = GetSlipMembersText(conn, slipId);
                    string equipmentText = GetSlipEquipmentText(conn, slipId);

                    Panel card = CreatePendingSlipCard(
                        slipId,
                        groupNumber,
                        subjectCode,
                        slipType,
                        status,
                        leaderName,
                        section,
                        dateCreated,
                        membersText,
                        equipmentText);
                    flowPendingCards.Controls.Add(card);
                }

                lblResPendingCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ? AND BS.SlipStatus = 'Pending'", SessionManager.LabID).ToString();

                lblResClaimedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ? AND BS.SlipStatus = 'Approved'", SessionManager.LabID).ToString();

                lblResUnclaimedCount.Text = GetScalarCount(conn, @"
SELECT COUNT(*)
FROM BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ? AND BS.SlipStatus = 'Declined'", SessionManager.LabID).ToString();

                UpdateReservationCardStyles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cards:\n" + ex.Message);
            }
        }


        private string GetSlipMembersText(OleDbConnection conn, int slipId)
        {
            List<string> members = new List<string>();

            string query = "SELECT MemberName FROM BorrowSlipMembers WHERE SlipID = ? ORDER BY MemberName";
            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", slipId);

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                string member = reader["MemberName"]?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(member))
                    members.Add(member);
            }

            return members.Count == 0
                ? "None"
                : string.Join(Environment.NewLine, members);
        }


        private string GetSlipEquipmentText(OleDbConnection conn, int slipId)
        {
            List<string> equipment = new List<string>();

            string query = @"
SELECT E.EquipmentName, BSI.QuantityRequested
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?
ORDER BY E.EquipmentName";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", slipId);

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                string name = reader["EquipmentName"]?.ToString() ?? "";
                int qty = reader["QuantityRequested"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityRequested"])
                    : 0;

                if (!string.IsNullOrWhiteSpace(name))
                    equipment.Add(name + "  Qty: " + qty);
            }

            return equipment.Count == 0
                ? "No equipment listed."
                : string.Join(Environment.NewLine, equipment);
        }



        private void dgvBorrowed_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvBorrowed.Rows[e.RowIndex];

            if (row.Cells["DueDate"].Value == null || row.Cells["DueDate"].Value == DBNull.Value)
                return;

            DateTime dueDate = Convert.ToDateTime(row.Cells["DueDate"].Value);
            string status = row.Cells["ItemReturnStatus"].Value?.ToString() ?? "";

            if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase) || 
                (status.Equals("Borrowed", StringComparison.OrdinalIgnoreCase) && DateTime.Now > dueDate))
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(140, 40, 40);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 200, 200);
                row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(120, 30, 30);
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 190, 225);
                row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(69, 45, 96);
            }
        }


        


        private void dgvBorrowed_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int slipId = Convert.ToInt32(dgvBorrowed.Rows[e.RowIndex].Cells["SlipID"].Value);
            ShowBorrowerSlipDialog(slipId, false);
        }

        private void dgvBorrowed_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int slipId = Convert.ToInt32(dgvBorrowed.Rows[e.RowIndex].Cells["SlipID"].Value);
            ShowBorrowerSlipDialog(slipId, false);
        }

        private void dgvHistory_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int slipId = Convert.ToInt32(dgvHistory.Rows[e.RowIndex].Cells["SlipID"].Value);
            ShowBorrowerSlipDialog(slipId, true);
        }



        private void ShowBorrowerSlipDialog(int slipId, bool readOnly)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string leaderName = "", schoolId = "", groupNumber = "", section = "", subjectCode = "", slipStatus = "";
                DateTime borrowDate = DateTime.Now, dueDate = DateTime.Now;

                string slipQuery = @"
SELECT BS.LeaderName, BS.GroupNumber, BS.DateCreated, BS.SlipStatus,
       U.SchoolID, LS.SubjectCode, SS.Section, SS.EndTime
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
                        MessageBox.Show("Borrow slip not found.");
                        return;
                    }
                    leaderName = reader["LeaderName"]?.ToString() ?? "";
                    schoolId = reader["SchoolID"]?.ToString() ?? "";
                    groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    section = reader["Section"]?.ToString() ?? "";
                    subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    slipStatus = reader["SlipStatus"]?.ToString() ?? "";
                    borrowDate = reader["DateCreated"] != DBNull.Value ? Convert.ToDateTime(reader["DateCreated"]) : DateTime.Now;
                    dueDate = BuildDueDate(borrowDate, reader["EndTime"]);
                }

                List<string> members = new List<string>();
                using (OleDbCommand cmd = new OleDbCommand(
                    "SELECT MemberName FROM BorrowSlipMembers WHERE SlipID = ? ORDER BY MemberName", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);
                    using OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        string m = reader["MemberName"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(m)) members.Add(m);
                    }
                }

                List<BorrowSlipReturnItem> items = LoadBorrowSlipReturnItems(conn, slipId);
                List<string> reportLines = new List<string>();
                using (OleDbCommand cmd = new OleDbCommand(@"
SELECT DR.ReportID, E.EquipmentName, DR.DamageType, DR.DamageQuantity, DR.ReportStatus, DR.CurrentReplacementCost
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
                        reportLines.Add("Report #" + reader["ReportID"] + " - " + reader["EquipmentName"] +
                            " | " + reader["DamageType"] + " Qty: " + reader["DamageQuantity"] +
                            " | " + reader["ReportStatus"] +
                            (totalCost > 0 ? " | Cost: ₱" + totalCost.ToString("N2") : ""));
                    }
                }

                // ── FORM ────────────────────────────────────────────────────────────
                Form form = new Form
                {
                    Text = "Borrower's Slip",
                    StartPosition = FormStartPosition.CenterParent,
                    AutoScaleMode = AutoScaleMode.None,
                    ClientSize = new Size(420, 690),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(250, 246, 238)
                };
                form.MinimumSize = form.Size;
                form.MaximumSize = form.Size;

                int py = 16; // running Y cursor
                int px = 24;
                int w = 372;

                // ── Header bar ──────────────────────────────────────────────────────
                Panel pnlHeader = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(420, 46),
                    BackColor = Color.FromArgb(139, 105, 20)
                };

                Label lblTitle = new Label
                {
                    Text = "Borrower's Slip",
                    Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(16, 11),
                    AutoSize = true
                };

                Label statusPill = new Label
                {
                    Text = readOnly ? "VIEW ONLY" : "ACTIVE BORROW",
                    Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(122, 16, 34),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(258, 12),
                    Size = new Size(110, 22)
                };
                RoundControl(statusPill, 11);

                pnlHeader.Controls.Add(lblTitle);
                pnlHeader.Controls.Add(statusPill);
                form.Controls.Add(pnlHeader);

                py = 58; // below header

                // ── Info block ──────────────────────────────────────────────────────
                string infoText =
                    "Group #: " + groupNumber + "\n" +
                    "Date Borrowed: " + borrowDate.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                    "Subject: " + subjectCode + "\n" +
                    "Section: " + (string.IsNullOrWhiteSpace(section) ? "N/A" : section) + "\n" +
                    "Leader: " + leaderName + "\n" +
                    "Student: " + leaderName + " (" + schoolId + ")\n" +
                    "Expected Return: " + dueDate.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                    "Status: " + slipStatus;

                Label lblInfo = new Label
                {
                    Text = infoText,
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = Color.FromArgb(72, 53, 84),
                    Location = new Point(px, py),
                    Size = new Size(w, 148)
                };
                form.Controls.Add(lblInfo);
                py += 154;

                // ── Members label + panel ───────────────────────────────────────────
                Label lblMembersTitle = new Label
                {
                    Text = "Members",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(px, py),
                    AutoSize = true
                };
                form.Controls.Add(lblMembersTitle);
                py += 22;

                Panel pnlMembers = CreateReadonlyDisplayPanel(
                    members.Count == 0 ? "No members listed." : string.Join(Environment.NewLine, members),
                    new Point(px, py),
                    new Size(w, 84),
                    new Font("Segoe UI", 9.5F));
                form.Controls.Add(pnlMembers);
                py += 90;

                // ── Equipments label + flow ─────────────────────────────────────────
                Label lblEquipTitle = new Label
                {
                    Text = "Equipments",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(69, 45, 96),
                    Location = new Point(px, py),
                    AutoSize = true
                };
                form.Controls.Add(lblEquipTitle);
                py += 22;

                int bottomButtonY = form.ClientSize.Height - 64;
                int reportsReserve = reportLines.Count > 0 ? 112 : 0;
                int itemH = bottomButtonY - py - reportsReserve - 18;
                itemH = Math.Max(150, Math.Min(itemH, 250));

                FlowLayoutPanel flowItems = new FlowLayoutPanel
                {
                    Location = new Point(px, py),
                    Size = new Size(w, itemH),
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                foreach (BorrowSlipReturnItem item in items)
                    flowItems.Controls.Add(CreateBorrowSlipItemRow(item, readOnly));
                form.Controls.Add(flowItems);
                py += itemH + 14;

                if (reportLines.Count > 0)
                {
                    Label lblReportsTitle = new Label
                    {
                        Text = "Reports",
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(69, 45, 96),
                        Location = new Point(px, py),
                        AutoSize = true
                    };
                    form.Controls.Add(lblReportsTitle);
                    py += 22;

                    Panel pnlReports = CreateReadonlyDisplayPanel(
                        string.Join(Environment.NewLine, reportLines),
                        new Point(px, py),
                        new Size(w, 76),
                        new Font("Segoe UI", 8.8F));
                    form.Controls.Add(pnlReports);
                    py += 88;
                }

                // ── Buttons ─────────────────────────────────────────────────────────
                Button btnReturnSlip = new Button
                {
                    Text = "Returned",
                    Size = new Size(172, 40),
                    Location = new Point(px, bottomButtonY),
                    BackColor = Color.FromArgb(90, 158, 106),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                    Visible = !readOnly
                };
                btnReturnSlip.FlatAppearance.BorderSize = 0;
                RoundControl(btnReturnSlip, 14);

                Button btnReportSlip = new Button
                {
                    Text = "Report",
                    Size = new Size(172, 40),
                    Location = new Point(px + 176, bottomButtonY),
                    BackColor = Color.FromArgb(192, 57, 75),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                    Visible = !readOnly
                };
                btnReportSlip.FlatAppearance.BorderSize = 0;
                RoundControl(btnReportSlip, 14);

                btnReturnSlip.Click += (s, e) =>
                {
                    if (ProcessReturnForSlip(slipId, items))
                    {
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    }
                };

                btnReportSlip.Click += (s, e) =>
                {
                    List<(int SlipItemID, int ReportQuantity)> reportSelections =
                        ShowReportQuantitySelectionDialog(slipId, items);

                    if (reportSelections.Count == 0)
                        return;

                    form.Hide();

                    foreach ((int slipItemId, int reportQuantity) in reportSelections)
                    {
                        ShowDamageReportForm(slipItemId, reportQuantity);
                    }

                    form.Close();
                };

                form.Controls.Add(btnReturnSlip);
                form.Controls.Add(btnReportSlip);
                form.Shown += (s, e) => form.ActiveControl = readOnly ? (Control)lblTitle : btnReturnSlip;

                form.ShowDialog(this);


                LoadBorrowedData(currentBorrowedSearch);
                LoadHistoryData();
                LoadAdminDashboardNew();
                LoadEquipmentCards(currentEquipmentCategory);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening borrow slip:\n" + ex.Message,
                    "Borrow Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private List<BorrowSlipReturnItem> LoadBorrowSlipReturnItems(OleDbConnection conn, int slipId)
        {
            List<BorrowSlipReturnItem> items = new List<BorrowSlipReturnItem>();

            string query = @"
SELECT
    BSI.SlipItemID,
    BSI.EquipmentID,
    E.EquipmentName,
    E.EquipmentType,
    E.HasSerial,
    BSI.QuantityRequested,
    BSI.QuantityReturned,
    BSI.ItemReturnStatus
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?
ORDER BY E.EquipmentName";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", slipId);
            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                int slipItemId = Convert.ToInt32(reader["SlipItemID"]);
                int quantityRequested = reader["QuantityRequested"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityRequested"]) : 0;

                // Get how many were already reported for this slip item
                int reportedQuantity = 0;
                string reportCheckQuery = @"
SELECT SUM(DamageQuantity)
FROM DamageReports
WHERE SlipItemID = ?";

                using (OleDbCommand reportCmd = new OleDbCommand(reportCheckQuery, conn))
                {
                    reportCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipItemId;
                    object reportResult = reportCmd.ExecuteScalar();
                    if (reportResult != null && reportResult != DBNull.Value)
                        reportedQuantity = Convert.ToInt32(reportResult);
                }

                int effectiveQuantity = Math.Max(0, quantityRequested - reportedQuantity);

                items.Add(new BorrowSlipReturnItem
                {
                    SlipItemID = slipItemId,
                    EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                    EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                    EquipmentType = reader["EquipmentType"]?.ToString() ?? "Reusable",
                    HasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"]),
                    QuantityRequested = effectiveQuantity,
                    QuantityReturned = reader["QuantityReturned"] != DBNull.Value
                        ? Convert.ToInt32(reader["QuantityReturned"]) : 0,
                    ItemReturnStatus = effectiveQuantity == 0
                        ? "Returned"
                        : reader["ItemReturnStatus"]?.ToString() ?? ""
                });
            }

            return items;
        }

        private int GetSlipIdForBorrowSlipItem(OleDbConnection conn, int slipItemId)
        {
            using OleDbCommand cmd = new OleDbCommand(
                "SELECT SlipID FROM BorrowSlipItems WHERE SlipItemID = ?",
                conn);
            cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipItemId;

            object? result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Borrow slip item was not found.");

            return Convert.ToInt32(result);
        }

        private Control CreateBorrowSlipItemRow(BorrowSlipReturnItem item, bool readOnly)
        {
            Panel row = new Panel
            {
                Width = 340,
                Height = item.EquipmentType == "Limited Use" ? 68 : 44,
                Margin = new Padding(8, 8, 8, 0),
                BackColor = item.ItemReturnStatus == "Borrowed"
                    ? Color.FromArgb(255, 251, 252)
                    : Color.FromArgb(245, 242, 247)
            };
            RoundControl(row, 10);

            item.Check = new CheckBox
            {
                Text = item.EquipmentName,
                Checked = item.ItemReturnStatus == "Borrowed",
                Enabled = !readOnly && item.ItemReturnStatus == "Borrowed",
                Location = new Point(10, 9),
                Size = new Size(190, 24),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(72, 53, 84)
            };

                Label qtyLabel = new Label
                {
                Text = item.EquipmentType == "Limited Use" ? "Qty:" : "Qty.",
                Location = new Point(218, 11),
                Size = new Size(30, 20),
                Font = new Font("Segoe UI", 9F)
            };

            item.QuantityPicker = new NumericUpDown
            {
                Minimum = 0,
                Maximum = Math.Max(1, item.QuantityRequested),
                Value = item.ItemReturnStatus == "Borrowed"
                    ? item.QuantityRequested
                    : Math.Min(item.QuantityRequested, item.QuantityReturned),
                Location = new Point(252, 8),
                Size = new Size(52, 24),
                Enabled = false
            };

            item.Check.CheckedChanged += (s, e) =>
            {
                bool canEdit = !readOnly && item.ItemReturnStatus == "Borrowed";

                if (item.Check.Checked)
                {
                    item.QuantityPicker.Value = item.QuantityRequested;
                    item.QuantityPicker.Enabled = false;
                    return;
                }

                item.QuantityPicker.Enabled = canEdit;
            };

            item.QuantityPicker.ValueChanged += (s, e) =>
            {
                if (item.Check.Checked && item.QuantityPicker.Value != item.QuantityRequested)
                    item.QuantityPicker.Value = item.QuantityRequested;
            };

            row.Controls.Add(item.Check);
            row.Controls.Add(qtyLabel);
            row.Controls.Add(item.QuantityPicker);

            if (item.EquipmentType == "Limited Use")
            {
                Label lblUsable = new Label
                {
                    Text = "Usable:",
                    Location = new Point(30, 40),
                    Size = new Size(70, 22),
                    Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(72, 53, 84)
                };

                item.UsableQuantityPicker = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = Math.Max(1, item.QuantityRequested),
                    Value = item.ItemReturnStatus == "Borrowed"
                        ? item.QuantityRequested
                        : Math.Min(item.QuantityRequested, item.QuantityReturned),
                    Location = new Point(106, 38),
                    Size = new Size(54, 24),
                    Enabled = !readOnly && item.ItemReturnStatus == "Borrowed",
                    Font = new Font("Segoe UI", 8.5F)
                };

                row.Controls.Add(lblUsable);
                row.Controls.Add(item.UsableQuantityPicker);
            }

            return row;
        }

        private Panel CreateReadonlyDisplayPanel(string text, Point location, Size size, Font font)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TabStop = false,
                AutoScroll = true
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

        private bool ProcessReturnForSlip(int slipId, List<BorrowSlipReturnItem> items)
        {
            List<BorrowSlipReturnItem> selected = items
                .Where(x => x.Check.Checked && x.ItemReturnStatus == "Borrowed")
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please check at least one equipment to return.",
                    "Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();
                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    foreach (BorrowSlipReturnItem item in selected)
                    {
                        int enteredQty = item.EquipmentType == "Limited Use" && item.UsableQuantityPicker != null
                            ? item.QuantityRequested
                            : (int)item.QuantityPicker.Value;
                        int quantityReturned = enteredQty;
                        string returnedUnitStatus = "Available";
                        string unreturnedUnitStatus = "Borrowed";

                        if (item.EquipmentType == "Consumable" || item.EquipmentType == "One Time Use")
                        {
                            quantityReturned = 0;
                            returnedUnitStatus = "Consumed";
                        }
                        else if (item.EquipmentType == "Limited Use" && item.UsableQuantityPicker != null)
                        {
                            quantityReturned = (int)item.UsableQuantityPicker.Value;
                            returnedUnitStatus = "Available";
                            unreturnedUnitStatus = "Consumed";
                        }

                        string updateItem = @"
UPDATE BorrowSlipItems
SET ItemReturnStatus = 'Returned',
    QuantityReturned = ?
WHERE SlipItemID = ?";

                        if (enteredQty < item.QuantityRequested)
                        {
                            int remainingQty = item.QuantityRequested - enteredQty;

                            using (OleDbCommand cmd = new OleDbCommand(@"
UPDATE BorrowSlipItems
SET QuantityRequested = ?,
    QuantityReturned = 0,
    ItemReturnStatus = 'Borrowed'
WHERE SlipItemID = ?", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@p1", remainingQty);
                                cmd.Parameters.AddWithValue("@p2", item.SlipItemID);
                                cmd.ExecuteNonQuery();
                            }

                            using (OleDbCommand cmd = new OleDbCommand(@"
INSERT INTO BorrowSlipItems
(SlipID, EquipmentID, QuantityRequested, QuantityReturned, ItemReturnStatus)
VALUES (?, ?, ?, ?, 'Returned')", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@p1", slipId);
                                cmd.Parameters.AddWithValue("@p2", item.EquipmentID);
                                cmd.Parameters.AddWithValue("@p3", enteredQty);
                                cmd.Parameters.AddWithValue("@p4", quantityReturned);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (OleDbCommand cmd = new OleDbCommand(updateItem, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@p1", quantityReturned);
                                cmd.Parameters.AddWithValue("@p2", item.SlipItemID);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        if (item.HasSerial)
                            UpdateReturnedSerialUnits(conn, trans, item.SlipItemID, quantityReturned, returnedUnitStatus, unreturnedUnitStatus);
                    }

                    trans.Commit();
                    MessageBox.Show("Selected equipment marked as returned.",
                        "Return", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Error returning equipment:\n" + ex.Message,
                    "Return", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void UpdateReturnedSerialUnits(
            OleDbConnection conn,
            OleDbTransaction trans,
            int slipItemId,
            int quantityReturned,
            string returnedStatus,
            string remainingStatus)
        {
            List<int> availableToUpdate = new List<int>();

            string selectUnits = @"
SELECT EU.UnitID
FROM BorrowSlipUnits AS BSU
INNER JOIN EquipmentUnits AS EU ON BSU.UnitID = EU.UnitID
WHERE BSU.SlipItemID = ?
AND EU.UnitID NOT IN
(
    SELECT DRU.UnitID
    FROM DamageReportUnits AS DRU
    INNER JOIN DamageReports AS DR ON DRU.ReportID = DR.ReportID
    WHERE DR.SlipItemID = ?
)
ORDER BY EU.UnitID";

            using (OleDbCommand cmd = new OleDbCommand(selectUnits, conn, trans))
            {
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipItemId;
                cmd.Parameters.Add("@p2", OleDbType.Integer).Value = slipItemId;

                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                    availableToUpdate.Add(Convert.ToInt32(reader["UnitID"]));
            }

            for (int i = 0; i < availableToUpdate.Count; i++)
            {
                string newStatus = i < quantityReturned ? returnedStatus : remainingStatus;

                using OleDbCommand updateCmd = new OleDbCommand(
                    "UPDATE EquipmentUnits SET UnitStatus = ? WHERE UnitID = ?",
                    conn,
                    trans);
                updateCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = newStatus;
                updateCmd.Parameters.Add("@p2", OleDbType.Integer).Value = availableToUpdate[i];
                updateCmd.ExecuteNonQuery();
            }
        }

        private class BorrowSlipReturnItem
        {
            public int SlipItemID { get; set; }
            public int EquipmentID { get; set; }
            public string EquipmentName { get; set; } = "";
            public string EquipmentType { get; set; } = "Reusable";
            public bool HasSerial { get; set; }
            public int QuantityRequested { get; set; }
            public int QuantityReturned { get; set; }
            public string ItemReturnStatus { get; set; } = "";
            public CheckBox Check { get; set; } = null!;
            public NumericUpDown QuantityPicker { get; set; } = null!;
            public RadioButton Usable { get; set; } = new RadioButton();
            public RadioButton DisposeChoice { get; set; } = new RadioButton();
            public NumericUpDown? UsableQuantityPicker { get; set; }
        }



        private void MarkBorrowAsReturned(int borrowId, int equipmentId, int quantityBorrowed, DateTime dueDate)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                OleDbTransaction tx = conn.BeginTransaction();

                try
                {
                    decimal penaltyAmount = 0;

                    if (DateTime.Now > dueDate)
                    {
                        int overdueDays = (DateTime.Now.Date - dueDate.Date).Days;
                        if (overdueDays < 0) overdueDays = 0;
                        penaltyAmount = overdueDays * 10m;
                    }

                    string updateBorrow = @"
UPDATE BorrowTransactions
SET ReturnDate = ?, BorrowStatus = 'Returned', PenaltyAmount = ?
WHERE BorrowID = ?";

                    using (OleDbCommand updateBorrowCmd = new OleDbCommand(updateBorrow, conn, tx))
                    {
                        updateBorrowCmd.Parameters.AddWithValue("@p1", DateTime.Now);
                        updateBorrowCmd.Parameters.AddWithValue("@p2", penaltyAmount);
                        updateBorrowCmd.Parameters.AddWithValue("@p3", borrowId);
                        updateBorrowCmd.ExecuteNonQuery();
                    }

                    tx.Commit();

                    MessageBox.Show("Item marked as returned successfully.", "Return", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllAdminData();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error returning item:\n" + ex.Message, "Return", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void dgvHistory_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvHistory.Rows[e.RowIndex];
            row.DefaultCellStyle.BackColor = Color.FromArgb(235, 255, 235);
            row.DefaultCellStyle.ForeColor = Color.FromArgb(40, 120, 50);
            row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 240, 200);
            row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 90, 40);
        }

        private void LoadAccountsData()
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string keyword = txtStudentSearch.Text.Trim();
                string likeValue = "%" + keyword + "%";

                string selectedSubject = cmbSubjectFilter.SelectedItem != null
                    ? cmbSubjectFilter.SelectedItem.ToString()
                    : "All";

                int selectedScheduleId = 0;

                if (cmbSectionFilter.SelectedItem is ScheduleFilterItem selectedSchedule)
                    selectedScheduleId = selectedSchedule.ScheduleID;

                string query = @"
SELECT DISTINCT
    U.UserID,
    U.FullName,
    U.SchoolID,
    U.SchoolEmail,
    U.IsActive,
    IIF(U.IsActive = True, 'Active', 'Inactive') AS AccountStatus,
    IIF(U.IsActive = True, 'Deactivate', 'Activate') AS AccountAction
FROM (((Users AS U
INNER JOIN StudentSubjectEnrollments AS SSE ON U.UserID = SSE.UserID)
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID)
INNER JOIN SubjectSchedules AS SS ON SSE.ScheduleID = SS.ScheduleID)
WHERE LS.LabID = ?
AND SSE.IsActive = True
AND
(
    U.FullName LIKE ?
    OR U.SchoolID LIKE ?
    OR U.SchoolEmail LIKE ?
)";

                if (selectedSubject != "All")
                    query += " AND LS.SubjectCode = ?";

                if (selectedScheduleId > 0)
                    query += " AND SSE.ScheduleID = ?";

                query += " ORDER BY U.FullName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);

                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                cmd.Parameters.AddWithValue("@p2", likeValue);
                cmd.Parameters.AddWithValue("@p3", likeValue);
                cmd.Parameters.AddWithValue("@p4", likeValue);

                if (selectedSubject != "All")
                    cmd.Parameters.AddWithValue("@p5", selectedSubject);

                if (selectedScheduleId > 0)
                    cmd.Parameters.AddWithValue("@p6", selectedScheduleId);

                DataTable dt = new DataTable();
                using OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                dgvPendingUsers.DataSource = dt;

                if (dt.Rows.Count > 0)
                {
                    int firstUserId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    LoadAccountDetails(firstUserId);
                }
                else
                {
                    lblStudentName.Text = "No student selected";
                    lblStudentSchoolID.Text = "School ID: ---";
                    lblStudentEmail.Text = "Email: ---";
                    lblStudentBorrowedCount.Text = "0";
                    lblStudentReturnedCount.Text = "0";
                    lblStudentReservationsCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students:\n" + ex.Message,
                    "Students Enrolled", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void InitializeEquipmentAdminButtons()
        {
            // remove old dynamic buttons if already added
            if (btnAddEquipmentDynamic != null && pnlEquipmentMain.Controls.Contains(btnAddEquipmentDynamic))
                pnlEquipmentMain.Controls.Remove(btnAddEquipmentDynamic);

            if (btnRefreshEquipmentDynamic != null && pnlEquipmentMain.Controls.Contains(btnRefreshEquipmentDynamic))
                pnlEquipmentMain.Controls.Remove(btnRefreshEquipmentDynamic);

            btnAddEquipmentDynamic = new Button
            {
                Name = "btnAddEquipmentDynamic",
                Text = "+ Add Equipment",
                Size = new Size(148, 34),
                Location = new Point(28, 104),
                BackColor = Color.FromArgb(169, 132, 194),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnAddEquipmentDynamic.FlatAppearance.BorderSize = 0;

            btnRefreshEquipmentDynamic = new Button
            {
                Name = "btnRefreshEquipmentDynamic",
                Text = "↻ Refresh",
                Size = new Size(100, 34),
                Location = new Point(188, 104),
                BackColor = Color.FromArgb(190, 170, 205),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnRefreshEquipmentDynamic.FlatAppearance.BorderSize = 0;

            ApplyButtonStyle(btnAddEquipmentDynamic);
            ApplyButtonStyle(btnRefreshEquipmentDynamic);

            RoundControl(btnAddEquipmentDynamic, 16);
            RoundControl(btnRefreshEquipmentDynamic, 16);

            btnAddEquipmentDynamic.Click += btnAddEquipment_Click;

            btnRefreshEquipmentDynamic.Click += (s, e) =>
            {
                LoadEquipmentCards(currentEquipmentCategory);
            };

            pnlEquipmentMain.Controls.Add(btnAddEquipmentDynamic);
            pnlEquipmentMain.Controls.Add(btnRefreshEquipmentDynamic);

            btnAddEquipmentDynamic.BringToFront();
            btnRefreshEquipmentDynamic.BringToFront();
        }
        private void LoadAccountDetails(int userId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string userQuery = @"
SELECT FullName, SchoolID, SchoolEmail, IsActive
FROM Users
WHERE UserID = ?";

                using (OleDbCommand cmd = new OleDbCommand(userQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", userId);

                    using OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader != null && reader.Read())
                    {
                        lblStudentName.Text = reader["FullName"]?.ToString() ?? "No name";
                        lblStudentSchoolID.Text = "School ID: " + (reader["SchoolID"]?.ToString() ?? "---");
                        lblStudentEmail.Text = "Email: " + (reader["SchoolEmail"]?.ToString() ?? "---");
                    }
                }

                lblStudentBorrowedCount.Text = GetScalarCount(conn, @"
SELECT SUM(BSI.QuantityRequested - BSI.QuantityReturned)
FROM (BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE BS.UserID = ?
AND LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'", userId, SessionManager.LabID).ToString();

                lblStudentReturnedCount.Text = GetScalarCount(conn, @"
SELECT SUM(BSI.QuantityReturned)
FROM (BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE BS.UserID = ?
AND LS.LabID = ?
AND BS.SlipStatus = 'Approved'", userId, SessionManager.LabID).ToString();

                lblStudentReservationsCount.Text = GetStudentOverdueQuantity(conn, userId).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student details:\n" + ex.Message,
                    "Students Enrolled", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetStudentOverdueQuantity(OleDbConnection conn, int userId)
        {
            int overdueQuantity = 0;

            string query = @"
SELECT
    BS.DateCreated,
    SS.EndTime,
    BSI.QuantityRequested,
    BSI.QuantityReturned
FROM ((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE BS.UserID = ?
AND LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", userId);
            cmd.Parameters.AddWithValue("@p2", SessionManager.LabID);

            using OleDbDataReader reader = cmd.ExecuteReader();

            while (reader != null && reader.Read())
            {
                DateTime dateCreated = Convert.ToDateTime(reader["DateCreated"]);
                DateTime endTime = Convert.ToDateTime(reader["EndTime"]);
                DateTime deadline = dateCreated.Date.Add(endTime.TimeOfDay);

                if (DateTime.Now <= deadline)
                    continue;

                int requested = reader["QuantityRequested"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityRequested"])
                    : 0;

                int returned = reader["QuantityReturned"] != DBNull.Value
                    ? Convert.ToInt32(reader["QuantityReturned"])
                    : 0;

                overdueQuantity += Math.Max(0, requested - returned);
            }

            return overdueQuantity;
        }



        private class MaintenanceReservationNotice
        {
            public string FullName { get; set; } = "";
            public string SchoolEmail { get; set; } = "";
            public string EquipmentName { get; set; } = "";
            public int QuantityReserved { get; set; }
            public DateTime ReservationDate { get; set; }
        }


        

        private Panel CreateReservationNotificationCard(
    string fullName,
    string equipmentName,
    int qty,
    DateTime reservationDate,
    string reservationStatus)
        {
            Color backColor = Color.FromArgb(255, 239, 213);
            Color badgeFore = Color.FromArgb(160, 98, 27);

            Panel card = new Panel
            {
                Width = 398,
                Height = 86,
                BackColor = backColor,
                Margin = new Padding(0, 0, 0, 12)
            };
            RoundControl(card, 18);

            Label lblTitle = new Label
            {
                Text = equipmentName,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(14, 12),
                Size = new Size(230, 22)
            };

            Label lblDetails = new Label
            {
                Text = $"{fullName} • Qty: {qty} • Reservation Date: {reservationDate:MM/dd/yyyy}",
                Font = new Font("Segoe UI", 8.8F),
                ForeColor = Color.FromArgb(110, 90, 122),
                Location = new Point(14, 42),
                Size = new Size(245, 18)
            };

            Label lblStatus = new Label
            {
                Text = reservationStatus,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = badgeFore,
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(278, 29),
                Size = new Size(104, 26)
            };
            RoundControl(lblStatus, 12);

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDetails);
            card.Controls.Add(lblStatus);

            return card;
        }



        private void dgvPendingUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                using (Pen headerPen = new Pen(Color.FromArgb(215, 205, 230), 1))
                {
                    e.Graphics.DrawLine(
                        headerPen,
                        e.CellBounds.Left,
                        e.CellBounds.Bottom - 1,
                        e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }

                e.Handled = true;
                return;
            }

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            using (Pen rowPen = new Pen(Color.FromArgb(228, 220, 238), 1))
            {
                e.Graphics.DrawLine(
                    rowPen,
                    e.CellBounds.Left + 6,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right - 6,
                    e.CellBounds.Bottom - 1);
            }

            e.Handled = true;
        }


        private void ApplyButtonStyle(Button btn)
        {
            if (styledButtons.Contains(btn))
                return;

            styledButtons.Add(btn);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.ForeColor = Color.White;

            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    pressedButtons.Add(btn);
                    btn.Invalidate();
                }
            };

            btn.MouseUp += (s, e) =>
            {
                pressedButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.MouseLeave += (s, e) =>
            {
                pressedButtons.Remove(btn);
                btn.Invalidate();
            };

            btn.Paint += (s, e) =>
            {
                if (s is not Button b) return;
                bool isPressed = pressedButtons.Contains(b);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using GraphicsPath path = GetRoundedPath(rect, 12);

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
            btnNavVerification.BackColor = baseYellow;
            btnNavEquipment.BackColor = baseYellow;
            btnNavBorrowed.BackColor = baseYellow;
            btnNavReservations.BackColor = baseYellow;
            btnNavHistory.BackColor = baseYellow;

            btnNavDashboard.ForeColor = Color.White;
            btnNavVerification.ForeColor = Color.White;
            btnNavEquipment.ForeColor = Color.White;
            btnNavBorrowed.ForeColor = Color.White;
            btnNavReservations.ForeColor = Color.White;
            btnNavHistory.ForeColor = Color.White;

            btnNavDashboard.Invalidate();
            btnNavVerification.Invalidate();
            btnNavEquipment.Invalidate();
            btnNavBorrowed.Invalidate();
            btnNavReservations.Invalidate();
            btnNavHistory.Invalidate();
        }


        private void SetActiveButton(Button btn)
        {
            ResetSidebarButtons();

            btn.BackColor = Color.FromArgb(212, 168, 45);
            btn.ForeColor = Color.White;
            btn.Invalidate();
        }


        private void SetEquipmentFilterButtons(Button active)
        {
            Color normalBack = Color.FromArgb(212, 168, 45);
            Color normalFore = Color.White;

            btnEqAll.BackColor = normalBack;
            btnEqTechnical.BackColor = normalBack;
            btnEqScience.BackColor = normalBack;
            btnEqSports.BackColor = normalBack;
            btnEqGeneral.BackColor = normalBack;

            btnEqAll.ForeColor = normalFore;
            btnEqTechnical.ForeColor = normalFore;
            btnEqScience.ForeColor = normalFore;
            btnEqSports.ForeColor = normalFore;
            btnEqGeneral.ForeColor = normalFore;

            btnEqAll.Invalidate();
            btnEqTechnical.Invalidate();
            btnEqScience.Invalidate();
            btnEqSports.Invalidate();
            btnEqGeneral.Invalidate();

            active.BackColor = Color.FromArgb(184, 140, 25); // darker yellow for active pressed look
            active.ForeColor = Color.White;
            active.Invalidate();

            UpdateEquipmentSectionTitle();
        }



        private void LoadLowStockAlerts()
        {
            try
            {
                flowRecentActivity.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT 
    EquipmentID,
    EquipmentName,
    QuantityTotal,
    QuantityMaintenance,
    LowStockThreshold,
    EquipmentType,
    HasSerial
FROM Equipment
WHERE LabID = ?
AND IsArchived = False
AND Status = 'Active'
ORDER BY EquipmentName";

                List<DashboardStockItem> stockItems = new List<DashboardStockItem>();

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                    using OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader != null && reader.Read())
                    {
                        stockItems.Add(new DashboardStockItem
                        {
                            EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                            EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                            QuantityTotal = reader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(reader["QuantityTotal"]) : 0,
                            QuantityMaintenance = reader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(reader["QuantityMaintenance"]) : 0,
                            LowStockThreshold = reader["LowStockThreshold"] != DBNull.Value ? Convert.ToInt32(reader["LowStockThreshold"]) : 3,
                            EquipmentType = reader["EquipmentType"] != DBNull.Value ? reader["EquipmentType"].ToString() ?? "Reusable" : "Reusable",
                            HasSerial = reader["HasSerial"] != DBNull.Value && Convert.ToBoolean(reader["HasSerial"])
                        });
                    }
                }

                bool hasAlert = false;

                string pendingSlipQuery = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    BS.LeaderName,
    BS.DateCreated,
    LS.SubjectCode,
    SS.Section
FROM (BorrowSlips AS BS
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Pending'
ORDER BY BS.DateCreated DESC";

                using (OleDbCommand pendingCmd = new OleDbCommand(pendingSlipQuery, conn))
                {
                    pendingCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                    using OleDbDataReader pendingReader = pendingCmd.ExecuteReader();
                    while (pendingReader != null && pendingReader.Read())
                    {
                        hasAlert = true;

                        int slipId = Convert.ToInt32(pendingReader["SlipID"]);
                        string groupNumber = pendingReader["GroupNumber"]?.ToString() ?? "";
                        string leaderName = pendingReader["LeaderName"]?.ToString() ?? "";
                        string subjectCode = pendingReader["SubjectCode"]?.ToString() ?? "";
                        string section = pendingReader["Section"]?.ToString() ?? "N/A";
                        DateTime sentDate = pendingReader["DateCreated"] != DBNull.Value
                            ? Convert.ToDateTime(pendingReader["DateCreated"])
                            : DateTime.Now;

                        Panel slipAlert = CreateDashboardNotificationCard(
                            "New Slip Request",
                            "Group " + groupNumber + " | " + subjectCode + " " + section + " | " + leaderName + " | " + sentDate.ToString("MMM dd, hh:mm tt"),
                            Color.FromArgb(255, 247, 224),
                            Color.FromArgb(153, 0, 0));

                        slipAlert.Cursor = Cursors.Hand;
                        slipAlert.Click += (s, e) => ShowSlipDetailsDialog(slipId);
                        foreach (Control child in slipAlert.Controls)
                            child.Click += (s, e) => ShowSlipDetailsDialog(slipId);

                        flowRecentActivity.Controls.Add(slipAlert);
                    }
                }

                foreach (DashboardStockItem item in stockItems)
                {
                    int available = GetCorrectAvailableQuantity(
                        conn,
                        item.EquipmentID,
                        item.QuantityTotal,
                        item.QuantityMaintenance,
                        item.HasSerial,
                        item.EquipmentType);

                    if (available <= item.LowStockThreshold)
                    {
                        hasAlert = true;

                        flowRecentActivity.Controls.Add(CreateDashboardNotificationCard(
                            "Low Stock Alert",
                            item.EquipmentName + " | " + item.EquipmentType + " | Available: " + available + " | Threshold: " + item.LowStockThreshold,
                            Color.FromArgb(255, 239, 213),
                            Color.FromArgb(160, 98, 27)));
                    }
                }

                string overdueQuery = @"
SELECT
    E.EquipmentName,
    BS.DateCreated,
    SS.EndTime,
    BSI.QuantityRequested,
    BSI.QuantityReturned
FROM (((BorrowSlipItems AS BSI
INNER JOIN BorrowSlips AS BS ON BSI.SlipID = BS.SlipID)
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID)
INNER JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
AND BS.SlipStatus = 'Approved'
AND BSI.ItemReturnStatus = 'Borrowed'
ORDER BY BS.DateCreated DESC";

                using (OleDbCommand overdueCmd = new OleDbCommand(overdueQuery, conn))
                {
                    overdueCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                    using OleDbDataReader overdueReader = overdueCmd.ExecuteReader();

                    while (overdueReader != null && overdueReader.Read())
                    {
                        DateTime dateCreated = Convert.ToDateTime(overdueReader["DateCreated"]);
                        DateTime endTime = Convert.ToDateTime(overdueReader["EndTime"]);
                        DateTime deadline = dateCreated.Date.Add(endTime.TimeOfDay);

                        if (DateTime.Now <= deadline)
                            continue;

                        int requested = overdueReader["QuantityRequested"] != DBNull.Value
                            ? Convert.ToInt32(overdueReader["QuantityRequested"])
                            : 0;

                        int returned = overdueReader["QuantityReturned"] != DBNull.Value
                            ? Convert.ToInt32(overdueReader["QuantityReturned"])
                            : 0;

                        int quantity = Math.Max(0, requested - returned);
                        string equipmentName = overdueReader["EquipmentName"]?.ToString() ?? "Equipment";

                        hasAlert = true;
                        flowRecentActivity.Controls.Add(CreateDashboardNotificationCard(
                            "Overdue Borrowed Equipment",
                            equipmentName + " | Qty: " + quantity + " | Deadline: " + deadline.ToString("MMM dd, yyyy hh:mm tt"),
                            Color.FromArgb(255, 235, 235),
                            Color.FromArgb(150, 38, 38)));
                    }
                }

                if (!hasAlert)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No dashboard notifications.";
                    lblEmpty.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                    lblEmpty.ForeColor = Color.FromArgb(126, 105, 136);
                    lblEmpty.AutoSize = false;
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
                    lblEmpty.Size = new Size(400, 60);

                    flowRecentActivity.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading low stock alerts:\n" + ex.Message,
                    "Low Stock",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private Panel CreateDashboardNotificationCard(string title, string details, Color backColor, Color titleColor)
        {
            Panel alertCard = new Panel();
            alertCard.Width = 380;
            alertCard.Height = 68;
            alertCard.BackColor = backColor;
            alertCard.Margin = new Padding(0, 0, 0, 10);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTitle.ForeColor = titleColor;
            lblTitle.Location = new Point(12, 8);
            lblTitle.AutoSize = true;

            Label lblInfo = new Label();
            lblInfo.Text = details;
            lblInfo.Font = new Font("Segoe UI", 8.8F);
            lblInfo.ForeColor = Color.FromArgb(92, 45, 58);
            lblInfo.Location = new Point(12, 32);
            lblInfo.Size = new Size(350, 28);

            alertCard.Controls.Add(lblTitle);
            alertCard.Controls.Add(lblInfo);
            RoundControl(alertCard, 16);

            return alertCard;
        }




        private Panel CreatePendingSlipCard(
            int slipId,
            string groupNumber,
            string subjectCode,
            string slipType,
            string status,
            string leaderName,
            string section,
            DateTime dateCreated,
            string membersText,
            string equipmentText)
        {
            Panel card = new Panel
            {
                Size = new Size(300, 380),
                BackColor = Color.White,
                Margin = new Padding(12),
                Cursor = Cursors.Hand,
                Tag = slipId
            };

            RoundControl(card, 18);
            ApplyNeumorphismPanel(card, 18);

            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(300, 48),
                BackColor = GetSubjectColor(subjectCode)
            };

            Label icon = new Label
            {
                Text = GetSubjectIcon(subjectCode),
                Font = new Font("Segoe UI Emoji", 18F),
                ForeColor = Color.White,
                Location = new Point(14, 10),
                AutoSize = true
            };

            header.Controls.Add(icon);

            Label lblGroup = new Label
            {
                Text = "Group #: " + groupNumber,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(16, 60),
                Size = new Size(260, 22)
            };

            Label lblSummary = new Label
            {
                Text =
                    "Date Borrowed: " + dateCreated.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                    "Subject: " + subjectCode + "\n" +
                    "Section: " + (string.IsNullOrWhiteSpace(section) ? "N/A" : section) + "\n" +
                    "Leader: " + leaderName,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(16, 84),
                Size = new Size(260, 74)
            };

            Label lblMembersTitle = new Label
            {
                Text = "Members:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(16, 160),
                Size = new Size(260, 20)
            };

            Label lblMembers = new Label
            {
                Text = membersText,
                Font = new Font("Segoe UI", 8.7F),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(16, 180),
                Size = new Size(260, 58)
            };

            Label lblEquipmentTitle = new Label
            {
                Text = "Equipments:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(16, 242),
                Size = new Size(260, 20)
            };

            Label lblEquipment = new Label
            {
                Text = equipmentText,
                Font = new Font("Segoe UI", 8.7F),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(16, 262),
                Size = new Size(260, 48)
            };

            Button btnView = new Button
            {
                Text = "View Slip",
                Size = new Size(86, 32),
                Location = new Point(16, 324),
                BackColor = Color.FromArgb(212, 168, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnView.FlatAppearance.BorderSize = 0;
            btnView.Click += (s, e) =>
            {
                ShowSlipDetailsDialog(slipId);
            };

            Button btnDecline = new Button
            {
                Text = "Decline",
                Size = new Size(82, 32),
                Location = new Point(108, 324),
                BackColor = Color.FromArgb(220, 95, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = status == "Pending"
            };
            btnDecline.FlatAppearance.BorderSize = 0;
            btnDecline.Click += (s, e) => DeclineSlipDirectly(slipId);

            Button btnApprove = new Button
            {
                Text = "Approve",
                Size = new Size(82, 32),
                Location = new Point(196, 324),
                BackColor = Color.FromArgb(169, 215, 159),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = status == "Pending"
            };
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Click += (s, e) => ApproveSlipDirectly(slipId);

            card.Controls.Add(header);
            card.Controls.Add(lblGroup);
            card.Controls.Add(lblSummary);
            card.Controls.Add(lblMembersTitle);
            card.Controls.Add(lblMembers);
            card.Controls.Add(lblEquipmentTitle);
            card.Controls.Add(lblEquipment);
            card.Controls.Add(btnView);
            card.Controls.Add(btnDecline);
            card.Controls.Add(btnApprove);

            RoundControl(btnView, 12);
            RoundControl(btnDecline, 12);
            RoundControl(btnApprove, 12);

            return card;
        }

        private void ShowSlipDetailsDialog(int slipId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT
    BS.SlipID,
    BS.GroupNumber,
    BS.LeaderName,
    BS.SlipType,
    BS.DateCreated,
    BS.SlipStatus,
    BS.DeclineReason,
    U.FullName,
    U.SchoolID,
    LS.SubjectCode,
    LS.SubjectName,
    SS.Section
FROM ((BorrowSlips AS BS
INNER JOIN Users AS U ON BS.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON BS.SubjectID = LS.SubjectID)
LEFT JOIN SubjectSchedules AS SS ON BS.ScheduleID = SS.ScheduleID
WHERE BS.SlipID = ?";

                string fullName = "";
                string schoolId = "";
                string subjectCode = "";
                string groupNumber = "";
                string leaderName = "";
                string slipType = "";
                string slipStatus = "";
                string section = "";
                DateTime dateCreated = DateTime.Now;

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", slipId);

                    using OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader == null || !reader.Read())
                    {
                        MessageBox.Show("Slip not found.", "Slip Details",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    fullName = reader["FullName"]?.ToString() ?? "";
                    schoolId = reader["SchoolID"]?.ToString() ?? "";
                    subjectCode = reader["SubjectCode"]?.ToString() ?? "";
                    groupNumber = reader["GroupNumber"]?.ToString() ?? "";
                    leaderName = reader["LeaderName"]?.ToString() ?? "";
                    slipType = reader["SlipType"]?.ToString() ?? "";
                    slipStatus = reader["SlipStatus"]?.ToString() ?? "";
                    section = reader["Section"]?.ToString() ?? "";
                    dateCreated = reader["DateCreated"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateCreated"])
                        : DateTime.Now;
                }

                string membersText = GetSlipMembersText(conn, slipId);
                string itemsText = GetSlipEquipmentText(conn, slipId);

                Form detailForm = new Form();
                detailForm.Text = "Slip Details - " + slipId;
                detailForm.StartPosition = FormStartPosition.CenterParent;
                detailForm.AutoScaleMode = AutoScaleMode.None;
                detailForm.ClientSize = new Size(380, 610);
                detailForm.MinimumSize = detailForm.Size;
                detailForm.MaximumSize = detailForm.Size;
                detailForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                detailForm.MaximizeBox = false;
                detailForm.MinimizeBox = false;
                detailForm.BackColor = Color.FromArgb(250, 245, 247);

                Label lblTitle = new Label();
                lblTitle.Text = "Borrow Slip #" + slipId;
                lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
                lblTitle.Location = new Point(28, 20);
                lblTitle.AutoSize = true;

                Label lblInfo = new Label();
                lblInfo.Text =
                    "Group #: " + groupNumber + "\n" +
                    "Date Borrowed: " + dateCreated.ToString("MM/dd/yyyy hh:mm tt") + "\n" +
                    "Subject: " + subjectCode + "\n" +
                    "Section: " + (string.IsNullOrWhiteSpace(section) ? "N/A" : section) + "\n" +
                    "Leader: " + leaderName + "\n" +
                    "Student: " + fullName + " (" + schoolId + ")\n" +
                    "Status: " + slipStatus;
                lblInfo.Font = new Font("Segoe UI", 10F);
                lblInfo.ForeColor = Color.FromArgb(72, 53, 84);
                lblInfo.Location = new Point(28, 68);
                lblInfo.Size = new Size(335, 150);

                Label lblMembersTitle = new Label();
                lblMembersTitle.Text = "Members:";
                lblMembersTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblMembersTitle.ForeColor = Color.FromArgb(69, 45, 96);
                lblMembersTitle.Location = new Point(28, 220);
                lblMembersTitle.AutoSize = true;

                Panel txtMembers = CreateReadonlyDisplayPanel(
                    membersText,
                    new Point(28, 246),
                    new Size(335, 90),
                    new Font("Segoe UI", 10F));

                Label lblItemsTitle = new Label();
                lblItemsTitle.Text = "Equipments:";
                lblItemsTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblItemsTitle.ForeColor = Color.FromArgb(69, 45, 96);
                lblItemsTitle.Location = new Point(28, 350);
                lblItemsTitle.AutoSize = true;

                Panel txtItems = CreateReadonlyDisplayPanel(
                    string.IsNullOrWhiteSpace(itemsText) ? "No items found." : itemsText,
                    new Point(28, 376),
                    new Size(335, 90),
                    new Font("Segoe UI", 10F));

                Button btnApprove = new Button();
                btnApprove.Text = "Approve";
                btnApprove.Size = new Size(150, 42);
                btnApprove.Location = new Point(212, 510);
                btnApprove.BackColor = Color.FromArgb(169, 215, 159);
                btnApprove.ForeColor = Color.White;
                btnApprove.FlatStyle = FlatStyle.Flat;
                btnApprove.FlatAppearance.BorderSize = 0;
                btnApprove.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                btnApprove.Visible = slipStatus == "Pending";
                btnApprove.Cursor = Cursors.Hand;

                Button btnDecline = new Button();
                btnDecline.Text = "Decline";
                btnDecline.Size = new Size(150, 42);
                btnDecline.Location = new Point(28, 510);
                btnDecline.BackColor = Color.FromArgb(220, 95, 107);
                btnDecline.ForeColor = Color.White;
                btnDecline.FlatStyle = FlatStyle.Flat;
                btnDecline.FlatAppearance.BorderSize = 0;
                btnDecline.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                btnDecline.Visible = slipStatus == "Pending";
                btnDecline.Cursor = Cursors.Hand;

                bool approvePressed = false;
                btnApprove.MouseDown += (s, e) => {
                    approvePressed = true;
                    btnApprove.BackColor = Color.FromArgb(120, 180, 110);
                    btnApprove.Location = new Point(btnApprove.Left + 1, btnApprove.Top + 1);
                };
                btnApprove.MouseUp += (s, e) => {
                    approvePressed = false;
                    btnApprove.BackColor = Color.FromArgb(169, 215, 159);
                    btnApprove.Location = new Point(btnApprove.Left - 1, btnApprove.Top - 1);
                };
                btnApprove.MouseLeave += (s, e) => {
                    if (approvePressed)
                    {
                        approvePressed = false;
                        btnApprove.BackColor = Color.FromArgb(169, 215, 159);
                        btnApprove.Location = new Point(btnApprove.Left - 1, btnApprove.Top - 1);
                    }
                };

                bool declinePressed = false;
                btnDecline.MouseDown += (s, e) => {
                    declinePressed = true;
                    btnDecline.BackColor = Color.FromArgb(180, 60, 75);
                    btnDecline.Location = new Point(btnDecline.Left + 1, btnDecline.Top + 1);
                };
                btnDecline.MouseUp += (s, e) => {
                    declinePressed = false;
                    btnDecline.BackColor = Color.FromArgb(220, 95, 107);
                    btnDecline.Location = new Point(btnDecline.Left - 1, btnDecline.Top - 1);
                };
                btnDecline.MouseLeave += (s, e) => {
                    if (declinePressed)
                    {
                        declinePressed = false;
                        btnDecline.BackColor = Color.FromArgb(220, 95, 107);
                        btnDecline.Location = new Point(btnDecline.Left - 1, btnDecline.Top - 1);
                    }
                };

                // APPROVE - directly use slipId, no row selection needed
                btnApprove.Click += (s, e) =>
                {
                    detailForm.Close();
                    ApproveSlipDirectly(slipId);
                };

                // DECLINE - directly use slipId, no row selection needed
                btnDecline.Click += (s, e) =>
                {
                    detailForm.Close();
                    DeclineSlipDirectly(slipId);
                };

                detailForm.Controls.Add(lblTitle);
                detailForm.Controls.Add(lblInfo);
                detailForm.Controls.Add(lblMembersTitle);
                detailForm.Controls.Add(txtMembers);
                detailForm.Controls.Add(lblItemsTitle);
                detailForm.Controls.Add(txtItems);
                detailForm.Controls.Add(btnApprove);
                detailForm.Controls.Add(btnDecline);
                detailForm.Shown += (s, e) =>
                {
                    if (btnDecline.Visible)
                        detailForm.ActiveControl = btnDecline;
                    else
                        detailForm.ActiveControl = lblTitle;
                };

                RoundControl(btnApprove, 18);
                RoundControl(btnDecline, 18);

                detailForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading slip details:\n" + ex.Message,
                    "Slip Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ApproveSlipDirectly(int slipId)
        {
            try
            {
                List<PendingSlipItemForApproval> items = new List<PendingSlipItemForApproval>();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string checkQuery = "SELECT SlipStatus FROM BorrowSlips WHERE SlipID = ?";
                using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@p1", slipId);
                    string status = checkCmd.ExecuteScalar()?.ToString() ?? "";

                    if (status != "Pending")
                    {
                        MessageBox.Show("Only pending slips can be approved.",
                            "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                string itemQuery = @"
SELECT
    BSI.SlipItemID,
    BSI.EquipmentID,
    BSI.QuantityRequested,
    E.EquipmentName,
    E.EquipmentType,
    E.HasSerial
FROM BorrowSlipItems AS BSI
INNER JOIN Equipment AS E ON BSI.EquipmentID = E.EquipmentID
WHERE BSI.SlipID = ?";

                using (OleDbCommand itemCmd = new OleDbCommand(itemQuery, conn))
                {
                    itemCmd.Parameters.AddWithValue("@p1", slipId);

                    using OleDbDataReader reader = itemCmd.ExecuteReader();

                    while (reader != null && reader.Read())
                    {
                        items.Add(new PendingSlipItemForApproval
                        {
                            SlipItemID = Convert.ToInt32(reader["SlipItemID"]),
                            EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                            EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                            QuantityRequested = reader["QuantityRequested"] != DBNull.Value
                                ? Convert.ToInt32(reader["QuantityRequested"]) : 1,
                            HasSerial = CanRequireSerialAssignment(
                                reader["EquipmentType"]?.ToString() ?? "",
                                reader["HasSerial"] != DBNull.Value &&
                                Convert.ToBoolean(reader["HasSerial"]))
                        });
                    }
                }

                if (items.Count == 0)
                {
                    MessageBox.Show("This slip has no equipment items.",
                        "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Approve this borrower slip?\n\nFor serialized equipment, you will assign the actual unit/s issued.",
                    "Approve Slip", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                foreach (PendingSlipItemForApproval item in items)
                {
                    if (!item.HasSerial) continue;

                    int availableUnits = CountAvailableSerialUnits(item.EquipmentID);

                    if (availableUnits < item.QuantityRequested)
                    {
                        MessageBox.Show(
                            "Not enough available serial units for " + item.EquipmentName + ".\n\n" +
                            "Needed: " + item.QuantityRequested + "\nAvailable: " + availableUnits,
                            "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    List<int>? selectedUnits = ShowSerialSelectionDialog(
                        item.EquipmentID, item.EquipmentName, item.QuantityRequested);

                    if (selectedUnits == null || selectedUnits.Count != item.QuantityRequested)
                    {
                        MessageBox.Show("Approval cancelled because serial number assignment was not completed.",
                            "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    item.SelectedUnitIDs = selectedUnits;
                }

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    foreach (PendingSlipItemForApproval item in items)
                    {
                        if (!item.HasSerial) continue;

                        foreach (int unitId in item.SelectedUnitIDs)
                        {
                            string insertUnitQuery = @"
INSERT INTO BorrowSlipUnits (SlipItemID, UnitID, DateAssigned)
VALUES (?, ?, ?)";
                            using (OleDbCommand unitCmd = new OleDbCommand(insertUnitQuery, conn, trans))
                            {
                                unitCmd.Parameters.AddWithValue("@p1", item.SlipItemID);
                                unitCmd.Parameters.AddWithValue("@p2", unitId);
                                unitCmd.Parameters.Add("@p3", OleDbType.Date).Value = DateTime.Now.Date;
                                unitCmd.ExecuteNonQuery();
                            }

                            string updateUnitQuery = "UPDATE EquipmentUnits SET UnitStatus = 'Borrowed' WHERE UnitID = ?";
                            using (OleDbCommand updateUnitCmd = new OleDbCommand(updateUnitQuery, conn, trans))
                            {
                                updateUnitCmd.Parameters.AddWithValue("@p1", unitId);
                                updateUnitCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    string approveQuery = "UPDATE BorrowSlips SET SlipStatus = 'Approved' WHERE SlipID = ?";
                    using (OleDbCommand approveCmd = new OleDbCommand(approveQuery, conn, trans))
                    {
                        approveCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                        approveCmd.ExecuteNonQuery();
                    }

                    // FIXED: was passing two params for one ?
                    string updateItemsQuery = "UPDATE BorrowSlipItems SET ItemReturnStatus = 'Borrowed' WHERE SlipID = ?";
                    using (OleDbCommand itemCmd = new OleDbCommand(updateItemsQuery, conn, trans))
                    {
                        itemCmd.Parameters.Add("@p1", OleDbType.Integer).Value = slipId;
                        itemCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }

                MessageBox.Show("Borrower slip approved successfully.",
                    "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // REPLACE the existing refresh block at the bottom of ApproveSlipDirectly:
                LoadReservationsData();
                LoadBorrowedData();
                LoadEquipmentCards(currentEquipmentCategory);
                LoadAdminDashboardNew(); // this already calls LoadBorrowedChart now
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving slip:\n" + ex.Message,
                    "Approve Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void DeclineSlipDirectly(int slipId)
        {
            string reason = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter reason for declining this borrower slip:",
                "Decline Slip",
                "Please approach the NAS/admin for clarification.");

            if (string.IsNullOrWhiteSpace(reason))
                return;

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string declineQuery = @"
UPDATE BorrowSlips
SET SlipStatus = 'Declined', DeclineReason = ?
WHERE SlipID = ?";

                using OleDbCommand cmd = new OleDbCommand(declineQuery, conn);
                cmd.Parameters.AddWithValue("@p1", reason);
                cmd.Parameters.AddWithValue("@p2", slipId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Borrower slip declined successfully.",
                    "Decline Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadReservationsData();
                LoadAdminDashboardNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error declining slip:\n" + ex.Message,
                    "Decline Slip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Color GetSubjectColor(string subjectCode)
        {
            string code = subjectCode.ToUpper();

            if (code.Contains("ME"))
                return Color.FromArgb(70, 130, 180);

            if (code.Contains("ECE") || code.Contains("EE"))
                return Color.FromArgb(212, 168, 45);

            if (code.Contains("CHE") || code.Contains("CHM"))
                return Color.FromArgb(100, 170, 120);

            return Color.FromArgb(150, 120, 170);
        }

        private string GetSubjectIcon(string subjectCode)
        {
            string code = subjectCode.ToUpper();

            if (code.Contains("ME"))
                return "⚙️";

            if (code.Contains("ECE") || code.Contains("EE"))
                return "🔌";

            if (code.Contains("CHE") || code.Contains("CHM"))
                return "🧪";

            return "📘";
        }





        private void BuildExperimentManualAdminPanel()
        {
            pnlExperimentManualAdmin = new Panel();
            pnlExperimentManualAdmin.Name = "pnlExperimentManualAdmin";
            pnlExperimentManualAdmin.Location = new Point(292, 144);
            pnlExperimentManualAdmin.Size = new Size(1040, 596);
            pnlExperimentManualAdmin.BackColor = Color.FromArgb(255, 251, 252);

            RoundControl(pnlExperimentManualAdmin, 28);

            Label lblTitle = new Label();
            lblTitle.Text = "CREATE EXPERIMENT MANUAL";
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(32, 26);
            lblTitle.AutoSize = true;

            Label lblSubject = new Label();
            lblSubject.Text = "Subject";
            lblSubject.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSubject.Location = new Point(35, 78);
            lblSubject.AutoSize = true;

            cmbManualSubject = new ComboBox();
            cmbManualSubject.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualSubject.Font = new Font("Segoe UI", 9.5F);
            cmbManualSubject.Location = new Point(35, 104);
            cmbManualSubject.Size = new Size(250, 25);

            Label lblExpName = new Label();
            lblExpName.Text = "Experiment Name";
            lblExpName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblExpName.Location = new Point(310, 78);
            lblExpName.AutoSize = true;

            txtManualExperimentName = new TextBox();
            txtManualExperimentName.Font = new Font("Segoe UI", 9.5F);
            txtManualExperimentName.Location = new Point(310, 104);
            txtManualExperimentName.Size = new Size(330, 25);
            txtManualExperimentName.PlaceholderText = "Example: Experiment 1 - Basic Circuit Testing";

            Label lblEquip = new Label();
            lblEquip.Text = "Equipment";
            lblEquip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEquip.Location = new Point(35, 152);
            lblEquip.AutoSize = true;

            cmbManualEquipment = new ComboBox();
            cmbManualEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualEquipment.Font = new Font("Segoe UI", 9.5F);
            cmbManualEquipment.Location = new Point(35, 178);
            cmbManualEquipment.Size = new Size(300, 25);

            Label lblQty = new Label();
            lblQty.Text = "Qty";
            lblQty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblQty.Location = new Point(360, 152);
            lblQty.AutoSize = true;

            numManualQty = new NumericUpDown();
            numManualQty.Font = new Font("Segoe UI", 9.5F);
            numManualQty.Location = new Point(360, 178);
            numManualQty.Size = new Size(80, 25);
            numManualQty.Minimum = 1;
            numManualQty.Maximum = 999;
            numManualQty.Value = 1;

            btnManualAddEquipment = new Button();
            btnManualAddEquipment.Text = "+ Add Equipment";
            btnManualAddEquipment.Location = new Point(460, 174);
            btnManualAddEquipment.Size = new Size(150, 32);
            btnManualAddEquipment.BackColor = Color.FromArgb(212, 168, 45);
            btnManualAddEquipment.ForeColor = Color.White;
            btnManualAddEquipment.FlatStyle = FlatStyle.Flat;
            btnManualAddEquipment.FlatAppearance.BorderSize = 0;
            btnManualAddEquipment.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnManualAddEquipment.Click += btnManualAddEquipment_Click;

            flowManualItems = new FlowLayoutPanel();
            flowManualItems.Location = new Point(35, 225);
            flowManualItems.Size = new Size(600, 250);
            flowManualItems.AutoScroll = true;
            flowManualItems.FlowDirection = FlowDirection.TopDown;
            flowManualItems.WrapContents = false;
            flowManualItems.BackColor = Color.FromArgb(245, 240, 247);
            flowManualItems.BorderStyle = BorderStyle.FixedSingle;

            btnManualSave = new Button();
            btnManualSave.Text = "Save Manual";
            btnManualSave.Location = new Point(485, 495);
            btnManualSave.Size = new Size(150, 38);
            btnManualSave.BackColor = Color.FromArgb(169, 215, 159);
            btnManualSave.ForeColor = Color.White;
            btnManualSave.FlatStyle = FlatStyle.Flat;
            btnManualSave.FlatAppearance.BorderSize = 0;
            btnManualSave.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnManualSave.Click += btnManualSave_Click;

            Label lblListTitle = new Label();
            lblListTitle.Text = "SAVED MANUALS";
            lblListTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblListTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblListTitle.Location = new Point(690, 26);
            lblListTitle.AutoSize = true;

            flowManualList = new FlowLayoutPanel();
            flowManualList.Location = new Point(690, 70);
            flowManualList.Size = new Size(310, 465);
            flowManualList.AutoScroll = true;
            flowManualList.FlowDirection = FlowDirection.TopDown;
            flowManualList.WrapContents = false;
            flowManualList.BackColor = Color.Transparent;

            pnlExperimentManualAdmin.Controls.Add(lblTitle);
            pnlExperimentManualAdmin.Controls.Add(lblSubject);
            pnlExperimentManualAdmin.Controls.Add(cmbManualSubject);
            pnlExperimentManualAdmin.Controls.Add(lblExpName);
            pnlExperimentManualAdmin.Controls.Add(txtManualExperimentName);
            pnlExperimentManualAdmin.Controls.Add(lblEquip);
            pnlExperimentManualAdmin.Controls.Add(cmbManualEquipment);
            pnlExperimentManualAdmin.Controls.Add(lblQty);
            pnlExperimentManualAdmin.Controls.Add(numManualQty);
            pnlExperimentManualAdmin.Controls.Add(btnManualAddEquipment);
            pnlExperimentManualAdmin.Controls.Add(flowManualItems);
            pnlExperimentManualAdmin.Controls.Add(btnManualSave);
            pnlExperimentManualAdmin.Controls.Add(lblListTitle);
            pnlExperimentManualAdmin.Controls.Add(flowManualList);

            Controls.Add(pnlExperimentManualAdmin);

            ApplyButtonStyle(btnManualAddEquipment);
            ApplyButtonStyle(btnManualSave);
            RoundControl(btnManualAddEquipment, 16);
            RoundControl(btnManualSave, 16);
        }



        private void LoadExperimentManualList()
        {
            try
            {
                flowManualList.Controls.Clear();

                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT 
    EM.ExperimentID,
    EM.ExperimentName,
    LS.SubjectCode
FROM ExperimentManuals AS EM
INNER JOIN LabSubjects AS LS ON EM.SubjectID = LS.SubjectID
WHERE EM.IsActive = True
AND LS.LabID = ?
ORDER BY LS.SubjectCode, EM.ExperimentName";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    Panel card = new Panel();
                    card.Size = new Size(270, 74);
                    card.BackColor = Color.FromArgb(241, 233, 245);
                    card.Margin = new Padding(0, 0, 0, 12);

                    Label lblName = new Label();
                    lblName.Text = reader["ExperimentName"].ToString();
                    lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    lblName.ForeColor = Color.FromArgb(69, 45, 96);
                    lblName.Location = new Point(12, 10);
                    lblName.Size = new Size(240, 24);

                    Label lblSubject = new Label();
                    lblSubject.Text = "Subject: " + reader["SubjectCode"].ToString();
                    lblSubject.Font = new Font("Segoe UI", 9F);
                    lblSubject.ForeColor = Color.FromArgb(126, 105, 136);
                    lblSubject.Location = new Point(12, 38);
                    lblSubject.Size = new Size(240, 22);

                    card.Controls.Add(lblName);
                    card.Controls.Add(lblSubject);

                    flowManualList.Controls.Add(card);
                    RoundControl(card, 16);
                }

                if (flowManualList.Controls.Count == 0)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No experiment manuals yet.";
                    lblEmpty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    lblEmpty.ForeColor = Color.FromArgb(126, 105, 136);
                    lblEmpty.AutoSize = false;
                    lblEmpty.Size = new Size(270, 40);
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;

                    flowManualList.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading experiment manuals:\n" + ex.Message);
            }
        }



        private class ScheduleFilterItem
        {
            public int ScheduleID { get; set; }
            public string DisplayText { get; set; } = "";

            public override string ToString()
            {
                return DisplayText;
            }
        }




        private void btnReportDamageDynamic_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Report Sent",
                "Report",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
SELECT SUM(BSI.QuantityRequested)
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



        private string GetBorrowedSerialNumbersForAdmin(int slipItemId)
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
                    string serial = reader["SerialNumber"]?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(serial))
                        serials.Add(serial);
                }

                return serials.Count > 0
                    ? string.Join(", ", serials)
                    : "N/A";
            }
            catch
            {
                return "N/A";
            }
        }


        private string GetReportSerialNumbers(int reportId)
        {
            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT EU.SerialNumber
FROM (DamageReportUnits AS DRU
INNER JOIN EquipmentUnits AS EU
ON DRU.UnitID = EU.UnitID)
WHERE DRU.ReportID = ?
ORDER BY EU.SerialNumber";

                using OleDbCommand cmd =
                    new OleDbCommand(query, conn);

                cmd.Parameters.AddWithValue("@p1", reportId);

                List<string> serials =
                    new List<string>();

                using OleDbDataReader reader =
                    cmd.ExecuteReader();

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



        private bool AskLimitedUseStillUsable(string equipmentName)
        {
            Form dialog = new Form();

            dialog.Text = "Limited Use Equipment";
            dialog.Size = new Size(420, 230);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text =
                equipmentName +
                "\n\nIs the item still usable?";
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(72, 53, 84);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(340, 60);
            lblTitle.Location = new Point(35, 25);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            bool result = false;

            Button btnStillUsable = new Button();
            btnStillUsable.Text = "Still Usable";
            btnStillUsable.Size = new Size(140, 42);
            btnStillUsable.Location = new Point(40, 120);
            btnStillUsable.BackColor = Color.FromArgb(120, 190, 120);
            btnStillUsable.ForeColor = Color.White;
            btnStillUsable.FlatStyle = FlatStyle.Flat;
            btnStillUsable.FlatAppearance.BorderSize = 0;

            btnStillUsable.Click += (s, e) =>
            {
                result = true;
                dialog.Close();
            };

            Button btnDisposed = new Button();
            btnDisposed.Text = "Consumed / Dispose";
            btnDisposed.Size = new Size(170, 42);
            btnDisposed.Location = new Point(200, 120);
            btnDisposed.BackColor = Color.Firebrick;
            btnDisposed.ForeColor = Color.White;
            btnDisposed.FlatStyle = FlatStyle.Flat;
            btnDisposed.FlatAppearance.BorderSize = 0;

            btnDisposed.Click += (s, e) =>
            {
                result = false;
                dialog.Close();
            };

            dialog.Controls.Add(lblTitle);
            dialog.Controls.Add(btnStillUsable);
            dialog.Controls.Add(btnDisposed);

            RoundControl(btnStillUsable, 14);
            RoundControl(btnDisposed, 14);

            dialog.ShowDialog();

            return result;
        }



    }


    
}
