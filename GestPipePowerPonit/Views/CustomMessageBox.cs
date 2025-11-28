using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views
{
    public partial class CustomMessageBox : Form
    {
        public enum MessageBoxType
        {
            Success,
            Error,
            Warning,
            Info,
            Question
        }

        public enum MessageBoxButtons
        {
            OK,
            OKCancel,
            YesNo,
            YesNoCancel
        }

        private MessageBoxType _type;
        private string _message;
        private string _title;
        private MessageBoxButtons _buttons;

        public DialogResult Result { get; private set; } = DialogResult.None;

        // ✅ Constructor mặc định cho Designer
        public CustomMessageBox()
        {
            InitializeComponent();
            SetupDesignDefaults();
        }

        private CustomMessageBox(string message, string title, MessageBoxType type, MessageBoxButtons buttons)
        {
            InitializeComponent();
            _message = message;
            _title = title;
            _type = type;
            _buttons = buttons;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            // ✅ DYNAMIC SIZE dựa trên nội dung
            var calculatedSize = CalculateFormSize(message, title, buttons);
            this.Size = calculatedSize;

            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.Opacity = 0.98;

            // ✅ Rounded corners với size động
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            SetupUI();
        }

        // ✅ Setup cho Designer mode
        private void SetupDesignDefaults()
        {
            _message = "This is a sample message for design purposes. You can see how the message box will look with longer text content.";
            _title = "Design Preview";
            _type = MessageBoxType.Info;
            _buttons = MessageBoxButtons.OKCancel;

            this.Size = new Size(450, 300);
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;

            if (!this.DesignMode)
            {
                SetupUI();
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private Size CalculateFormSize(string message, string title, MessageBoxButtons buttons)
        {
            // ✅ Tính chiều rộng dựa trên text
            using (var g = this.CreateGraphics() ?? Graphics.FromImage(new Bitmap(1, 1)))
            {
                var titleFont = new Font("Segoe UI", 18, FontStyle.Bold);
                var messageFont = new Font("Segoe UI", 11);

                var titleSize = g.MeasureString(title, titleFont);
                var messageSize = g.MeasureString(message, messageFont, 350); // Max width 350px

                // ✅ Tính chiều rộng tối thiểu
                int minWidth = 450;
                int calculatedWidth = Math.Max(minWidth, Math.Max((int)titleSize.Width + 100, (int)messageSize.Width + 100));

                // ✅ Tính chiều cao dựa trên message
                int baseHeight = 300; // Height cơ bản
                int extraHeight = Math.Max(0, (int)messageSize.Height - 60); // Nếu message dài hơn 60px

                // ✅ Thêm height cho buttons
                int buttonExtraHeight = buttons == MessageBoxButtons.YesNoCancel ? 15 : 0;

                return new Size(
                    Math.Min(calculatedWidth, 650), // Max width 650px
                    Math.Min(baseHeight + extraHeight + buttonExtraHeight, 450) // Max height 450px
                );
            }
        }

        private void SetupUI()
        {
            // Clear existing controls
            this.Controls.Clear();

            int formWidth = this.Width;
            int formHeight = this.Height;

            // ✅ Main Card Panel - responsive size
            var pnlCard = new Guna2Panel
            {
                Size = new Size(formWidth, formHeight),
                Location = new Point(0, 0),
                BorderRadius = 20,
                FillColor = Color.FromArgb(120, 0, 0, 0), // Semi-transparent black
                BorderThickness = 2,
                BorderColor = Color.FromArgb(0, 188, 212),
                CustomBorderColor = Color.FromArgb(0, 188, 212)
            };
            this.Controls.Add(pnlCard);

            // ✅ Icon (Circle với màu theo type)
            var iconColor = GetIconColor(_type);
            var iconText = GetIconText(_type);

            var picIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point((formWidth - 60) / 2, 25),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = CreateCircleIcon(iconColor, iconText)
            };
            pnlCard.Controls.Add(picIcon);

            // ✅ Title Label (màu cyan)
            var lblTitle = new Label
            {
                Text = _title,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 188, 212), // Cyan
                Location = new Point(20, 95),
                Size = new Size(formWidth - 40, 35),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            pnlCard.Controls.Add(lblTitle);

            // ✅ Message Label với auto-size
            var messageStartY = 160;
            var messageHeight = formHeight - messageStartY - 80; // 80px cho buttons area

            var lblMessage = new Label
            {
                Text = _message,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(30, messageStartY),
                Size = new Size(formWidth - 60, messageHeight),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter,
                AutoSize = false
            };
            pnlCard.Controls.Add(lblMessage);

            // ✅ Buttons Panel ở bottom
            var pnlButtons = new Panel
            {
                Size = new Size(formWidth, 55),
                Location = new Point(0, formHeight - 70),
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(pnlButtons);

            AddButtons(pnlButtons);

            // ✅ Close button (X)
            var btnClose = new Guna2CircleButton
            {
                Size = new Size(30, 30),
                Location = new Point(formWidth - 40, 10),
                Text = "✕",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.Transparent,
                BorderColor = Color.Transparent,
                HoverState = { FillColor = Color.FromArgb(50, 255, 255, 255) },
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => { Result = DialogResult.Cancel; this.Close(); };
            pnlCard.Controls.Add(btnClose);

            // ✅ Draggable form
            MakeDraggable(pnlCard);

            // ✅ Apply rounded corners nếu không phải design mode
            if (!this.DesignMode)
            {
                this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            }
        }

        private void AddButtons(Panel pnlButtons)
        {
            int btnWidth = 120;
            int btnHeight = 40;
            int spacing = 15;
            int panelWidth = pnlButtons.Width;

            switch (_buttons)
            {
                case MessageBoxButtons.OK:
                    AddGradientButton(pnlButtons, "OK", (panelWidth - btnWidth) / 2, 8, DialogResult.OK, true);
                    break;

                case MessageBoxButtons.OKCancel:
                    int startX2 = (panelWidth - (btnWidth * 2 + spacing)) / 2;
                    AddGradientButton(pnlButtons, "OK", startX2, 8, DialogResult.OK, true);
                    AddGradientButton(pnlButtons, "Cancel", startX2 + btnWidth + spacing, 8, DialogResult.Cancel, false);
                    break;

                case MessageBoxButtons.YesNo:
                    int startXYN = (panelWidth - (btnWidth * 2 + spacing)) / 2;
                    AddGradientButton(pnlButtons, "Yes", startXYN, 8, DialogResult.Yes, true);
                    AddGradientButton(pnlButtons, "No", startXYN + btnWidth + spacing, 8, DialogResult.No, false);
                    break;

                case MessageBoxButtons.YesNoCancel:
                    btnWidth = 100;
                    int startX3 = (panelWidth - (btnWidth * 3 + spacing * 2)) / 2;
                    AddGradientButton(pnlButtons, "Yes", startX3, 8, DialogResult.Yes, true);
                    AddGradientButton(pnlButtons, "No", startX3 + btnWidth + spacing, 8, DialogResult.No, false);
                    AddGradientButton(pnlButtons, "Cancel", startX3 + (btnWidth + spacing) * 2, 8, DialogResult.Cancel, false);
                    break;
            }
        }

        private void AddGradientButton(Control parent, string text, int x, int y, DialogResult result, bool isPrimary)
        {
            var btn = new Guna2GradientButton
            {
                Text = text,
                Size = new Size(text == "Cancel" ? 100 : 120, 40),
                Location = new Point(x, y),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Animated = true
            };

            if (isPrimary)
            {
                // ✅ Primary button - GRADIENT XANH CYAN → XANH DƯƠNG
                btn.FillColor = Color.FromArgb(0, 188, 212);      // Cyan
                btn.FillColor2 = SystemColors.Highlight;          // Blue (Highlight)
                btn.HoverState.FillColor = SystemColors.Highlight;
                btn.HoverState.FillColor2 = Color.FromArgb(0, 188, 212);
            }
            else
            {
                // ✅ Secondary button (Cancel/No) - GRADIENT ĐỎ
                btn.FillColor = Color.FromArgb(244, 67, 54);      // Red
                btn.FillColor2 = Color.FromArgb(198, 40, 40);     // Dark Red
                btn.HoverState.FillColor = Color.FromArgb(198, 40, 40);
                btn.HoverState.FillColor2 = Color.FromArgb(244, 67, 54);
            }

            btn.Click += (s, e) =>
            {
                Result = result;
                this.Close();
            };

            parent.Controls.Add(btn);
        }

        private Color GetIconColor(MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.Success: return Color.FromArgb(76, 175, 80);
                case MessageBoxType.Error: return Color.FromArgb(244, 67, 54);
                case MessageBoxType.Warning: return Color.FromArgb(255, 152, 0);
                case MessageBoxType.Info: return Color.FromArgb(0, 188, 212); // Cyan
                case MessageBoxType.Question: return Color.FromArgb(156, 39, 176);
                default: return Color.FromArgb(0, 188, 212);
            }
        }

        private string GetIconText(MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.Success: return "✓";
                case MessageBoxType.Error: return "✕";
                case MessageBoxType.Warning: return "!";
                case MessageBoxType.Info: return "i";
                case MessageBoxType.Question: return "?";
                default: return "i";
            }
        }

        private Bitmap CreateCircleIcon(Color bgColor, string text)
        {
            var bmp = new Bitmap(60, 60);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // ✅ Draw circle with gradient
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, 60, 60);
                    using (var brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = bgColor;
                        brush.SurroundColors = new[] { Color.FromArgb(200, bgColor) };
                        g.FillEllipse(brush, 0, 0, 60, 60);
                    }
                }

                // ✅ Draw border
                using (var pen = new Pen(Color.FromArgb(150, 255, 255, 255), 2))
                {
                    g.DrawEllipse(pen, 1, 1, 58, 58);
                }

                // ✅ Draw text (icon)
                using (var font = new Font("Segoe UI", 28, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var size = g.MeasureString(text, font);
                    var x = (60 - size.Width) / 2;
                    var y = (60 - size.Height) / 2;
                    g.DrawString(text, font, textBrush, x, y);
                }
            }
            return bmp;
        }

        private void MakeDraggable(Control control)
        {
            Point lastPoint = Point.Empty;
            control.MouseDown += (s, e) =>
            {
                lastPoint = new Point(e.X, e.Y);
            };
            control.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Left += e.X - lastPoint.X;
                    this.Top += e.Y - lastPoint.Y;
                }
            };
        }

        // ✅ Override để refresh UI khi resize trong Designer
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!this.DesignMode && this.Controls.Count > 0)
            {
                SetupUI();
            }
        }

        // ✅ Static Show methods
        public static DialogResult Show(string message, string title = "Notification",
            MessageBoxType type = MessageBoxType.Info,
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            using (var msgBox = new CustomMessageBox(message, title, type, buttons))
            {
                msgBox.ShowDialog();
                return msgBox.Result;
            }
        }

        public static DialogResult ShowSuccess(string message, string title = "Success")
        {
            return Show(message, title, MessageBoxType.Success, MessageBoxButtons.OK);
        }

        public static DialogResult ShowError(string message, string title = "Error")
        {
            return Show(message, title, MessageBoxType.Error, MessageBoxButtons.OK);
        }

        public static DialogResult ShowWarning(string message, string title = "Warning")
        {
            return Show(message, title, MessageBoxType.Warning, MessageBoxButtons.OK);
        }

        public static DialogResult ShowInfo(string message, string title = "Information")
        {
            return Show(message, title, MessageBoxType.Info, MessageBoxButtons.OK);
        }

        public static DialogResult ShowQuestion(string message, string title = "Question")
        {
            return Show(message, title, MessageBoxType.Question, MessageBoxButtons.YesNo);
        }

        // ✅ THÊM METHOD ĐỂ TEST DESIGN
        public static void ShowDesignTest()
        {
            var testBox = new CustomMessageBox(
                "This is a very long message to test the dynamic sizing functionality of the custom message box. It should resize automatically based on content length and wrap text properly without cutting off any characters. This message contains multiple sentences to demonstrate how the form handles longer content and ensures all text remains visible.",
                "Design Test - Long Title Example",
                MessageBoxType.Warning,
                MessageBoxButtons.YesNoCancel
            );

            testBox.ShowDialog();
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMessageBox));
            this.SuspendLayout();
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // ✅ THAY ĐỔI
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(450, 300); // ✅ Size cho design
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CustomMessageBox";
            this.ResumeLayout(false);
        }

        #endregion
    }
}


