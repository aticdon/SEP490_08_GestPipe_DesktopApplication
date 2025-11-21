using Guna.UI2.WinForms;
using System.Drawing;

namespace GestPipePowerPonit
{
    partial class InstructionForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.btnCustomGesture = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPresentation = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pnlFooter = new Guna.UI2.WinForms.Guna2Panel();
            this.btnLanguageEN = new Guna.UI2.WinForms.Guna2Button();
            this.btnLanguageVN = new Guna.UI2.WinForms.Guna2Button();
            this.btnInstruction = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnGestureControl = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnHome = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.btnLogout = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.btnProfile = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.btnSubtab3 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnSubtab2 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnSubtab1 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlSidebar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.Black;
            this.pnlSidebar.Controls.Add(this.btnCustomGesture);
            this.pnlSidebar.Controls.Add(this.btnPresentation);
            this.pnlSidebar.Controls.Add(this.pnlFooter);
            this.pnlSidebar.Controls.Add(this.btnInstruction);
            this.pnlSidebar.Controls.Add(this.btnGestureControl);
            this.pnlSidebar.Controls.Add(this.btnHome);
            this.pnlSidebar.Controls.Add(this.pictureBoxLogo);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(267, 768);
            this.pnlSidebar.TabIndex = 0;
            // 
            // btnCustomGesture
            // 
            this.btnCustomGesture.BorderRadius = 10;
            this.btnCustomGesture.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomGesture.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomGesture.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCustomGesture.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCustomGesture.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnCustomGesture.FillColor2 = System.Drawing.Color.Gray;
            this.btnCustomGesture.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomGesture.ForeColor = System.Drawing.Color.White;
            this.btnCustomGesture.Image = global::GestPipePowerPonit.Properties.Resources.CustomCamera;
            this.btnCustomGesture.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomGesture.ImageSize = new System.Drawing.Size(24, 24);
            this.btnCustomGesture.Location = new System.Drawing.Point(16, 263);
            this.btnCustomGesture.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustomGesture.Name = "btnCustomGesture";
            this.btnCustomGesture.Size = new System.Drawing.Size(233, 55);
            this.btnCustomGesture.TabIndex = 11;
            this.btnCustomGesture.Text = "Custom Gesture";
            this.btnCustomGesture.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomGesture.Click += new System.EventHandler(this.btnCustomGesture_Click);
            // 
            // btnPresentation
            // 
            this.btnPresentation.BorderRadius = 10;
            this.btnPresentation.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPresentation.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPresentation.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnPresentation.FillColor2 = System.Drawing.Color.Gray;
            this.btnPresentation.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPresentation.ForeColor = System.Drawing.Color.White;
            this.btnPresentation.Image = global::GestPipePowerPonit.Properties.Resources.icon_search;
            this.btnPresentation.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPresentation.ImageSize = new System.Drawing.Size(24, 24);
            this.btnPresentation.Location = new System.Drawing.Point(16, 333);
            this.btnPresentation.Margin = new System.Windows.Forms.Padding(4);
            this.btnPresentation.Name = "btnPresentation";
            this.btnPresentation.Size = new System.Drawing.Size(233, 55);
            this.btnPresentation.TabIndex = 10;
            this.btnPresentation.Text = "Presentation";
            this.btnPresentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPresentation.Click += new System.EventHandler(this.btnPresentation_Click);
            // 
            // pnlFooter
            // 
            this.pnlFooter.Controls.Add(this.btnLanguageEN);
            this.pnlFooter.Controls.Add(this.btnLanguageVN);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 706);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(267, 62);
            this.pnlFooter.TabIndex = 5;
            // 
            // btnLanguageEN
            // 
            this.btnLanguageEN.BorderRadius = 5;
            this.btnLanguageEN.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageEN.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageEN.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLanguageEN.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLanguageEN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnLanguageEN.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLanguageEN.ForeColor = System.Drawing.Color.White;
            this.btnLanguageEN.Image = global::GestPipePowerPonit.Properties.Resources.English;
            this.btnLanguageEN.ImageSize = new System.Drawing.Size(24, 16);
            this.btnLanguageEN.Location = new System.Drawing.Point(71, 12);
            this.btnLanguageEN.Margin = new System.Windows.Forms.Padding(4);
            this.btnLanguageEN.Name = "btnLanguageEN";
            this.btnLanguageEN.Size = new System.Drawing.Size(47, 37);
            this.btnLanguageEN.TabIndex = 1;
            // 
            // btnLanguageVN
            // 
            this.btnLanguageVN.BorderRadius = 5;
            this.btnLanguageVN.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageVN.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageVN.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLanguageVN.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLanguageVN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnLanguageVN.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLanguageVN.ForeColor = System.Drawing.Color.White;
            this.btnLanguageVN.Image = global::GestPipePowerPonit.Properties.Resources.Vietnamese;
            this.btnLanguageVN.ImageSize = new System.Drawing.Size(24, 16);
            this.btnLanguageVN.Location = new System.Drawing.Point(16, 12);
            this.btnLanguageVN.Margin = new System.Windows.Forms.Padding(4);
            this.btnLanguageVN.Name = "btnLanguageVN";
            this.btnLanguageVN.Size = new System.Drawing.Size(47, 37);
            this.btnLanguageVN.TabIndex = 0;
            // 
            // btnInstruction
            // 
            this.btnInstruction.BorderRadius = 10;
            this.btnInstruction.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnInstruction.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnInstruction.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInstruction.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnInstruction.FillColor = System.Drawing.Color.Navy;
            this.btnInstruction.FillColor2 = System.Drawing.Color.DeepSkyBlue;
            this.btnInstruction.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInstruction.ForeColor = System.Drawing.Color.White;
            this.btnInstruction.Image = global::GestPipePowerPonit.Properties.Resources.icon_instruction;
            this.btnInstruction.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInstruction.ImageSize = new System.Drawing.Size(24, 24);
            this.btnInstruction.Location = new System.Drawing.Point(16, 403);
            this.btnInstruction.Margin = new System.Windows.Forms.Padding(4);
            this.btnInstruction.Name = "btnInstruction";
            this.btnInstruction.Size = new System.Drawing.Size(233, 55);
            this.btnInstruction.TabIndex = 4;
            this.btnInstruction.Text = "Instruction";
            this.btnInstruction.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnGestureControl
            // 
            this.btnGestureControl.BorderRadius = 10;
            this.btnGestureControl.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGestureControl.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGestureControl.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnGestureControl.FillColor2 = System.Drawing.Color.Gray;
            this.btnGestureControl.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.btnGestureControl.ForeColor = System.Drawing.Color.White;
            this.btnGestureControl.Image = global::GestPipePowerPonit.Properties.Resources.icon_gesture;
            this.btnGestureControl.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGestureControl.ImageSize = new System.Drawing.Size(24, 24);
            this.btnGestureControl.Location = new System.Drawing.Point(16, 193);
            this.btnGestureControl.Margin = new System.Windows.Forms.Padding(4);
            this.btnGestureControl.Name = "btnGestureControl";
            this.btnGestureControl.Size = new System.Drawing.Size(233, 55);
            this.btnGestureControl.TabIndex = 2;
            this.btnGestureControl.Text = "Gesture Control";
            this.btnGestureControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnHome
            // 
            this.btnHome.BorderRadius = 10;
            this.btnHome.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHome.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHome.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnHome.FillColor2 = System.Drawing.Color.Gray;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.Image = global::GestPipePowerPonit.Properties.Resources.icon_home;
            this.btnHome.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnHome.ImageSize = new System.Drawing.Size(24, 24);
            this.btnHome.Location = new System.Drawing.Point(16, 123);
            this.btnHome.Margin = new System.Windows.Forms.Padding(4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(233, 55);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "Home";
            this.btnHome.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::GestPipePowerPonit.Properties.Resources.Logo;
            this.pictureBoxLogo.Location = new System.Drawing.Point(16, 15);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(233, 74);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnLogout);
            this.pnlHeader.Controls.Add(this.btnProfile);
            this.pnlHeader.Controls.Add(this.guna2ControlBoxClose);
            this.pnlHeader.Controls.Add(this.guna2ControlBoxMinimize);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.FillColor = System.Drawing.Color.Black;
            this.pnlHeader.Location = new System.Drawing.Point(267, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1099, 120);
            this.pnlHeader.TabIndex = 2;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.FillColor = System.Drawing.Color.Transparent;
            this.btnLogout.Image = global::GestPipePowerPonit.Properties.Resources.icon_logout;
            this.btnLogout.ImageRotate = 0F;
            this.btnLogout.Location = new System.Drawing.Point(1027, 48);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnLogout.Size = new System.Drawing.Size(47, 43);
            this.btnLogout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLogout.TabIndex = 5;
            this.btnLogout.TabStop = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfile.FillColor = System.Drawing.Color.Transparent;
            this.btnProfile.Image = global::GestPipePowerPonit.Properties.Resources.icon_user;
            this.btnProfile.ImageRotate = 0F;
            this.btnProfile.Location = new System.Drawing.Point(978, 48);
            this.btnProfile.Margin = new System.Windows.Forms.Padding(4);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnProfile.Size = new System.Drawing.Size(47, 43);
            this.btnProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnProfile.TabIndex = 6;
            this.btnProfile.TabStop = false;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1037, 12);
            this.guna2ControlBoxClose.Name = "guna2ControlBoxClose";
            this.guna2ControlBoxClose.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxClose.TabIndex = 6;
            this.guna2ControlBoxClose.Click += new System.EventHandler(this.guna2ControlBoxClose_Click);
            // 
            // guna2ControlBoxMinimize
            // 
            this.guna2ControlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(981, 12);
            this.guna2ControlBoxMinimize.Name = "guna2ControlBoxMinimize";
            this.guna2ControlBoxMinimize.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxMinimize.TabIndex = 8;
            // 
            // pnlMain
            // 
            this.pnlMain.BorderRadius = 20;
            this.pnlMain.Controls.Add(this.guna2PictureBox1);
            this.pnlMain.Controls.Add(this.btnSubtab3);
            this.pnlMain.Controls.Add(this.btnSubtab2);
            this.pnlMain.Controls.Add(this.btnSubtab1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(135)))), ((int)(((byte)(202)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.Black;
            this.pnlMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlMain.Location = new System.Drawing.Point(267, 120);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1099, 648);
            this.pnlMain.TabIndex = 1;
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.Image = global::GestPipePowerPonit.Properties.Resources.IntructionTest_1;
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(3, 88);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(1096, 506);
            this.guna2PictureBox1.TabIndex = 2;
            this.guna2PictureBox1.TabStop = false;
            // 
            // btnSubtab3
            // 
            this.btnSubtab3.BackColor = System.Drawing.Color.Transparent;
            this.btnSubtab3.BorderRadius = 10;
            this.btnSubtab3.FillColor = System.Drawing.Color.Black;
            this.btnSubtab3.FillColor2 = System.Drawing.Color.Navy;
            this.btnSubtab3.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubtab3.ForeColor = System.Drawing.Color.White;
            this.btnSubtab3.Location = new System.Drawing.Point(400, 20);
            this.btnSubtab3.Name = "btnSubtab3";
            this.btnSubtab3.Size = new System.Drawing.Size(180, 50);
            this.btnSubtab3.TabIndex = 1;
            this.btnSubtab3.Text = "Environment & Practice Tips";
            // 
            // btnSubtab2
            // 
            this.btnSubtab2.BackColor = System.Drawing.Color.Transparent;
            this.btnSubtab2.BorderRadius = 10;
            this.btnSubtab2.FillColor = System.Drawing.Color.Black;
            this.btnSubtab2.FillColor2 = System.Drawing.Color.Navy;
            this.btnSubtab2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubtab2.ForeColor = System.Drawing.Color.White;
            this.btnSubtab2.Location = new System.Drawing.Point(210, 20);
            this.btnSubtab2.Name = "btnSubtab2";
            this.btnSubtab2.Size = new System.Drawing.Size(180, 50);
            this.btnSubtab2.TabIndex = 1;
            this.btnSubtab2.Text = "3D Model Setup";
            // 
            // btnSubtab1
            // 
            this.btnSubtab1.BackColor = System.Drawing.Color.Transparent;
            this.btnSubtab1.BorderRadius = 10;
            this.btnSubtab1.FillColor = System.Drawing.Color.Black;
            this.btnSubtab1.FillColor2 = System.Drawing.Color.Navy;
            this.btnSubtab1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubtab1.ForeColor = System.Drawing.Color.White;
            this.btnSubtab1.Location = new System.Drawing.Point(20, 20);
            this.btnSubtab1.Name = "btnSubtab1";
            this.btnSubtab1.Size = new System.Drawing.Size(180, 50);
            this.btnSubtab1.TabIndex = 0;
            this.btnSubtab1.Text = "Gesture Recording Guide";
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.pnlHeader;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // InstructionForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "InstructionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GestPipe - Instruction";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).EndInit();
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar;
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private Guna.UI2.WinForms.Guna2GradientButton btnHome;
        private Guna.UI2.WinForms.Guna2GradientButton btnGestureControl;
        private Guna.UI2.WinForms.Guna2GradientButton btnInstruction;
        private Guna.UI2.WinForms.Guna2CirclePictureBox btnLogout;
        private Guna.UI2.WinForms.Guna2CirclePictureBox btnProfile;
        private Guna.UI2.WinForms.Guna2Panel pnlFooter;
        private Guna.UI2.WinForms.Guna2Button btnLanguageVN;
        private Guna.UI2.WinForms.Guna2Button btnLanguageEN;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxClose;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxMinimize;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientButton btnPresentation;
        private Guna.UI2.WinForms.Guna2GradientButton btnCustomGesture;
        private Guna.UI2.WinForms.Guna2GradientButton btnSubtab1;
        private Guna.UI2.WinForms.Guna2GradientButton btnSubtab2;
        private Guna.UI2.WinForms.Guna2GradientButton btnSubtab3;
        private Guna2PictureBox guna2PictureBox1;
    }
}