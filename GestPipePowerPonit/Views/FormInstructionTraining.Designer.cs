namespace GestPipePowerPonit
{
    partial class FormInstructionTraining
    {
        private System.ComponentModel.IContainer components = null;

        // CÁC CONTROLS GUNA UI
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel panelMain; // Panel chứa chính
        private Guna.UI2.WinForms.Guna2Button btnClose; // Nút đóng
        private Guna.UI2.WinForms.Guna2PictureBox pictureBoxHandLayer; // Giữ nguyên
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2Button btnTraining;

        // CÁC LABEL HIỂN THỊ DỮ LIỆU
        private System.Windows.Forms.Label lblTitleName;
        private System.Windows.Forms.Label lblValueName;
        private System.Windows.Forms.Label lblTitleType;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.Label lblTitleDirection;
        private System.Windows.Forms.Label lblValueDirection;
        private System.Windows.Forms.Label lblTitleInstruction;
        private System.Windows.Forms.Label lblTitleNote; // THÊM MỚI
        private System.Windows.Forms.Label lblValueNote; // THÊM MỚI

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstructionTraining));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.lblValueName = new System.Windows.Forms.Label();
            this.lblTitleType = new System.Windows.Forms.Label();
            this.lblValueType = new System.Windows.Forms.Label();
            this.lblTitleDirection = new System.Windows.Forms.Label();
            this.lblValueDirection = new System.Windows.Forms.Label();
            this.lblTitleInstruction = new System.Windows.Forms.Label();
            this.lblTitleNote = new System.Windows.Forms.Label();
            this.lblValueNote = new System.Windows.Forms.Label();
            this.btnTraining = new Guna.UI2.WinForms.Guna2Button();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBoxHandLayer = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHandLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Transparent;
            this.panelMain.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panelMain.BorderRadius = 15;
            this.panelMain.BorderThickness = 3;
            this.panelMain.Controls.Add(this.lblTitleName);
            this.panelMain.Controls.Add(this.lblValueName);
            this.panelMain.Controls.Add(this.lblTitleType);
            this.panelMain.Controls.Add(this.lblValueType);
            this.panelMain.Controls.Add(this.lblTitleDirection);
            this.panelMain.Controls.Add(this.lblValueDirection);
            this.panelMain.Controls.Add(this.lblTitleInstruction);
            this.panelMain.Controls.Add(this.lblTitleNote);
            this.panelMain.Controls.Add(this.lblValueNote);
            this.panelMain.Controls.Add(this.btnTraining);
            this.panelMain.Controls.Add(this.btnClose);
            this.panelMain.Controls.Add(this.pictureBoxHandLayer);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.FillColor = System.Drawing.Color.Black;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(750, 750);
            this.panelMain.TabIndex = 0;
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTitleName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitleName.ForeColor = System.Drawing.Color.Cyan;
            this.lblTitleName.Location = new System.Drawing.Point(33, 20);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(109, 41);
            this.lblTitleName.TabIndex = 6;
            this.lblTitleName.Text = "Name:";
            // 
            // lblValueName
            // 
            this.lblValueName.AutoSize = true;
            this.lblValueName.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblValueName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblValueName.ForeColor = System.Drawing.Color.Cyan;
            this.lblValueName.Location = new System.Drawing.Point(300, 20);
            this.lblValueName.Name = "lblValueName";
            this.lblValueName.Size = new System.Drawing.Size(123, 41);
            this.lblValueName.TabIndex = 7;
            this.lblValueName.Text = "[Name]";
            // 
            // lblTitleType
            // 
            this.lblTitleType.AutoSize = true;
            this.lblTitleType.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTitleType.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleType.ForeColor = System.Drawing.Color.White;
            this.lblTitleType.Location = new System.Drawing.Point(35, 73);
            this.lblTitleType.Name = "lblTitleType";
            this.lblTitleType.Size = new System.Drawing.Size(62, 28);
            this.lblTitleType.TabIndex = 8;
            this.lblTitleType.Text = "Type:";
            // 
            // lblValueType
            // 
            this.lblValueType.AutoSize = true;
            this.lblValueType.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblValueType.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblValueType.ForeColor = System.Drawing.Color.LightGray;
            this.lblValueType.Location = new System.Drawing.Point(302, 73);
            this.lblValueType.Name = "lblValueType";
            this.lblValueType.Size = new System.Drawing.Size(65, 28);
            this.lblValueType.TabIndex = 9;
            this.lblValueType.Text = "[Type]";
            // 
            // lblTitleDirection
            // 
            this.lblTitleDirection.AutoSize = true;
            this.lblTitleDirection.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTitleDirection.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleDirection.ForeColor = System.Drawing.Color.Cyan;
            this.lblTitleDirection.Location = new System.Drawing.Point(35, 118);
            this.lblTitleDirection.Name = "lblTitleDirection";
            this.lblTitleDirection.Size = new System.Drawing.Size(105, 28);
            this.lblTitleDirection.TabIndex = 10;
            this.lblTitleDirection.Text = "Direction:";
            // 
            // lblValueDirection
            // 
            this.lblValueDirection.AutoSize = true;
            this.lblValueDirection.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblValueDirection.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblValueDirection.ForeColor = System.Drawing.Color.Cyan;
            this.lblValueDirection.Location = new System.Drawing.Point(302, 118);
            this.lblValueDirection.Name = "lblValueDirection";
            this.lblValueDirection.Size = new System.Drawing.Size(104, 28);
            this.lblValueDirection.TabIndex = 11;
            this.lblValueDirection.Text = "[Direction]";
            // 
            // lblTitleInstruction
            // 
            this.lblTitleInstruction.AutoSize = true;
            this.lblTitleInstruction.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTitleInstruction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleInstruction.ForeColor = System.Drawing.Color.White;
            this.lblTitleInstruction.Location = new System.Drawing.Point(35, 159);
            this.lblTitleInstruction.Name = "lblTitleInstruction";
            this.lblTitleInstruction.Size = new System.Drawing.Size(199, 28);
            this.lblTitleInstruction.TabIndex = 12;
            this.lblTitleInstruction.Text = "Gesture Instruction:";
            // 
            // lblTitleNote
            // 
            this.lblTitleNote.AutoSize = true;
            this.lblTitleNote.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTitleNote.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleNote.ForeColor = System.Drawing.Color.Wheat;
            this.lblTitleNote.Location = new System.Drawing.Point(46, 547);
            this.lblTitleNote.Name = "lblTitleNote";
            this.lblTitleNote.Size = new System.Drawing.Size(64, 28);
            this.lblTitleNote.TabIndex = 13;
            this.lblTitleNote.Text = "Note:";
            // 
            // lblValueNote
            // 
            this.lblValueNote.AutoSize = true;
            this.lblValueNote.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblValueNote.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblValueNote.ForeColor = System.Drawing.Color.Wheat;
            this.lblValueNote.Location = new System.Drawing.Point(137, 547);
            this.lblValueNote.Name = "lblValueNote";
            this.lblValueNote.Size = new System.Drawing.Size(68, 28);
            this.lblValueNote.TabIndex = 14;
            this.lblValueNote.Text = "[Note]";
            // 
            // btnTraining
            // 
            this.btnTraining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTraining.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnTraining.BorderRadius = 10;
            this.btnTraining.FillColor = System.Drawing.Color.SteelBlue;
            this.btnTraining.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTraining.ForeColor = System.Drawing.Color.White;
            this.btnTraining.Location = new System.Drawing.Point(441, 690);
            this.btnTraining.Name = "btnTraining";
            this.btnTraining.Size = new System.Drawing.Size(146, 40);
            this.btnTraining.TabIndex = 5;
            this.btnTraining.Text = "Training";
            this.btnTraining.Click += new System.EventHandler(this.btnTraining_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnClose.BorderRadius = 10;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(620, 690);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 40);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBoxHandLayer
            // 
            this.pictureBoxHandLayer.BackColor = System.Drawing.Color.Black;
            this.pictureBoxHandLayer.BorderRadius = 10;
            this.pictureBoxHandLayer.FillColor = System.Drawing.Color.Black;
            this.pictureBoxHandLayer.ImageRotate = 0F;
            this.pictureBoxHandLayer.Location = new System.Drawing.Point(38, 216);
            this.pictureBoxHandLayer.Name = "pictureBoxHandLayer";
            this.pictureBoxHandLayer.Size = new System.Drawing.Size(674, 300);
            this.pictureBoxHandLayer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxHandLayer.TabIndex = 0;
            this.pictureBoxHandLayer.TabStop = false;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.panelMain;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // FormInstructionTraining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 750);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormInstructionTraining";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormInstructionTraining";
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHandLayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}