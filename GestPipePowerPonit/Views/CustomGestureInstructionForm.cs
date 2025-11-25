using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views
{
    public partial class CustomGestureInstructionForm : Form
    {
        public DialogResult Result { get; private set; } = DialogResult.None;
        private string _gestureName;

        public CustomGestureInstructionForm(string gestureName = "")
        {
            InitializeComponent();
            _gestureName = gestureName;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.Size = new Size(600, 500); // ✅ KÍCH THƯỚC LỚN HƠN
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.Opacity = 0.98;
            this.TopMost = true;

            // ✅ Rounded corners
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));

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
            // ✅ Main Card Panel
            var pnlCard = new Guna2Panel
            {
                Size = new Size(600, 500),
                Location = new Point(0, 0),
                BorderRadius = 25,
                FillColor = Color.FromArgb(30, 30, 30), // Dark background
                BorderThickness = 3,
                BorderColor = Color.FromArgb(0, 188, 212),
                CustomBorderColor = Color.FromArgb(0, 188, 212)
            };
            this.Controls.Add(pnlCard);

            // ✅ Header với icon và title
            CreateHeader(pnlCard);

            // ✅ Main content area
            CreateContent(pnlCard);

            // ✅ Footer với buttons
            CreateFooter(pnlCard);

            // ✅ Close button (X)
            var btnClose = new Guna2CircleButton
            {
                Size = new Size(35, 35),
                Location = new Point(pnlCard.Width - 50, 15),
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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

        private void CreateHeader(Guna2Panel parent)
        {
            // ✅ Icon
            var picIcon = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(30, 25),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = CreateHeaderIcon()
            };
            parent.Controls.Add(picIcon);

            // ✅ Title
            var lblTitle = new Label
            {
                Text = GetTitle(),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 188, 212),
                Location = new Point(90, 25),
                Size = new Size(450, 50),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            parent.Controls.Add(lblTitle);

            // ✅ Separator line
            var separator = new Panel
            {
                Size = new Size(540, 2),
                Location = new Point(30, 85),
                BackColor = Color.FromArgb(0, 188, 212)
            };
            parent.Controls.Add(separator);
        }

        private void CreateContent(Guna2Panel parent)
        {
            // ✅ Scrollable content panel
            var pnlContent = new Panel
            {
                Size = new Size(540, 320),
                Location = new Point(30, 100),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            parent.Controls.Add(pnlContent);

            // ✅ Instructions
            CreateInstructions(pnlContent);
        }

        private void CreateInstructions(Panel parent)
        {
            var instructions = GetInstructions();
            int yPos = 10;

            foreach (var instruction in instructions)
            {
                // ✅ Icon cho mỗi instruction
                var picInstructionIcon = new Label
                {
                    Text = instruction.Icon,
                    Font = new Font("Segoe UI", 16),
                    ForeColor = Color.FromArgb(0, 188, 212),
                    Location = new Point(10, yPos),
                    Size = new Size(30, 30),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                parent.Controls.Add(picInstructionIcon);

                // ✅ Text instruction
                var lblInstruction = new Label
                {
                    Text = instruction.Text,
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.White,
                    Location = new Point(50, yPos),
                    Size = new Size(470, 30),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                parent.Controls.Add(lblInstruction);

                yPos += 40;
            }

            // ✅ Additional info
            var lblNote = new Label
            {
                Text = GetAdditionalNote(),
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(10, yPos + 10),
                Size = new Size(500, 40),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopLeft
            };
            parent.Controls.Add(lblNote);
        }

        private void CreateFooter(Guna2Panel parent)
        {
            // ✅ Footer panel
            var pnlFooter = new Panel
            {
                Size = new Size(540, 60),
                Location = new Point(30, 430),
                BackColor = Color.Transparent
            };
            parent.Controls.Add(pnlFooter);

            // ✅ Start button
            var btnStart = new Guna2GradientButton
            {
                Text = GetStartButtonText(),
                Size = new Size(150, 45),
                Location = new Point(250, 8),
                BorderRadius = 15,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.FromArgb(0, 188, 212),
                FillColor2 = SystemColors.Highlight,
                HoverState =
                {
                    FillColor = SystemColors.Highlight,
                    FillColor2 = Color.FromArgb(0, 188, 212)
                },
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnStart.Click += (s, e) => { Result = DialogResult.OK; this.Close(); };
            pnlFooter.Controls.Add(btnStart);

            // ✅ Cancel button
            var btnCancel = new Guna2GradientButton
            {
                Text = GetCancelButtonText(),
                Size = new Size(120, 45),
                Location = new Point(410, 8),
                BorderRadius = 15,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.FromArgb(244, 67, 54),
                FillColor2 = Color.FromArgb(198, 40, 40),
                HoverState =
                {
                    FillColor = Color.FromArgb(198, 40, 40),
                    FillColor2 = Color.FromArgb(244, 67, 54)
                },
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnCancel.Click += (s, e) => { Result = DialogResult.Cancel; this.Close(); };
            pnlFooter.Controls.Add(btnCancel);
        }

        private Bitmap CreateHeaderIcon()
        {
            var bmp = new Bitmap(50, 50);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // ✅ SỬA LinearGradientBrush
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0),                    // ✅ Start point
                    new Point(50, 50),                  // ✅ End point  
                    Color.FromArgb(0, 188, 212),        // ✅ Start color
                    Color.FromArgb(0, 150, 170)))       // ✅ End color
                {
                    g.FillEllipse(brush, 0, 0, 50, 50);
                }

                // ✅ Draw hand icon
                using (var font = new Font("Segoe UI", 20, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var text = "✋";
                    var size = g.MeasureString(text, font);
                    var x = (50 - size.Width) / 2;
                    var y = (50 - size.Height) / 2;
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

        // ✅ Localization methods
        private string GetTitle()
        {
            bool isVietnamese = IsVietnamese();
            string gestureInfo = !string.IsNullOrEmpty(_gestureName) ? $" - {_gestureName}" : "";

            return isVietnamese
                ? $"📋 Hướng dẫn Custom Gesture{gestureInfo}"
                : $"📋 Custom Gesture Instructions{gestureInfo}";
        }

        private string GetStartButtonText()
        {
            return IsVietnamese() ? "🚀 Bắt đầu" : "🚀 Start";
        }

        private string GetCancelButtonText()
        {
            return IsVietnamese() ? "❌ Hủy" : "❌ Cancel";
        }

        private string GetAdditionalNote()
        {
            return IsVietnamese()
                ? "💡 Lưu ý: Đảm bảo môi trường quay phù hợp để có kết quả tốt nhất!"
                : "💡 Note: Ensure proper recording environment for best results!";
        }

        private (string Icon, string Text)[] GetInstructions()
        {
            bool isVietnamese = IsVietnamese();

            if (isVietnamese)
            {
                return new[]
                {
                    ("🎯", "Giữ tay trong khung hình, cách camera 40–60 cm"),
                    ("✋", "Không để tay ra ngoài mép khung"),
                    ("💡", "Đảm bảo đủ ánh sáng"),
                    ("📵", "Không để vật thể khác che tay"),
                    ("⏱", "Giữ tư thế trong 0.8–1.0 giây để mẫu được ghi"),
                    ("🔄", "Lặp lại 5 lần để đảm bảo chất lượng mẫu"),
                    ("🎬", "Camera sẽ tự động ghi nhận gesture của bạn"),
                    ("✅", "Hoàn thành khi đủ 5 mẫu chất lượng")
                };
            }
            else
            {
                return new[]
                {
                    ("🎯", "Keep hands within frame, 40–60 cm from camera"),
                    ("✋", "Don't let hands go outside frame edges"),
                    ("💡", "Ensure sufficient lighting"),
                    ("📵", "Don't let other objects cover hands"),
                    ("⏱", "Hold pose for 0.8–1.0 seconds for sample recording"),
                    ("🔄", "Repeat 5 times to ensure sample quality"),
                    ("🎬", "Camera will automatically capture your gesture"),
                    ("✅", "Complete when 5 quality samples are collected")
                };
            }
        }

        private bool IsVietnamese()
        {
            // ✅ Check current language
            try
            {
                return GestPipePowerPonit.CultureManager.CurrentCultureCode.Contains("vi") ||
                       AppSettings.CurrentLanguage == "VN";
            }
            catch
            {
                return false; // Default to English
            }
        }

        // ✅ Static show method
        public static DialogResult ShowInstructions(string gestureName = "")
        {
            using (var form = new CustomGestureInstructionForm(gestureName))
            {
                return form.ShowDialog();
            }
        }
    }
}