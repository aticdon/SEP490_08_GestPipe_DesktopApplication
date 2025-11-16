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
    public partial class TraingResultForm : Form
    {
        //private FormUserGesture _userGestureForm;
        private ListDefaultGestureForm _dfgestureForm;

        //public FormTrainingResult(string gestureName, string action, double accuracy, DateTime trainingDay, FormUserGesture userGestureForm)
        public TraingResultForm(string gestureName, string action, double accuracy, DateTime trainingDay, ListDefaultGestureForm dfgestureForm)
        {
            InitializeComponent();
            ApplyLanguage();
            // Gán giá trị động vào các Labels chứa giá trị (lblValue...)
            this.lblValueName.Text = gestureName;
            this.lblValueAction.Text = action;
            this.lblValueAccuracy.Text = $"{accuracy:F1}%";
            //this.lblValueDay.Text = trainingDay.ToString("dd-MM-yyyy");
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vnTime = TimeZoneInfo.ConvertTimeFromUtc(trainingDay.ToUniversalTime(), vnTimeZone);
            this.lblValueDay.Text = vnTime.ToString("dd-MM-yyyy HH:mm:ss"); // CÓ cả ngày và giờ


            // Thiết lập DialogResult là Unknown ban đầu
            this.DialogResult = DialogResult.None;
            //_userGestureForm = userGestureForm;
            _dfgestureForm = dfgestureForm;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            _dfgestureForm.Show();
        }
        private void ApplyLanguage()
        {
            lblNameGesture.Text = Properties.Resources.Col_Name;
            lblAction.Text = Properties.Resources.Lbl_PoseTarget;
            lblAccuracyTraining.Text = Properties.Resources.Col_Accuracy;
            lblTrainingDay.Text = Properties.Resources.Lbl_TrainingDay;
            lblTitle.Text = Properties.Resources.Lbl_TrainingResult;
            btnClose.Text = Properties.Resources.LblClose;
        }
    }
}