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
    public partial class FormTrainingResult : Form
    {

        public FormTrainingResult(string gestureName, string action, double accuracy, DateTime trainingDay)
        {
            InitializeComponent();

            // Gán giá trị động vào các Labels chứa giá trị (lblValue...)
            this.lblValueName.Text = gestureName;
            this.lblValueAction.Text = action;
            this.lblValueAccuracy.Text = $"{accuracy:F1}%";
            this.lblValueDay.Text = trainingDay.ToString("dd-MM-yyyy");

            // Thiết lập DialogResult là Unknown ban đầu
            this.DialogResult = DialogResult.None;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}