namespace WildcatHub
{
    partial class frmAddEquipment
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblName;
        private TextBox txtName;
        private Label lblBrand;
        private TextBox txtBrand;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblTotal;
        private NumericUpDown numTotal;
        private Label lblMaintenance;
        private NumericUpDown numMaintenance;
        private Label lblThreshold;
        private NumericUpDown numThreshold;
        private Label lblSubjects;
        private CheckedListBox clbSubjects;
        private Button btnSave;
        private Button btnCancel;

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
            lblTitle = new Label();
            lblName = new Label();
            txtName = new TextBox();
            lblBrand = new Label();
            txtBrand = new TextBox();
            lblCategory = new Label();
            cmbCategory = new ComboBox();
            lblTotal = new Label();
            numTotal = new NumericUpDown();
            lblMaintenance = new Label();
            numMaintenance = new NumericUpDown();
            lblThreshold = new Label();
            numThreshold = new NumericUpDown();
            lblSubjects = new Label();
            clbSubjects = new CheckedListBox();
            btnSave = new Button();
            btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)numTotal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaintenance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).BeginInit();

            SuspendLayout();

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.Location = new Point(28, 22);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Add Equipment";

            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblName.ForeColor = Color.Black;
            lblName.Location = new Point(40, 82);
            lblName.Name = "lblName";
            lblName.Text = "Equipment Name";

            txtName.Font = new Font("Segoe UI", 10F);
            txtName.Location = new Point(40, 105);
            txtName.Name = "txtName";
            txtName.Size = new Size(170, 25);

            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblTotal.ForeColor = Color.Black;
            lblTotal.Location = new Point(240, 82);
            lblTotal.Name = "lblTotal";
            lblTotal.Text = "Available";

            numTotal.Font = new Font("Segoe UI", 10F);
            numTotal.Location = new Point(240, 105);
            numTotal.Name = "numTotal";
            numTotal.Size = new Size(80, 25);

            lblBrand.AutoSize = true;
            lblBrand.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblBrand.ForeColor = Color.Black;
            lblBrand.Location = new Point(40, 145);
            lblBrand.Name = "lblBrand";
            lblBrand.Text = "Brand";

            txtBrand.Font = new Font("Segoe UI", 10F);
            txtBrand.Location = new Point(40, 168);
            txtBrand.Name = "txtBrand";
            txtBrand.Size = new Size(170, 25);

            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblCategory.ForeColor = Color.Black;
            lblCategory.Location = new Point(240, 145);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Category";

            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Font = new Font("Segoe UI", 10F);
            cmbCategory.Location = new Point(240, 168);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(170, 25);

            lblMaintenance.AutoSize = true;
            lblMaintenance.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblMaintenance.ForeColor = Color.Black;
            lblMaintenance.Location = new Point(40, 208);
            lblMaintenance.Name = "lblMaintenance";
            lblMaintenance.Text = "Maintenance Quantity";

            numMaintenance.Font = new Font("Segoe UI", 10F);
            numMaintenance.Location = new Point(40, 231);
            numMaintenance.Name = "numMaintenance";
            numMaintenance.Size = new Size(80, 25);

            lblThreshold.AutoSize = true;
            lblThreshold.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            lblThreshold.ForeColor = Color.Black;
            lblThreshold.Location = new Point(240, 208);
            lblThreshold.Name = "lblThreshold";
            lblThreshold.Text = "Low Stock Threshold";

            numThreshold.Font = new Font("Segoe UI", 10F);
            numThreshold.Location = new Point(240, 231);
            numThreshold.Name = "numThreshold";
            numThreshold.Size = new Size(80, 25);

            lblSubjects.Visible = false;
            clbSubjects.Visible = false;

            btnCancel.Visible = false;

            btnSave.BackColor = Color.FromArgb(153, 0, 0);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(600, 520);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(125, 38);
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 590);

            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(clbSubjects);
            Controls.Add(lblSubjects);
            Controls.Add(numThreshold);
            Controls.Add(lblThreshold);
            Controls.Add(numMaintenance);
            Controls.Add(lblMaintenance);
            Controls.Add(txtBrand);
            Controls.Add(lblBrand);
            Controls.Add(numTotal);
            Controls.Add(lblTotal);
            Controls.Add(cmbCategory);
            Controls.Add(lblCategory);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Controls.Add(lblTitle);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAddEquipment";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Equipment";
            Load += frmAddEquipment_Load;

            ((System.ComponentModel.ISupportInitialize)numTotal).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaintenance).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
