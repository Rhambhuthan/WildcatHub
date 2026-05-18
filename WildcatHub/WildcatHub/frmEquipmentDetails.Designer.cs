namespace WildcatHub
{
    partial class frmEquipmentDetails
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblMaintenance;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.TextBox txtBrand;
        private System.Windows.Forms.NumericUpDown numTotal;
        private System.Windows.Forms.NumericUpDown numMaintenance;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.ComboBox cmbStatus;

        private System.Windows.Forms.LinkLabel linkDelete;
        private System.Windows.Forms.LinkLabel linkEditSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblMaintenance = new System.Windows.Forms.Label();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();

            this.txtName = new System.Windows.Forms.TextBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.txtBrand = new System.Windows.Forms.TextBox();
            this.numTotal = new System.Windows.Forms.NumericUpDown();
            this.numMaintenance = new System.Windows.Forms.NumericUpDown();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.cmbStatus = new System.Windows.Forms.ComboBox();

            this.linkDelete = new System.Windows.Forms.LinkLabel();
            this.linkEditSave = new System.Windows.Forms.LinkLabel();

            ((System.ComponentModel.ISupportInitialize)(this.numTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaintenance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();

            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(760, 610);
            this.Text = "Equipment Details";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(255, 251, 240);
            this.Load += new System.EventHandler(this.frmEquipmentDetails_Load);

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(92, 45, 58);
            this.lblTitle.Location = new System.Drawing.Point(155, 25);
            this.lblTitle.Text = "Equipment Details";

            this.lblName.Location = new System.Drawing.Point(40, 85);
            this.lblName.Size = new System.Drawing.Size(150, 20);
            this.lblName.Text = "Equipment Name";

            this.txtName.Location = new System.Drawing.Point(40, 108);
            this.txtName.Size = new System.Drawing.Size(470, 27);
            this.txtName.TabStop = false;

            this.lblCategory.Location = new System.Drawing.Point(40, 150);
            this.lblCategory.Size = new System.Drawing.Size(150, 20);
            this.lblCategory.Text = "Category";

            this.cmbCategory.Location = new System.Drawing.Point(40, 173);
            this.cmbCategory.Size = new System.Drawing.Size(470, 28);
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblBrand.Location = new System.Drawing.Point(40, 215);
            this.lblBrand.Size = new System.Drawing.Size(150, 20);
            this.lblBrand.Text = "Brand";

            this.txtBrand.Location = new System.Drawing.Point(40, 238);
            this.txtBrand.Size = new System.Drawing.Size(470, 27);

            this.lblTotal.Location = new System.Drawing.Point(40, 285);
            this.lblTotal.Size = new System.Drawing.Size(90, 20);
            this.lblTotal.Text = "Available";

            this.numTotal.Location = new System.Drawing.Point(40, 308);
            this.numTotal.Size = new System.Drawing.Size(110, 27);

            this.lblMaintenance.Location = new System.Drawing.Point(205, 285);
            this.lblMaintenance.Size = new System.Drawing.Size(130, 20);
            this.lblMaintenance.Text = "Maintenance";

            this.numMaintenance.Location = new System.Drawing.Point(205, 308);
            this.numMaintenance.Size = new System.Drawing.Size(110, 27);

            this.lblThreshold.Location = new System.Drawing.Point(370, 285);
            this.lblThreshold.Size = new System.Drawing.Size(130, 20);
            this.lblThreshold.Text = "Low Stock";

            this.numThreshold.Location = new System.Drawing.Point(370, 308);
            this.numThreshold.Size = new System.Drawing.Size(110, 27);

            this.lblStatus.Location = new System.Drawing.Point(40, 355);
            this.lblStatus.Size = new System.Drawing.Size(150, 20);
            this.lblStatus.Text = "Status";

            this.cmbStatus.Location = new System.Drawing.Point(40, 378);
            this.cmbStatus.Size = new System.Drawing.Size(175, 28);
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.linkDelete.Location = new System.Drawing.Point(40, 570);
            this.linkDelete.Text = "Archive";
            this.linkDelete.LinkColor = System.Drawing.Color.FromArgb(180, 75, 75);
            this.linkDelete.AutoSize = true;
            this.linkDelete.Click += new System.EventHandler(this.linkDelete_Click);

            this.linkEditSave.Location = new System.Drawing.Point(665, 570);
            this.linkEditSave.Text = "Edit";
            this.linkEditSave.LinkColor = System.Drawing.Color.FromArgb(160, 120, 20);
            this.linkEditSave.AutoSize = true;
            this.linkEditSave.Click += new System.EventHandler(this.linkEditSave_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.lblBrand);
            this.Controls.Add(this.txtBrand);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.numTotal);
            this.Controls.Add(this.lblMaintenance);
            this.Controls.Add(this.numMaintenance);
            this.Controls.Add(this.lblThreshold);
            this.Controls.Add(this.numThreshold);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.linkDelete);
            this.Controls.Add(this.linkEditSave);

            ((System.ComponentModel.ISupportInitialize)(this.numTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaintenance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
