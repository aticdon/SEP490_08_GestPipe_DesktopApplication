using Guna.UI2.WinForms;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    partial class TrainingDefaultGestureForm
    {
        private System.ComponentModel.IContainer components = null;

        // Guna Controls - Tích hợp từ Form1
        //private Guna.UI2.WinForms.Guna2Panel pnlSidebar, pnlFooter;
        //private System.Windows.Forms.PictureBox pictureBoxLogo;
        //private Guna2Panel pnlHeader;
        private System.Windows.Forms.Label lblTrainingTitle; // Tiêu đề cho Header
        private Guna2GradientPanel pnlMain; // Panel chứa nội dung luyện tập
        //private Guna2ControlBox guna2ControlBoxMinimize;
        //private Guna2ControlBox guna2ControlBoxClose;
        private Guna2Elipse guna2Elipse1;
        private Guna2DragControl guna2DragControl1;
        //private Guna.UI2.WinForms.Guna2Button btnCustomeGesture, btnInstruction, btnVersion, btnGestureControl, btnHome;
        //private Guna.UI2.WinForms.Guna2Button btnLanguageVN;
        //private Guna.UI2.WinForms.Guna2Button btnLanguageEN;

        // Các Control luyện tập gốc, được thêm vào pnlMain
        private System.Windows.Forms.PictureBox picCamera;
        private System.Windows.Forms.Label lblCorrect, lblWrong, lblAccuracy, lblResult;
        private Guna.UI2.WinForms.Guna2Panel loadingPanel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.PictureBox loadingSpinner;
        private System.Windows.Forms.Timer spinnerTimer;


        protected override void Dispose(bool disposing)
        {
            //if (disposing && (components != null)) components.Dispose();
            //base.Dispose(disposing);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainingDefaultGestureForm));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.lblTrainingTitle = new System.Windows.Forms.Label();
            this.btnPresentation = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTraining = new System.Windows.Forms.Label();
            this.lblGestureName = new System.Windows.Forms.Label();
            this.btnEndTraining = new Guna.UI2.WinForms.Guna2Button();
            this.picCamera = new System.Windows.Forms.PictureBox();
            this.lblPose = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblCorrect = new System.Windows.Forms.Label();
            this.lblWrong = new System.Windows.Forms.Label();
            this.lblAccuracy = new System.Windows.Forms.Label();
            this.lblReason = new System.Windows.Forms.Label();
            this.loadingPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.loadingSpinner = new System.Windows.Forms.PictureBox();
            this.spinnerTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).BeginInit();
            this.loadingPanel.SuspendLayout();
            this.SuspendLayout();
            // ✅ THÊM loading panel setup
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.loadingPanel.BorderRadius = 0;
            this.loadingPanel.Controls.Add(this.loadingSpinner);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.None;
            this.loadingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.loadingPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.loadingPanel.Location = new System.Drawing.Point(0, 0);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(1100, 750);
            this.loadingPanel.TabIndex = 999;
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
            this.loadingLabel.Size = new System.Drawing.Size(200, 32);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "Loading...";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spinnerTimer
            // 
            this.spinnerTimer.Interval = 50;
            this.spinnerTimer.Tick += new System.EventHandler(this.SpinnerTimer_Tick);

            // ✅ THÊM loadingPanel vào pnlMain CUỐI CÙNG
            this.pnlMain.Controls.Add(this.loadingPanel);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // lblTrainingTitle
            // 
            this.lblTrainingTitle.AutoSize = true;
            this.lblTrainingTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTrainingTitle.ForeColor = System.Drawing.Color.White;
            this.lblTrainingTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTrainingTitle.Name = "lblTrainingTitle";
            this.lblTrainingTitle.Size = new System.Drawing.Size(100, 23);
            this.lblTrainingTitle.TabIndex = 0;
            this.lblTrainingTitle.Text = "Gesture Training Session";
            // 
            // btnPresentation
            // 
            this.btnPresentation.BorderRadius = 10;
            this.btnPresentation.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPresentation.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPresentation.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnPresentation.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPresentation.ForeColor = System.Drawing.Color.White;
            this.btnPresentation.Image = global::GestPipePowerPonit.Properties.Resources.icon_search;
            this.btnPresentation.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPresentation.ImageSize = new System.Drawing.Size(24, 24);
            this.btnPresentation.Location = new System.Drawing.Point(16, 493);
            this.btnPresentation.Margin = new System.Windows.Forms.Padding(4);
            this.btnPresentation.Name = "btnPresentation";
            this.btnPresentation.Size = new System.Drawing.Size(233, 55);
            this.btnPresentation.TabIndex = 11;
            this.btnPresentation.Text = "Presentation";
            this.btnPresentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlMain.Controls.Add(this.lblTraining);
            this.pnlMain.Controls.Add(this.lblGestureName);
            this.pnlMain.Controls.Add(this.btnEndTraining);
            this.pnlMain.Controls.Add(this.picCamera);
            this.pnlMain.Controls.Add(this.lblPose);
            this.pnlMain.Controls.Add(this.lblResult);
            this.pnlMain.Controls.Add(this.lblCorrect);
            this.pnlMain.Controls.Add(this.lblWrong);
            this.pnlMain.Controls.Add(this.lblAccuracy);
            this.pnlMain.Controls.Add(this.lblReason);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.Black;
            this.pnlMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1100, 750);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTraining
            // 
            this.lblTraining.AutoSize = true;
            this.lblTraining.Font = new System.Drawing.Font("Segoe UI Black", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTraining.ForeColor = System.Drawing.Color.Cyan;
            this.lblTraining.Location = new System.Drawing.Point(209, 43);
            this.lblTraining.Name = "lblTraining";
            this.lblTraining.Size = new System.Drawing.Size(681, 81);
            this.lblTraining.TabIndex = 9;
            this.lblTraining.Text = "User Training Gesture";
            // 
            // lblGestureName
            // 
            this.lblGestureName.AutoSize = true;
            this.lblGestureName.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGestureName.ForeColor = System.Drawing.Color.White;
            this.lblGestureName.Location = new System.Drawing.Point(701, 172);
            this.lblGestureName.Name = "lblGestureName";
            this.lblGestureName.Size = new System.Drawing.Size(171, 31);
            this.lblGestureName.TabIndex = 8;
            this.lblGestureName.Text = "Gesture Name:";
            // 
            // btnEndTraining
            // 
            this.btnEndTraining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEndTraining.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnEndTraining.BorderRadius = 10;
            this.btnEndTraining.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnEndTraining.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEndTraining.ForeColor = System.Drawing.Color.Black;
            this.btnEndTraining.Location = new System.Drawing.Point(918, 680);
            this.btnEndTraining.Name = "btnEndTraining";
            this.btnEndTraining.Size = new System.Drawing.Size(143, 40);
            this.btnEndTraining.TabIndex = 7;
            this.btnEndTraining.Text = "End Training";
            this.btnEndTraining.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // picCamera
            // 
            this.picCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(40)))));
            this.picCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCamera.Location = new System.Drawing.Point(25, 163);
            this.picCamera.Name = "picCamera";
            this.picCamera.Size = new System.Drawing.Size(640, 480);
            this.picCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCamera.TabIndex = 0;
            this.picCamera.TabStop = false;
            // 
            // lblPose
            // 
            this.lblPose.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPose.ForeColor = System.Drawing.Color.White;
            this.lblPose.Location = new System.Drawing.Point(702, 232);
            this.lblPose.Name = "lblPose";
            this.lblPose.Size = new System.Drawing.Size(359, 24);
            this.lblPose.TabIndex = 1;
            this.lblPose.Text = "🎯 Pose Target: [Gesture]";
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblResult.ForeColor = System.Drawing.Color.White;
            this.lblResult.Location = new System.Drawing.Point(702, 292);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(220, 24);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "✅ Last Result: ";
            // 
            // lblCorrect
            // 
            this.lblCorrect.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCorrect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.lblCorrect.Location = new System.Drawing.Point(703, 352);
            this.lblCorrect.Name = "lblCorrect";
            this.lblCorrect.Size = new System.Drawing.Size(150, 24);
            this.lblCorrect.TabIndex = 3;
            this.lblCorrect.Text = "✅ Correct: ";
            // 
            // lblWrong
            // 
            this.lblWrong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWrong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblWrong.Location = new System.Drawing.Point(703, 412);
            this.lblWrong.Name = "lblWrong";
            this.lblWrong.Size = new System.Drawing.Size(150, 24);
            this.lblWrong.TabIndex = 4;
            this.lblWrong.Text = "❌ Wrong: ";
            // 
            // lblAccuracy
            // 
            this.lblAccuracy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAccuracy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblAccuracy.Location = new System.Drawing.Point(703, 472);
            this.lblAccuracy.Name = "lblAccuracy";
            this.lblAccuracy.Size = new System.Drawing.Size(292, 24);
            this.lblAccuracy.TabIndex = 5;
            this.lblAccuracy.Text = "📊 Accuracy:";
            // 
            // lblReason
            // 
            this.lblReason.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReason.ForeColor = System.Drawing.Color.Wheat;
            this.lblReason.Location = new System.Drawing.Point(701, 553);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(350, 90);
            this.lblReason.TabIndex = 6;
            this.lblReason.Text = "Reason:";
            // 
            // FormTrainingGesture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTrainingGesture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.loadingPanel.ResumeLayout(false); // ✅ THÊM
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPose;
        private System.Windows.Forms.Label lblReason;
        private Guna2Button btnEndTraining;
        private System.Windows.Forms.Label lblGestureName;
        private Guna2Button btnPresentation;
        private System.Windows.Forms.Label lblTraining;
    }
}