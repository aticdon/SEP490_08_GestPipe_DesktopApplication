using GestPipePowerPonit.I18n;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class DetailGestureForm : Form
    {
        // Constructor nhận 7 tham số string
        public DetailGestureForm(string name, string action, string accuracy, string status, string lastUpdate, string description, string instruction)
        {
            InitializeComponent();
            ApplyLanguage();
            // 1. Cài đặt Form
            this.Text = "Gesture Detail";
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            // 2. Đổ dữ liệu cơ bản vào controls Guna.UI2 mới
            lblNameValue.Text = name;
            lblActionValue.Text = action;
            lblAccuracyValue.Text = accuracy;
            lblStatusValue.Text = status;
            lblLastUpdateValue.Text = lastUpdate;
            txtDescriptionValue.Text = description;

            // 3. Phân tích và đổ dữ liệu bảng Instruction
            ParseAndPopulateInstructionTable(instruction);

            // 4. Gán sự kiện nút
            btnCancel.Click += (s, e) => this.Close();
        }

        // Phương thức phân tích chuỗi instruction và điền vào TableLayoutPanel
        private void ParseAndPopulateInstructionTable(string instructionData)
        {
            // Tách chuỗi thành các dòng
            var lines = instructionData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Where(line => !string.IsNullOrWhiteSpace(line) && !line.Contains("---"))
                                      .ToList();

            if (lines.Count < 1) return;

            if (tableLayoutPanelHands.Controls.Count > 2)
            {
                for (int i = tableLayoutPanelHands.Controls.Count - 1; i >= 2; i--)
                {
                    tableLayoutPanelHands.Controls.RemoveAt(i);
                }
                // Xóa RowStyles dư thừa (giữ lại Style cho hàng 0)
                while (tableLayoutPanelHands.RowStyles.Count > 1)
                {
                    tableLayoutPanelHands.RowStyles.RemoveAt(tableLayoutPanelHands.RowStyles.Count - 1);
                }
                tableLayoutPanelHands.RowCount = 1; 
            }

            int rowIndex = 0;

            foreach (var line in lines)
            {
                if (line.Contains("Left Hands")) continue;

                if (line.TrimStart().StartsWith("Direction:") || line.TrimStart().StartsWith("Hướng di chuyển:"))
                {
                    string direction = line.Contains(":") ? line.Split(':')[1].Trim() : line.Trim();
                    lblDirectionValue.Text = direction;
                    continue; // KHÔNG add vào bảng
                }

                try
                {
                    string fingerName = line.Substring(0, 16).Trim().Replace(":", "");
                    string leftState = line.Substring(16, 12).Trim();
                    string rightState = line.Substring(28).Trim();

                    if (string.IsNullOrWhiteSpace(fingerName)) continue;

                    rowIndex++;
                    tableLayoutPanelHands.RowCount = rowIndex + 1;
                    tableLayoutPanelHands.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));

                    // Cột 0: Tên Ngón tay
                    //Label lblFinger = new Label() { Text = fingerName + ":", TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left, Padding = new Padding(10, 0, 0, 0), Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.White };
                    Label lblFinger = new Label()
                    {
                        Text = fingerName + ":",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                        Padding = new Padding(10, 0, 0, 0),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.White,
                        AutoSize = false,
                    };
                    tableLayoutPanelHands.Controls.Add(lblFinger, 0, rowIndex);

                    // Cột 1: Tay Trái
                    Label lblLeft = new Label() { Text = leftState, TextAlign = ContentAlignment.MiddleCenter, Anchor = AnchorStyles.None, Font = new Font("Segoe UI", 10F), ForeColor = Color.White };
                    tableLayoutPanelHands.Controls.Add(lblLeft, 1, rowIndex);

                    // Cột 2: Tay Phải
                    Label lblRight = new Label() { Text = rightState, TextAlign = ContentAlignment.MiddleCenter, Anchor = AnchorStyles.None, Font = new Font("Segoe UI", 10F), ForeColor = Color.White };
                    tableLayoutPanelHands.Controls.Add(lblRight, 2, rowIndex);
                }
                catch { /* Bỏ qua nếu dòng không hợp lệ */ }
            }
        }
        private void ApplyLanguage()
        {
            ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);
            lblNameLabel.Text = Properties.Resources.Col_Name;
            lblActionLabel.Text = Properties.Resources.Col_Type;
            lblAccuracyLabel.Text = Properties.Resources.Col_Accuracy;
            lblStatusLabel.Text = Properties.Resources.Col_Status;
            lblLastUpdateLabel.Text = Properties.Resources.Col_LastUpdate;
            lblInstructionLabel.Text = Properties.Resources.LblInstruction;
            lblDescriptionLabel.Text = Properties.Resources.LblDescription;
            labelLeftHands.Text = Properties.Resources.LblLeftHand;
            labelRightHands.Text = Properties.Resources.LblRightHand;
            lblDirectionLabel.Text = Properties.Resources.LblDirection;
            btnCancel.Text = Properties.Resources.LblClose;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}