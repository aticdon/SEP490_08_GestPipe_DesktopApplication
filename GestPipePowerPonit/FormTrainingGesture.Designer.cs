using Guna.UI2.WinForms;

namespace GestPipePowerPonit
{
    partial class FormTrainingGesture
    {
        private System.ComponentModel.IContainer components = null;

        // Guna Controls - Tích hợp từ Form1
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar, pnlFooter;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private Guna2Panel pnlHeader;
        private System.Windows.Forms.Label lblTrainingTitle; // Tiêu đề cho Header
        private Guna2GradientPanel pnlMain; // Panel chứa nội dung luyện tập
        private Guna2ControlBox guna2ControlBoxMinimize;
        private Guna2ControlBox guna2ControlBoxMaximize;
        private Guna2ControlBox guna2ControlBoxClose;
        private Guna2Elipse guna2Elipse1;
        private Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2Button btnCustomeGesture, btnInstruction, btnVersion, btnGestureControl, btnHome;
        private Guna.UI2.WinForms.Guna2Button btnLanguageVN;
        private Guna.UI2.WinForms.Guna2Button btnLanguageEN;

        // Các Control luyện tập gốc, được thêm vào pnlMain
        private System.Windows.Forms.PictureBox picCamera;
        private System.Windows.Forms.Label lblCorrect, lblWrong, lblAccuracy, lblResult;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMaximize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.lblTrainingTitle = new System.Windows.Forms.Label();
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
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
            this.btnEndTraining = new Guna.UI2.WinForms.Guna2Button();
            this.picCamera = new System.Windows.Forms.PictureBox();
            this.lblPose = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblCorrect = new System.Windows.Forms.Label();
            this.lblWrong = new System.Windows.Forms.Label();
            this.lblAccuracy = new System.Windows.Forms.Label();
            this.lblReason = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCamera)).BeginInit();
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
            this.pnlHeader.Controls.Add(this.guna2ControlBoxClose);
            this.pnlHeader.Controls.Add(this.guna2ControlBoxMaximize);
            this.pnlHeader.Controls.Add(this.guna2ControlBoxMinimize);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.FillColor = System.Drawing.Color.Black;
            this.pnlHeader.Location = new System.Drawing.Point(267, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1099, 120);
            this.pnlHeader.TabIndex = 2;
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1025, 12);
            this.guna2ControlBoxClose.Name = "guna2ControlBoxClose";
            this.guna2ControlBoxClose.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxClose.TabIndex = 6;
            // 
            // guna2ControlBoxMaximize
            // 
            this.guna2ControlBoxMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMaximize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox;
            this.guna2ControlBoxMaximize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMaximize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMaximize.Location = new System.Drawing.Point(975, 12);
            this.guna2ControlBoxMaximize.Name = "guna2ControlBoxMaximize";
            this.guna2ControlBoxMaximize.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxMaximize.TabIndex = 7;
            // 
            // guna2ControlBoxMinimize
            // 
            this.guna2ControlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(925, 12);
            this.guna2ControlBoxMinimize.Name = "guna2ControlBoxMinimize";
            this.guna2ControlBoxMinimize.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxMinimize.TabIndex = 8;
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
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.Black;
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
            // btnCustomeGesture
            // 
            this.btnCustomeGesture.BorderRadius = 10;
            this.btnCustomeGesture.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomeGesture.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCustomeGesture.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCustomeGesture.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCustomeGesture.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(135)))), ((int)(((byte)(202)))));
            this.btnCustomeGesture.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCustomeGesture.ForeColor = System.Drawing.Color.White;
            this.btnCustomeGesture.Image = global::GestPipePowerPonit.Properties.Resources.icon_add;
            this.btnCustomeGesture.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomeGesture.ImageSize = new System.Drawing.Size(24, 24);
            this.btnCustomeGesture.Location = new System.Drawing.Point(16, 419);
            this.btnCustomeGesture.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustomeGesture.Name = "btnCustomeGesture";
            this.btnCustomeGesture.Size = new System.Drawing.Size(233, 55);
            this.btnCustomeGesture.TabIndex = 6;
            this.btnCustomeGesture.Text = "CustomGesture";
            this.btnCustomeGesture.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
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
            this.btnVersion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnVersion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnVersion.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnVersion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
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
            this.btnGestureControl.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGestureControl.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
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
            // 
            // btnHome
            // 
            this.btnHome.BorderRadius = 10;
            this.btnHome.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHome.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHome.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHome.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F);
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
            this.pnlMain.Location = new System.Drawing.Point(267, 120);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1099, 648);
            this.pnlMain.TabIndex = 0;
            // 
            // btnEndTraining
            // 
            this.btnEndTraining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEndTraining.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnEndTraining.BorderRadius = 10;
            this.btnEndTraining.FillColor = System.Drawing.Color.Teal;
            this.btnEndTraining.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEndTraining.ForeColor = System.Drawing.Color.White;
            this.btnEndTraining.Location = new System.Drawing.Point(24, 574);
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
            this.picCamera.Location = new System.Drawing.Point(24, 25);
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
            this.lblPose.Location = new System.Drawing.Point(702, 50);
            this.lblPose.Name = "lblPose";
            this.lblPose.Size = new System.Drawing.Size(220, 24);
            this.lblPose.TabIndex = 1;
            this.lblPose.Text = "🎯 Pose Target: [Gesture]";
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblResult.ForeColor = System.Drawing.Color.White;
            this.lblResult.Location = new System.Drawing.Point(702, 110);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(220, 24);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "✅ Last Result: N/A";
            // 
            // lblCorrect
            // 
            this.lblCorrect.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCorrect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.lblCorrect.Location = new System.Drawing.Point(703, 170);
            this.lblCorrect.Name = "lblCorrect";
            this.lblCorrect.Size = new System.Drawing.Size(150, 24);
            this.lblCorrect.TabIndex = 3;
            this.lblCorrect.Text = "✅ Correct: 0";
            // 
            // lblWrong
            // 
            this.lblWrong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWrong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblWrong.Location = new System.Drawing.Point(703, 230);
            this.lblWrong.Name = "lblWrong";
            this.lblWrong.Size = new System.Drawing.Size(150, 24);
            this.lblWrong.TabIndex = 4;
            this.lblWrong.Text = "❌ Wrong: 0";
            // 
            // lblAccuracy
            // 
            this.lblAccuracy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAccuracy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblAccuracy.Location = new System.Drawing.Point(703, 290);
            this.lblAccuracy.Name = "lblAccuracy";
            this.lblAccuracy.Size = new System.Drawing.Size(150, 24);
            this.lblAccuracy.TabIndex = 5;
            this.lblAccuracy.Text = "📊 Accuracy: 0%";
            // 
            // lblReason
            // 
            this.lblReason.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblReason.ForeColor = System.Drawing.Color.White;
            this.lblReason.Location = new System.Drawing.Point(707, 415);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(350, 90);
            this.lblReason.TabIndex = 6;
            this.lblReason.Text = "Reason: Keep your hand stable and clearly visible in the camera frame.";
            // 
            // FormTrainingGesture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTrainingGesture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TrainingGesture";
            this.pnlHeader.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCamera)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPose;
        private System.Windows.Forms.Label lblReason;
        private Guna2Button btnEndTraining;
    }
}