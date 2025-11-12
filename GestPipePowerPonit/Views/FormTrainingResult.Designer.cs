namespace GestPipePowerPonit
{
    partial class FormTrainingResult
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Guna Controls and Standard Labels
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private System.Windows.Forms.Label lblNameGesture;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label lblAccuracyTraining;
        private System.Windows.Forms.Label lblTrainingDay;
        private System.Windows.Forms.Label lblValueName;
        private System.Windows.Forms.Label lblValueAction;
        private System.Windows.Forms.Label lblValueAccuracy;
        private System.Windows.Forms.Label lblValueDay;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrainingResult));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblNameGesture = new System.Windows.Forms.Label();
            this.lblValueName = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.lblValueAction = new System.Windows.Forms.Label();
            this.lblAccuracyTraining = new System.Windows.Forms.Label();
            this.lblValueAccuracy = new System.Windows.Forms.Label();
            this.lblTrainingDay = new System.Windows.Forms.Label();
            this.lblValueDay = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Controls.Add(this.lblNameGesture);
            this.pnlMain.Controls.Add(this.lblValueName);
            this.pnlMain.Controls.Add(this.lblAction);
            this.pnlMain.Controls.Add(this.lblValueAction);
            this.pnlMain.Controls.Add(this.lblAccuracyTraining);
            this.pnlMain.Controls.Add(this.lblValueAccuracy);
            this.pnlMain.Controls.Add(this.lblTrainingDay);
            this.pnlMain.Controls.Add(this.lblValueDay);
            this.pnlMain.Controls.Add(this.btnClose);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.Black;
            this.pnlMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(600, 500);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Black", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblTitle.Location = new System.Drawing.Point(57, 23);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 57);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Training results";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNameGesture
            // 
            this.lblNameGesture.BackColor = System.Drawing.Color.Transparent;
            this.lblNameGesture.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblNameGesture.ForeColor = System.Drawing.Color.Cyan;
            this.lblNameGesture.Location = new System.Drawing.Point(31, 120);
            this.lblNameGesture.Name = "lblNameGesture";
            this.lblNameGesture.Size = new System.Drawing.Size(257, 60);
            this.lblNameGesture.TabIndex = 1;
            this.lblNameGesture.Text = "Name Gesture:";
            // 
            // lblValueName
            // 
            this.lblValueName.BackColor = System.Drawing.Color.Transparent;
            this.lblValueName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueName.ForeColor = System.Drawing.Color.Cyan;
            this.lblValueName.Location = new System.Drawing.Point(353, 120);
            this.lblValueName.Name = "lblValueName";
            this.lblValueName.Size = new System.Drawing.Size(247, 60);
            this.lblValueName.TabIndex = 2;
            this.lblValueName.Text = "Next Slide";
            // 
            // lblAction
            // 
            this.lblAction.BackColor = System.Drawing.Color.Transparent;
            this.lblAction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblAction.ForeColor = System.Drawing.Color.White;
            this.lblAction.Location = new System.Drawing.Point(31, 180);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(257, 60);
            this.lblAction.TabIndex = 3;
            this.lblAction.Text = "Action:";
            // 
            // lblValueAction
            // 
            this.lblValueAction.BackColor = System.Drawing.Color.Transparent;
            this.lblValueAction.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblValueAction.ForeColor = System.Drawing.Color.White;
            this.lblValueAction.Location = new System.Drawing.Point(353, 180);
            this.lblValueAction.Name = "lblValueAction";
            this.lblValueAction.Size = new System.Drawing.Size(244, 23);
            this.lblValueAction.TabIndex = 4;
            this.lblValueAction.Text = "next_slide";
            // 
            // lblAccuracyTraining
            // 
            this.lblAccuracyTraining.BackColor = System.Drawing.Color.Transparent;
            this.lblAccuracyTraining.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblAccuracyTraining.ForeColor = System.Drawing.Color.Cyan;
            this.lblAccuracyTraining.Location = new System.Drawing.Point(31, 240);
            this.lblAccuracyTraining.Name = "lblAccuracyTraining";
            this.lblAccuracyTraining.Size = new System.Drawing.Size(257, 60);
            this.lblAccuracyTraining.TabIndex = 5;
            this.lblAccuracyTraining.Text = "Accuracy Training:";
            // 
            // lblValueAccuracy
            // 
            this.lblValueAccuracy.BackColor = System.Drawing.Color.Transparent;
            this.lblValueAccuracy.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblValueAccuracy.ForeColor = System.Drawing.Color.Cyan;
            this.lblValueAccuracy.Location = new System.Drawing.Point(353, 240);
            this.lblValueAccuracy.Name = "lblValueAccuracy";
            this.lblValueAccuracy.Size = new System.Drawing.Size(247, 23);
            this.lblValueAccuracy.TabIndex = 6;
            this.lblValueAccuracy.Text = "96.8%";
            // 
            // lblTrainingDay
            // 
            this.lblTrainingDay.BackColor = System.Drawing.Color.Transparent;
            this.lblTrainingDay.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTrainingDay.ForeColor = System.Drawing.Color.White;
            this.lblTrainingDay.Location = new System.Drawing.Point(31, 300);
            this.lblTrainingDay.Name = "lblTrainingDay";
            this.lblTrainingDay.Size = new System.Drawing.Size(257, 72);
            this.lblTrainingDay.TabIndex = 7;
            this.lblTrainingDay.Text = "Training Day:";
            // 
            // lblValueDay
            // 
            this.lblValueDay.BackColor = System.Drawing.Color.Transparent;
            this.lblValueDay.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblValueDay.ForeColor = System.Drawing.Color.White;
            this.lblValueDay.Location = new System.Drawing.Point(353, 300);
            this.lblValueDay.Name = "lblValueDay";
            this.lblValueDay.Size = new System.Drawing.Size(247, 23);
            this.lblValueDay.TabIndex = 8;
            this.lblValueDay.Text = "30-09-2025";
            // 
            // btnClose
            // 
            this.btnClose.BorderRadius = 10;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(236, 415);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 45);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormTrainingResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTrainingResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTrainingResult";
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}