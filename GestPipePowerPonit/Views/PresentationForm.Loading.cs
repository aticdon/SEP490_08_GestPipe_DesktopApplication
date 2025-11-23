using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class PresentationForm : Form
    {
        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15;
            if (spinnerAngle >= 360) spinnerAngle = 0;
            DrawSpinner();
        }

        private void DrawSpinner()
        {
            Bitmap oldBitmap = loadingSpinner.Image as Bitmap;

            try
            {
                Bitmap spinnerBitmap = new Bitmap(loadingSpinner.Width, loadingSpinner.Height);
                using (Graphics g = Graphics.FromImage(spinnerBitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    int centerX = loadingSpinner.Width / 2;
                    int centerY = loadingSpinner.Height / 2;
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

                loadingSpinner.Image = spinnerBitmap;
                oldBitmap?.Dispose();
            }
            catch
            {
                oldBitmap?.Dispose();
            }
        }

        private void UpdateLoadingText(string message = null)
        {
            if (message == null)
            {
                message = CultureManager.CurrentCultureCode.Contains("vi")
                    ? "Đang tải...\nVui lòng đợi..."
                    : "Loading...\nPlease wait...";
            }

            loadingLabel.Text = message;

            if (loadingPanel.IsHandleCreated && !loadingPanel.IsDisposed)
            {
                this.Invoke(new Action(() =>
                {
                    loadingLabel.Location = new Point(
                        (loadingPanel.Width - loadingLabel.Width) / 2,
                        loadingSpinner.Bottom + 20
                    );
                }));
            }
        }

        private void ShowLoading(string message = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowLoading(message)));
                return;
            }

            isLoading = true;
            UpdateLoadingText(message);
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            spinnerTimer.Start();

            Debug.WriteLine("🔄 Loading screen shown");
        }

        private void HideLoading()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HideLoading));
                return;
            }

            isLoading = false;
            loadingPanel.Visible = false;
            spinnerTimer.Stop();

            Debug.WriteLine("✅ Loading screen hidden");
        }
    }
}
