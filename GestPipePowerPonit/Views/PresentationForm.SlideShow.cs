using GestPipePowerPonit.Models;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class PresentationForm : Form
    {
        private List<int> slidesWith3D = new List<int>();
        private int zoomSlideCount = 0;
        private const int ZOOM_SLIDE_MIN = 0;
        private const int ZOOM_SLIDE_MAX = 5;
        private bool isSlideShow = false;
        private Dictionary<string, int> gestureCounts = new Dictionary<string, int>();
        private void SetButtonsEnabled(bool fileLoaded, bool inSlideShow)
        {
            btnOpen.Enabled = !fileLoaded;

            btnSlideShow.Enabled = fileLoaded && !inSlideShow;
            btnFirst.Enabled = inSlideShow;
            btnPrev.Enabled = inSlideShow;
            btnNext.Enabled = inSlideShow;
            btnLast.Enabled = inSlideShow;
            btnPause.Enabled = inSlideShow;
            btnClose.Enabled = inSlideShow;
            btnViewLeft.Enabled = inSlideShow;
            btnViewRight.Enabled = inSlideShow;
            btnViewTop.Enabled = inSlideShow;
            btnViewBottom.Enabled = inSlideShow;
            btnZoomInTop.Enabled = inSlideShow;
            btnZoomOutTop.Enabled = inSlideShow;
            btnZoomInSlide.Enabled = inSlideShow;
            btnZoomOutSlide.Enabled = inSlideShow;
        }
        private void UpdateSlideLabel(Microsoft.Office.Interop.PowerPoint.SlideShowWindow wn = null)
        {
            try
            {
                if (oPPT == null || oPres == null) return;

                Microsoft.Office.Interop.PowerPoint.SlideShowView view;
                if (wn != null)
                {
                    view = wn.View;
                }
                else
                {
                    if (oPPT.SlideShowWindows == null || oPPT.SlideShowWindows.Count == 0)
                        return;
                    view = oPPT.SlideShowWindows[1].View;
                }

                int current = view.CurrentShowPosition;
                int total = oPres.Slides.Count;
                lblSlide.Text = $"Slide {current} / {total}";

                bool has3D = slidesWith3D.Contains(current);

                btnViewLeft.Visible = has3D;
                btnViewRight.Visible = has3D;
                btnViewTop.Visible = has3D;
                btnViewBottom.Visible = has3D;
                btnZoomInTop.Visible = has3D;
                btnZoomOutTop.Visible = has3D;
            }
            catch { }
        }
        private void btnSlideShow_Click(object sender, EventArgs e)
        {
            webView2_3D.Visible = false;
            pictureBoxCamera.Visible = false;
            isSlideShow = true;
            _startTime = DateTime.UtcNow;

            if (oPres != null)
            {
                oPres.SlideShowSettings.ShowWithNarration = MsoTriState.msoFalse;
                oPres.SlideShowSettings.ShowWithAnimation = MsoTriState.msoTrue;
                oPres.SlideShowSettings.LoopUntilStopped = MsoTriState.msoFalse;
                oPres.SlideShowSettings.Run();

                if (oPPT.SlideShowWindows.Count > 0)
                {
                    IntPtr pptHandle = (IntPtr)oPPT.SlideShowWindows[1].HWND;
                    SetForegroundWindow(pptHandle);
                }
            }
            SetButtonsEnabled(true, true);
            zoomSlideCount = 0;

            UpdateSlideLabel();
            CheckAndShowGLBForCurrentSlide();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                oPPT.SlideShowWindows[1].View.First();
                UpdateSlideLabel();
                CheckAndShowGLBForCurrentSlide();
                zoomSlideCount = 0;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                int current = oPPT.SlideShowWindows[1].View.CurrentShowPosition;
                if (current > 1)
                {
                    oPPT.SlideShowWindows[1].View.Previous();
                    UpdateSlideLabel();
                    CheckAndShowGLBForCurrentSlide();
                    zoomSlideCount = 0;
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                int current = oPPT.SlideShowWindows[1].View.CurrentShowPosition;
                int total = oPres.Slides.Count;
                if (current < total)
                {
                    oPPT.SlideShowWindows[1].View.Next();
                    UpdateSlideLabel();
                    CheckAndShowGLBForCurrentSlide();
                    zoomSlideCount = 0;
                }
            }
        }
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                oPPT.SlideShowWindows[1].View.Last();
                UpdateSlideLabel();
                CheckAndShowGLBForCurrentSlide();
                zoomSlideCount = 0;
            }
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (autoSlideTimer.Enabled)
            {
                autoSlideTimer.Stop();
                btnPause.Text = "Resume";
            }
            else
            {
                autoSlideTimer.Start();
                btnPause.Text = "Pause";
            }
        }
        private async void btnClose_Click(object sender, EventArgs e)
        {
            if (isSlideShow)
            {
                HideLoading();
                DateTime endTime = DateTime.UtcNow;
                double duration = 0;
                if (_startTime.HasValue)
                    duration = (endTime - _startTime.Value).TotalSeconds;

                var session = new Session
                {
                    Id = "",
                    UserId = userId,
                    CategoryId = cmbCategory.SelectedValue?.ToString(),
                    TopicId = cmbTopic.SelectedValue?.ToString(),
                    Records = gestureCounts,
                    Duration = duration,
                    CreatedAt = DateTime.UtcNow
                };

                bool success = await sessionService.SaveSessionAsync(session);
                if (success)
                    Console.WriteLine("Lưu session thành công!");
                else
                    Console.WriteLine("Lỗi lưu session!");
                if (oPPT != null)
                {
                    try { oPPT.SlideShowNextSlide -= O_PPT_SlideShowNextSlide; } catch { }
                }
                if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
                {
                    oPPT.SlideShowWindows[1].View.Exit();
                    lblSlide.Text = "Slide - / -";
                }
                webView2_3D.Visible = false;
                //SetButtonsEnabled(false, false);
                SetButtonsEnabled(true, false);
                btnSlideShow.Enabled = false;
                isSlideShow = false;
                try
                {
                    if (oPres != null) { oPres.Close(); oPres = null; }
                    if (oPPT != null) { oPPT.Quit(); oPPT = null; }
                }
                catch { }
                CleanupResources(keepPython: false);
            }
            else { return; }
        }
        private void btnZoomInSlide_Click(object sender, EventArgs e)
        {
            try
            {
                if (zoomSlideCount < ZOOM_SLIDE_MAX && oPPT != null)
                {
                    IntPtr pptHandle;
                    // Nếu đang ở SlideShow
                    if (oPPT.SlideShowWindows.Count > 0)
                        pptHandle = (IntPtr)oPPT.SlideShowWindows[1].HWND;
                    // Nếu ở Edit View
                    else if (oPPT.ActiveWindow != null)
                        pptHandle = (IntPtr)oPPT.ActiveWindow.HWND;
                    else
                        return;

                    SetForegroundWindow(pptHandle);
                    Thread.Sleep(100); // Đợi cửa sổ PowerPoint foreground
                    SendKeys.SendWait("^{+}");

                    zoomSlideCount++;
                    if (zoomSlideCount > ZOOM_SLIDE_MAX)
                        zoomSlideCount = ZOOM_SLIDE_MAX;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Zoom in error: " + ex.Message);
            }
        }

        private void btnZoomOutSlide_Click(object sender, EventArgs e)
        {
            try
            {
                if (zoomSlideCount > ZOOM_SLIDE_MIN && oPPT != null)
                {
                    IntPtr pptHandle;
                    // Nếu đang ở SlideShow
                    if (oPPT.SlideShowWindows.Count > 0)
                        pptHandle = (IntPtr)oPPT.SlideShowWindows[1].HWND;
                    // Nếu ở Edit View
                    else if (oPPT.ActiveWindow != null)
                        pptHandle = (IntPtr)oPPT.ActiveWindow.HWND;
                    else
                        return;

                    SetForegroundWindow(pptHandle);
                    Thread.Sleep(100); // Đợi cửa sổ PowerPoint foreground
                    SendKeys.SendWait("^{-}");

                    zoomSlideCount--;
                    if (zoomSlideCount < ZOOM_SLIDE_MIN)
                        zoomSlideCount = ZOOM_SLIDE_MIN;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Zoom in error: " + ex.Message);
            }
        }
    }
}
