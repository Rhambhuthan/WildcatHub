using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace WildcatHub
{
    public partial class frmEquipmentDetails : Form
    {
        private int equipmentId;
        private bool isEditing = false;

        private ComboBox cmbEquipmentType = null!;
        private CheckBox chkHasSerial = null!;
        private TextBox txtDescription = null!;
        private Button btnViewUnits = null!;
        private Button btnChangeImage = null!;
        private PictureBox picEquipment = null!;
        private ComboBox cmbAddSubject = null!;
        private CheckedListBox clbSubjects = null!;
        private LinkLabel linkDeleteSubjects = null!;
        private readonly Dictionary<int, string> linkedSubjects = new Dictionary<int, string>();
        private int storedQuantityTotal;
        private string selectedImagePath = "";

        public frmEquipmentDetails(int id)
        {
            InitializeComponent();
            equipmentId = id;
        }

        private void frmEquipmentDetails_Load(object sender, EventArgs e)
        {
            LoadCategoryDropdown();
            cmbCategory.MouseUp += cmbCategory_MouseUp;

            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[]
            {
                "Active",
                "Maintenance"
            });

            AddNewDetailsControls();
            numTotal.Maximum = 100000;
            numMaintenance.Maximum = 100000;
            numThreshold.Maximum = 100000;

            LoadData();
            SetEditMode(false);
            this.ActiveControl = lblTitle;
            txtName.SelectionStart = 0;
            txtName.SelectionLength = 0;
            txtDescription.SelectionStart = 0;
            txtDescription.SelectionLength = 0;
        }

        private void AddNewDetailsControls()
        {
            Label lblType = new Label();
            lblType.Text = "Equipment Type";
            lblType.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblType.Location = new Point(250, 355);
            lblType.AutoSize = true;

            cmbEquipmentType = new ComboBox();
            cmbEquipmentType.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbEquipmentType.Font =
                new Font("Segoe UI", 10F);

            cmbEquipmentType.Location =
                new Point(250, 378);

            cmbEquipmentType.Size =
                new Size(185, 30);

            cmbEquipmentType.Items.AddRange(new string[]
            {
                "Reusable",
                "Limited Use",
                "One Time Use"
            });
            cmbEquipmentType.SelectedIndexChanged += (s, e) => ApplySerialAvailability();

            chkHasSerial = new CheckBox();
            chkHasSerial.Text = "Has Serial Numbers";

            chkHasSerial.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            chkHasSerial.Location =
                new Point(40, 418);

            chkHasSerial.AutoSize = true;

            btnViewUnits = new Button();

            btnViewUnits.Text = "+";

            btnViewUnits.Font =
                new Font("Segoe UI", 12F, FontStyle.Bold);

            btnViewUnits.Size =
                new Size(42, 34);

            btnViewUnits.Location =
                new Point(205, 412);

            btnViewUnits.BackColor =
                Color.FromArgb(128, 0, 0);

            btnViewUnits.ForeColor = Color.White;

            btnViewUnits.FlatStyle = FlatStyle.Flat;

            btnViewUnits.FlatAppearance.BorderSize = 0;

            btnViewUnits.Visible = false;

            chkHasSerial.CheckedChanged += (s, e) =>
            {
                btnViewUnits.Visible =
                    chkHasSerial.Checked;
            };

            btnViewUnits.Click += (s, e) =>
            {
                ShowEquipmentUnitsPopup();
            };

            Label lblDesc = new Label();

            lblDesc.Text = "Description / Remarks";

            lblDesc.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            lblDesc.Location =
                new Point(40, 455);

            lblDesc.AutoSize = true;

            txtDescription = new TextBox();

            txtDescription.Font =
                new Font("Segoe UI", 10F);

            txtDescription.Location =
                new Point(40, 482);

            txtDescription.Size =
                new Size(470, 72);

            txtDescription.Multiline = true;

            txtDescription.ScrollBars =
                ScrollBars.Vertical;

            Label lblAddSubject = new Label();
            lblAddSubject.Text = "Add Subject";
            lblAddSubject.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAddSubject.Location = new Point(550, 285);
            lblAddSubject.AutoSize = true;

            cmbAddSubject = new ComboBox();
            cmbAddSubject.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAddSubject.Font = new Font("Segoe UI", 9F);
            cmbAddSubject.Location = new Point(550, 308);
            cmbAddSubject.Size = new Size(165, 25);
            cmbAddSubject.SelectedIndexChanged += cmbAddSubject_SelectedIndexChanged;

            Label lblSubjects = new Label();
            lblSubjects.Text = "Subjects";
            lblSubjects.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSubjects.Location = new Point(550, 345);
            lblSubjects.AutoSize = true;

            linkDeleteSubjects = new LinkLabel();
            linkDeleteSubjects.Text = "Delete";
            linkDeleteSubjects.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            linkDeleteSubjects.LinkColor = Color.FromArgb(153, 0, 0);
            linkDeleteSubjects.ActiveLinkColor = Color.FromArgb(210, 160, 30);
            linkDeleteSubjects.VisitedLinkColor = Color.FromArgb(153, 0, 0);
            linkDeleteSubjects.Location = new Point(670, 346);
            linkDeleteSubjects.AutoSize = true;
            linkDeleteSubjects.Click += linkDeleteSubjects_Click;

            clbSubjects = new CheckedListBox();
            clbSubjects.Font = new Font("Segoe UI", 9.5F);
            clbSubjects.Location = new Point(550, 372);
            clbSubjects.Size = new Size(165, 116);
            clbSubjects.CheckOnClick = true;
            clbSubjects.BorderStyle = BorderStyle.FixedSingle;

            picEquipment = new PictureBox();
            picEquipment.Location = new Point(550, 108);
            picEquipment.Size = new Size(165, 140);
            picEquipment.BackColor = Color.FromArgb(250, 246, 248);
            picEquipment.BorderStyle = BorderStyle.FixedSingle;
            picEquipment.SizeMode = PictureBoxSizeMode.Zoom;

            btnChangeImage = new Button();
            btnChangeImage.Text = "Change Image";
            btnChangeImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnChangeImage.Location = new Point(550, 255);
            btnChangeImage.Size = new Size(165, 26);
            btnChangeImage.BackColor = Color.FromArgb(212, 168, 45);
            btnChangeImage.ForeColor = Color.White;
            btnChangeImage.FlatStyle = FlatStyle.Flat;
            btnChangeImage.FlatAppearance.BorderSize = 0;
            btnChangeImage.Visible = false;
            btnChangeImage.Click += btnChangeImage_Click;

            Controls.Add(lblType);
            Controls.Add(cmbEquipmentType);
            Controls.Add(chkHasSerial);
            Controls.Add(btnViewUnits);
            Controls.Add(lblDesc);
            Controls.Add(txtDescription);
            Controls.Add(lblAddSubject);
            Controls.Add(cmbAddSubject);
            Controls.Add(lblSubjects);
            Controls.Add(linkDeleteSubjects);
            Controls.Add(clbSubjects);
            Controls.Add(picEquipment);
            Controls.Add(btnChangeImage);
        }

        private void LoadCategoryDropdown()
        {
            string current = cmbCategory.Text;
            cmbCategory.Items.Clear();

            foreach (string category in EquipmentCategoryService.GetCategories())
                cmbCategory.Items.Add(category);

            if (!string.IsNullOrWhiteSpace(current) && cmbCategory.Items.Contains(current))
                cmbCategory.SelectedItem = current;
            else if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void cmbCategory_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            using EquipmentCategoryManagerForm form =
                new EquipmentCategoryManagerForm(EquipmentCategoryService.GetCategories());

            if (form.ShowDialog(this) == DialogResult.OK)
                LoadCategoryDropdown();
        }



        private void LoadData()
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = "SELECT * FROM Equipment WHERE EquipmentID = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            bool hasSerial = false;
            int maintenance = 0;
            string equipmentType = "Reusable";

            using OleDbDataReader r = cmd.ExecuteReader();

            if (r.Read())
            {
                txtName.Text = r["EquipmentName"]?.ToString() ?? "";
                cmbCategory.Text = r["Category"]?.ToString() ?? "";
                txtBrand.Text = r["Brand"]?.ToString() ?? "";

                storedQuantityTotal = r["QuantityTotal"] != DBNull.Value
                    ? Convert.ToInt32(r["QuantityTotal"])
                    : 0;

                maintenance = r["QuantityMaintenance"] != DBNull.Value
                    ? Convert.ToInt32(r["QuantityMaintenance"])
                    : 0;

                numMaintenance.Value = maintenance;

                numThreshold.Value = r["LowStockThreshold"] != DBNull.Value
                    ? Convert.ToDecimal(r["LowStockThreshold"])
                    : 1;

                cmbStatus.Text = r["Status"]?.ToString() ?? "Active";

                equipmentType = r["EquipmentType"] != DBNull.Value
                    ? r["EquipmentType"].ToString() ?? "Reusable"
                    : "Reusable";

                cmbEquipmentType.Text = equipmentType == "Consumable"
                    ? "One Time Use"
                    : equipmentType;

                hasSerial = r["HasSerial"] != DBNull.Value &&
                            Convert.ToBoolean(r["HasSerial"]);

                txtDescription.Text = r["Description"] != DBNull.Value
                    ? r["Description"].ToString()
                    : "";

                selectedImagePath = r["ImagePath"]?.ToString() ?? "";
                LoadEquipmentImage(selectedImagePath);

                btnViewUnits.Visible = chkHasSerial.Checked;

                bool isArchived = r["IsArchived"] != DBNull.Value &&
                                  Convert.ToBoolean(r["IsArchived"]);
                linkDelete.Text = isArchived ? "Unarchive" : "Archive";
            }

            r.Close();

            bool canUseSerials = equipmentType != "Consumable" && equipmentType != "One Time Use";
            hasSerial = canUseSerials && (hasSerial || GetExistingUnitCount(conn) > 0);
            chkHasSerial.Checked = hasSerial;
            chkHasSerial.Enabled = canUseSerials;
            btnViewUnits.Visible = hasSerial;

            int currentAvailable = GetCurrentAvailableQuantity(
                conn,
                storedQuantityTotal,
                maintenance,
                hasSerial,
                equipmentType);

            numTotal.Value = Math.Max(numTotal.Minimum, Math.Min(numTotal.Maximum, currentAvailable));

            LoadSubjectOptions(conn);
        }

        private void LoadSubjectOptions(OleDbConnection conn)
        {
            linkedSubjects.Clear();
            clbSubjects.Items.Clear();
            cmbAddSubject.Items.Clear();

            string linkedQuery = @"
SELECT LS.SubjectID, LS.SubjectCode
FROM SubjectEquipments AS SE
INNER JOIN LabSubjects AS LS ON SE.SubjectID = LS.SubjectID
WHERE SE.EquipmentID = ?
ORDER BY LS.SubjectCode";

            using (OleDbCommand cmd = new OleDbCommand(linkedQuery, conn))
            {
                cmd.Parameters.AddWithValue("@p1", equipmentId);

                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    int subjectId = Convert.ToInt32(reader["SubjectID"]);
                    string code = reader["SubjectCode"]?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(code))
                        continue;

                    linkedSubjects[subjectId] = code;
                    clbSubjects.Items.Add(new SubjectOption(subjectId, code), false);
                }
            }

            string availableQuery = @"
