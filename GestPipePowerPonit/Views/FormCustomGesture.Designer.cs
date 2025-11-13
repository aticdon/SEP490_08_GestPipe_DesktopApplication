namespace GestPipePowerPonit
{
    partial class FormCustomGesture
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCustomInfo = new System.Windows.Forms.Label();
            this.lblCustomCount = new System.Windows.Forms.Label();
            this.lblCustomStatus = new System.Windows.Forms.Label();
            this.pictureBoxCustomCamera = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustomCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCustomInfo
            // 
            this.lblCustomInfo.AutoSize = true;
            this.lblCustomInfo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCustomInfo.Location = new System.Drawing.Point(30, 20);
            this.lblCustomInfo.Name = "lblCustomInfo";
            this.lblCustomInfo.Size = new System.Drawing.Size(106, 20);
            this.lblCustomInfo.TabIndex = 0;
            this.lblCustomInfo.Text = "User/Pose info";
            // 
            // lblCustomCount
            // 
            this.lblCustomCount.AutoSize = true;
            this.lblCustomCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCustomCount.Location = new System.Drawing.Point(30, 50);
            this.lblCustomCount.Name = "lblCustomCount";
            this.lblCustomCount.Size = new System.Drawing.Size(111, 20);
            this.lblCustomCount.TabIndex = 1;
            this.lblCustomCount.Text = "Đã ghi: 0/5 (mẫu)";
            // 
            // lblCustomStatus
            // 
            this.lblCustomStatus.AutoSize = true;
            this.lblCustomStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCustomStatus.Location = new System.Drawing.Point(30, 80);
            this.lblCustomStatus.Name = "lblCustomStatus";
            this.lblCustomStatus.Size = new System.Drawing.Size(97, 20);
            this.lblCustomStatus.TabIndex = 2;
            this.lblCustomStatus.Text = "Trạng thái: ...";
            // 
            // pictureBoxCustomCamera
            // 
            this.pictureBoxCustomCamera.BackColor = System.Drawing.Color.Silver;
            this.pictureBoxCustomCamera.Location = new System.Drawing.Point(30, 120);
            this.pictureBoxCustomCamera.Name = "pictureBoxCustomCamera";
            this.pictureBoxCustomCamera.Size = new System.Drawing.Size(640, 480);
            this.pictureBoxCustomCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustomCamera.TabIndex = 3;
            this.pictureBoxCustomCamera.TabStop = false;
            // 
            // btnHome
            // 
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnHome.Location = new System.Drawing.Point(570, 30);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(100, 40);
            this.btnHome.TabIndex = 4;
            this.btnHome.Text = "Về trang chủ";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // FormCustomGesture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 640);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.pictureBoxCustomCamera);
            this.Controls.Add(this.lblCustomStatus);
            this.Controls.Add(this.lblCustomCount);
            this.Controls.Add(this.lblCustomInfo);
            this.Name = "FormCustomGesture";
            this.Text = "Custom Gesture Collector";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustomCamera)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomInfo;
        private System.Windows.Forms.Label lblCustomCount;
        private System.Windows.Forms.Label lblCustomStatus;
        private System.Windows.Forms.PictureBox pictureBoxCustomCamera;
        private System.Windows.Forms.Button btnHome;
    }
}