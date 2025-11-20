using System;
using System.Windows.Forms;
using System.Drawing;

namespace GestPipePowerPonit.Services
{
    public class LoadingManager
    {
        private Control loadingPanel;
        private Label loadingLabel;
        private PictureBox spinner;
        private Timer timer;
        private int spinnerAngle;

        public LoadingManager(Control panel, Label label, PictureBox spinner)
        {
            this.loadingPanel = panel;
            this.loadingLabel = label;
            this.spinner = spinner;
            timer = new Timer();
            timer.Interval = 60;
            timer.Tick += Timer_Tick;
        }

        public void Show(string message = "Loading...\nPlease wait...")
        {
            loadingLabel.Text = message;
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            timer.Start();
        }

        public void Hide()
        {
            loadingPanel.Visible = false;
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15;
            if (spinnerAngle >= 360) spinnerAngle = 0;
            DrawSpinner();
        }

        private void DrawSpinner()
        {
            Bitmap spinnerBitmap = new Bitmap(spinner.Width, spinner.Height);
            using (Graphics g = Graphics.FromImage(spinnerBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                int centerX = spinner.Width / 2;
                int centerY = spinner.Height / 2;
                int radius = 20;
                for (int i = 0; i < 8; i++)
                {
                    double angle = (spinnerAngle + i * 45) * Math.PI / 180;
                    int x = centerX + (int)(Math.Cos(angle) * radius);
                    int y = centerY + (int)(Math.Sin(angle) * radius);
                    int alpha = 255 - (i * 30);
                    if (alpha < 0) alpha = 0;
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                    {
                        g.FillEllipse(brush, x - 3, y - 3, 6, 6);
                    }
                }
            }
            spinner.Image = spinnerBitmap;
        }
    }
}