SELECT LS.SubjectID, LS.SubjectCode
FROM LabSubjects AS LS
WHERE LS.LabID = (SELECT E.LabID FROM Equipment AS E WHERE E.EquipmentID = ?)
  AND LS.IsActive = True
  AND LS.SubjectID NOT IN
  (
      SELECT SE.SubjectID
      FROM SubjectEquipments AS SE
      WHERE SE.EquipmentID = ?
  )
ORDER BY LS.SubjectCode";

            using (OleDbCommand cmd = new OleDbCommand(availableQuery, conn))
            {
                cmd.Parameters.AddWithValue("@p1", equipmentId);
                cmd.Parameters.AddWithValue("@p2", equipmentId);

                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    int subjectId = Convert.ToInt32(reader["SubjectID"]);
                    string code = reader["SubjectCode"]?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(code))
                        cmbAddSubject.Items.Add(new SubjectOption(subjectId, code));
                }
            }

            cmbAddSubject.SelectedIndex = -1;
        }

        private void btnChangeImage_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select equipment image";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (ofd.ShowDialog(this) != DialogResult.OK)
                return;

            selectedImagePath = ofd.FileName;
            LoadEquipmentImage(selectedImagePath);
        }

        private void LoadEquipmentImage(string imagePath)
        {
            if (picEquipment.Image != null)
            {
                Image oldImage = picEquipment.Image;
                picEquipment.Image = null;
                oldImage.Dispose();
            }

            if (string.IsNullOrWhiteSpace(imagePath) || !System.IO.File.Exists(imagePath))
                return;

            using System.IO.FileStream fs = new System.IO.FileStream(
                imagePath,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            using Image temp = Image.FromStream(fs);
            picEquipment.Image = new Bitmap(temp);
        }

        private void SetEditMode(bool enabled)
        {
            isEditing = enabled;

            txtName.ReadOnly = !enabled;
            txtBrand.ReadOnly = !enabled;
            txtDescription.ReadOnly = !enabled;

            cmbCategory.Enabled = enabled;
            cmbStatus.Enabled = enabled;
            cmbEquipmentType.Enabled = enabled;
            cmbAddSubject.Enabled = enabled;
            clbSubjects.Enabled = enabled;
            linkDeleteSubjects.Enabled = enabled;
            chkHasSerial.Enabled = false;
            btnChangeImage.Visible = enabled;
            btnChangeImage.Enabled = enabled;

            numTotal.Enabled = enabled;
            numMaintenance.Enabled = enabled;
            numThreshold.Enabled = enabled;

            linkEditSave.Text = enabled ? "Save" : "Edit";
            ApplySerialAvailability();
        }

        private void ApplySerialAvailability()
        {
            if (chkHasSerial == null || btnViewUnits == null || cmbEquipmentType == null)
                return;

            bool canUseSerials = cmbEquipmentType.Text != "Consumable" &&
                                 cmbEquipmentType.Text != "One Time Use";

            if (!canUseSerials)
                chkHasSerial.Checked = false;

            chkHasSerial.Enabled = false;
            btnViewUnits.Visible = canUseSerials && chkHasSerial.Checked;
        }

        private void cmbAddSubject_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!isEditing || cmbAddSubject.SelectedItem is not SubjectOption subject)
                return;

            linkedSubjects[subject.SubjectID] = subject.DisplayText;
            clbSubjects.Items.Add(subject, false);

            cmbAddSubject.Items.Remove(subject);
            cmbAddSubject.SelectedIndex = -1;
        }

        private void linkDeleteSubjects_Click(object? sender, EventArgs e)
        {
            if (!isEditing)
                return;

            if (clbSubjects.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please check the subject(s) you want to delete.",
                    "Delete Subject",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<SubjectOption> subjectsToDelete = new List<SubjectOption>();

            foreach (object item in clbSubjects.CheckedItems)
            {
                if (item is SubjectOption subject)
                    subjectsToDelete.Add(subject);
            }

            foreach (SubjectOption subject in subjectsToDelete)
            {
                linkedSubjects.Remove(subject.SubjectID);
                clbSubjects.Items.Remove(subject);
                cmbAddSubject.Items.Add(subject);
            }

            cmbAddSubject.Sorted = true;
        }

        private void linkEditSave_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                SetEditMode(true);
                return;
            }

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            int oldTotal = GetExistingTotal(conn);
            int desiredAvailable = (int)numTotal.Value;
            bool canUseSerials = cmbEquipmentType.Text != "Consumable" &&
                                 cmbEquipmentType.Text != "One Time Use";
            bool isSerialEquipment = canUseSerials &&
                                     (chkHasSerial.Checked || GetExistingUnitCount(conn) > 0);
            int savedTotal = GetStoredTotalForDesiredAvailable(
                conn,
                desiredAvailable,
                (int)numMaintenance.Value,
                isSerialEquipment);

            List<string> serialsToAdd = new List<string>();
            List<int> unitIdsToDelete = new List<int>();

            if (isSerialEquipment)
            {
                int quantityDelta = desiredAvailable - GetAvailableSerialUnitCount(conn);

                if (quantityDelta > 0)
                {
                    serialsToAdd = PromptForAdditionalSerials(quantityDelta);
                    if (serialsToAdd.Count != quantityDelta)
                    {
                        MessageBox.Show("Failed to provide serial number, changes not saved",
                            "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (quantityDelta < 0)
                {
                    int countToDelete = Math.Abs(quantityDelta);
                    unitIdsToDelete = PromptForSerialUnitsToDelete(countToDelete);
                    if (unitIdsToDelete.Count != countToDelete)
                    {
                        MessageBox.Show("Failed to provide serial number, changes not saved",
                            "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            string query = @"
UPDATE Equipment SET 
    EquipmentName = ?,
    Category = ?,
    Brand = ?,
    QuantityTotal = ?,
    QuantityMaintenance = ?,
    LowStockThreshold = ?,
    Status = ?,
    EquipmentType = ?,
    HasSerial = ?,
    Description = ?,
    ImagePath = ?
WHERE EquipmentID = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn);

            cmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = txtName.Text.Trim();
            cmd.Parameters.Add("@p2", OleDbType.VarWChar).Value = cmbCategory.Text;
            cmd.Parameters.Add("@p3", OleDbType.VarWChar).Value = txtBrand.Text.Trim();
            cmd.Parameters.Add("@p4", OleDbType.Integer).Value = savedTotal;
            cmd.Parameters.Add("@p5", OleDbType.Integer).Value = (int)numMaintenance.Value;
            cmd.Parameters.Add("@p6", OleDbType.Integer).Value = (int)numThreshold.Value;
            cmd.Parameters.Add("@p7", OleDbType.VarWChar).Value = cmbStatus.Text;
            cmd.Parameters.Add("@p8", OleDbType.VarWChar).Value = cmbEquipmentType.Text;
            cmd.Parameters.Add("@p9", OleDbType.Boolean).Value = isSerialEquipment;
            cmd.Parameters.Add("@p10", OleDbType.LongVarWChar).Value = txtDescription.Text.Trim();
            cmd.Parameters.Add("@p11", OleDbType.VarWChar).Value = selectedImagePath;
            cmd.Parameters.Add("@p12", OleDbType.Integer).Value = equipmentId;

            cmd.ExecuteNonQuery();

            if (isSerialEquipment)
                SyncSerialUnitsAfterQuantityChange(conn, serialsToAdd, unitIdsToDelete);

            SaveSubjectLinks(conn);

            MessageBox.Show("Updated successfully.");

            DialogResult = DialogResult.OK;
            Close();
        }

        private int GetExistingTotal(OleDbConnection conn)
        {
            using OleDbCommand cmd = new OleDbCommand(
                "SELECT QuantityTotal FROM Equipment WHERE EquipmentID = ?",
                conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        private int GetCurrentAvailableQuantity(
            OleDbConnection conn,
            int total,
            int maintenance,
            bool hasSerial,
            string equipmentType)
        {
            if (hasSerial)
                return GetAvailableSerialUnitCount(conn);

            int unavailable = maintenance
                + GetEquipmentBorrowedQuantity(conn)
                + GetEquipmentUsedUpQuantity(conn)
                + GetEquipmentReportedQuantity(conn);

            int available = total - unavailable;
            return available < 0 ? 0 : available;
        }

        private int GetStoredTotalForDesiredAvailable(
            OleDbConnection conn,
            int desiredAvailable,
            int maintenance,
            bool hasSerial)
        {
            if (hasSerial)
            {
                int unavailableUnits = GetExistingUnitCount(conn) - GetAvailableSerialUnitCount(conn);
                return Math.Max(0, desiredAvailable + unavailableUnits);
            }

            int unavailable = maintenance
                + GetEquipmentBorrowedQuantity(conn)
                + GetEquipmentUsedUpQuantity(conn)
                + GetEquipmentReportedQuantity(conn);

            return Math.Max(0, desiredAvailable + unavailable);
        }

        private int GetAvailableSerialUnitCount(OleDbConnection conn)
        {
            using OleDbCommand cmd = new OleDbCommand(
                "SELECT COUNT(*) FROM EquipmentUnits WHERE EquipmentID = ? AND UnitStatus = 'Available'",
                conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        private int GetEquipmentBorrowedQuantity(OleDbConnection conn)
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

            return ExecuteQuantitySum(conn, query);
        }

        private int GetEquipmentUsedUpQuantity(OleDbConnection conn)
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

            return ExecuteQuantitySum(conn, query);
        }

        private int GetEquipmentReportedQuantity(OleDbConnection conn)
        {
            string query = @"
SELECT SUM(DamageQuantity)
FROM DamageReports
WHERE EquipmentID = ?";

            return ExecuteQuantitySum(conn, query);
        }

        private int ExecuteQuantitySum(OleDbConnection conn, string query)
        {
            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        private void SyncSerialUnitsAfterQuantityChange(
            OleDbConnection conn,
            List<string> serialsToAdd,
            List<int> unitIdsToDelete)
        {
            foreach (string serial in serialsToAdd)
            {
                string insertQuery = @"
INSERT INTO EquipmentUnits
(
    EquipmentID,
    SerialNumber,
    UnitStatus,
    DateAdded
)
VALUES (?, ?, ?, ?)";

                using OleDbCommand cmd = new OleDbCommand(insertQuery, conn);
                cmd.Parameters.Add("@p1", OleDbType.Integer).Value = equipmentId;
                cmd.Parameters.Add("@p2", OleDbType.VarWChar).Value = serial;
                cmd.Parameters.Add("@p3", OleDbType.VarWChar).Value = "Available";
                cmd.Parameters.Add("@p4", OleDbType.Date).Value = DateTime.Now;
                cmd.ExecuteNonQuery();
            }

            foreach (int unitId in unitIdsToDelete)
            {
                using OleDbCommand cmd = new OleDbCommand(
                    "DELETE FROM EquipmentUnits WHERE UnitID = ? AND EquipmentID = ? AND UnitStatus = 'Available'",
                    conn);
                cmd.Parameters.AddWithValue("@p1", unitId);
                cmd.Parameters.AddWithValue("@p2", equipmentId);
                cmd.ExecuteNonQuery();
            }
        }

        private int GetExistingUnitCount(OleDbConnection conn)
        {
            using OleDbCommand cmd = new OleDbCommand(
                "SELECT COUNT(*) FROM EquipmentUnits WHERE EquipmentID = ?",
                conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        private List<string> PromptForAdditionalSerials(int count)
        {
            using Form popup = new Form();
            popup.Text = "Additional Serial Numbers";
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.Size = new Size(430, 420);
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;
            popup.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label
            {
                Text = "Add " + count + " serial number(s)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(24, 20),
                AutoSize = true
            };

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Location = new Point(24, 64),
                Size = new Size(360, 250),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White
            };

            for (int i = 1; i <= count; i++)
            {
                TextBox txt = new TextBox
                {
                    Width = 320,
                    Font = new Font("Segoe UI", 10F),
                    PlaceholderText = "New serial #" + i,
                    Margin = new Padding(8)
                };
                flow.Controls.Add(txt);
            }

            List<string> serials = new List<string>();

            Button btnSave = new Button
            {
                Text = "Save",
                Size = new Size(110, 36),
                Location = new Point(274, 330),
                BackColor = Color.FromArgb(153, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                foreach (Control ctrl in flow.Controls)
                {
                    if (ctrl is TextBox txt)
                    {
                        if (string.IsNullOrWhiteSpace(txt.Text))
                        {
                            MessageBox.Show("Please complete all serial numbers.");
                            return;
                        }

                        serials.Add(txt.Text.Trim());
                    }
                }

                popup.DialogResult = DialogResult.OK;
                popup.Close();
            };

            popup.Controls.Add(lblTitle);
            popup.Controls.Add(flow);
            popup.Controls.Add(btnSave);

            return popup.ShowDialog(this) == DialogResult.OK
                ? serials
                : new List<string>();
        }

        private List<int> PromptForSerialUnitsToDelete(int count)
        {
            using Form popup = new Form();
            popup.Text = "Serial Numbers";
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.Size = new Size(520, 480);
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;
            popup.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label
            {
                Text = "Select " + count + " serial number(s) to delete",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 45, 96),
                Location = new Point(24, 20),
                AutoSize = true
            };

            CheckedListBox list = new CheckedListBox
            {
                Location = new Point(24, 64),
                Size = new Size(450, 310),
                Font = new Font("Segoe UI", 10F),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            using (OleDbConnection conn = DbHelper.GetConnection())
            using (OleDbCommand cmd = new OleDbCommand(
                "SELECT UnitID, SerialNumber FROM EquipmentUnits WHERE EquipmentID = ? AND UnitStatus = 'Available' ORDER BY SerialNumber",
                conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@p1", equipmentId);
                using OleDbDataReader reader = cmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    list.Items.Add(new SerialUnitChoice(
                        Convert.ToInt32(reader["UnitID"]),
                        reader["SerialNumber"]?.ToString() ?? ""));
                }
            }

            List<int> selectedIds = new List<int>();

            Button btnSave = new Button
            {
                Text = "Save",
                Size = new Size(110, 36),
                Location = new Point(364, 392),
                BackColor = Color.FromArgb(153, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                if (list.CheckedItems.Count != count)
                {
                    MessageBox.Show("Please check exactly " + count + " serial number(s).",
                        "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedIds = list.CheckedItems
                    .OfType<SerialUnitChoice>()
                    .Select(x => x.UnitID)
                    .ToList();

                popup.DialogResult = DialogResult.OK;
                popup.Close();
            };

            popup.Controls.Add(lblTitle);
            popup.Controls.Add(list);
            popup.Controls.Add(btnSave);

            return popup.ShowDialog(this) == DialogResult.OK
                ? selectedIds
                : new List<int>();
        }

        private void ShowEquipmentUnitsPopup()
        {
            Form popup = new Form();
            popup.Text = "Serial Numbers";
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.Size = new Size(520, 480);
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;

            DataGridView dgvUnits = new DataGridView();
            dgvUnits.Location = new Point(20, 60);
            dgvUnits.Size = new Size(460, 320);
            dgvUnits.ReadOnly = true;
            dgvUnits.AllowUserToAddRows = false;
            dgvUnits.AllowUserToDeleteRows = false;
            dgvUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvUnits.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvUnits.MultiSelect = false;
            dgvUnits.RowHeadersVisible = false;
            dgvUnits.DefaultCellStyle.SelectionBackColor = dgvUnits.DefaultCellStyle.BackColor;
            dgvUnits.DefaultCellStyle.SelectionForeColor = dgvUnits.DefaultCellStyle.ForeColor;

            Label lblTitle = new Label();
            lblTitle.Text = "Serial Numbers";
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(20, 18);
            lblTitle.AutoSize = true;

            Button btnEditSave = new Button();
            btnEditSave.Text = "Edit";
            btnEditSave.Size = new Size(110, 34);
            btnEditSave.Location = new Point(370, 395);
            btnEditSave.BackColor = Color.FromArgb(214, 197, 224);
            btnEditSave.ForeColor = Color.FromArgb(87, 60, 99);
            btnEditSave.FlatStyle = FlatStyle.Flat;
            btnEditSave.FlatAppearance.BorderSize = 0;

            DataTable dt = new DataTable();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
SELECT
    UnitID,
    SerialNumber,
    UnitStatus,
    DateAdded
FROM EquipmentUnits
WHERE EquipmentID = ?
ORDER BY SerialNumber";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            using OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);

            if (!dt.Columns.Contains("DisplayStatus"))
                dt.Columns.Add("DisplayStatus", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string rawStatus = row["UnitStatus"]?.ToString() ?? "";
                row["DisplayStatus"] =
                    rawStatus == "Broken" || rawStatus == "Lost" || rawStatus == "Consumed"
                        ? rawStatus
                        : "";
            }

            dgvUnits.AutoGenerateColumns = false;
            dgvUnits.Columns.Clear();

            DataGridViewTextBoxColumn serialColumn = new DataGridViewTextBoxColumn();
            serialColumn.Name = "SerialNumber";
            serialColumn.DataPropertyName = "SerialNumber";
            serialColumn.HeaderText = "Serial Number";
            serialColumn.ReadOnly = true;
            serialColumn.Width = 250;

            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "UnitStatus";
            statusColumn.DataPropertyName = "DisplayStatus";
            statusColumn.HeaderText = "Status";
            statusColumn.ReadOnly = true;
            statusColumn.Width = 82;

            DataGridViewTextBoxColumn dateAddedColumn = new DataGridViewTextBoxColumn();
            dateAddedColumn.Name = "DateAdded";
            dateAddedColumn.DataPropertyName = "DateAdded";
            dateAddedColumn.HeaderText = "Date Added";
            dateAddedColumn.ReadOnly = true;
            dateAddedColumn.Width = 126;

            dgvUnits.Columns.Add(serialColumn);
            dgvUnits.Columns.Add(statusColumn);
            dgvUnits.Columns.Add(dateAddedColumn);
            dgvUnits.DataSource = dt;

            foreach (DataGridViewRow row in dgvUnits.Rows)
            {
                string status = row.Cells[statusColumn.Index].Value?.ToString() ?? "";
                Color rowBackColor = Color.White;

                if (status == "Broken" || status == "Lost" || status == "Consumed")
                {
                    rowBackColor = Color.FromArgb(255, 220, 220);
                }

                row.DefaultCellStyle.BackColor = rowBackColor;
                row.DefaultCellStyle.SelectionBackColor = rowBackColor;
                row.DefaultCellStyle.SelectionForeColor = row.DefaultCellStyle.ForeColor;
            }

            btnEditSave.Click += (s, e) =>
            {
                if (btnEditSave.Text == "Edit")
                {
                    dgvUnits.ReadOnly = false;
                    serialColumn.ReadOnly = false;
                    statusColumn.ReadOnly = true;
                    dateAddedColumn.ReadOnly = true;
                    btnEditSave.Text = "Save";
                    btnEditSave.BackColor = Color.FromArgb(153, 0, 0);
                    btnEditSave.ForeColor = Color.White;
                    return;
                }

                try
                {
                    using OleDbConnection updateConn = DbHelper.GetConnection();
                    updateConn.Open();

                    foreach (DataGridViewRow row in dgvUnits.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        string serial = row.Cells[serialColumn.Index].Value?.ToString()?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(serial))
                        {
                            MessageBox.Show("Serial number cannot be blank.",
                                "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (row.DataBoundItem is not DataRowView boundRow)
                            continue;

                        int unitId = Convert.ToInt32(boundRow["UnitID"]);
                        using OleDbCommand updateCmd = new OleDbCommand(
                            "UPDATE EquipmentUnits SET SerialNumber = ? WHERE UnitID = ? AND EquipmentID = ?",
                            updateConn);
                        updateCmd.Parameters.Add("@p1", OleDbType.VarWChar).Value = serial;
                        updateCmd.Parameters.Add("@p2", OleDbType.Integer).Value = unitId;
                        updateCmd.Parameters.Add("@p3", OleDbType.Integer).Value = equipmentId;
                        updateCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Serial numbers updated.",
                        "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    popup.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating serial numbers:\n" + ex.Message,
                        "Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            popup.Controls.Add(lblTitle);
            popup.Controls.Add(dgvUnits);
            popup.Controls.Add(btnEditSave);
            popup.Shown += (s, e) => dgvUnits.ClearSelection();

            popup.ShowDialog(this);
        }

        private void linkDelete_Click(object sender, EventArgs e)
        {
            bool isArchived = IsEquipmentArchived();
            string action = isArchived ? "Unarchive" : "Archive";

            if (MessageBox.Show(action + " this equipment?", "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = "UPDATE Equipment SET IsArchived = ? WHERE EquipmentID = ?";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", !isArchived);
            cmd.Parameters.AddWithValue("@p2", equipmentId);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Equipment " + (isArchived ? "unarchived" : "archived") + " successfully.");

            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveSubjectLinks(OleDbConnection conn)
        {
            HashSet<int> subjectIdsToRemove = new HashSet<int>();

            foreach (object item in clbSubjects.CheckedItems)
            {
                if (item is SubjectOption subject)
                    subjectIdsToRemove.Add(subject.SubjectID);
            }

            using (OleDbCommand deleteCmd = new OleDbCommand(
                "DELETE FROM SubjectEquipments WHERE EquipmentID = ?",
                conn))
            {
                deleteCmd.Parameters.AddWithValue("@p1", equipmentId);
                deleteCmd.ExecuteNonQuery();
            }

            foreach (KeyValuePair<int, string> subject in linkedSubjects)
            {
                if (subjectIdsToRemove.Contains(subject.Key))
                    continue;

                using OleDbCommand insertCmd = new OleDbCommand(
                    "INSERT INTO SubjectEquipments (EquipmentID, SubjectID) VALUES (?, ?)",
                    conn);
                insertCmd.Parameters.AddWithValue("@p1", equipmentId);
                insertCmd.Parameters.AddWithValue("@p2", subject.Key);
                insertCmd.ExecuteNonQuery();
            }
        }

        private bool IsEquipmentArchived()
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbCommand cmd = new OleDbCommand(
                "SELECT IsArchived FROM Equipment WHERE EquipmentID = ?",
                conn);
            cmd.Parameters.AddWithValue("@p1", equipmentId);

            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value && Convert.ToBoolean(result);
        }

        private void linkClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private sealed class SubjectOption
        {
            public SubjectOption(int subjectId, string displayText)
            {
                SubjectID = subjectId;
                DisplayText = displayText;
            }

            public int SubjectID { get; }
            public string DisplayText { get; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private sealed class SerialUnitChoice
        {
            public SerialUnitChoice(int unitId, string serialNumber)
            {
                UnitID = unitId;
                SerialNumber = serialNumber;
            }

            public int UnitID { get; }
            public string SerialNumber { get; }

            public override string ToString()
            {
                return SerialNumber;
            }
        }
    }
}
