namespace GestPipePowerPonit.Views.Auth
{
    partial class TermsConditionsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermsConditionsForm));
            this.pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTerms = new System.Windows.Forms.RichTextBox();
            this.pnlOptions = new Guna.UI2.WinForms.Guna2Panel();
            this.rbAgree = new Guna.UI2.WinForms.Guna2RadioButton();
            this.rbDisagree = new Guna.UI2.WinForms.Guna2RadioButton();
            this.btnNext = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlCard.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlCard.BorderRadius = 20;
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.txtTerms);
            this.pnlCard.Controls.Add(this.pnlOptions);
            this.pnlCard.Controls.Add(this.btnNext);
            this.pnlCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlCard.Location = new System.Drawing.Point(183, 60);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(1000, 650);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(940, 68);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Terms & Conditions";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtTerms
            // 
            this.txtTerms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtTerms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTerms.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTerms.ForeColor = System.Drawing.Color.Bisque;
            this.txtTerms.Location = new System.Drawing.Point(30, 90);
            this.txtTerms.Name = "txtTerms";
            this.txtTerms.ReadOnly = true;
            this.txtTerms.Size = new System.Drawing.Size(940, 400);
            this.txtTerms.TabIndex = 1;
            this.txtTerms.Text = resources.GetString("txtTerms.Text");
            // 
            // pnlOptions
            // 
            this.pnlOptions.BackColor = System.Drawing.Color.Transparent;
            this.pnlOptions.BorderRadius = 10;
            this.pnlOptions.BorderThickness = 2;
            this.pnlOptions.Controls.Add(this.rbAgree);
            this.pnlOptions.Controls.Add(this.rbDisagree);
            this.pnlOptions.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlOptions.Location = new System.Drawing.Point(30, 513);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(569, 107);
            this.pnlOptions.TabIndex = 2;
            // 
            // rbAgree
            // 
            this.rbAgree.AutoSize = true;
            this.rbAgree.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.rbAgree.CheckedState.BorderThickness = 0;
            this.rbAgree.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.rbAgree.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbAgree.CheckedState.InnerOffset = -4;
            this.rbAgree.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.rbAgree.ForeColor = System.Drawing.Color.White;
            this.rbAgree.Location = new System.Drawing.Point(25, 20);
            this.rbAgree.Name = "rbAgree";
            this.rbAgree.Size = new System.Drawing.Size(98, 32);
            this.rbAgree.TabIndex = 0;
            this.rbAgree.Text = "I agree";
            this.rbAgree.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbAgree.UncheckedState.BorderThickness = 2;
            this.rbAgree.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbAgree.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbAgree.CheckedChanged += new System.EventHandler(this.rbAgree_CheckedChanged);
            // 
            // rbDisagree
            // 
            this.rbDisagree.AutoSize = true;
            this.rbDisagree.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.rbDisagree.CheckedState.BorderThickness = 0;
            this.rbDisagree.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.rbDisagree.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbDisagree.CheckedState.InnerOffset = -4;
            this.rbDisagree.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.rbDisagree.ForeColor = System.Drawing.Color.White;
            this.rbDisagree.Location = new System.Drawing.Point(25, 65);
            this.rbDisagree.Name = "rbDisagree";
            this.rbDisagree.Size = new System.Drawing.Size(125, 32);
            this.rbDisagree.TabIndex = 1;
            this.rbDisagree.Text = "I disagree";
            this.rbDisagree.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbDisagree.UncheckedState.BorderThickness = 2;
            this.rbDisagree.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbDisagree.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbDisagree.CheckedChanged += new System.EventHandler(this.rbDisagree_CheckedChanged);
            // 
            // btnNext
            // 
            this.btnNext.Animated = true;
            this.btnNext.BorderRadius = 10;
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNext.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNext.Enabled = false;
            this.btnNext.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnNext.FillColor2 = System.Drawing.SystemColors.Highlight;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.HoverState.FillColor = System.Drawing.SystemColors.Highlight;
            this.btnNext.HoverState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnNext.Location = new System.Drawing.Point(720, 540);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(250, 60);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Continue";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
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
            // TermsConditionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.guna2ControlBoxClose);
            this.Controls.Add(this.guna2ControlBoxMinimize);
            this.Controls.Add(this.pnlCard);
            this.DoubleBuffered = true;
            //this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TermsConditionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Terms & Conditions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TermsConditionsForm_FormClosing);
            this.Load += new System.EventHandler(this.TermsConditionsForm_Load);
            this.pnlCard.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.RichTextBox txtTerms;
        private Guna.UI2.WinForms.Guna2Panel pnlOptions;
        private Guna.UI2.WinForms.Guna2RadioButton rbAgree;
        private Guna.UI2.WinForms.Guna2RadioButton rbDisagree;
        private Guna.UI2.WinForms.Guna2GradientButton btnNext;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxClose;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxMinimize;
    }
}