using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Web.WebView2.WinForms;
using WinFormControl = System.Windows.Forms.Control;

namespace GestPipePowerPonit
{
    public partial class PresentationForm : Form
    {
        private void Extract3DModels(string pptxPath)
        {
            slide3DModelFiles.Clear();

            using (PresentationDocument doc = PresentationDocument.Open(pptxPath, false))
            {
                var presPart = doc.PresentationPart;
                if (presPart?.Presentation?.SlideIdList == null)
                    return;

                var slideIds = presPart.Presentation.SlideIdList.Elements<SlideId>().ToList();

                // Thư mục tạm để lưu các file 3D export ra
                string tempRoot = Path.Combine(Path.GetTempPath(), "GestPipe3D");
                Directory.CreateDirectory(tempRoot);

                string subFolderName = Path.GetFileNameWithoutExtension(pptxPath);
                string tempDir = Path.Combine(tempRoot, subFolderName);
                Directory.CreateDirectory(tempDir);

                for (int i = 0; i < slideIds.Count; i++)
                {
                    var slideId = slideIds[i];
                    var slidePart = (SlidePart)presPart.GetPartById(slideId.RelationshipId);
                    int slideIndex = i + 1;

                    OpenXmlPart modelPart = null;
                    DataPart modelDataPart = null;
                    string ext = ".glb";

                    // 1️⃣ Quét các OpenXmlPart bình thường
                    foreach (var pair in slidePart.Parts)
                    {
                        var p = pair.OpenXmlPart;
                        string ct = (p.ContentType ?? "").ToLowerInvariant();
                        string uri = (p.Uri?.ToString() ?? "").ToLowerInvariant();

                        Debug.WriteLine($"[3D DEBUG] Slide {slideIndex} - Part CT={ct}, Uri={uri}");

                        if (Is3DContent(ct, uri))
                        {
                            modelPart = p;
                            ext = Infer3DExtension(uri, ct);
                            break;
                        }
                    }

                    // 2️⃣ Nếu chưa thấy, quét tiếp DataPartReferenceRelationships
                    if (modelPart == null)
                    {
                        foreach (var dpr in slidePart.DataPartReferenceRelationships)
                        {
                            var dp = dpr.DataPart;
                            string ct = (dp?.ContentType ?? "").ToLowerInvariant();
                            string uri = (dp?.Uri?.ToString() ?? "").ToLowerInvariant();

                            Debug.WriteLine($"[3D DEBUG] Slide {slideIndex} - DataPart CT={ct}, Uri={uri}");

                            if (Is3DContent(ct, uri))
                            {
                                modelDataPart = dp;
                                ext = Infer3DExtension(uri, ct);
                                break;
                            }
                        }
                    }

                    if (modelPart == null && modelDataPart == null)
                    {
                        // Optional: log nếu XML có từ khoá 3D mà không tìm được part
                        string xml = (slidePart.Slide?.OuterXml ?? "");
                        if (xml.IndexOf("model3d", StringComparison.OrdinalIgnoreCase) >= 0
                            || xml.IndexOf("a3d:", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Debug.WriteLine($"[3D DEBUG] Slide {slideIndex} có tag model3d trong XML nhưng không tìm được part 3D");
                        }
                        continue;
                    }

                    string outPath = Path.Combine(tempDir, $"slide_{slideIndex}{ext}");

                    using (var dst = File.Create(outPath))
                    {
                        if (modelPart != null)
                        {
                            using (var src = modelPart.GetStream())
                            {
                                src.CopyTo(dst);
                            }
                        }
                        else
                        {
                            using (var src = modelDataPart.GetStream())
                            {
                                src.CopyTo(dst);
                            }
                        }
                    }

                    slide3DModelFiles[slideIndex] = outPath;
                    Debug.WriteLine($"[3D DEBUG] Slide {slideIndex}: exported 3D model to {outPath}");
                }
            }
        }

        private bool Is3DContent(string contentType, string uri)
        {
            contentType = contentType ?? "";
            uri = uri ?? "";

            contentType = contentType.ToLowerInvariant();
            uri = uri.ToLowerInvariant();

            // ContentType chứa chữ "model" hoặc "3d"
            if (contentType.Contains("model") || contentType.Contains("3d"))
                return true;

            // Đuôi file
            if (uri.EndsWith(".glb") || uri.EndsWith(".gltf"))
                return true;

            if (uri.Contains("/model3d/"))
                return true;

            return false;
        }

        private string Infer3DExtension(string uri, string contentType)
        {
            uri = (uri ?? "").ToLowerInvariant();
            contentType = (contentType ?? "").ToLowerInvariant();

            if (uri.EndsWith(".gltf") || contentType.Contains("gltf+json"))
                return ".gltf";
            if (uri.EndsWith(".glb") || contentType.Contains("gltf-binary"))
                return ".glb";

            if (contentType.Contains("gltf"))
                return ".gltf";

            // fallback: phần lớn các model 3D nhúng trong PPT là .glb
            return ".glb";
        }
        private async Task ShowGLBModelOnAsync(WebView2 target, string glbPath, bool updateRadius)
        {
            if (target == null || target.IsDisposed) return;
            if (!File.Exists(glbPath)) return;

            target.Visible = true;

            string folderPath = Path.GetDirectoryName(glbPath);
            string fileName = Path.GetFileName(glbPath);
            string virtualHostName = "http://virtualhost.local/";

            string html = $@"
    <html>
      <head>
        <meta charset='UTF-8'>
        <script type='module' src='https://unpkg.com/@google/model-viewer/dist/model-viewer.min.js'></script>
        <style>
          html, body {{ width: 100%; height: 100%; margin: 0; background: #222; }}
          model-viewer {{ width: 100vw; height: 100vh; background: #222; }}
        </style>
        <script>
          window.mvReady = false;
          window.addEventListener('DOMContentLoaded', () => {{
            const mv = document.getElementById('mv');
            mv.addEventListener('load', () => {{
              window.mvReady = true;
            }});
            setTimeout(() => {{ window.mvReady = true; }}, 1000);
          }});
        </script>
      </head>
      <body>
        <model-viewer id='mv' src='{virtualHostName}{fileName}'
          camera-controls
          interaction-prompt='none'
          background-color='#222'></model-viewer>
      </body>
    </html>";

            if (target.CoreWebView2 == null)
            {
                await target.EnsureCoreWebView2Async();
                target.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "virtualhost.local",
                    folderPath,
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            }

            target.NavigateToString(html);
        }
        private async void ShowGLBModel(string glbPath)
        {
            try
            {
                // load lên màn chính (có tính initialRadius)
                await ShowGLBModelOnAsync(webView2_3D, glbPath, true);

                // nếu đang có form projector thì load luôn
                if (webView2_3D_External != null)
                {
                    await ShowGLBModelOnAsync(webView2_3D_External, glbPath, false);
                }

                zoomPercent = 100;
                modelAzimuth = 0;
                modelPolar = Math.PI / 2;
                UpdateZoomOverlay();
            }
            catch (Exception ex)
            {
                string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                    ? $"Lỗi khi tải mô hình 3D: {ex.Message}"
                    : $"Error loading 3D model: {ex.Message}";
                Console.WriteLine(errorMsg);
            }
            finally
            {
                HideLoading();
            }
        }
        private void ExecOnAllViewers(string script)
        {
            if (webView2_3D != null && webView2_3D.Visible)
                webView2_3D.ExecuteScriptAsync(script);

            if (webView2_3D_External != null && webView2_3D_External.Visible)
                webView2_3D_External.ExecuteScriptAsync(script);
        }
        private void CheckAndShowGLBForCurrentSlide(Microsoft.Office.Interop.PowerPoint.SlideShowWindow wn = null)
        {
            try
            {
                if (oPPT == null || oPres == null)
                    return;

                Microsoft.Office.Interop.PowerPoint.SlideShowView view = null;

                if (wn != null)
                {
                    // Event từ PowerPoint
                    try
                    {
                        view = wn.View;
                    }
                    catch
                    {
                        return;
                    }
                }
                else
                {
                    // Gọi từ các button First/Next/Prev/Last
                    if (oPPT.SlideShowWindows == null || oPPT.SlideShowWindows.Count == 0)
                        return;

                    view = oPPT.SlideShowWindows[1].View;
                }

                int current = view.CurrentShowPosition;
                if (current <= 0)
                    return;

                // 🔑 Không dùng title nữa, chỉ nhìn map slide3DModelFiles
                if (slide3DModelFiles.TryGetValue(current, out string glbPath) &&
                    File.Exists(glbPath))
                {
                    ShowGLBModel(glbPath);

                    if (!isFullScreenGLB)
                        EnterFullScreenGLB();
                }
                else
                {
                    webView2_3D.Visible = false;

                    if (isFullScreenGLB)
                        ExitFullScreenGLB();
                }
            }
            catch (COMException comEx)
            {
                Debug.WriteLine($"[CheckAndShowGLBForCurrentSlide] COM error: {comEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CheckAndShowGLBForCurrentSlide] error: {ex.Message}");
            }
        }
        private void EnterFullScreenGLB()
        {
            if (isFullScreen || !webView2_3D.Visible) return;
            isFullScreen = true;
            isFullScreenGLB = true;

            // Lưu lại thông tin panel trên màn chính
            panelSlideOriginalSize = panelSlide.Size;
            panelSlideOriginalLocation = panelSlide.Location;

            // Xác định màn hình đang chiếu PowerPoint
            Screen targetScreen;
            if (oPPT != null && oPPT.SlideShowWindows != null && oPPT.SlideShowWindows.Count > 0)
            {
                IntPtr pptHwnd = (IntPtr)oPPT.SlideShowWindows[1].HWND;
                targetScreen = Screen.FromHandle(pptHwnd);   // màn trình chiếu
            }
            else
            {
                targetScreen = Screen.FromControl(this);     // fallback
            }

            fullScreenForm = new Form();
            fullScreenForm.FormBorderStyle = FormBorderStyle.None;
            fullScreenForm.StartPosition = FormStartPosition.Manual;
            fullScreenForm.Bounds = targetScreen.Bounds;     // ⭐ full screen đúng màn hình projector
            fullScreenForm.TopMost = true;
            fullScreenForm.BackColor = Color.Black;
            fullScreenForm.KeyPreview = true;
            fullScreenForm.KeyDown += Form1_KeyDown;

            webView2_3D_External = new WebView2();
            webView2_3D_External.Dock = DockStyle.Fill;
            fullScreenForm.Controls.Add(webView2_3D_External);

            // Overlay zoom như cũ
            lblZoomOverlay = new Label();
            lblZoomOverlay.AutoSize = true;
            lblZoomOverlay.Font = new System.Drawing.Font("Segoe UI", 32, FontStyle.Bold);
            lblZoomOverlay.ForeColor = Color.White;
            lblZoomOverlay.BackColor = Color.FromArgb(200, 30, 30, 30);
            lblZoomOverlay.Visible = false;
            lblZoomOverlay.Top = 20;
            lblZoomOverlay.Left = 20;
            lblZoomOverlay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fullScreenForm.Controls.Add(lblZoomOverlay);

            overlayTimer = new System.Windows.Forms.Timer();
            overlayTimer.Interval = 1500;
            overlayTimer.Tick += (s, e) =>
            {
                lblZoomOverlay.Visible = false;
                overlayTimer.Stop();
            };

            fullScreenForm.Show();
            fullScreenForm.Activate();
        }
        private void ExitFullScreenGLB()
        {
            if (!isFullScreen || !isFullScreenGLB) return;
            isFullScreen = false;
            isFullScreenGLB = false;

            panelSlide.Parent = this;
            panelSlide.Dock = DockStyle.None;
            panelSlide.Size = panelSlideOriginalSize;
            panelSlide.Location = panelSlideOriginalLocation;

            foreach (WinFormControl ctl in this.Controls)
                ctl.Visible = true;
            if (fullScreenForm != null)
            {
                fullScreenForm.Close();
                fullScreenForm.Dispose();
                fullScreenForm = null;
            }
        }

        private void O_PPT_SlideShowNextSlide(Microsoft.Office.Interop.PowerPoint.SlideShowWindow Wn)
        {
            if (this.IsDisposed) return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    CheckAndShowGLBForCurrentSlide(Wn);
                }));
            }
            else
            {
                CheckAndShowGLBForCurrentSlide(Wn);
            }
        }

        private void UpdateModelView()
        {
            ExecOnAllViewers($@"
        if(window.mvReady){{
            let el = document.getElementById('mv');
            let orbit = el.getCameraOrbit();
            let radius = parseFloat(orbit.radius||2.5);
            el.cameraOrbit = '{modelAzimuth}rad {modelPolar}rad ' + radius + 'm';
        }}
    ");

            if (fullScreenForm != null && fullScreenForm.Visible)
            {
                fullScreenForm.Focus();
                fullScreenForm.Activate();
            }
        }

        private void ShowZoomOverlay()
        {
            if (lblZoomOverlay == null || fullScreenForm == null) return;
            lblZoomOverlay.Text = $"Zoom: {zoomPercent}%";
            lblZoomOverlay.Left = (fullScreenForm.ClientSize.Width - lblZoomOverlay.Width) / 2;
            lblZoomOverlay.Visible = true;
            fullScreenForm.Controls.SetChildIndex(lblZoomOverlay, 0);
            overlayTimer.Stop();
            overlayTimer.Start();
        }

        private void UpdateZoomOverlay()
        {
            ShowZoomOverlay();
        }

        private void btnZoomInTop_Click(object sender, EventArgs e)
        {
            zoomPercent += ZOOM_STEP;
            if (zoomPercent > ZOOM_MAX) zoomPercent = ZOOM_MAX;
            ExecOnAllViewers(@"
                if(window.mvReady){
                    let el = document.getElementById('mv');
                    let orbit = el.getCameraOrbit();
                    let az = parseFloat(orbit.azimuth);
                    let polar = parseFloat(orbit.polar);
                    let radius = parseFloat(orbit.radius);
                    radius -= 0.5;
                    if (radius < 0.1) radius = 0.1;
                    el.cameraOrbit = `${az}rad ${polar}rad ${radius}m`;
                }
            ");
            ShowZoomOverlay();
        }

        private void btnZoomOutTop_Click(object sender, EventArgs e)
        {
            zoomPercent -= ZOOM_STEP;
            if (zoomPercent < ZOOM_MIN) zoomPercent = ZOOM_MIN;
            ExecOnAllViewers(@"
                if(window.mvReady){
                    let el = document.getElementById('mv');
                    let orbit = el.getCameraOrbit();
                    let az = parseFloat(orbit.azimuth);
                    let polar = parseFloat(orbit.polar);
                    let radius = parseFloat(orbit.radius);
                    radius += 0.5;
                    el.cameraOrbit = `${az}rad ${polar}rad ${radius}m`;
                }
            ");
            ShowZoomOverlay();
        }
        private void btnViewLeft_Click(object sender, EventArgs e)
        {
            modelAzimuth -= ROTATE_STEP;
            UpdateModelView();
        }

        private void btnViewRight_Click(object sender, EventArgs e)
        {
            modelAzimuth += ROTATE_STEP;
            UpdateModelView();
        }

        private void btnViewTop_Click(object sender, EventArgs e)
        {
            modelPolar -= ROTATE_STEP;
            if (modelPolar < 0.1) modelPolar = 0.1;
            UpdateModelView();
        }

        private void btnViewBottom_Click(object sender, EventArgs e)
        {
            modelPolar += ROTATE_STEP;
            if (modelPolar > Math.PI - 0.1) modelPolar = Math.PI - 0.1;
            UpdateModelView();
        }
    }
}
