using Guna.UI2.WinForms;
using System.Windows.Forms;
using System.Drawing;

namespace GestPipePowerPonit.Views.Auth
{
    partial class VerifyOtpForm
    {
        private System.ComponentModel.IContainer components = null;

        // ✅ Variable declarations
        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtOtp;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblOtpError;  // ✅ THÊM
        private System.Windows.Forms.Label lblTimer;           // ✅ THÊM: dùng Label thay vì Guna2HtmlLabel
        private Guna.UI2.WinForms.Guna2GradientButton btnVerify;
        private System.Windows.Forms.LinkLabel lnkResend;
        private System.Windows.Forms.PictureBox picLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerifyOtpForm));
            this.pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lnkLogin = new System.Windows.Forms.LinkLabel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtOtp = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblOtpError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnVerify = new Guna.UI2.WinForms.Guna2GradientButton();
            this.lnkResend = new System.Windows.Forms.LinkLabel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlCard.BorderRadius = 20;
            this.pnlCard.Controls.Add(this.lnkLogin);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.txtOtp);
            this.pnlCard.Controls.Add(this.lblOtpError);
            this.pnlCard.Controls.Add(this.lblTimer);
            this.pnlCard.Controls.Add(this.btnVerify);
            this.pnlCard.Controls.Add(this.lnkResend);
            this.pnlCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlCard.Location = new System.Drawing.Point(458, 209);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(450, 350);
            this.pnlCard.TabIndex = 1;
            // 
            // lnkLogin
            // 
            this.lnkLogin.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lnkLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkLogin.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(180)))), ((int)(((byte)(218)))));
            this.lnkLogin.Location = new System.Drawing.Point(0, 274);
            this.lnkLogin.Name = "lnkLogin";
            this.lnkLogin.Size = new System.Drawing.Size(450, 25);
            this.lnkLogin.TabIndex = 13;
            this.lnkLogin.TabStop = true;
            this.lnkLogin.Text = "Login?";
            this.lnkLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkLogin.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLogin_LinkClicked);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblTitle.Location = new System.Drawing.Point(0, -16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(450, 101);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Verify OTP";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtOtp
            // 
            this.txtOtp.Animated = true;
            this.txtOtp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtOtp.BorderRadius = 10;
            this.txtOtp.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOtp.DefaultText = "";
            this.txtOtp.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtOtp.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtOtp.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtOtp.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtOtp.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtOtp.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtOtp.ForeColor = System.Drawing.Color.Black;
            this.txtOtp.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtOtp.Location = new System.Drawing.Point(40, 120);
            this.txtOtp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOtp.MaxLength = 6;
            this.txtOtp.Name = "txtOtp";
            this.txtOtp.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtOtp.PlaceholderText = "Enter 6-digit OTP";
            this.txtOtp.SelectedText = "";
            this.txtOtp.Size = new System.Drawing.Size(370, 40);
            this.txtOtp.TabIndex = 1;
            this.txtOtp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtOtp.TextChanged += new System.EventHandler(this.txtOtp_TextChanged);
            // 
            // lblOtpError
            // 
            this.lblOtpError.BackColor = System.Drawing.Color.Transparent;
            this.lblOtpError.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOtpError.ForeColor = System.Drawing.Color.Wheat;
            this.lblOtpError.Location = new System.Drawing.Point(45, 163);
            this.lblOtpError.Name = "lblOtpError";
            this.lblOtpError.Size = new System.Drawing.Size(3, 2);
            this.lblOtpError.TabIndex = 2;
            this.lblOtpError.Text = null;
            this.lblOtpError.Visible = false;
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.Color.Transparent;
            this.lblTimer.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTimer.ForeColor = System.Drawing.Color.White;
            this.lblTimer.Location = new System.Drawing.Point(39, 89);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(370, 26);
            this.lblTimer.TabIndex = 6;
            this.lblTimer.Text = "05:00";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnVerify
            // 
            this.btnVerify.Animated = true;
            this.btnVerify.BorderRadius = 10;
            this.btnVerify.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerify.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnVerify.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnVerify.FillColor2 = System.Drawing.SystemColors.Highlight;
            this.btnVerify.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnVerify.ForeColor = System.Drawing.Color.White;
            this.btnVerify.HoverState.FillColor = System.Drawing.SystemColors.Highlight;
            this.btnVerify.HoverState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnVerify.Location = new System.Drawing.Point(119, 201);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(215, 45);
            this.btnVerify.TabIndex = 3;
            this.btnVerify.Text = "Verify";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // lnkResend
            // 
            this.lnkResend.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lnkResend.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkResend.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(180)))), ((int)(((byte)(218)))));
            this.lnkResend.Location = new System.Drawing.Point(40, 249);
            this.lnkResend.Name = "lnkResend";
            this.lnkResend.Size = new System.Drawing.Size(370, 25);
            this.lnkResend.TabIndex = 4;
            this.lnkResend.TabStop = true;
            this.lnkResend.Text = "Resend OTP";
            this.lnkResend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkResend.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkResend.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResend_LinkClicked);
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Location = new System.Drawing.Point(458, 53);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(450, 117);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1304, 12);
            this.guna2ControlBoxClose.Name = "guna2ControlBoxClose";
            this.guna2ControlBoxClose.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxClose.TabIndex = 11;
            this.guna2ControlBoxClose.Click += new System.EventHandler(this.guna2ControlBoxClose_Click);
            // 
            // guna2ControlBoxMinimize
            // 
            this.guna2ControlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMinimize.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(1248, 12);
            this.guna2ControlBoxMinimize.Name = "guna2ControlBoxMinimize";
            this.guna2ControlBoxMinimize.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxMinimize.TabIndex = 12;
            // 
            // VerifyOtpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.guna2ControlBoxClose);
            this.Controls.Add(this.guna2ControlBoxMinimize);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.pnlCard);
            this.DoubleBuffered = true;
            //this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "VerifyOtpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Verify OTP";
            this.Load += new System.EventHandler(this.VerifyOtpForm_Load);
            this.Resize += new System.EventHandler(this.VerifyOtpForm_Resize);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        private Guna2ControlBox guna2ControlBoxClose;
        private Guna2ControlBox guna2ControlBoxMinimize;
        private LinkLabel lnkLogin;
    }
}