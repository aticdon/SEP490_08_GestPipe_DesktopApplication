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
            this.Size = new Size(500, 300);
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.Opacity = 0.98;

            // ✅ Rounded corners
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            SetupUI();
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

        private void SetupUI()
        {
            // ✅ Main Card Panel - VIỀN XANH CYAN
            var pnlCard = new Guna2Panel
            {
                Size = new Size(480, 280),
                Location = new Point(10, 10),
                BorderRadius = 20,
                FillColor = Color.FromArgb(200, 0, 0, 0), // Semi-transparent black
                BorderThickness = 2,  // ✅ Tăng độ dày viền
                BorderColor = Color.FromArgb(0, 188, 212), // ✅ VIỀN XANH CYAN
                CustomBorderColor = Color.FromArgb(0, 188, 212)
            };
            this.Controls.Add(pnlCard);

            // ✅ Icon (Circle với màu theo type)
            var iconColor = GetIconColor(_type);
            var iconText = GetIconText(_type);

            var picIcon = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(210, 30), // Center horizontally
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
                Location = new Point(0, 105),
                Size = new Size(480, 35),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlCard.Controls.Add(lblTitle);

            // ✅ Message Label (màu trắng)
            var lblMessage = new Label
            {
                Text = _message,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(40, 145),
                Size = new Size(400, 60),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter,
                AutoSize = false
            };
            pnlCard.Controls.Add(lblMessage);

            // ✅ Buttons Panel
            var pnlButtons = new Panel
            {
                Size = new Size(480, 50),
                Location = new Point(0, 215),
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(pnlButtons);

            AddButtons(pnlButtons);

            // ✅ Close button (X)
            var btnClose = new Guna2CircleButton
            {
                Size = new Size(30, 30),
                Location = new Point(445, 10),
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
        }

        private void AddButtons(Panel pnlButtons)
        {
            int btnWidth = 120;
            int btnHeight = 40;
            int spacing = 15;

            switch (_buttons)
            {
                case MessageBoxButtons.OK:
                    AddGradientButton(pnlButtons, "OK", (480 - btnWidth) / 2, 5, DialogResult.OK, true);
                    break;

                case MessageBoxButtons.OKCancel:
                    int startX2 = (480 - (btnWidth * 2 + spacing)) / 2;
                    AddGradientButton(pnlButtons, "OK", startX2, 5, DialogResult.OK, true);
                    AddGradientButton(pnlButtons, "Cancel", startX2 + btnWidth + spacing, 5, DialogResult.Cancel, false);
                    break;

                case MessageBoxButtons.YesNo:
                    int startXYN = (480 - (btnWidth * 2 + spacing)) / 2;
                    AddGradientButton(pnlButtons, "Yes", startXYN, 5, DialogResult.Yes, true);
                    AddGradientButton(pnlButtons, "No", startXYN + btnWidth + spacing, 5, DialogResult.No, false);
                    break;

                case MessageBoxButtons.YesNoCancel:
                    btnWidth = 100;
                    int startX3 = (480 - (btnWidth * 3 + spacing * 2)) / 2;
                    AddGradientButton(pnlButtons, "Yes", startX3, 5, DialogResult.Yes, true);
                    AddGradientButton(pnlButtons, "No", startX3 + btnWidth + spacing, 5, DialogResult.No, false);
                    AddGradientButton(pnlButtons, "Cancel", startX3 + (btnWidth + spacing) * 2, 5, DialogResult.Cancel, false);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Name = "CustomMessageBox";
            this.ResumeLayout(false);
        }
    }
}