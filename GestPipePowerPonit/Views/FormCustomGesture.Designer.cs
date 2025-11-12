using System.Drawing;
using System.Windows.Forms;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomGesture));
            this.txtGestureName = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.panelImage = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pictureBoxCustom = new System.Windows.Forms.PictureBox();
            this.lblState = new System.Windows.Forms.Label();
            this.lblPose = new System.Windows.Forms.Label();
            this.lblSaved = new System.Windows.Forms.Label();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGestureName
            // 
            this.txtGestureName.Location = new System.Drawing.Point(12, 12);
            this.txtGestureName.Name = "txtGestureName";
            this.txtGestureName.Size = new System.Drawing.Size(699, 22);
            this.txtGestureName.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(737, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            // 
            // panelImage
            // 
            this.panelImage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelImage.Controls.Add(this.btnClose);
            this.panelImage.Controls.Add(this.pictureBoxCustom);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(0, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(1280, 1000);
            this.panelImage.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(818, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBoxCustom
            // 
            this.pictureBoxCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCustom.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCustom.Name = "pictureBoxCustom";
            this.pictureBoxCustom.Size = new System.Drawing.Size(1280, 1000);
            this.pictureBoxCustom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom.TabIndex = 0;
            this.pictureBoxCustom.TabStop = false;
            // 
            // lblState
            // 
            this.lblState.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblState.Location = new System.Drawing.Point(12, 50);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(400, 30);
            this.lblState.TabIndex = 2;
            // 
            // lblPose
            // 
            this.lblPose.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblPose.Location = new System.Drawing.Point(12, 85);
            this.lblPose.Name = "lblPose";
            this.lblPose.Size = new System.Drawing.Size(400, 30);
            this.lblPose.TabIndex = 3;
            // 
            // lblSaved
            // 
            this.lblSaved.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblSaved.Location = new System.Drawing.Point(12, 120);
            this.lblSaved.Name = "lblSaved";
            this.lblSaved.Size = new System.Drawing.Size(400, 30);
            this.lblSaved.TabIndex = 4;
            // 
            // FormCustomGesture
            // 
            this.ClientSize = new System.Drawing.Size(1280, 1000);
            this.Controls.Add(this.txtGestureName);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.lblPose);
            this.Controls.Add(this.lblSaved);
            this.Controls.Add(this.panelImage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCustomGesture";
            this.Text = "FormCustomGesture";
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGestureName;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pictureBoxCustom;
        private System.Windows.Forms.Panel panelImage;
        private Label lblState;
        private Label lblPose;
        private Label lblSaved;
        private Button btnClose;
    }
}