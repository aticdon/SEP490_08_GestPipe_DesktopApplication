using GestPipePowerPonit.Properties;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    partial class Form1
    {   
        private System.ComponentModel.IContainer components = null;

        // Guna Controls - Khai báo đồng bộ với HomeUser
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar, pnlFooter;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private Guna.UI2.WinForms.Guna2Button btnHome;
        private Guna.UI2.WinForms.Guna2Button btnGestureControl;
        private Guna.UI2.WinForms.Guna2Button btnVersion;
        private Guna.UI2.WinForms.Guna2Button btnInstruction;
        private Guna.UI2.WinForms.Guna2Button btnLanguageVN;
        private Guna.UI2.WinForms.Guna2Button btnLanguageEN;
        private Guna2Panel pnlHeader;
        private Guna2GradientPanel pnlMain;
        private Label lblPresentationFile;
        private Label lblOpenFile;
        private Guna2TextBox txtFile;
        private Guna2Button btnOpen;
        private Guna2Button btnSlideShow;
        private PictureBox pictureBoxCamera;
        private Panel panelSlide;
        private OpenFileDialog openFileDialog1;
        private Panel panelPreview;
        private Label lblPresentationPreview;
        private Guna2ComboBox cmbTopic, cmbCategory;
        private Guna2Button btnExit;

        // Title Bar Controls MỚI
        private Guna2ControlBox guna2ControlBoxMinimize;
        private Guna2ControlBox guna2ControlBoxClose;
        private Guna2CirclePictureBox btnLogout;
        private Guna2CirclePictureBox btnProfile;


        // Vẫn giữ lại các controls cũ nếu cần cho logic
        private Label lblSlide, lblZoomOverlay, lblHints;
        private Button btnFirst, btnPrev, btnNext, btnLast, btnPause, btnClose;
        private Button btnViewLeft, btnViewRight, btnViewTop, btnViewBottom;
        private Button btnZoomInTop, btnZoomOutTop, btnZoomInSlide, btnZoomOutSlide;
        private Timer overlayTimer;
        private Guna2Elipse guna2Elipse1;
        private Guna2DragControl guna2DragControl1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnLogout = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.btnProfile = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.btnPresentation = new Guna.UI2.WinForms.Guna2Button();
            this.btnCustomeGesture = new Guna.UI2.WinForms.Guna2Button();
            this.pnlFooter = new Guna.UI2.WinForms.Guna2Panel();
            this.btnLanguageEN = new Guna.UI2.WinForms.Guna2Button();
            this.btnLanguageVN = new Guna.UI2.WinForms.Guna2Button();
            this.btnInstruction = new Guna.UI2.WinForms.Guna2Button();
            this.btnVersion = new Guna.UI2.WinForms.Guna2Button();
            this.btnGestureControl = new Guna.UI2.WinForms.Guna2Button();
            this.btnHome = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.btnExit = new Guna.UI2.WinForms.Guna2Button();
            this.btnSlideShow = new Guna.UI2.WinForms.Guna2Button();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.lblPresentationPreview = new System.Windows.Forms.Label();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.cmbCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbTopic = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblTopic = new System.Windows.Forms.Label();
            this.btnOpen = new Guna.UI2.WinForms.Guna2Button();
            this.txtFile = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblOpenFile = new System.Windows.Forms.Label();
            this.lblPresentationFile = new System.Windows.Forms.Label();
            this.panelSlide = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblSlide = new System.Windows.Forms.Label();
            this.lblZoomOverlay = new System.Windows.Forms.Label();
            this.lblHints = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnViewLeft = new System.Windows.Forms.Button();
            this.btnViewRight = new System.Windows.Forms.Button();
            this.btnViewTop = new System.Windows.Forms.Button();
            this.btnViewBottom = new System.Windows.Forms.Button();
            this.btnZoomInTop = new System.Windows.Forms.Button();
            this.btnZoomOutTop = new System.Windows.Forms.Button();
            this.btnZoomInSlide = new System.Windows.Forms.Button();
            this.btnZoomOutSlide = new System.Windows.Forms.Button();
            this.overlayTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).BeginInit();
            this.pnlSidebar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.pnlHeader;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Black;
            this.pnlHeader.Controls.Add(this.guna2ControlBoxClose);
            this.pnlHeader.Controls.Add(this.guna2ControlBoxMinimize);
            this.pnlHeader.Controls.Add(this.btnLogout);
            this.pnlHeader.Controls.Add(this.btnProfile);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(267, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1099, 120);
            this.pnlHeader.TabIndex = 101;
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1046, 10);
            this.guna2ControlBoxClose.Name = "guna2ControlBoxClose";
            this.guna2ControlBoxClose.Size = new System.Drawing.Size(50, 30);
            this.guna2ControlBoxClose.TabIndex = 10;
            this.guna2ControlBoxClose.Click += new System.EventHandler(this.guna2ControlBoxClose_Click);
            // 
            // guna2ControlBoxMinimize
            // 
            this.guna2ControlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(1006, 10);
            this.guna2ControlBoxMinimize.Name = "guna2ControlBoxMinimize";
            this.guna2ControlBoxMinimize.Size = new System.Drawing.Size(50, 30);
            this.guna2ControlBoxMinimize.TabIndex = 12;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.FillColor = System.Drawing.Color.Transparent;
            this.btnLogout.Image = global::GestPipePowerPonit.Properties.Resources.icon_logout;
            this.btnLogout.ImageRotate = 0F;
            this.btnLogout.Location = new System.Drawing.Point(1033, 48);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnLogout.Size = new System.Drawing.Size(47, 43);
            this.btnLogout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLogout.TabIndex = 5;
            this.btnLogout.TabStop = false;
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
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.Black;
            this.pnlSidebar.Controls.Add(this.btnPresentation);
            this.pnlSidebar.Controls.Add(this.btnCustomeGesture);
            this.pnlSidebar.Controls.Add(this.pnlFooter);
            this.pnlSidebar.Controls.Add(this.btnInstruction);
            this.pnlSidebar.Controls.Add(this.btnVersion);
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
            // btnPresentation
            // 
            this.btnPresentation.BorderRadius = 10;
            this.btnPresentation.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPresentation.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPresentation.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPresentation.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(125)))), ((int)(((byte)(202)))));
            this.btnPresentation.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPresentation.ForeColor = System.Drawing.Color.White;
            this.btnPresentation.Image = global::GestPipePowerPonit.Properties.Resources.icon_search;
            this.btnPresentation.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPresentation.ImageSize = new System.Drawing.Size(24, 24);
            this.btnPresentation.Location = new System.Drawing.Point(16, 493);
            this.btnPresentation.Margin = new System.Windows.Forms.Padding(4);
            this.btnPresentation.Name = "btnPresentation";
            this.btnPresentation.Size = new System.Drawing.Size(233, 55);
            this.btnPresentation.TabIndex = 9;
            this.btnPresentation.Text = "Presentation";
            this.btnPresentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnCustomeGesture
            // 
            this.btnCustomeGesture.BorderRadius = 10;
            this.btnCustomeGesture.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomeGesture.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomeGesture.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCustomeGesture.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCustomeGesture.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnCustomeGesture.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomeGesture.ForeColor = System.Drawing.Color.White;
            this.btnCustomeGesture.Image = global::GestPipePowerPonit.Properties.Resources.icon_add;
            this.btnCustomeGesture.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomeGesture.ImageSize = new System.Drawing.Size(24, 24);
            this.btnCustomeGesture.Location = new System.Drawing.Point(16, 419);
            this.btnCustomeGesture.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustomeGesture.Name = "btnCustomeGesture";
            this.btnCustomeGesture.Size = new System.Drawing.Size(233, 55);
            this.btnCustomeGesture.TabIndex = 8;
            this.btnCustomeGesture.Text = "CustomGesture";
            this.btnCustomeGesture.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomeGesture.Click += new System.EventHandler(this.btnCustomeGesture_Click);
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
            this.btnInstruction.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnInstruction.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnInstruction.ForeColor = System.Drawing.Color.White;
            this.btnInstruction.Image = global::GestPipePowerPonit.Properties.Resources.icon_instruction;
            this.btnInstruction.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInstruction.ImageSize = new System.Drawing.Size(24, 24);
            this.btnInstruction.Location = new System.Drawing.Point(16, 345);
            this.btnInstruction.Margin = new System.Windows.Forms.Padding(4);
            this.btnInstruction.Name = "btnInstruction";
            this.btnInstruction.Size = new System.Drawing.Size(233, 55);
            this.btnInstruction.TabIndex = 4;
            this.btnInstruction.Text = "Instruction";
            this.btnInstruction.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnVersion
            // 
            this.btnVersion.BorderRadius = 10;
            this.btnVersion.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnVersion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnVersion.ForeColor = System.Drawing.Color.White;
            this.btnVersion.Image = global::GestPipePowerPonit.Properties.Resources.icon_version;
            this.btnVersion.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnVersion.ImageSize = new System.Drawing.Size(24, 24);
            this.btnVersion.Location = new System.Drawing.Point(16, 271);
            this.btnVersion.Margin = new System.Windows.Forms.Padding(4);
            this.btnVersion.Name = "btnVersion";
            this.btnVersion.Size = new System.Drawing.Size(233, 55);
            this.btnVersion.TabIndex = 3;
            this.btnVersion.Text = "Version";
            this.btnVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnGestureControl
            // 
            this.btnGestureControl.BorderRadius = 10;
            this.btnGestureControl.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnGestureControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGestureControl.ForeColor = System.Drawing.Color.White;
            this.btnGestureControl.Image = global::GestPipePowerPonit.Properties.Resources.icon_gesture;
            this.btnGestureControl.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGestureControl.ImageSize = new System.Drawing.Size(24, 24);
            this.btnGestureControl.Location = new System.Drawing.Point(16, 197);
            this.btnGestureControl.Margin = new System.Windows.Forms.Padding(4);
            this.btnGestureControl.Name = "btnGestureControl";
            this.btnGestureControl.Size = new System.Drawing.Size(233, 55);
            this.btnGestureControl.TabIndex = 2;
            this.btnGestureControl.Text = "Gesture Control";
            this.btnGestureControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGestureControl.Click += new System.EventHandler(this.btnGestureControl_Click);
            // 
            // btnHome
            // 
            this.btnHome.BorderRadius = 10;
            this.btnHome.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
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
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlMain.Controls.Add(this.btnExit);
            this.pnlMain.Controls.Add(this.btnSlideShow);
            this.pnlMain.Controls.Add(this.panelPreview);
            this.pnlMain.Controls.Add(this.cmbCategory);
            this.pnlMain.Controls.Add(this.lblCategory);
            this.pnlMain.Controls.Add(this.cmbTopic);
            this.pnlMain.Controls.Add(this.lblTopic);
            this.pnlMain.Controls.Add(this.btnOpen);
            this.pnlMain.Controls.Add(this.txtFile);
            this.pnlMain.Controls.Add(this.lblOpenFile);
            this.pnlMain.Controls.Add(this.lblPresentationFile);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.Black;
            this.pnlMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlMain.Location = new System.Drawing.Point(267, 120);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1099, 648);
            this.pnlMain.TabIndex = 1;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BorderRadius = 10;
            this.btnExit.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(946, 590);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(90, 36);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnSlideShow
            // 
            this.btnSlideShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSlideShow.BorderRadius = 10;
            this.btnSlideShow.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(230)))));
            this.btnSlideShow.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSlideShow.ForeColor = System.Drawing.Color.White;
            this.btnSlideShow.Location = new System.Drawing.Point(27, 563);
            this.btnSlideShow.Name = "btnSlideShow";
            this.btnSlideShow.Size = new System.Drawing.Size(225, 63);
            this.btnSlideShow.TabIndex = 8;
            this.btnSlideShow.Text = "Slide Show";
            this.btnSlideShow.Click += new System.EventHandler(this.btnSlideShow_Click);
            // 
            // panelPreview
            // 
            this.panelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.panelPreview.Controls.Add(this.lblPresentationPreview);
            this.panelPreview.Controls.Add(this.pictureBoxCamera);
            this.panelPreview.Location = new System.Drawing.Point(400, 24);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(656, 540);
            this.panelPreview.TabIndex = 10;
            // 
            // lblPresentationPreview
            // 
            this.lblPresentationPreview.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPresentationPreview.ForeColor = System.Drawing.Color.White;
            this.lblPresentationPreview.Location = new System.Drawing.Point(30, 5);
            this.lblPresentationPreview.Name = "lblPresentationPreview";
            this.lblPresentationPreview.Size = new System.Drawing.Size(244, 24);
            this.lblPresentationPreview.TabIndex = 0;
            this.lblPresentationPreview.Text = "Presentation preview:";
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(40)))));
            this.pictureBoxCamera.Location = new System.Drawing.Point(20, 32);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(616, 490);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabIndex = 1;
            this.pictureBoxCamera.TabStop = false;
            // 
            // cmbCategory
            // 
            this.cmbCategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.cmbCategory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.cmbCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.cmbCategory.FocusedColor = System.Drawing.Color.Empty;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCategory.ForeColor = System.Drawing.Color.White;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.ItemHeight = 25;
            this.cmbCategory.Location = new System.Drawing.Point(27, 208);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(350, 31);
            this.cmbCategory.TabIndex = 5;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            // 
            // lblCategory
            // 
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCategory.ForeColor = System.Drawing.Color.White;
            this.lblCategory.Location = new System.Drawing.Point(27, 172);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(350, 34);
            this.lblCategory.TabIndex = 4;
            this.lblCategory.Text = "Category:";
            // 
            // cmbTopic
            // 
            this.cmbTopic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.cmbTopic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.cmbTopic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTopic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTopic.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.cmbTopic.FocusedColor = System.Drawing.Color.Empty;
            this.cmbTopic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbTopic.ForeColor = System.Drawing.Color.White;
            this.cmbTopic.FormattingEnabled = true;
            this.cmbTopic.ItemHeight = 25;
            this.cmbTopic.Location = new System.Drawing.Point(27, 288);
            this.cmbTopic.Name = "cmbTopic";
            this.cmbTopic.Size = new System.Drawing.Size(350, 31);
            this.cmbTopic.TabIndex = 7;
            // 
            // lblTopic
            // 
            this.lblTopic.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTopic.ForeColor = System.Drawing.Color.White;
            this.lblTopic.Location = new System.Drawing.Point(27, 252);
            this.lblTopic.Name = "lblTopic";
            this.lblTopic.Size = new System.Drawing.Size(350, 33);
            this.lblTopic.TabIndex = 6;
            this.lblTopic.Text = "Topic:";
            // 
            // btnOpen
            // 
            this.btnOpen.BorderRadius = 10;
            this.btnOpen.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(200)))));
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOpen.ForeColor = System.Drawing.Color.White;
            this.btnOpen.Location = new System.Drawing.Point(27, 118);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(216, 40);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Browse Files...";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtFile
            // 
            this.txtFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.txtFile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtFile.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFile.DefaultText = "";
            this.txtFile.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFile.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.txtFile.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFile.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtFile.ForeColor = System.Drawing.Color.White;
            this.txtFile.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFile.Location = new System.Drawing.Point(27, 84);
            this.txtFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtFile.Name = "txtFile";
            this.txtFile.PlaceholderText = "";
            this.txtFile.SelectedText = "";
            this.txtFile.Size = new System.Drawing.Size(350, 25);
            this.txtFile.TabIndex = 2;
            // 
            // lblOpenFile
            // 
            this.lblOpenFile.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblOpenFile.ForeColor = System.Drawing.Color.White;
            this.lblOpenFile.Location = new System.Drawing.Point(27, 56);
            this.lblOpenFile.Name = "lblOpenFile";
            this.lblOpenFile.Size = new System.Drawing.Size(350, 23);
            this.lblOpenFile.TabIndex = 1;
            this.lblOpenFile.Text = "Open File:";
            // 
            // lblPresentationFile
            // 
            this.lblPresentationFile.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblPresentationFile.ForeColor = System.Drawing.Color.White;
            this.lblPresentationFile.Location = new System.Drawing.Point(27, 10);
            this.lblPresentationFile.Name = "lblPresentationFile";
            this.lblPresentationFile.Size = new System.Drawing.Size(367, 35);
            this.lblPresentationFile.TabIndex = 0;
            this.lblPresentationFile.Text = "Presentation File";
            // 
            // panelSlide
            // 
            this.panelSlide.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSlide.BackColor = System.Drawing.Color.Black;
            this.panelSlide.Location = new System.Drawing.Point(267, 768);
            this.panelSlide.Name = "panelSlide";
            this.panelSlide.Size = new System.Drawing.Size(1099, 50);
            this.panelSlide.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblSlide
            // 
            this.lblSlide.Location = new System.Drawing.Point(0, 0);
            this.lblSlide.Name = "lblSlide";
            this.lblSlide.Size = new System.Drawing.Size(100, 23);
            this.lblSlide.TabIndex = 0;
            // 
            // lblZoomOverlay
            // 
            this.lblZoomOverlay.Location = new System.Drawing.Point(0, 0);
            this.lblZoomOverlay.Name = "lblZoomOverlay";
            this.lblZoomOverlay.Size = new System.Drawing.Size(100, 23);
            this.lblZoomOverlay.TabIndex = 0;
            // 
            // lblHints
            // 
            this.lblHints.Location = new System.Drawing.Point(0, 0);
            this.lblHints.Name = "lblHints";
            this.lblHints.Size = new System.Drawing.Size(100, 23);
            this.lblHints.TabIndex = 0;
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(0, 0);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(75, 23);
            this.btnFirst.TabIndex = 0;
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(0, 0);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 0;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(0, 0);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 0;
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(0, 0);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(75, 23);
            this.btnLast.TabIndex = 0;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(0, 0);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(0, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            // 
            // btnViewLeft
            // 
            this.btnViewLeft.Location = new System.Drawing.Point(0, 0);
            this.btnViewLeft.Name = "btnViewLeft";
            this.btnViewLeft.Size = new System.Drawing.Size(75, 23);
            this.btnViewLeft.TabIndex = 0;
            // 
            // btnViewRight
            // 
            this.btnViewRight.Location = new System.Drawing.Point(0, 0);
            this.btnViewRight.Name = "btnViewRight";
            this.btnViewRight.Size = new System.Drawing.Size(75, 23);
            this.btnViewRight.TabIndex = 0;
            // 
            // btnViewTop
            // 
            this.btnViewTop.Location = new System.Drawing.Point(0, 0);
            this.btnViewTop.Name = "btnViewTop";
            this.btnViewTop.Size = new System.Drawing.Size(75, 23);
            this.btnViewTop.TabIndex = 0;
            // 
            // btnViewBottom
            // 
            this.btnViewBottom.Location = new System.Drawing.Point(0, 0);
            this.btnViewBottom.Name = "btnViewBottom";
            this.btnViewBottom.Size = new System.Drawing.Size(75, 23);
            this.btnViewBottom.TabIndex = 0;
            // 
            // btnZoomInTop
            // 
            this.btnZoomInTop.Location = new System.Drawing.Point(0, 0);
            this.btnZoomInTop.Name = "btnZoomInTop";
            this.btnZoomInTop.Size = new System.Drawing.Size(75, 23);
            this.btnZoomInTop.TabIndex = 0;
            // 
            // btnZoomOutTop
            // 
            this.btnZoomOutTop.Location = new System.Drawing.Point(0, 0);
            this.btnZoomOutTop.Name = "btnZoomOutTop";
            this.btnZoomOutTop.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOutTop.TabIndex = 0;
            // 
            // btnZoomInSlide
            // 
            this.btnZoomInSlide.Location = new System.Drawing.Point(0, 0);
            this.btnZoomInSlide.Name = "btnZoomInSlide";
            this.btnZoomInSlide.Size = new System.Drawing.Size(75, 23);
            this.btnZoomInSlide.TabIndex = 0;
            // 
            // btnZoomOutSlide
            // 
            this.btnZoomOutSlide.Location = new System.Drawing.Point(0, 0);
            this.btnZoomOutSlide.Name = "btnZoomOutSlide";
            this.btnZoomOutSlide.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOutSlide.TabIndex = 0;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.panelSlide);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GestPipe - Presentation";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).EndInit();
            this.pnlSidebar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            this.ResumeLayout(false);

        }

        private Label lblTopic;
        private Label lblCategory;
        private Guna2Button btnCustomeGesture;
        private Guna2Button btnPresentation;
    }
}