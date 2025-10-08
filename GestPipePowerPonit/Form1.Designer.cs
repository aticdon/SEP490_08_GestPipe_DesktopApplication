using System;
using System.Windows.Forms;
using System.Drawing;

namespace GestPipePowerPonit
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtFile;
        private Button btnOpen, btnSlideShow, btnFirst, btnPrev, btnNext, btnLast, btnPause, btnClose;
        private OpenFileDialog openFileDialog1;
        private Panel panelSlide;
        private Label lblSlide, lblHints;
        private Button btnCloseSlide;

        private Button btnViewLeft, btnViewRight, btnViewTop, btnViewBottom;
        private Button btnZoomInTop, btnZoomOutTop;
        private Label lblZoomOverlay;
        private Timer overlayTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSlideShow = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panelSlide = new System.Windows.Forms.Panel();
            this.lblSlide = new System.Windows.Forms.Label();
            this.lblHints = new System.Windows.Forms.Label();
            this.btnZoomInTop = new System.Windows.Forms.Button();
            this.btnZoomOutTop = new System.Windows.Forms.Button();
            this.btnViewLeft = new System.Windows.Forms.Button();
            this.btnViewRight = new System.Windows.Forms.Button();
            this.btnViewTop = new System.Windows.Forms.Button();
            this.btnViewBottom = new System.Windows.Forms.Button();
            this.btnCloseSlide = new System.Windows.Forms.Button();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.panelSlide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(14, 12);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(400, 22);
            this.txtFile.TabIndex = 0;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(420, 10);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 26);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Browse";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSlideShow
            // 
            this.btnSlideShow.Location = new System.Drawing.Point(12, 50);
            this.btnSlideShow.Name = "btnSlideShow";
            this.btnSlideShow.Size = new System.Drawing.Size(90, 30);
            this.btnSlideShow.TabIndex = 2;
            this.btnSlideShow.Text = "Slide Show";
            this.btnSlideShow.Click += new System.EventHandler(this.btnSlideShow_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(110, 50);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(50, 30);
            this.btnFirst.TabIndex = 3;
            this.btnFirst.Text = "<<";
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(165, 50);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(50, 30);
            this.btnPrev.TabIndex = 4;
            this.btnPrev.Text = "<";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(325, 50);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(50, 30);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = ">";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(380, 50);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(50, 30);
            this.btnLast.TabIndex = 7;
            this.btnLast.Text = ">>";
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(435, 50);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 30);
            this.btnPause.TabIndex = 8;
            this.btnPause.Text = "Pause";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(515, 50);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelSlide
            // 
            this.panelSlide.BackColor = System.Drawing.Color.Black;
            this.panelSlide.Controls.Add(this.pictureBoxCamera);
            this.panelSlide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSlide.Location = new System.Drawing.Point(0, 0);
            this.panelSlide.Name = "panelSlide";
            this.panelSlide.Size = new System.Drawing.Size(1086, 700);
            this.panelSlide.TabIndex = 11;
            this.panelSlide.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSlide_Paint);
            // 
            // lblSlide
            // 
            this.lblSlide.Location = new System.Drawing.Point(220, 50);
            this.lblSlide.Name = "lblSlide";
            this.lblSlide.Size = new System.Drawing.Size(100, 30);
            this.lblSlide.TabIndex = 5;
            this.lblSlide.Text = "Slide - / -";
            this.lblSlide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHints
            // 
            this.lblHints.Location = new System.Drawing.Point(501, 12);
            this.lblHints.Name = "lblHints";
            this.lblHints.Size = new System.Drawing.Size(180, 30);
            this.lblHints.TabIndex = 10;
            this.lblHints.Text = "1:First  2:Prev  3:Next  4:Last  5:Pause  6:Close";
            // 
            // btnZoomInTop
            // 
            this.btnZoomInTop.Location = new System.Drawing.Point(792, 52);
            this.btnZoomInTop.Name = "btnZoomInTop";
            this.btnZoomInTop.Size = new System.Drawing.Size(40, 26);
            this.btnZoomInTop.TabIndex = 19;
            this.btnZoomInTop.Text = "+";
            this.btnZoomInTop.Click += new System.EventHandler(this.btnZoomInTop_Click);
            // 
            // btnZoomOutTop
            // 
            this.btnZoomOutTop.Location = new System.Drawing.Point(838, 52);
            this.btnZoomOutTop.Name = "btnZoomOutTop";
            this.btnZoomOutTop.Size = new System.Drawing.Size(40, 26);
            this.btnZoomOutTop.TabIndex = 20;
            this.btnZoomOutTop.Text = "–";
            this.btnZoomOutTop.Click += new System.EventHandler(this.btnZoomOutTop_Click);
            // 
            // btnViewLeft
            // 
            this.btnViewLeft.Location = new System.Drawing.Point(608, 52);
            this.btnViewLeft.Name = "btnViewLeft";
            this.btnViewLeft.Size = new System.Drawing.Size(40, 26);
            this.btnViewLeft.TabIndex = 13;
            this.btnViewLeft.Text = "←";
            this.btnViewLeft.Click += new System.EventHandler(this.btnViewLeft_Click);
            // 
            // btnViewRight
            // 
            this.btnViewRight.Location = new System.Drawing.Point(654, 52);
            this.btnViewRight.Name = "btnViewRight";
            this.btnViewRight.Size = new System.Drawing.Size(40, 26);
            this.btnViewRight.TabIndex = 15;
            this.btnViewRight.Text = "→";
            this.btnViewRight.Click += new System.EventHandler(this.btnViewRight_Click);
            // 
            // btnViewTop
            // 
            this.btnViewTop.Location = new System.Drawing.Point(700, 52);
            this.btnViewTop.Name = "btnViewTop";
            this.btnViewTop.Size = new System.Drawing.Size(40, 26);
            this.btnViewTop.TabIndex = 17;
            this.btnViewTop.Text = "↑";
            this.btnViewTop.Click += new System.EventHandler(this.btnViewTop_Click);
            // 
            // btnViewBottom
            // 
            this.btnViewBottom.Location = new System.Drawing.Point(746, 52);
            this.btnViewBottom.Name = "btnViewBottom";
            this.btnViewBottom.Size = new System.Drawing.Size(40, 26);
            this.btnViewBottom.TabIndex = 18;
            this.btnViewBottom.Text = "↓";
            this.btnViewBottom.Click += new System.EventHandler(this.btnViewBottom_Click);
            // 
            // btnCloseSlide
            // 
            this.btnCloseSlide.Location = new System.Drawing.Point(950, 50);
            this.btnCloseSlide.Name = "btnCloseSlide";
            this.btnCloseSlide.Size = new System.Drawing.Size(120, 30);
            this.btnCloseSlide.TabIndex = 21;
            this.btnCloseSlide.Text = "Đóng Slide";
            this.btnCloseSlide.Click += new System.EventHandler(this.btnCloseSlide_Click);
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.TabIndex = 0;
            this.pictureBoxCamera.TabStop = false;
            pictureBoxCamera.Dock = DockStyle.Fill;
            pictureBoxCamera.SizeMode = PictureBoxSizeMode.StretchImage;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1086, 700);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSlideShow);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.lblSlide);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblHints);
            this.Controls.Add(this.panelSlide);
            this.Controls.Add(this.btnViewLeft);
            this.Controls.Add(this.btnViewRight);
            this.Controls.Add(this.btnViewTop);
            this.Controls.Add(this.btnViewBottom);
            this.Controls.Add(this.btnZoomInTop);
            this.Controls.Add(this.btnZoomOutTop);
            this.Controls.Add(this.btnCloseSlide);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GestPipe";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panelSlide.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private PictureBox pictureBoxCamera;
    }
}