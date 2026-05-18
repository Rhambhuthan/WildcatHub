using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WildcatHub
{
    internal static class EquipmentCategoryService
    {
        public const int MaxCategoriesPerLab = 5;

        public static void EnsureCategoryTable()
        {
            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            DataTable tables = conn.GetSchema("Tables");
            bool exists = tables.AsEnumerable().Any(row =>
                string.Equals(row["TABLE_NAME"]?.ToString(), "EquipmentCategories", StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                using OleDbCommand createCmd = new OleDbCommand(@"
CREATE TABLE EquipmentCategories
(
    CategoryID AUTOINCREMENT PRIMARY KEY,
    LabID INTEGER,
    CategoryName TEXT(100),
    IsActive YESNO
)", conn);
                createCmd.ExecuteNonQuery();
            }

            SeedFromExistingEquipment(conn);
        }

        public static List<string> GetCategories()
        {
            EnsureCategoryTable();

            List<string> categories = new List<string>();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbCommand cmd = new OleDbCommand(@"
SELECT CategoryName
FROM EquipmentCategories
WHERE LabID = ?
AND IsActive = True
ORDER BY CategoryName", conn);

            cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader != null && reader.Read())
            {
                string category = reader["CategoryName"]?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(category))
                    categories.Add(category);
            }

            return categories;
        }

        public static void SaveCategories(IEnumerable<string> categoryNames)
        {
            SaveCategories(categoryNames, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        public static void SaveCategories(
            IEnumerable<string> categoryNames,
            Dictionary<string, string> renameMap)
        {
            EnsureCategoryTable();

            List<string> newCategories = categoryNames
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            if (newCategories.Count > MaxCategoriesPerLab)
                throw new InvalidOperationException(
                    $"Only {MaxCategoriesPerLab} categories are allowed per laboratory.");

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbTransaction trans = conn.BeginTransaction();

            try
            {
                foreach (KeyValuePair<string, string> rename in renameMap)
                {
                    string oldName = rename.Key.Trim();
                    string newName = rename.Value.Trim();

                    if (string.IsNullOrWhiteSpace(oldName) ||
                        string.IsNullOrWhiteSpace(newName) ||
                        string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    using (OleDbCommand updateEquipmentCmd = new OleDbCommand(@"
UPDATE Equipment
SET Category = ?
WHERE LabID = ?
AND Category = ?", conn, trans))
                    {
                        updateEquipmentCmd.Parameters.AddWithValue("@p1", newName);
                        updateEquipmentCmd.Parameters.AddWithValue("@p2", SessionManager.LabID);
                        updateEquipmentCmd.Parameters.AddWithValue("@p3", oldName);
                        updateEquipmentCmd.ExecuteNonQuery();
                    }

                    using (OleDbCommand updateCategoryCmd = new OleDbCommand(@"
UPDATE EquipmentCategories
SET CategoryName = ?
WHERE LabID = ?
AND CategoryName = ?", conn, trans))
                    {
                        updateCategoryCmd.Parameters.AddWithValue("@p1", newName);
                        updateCategoryCmd.Parameters.AddWithValue("@p2", SessionManager.LabID);
                        updateCategoryCmd.Parameters.AddWithValue("@p3", oldName);
                        updateCategoryCmd.ExecuteNonQuery();
                    }
                }

                using (OleDbCommand deactivateCmd = new OleDbCommand(@"
UPDATE EquipmentCategories
SET IsActive = False
WHERE LabID = ?", conn, trans))
                {
                    deactivateCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                    deactivateCmd.ExecuteNonQuery();
                }

                foreach (string category in newCategories)
                {
                    int existingId = 0;
                    using (OleDbCommand findCmd = new OleDbCommand(@"
SELECT CategoryID
FROM EquipmentCategories
WHERE LabID = ?
AND CategoryName = ?", conn, trans))
                    {
                        findCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                        findCmd.Parameters.AddWithValue("@p2", category);
                        object? result = findCmd.ExecuteScalar();
                        existingId = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }

                    if (existingId > 0)
                    {
                        using OleDbCommand activateCmd = new OleDbCommand(@"
UPDATE EquipmentCategories
SET IsActive = True
WHERE CategoryID = ?", conn, trans);
                        activateCmd.Parameters.AddWithValue("@p1", existingId);
                        activateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using OleDbCommand insertCmd = new OleDbCommand(@"
INSERT INTO EquipmentCategories
(
    LabID,
    CategoryName,
    IsActive
)
VALUES (?, ?, ?)", conn, trans);
                        insertCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                        insertCmd.Parameters.AddWithValue("@p2", category);
                        insertCmd.Parameters.AddWithValue("@p3", true);
                        insertCmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public static int CountEquipmentUsingCategory(string categoryName)
        {
            EnsureCategoryTable();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            using OleDbCommand cmd = new OleDbCommand(@"
SELECT COUNT(*)
FROM Equipment
WHERE LabID = ?
AND Category = ?", conn);

            cmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
            cmd.Parameters.AddWithValue("@p2", categoryName);

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        private static void SeedFromExistingEquipment(OleDbConnection conn)
        {
            int count;
            using (OleDbCommand countCmd = new OleDbCommand(
                "SELECT COUNT(*) FROM EquipmentCategories WHERE LabID = ?",
                conn))
            {
                countCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                count = Convert.ToInt32(countCmd.ExecuteScalar() ?? 0);
            }

            if (count > 0)
                return;

            List<string> categories = new List<string>();

            using (OleDbCommand existingCmd = new OleDbCommand(@"
SELECT DISTINCT Category
FROM Equipment
WHERE LabID = ?
AND Category IS NOT NULL
ORDER BY Category", conn))
            {
                existingCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);

                using OleDbDataReader reader = existingCmd.ExecuteReader();
                while (reader != null && reader.Read())
                {
                    string category = reader["Category"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(category))
                        categories.Add(category);
                }
            }

            if (categories.Count == 0)
            {
                categories.Add("Apparatus");
                categories.Add("Glassware");
                categories.Add("Materials");
            }

            foreach (string category in categories.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                using OleDbCommand insertCmd = new OleDbCommand(@"
INSERT INTO EquipmentCategories
(
    LabID,
    CategoryName,
    IsActive
)
VALUES (?, ?, ?)", conn);
                insertCmd.Parameters.AddWithValue("@p1", SessionManager.LabID);
                insertCmd.Parameters.AddWithValue("@p2", category);
                insertCmd.Parameters.AddWithValue("@p3", true);
                insertCmd.ExecuteNonQuery();
            }
        }
    }

    internal sealed class EquipmentCategoryManagerForm : Form
    {
        private readonly TextBox txtNewCategory = new TextBox();
        private readonly FlowLayoutPanel flowCategories = new FlowLayoutPanel();
        private readonly LinkLabel linkEdit = new LinkLabel();
        private readonly LinkLabel linkDelete = new LinkLabel();
        private readonly LinkLabel linkSave = new LinkLabel();
        private bool isEditing = false;
        private List<string> categories;
        private readonly List<string> originalCategories;

        public EquipmentCategoryManagerForm(List<string> existingCategories)
        {
            categories = existingCategories.ToList();
            originalCategories = existingCategories.ToList();
            InitializeCategoryForm();
            RenderCategories();
        }

        public List<string> Categories => categories.ToList();

        private void InitializeCategoryForm()
        {
            Text = "Categories";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(320, 390);
            BackColor = Color.FromArgb(250, 245, 247);

            txtNewCategory.PlaceholderText = "Add Category";
            txtNewCategory.Font = new Font("Segoe UI", 10F);
            txtNewCategory.Location = new Point(30, 26);
            txtNewCategory.Size = new Size(250, 27);
            txtNewCategory.KeyDown += txtNewCategory_KeyDown;

            flowCategories.Location = new Point(30, 70);
            flowCategories.Size = new Size(250, 235);
            flowCategories.FlowDirection = FlowDirection.TopDown;
            flowCategories.WrapContents = false;
            flowCategories.AutoScroll = true;
            flowCategories.BackColor = Color.Transparent;

            linkEdit.Text = "Edit";
            linkEdit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            linkEdit.LinkColor = Color.FromArgb(160, 120, 20);
            linkEdit.Location = new Point(30, 326);
            linkEdit.AutoSize = true;
            linkEdit.Click += (s, e) =>
            {
                isEditing = true;
                RenderCategories();
            };

            linkDelete.Text = "Delete";
            linkDelete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            linkDelete.LinkColor = Color.FromArgb(180, 75, 75);
            linkDelete.Location = new Point(30, 326);
            linkDelete.AutoSize = true;
            linkDelete.Visible = false;
            linkDelete.Click += linkDelete_Click;

            linkSave.Text = "Save";
            linkSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            linkSave.LinkColor = Color.FromArgb(45, 110, 60);
            linkSave.Location = new Point(230, 326);
            linkSave.AutoSize = true;
            linkSave.Visible = false;
            linkSave.Click += linkSave_Click;

            Controls.Add(txtNewCategory);
            Controls.Add(flowCategories);
            Controls.Add(linkEdit);
            Controls.Add(linkDelete);
            Controls.Add(linkSave);
        }

        private void txtNewCategory_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            AddCategoryFromTextBox();
            e.SuppressKeyPress = true;
        }

        private void AddCategoryFromTextBox()
        {
            string category = txtNewCategory.Text.Trim();
            if (string.IsNullOrWhiteSpace(category))
                return;

            if (!categories.Contains(category, StringComparer.OrdinalIgnoreCase) &&
                categories.Count >= EquipmentCategoryService.MaxCategoriesPerLab)
            {
                MessageBox.Show(
                    $"Each laboratory can only have up to {EquipmentCategoryService.MaxCategoriesPerLab} categories.",
                    "Category Limit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!categories.Contains(category, StringComparer.OrdinalIgnoreCase))
                categories.Add(category);

            txtNewCategory.Clear();
            RenderCategories();
        }

        private void RenderCategories()
        {
            flowCategories.Controls.Clear();

            foreach (string category in categories.OrderBy(x => x))
            {
                if (isEditing)
                {
                    Panel row = new Panel
                    {
                        Width = 230,
                        Height = 32,
                        Margin = new Padding(0, 0, 0, 5),
                        Tag = category
                    };

                    CheckBox check = new CheckBox
                    {
                        Location = new Point(0, 6),
                        AutoSize = true,
                        Tag = category
                    };

                    TextBox txt = new TextBox
                    {
                        Text = category,
                        Font = new Font("Segoe UI", 9.5F),
                        Location = new Point(28, 2),
                        Size = new Size(190, 25),
                        Tag = category
                    };

                    row.Controls.Add(check);
                    row.Controls.Add(txt);
                    flowCategories.Controls.Add(row);
                }
                else
                {
                    Label label = new Label
                    {
                        Text = category,
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.FromArgb(72, 53, 84),
                        Width = 230,
                        Height = 26,
                        Margin = new Padding(0, 0, 0, 3)
                    };

                    flowCategories.Controls.Add(label);
                }
            }

            linkEdit.Visible = !isEditing;
            linkDelete.Visible = isEditing;
            linkSave.Visible = isEditing;
        }

        private void linkDelete_Click(object? sender, EventArgs e)
        {
            if (!isEditing)
                return;

            List<string> toDelete = new List<string>();

            foreach (Panel row in flowCategories.Controls.OfType<Panel>())
            {
                CheckBox? check = row.Controls.OfType<CheckBox>().FirstOrDefault();
                TextBox? txt = row.Controls.OfType<TextBox>().FirstOrDefault();

                if (check != null && check.Checked && txt != null)
                {
                    string originalName = check.Tag?.ToString() ?? txt.Text.Trim();
                    int usageCount = EquipmentCategoryService.CountEquipmentUsingCategory(originalName);

                    if (usageCount > 0)
                    {
                        MessageBox.Show(
                            $"{originalName} cannot be deleted because {usageCount} equipment item(s) still use it.\n\nYou can rename it instead.",
                            "Category In Use",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    toDelete.Add(originalName);
                }
            }

            categories = categories
                .Where(x => !toDelete.Contains(x, StringComparer.OrdinalIgnoreCase))
                .ToList();

            RenderCategories();
        }

        private void linkSave_Click(object? sender, EventArgs e)
        {
            AddCategoryFromTextBox();

            if (isEditing)
            {
                Dictionary<string, string> renameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                List<string> editedCategories = new List<string>();

                foreach (Panel row in flowCategories.Controls.OfType<Panel>())
                {
                    TextBox? txt = row.Controls.OfType<TextBox>().FirstOrDefault();
                    if (txt == null || string.IsNullOrWhiteSpace(txt.Text))
                        continue;

                    string originalName = txt.Tag?.ToString() ?? txt.Text.Trim();
                    string newName = txt.Text.Trim();

                    editedCategories.Add(newName);

                    if (!string.Equals(originalName, newName, StringComparison.OrdinalIgnoreCase))
                        renameMap[originalName] = newName;
                }

                categories = editedCategories
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList();

                try
                {
                    EquipmentCategoryService.SaveCategories(categories, renameMap);
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving categories:\n" + ex.Message,
                        "Categories", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                EquipmentCategoryService.SaveCategories(categories);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving categories:\n" + ex.Message,
                    "Categories", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
