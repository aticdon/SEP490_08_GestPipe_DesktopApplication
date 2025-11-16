using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views
{
    partial class RequestGestureForm
    {
        private System.ComponentModel.IContainer components = null;

        // Declare Guna2 controls
        private Guna2Panel mainPanel;
        private Guna2HtmlLabel lblTitle;
        private Guna2Button btnBack;
        private Guna2Button btnStartRequest;

        private Guna2HtmlLabel lblUserValue;
        private Guna2HtmlLabel lblDescriptionValue;
        private Guna2HtmlLabel lblRequestDateValue;
        private Guna2HtmlLabel lblRequestNumberValue;
        private Guna2HtmlLabel lblStatusValue;

        private Guna2HtmlLabel lblUserTitle;
        private Guna2HtmlLabel lblDescriptionTitle;
        private Guna2HtmlLabel lblRequestDateTitle;
        private Guna2HtmlLabel lblRequestNumberTitle;
        private Guna2HtmlLabel lblStatusTitle;

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
            this.mainPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnBack = new Guna.UI2.WinForms.Guna2Button();
            this.btnStartRequest = new Guna.UI2.WinForms.Guna2Button();
            this.lblUserTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblUserValue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDescriptionTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDescriptionValue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRequestDateTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRequestDateValue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRequestNumberTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRequestNumberValue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblStatusTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblStatusValue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.mainPanel.BorderRadius = 15;
            this.mainPanel.Controls.Add(this.lblTitle);
            this.mainPanel.Controls.Add(this.btnBack);
            this.mainPanel.Controls.Add(this.btnStartRequest);
            this.mainPanel.Controls.Add(this.lblUserTitle);
            this.mainPanel.Controls.Add(this.lblUserValue);
            this.mainPanel.Controls.Add(this.lblDescriptionTitle);
            this.mainPanel.Controls.Add(this.lblDescriptionValue);
            this.mainPanel.Controls.Add(this.lblRequestDateTitle);
            this.mainPanel.Controls.Add(this.lblRequestDateValue);
            this.mainPanel.Controls.Add(this.lblRequestNumberTitle);
            this.mainPanel.Controls.Add(this.lblRequestNumberValue);
            this.mainPanel.Controls.Add(this.lblStatusTitle);
            this.mainPanel.Controls.Add(this.lblStatusValue);
            this.mainPanel.FillColor = System.Drawing.Color.Black;
            this.mainPanel.Location = new System.Drawing.Point(50, 50);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.ShadowDecoration.BorderRadius = 15;
            this.mainPanel.ShadowDecoration.Enabled = true;
            this.mainPanel.Size = new System.Drawing.Size(600, 600);
            this.mainPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(175, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(233, 43);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Request Gesture";
            // 
            // btnBack
            // 
            this.btnBack.Animated = true;
            this.btnBack.AutoRoundedCorners = true;
            this.btnBack.BorderRadius = 21;
            this.btnBack.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.Black;
            this.btnBack.Location = new System.Drawing.Point(321, 520);
            this.btnBack.Name = "btnBack";
            this.btnBack.PressedColor = System.Drawing.Color.Red;
            this.btnBack.Size = new System.Drawing.Size(200, 45);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnStartRequest
            // 
            this.btnStartRequest.Animated = true;
            this.btnStartRequest.AutoRoundedCorners = true;
            this.btnStartRequest.BorderRadius = 21;
            this.btnStartRequest.DisabledState.FillColor = System.Drawing.Color.Gray;
            this.btnStartRequest.FillColor = System.Drawing.Color.LemonChiffon;
            this.btnStartRequest.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnStartRequest.ForeColor = System.Drawing.Color.Black;
            this.btnStartRequest.Location = new System.Drawing.Point(72, 520);
            this.btnStartRequest.Name = "btnStartRequest";
            this.btnStartRequest.PressedColor = System.Drawing.Color.Yellow;
            this.btnStartRequest.Size = new System.Drawing.Size(200, 45);
            this.btnStartRequest.TabIndex = 1;
            this.btnStartRequest.Text = "Start Request";
            this.btnStartRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // lblUserTitle
            // 
            this.lblUserTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblUserTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblUserTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblUserTitle.Location = new System.Drawing.Point(50, 150);
            this.lblUserTitle.Name = "lblUserTitle";
            this.lblUserTitle.Size = new System.Drawing.Size(38, 27);
            this.lblUserTitle.TabIndex = 2;
            this.lblUserTitle.Text = "User";
            // 
            // lblUserValue
            // 
            this.lblUserValue.BackColor = System.Drawing.Color.Transparent;
            this.lblUserValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblUserValue.ForeColor = System.Drawing.Color.White;
            this.lblUserValue.Location = new System.Drawing.Point(250, 150);
            this.lblUserValue.Name = "lblUserValue";
            this.lblUserValue.Size = new System.Drawing.Size(38, 27);
            this.lblUserValue.TabIndex = 3;
            this.lblUserValue.Text = "N/A";
            // 
            // lblDescriptionTitle
            // 
            this.lblDescriptionTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblDescriptionTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDescriptionTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblDescriptionTitle.Location = new System.Drawing.Point(50, 200);
            this.lblDescriptionTitle.Name = "lblDescriptionTitle";
            this.lblDescriptionTitle.Size = new System.Drawing.Size(162, 27);
            this.lblDescriptionTitle.TabIndex = 4;
            this.lblDescriptionTitle.Text = "Description Request";
            // 
            // lblDescriptionValue
            // 
            this.lblDescriptionValue.BackColor = System.Drawing.Color.Transparent;
            this.lblDescriptionValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDescriptionValue.ForeColor = System.Drawing.Color.White;
            this.lblDescriptionValue.Location = new System.Drawing.Point(50, 233);
            this.lblDescriptionValue.Name = "lblDescriptionValue";
            this.lblDescriptionValue.Size = new System.Drawing.Size(83, 25);
            this.lblDescriptionValue.TabIndex = 5;
            this.lblDescriptionValue.Text = "Loading...";
            // 
            // lblRequestDateTitle
            // 
            this.lblRequestDateTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblRequestDateTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblRequestDateTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblRequestDateTitle.Location = new System.Drawing.Point(50, 340);
            this.lblRequestDateTitle.Name = "lblRequestDateTitle";
            this.lblRequestDateTitle.Size = new System.Drawing.Size(109, 27);
            this.lblRequestDateTitle.TabIndex = 6;
            this.lblRequestDateTitle.Text = "Request Date";
            // 
            // lblRequestDateValue
            // 
            this.lblRequestDateValue.BackColor = System.Drawing.Color.Transparent;
            this.lblRequestDateValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblRequestDateValue.ForeColor = System.Drawing.Color.White;
            this.lblRequestDateValue.Location = new System.Drawing.Point(250, 340);
            this.lblRequestDateValue.Name = "lblRequestDateValue";
            this.lblRequestDateValue.Size = new System.Drawing.Size(38, 27);
            this.lblRequestDateValue.TabIndex = 7;
            this.lblRequestDateValue.Text = "N/A";
            // 
            // lblRequestNumberTitle
            // 
            this.lblRequestNumberTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblRequestNumberTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblRequestNumberTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblRequestNumberTitle.Location = new System.Drawing.Point(50, 380);
            this.lblRequestNumberTitle.Name = "lblRequestNumberTitle";
            this.lblRequestNumberTitle.Size = new System.Drawing.Size(137, 27);
            this.lblRequestNumberTitle.TabIndex = 8;
            this.lblRequestNumberTitle.Text = "Request Number";
            // 
            // lblRequestNumberValue
            // 
            this.lblRequestNumberValue.BackColor = System.Drawing.Color.Transparent;
            this.lblRequestNumberValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblRequestNumberValue.ForeColor = System.Drawing.Color.White;
            this.lblRequestNumberValue.Location = new System.Drawing.Point(250, 380);
            this.lblRequestNumberValue.Name = "lblRequestNumberValue";
            this.lblRequestNumberValue.Size = new System.Drawing.Size(13, 27);
            this.lblRequestNumberValue.TabIndex = 9;
            this.lblRequestNumberValue.Text = "0";
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblStatusTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusTitle.Location = new System.Drawing.Point(50, 430);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(52, 27);
            this.lblStatusTitle.TabIndex = 10;
            this.lblStatusTitle.Text = "Status";
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblStatusValue.Location = new System.Drawing.Point(250, 430);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(38, 27);
            this.lblStatusValue.TabIndex = 11;
            this.lblStatusValue.Text = "N/A";
            // 
            // FormRequestGestures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(20)))));
            this.BackgroundImage = global::GestPipePowerPonit.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRequestGestures";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Request Gesture Detail";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}