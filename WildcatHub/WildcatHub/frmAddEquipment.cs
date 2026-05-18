using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WildcatHub
{
    public partial class frmAddEquipment : Form
    {
        private ComboBox cmbEquipmentType = null!;
        private RadioButton rbSerialYes = null!;
        private RadioButton rbSerialNo = null!;
        private TextBox txtDescription = null!;
        private FlowLayoutPanel flowSerialInputs = null!;
        private Label lblSerialTitle = null!;
        private PictureBox picEquipment = null!;
        private Button btnChooseImage = null!;
        private ComboBox cmbSubjectUsed = null!;
        private TextBox txtSubjectsChosen = null!;
        private Button btnOpenChosenSubjects = null!;

        private string selectedImagePath = "";
        private readonly List<SubjectCheckItem> availableSubjects = new List<SubjectCheckItem>();
        private readonly List<SubjectCheckItem> selectedSubjects = new List<SubjectCheckItem>();

        private class SubjectCheckItem
        {
            public int SubjectID { get; set; }
            public string DisplayText { get; set; } = "";

            public override string ToString()
            {
                return DisplayText;
            }
        }

        public frmAddEquipment()
        {
            InitializeComponent();
        }

        private void frmAddEquipment_Load(object sender, EventArgs e)
        {
            Text = "Add Equipment";
            Size = new Size(840, 620);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(250, 245, 247);

            LoadCategoryDropdown();
            cmbCategory.MouseUp += cmbCategory_MouseUp;

            numTotal.Minimum = 1;
            numMaintenance.Minimum = 0;
            numThreshold.Minimum = 1;

            AddNewEquipmentFields();
            LoadSubjectsForCurrentLab();

            numTotal.ValueChanged += (s, ev) =>
            {
                GenerateSerialInputs();
            };

            btnCancel.Visible = false;

            StyleButton(btnSave, Color.FromArgb(153, 0, 0), Color.White);
        }

        private void AddNewEquipmentFields()
        {
            Label lblType = CreateLabel("Equipment Type", 455, 285);

            cmbEquipmentType = new ComboBox();
            cmbEquipmentType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEquipmentType.Font = new Font("Segoe UI", 10F);
            cmbEquipmentType.Location = new Point(455, 310);
            cmbEquipmentType.Size = new Size(250, 28);
            cmbEquipmentType.Items.Add("Reusable");
            cmbEquipmentType.Items.Add("Limited Use");
            cmbEquipmentType.Items.Add("One Time Use");
            cmbEquipmentType.SelectedIndex = 0;
            cmbEquipmentType.SelectedIndexChanged += (s, ev) => GenerateSerialInputs();

            Label lblSerial = CreateLabel("Serial Number Option", 455, 355);

            rbSerialNo = new RadioButton();
            rbSerialNo.Text = "N/A";
            rbSerialNo.Font = new Font("Segoe UI", 9.5F);
            rbSerialNo.Location = new Point(455, 382);
            rbSerialNo.AutoSize = true;
            rbSerialNo.Checked = true;

            rbSerialYes = new RadioButton();
            rbSerialYes.Text = "With Serial Number";
            rbSerialYes.Font = new Font("Segoe UI", 9.5F);
            rbSerialYes.Location = new Point(530, 382);
            rbSerialYes.AutoSize = true;

            rbSerialNo.CheckedChanged += (s, ev) => GenerateSerialInputs();
            rbSerialYes.CheckedChanged += (s, ev) => GenerateSerialInputs();

            Label lblDesc = CreateLabel("Descriptions", 40, 335);

            txtDescription = new TextBox();
            txtDescription.Font = new Font("Segoe UI", 10F);
            txtDescription.Location = new Point(40, 360);
            txtDescription.Size = new Size(350, 100);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;

            picEquipment = new PictureBox();
            picEquipment.Location = new Point(520, 55);
            picEquipment.Size = new Size(150, 130);
            picEquipment.BackColor = Color.FromArgb(255, 251, 252);
            picEquipment.BorderStyle = BorderStyle.FixedSingle;
            picEquipment.SizeMode = PictureBoxSizeMode.Zoom;
            picEquipment.Paint += (s, e) =>
            {
                if (picEquipment.Image == null)
                {
                    using Pen pen = new Pen(Color.Black, 3);
                    e.Graphics.DrawLine(pen, 75, 30, 75, 100);
                    e.Graphics.DrawLine(pen, 40, 65, 110, 65);
                }
            };

            btnChooseImage = new Button();
            btnChooseImage.Text = "Choose Image";
            btnChooseImage.Location = new Point(530, 198);
            btnChooseImage.Size = new Size(130, 34);
            btnChooseImage.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnChooseImage.Click += btnChooseImage_Click;
            StyleButton(btnChooseImage, Color.FromArgb(212, 168, 45), Color.White);

            Label lblSubjectUsed = CreateLabel("Subjects Used", 40, 265);

            cmbSubjectUsed = new ComboBox();
            cmbSubjectUsed.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSubjectUsed.Font = new Font("Segoe UI", 10F);
            cmbSubjectUsed.Location = new Point(40, 290);
            cmbSubjectUsed.Size = new Size(165, 28);
            cmbSubjectUsed.SelectedIndexChanged += cmbSubjectUsed_SelectedIndexChanged;

            Label lblSubjectsChosen = CreateLabel("Subjects Chosen", 230, 265);

            txtSubjectsChosen = new TextBox();
            txtSubjectsChosen.Font = new Font("Segoe UI", 10F);
            txtSubjectsChosen.Location = new Point(230, 290);
            txtSubjectsChosen.Size = new Size(135, 28);
            txtSubjectsChosen.ReadOnly = true;

            btnOpenChosenSubjects = new Button();
            btnOpenChosenSubjects.Text = "+";
            btnOpenChosenSubjects.Location = new Point(370, 289);
            btnOpenChosenSubjects.Size = new Size(36, 30);
            btnOpenChosenSubjects.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnOpenChosenSubjects.Click += btnOpenChosenSubjects_Click;
            StyleButton(btnOpenChosenSubjects, Color.FromArgb(153, 0, 0), Color.White);

            lblSerialTitle = CreateLabel("Serial Numbers", 455, 420);
            lblSerialTitle.Visible = false;

            flowSerialInputs = new FlowLayoutPanel();
            flowSerialInputs.Location = new Point(455, 445);
            flowSerialInputs.Size = new Size(250, 80);
            flowSerialInputs.FlowDirection = FlowDirection.TopDown;
            flowSerialInputs.WrapContents = false;
            flowSerialInputs.AutoScroll = true;
            flowSerialInputs.Visible = false;
            flowSerialInputs.BackColor = Color.FromArgb(255, 251, 252);
            flowSerialInputs.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(lblType);
            Controls.Add(cmbEquipmentType);
            Controls.Add(lblSerial);
            Controls.Add(rbSerialNo);
            Controls.Add(rbSerialYes);
            Controls.Add(lblDesc);
            Controls.Add(txtDescription);
            Controls.Add(picEquipment);
            Controls.Add(btnChooseImage);
            Controls.Add(lblSubjectUsed);
            Controls.Add(cmbSubjectUsed);
            Controls.Add(lblSubjectsChosen);
            Controls.Add(txtSubjectsChosen);
            Controls.Add(btnOpenChosenSubjects);
            Controls.Add(lblSerialTitle);
            Controls.Add(flowSerialInputs);
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

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(72, 53, 84),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private void StyleButton(Button button, Color backColor, Color foreColor)
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            button.Paint += (s, e) =>
            {
                Rectangle rect = new Rectangle(0, 0, button.Width - 1, button.Height - 1);

                using Pen lightPen = new Pen(Color.FromArgb(255, 245, 190), 2);
                using Pen darkPen = new Pen(Color.FromArgb(120, 60, 30), 2);

                e.Graphics.DrawLine(lightPen, rect.Left + 4, rect.Top + 2, rect.Right - 4, rect.Top + 2);
                e.Graphics.DrawLine(lightPen, rect.Left + 2, rect.Top + 4, rect.Left + 2, rect.Bottom - 4);

                e.Graphics.DrawLine(darkPen, rect.Left + 4, rect.Bottom - 2, rect.Right - 4, rect.Bottom - 2);
                e.Graphics.DrawLine(darkPen, rect.Right - 2, rect.Top + 4, rect.Right - 2, rect.Bottom - 4);
            };

            button.MouseDown += (s, e) =>
            {
                button.Padding = new Padding(2, 2, 0, 0);
                button.Invalidate();
            };

            button.MouseUp += (s, e) =>
            {
                button.Padding = new Padding(0);
                button.Invalidate();
            };

            RoundControl(button, 10);
        }

        private void RoundControl(Control control, int radius)
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

        private void LoadSubjectsForCurrentLab()
        {
            availableSubjects.Clear();
            cmbSubjectUsed.Items.Clear();

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                string query = @"
SELECT SubjectID, SubjectCode, SubjectName
FROM LabSubjects
WHERE LabID = ?
AND IsActive = True
ORDER BY SubjectCode";

                using OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                using OleDbDataReader reader = cmd.ExecuteReader();

                while (reader != null && reader.Read())
                {
                    SubjectCheckItem item = new SubjectCheckItem
                    {
                        SubjectID = Convert.ToInt32(reader["SubjectID"]),
                        DisplayText =
                            (reader["SubjectCode"]?.ToString() ?? "") +
                            " - " +
                            (reader["SubjectName"]?.ToString() ?? "")
                    };

                    availableSubjects.Add(item);
                    cmbSubjectUsed.Items.Add(item);
                }

                if (cmbSubjectUsed.Items.Count > 0)
                    cmbSubjectUsed.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading subjects:\n" + ex.Message,
                    "Subjects",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void cmbSubjectUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSubjectUsed.SelectedItem is not SubjectCheckItem selected)
                return;

            bool alreadyAdded =
                selectedSubjects.Any(x => x.SubjectID == selected.SubjectID);

            if (!alreadyAdded)
            {
                selectedSubjects.Add(selected);
                RefreshChosenSubjectsText();
            }
        }

        private void RefreshChosenSubjectsText()
        {
            if (selectedSubjects.Count == 0)
            {
                txtSubjectsChosen.Text = "";
                return;
            }

            txtSubjectsChosen.Text =
                selectedSubjects.Count + " subject(s)";
        }

        private void btnOpenChosenSubjects_Click(object sender, EventArgs e)
        {
            Form popup = new Form();
            popup.Text = "Subjects Chosen";
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.Size = new Size(430, 420);
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;
            popup.BackColor = Color.FromArgb(250, 245, 247);

            Label lblTitle = new Label();
            lblTitle.Text = "Selected Subjects";
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(69, 45, 96);
            lblTitle.Location = new Point(28, 22);
            lblTitle.AutoSize = true;

            Label lblGuide = new Label();
            lblGuide.Text = "Uncheck a subject to remove it.";
            lblGuide.Font = new Font("Segoe UI", 9.5F);
            lblGuide.ForeColor = Color.FromArgb(126, 105, 136);
            lblGuide.Location = new Point(30, 58);
            lblGuide.AutoSize = true;

            CheckedListBox list = new CheckedListBox();
            list.CheckOnClick = true;
            list.Font = new Font("Segoe UI", 9.5F);
            list.Location = new Point(30, 90);
            list.Size = new Size(350, 220);
            list.BackColor = Color.White;

            foreach (SubjectCheckItem subject in selectedSubjects)
            {
                list.Items.Add(subject, true);
            }

            Button btnDone = new Button();
            btnDone.Text = "Done";
            btnDone.Size = new Size(120, 36);
            btnDone.Location = new Point(260, 325);
            btnDone.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            StyleButton(btnDone, Color.FromArgb(153, 0, 0), Color.White);

            btnDone.Click += (s, ev) =>
            {
                List<SubjectCheckItem> remaining = new List<SubjectCheckItem>();

                for (int i = 0; i < list.Items.Count; i++)
                {
                    if (list.GetItemChecked(i) &&
                        list.Items[i] is SubjectCheckItem item)
                    {
                        remaining.Add(item);
                    }
                }

                selectedSubjects.Clear();
                selectedSubjects.AddRange(remaining);
                RefreshChosenSubjectsText();

                popup.Close();
            };

            popup.Controls.Add(lblTitle);
            popup.Controls.Add(lblGuide);
            popup.Controls.Add(list);
            popup.Controls.Add(btnDone);

            popup.ShowDialog(this);
        }

        private void GenerateSerialInputs()
        {
            if (flowSerialInputs == null || lblSerialTitle == null)
                return;

            flowSerialInputs.Controls.Clear();

            bool oneTimeUse =
                cmbEquipmentType != null &&
                cmbEquipmentType.Text == "One Time Use";

            if (oneTimeUse)
            {
                rbSerialNo.Checked = true;
                rbSerialYes.Checked = false;
                rbSerialYes.Enabled = false;
            }
            else
            {
                rbSerialYes.Enabled = true;
            }

            bool withSerial =
                rbSerialYes != null &&
                rbSerialYes.Checked &&
                !oneTimeUse;

            lblSerialTitle.Visible = withSerial;
            flowSerialInputs.Visible = withSerial;

            if (!withSerial)
                return;

            int total = (int)numTotal.Value;

            for (int i = 1; i <= total; i++)
            {
                TextBox txtSerial = new TextBox();
                txtSerial.Name = "txtSerial" + i;
                txtSerial.Font = new Font("Segoe UI", 9F);
                txtSerial.Size = new Size(220, 24);
                txtSerial.Margin = new Padding(8, 6, 8, 0);
                txtSerial.PlaceholderText = "Serial #" + i;

                flowSerialInputs.Controls.Add(txtSerial);
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Choose Equipment Image";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = ofd.FileName;

                using FileStream fs = new FileStream(selectedImagePath, FileMode.Open, FileAccess.Read);
                using Image temp = Image.FromStream(fs);

                picEquipment.Image = new Bitmap(temp);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Equipment name is required.");
                return;
            }

            if (selectedSubjects.Count == 0)
            {
                MessageBox.Show("Please select at least one subject.");
                return;
            }

            bool saveWithSerial = rbSerialYes.Checked && cmbEquipmentType.Text != "One Time Use";

            if (saveWithSerial)
            {
                foreach (Control ctrl in flowSerialInputs.Controls)
                {
                    if (ctrl is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
                    {
                        MessageBox.Show("Please complete all serial numbers.");
                        return;
                    }
                }
            }

            try
            {
                using OleDbConnection conn = DbHelper.GetConnection();
                conn.Open();

                using OleDbTransaction trans = conn.BeginTransaction();

                try
                {
                    string insertEquipmentQuery = @"
INSERT INTO Equipment
(
    EquipmentName,
    Category,
    QuantityTotal,
    QuantityMaintenance,
    LowStockThreshold,
    Status,
    LabID,
    Brand,
    IsArchived,
    EquipmentType,
    HasSerial,
    Description,
    ImagePath
)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(insertEquipmentQuery, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@p1", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@p2", cmbCategory.Text);
                        cmd.Parameters.AddWithValue("@p3", (int)numTotal.Value);
                        cmd.Parameters.AddWithValue("@p4", (int)numMaintenance.Value);
                        cmd.Parameters.AddWithValue("@p5", (int)numThreshold.Value);
                        cmd.Parameters.AddWithValue("@p6", "Active");
                        cmd.Parameters.AddWithValue("@p7", SessionManager.LabID);
                        cmd.Parameters.AddWithValue("@p8", txtBrand.Text.Trim());
                        cmd.Parameters.AddWithValue("@p9", false);
                        cmd.Parameters.AddWithValue("@p10", cmbEquipmentType.Text);
                        cmd.Parameters.AddWithValue("@p11", saveWithSerial);
                        cmd.Parameters.AddWithValue("@p12", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@p13", selectedImagePath);

                        cmd.ExecuteNonQuery();
                    }

                    int newEquipmentId = 0;

                    using (OleDbCommand idCmd = new OleDbCommand("SELECT @@IDENTITY", conn, trans))
                    {
                        object result = idCmd.ExecuteScalar();

                        newEquipmentId =
                            result != null && result != DBNull.Value
                            ? Convert.ToInt32(result)
                            : 0;
                    }

                    foreach (SubjectCheckItem subject in selectedSubjects)
                    {
                        string insertSubjectQuery = @"
INSERT INTO SubjectEquipments
(
    SubjectID,
    EquipmentID
)
VALUES (?, ?)";

                        using OleDbCommand cmd = new OleDbCommand(insertSubjectQuery, conn, trans);
                        cmd.Parameters.AddWithValue("@p1", subject.SubjectID);
                        cmd.Parameters.AddWithValue("@p2", newEquipmentId);
                        cmd.ExecuteNonQuery();
                    }

                    if (saveWithSerial)
                    {
                        foreach (Control ctrl in flowSerialInputs.Controls)
                        {
                            if (ctrl is TextBox txt)
                            {
                                string insertUnitQuery = @"
INSERT INTO EquipmentUnits
(
    EquipmentID,
    SerialNumber,
    UnitStatus,
    DateAdded
)
VALUES (?, ?, ?, ?)";

                                using OleDbCommand cmd =
                                    new OleDbCommand(insertUnitQuery, conn, trans);

                                cmd.Parameters.Add(
                                    "@p1",
                                    OleDbType.Integer
                                ).Value = newEquipmentId;

                                cmd.Parameters.Add(
                                    "@p2",
                                    OleDbType.VarWChar
                                ).Value = txt.Text.Trim();

                                cmd.Parameters.Add(
                                    "@p3",
                                    OleDbType.VarWChar
                                ).Value = "Available";

                                cmd.Parameters.Add(
                                    "@p4",
                                    OleDbType.Date
                                ).Value = DateTime.Now;

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    trans.Commit();

                    MessageBox.Show("Equipment added successfully.");

                    DialogResult = DialogResult.OK;
                    Close();
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
                    "Error saving equipment:\n" + ex.Message,
                    "Add Equipment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
