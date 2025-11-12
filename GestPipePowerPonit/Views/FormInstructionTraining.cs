using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GestPipePowerPonit
{
    public partial class FormInstructionTraining : Form
    {
        // Ảnh clenched của mỗi tay và các ngón mở
        private Bitmap leftClenchedImg, rightClenchedImg;
        private Bitmap[] rightFingerImgs; // thumb, index, middle, ring, pinky
        private string gestureName;
        private string gestureType;
        public string gestureAction;
        private FormUserGesture formUserGesture;
        public GestureDetailsDto GestureDetail { get; set; }
        public FormInstructionTraining(int[] fingers, Bitmap arrowImg, string gestureName, string gestureAction, string gestureType, string direction, FormUserGesture parentForm)
        {
            InitializeComponent();
            ApplyLanguage();
            this.gestureAction = gestureAction;
            this.gestureName = gestureName;
            this.gestureType = gestureType;
            this.formUserGesture = parentForm;

            // 🐛 Sửa lỗi 1: Đổi lblDirectionValue -> lblValueDirection
            lblValueDirection.Text = direction;
            lblValueName.Text = gestureName;
            lblValueType.Text = gestureType;

            string noteText = "";
            if (gestureType.ToLower() == "dynamic")
            {
                noteText = "Close the left hand to start the gesture. " + Environment.NewLine +
                    "The right hand moves as required."+ Environment.NewLine
                    +"Open the left hand to end the movement.";
            }
            else if (gestureType.ToLower() == "động")
            {
                noteText = "Hãy nắm chặt tay trái để bắt đầu động tác." + Environment.NewLine +
                    "Tay phải giữ nguyên vị trí trong 1 giây." + Environment.NewLine +
                    "Mở tay trái ra để kết thúc động tác.";
            }
            else if (gestureType.ToLower() == "static")
            {
                noteText = "Close the left hand to start the gesture." + Environment.NewLine +
                    "The right hand remains still for 1 second." + Environment.NewLine +
                    "Open the left hand to end the movement.";
            }
            else if (gestureType.ToLower() == "tĩnh")
            {
                noteText = "Hãy nắm chặt tay trái để bắt đầu động tác." + Environment.NewLine+
                    "Tay phải giữ nguyên vị trí trong 1 giây." + Environment.NewLine+
                    "Mở tay trái ra để kết thúc động tác.";
            }
            else
            {
                noteText = "N/A or Unknown gesture type.";
            }

            // Gán Lưu ý vào Label
            lblValueNote.Text = noteText;

            // Load ảnh từ resource (đổi tên cho đúng nếu cần)
            leftClenchedImg = Properties.Resources.clenchLeftImg;
            rightClenchedImg = Properties.Resources.clenchedImg;
            rightFingerImgs = new Bitmap[] {
                Properties.Resources.thumbImg,
                Properties.Resources.indexImg,
                Properties.Resources.middleImg,
                Properties.Resources.ringImg,
                Properties.Resources.pinkyImg
            };
            //StartPythonProcess();
            ShowInstruction(fingers, arrowImg);
            // ShowInstructionTable(fingers); // Hàm này không còn dùng khi bỏ bảng

        }
        public void SetDirectionText(string txt) { lblValueDirection.Text = txt; }
        public void ShowInstruction(int[] fingers, Bitmap arrowImg)
        {
            // Kích thước cố định của bàn tay
            int itemWidth = 160;
            int itemHeight = 320;

            // Khoảng cách giữa các thành phần
            int componentGap = 20;

            // Line Width
            int lineWidth = 5;

            int requiredContentWidth = itemWidth * 3 + componentGap * 2 + lineWidth; 


            int totalWidth = pictureBoxHandLayer.Width; 
            int totalHeight = itemHeight;

            Bitmap layered = new Bitmap(totalWidth, totalHeight);

            // Tính toán Offset để căn giữa toàn bộ khối (523px) trong PictureBox (674px)
            int offsetX = (totalWidth - requiredContentWidth) / 2; // (674 - 523) / 2 = 75.5 -> 75

            using (Graphics g = Graphics.FromImage(layered))
            {
                g.Clear(Color.Black);

                int currentX = offsetX;
                Rectangle leftRect = new Rectangle(20, 0, itemWidth, itemHeight);
                g.DrawImage(leftClenchedImg, leftRect);

                currentX += itemWidth;
                int separatorX = currentX + (componentGap / 2);

                using (Pen separatorPen = new Pen(Color.BlanchedAlmond, lineWidth))
                {
                    float lineStartX = separatorX - (lineWidth / 2f) -10;
                    g.DrawLine(separatorPen, lineStartX, 0, lineStartX, totalHeight);
                }

                currentX += componentGap; // Bỏ qua khoảng cách và line width

                // 2. Tay phải (Vị trí sau Line)
                int rightRectStartX = currentX + 30;
                Rectangle rightRect = new Rectangle(rightRectStartX, 0, itemWidth, itemHeight);

                // Vẽ bàn tay phải đóng
                g.DrawImage(rightClenchedImg, rightRect);

                // Layer các ngón tay mở lên trên tay phải
                for (int i = 0; i < 5; i++)
                {
                    if (fingers[5 + i] == 1)
                    {
                        g.DrawImage(rightFingerImgs[i], rightRect);
                    }
                }

                currentX = rightRectStartX + itemWidth; // Vị trí sau Tay Phải
                currentX += componentGap; // Thêm khoảng cách

                // 3. Mũi tên 
                //int arrowStartX = currentX;
                int arrowStartX = totalWidth - 20 - itemWidth;
                int arrowHeight = itemHeight / 2;

                Rectangle arrowRect = new Rectangle(
                    arrowStartX,
                    itemHeight / 2 - arrowHeight / 2, // Căn giữa theo chiều dọc
                    itemWidth,
                    arrowHeight
                );

                if (arrowImg != null)
                {
                    g.DrawImage(arrowImg, arrowRect);
                }
            }
            pictureBoxHandLayer.Image = layered;
        }
        private void btnTraining_Click(object sender, EventArgs e)
        {
            var detail = this.GestureDetail;
            string userId = "68fa3209582c17a482c5b11e";
            var homeForm = new HomeUser(userId);
            var trainForm = new FormTrainingGesture(
                homeForm, 
                formUserGesture,
                detail.PoseLabel,
                detail.VectorData,
                this.gestureName);
            trainForm.StartTrainingWithAction(this.gestureAction);
            trainForm.Show();

            // Đóng form hướng dẫn hiện tại
            this.Close();

            // Đóng form FormUserGesture nếu còn mở, dùng tham chiếu vừa lưu
            if (formUserGesture != null && !formUserGesture.IsDisposed)
                formUserGesture.Hide();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ApplyLanguage()
        {
            lblTitleName.Text = Properties.Resources.Col_Name;
            lblTitleType.Text = Properties.Resources.Col_Type;
            lblTitleDirection.Text = Properties.Resources.LblDirection;
            lblTitleInstruction.Text = Properties.Resources.LblInstruction;
            lblTitleNote.Text = Properties.Resources.LblNote;
            btnTraining.Text = Properties.Resources.Btn_Training;
            btnClose.Text = Properties.Resources.LblClose;
        }
    }
}