using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class FormDefaultGesture : Form
    {
        private DefaultGestureService _gestureService = new DefaultGestureService();
        private HomeUser _homeForm;
        private List<DefaultGestureDto> gestures;

        public FormDefaultGesture(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += FormDefaultGesture_Load;
            _homeForm = homeForm;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;
        }

        private async void FormDefaultGesture_Load(object sender, EventArgs e)
        {
            await LoadDefaultGesturesAsync();
        }

        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                gestures = await _gestureService.GetDefaultGesturesAsync();
                guna2DataGridView1.Rows.Clear();

                foreach (var g in gestures)
                {
                    guna2DataGridView1.Rows.Add(
                        g.Name,
                        g.Type,
                        $"{g.Accuracy * 100:F1}%",   // Hiển thị phần trăm
                        g.Status,
                        g.LastUpdate.ToString("dd-MM-yyyy"),
                        Properties.Resources.icon_view
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
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }
    }
}
