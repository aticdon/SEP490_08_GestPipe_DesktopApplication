using System.Windows.Forms;

namespace GestPipePowerPonit
{
    partial class ListDefaultGestureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListDefaultGestureForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnCustomGesture = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPresentation = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnLanguageEN = new Guna.UI2.WinForms.Guna2Button();
            this.btnLanguageVN = new Guna.UI2.WinForms.Guna2Button();
            this.btnInstruction = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnGestureControl = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnHome = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.guna2HeaderPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnLogout = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.btnProfile = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2DataGridView1 = new Guna.UI2.WinForms.Guna2DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAccuracy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastUpdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnView = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnTraining = new System.Windows.Forms.DataGridViewImageColumn();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.guna2Panel1.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.guna2HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).BeginInit();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2DataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.Black;
            this.guna2Panel1.Controls.Add(this.btnCustomGesture);
            this.guna2Panel1.Controls.Add(this.btnPresentation);
            this.guna2Panel1.Controls.Add(this.guna2Panel3);
            this.guna2Panel1.Controls.Add(this.btnInstruction);
            this.guna2Panel1.Controls.Add(this.btnGestureControl);
            this.guna2Panel1.Controls.Add(this.btnHome);
            this.guna2Panel1.Controls.Add(this.pictureBoxLogo);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(267, 768);
            this.guna2Panel1.TabIndex = 0;
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
            this.btnCustomGesture.TabIndex = 12;
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
            this.btnPresentation.TabIndex = 11;
            this.btnPresentation.Text = "Presentation";
            this.btnPresentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPresentation.Click += new System.EventHandler(this.btnPresentation_Click);
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.Controls.Add(this.btnLanguageEN);
            this.guna2Panel3.Controls.Add(this.btnLanguageVN);
            this.guna2Panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel3.Location = new System.Drawing.Point(0, 706);
            this.guna2Panel3.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(267, 62);
            this.guna2Panel3.TabIndex = 5;
            // 
            // btnLanguageEN
            // 
            this.btnLanguageEN.BorderRadius = 5;
            this.btnLanguageEN.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageEN.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLanguageEN.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLanguageEN.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLanguageEN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            this.btnLanguageVN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            this.btnInstruction.FillColor2 = System.Drawing.Color.Gray;
            this.btnInstruction.Font = new System.Drawing.Font("Segoe UI", 10F);
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
            this.btnInstruction.Click += new System.EventHandler(this.btnInstruction_Click);
            // 
            // btnGestureControl
            // 
            this.btnGestureControl.BorderRadius = 10;
            this.btnGestureControl.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGestureControl.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGestureControl.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGestureControl.FillColor = System.Drawing.Color.Navy;
            this.btnGestureControl.FillColor2 = System.Drawing.Color.DeepSkyBlue;
            this.btnGestureControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
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
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.Image = ((System.Drawing.Image)(resources.GetObject("btnHome.Image")));
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
            // guna2HeaderPanel
            // 
            this.guna2HeaderPanel.Controls.Add(this.guna2ControlBoxClose);
            this.guna2HeaderPanel.Controls.Add(this.guna2ControlBoxMinimize);
            this.guna2HeaderPanel.Controls.Add(this.btnLogout);
            this.guna2HeaderPanel.Controls.Add(this.btnProfile);
            this.guna2HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2HeaderPanel.FillColor = System.Drawing.Color.Black;
            this.guna2HeaderPanel.Location = new System.Drawing.Point(267, 0);
            this.guna2HeaderPanel.Name = "guna2HeaderPanel";
            this.guna2HeaderPanel.Size = new System.Drawing.Size(1099, 120);
            this.guna2HeaderPanel.TabIndex = 2;
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1029, 11);
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
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(979, 11);
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
            this.btnLogout.Location = new System.Drawing.Point(1032, 48);
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
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BackColor = System.Drawing.Color.Black;
            this.panelMain.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(125)))), ((int)(((byte)(202)))));
            this.panelMain.BorderRadius = 15;
            this.panelMain.BorderThickness = 1;
            this.panelMain.Controls.Add(this.guna2DataGridView1);
            this.panelMain.Location = new System.Drawing.Point(297, 118);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1049, 601);
            this.panelMain.TabIndex = 3;
            // 
            // guna2DataGridView1
            // 
            this.guna2DataGridView1.AllowUserToAddRows = false;
            this.guna2DataGridView1.AllowUserToDeleteRows = false;
            this.guna2DataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.guna2DataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.guna2DataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2DataGridView1.BackgroundColor = System.Drawing.Color.Black;
            this.guna2DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.guna2DataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.guna2DataGridView1.ColumnHeadersHeight = 40;
            this.guna2DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.guna2DataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnAction,
            this.ColumnAccuracy,
            this.ColumnStatus,
            this.ColumnLastUpdate,
            this.ColumnView,
            this.ColumnTraining});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.guna2DataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.guna2DataGridView1.GridColor = System.Drawing.Color.Silver;
            this.guna2DataGridView1.Location = new System.Drawing.Point(40, 32);
            this.guna2DataGridView1.Name = "guna2DataGridView1";
            this.guna2DataGridView1.ReadOnly = true;
            this.guna2DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.guna2DataGridView1.RowHeadersVisible = false;
            this.guna2DataGridView1.RowHeadersWidth = 51;
            this.guna2DataGridView1.RowTemplate.Height = 50;
            this.guna2DataGridView1.Size = new System.Drawing.Size(976, 533);
            this.guna2DataGridView1.TabIndex = 4;
            this.guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(50)))));
            this.guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.White;
            this.guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.guna2DataGridView1.ThemeStyle.BackColor = System.Drawing.Color.Black;
            this.guna2DataGridView1.ThemeStyle.GridColor = System.Drawing.Color.Silver;
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.Black;
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.guna2DataGridView1.ThemeStyle.HeaderStyle.Height = 40;
            this.guna2DataGridView1.ThemeStyle.ReadOnly = true;
            this.guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(50)))));
            this.guna2DataGridView1.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.guna2DataGridView1.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.White;
            this.guna2DataGridView1.ThemeStyle.RowsStyle.Height = 50;
            this.guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 6;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnAction
            // 
            this.ColumnAction.HeaderText = "Type";
            this.ColumnAction.MinimumWidth = 6;
            this.ColumnAction.Name = "ColumnAction";
            this.ColumnAction.ReadOnly = true;
            // 
            // ColumnAccuracy
            // 
            this.ColumnAccuracy.HeaderText = "Accuracy";
            this.ColumnAccuracy.MinimumWidth = 6;
            this.ColumnAccuracy.Name = "ColumnAccuracy";
            this.ColumnAccuracy.ReadOnly = true;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.HeaderText = "Status";
            this.ColumnStatus.MinimumWidth = 6;
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            // 
            // ColumnLastUpdate
            // 
            this.ColumnLastUpdate.HeaderText = "Last Update";
            this.ColumnLastUpdate.MinimumWidth = 6;
            this.ColumnLastUpdate.Name = "ColumnLastUpdate";
            this.ColumnLastUpdate.ReadOnly = true;
            // 
            // ColumnView
            // 
            this.ColumnView.FillWeight = 50F;
            this.ColumnView.HeaderText = "";
            this.ColumnView.MinimumWidth = 6;
            this.ColumnView.Name = "ColumnView";
            this.ColumnView.ReadOnly = true;
            // 
            // ColumnTraining
            // 
            this.ColumnTraining.FillWeight = 50F;
            this.ColumnTraining.HeaderText = "";
            this.ColumnTraining.MinimumWidth = 6;
            this.ColumnTraining.Name = "ColumnTraining";
            this.ColumnTraining.ReadOnly = true;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.guna2HeaderPanel;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // ListDefaultGestureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.guna2HeaderPanel);
            this.Controls.Add(this.guna2Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ListDefaultGestureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GestPipe - Gesture Control";
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.guna2HeaderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).EndInit();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.guna2DataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2HeaderPanel;
        private Guna.UI2.WinForms.Guna2Panel panelMain;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private Guna.UI2.WinForms.Guna2GradientButton btnHome;
        private Guna.UI2.WinForms.Guna2GradientButton btnGestureControl;
        private Guna.UI2.WinForms.Guna2GradientButton btnInstruction;
        private Guna.UI2.WinForms.Guna2CirclePictureBox btnLogout;
        private Guna.UI2.WinForms.Guna2CirclePictureBox btnProfile;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2Button btnLanguageVN;
        private Guna.UI2.WinForms.Guna2Button btnLanguageEN;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxClose;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBoxMinimize;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2DataGridView guna2DataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAccuracy;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastUpdate;
        private System.Windows.Forms.DataGridViewImageColumn ColumnView;
        private Guna.UI2.WinForms.Guna2GradientButton btnPresentation;
        private Guna.UI2.WinForms.Guna2GradientButton btnCustomGesture;
        private System.Windows.Forms.DataGridViewImageColumn ColumnTraining;

    }
}