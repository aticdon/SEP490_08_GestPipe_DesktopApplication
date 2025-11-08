using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class FormUserGesture : Form
    {
        private UserGestureConfigService _gestureService = new UserGestureConfigService();
        private HomeUser _homeForm;
        private List<UserGestureConfigDto> gestures;
        private string userId;

        public FormUserGesture(HomeUser homeForm, string userId)
        {
            InitializeComponent();
            this.Load += FormUserGesture_Load;
            _homeForm = homeForm;
            this.userId = userId;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;
        }

        private async void FormUserGesture_Load(object sender, EventArgs e)
        {
            await LoadDefaultGesturesAsync();
        }

        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                gestures = await _gestureService.GetUserGesturesAsync(userId);
                guna2DataGridView1.Rows.Clear();

                foreach (var g in gestures)
                {
                    guna2DataGridView1.Rows.Add(
                        g.Name,
                        g.Type,
                        $"{g.Accuracy * 100:F1}%",   // Hiển thị phần trăm
                        g.Status,
                        g.LastUpdate.ToString("dd-MM-yyyy"),
                        Properties.Resources.icon_view,
                        Properties.Resources.icon_traininggesture,
                        Properties.Resources.CustomCamera
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
            }
        }
        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index && e.RowIndex >= 0)
            {
                var basic = gestures[e.RowIndex];
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);

                if (detail == null)
                {
                    MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string description = _gestureService.GetGestureDescription(detail);
                string instruction = _gestureService.GetInstructionTable(detail);

                var detailForm = new FormGestureDetails(
                    detail.Name,
                    detail.Type,
                    $"{detail.Accuracy * 100:F1}%",
                    detail.Status,
                    detail.LastUpdate.ToString("dd-MM-yyyy"),
                    description,
                    instruction
                );
                detailForm.ShowDialog();
            }
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnCustom"].Index && e.RowIndex >= 0)
            {
                // Lấy thông tin gesture nếu cần
                var basic = gestures[e.RowIndex];
                // Mở FormCustomGesture, truyền HomeUser (hoặc truyền gì bạn cần)
                var customForm = new FormCustomGesture(_homeForm);
                customForm.Show();
                // Nếu muốn, có thể hide form hiện tại: this.Hide();
                this.Hide();
            }
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index && e.RowIndex >= 0)
            {
                var basic = gestures[e.RowIndex];
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                // ... Sau khi lấy detail xong
                Bitmap arrowImg = null;
                string directionStr = "";
                if(detail.Type == "Static")
                {
                    arrowImg = Properties.Resources.handlestaticImg; // Hoặc một hình ảnh phù hợp cho động tác tĩnh
                    directionStr = "Stand Still";
                }
                else
                {
                    if (detail.VectorData.MainAxisX == 1)
                    {
                        if (detail.VectorData.DeltaX > 0)
                        {
                            arrowImg = Properties.Resources.Left_to_right;
                            directionStr = "Left to Right";
                        }
                        else
                        {
                            arrowImg = Properties.Resources.Right_to_left;
                            directionStr = "Right to Left";
                        }
                    }
                    else if (detail.VectorData.MainAxisY == 1)
                    {
                        if (detail.VectorData.DeltaY > 0)
                        {
                            arrowImg = Properties.Resources.Top_to_bottom;
                            directionStr = "Top to Bottom";
                        }
                        else
                        {
                            arrowImg = Properties.Resources.Bottom_to_top;
                            directionStr = "Bottom to Top";
                        }
                    }
                }

                // Truyền tất cả sang form instruction, cùng một nguồn!
                var trainingForm = new FormInstructionTraining(
                    detail.VectorData.Fingers,
                    arrowImg,
                    detail.Name,
                    detail.PoseLabel,
                    detail.Type,
                    directionStr, // truyền string direction
                    this
                );

                trainingForm.GestureDetail = detail;
                trainingForm.SetDirectionText(directionStr); // -> bạn thêm hàm public void SetDirectionText(string txt) { lblDirectionValue.Text = txt; }

                trainingForm.Show();
            }
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }
    }
}