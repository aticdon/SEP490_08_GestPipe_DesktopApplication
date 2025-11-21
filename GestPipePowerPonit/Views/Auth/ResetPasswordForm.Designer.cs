// Add these 'using' statements at the top
using Guna.UI2.WinForms;
using System.Windows.Forms;
using System.Drawing;

namespace GestPipePowerPonit.Views.Auth
{
    partial class ResetPasswordForm
    {
        private System.ComponentModel.IContainer components = null;

        // ✅ Variable declarations
        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtNewPassword;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNewPasswordError;      // ✅ THÊM
        private Guna.UI2.WinForms.Guna2TextBox txtConfirmPassword;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblConfirmPasswordError;  // ✅ THÊM
        private Guna.UI2.WinForms.Guna2GradientButton btnResetPassword;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.LinkLabel lnkLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResetPasswordForm));
            this.pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lnkLogin = new System.Windows.Forms.LinkLabel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtNewPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblNewPasswordError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtConfirmPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblConfirmPasswordError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnResetPassword = new Guna.UI2.WinForms.Guna2GradientButton();
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
            this.pnlCard.Controls.Add(this.txtNewPassword);
            this.pnlCard.Controls.Add(this.lblNewPasswordError);
            this.pnlCard.Controls.Add(this.txtConfirmPassword);
            this.pnlCard.Controls.Add(this.lblConfirmPasswordError);
            this.pnlCard.Controls.Add(this.btnResetPassword);
            this.pnlCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlCard.Location = new System.Drawing.Point(458, 179);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(450, 410);
            this.pnlCard.TabIndex = 1;
            // 
            // lnkLogin
            // 
            this.lnkLogin.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lnkLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkLogin.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(180)))), ((int)(((byte)(218)))));
            this.lnkLogin.Location = new System.Drawing.Point(40, 330);
            this.lnkLogin.Name = "lnkLogin";
            this.lnkLogin.Size = new System.Drawing.Size(370, 25);
            this.lnkLogin.TabIndex = 6;
            this.lnkLogin.TabStop = true;
            this.lnkLogin.Text = "Back to Login";
            this.lnkLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkLogin.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLogin_LinkClicked);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(450, 70);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Reset Password";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Animated = true;
            this.txtNewPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtNewPassword.BorderRadius = 10;
            this.txtNewPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNewPassword.DefaultText = "";
            this.txtNewPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtNewPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtNewPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNewPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNewPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtNewPassword.ForeColor = System.Drawing.Color.Black;
            this.txtNewPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtNewPassword.IconRight = global::GestPipePowerPonit.Properties.Resources.eye;
            this.txtNewPassword.IconRightCursor = System.Windows.Forms.Cursors.Hand;
            this.txtNewPassword.Location = new System.Drawing.Point(40, 130);
            this.txtNewPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtNewPassword.PlaceholderText = "New Password";
            this.txtNewPassword.SelectedText = "";
            this.txtNewPassword.Size = new System.Drawing.Size(370, 40);
            this.txtNewPassword.TabIndex = 1;
            this.txtNewPassword.IconRightClick += new System.EventHandler(this.txtNewPassword_IconRightClick);
            this.txtNewPassword.TextChanged += new System.EventHandler(this.txtNewPassword_TextChanged);
            // 
            // lblNewPasswordError
            // 
            this.lblNewPasswordError.BackColor = System.Drawing.Color.Transparent;
            this.lblNewPasswordError.ForeColor = System.Drawing.Color.Wheat;
            this.lblNewPasswordError.Location = new System.Drawing.Point(45, 173);
            this.lblNewPasswordError.Name = "lblNewPasswordError";
            this.lblNewPasswordError.Size = new System.Drawing.Size(3, 2);
            this.lblNewPasswordError.TabIndex = 2;
            this.lblNewPasswordError.Text = null;
            this.lblNewPasswordError.Visible = false;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Animated = true;
            this.txtConfirmPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtConfirmPassword.BorderRadius = 10;
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.DefaultText = "";
            this.txtConfirmPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtConfirmPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtConfirmPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtConfirmPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtConfirmPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtConfirmPassword.ForeColor = System.Drawing.Color.Black;
            this.txtConfirmPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtConfirmPassword.IconRight = global::GestPipePowerPonit.Properties.Resources.eye;
            this.txtConfirmPassword.IconRightCursor = System.Windows.Forms.Cursors.Hand;
            this.txtConfirmPassword.Location = new System.Drawing.Point(40, 197);
            this.txtConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtConfirmPassword.PlaceholderText = "Confirm Password";
            this.txtConfirmPassword.SelectedText = "";
            this.txtConfirmPassword.Size = new System.Drawing.Size(370, 40);
            this.txtConfirmPassword.TabIndex = 3;
            this.txtConfirmPassword.IconRightClick += new System.EventHandler(this.txtConfirmPassword_IconRightClick);
            this.txtConfirmPassword.TextChanged += new System.EventHandler(this.txtConfirmPassword_TextChanged);
            // 
            // lblConfirmPasswordError
            // 
            this.lblConfirmPasswordError.BackColor = System.Drawing.Color.Transparent;
            this.lblConfirmPasswordError.ForeColor = System.Drawing.Color.Wheat;
            this.lblConfirmPasswordError.Location = new System.Drawing.Point(45, 240);
            this.lblConfirmPasswordError.Name = "lblConfirmPasswordError";
            this.lblConfirmPasswordError.Size = new System.Drawing.Size(3, 2);
            this.lblConfirmPasswordError.TabIndex = 4;
            this.lblConfirmPasswordError.Text = null;
            this.lblConfirmPasswordError.Visible = false;
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Animated = true;
            this.btnResetPassword.BorderRadius = 10;
            this.btnResetPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnResetPassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnResetPassword.FillColor2 = System.Drawing.Color.MidnightBlue;
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(150)))), ((int)(((byte)(200)))));
            this.btnResetPassword.Location = new System.Drawing.Point(113, 270);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(231, 45);
            this.btnResetPassword.TabIndex = 5;
            this.btnResetPassword.Text = "Reset Password";
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Location = new System.Drawing.Point(458, 43);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(450, 130);
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
            // ResetPasswordForm
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
            this.Name = "ResetPasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reset Password";
            this.Load += new System.EventHandler(this.ResetPasswordForm_Load);
            this.Resize += new System.EventHandler(this.ResetPasswordForm_Resize);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        private Guna2ControlBox guna2ControlBoxClose;
        private Guna2ControlBox guna2ControlBoxMinimize;
    }
}