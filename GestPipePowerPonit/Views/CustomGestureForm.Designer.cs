using Guna.UI2.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    partial class CustomGestureForm
    {
        private System.ComponentModel.IContainer components = null;

        // Existing controls
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;

        // CÁC KHAI BÁO BIẾN CẦN THIẾT
        private System.Windows.Forms.Label lblCustomInfo;
        private System.Windows.Forms.Label lblCustomCount;
        private System.Windows.Forms.Label lblCustomStatus; // Dòng này là biến thành viên
        private System.Windows.Forms.PictureBox pictureBoxCustomCamera;
        private Guna.UI2.WinForms.Guna2Button btnHome;
        private System.Windows.Forms.Label lblName;

        // NEW CONTROLS FOR INSTRUCTIONS
        private Guna.UI2.WinForms.Guna2Panel pnlInstructions;
        private System.Windows.Forms.Label lblInstructionTitle;
        private System.Windows.Forms.Label lblInstruction1;
        private System.Windows.Forms.Label lblInstruction2;
        private System.Windows.Forms.Label lblInstruction3;
        private System.Windows.Forms.Label lblInstruction4;
        private System.Windows.Forms.Label lblInstruction5;
        private System.Windows.Forms.Label lblInstruction6;
        private Guna.UI2.WinForms.Guna2Button btnStartRecording;

        private Guna.UI2.WinForms.Guna2Panel loadingPanel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.PictureBox loadingSpinner;
        private System.Windows.Forms.Timer spinnerTimer;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                // ✅ THÊM cleanup cho loading components
                spinnerTimer?.Stop();
                spinnerTimer?.Dispose();
                loadingSpinner?.Image?.Dispose();

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomGestureForm));
            this.lblCustomInfo = new System.Windows.Forms.Label();
            this.lblCustomCount = new System.Windows.Forms.Label();
            this.lblCustomStatus = new System.Windows.Forms.Label();
            this.pictureBoxCustomCamera = new System.Windows.Forms.PictureBox();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.pnlInstructions = new Guna.UI2.WinForms.Guna2Panel();
            this.btnStartRecording = new Guna.UI2.WinForms.Guna2Button();
            this.lblInstructionTitle = new System.Windows.Forms.Label();
            this.lblInstruction1 = new System.Windows.Forms.Label();
            this.lblInstruction6 = new System.Windows.Forms.Label();
            this.lblInstruction2 = new System.Windows.Forms.Label();
            this.lblInstruction3 = new System.Windows.Forms.Label();
            this.lblInstruction5 = new System.Windows.Forms.Label();
            this.lblInstruction4 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.btnHome = new Guna.UI2.WinForms.Guna2Button();
            this.loadingPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.loadingSpinner = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.spinnerTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustomCamera)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.pnlInstructions.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCustomInfo
            // 
            this.lblCustomInfo.AutoSize = true;
            this.lblCustomInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomInfo.Font = new System.Drawing.Font("Segoe UI Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblCustomInfo.Location = new System.Drawing.Point(30, 80);
            this.lblCustomInfo.Name = "lblCustomInfo";
            this.lblCustomInfo.Size = new System.Drawing.Size(217, 31);
            this.lblCustomInfo.TabIndex = 0;
            this.lblCustomInfo.Text = "TARGET GESTURE";
            // 
            // lblCustomCount
            // 
            this.lblCustomCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomCount.AutoSize = true;
            this.lblCustomCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomCount.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblCustomCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblCustomCount.Location = new System.Drawing.Point(893, 40);
            this.lblCustomCount.Name = "lblCustomCount";
            this.lblCustomCount.Size = new System.Drawing.Size(0, 31);
            this.lblCustomCount.TabIndex = 1;
            // 
            // lblCustomStatus
            // 
            this.lblCustomStatus.AutoSize = true;
            this.lblCustomStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomStatus.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomStatus.ForeColor = System.Drawing.Color.White;
            this.lblCustomStatus.Location = new System.Drawing.Point(30, 643);
            this.lblCustomStatus.Name = "lblCustomStatus";
            this.lblCustomStatus.Size = new System.Drawing.Size(152, 31);
            this.lblCustomStatus.TabIndex = 2;
            this.lblCustomStatus.Text = "Trạng thái: ...";
            // 
            // pictureBoxCustomCamera
            // 
            this.pictureBoxCustomCamera.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCustomCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCustomCamera.Location = new System.Drawing.Point(30, 150);
            this.pictureBoxCustomCamera.Name = "pictureBoxCustomCamera";
            this.pictureBoxCustomCamera.Size = new System.Drawing.Size(640, 480);
            this.pictureBoxCustomCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustomCamera.TabIndex = 3;
            this.pictureBoxCustomCamera.TabStop = false;
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlMain.Controls.Add(this.pnlInstructions);
            this.pnlMain.Controls.Add(this.lblCustomCount);
            this.pnlMain.Controls.Add(this.lblName);
            this.pnlMain.Controls.Add(this.btnHome);
            this.pnlMain.Controls.Add(this.lblCustomInfo);
            this.pnlMain.Controls.Add(this.lblCustomStatus);
            this.pnlMain.Controls.Add(this.pictureBoxCustomCamera);
            this.pnlMain.Controls.Add(this.loadingPanel);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.Black;
            this.pnlMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1100, 750);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlInstructions
            // 
            this.pnlInstructions.BackColor = System.Drawing.Color.Transparent;
            this.pnlInstructions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.pnlInstructions.BorderRadius = 15;
            this.pnlInstructions.BorderThickness = 2;
            this.pnlInstructions.Controls.Add(this.btnStartRecording);
            this.pnlInstructions.Controls.Add(this.lblInstructionTitle);
            this.pnlInstructions.Controls.Add(this.lblInstruction1);
            this.pnlInstructions.Controls.Add(this.lblInstruction6);
            this.pnlInstructions.Controls.Add(this.lblInstruction2);
            this.pnlInstructions.Controls.Add(this.lblInstruction3);
            this.pnlInstructions.Controls.Add(this.lblInstruction5);
            this.pnlInstructions.Controls.Add(this.lblInstruction4);
            this.pnlInstructions.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlInstructions.Location = new System.Drawing.Point(680, 150);
            this.pnlInstructions.Name = "pnlInstructions";
            this.pnlInstructions.Size = new System.Drawing.Size(390, 480);
            this.pnlInstructions.TabIndex = 7;
            // 
            // btnStartRecording
            // 
            this.btnStartRecording.BackColor = System.Drawing.Color.Transparent;
            this.btnStartRecording.BorderRadius = 10;
            this.btnStartRecording.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnStartRecording.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnStartRecording.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnStartRecording.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnStartRecording.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnStartRecording.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnStartRecording.ForeColor = System.Drawing.Color.White;
            this.btnStartRecording.Location = new System.Drawing.Point(105, 420);
            this.btnStartRecording.Name = "btnStartRecording";
            this.btnStartRecording.Size = new System.Drawing.Size(180, 35);
            this.btnStartRecording.TabIndex = 8;
            this.btnStartRecording.Text = "🚀 Bắt đầu ghi";
            this.btnStartRecording.Click += new System.EventHandler(this.btnStartRecording_Click);
            // 
            // lblInstructionTitle
            // 
            this.lblInstructionTitle.AutoSize = true;
            this.lblInstructionTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblInstructionTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblInstructionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblInstructionTitle.Location = new System.Drawing.Point(37, 26);
            this.lblInstructionTitle.Name = "lblInstructionTitle";
            this.lblInstructionTitle.Size = new System.Drawing.Size(248, 28);
            this.lblInstructionTitle.TabIndex = 0;
            this.lblInstructionTitle.Text = "📋 Hướng dẫn thực hiện";
            // 
            // lblInstruction1
            // 
            this.lblInstruction1.AutoSize = true;
            this.lblInstruction1.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction1.ForeColor = System.Drawing.Color.White;
            this.lblInstruction1.Location = new System.Drawing.Point(30, 100);
            this.lblInstruction1.Name = "lblInstruction1";
            this.lblInstruction1.Size = new System.Drawing.Size(332, 46);
            this.lblInstruction1.TabIndex = 2;
            this.lblInstruction1.Text = "🎯 Giữ tay trong khung hình cách camera\r\n 40–60 cm";
            // 
            // lblInstruction6
            // 
            this.lblInstruction6.AutoSize = true;
            this.lblInstruction6.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction6.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction6.ForeColor = System.Drawing.Color.White;
            this.lblInstruction6.Location = new System.Drawing.Point(30, 341);
            this.lblInstruction6.Name = "lblInstruction6";
            this.lblInstruction6.Size = new System.Drawing.Size(352, 23);
            this.lblInstruction6.TabIndex = 7;
            this.lblInstruction6.Text = "🔄 Lặp lại 5 lần để đảm bảo chất lượng mẫu";
            // 
            // lblInstruction2
            // 
            this.lblInstruction2.AutoSize = true;
            this.lblInstruction2.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction2.ForeColor = System.Drawing.Color.White;
            this.lblInstruction2.Location = new System.Drawing.Point(30, 157);
            this.lblInstruction2.Name = "lblInstruction2";
            this.lblInstruction2.Size = new System.Drawing.Size(300, 23);
            this.lblInstruction2.TabIndex = 3;
            this.lblInstruction2.Text = "✋ Không để tay ra ngoài mép khung";
            // 
            // lblInstruction3
            // 
            this.lblInstruction3.AutoSize = true;
            this.lblInstruction3.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction3.ForeColor = System.Drawing.Color.White;
            this.lblInstruction3.Location = new System.Drawing.Point(30, 195);
            this.lblInstruction3.Name = "lblInstruction3";
            this.lblInstruction3.Size = new System.Drawing.Size(208, 23);
            this.lblInstruction3.TabIndex = 4;
            this.lblInstruction3.Text = "💡 Đảm bảo đủ ánh sáng";
            // 
            // lblInstruction5
            // 
            this.lblInstruction5.AutoSize = true;
            this.lblInstruction5.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction5.ForeColor = System.Drawing.Color.White;
            this.lblInstruction5.Location = new System.Drawing.Point(30, 277);
            this.lblInstruction5.Name = "lblInstruction5";
            this.lblInstruction5.Size = new System.Drawing.Size(319, 46);
            this.lblInstruction5.TabIndex = 6;
            this.lblInstruction5.Text = "⏱ Giữ tư thế trong 0.8–1.0 giây để mẫu\r\nđược ghi";
            // 
            // lblInstruction4
            // 
            this.lblInstruction4.AutoSize = true;
            this.lblInstruction4.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstruction4.ForeColor = System.Drawing.Color.White;
            this.lblInstruction4.Location = new System.Drawing.Point(30, 236);
            this.lblInstruction4.Name = "lblInstruction4";
            this.lblInstruction4.Size = new System.Drawing.Size(270, 23);
            this.lblInstruction4.TabIndex = 5;
            this.lblInstruction4.Text = "📵 Không để vật thể khác che tay";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Segoe UI Black", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblName.Location = new System.Drawing.Point(30, 40);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(115, 31);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Tên: XYZ";
            // 
            // btnHome
            // 
            this.btnHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.BorderRadius = 10;
            this.btnHome.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHome.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHome.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHome.ForeColor = System.Drawing.Color.Black;
            this.btnHome.Location = new System.Drawing.Point(895, 685);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(175, 40);
            this.btnHome.TabIndex = 4;
            this.btnHome.Text = "Quay Lại";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.loadingPanel.Controls.Add(this.loadingSpinner);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.loadingPanel.Location = new System.Drawing.Point(0, 0);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(1100, 750);
            this.loadingPanel.TabIndex = 8;
            this.loadingPanel.Visible = false;
            // 
            // loadingSpinner
            // 
            this.loadingSpinner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingSpinner.BackColor = System.Drawing.Color.Transparent;
            this.loadingSpinner.Location = new System.Drawing.Point(520, 335);
            this.loadingSpinner.Name = "loadingSpinner";
            this.loadingSpinner.Size = new System.Drawing.Size(60, 60);
            this.loadingSpinner.TabIndex = 0;
            this.loadingSpinner.TabStop = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.loadingLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.loadingLabel.ForeColor = System.Drawing.Color.White;
            this.loadingLabel.Location = new System.Drawing.Point(450, 410);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(127, 32);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "Loading...";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spinnerTimer
            // 
            this.spinnerTimer.Interval = 50;
            this.spinnerTimer.Tick += new System.EventHandler(this.SpinnerTimer_Tick);
            // 
            // FormCustomGesture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F); 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCustomGesture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Gesture Collector";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustomCamera)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlInstructions.ResumeLayout(false);
            this.pnlInstructions.PerformLayout();
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}