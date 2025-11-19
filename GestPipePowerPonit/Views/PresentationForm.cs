using DnsClient.Protocol;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using GestPipePowerPonit.Views;
using Microsoft.Office.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using A = DocumentFormat.OpenXml.Drawing;
using OpenXmlShape = DocumentFormat.OpenXml.Presentation.Shape;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using WinFormControl = System.Windows.Forms.Control;

namespace GestPipePowerPonit
{
    public partial class PresentationForm : Form
    {
        PowerPoint.Application oPPT = null;
        PowerPoint.Presentation oPres = null;
        private bool isPaused = false;
        private System.Windows.Forms.Timer autoSlideTimer;
        private int slideInterval = 2000;

        private bool isFullScreen = false;
        private Form fullScreenForm = null;
        private bool isFullScreenGLB = false;
        private List<int> slidesWith3D = new List<int>();
        private List<string> slideTitles = new List<string>();
        private string pptxFolderPath = "";

        private WebView2 webView2_3D;

        private double modelAzimuth = 0;
        private double modelPolar = Math.PI / 2;
        private const double ROTATE_STEP = Math.PI / 4;

        private Size panelSlideOriginalSize;
        private Point panelSlideOriginalLocation;

        private const double minRadius = 0.1;
        private const double maxRadius = 10.0;

        private double initialRadius = 0;

        private int zoomPercent = 100;
        private const int ZOOM_STEP = 10;
        private const int ZOOM_MIN = 100;
        private const int ZOOM_MAX = 200;

        private SocketServer server;
        private TcpListener cameraListener;
        private Thread cameraThread;
        private bool cameraRunning = false;

        private int zoomSlideCount = 0;
        private const int ZOOM_SLIDE_MIN = 0;
        private const int ZOOM_SLIDE_MAX = 5;

        private Process pythonProcess = null;
        private HomeUser _homeForm;
        private SessionService sessionService = new SessionService();
        private CategoryService categoryService = new CategoryService();
        private TopicService topicService = new TopicService();
        private DateTime? _startTime = null;
        private Dictionary<string, int> gestureCounts = new Dictionary<string, int>();
        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private bool firstCameraFrameReceived = false;

        // ✅ THÊM AuthService
        private readonly AuthService _authService;

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public PresentationForm(HomeUser homeForm)
        {
            InitializeComponent();
            btnViewLeft.Visible = false;
            btnViewRight.Visible = false;
            btnViewTop.Visible = false;
            btnViewBottom.Visible = false;
            btnZoomInTop.Visible = false;
            btnZoomOutTop.Visible = false;
            btnZoomInSlide.Visible = false;
            btnZoomOutSlide.Visible = false;

            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            autoSlideTimer = new System.Windows.Forms.Timer();

            webView2_3D = new WebView2();
            webView2_3D.Location = new Point(0, 0);
            webView2_3D.Size = panelSlide.Size;
            webView2_3D.Visible = false;
            webView2_3D.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelSlide.Controls.Add(webView2_3D);

            _homeForm = homeForm;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            // ✅ GẮN SỰ KIỆN LOGOUT VÀ PROFILE
            btnLogout.Click += btnLogout_Click;
            btnProfile.Click += btnProfile_Click;

            _apiClient = new ApiClient("https://localhost:7219");

            // ✅ KHỞI TẠO AuthService
            _authService = new AuthService();

            CultureManager.CultureChanged += async (s, e) =>
            {
                ApplyLanguage(CultureManager.CurrentCultureCode);
            };
        }

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
        private void UpdateSlideLabel()
        {
            try
            {
                int current = oPPT.SlideShowWindows[1].View.CurrentShowPosition;
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

        //private void StartCameraReceiver(int port = 6000)
        //{
        //    try
        //    {
        //        cameraListener = new TcpListener(System.Net.IPAddress.Any, port);
        //        cameraListener.Start();
        //        cameraRunning = true;
        //        cameraThread = new Thread(() =>
        //        {
        //            try
        //            {
        //                using (TcpClient client = cameraListener.AcceptTcpClient())
        //                using (NetworkStream ns = client.GetStream())
        //                {
        //                    while (cameraRunning)
        //                    {
        //                        byte[] lengthBytes = new byte[4];
        //                        int bytesRead = 0;
        //                        while (bytesRead < 4)
        //                        {
        //                            int r = ns.Read(lengthBytes, bytesRead, 4 - bytesRead);
        //                            if (r <= 0) return;
        //                            bytesRead += r;
        //                        }
        //                        int length = System.BitConverter.ToInt32(lengthBytes.Reverse().ToArray(), 0);
        //                        byte[] imageBytes = new byte[length];
        //                        int read = 0;
        //                        while (read < length)
        //                        {
        //                            int r = ns.Read(imageBytes, read, length - read);
        //                            if (r <= 0) return;
        //                            read += r;
        //                        }
        //                        using (var ms = new MemoryStream(imageBytes))
        //                        {
        //                            var img = Image.FromStream(ms);
        //                            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                            this.Invoke(new Action(() =>
        //                            {
        //                                pictureBoxCamera.Image = img;
        //                            }));
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                //MessageBox.Show("Camera thread error: " + ex.Message);
        //                Debug.WriteLine("Camera thread error: " + ex.ToString());
        //            }
        //        });
        //        cameraThread.IsBackground = true;
        //        cameraThread.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("StartCameraReceiver error: " + ex.Message);
        //        Debug.WriteLine("StartCameraReceiver error: " + ex.ToString());
        //    }
        //}
        private void StartCameraReceiver(int port = 6000)
        {
            try
            {
                cameraListener = new TcpListener(System.Net.IPAddress.Any, port);
                cameraListener.Start();
                cameraRunning = true;

                cameraThread = new Thread(() =>
                {
                    try
                    {
                        Debug.WriteLine($"[Camera] Waiting for connection on port {port}...");

                        using (TcpClient client = cameraListener.AcceptTcpClient())
                        using (NetworkStream ns = client.GetStream())
                        {
                            Debug.WriteLine("[Camera] Client connected!");

                            while (cameraRunning)
                            {
                                byte[] lengthBytes = new byte[4];
                                int bytesRead = 0;

                                while (bytesRead < 4)
                                {
                                    int r = ns.Read(lengthBytes, bytesRead, 4 - bytesRead);
                                    if (r <= 0)
                                    {
                                        Debug.WriteLine("[Camera] Connection closed while reading length");
                                        return;
                                    }
                                    bytesRead += r;
                                }

                                int length = System.BitConverter.ToInt32(lengthBytes.Reverse().ToArray(), 0);

                                // Validate frame size
                                if (length < 1000 || length > 10000000)
                                {
                                    Debug.WriteLine($"[Camera] Invalid frame size: {length}");
                                    continue;
                                }

                                byte[] imageBytes = new byte[length];
                                int read = 0;

                                while (read < length)
                                {
                                    int r = ns.Read(imageBytes, read, length - read);
                                    if (r <= 0)
                                    {
                                        Debug.WriteLine("[Camera] Connection closed while reading image");
                                        return;
                                    }
                                    read += r;
                                }

                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    try
                                    {
                                        var img = Image.FromStream(ms);
                                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);

                                        this.Invoke(new Action(() =>
                                        {
                                            var oldImage = pictureBoxCamera.Image;
                                            pictureBoxCamera.Image = img;
                                            oldImage?.Dispose();

                                            // ✅ ĐÁNh DẤU ĐÃ NHẬN FRAME ĐẦU TIÊN
                                            if (!firstCameraFrameReceived)
                                            {
                                                firstCameraFrameReceived = true;
                                                Debug.WriteLine("✅ First camera frame received!");
                                            }
                                        }));

                                        //Debug.WriteLine($"[Camera] Frame displayed ({length} bytes)");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"[Camera] Error decoding frame: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Camera] Thread error: {ex.Message}");
                    }
                    finally
                    {
                        Debug.WriteLine("[Camera] Thread exited");
                    }
                });

                cameraThread.IsBackground = true;
                cameraThread.Start();

                Debug.WriteLine($"[Camera] Receiver thread started on port {port}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Camera] StartCameraReceiver error: {ex.Message}");
            }
        }
        //private void StopCameraReceiver()
        //{
        //    cameraRunning = false;
        //    try
        //    {
        //        cameraListener?.Stop();
        //        cameraThread?.Abort();
        //    }
        //    catch { }
        //}
        private void StopCameraReceiver()
        {
            Debug.WriteLine("[Camera] Stopping camera receiver...");

            cameraRunning = false;
            firstCameraFrameReceived = false; // ✅ Reset flag

            try
            {
                cameraListener?.Stop();

                if (cameraThread != null && cameraThread.IsAlive)
                {
                    if (!cameraThread.Join(2000)) // Chờ 2 giây
                    {
                        Debug.WriteLine("[Camera] Thread did not stop gracefully, aborting...");
                        cameraThread.Abort();
                    }
                }

                Debug.WriteLine("[Camera] Camera receiver stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Camera] Error stopping camera: {ex.Message}");
            }
        }

        private List<int> Detect3DSlides(string pptxPath)
        {
            var result = new List<int>();
            using (PresentationDocument doc = PresentationDocument.Open(pptxPath, false))
            {
                var presPart = doc.PresentationPart;
                var slides = presPart.SlideParts.ToList();
                for (int i = 0; i < slides.Count; i++)
                {
                    var sp = slides[i];
                    bool found = false;
                    foreach (var pair in sp.Parts)
                    {
                        var p = pair.OpenXmlPart;
                        string ct = (p.ContentType ?? "").ToLower();
                        string uri = (p.Uri?.ToString() ?? "").ToLower();
                        if (ct.Contains("model/gltf") || ct.Contains("model3d") ||
                            uri.EndsWith(".glb") || uri.Contains("/model3d/"))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (var dpr in sp.DataPartReferenceRelationships)
                        {
                            var dp = dpr.DataPart;
                            string ct = (dp?.ContentType ?? "").ToLower();
                            string uri = (dp?.Uri?.ToString() ?? "").ToLower();
                            if (ct.Contains("model/gltf") || ct.Contains("model3d") ||
                                uri.EndsWith(".glb") || uri.Contains("/model3d/"))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        string xml = (sp.Slide?.OuterXml ?? "").ToLower();
                        if (xml.Contains("model3d") || xml.Contains("a3d:"))
                        {
                            found = true;
                        }
                    }
                    if (found) result.Add(i + 1);
                }
            }
            return result;
        }

        private List<string> GetSlideTitles(string pptxPath)
        {
            var titles = new List<string>();
            using (PresentationDocument presentationDocument = PresentationDocument.Open(pptxPath, false))
            {
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                var slideIds = presentationPart.Presentation.SlideIdList.ChildElements;

                foreach (SlideId slideId in slideIds)
                {
                    SlidePart slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId);
                    string title = null;
                    foreach (OpenXmlShape shape in slidePart.Slide.Descendants<OpenXmlShape>())
                    {
                        var ph = shape.NonVisualShapeProperties?.ApplicationNonVisualDrawingProperties?.GetFirstChild<PlaceholderShape>();
                        if (ph != null && (ph.Type == PlaceholderValues.Title || ph.Type == PlaceholderValues.CenteredTitle))
                        {
                            title = string.Join(" ", shape.TextBody.Descendants<A.Text>().Select(t => t.Text));
                            break;
                        }
                    }
                    if (title == null)
                    {
                        var firstShape = slidePart.Slide.Descendants<OpenXmlShape>().FirstOrDefault(s => s.TextBody != null);
                        if (firstShape != null)
                        {
                            title = string.Join(" ", firstShape.TextBody.Descendants<A.Text>().Select(t => t.Text));
                        }
                    }
                    titles.Add(title ?? "");
                }
            }
            return titles;
        }

        private async void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PowerPoint Files|*.pptx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Cleanup PowerPoint
                    if (oPres != null)
                    {
                        oPres.Close();
                        Marshal.FinalReleaseComObject(oPres);
                        oPres = null;
                    }
                    if (oPPT != null)
                    {
                        oPPT.Quit();
                        Marshal.FinalReleaseComObject(oPPT);
                        oPPT = null;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    foreach (var process in System.Diagnostics.Process.GetProcessesByName("POWERPNT"))
                    {
                        process.Kill();
                    }

                    txtFile.Text = openFileDialog1.FileName;
                    string ext = System.IO.Path.GetExtension(txtFile.Text).ToLower();

                    firstCameraFrameReceived = false;

                    // Start Python process
                    await Task.Run(() =>
                    {
                        StartPythonProcess();
                    });

                    RegisterKeys();
                    server = new SocketServer(5006, this, HandleGestureCommand);
                    server.Start();

                    // -- Đến bước này bạn chỉ show loading khi chờ CAMERA --
                    ShowLoading(
                        CultureManager.CurrentCultureCode.Contains("vi")
                            ? "🎥 Đang kết nối camera..." : "🎥 Connecting camera..."
                    );
                    // Start camera receiver
                    StartCameraReceiver(6000);
                    pictureBoxCamera.Visible = true;

                    if (ext == ".pptx")
                    {
                        webView2_3D.Visible = false;
                        SetButtonsEnabled(true, false);

                        await Task.Run(() =>
                        {
                            slidesWith3D = Detect3DSlides(txtFile.Text);
                            pptxFolderPath = System.IO.Path.GetDirectoryName(txtFile.Text);
                            slideTitles = GetSlideTitles(txtFile.Text);
                        });

                        try
                        {
                            oPPT = new PowerPoint.Application();
                            oPres = oPPT.Presentations.Open(txtFile.Text,
                                MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

                            oPPT.SlideShowNextSlide += O_PPT_SlideShowNextSlide;
                            btnSlideShow.Enabled = true;

                            // *** KHÔNG update lại loading hoặc success nữa ***
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                                ? $"Không thể mở PowerPoint: {ex.Message}"
                                : $"Cannot open PowerPoint: {ex.Message}";
                            MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            oPPT = null;
                            oPres = null;
                            btnSlideShow.Enabled = false;
                        }
                    }

                    // *** CHỈ update loading panel nội dung trong WaitForCameraConnectionAsync ***
                    // *** CHỈ ẩn loading tại đây sau khi camera đã xong ***
                    await WaitForCameraConnectionAsync();
                    HideLoading();
                }
                catch (Exception ex)
                {
                    string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? $"Lỗi: {ex.Message}"
                        : $"Error: {ex.Message}";
                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    HideLoading(); // dù lỗi cũng ẩn loading
                }
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 1: btnFirst_Click(null, null); break;
                    case 2: btnPrev_Click(null, null); break;
                    case 3: btnNext_Click(null, null); break;
                    case 4: btnLast_Click(null, null); break;
                    case 5: btnPause_Click(null, null); break;
                    case 6: btnClose_Click(null, null); break;
                    case 7: btnViewLeft_Click(null, null); break;
                    case 8: btnViewRight_Click(null, null); break;
                    case 9: btnViewTop_Click(null, null); break;
                    case 10: btnViewBottom_Click(null, null); break;
                    case 11: btnZoomInTop_Click(null, null); break;
                    case 12: btnZoomOutTop_Click(null, null); break;
                    case 13: btnZoomInSlide_Click(null, null); break;
                    case 14: btnZoomOutSlide_Click(null, null); break;
                }
            }
            base.WndProc(ref m);
        }

        private void RegisterKeys()
        {
            RegisterHotKey(this.Handle, 1, 0, (int)Keys.D1);
            RegisterHotKey(this.Handle, 2, 0, (int)Keys.D2);
            RegisterHotKey(this.Handle, 3, 0, (int)Keys.D3);
            RegisterHotKey(this.Handle, 4, 0, (int)Keys.D4);
            RegisterHotKey(this.Handle, 5, 0, (int)Keys.D5);
            RegisterHotKey(this.Handle, 6, 0, (int)Keys.Escape);
            RegisterHotKey(this.Handle, 7, 0, (int)Keys.Q);
            RegisterHotKey(this.Handle, 8, 0, (int)Keys.W);
            RegisterHotKey(this.Handle, 9, 0, (int)Keys.E);
            RegisterHotKey(this.Handle, 10, 0, (int)Keys.R);
            RegisterHotKey(this.Handle, 11, 0, (int)Keys.A);
            RegisterHotKey(this.Handle, 12, 0, (int)Keys.S);
            RegisterHotKey(this.Handle, 13, 0, (int)Keys.Z);
            RegisterHotKey(this.Handle, 14, 0, (int)Keys.X);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterKeys();
            base.OnHandleDestroyed(e);
        }

        private void UnregisterKeys()
        {
            for (int i = 1; i <= 14; i++)
                UnregisterHotKey(this.Handle, i);
        }
        private async void ShowGLBModel(string glbPath)
        {
            //string loadingMsg = CultureManager.CurrentCultureCode.Contains("vi")
            //    ? "Đang tải mô hình 3D...\nVui lòng đợi..."
            //    : "Loading 3D model...\nPlease wait...";
            //ShowLoading(loadingMsg);

            try
            {
                webView2_3D.Visible = true;

                if (!File.Exists(glbPath))
                {
                    //HideLoading();
                    //MessageBox.Show("Không tìm thấy file GLB: " + glbPath);
                    return;
                }

                if (webView2_3D.Parent != panelSlide)
                {
                    //HideLoading();
                    //MessageBox.Show("WebView2 không nằm trong panelSlide!");
                    return;
                }

                string folderPath = System.IO.Path.GetDirectoryName(glbPath);
                string fileName = System.IO.Path.GetFileName(glbPath);
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
                  background-color='#222' ></model-viewer>
              </body>
            </html>";

                if (webView2_3D.CoreWebView2 == null)
                {
                    await webView2_3D.EnsureCoreWebView2Async();
                    webView2_3D.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        "virtualhost.local",
                        folderPath,
                        Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
                }

                webView2_3D.NavigateToString(html);

                // ✅ Chờ model load
                await Task.Delay(1500);

                var result = await webView2_3D.ExecuteScriptAsync(@"
            (function() {
                let el = document.getElementById('mv');
                if (el && el.getCameraOrbit) {
                    let orbit = el.getCameraOrbit();
                    return orbit.radius;
                }
                return '';
            })();
        ");

                result = result.Trim('"');
                if (double.TryParse(result, out double radius))
                {
                    initialRadius = radius;
                }
                else
                {
                    initialRadius = 2.5;
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

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideLoading();
            }
        }
        private void CheckAndShowGLBForCurrentSlide()
        {
            if (oPPT == null || oPres == null || slideTitles.Count == 0) return;
            int current = oPPT.SlideShowWindows[1].View.CurrentShowPosition;
            if (current <= 0 || current > slideTitles.Count) return;
            string currTitle = slideTitles[current - 1]?.Trim();

            if (!string.IsNullOrEmpty(currTitle) && currTitle.StartsWith("3D"))
            {
                string glbFileName = currTitle + ".glb";
                string glbPath = System.IO.Path.Combine(pptxFolderPath, glbFileName);
                if (System.IO.File.Exists(glbPath))
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
            else
            {
                webView2_3D.Visible = false;
                if (isFullScreenGLB)
                    ExitFullScreenGLB();
            }
        }
        private void btnSlideShow_Click(object sender, EventArgs e)
        {
            webView2_3D.Visible = false;
            pictureBoxCamera.Visible = false;
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
                MessageBox.Show("Lỗi lưu session!");
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                oPPT.SlideShowWindows[1].View.Exit();
                lblSlide.Text = "Slide - / -";
            }
            webView2_3D.Visible = false;
            SetButtonsEnabled(false, false);
            try
            {
                if (oPres != null) { oPres.Close(); oPres = null; }
                if (oPPT != null) { oPPT.Quit(); oPPT = null; }
            }
            catch { }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (panelSlide.Parent == this && !isFullScreen)
            {
                panelSlide.Size = new Size((int)(ClientSize.Width * 0.98f), (int)(ClientSize.Height * 0.74f));
                panelSlide.Location = new Point((ClientSize.Width - panelSlide.Width) / 2,
                                                (int)(ClientSize.Height * 0.24f));
                webView2_3D.Size = panelSlide.Size;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1: case Keys.NumPad1: btnFirst.PerformClick(); break;
                case Keys.D2: case Keys.NumPad2: btnPrev.PerformClick(); break;
                case Keys.D3: case Keys.NumPad3: btnNext.PerformClick(); break;
                case Keys.D4: case Keys.NumPad4: btnLast.PerformClick(); break;
                case Keys.D5: case Keys.NumPad5: btnPause.PerformClick(); break;
                case Keys.D6: case Keys.NumPad6: btnClose.PerformClick(); break;
                case Keys.Q: btnViewLeft_Click(null, null); break;
                case Keys.W: btnViewRight_Click(null, null); break;
                case Keys.E: btnViewTop_Click(null, null); break;
                case Keys.R: btnViewBottom_Click(null, null); break;
                case Keys.A: btnZoomInTop_Click(null, null); break;
                case Keys.S: btnZoomOutTop_Click(null, null); break;
                case Keys.Z: btnZoomInSlide_Click(null, null); break;
                case Keys.X: btnZoomOutSlide_Click(null, null); break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupResources();
            base.OnFormClosing(e);
        }

        private void panelSlide_Paint(object sender, PaintEventArgs e) { }

        private async void Form1_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            pictureBoxCamera.Visible = false;
            SetButtonsEnabled(false, false);
            var categories = await categoryService.GetCategoriesAsync();
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "DisplayName";
            cmbCategory.ValueMember = "Id";
        }

        private void btnZoomInTop_Click(object sender, EventArgs e)
        {
            zoomPercent += ZOOM_STEP;
            if (zoomPercent > ZOOM_MAX) zoomPercent = ZOOM_MAX;
            webView2_3D.ExecuteScriptAsync(@"
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
            webView2_3D.ExecuteScriptAsync(@"
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

        private void SetModelView(double azimuth, double polar)
        {
            webView2_3D.ExecuteScriptAsync($@"
            if(window.mvReady){{
                let el = document.getElementById('mv');
                let orbit = el.getCameraOrbit();
                let radius = parseFloat(orbit.radius||2.5);
                el.cameraOrbit = '{azimuth}rad {polar}rad ' + radius + 'm';
            }}
            ");
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

        private void EnterFullScreenGLB()
        {
            if (isFullScreen || !webView2_3D.Visible) return;
            isFullScreen = true;
            isFullScreenGLB = true;

            panelSlideOriginalSize = panelSlide.Size;
            panelSlideOriginalLocation = panelSlide.Location;

            fullScreenForm = new Form();
            fullScreenForm.FormBorderStyle = FormBorderStyle.None;
            fullScreenForm.WindowState = FormWindowState.Maximized;
            fullScreenForm.TopMost = true;
            fullScreenForm.BackColor = Color.Black;
            fullScreenForm.KeyPreview = true;
            fullScreenForm.StartPosition = FormStartPosition.Manual;

            fullScreenForm.KeyDown += Form1_KeyDown;

            panelSlide.Parent = fullScreenForm;
            panelSlide.Dock = DockStyle.Fill;

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

            foreach (WinFormControl ctl in this.Controls)
                ctl.Visible = false;

            fullScreenForm.Show();
            fullScreenForm.Activate();
            //fullScreenForm.ActiveControl = null;
        }

        private void SwitchToPowerPointFromGLB()
        {
            ExitFullScreenGLB();
            if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
            {
                oPPT.SlideShowWindows[1].Activate();
            }
            else if (oPres != null)
            {
                oPres.SlideShowSettings.Run();
            }
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

        private void O_PPT_SlideShowNextSlide(PowerPoint.SlideShowWindow Wn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    CheckAndShowGLBForCurrentSlide();
                }));
            }
            else
            {
                CheckAndShowGLBForCurrentSlide();
            }
        }
        private void UpdateModelView()
        {
            webView2_3D.ExecuteScriptAsync($@"
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

        private void HandleGestureCommand(string command)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleGestureCommand(command)));
                return;
            }

            Debug.WriteLine($"[HandleGestureCommand] Received: {command}");
            if (gestureCounts.ContainsKey(command))
                gestureCounts[command]++;
            else
                gestureCounts[command] = 1;
            switch (command)
            {
                case "zoom_in": btnZoomInTop_Click(null, null); Console.WriteLine("Zoom in"); break;
                case "zoom_out": btnZoomOutTop_Click(null, null); Console.WriteLine("Zoom out"); break;
                case "next_slide": btnNext_Click(null, null); Console.WriteLine("Next Slide"); break;
                case "previous_slide": btnPrev_Click(null, null); Console.WriteLine("Previous Slide"); break;
                case "home": btnFirst_Click(null, null); Console.WriteLine("Home"); break;
                case "end": btnLast_Click(null, null); Console.WriteLine("End"); break;
                case "rotate_left": btnViewRight_Click(null, null); Console.WriteLine("Rotate Left"); break;
                case "rotate_right": btnViewLeft_Click(null, null); Console.WriteLine("Rotate Right"); break;
                case "rotate_up": btnViewBottom_Click(null, null); Console.WriteLine("Rotate Up"); break;
                case "rotate_down": btnViewTop_Click(null, null); Console.WriteLine("Rotate Down"); break;
                //case "Slide_show": btnSlideShow_Click(null, null); break;
                default: MessageBox.Show($"Nhận lệnh không xác định: {command}"); break;
            }
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
                MessageBox.Show("Zoom in error: " + ex.Message);
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
                MessageBox.Show("Zoom in error: " + ex.Message);
            }
        }
        private void StartPythonProcess()
        {
            try
            {
                string pythonExePath = "python"; // hoặc @"C:\Users\THUCLTCE171961\AppData\Local\Programs\Python\Python39\python.exe"
                string userFolder = $"user_{userId}";
                string scriptFile = $@"D:\Semester9\codepython\hybrid_realtime_pipeline\code\{userFolder}\test_gesture_recognition.py";
                //string userArgument = $"user_{userId}";

                Debug.WriteLine("Python exe path: " + pythonExePath);
                Debug.WriteLine("Python script path: " + scriptFile);

                if (!File.Exists(scriptFile))
                {
                    //MessageBox.Show("Không tìm thấy file script python: " + scriptFile, "Python Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("Không tìm thấy script python: " + scriptFile);
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    Debug.WriteLine("Python process đã chạy rồi.");
                    return;
                }

                pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonExePath;
                pythonProcess.StartInfo.Arguments = $"\"{scriptFile}\"";
                //pythonProcess.StartInfo.Arguments = $"\"{scriptFile}\" {userArgument}";
                pythonProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptFile);
                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.StartInfo.CreateNoWindow = true;

                pythonProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON OUT] " + e.Data);
                };
                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON ERR] " + e.Data);
                };

                bool started = pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();

                Debug.WriteLine(started
                    ? $"Started Python process: {pythonExePath} {pythonProcess.StartInfo.Arguments}"
                    : "Failed to start Python process.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Lỗi chạy Python: " + ex.Message, "Python Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("Lỗi chạy Python: " + ex.ToString());
            }
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }
        private async void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string categoryId = cmbCategory.SelectedValue?.ToString();
            var allTopics = await topicService.GetTopicsAsync();
            var filteredTopics = allTopics.Where(t => t.CategoryId == categoryId).ToList();
            cmbTopic.DataSource = filteredTopics;
            cmbTopic.DisplayMember = "DisplayTitle";
            cmbTopic.ValueMember = "Id";
        }

        private void UpdateControlTexts()
        {
            // Gán lại text cho từng button từ Properties.Resources
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            lblPresentationFile.Text = Properties.Resources.Lbl_PresentiontationFile;
            // ... Gán cho các label/button khác tương tự
        }
        private async void UpdateCultureAndApply(string cultureCode)
        {
            try
            {
                CultureManager.CurrentCultureCode = cultureCode;
                ResourceHelper.SetCulture(cultureCode, this);
                var categories = await categoryService.GetCategoriesAsync();
                cmbCategory.DataSource = null;
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "DisplayName";
                cmbCategory.ValueMember = "Id";
                await _apiClient.SetUserLanguageAsync(userId, cultureCode);
                CustomMessageBox.ShowSuccess(
                    Properties.Resources.Message_ChangeLanguageSuccess,
                    Properties.Resources.Title_Success
                );
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(
                    Properties.Resources.Message_ChangeLanguageFailed,
                    Properties.Resources.Title_Error
                );
            }

        }
        public void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            lblPresentationFile.Text = Properties.Resources.Lbl_PresentiontationFile;
            lblOpenFile.Text = Properties.Resources.LblOpenFile;
            lblCategory.Text = Properties.Resources.Lbl_Category;
            lblTopic.Text = Properties.Resources.Lbl_Topic;
            btnSlideShow.Text = Properties.Resources.Btn_SlideShow;
            //btnExit.Text = Properties.Resources.Btn_Exit;
            lblPresentationPreview.Text = Properties.Resources.Lbl_PresentationPreview;
            //btnClose.Text = Properties.Resources.LblClose;
            btnOpen.Text = Properties.Resources.Btn_Browse;
            btnExit.Text = Properties.Resources.LblClose;
        }

        private void btnGestureControl_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm dGestureForm = new ListDefaultGestureForm(_homeForm);
            dGestureForm.Show();
            this.Hide();
        }

        private void btnTrainingGesture_Click(object sender, EventArgs e)
        {
            FormUserGesture uGestureForm = new FormUserGesture(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }
        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
        private async void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                var result = CustomMessageBox.ShowQuestion(
                    Properties.Resources.Message_LogoutConfirm,
                    Properties.Resources.Title_Confirmation
                );

                if (result != DialogResult.Yes)
                {
                    return;
                }

                btnLogout.Enabled = false;
                btnProfile.Enabled = false;

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[PresentationForm] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                // Cleanup resources
                CleanupResources();

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[PresentationForm] ✅ Logout successful");

                    // ✅ Update loading message
                    string successMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? "✅ Đăng xuất thành công!"
                        : "✅ Logout successful!";
                    UpdateLoadingText(successMsg);

                    await Task.Delay(1000);

                    HideLoading();

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new LoginForm();
                    _homeForm?.Close();
                    this.Hide();
                    loginForm.Show();
                    this.Dispose();
                }
                else
                {
                    HideLoading();

                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_LogoutFailed,
                        Properties.Resources.Title_Error
                    );

                    btnLogout.Enabled = true;
                    btnProfile.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                HideLoading();

                CustomMessageBox.ShowError(
                    $"{Properties.Resources.Message_LogoutError}: {ex.Message}",
                    Properties.Resources.Title_ConnectionError
                );

                btnLogout.Enabled = true;
                btnProfile.Enabled = true;
            }
        }

        // ✅ THÊM PROFILE HANDLER
        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Cách 2: Truyền this (Form) thay vì _homeForm (HomeUser)
                ProfileForm profileForm = new ProfileForm(userId, _homeForm);

                this.Hide();

                profileForm.Show();

                profileForm.FormClosed += (s, args) =>
                {
                    // Quay lại Form1 sau khi đóng Profile
                    this.Show();
                };
            }
            catch (Exception ex)
            {
                string errorMessage = CultureManager.CurrentCultureCode == "vi-VN"
                    ? $"Không thể mở trang profile: {ex.Message}"
                    : $"Cannot open profile page: {ex.Message}";

                CustomMessageBox.ShowError(
                    errorMessage,
                    Properties.Resources.Title_Error
                );
            }
        }
        private void CleanupResources()
        {
            try
            {
                // ✅ Stop loading animation
                spinnerTimer?.Stop();

                // Stop camera receiver
                StopCameraReceiver();

                // Stop Python process
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill();
                    pythonProcess.Dispose();
                }

                // Close PowerPoint
                if (oPPT != null && oPPT.SlideShowWindows.Count > 0)
                {
                    oPPT.SlideShowWindows[1].View.Exit();
                }

                if (oPres != null)
                {
                    oPres.Close();
                    Marshal.FinalReleaseComObject(oPres);
                    oPres = null;
                }

                if (oPPT != null)
                {
                    oPPT.Quit();
                    Marshal.FinalReleaseComObject(oPPT);
                    oPPT = null;
                }

                // Stop socket server
                server?.Stop();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up resources: {ex.Message}");
            }
        }

        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            ListRequestGestureForm uGestureForm = new ListRequestGestureForm(_homeForm);
            uGestureForm.Show();
            this.Hide();

        }

        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15;
            if (spinnerAngle >= 360) spinnerAngle = 0;
            DrawSpinner();
        }

        // ✅ THÊM: Draw spinner
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

        // ✅ THÊM: Update loading text (bilingual support)
        private void UpdateLoadingText(string message = null)
        {
            if (message == null)
            {
                message = CultureManager.CurrentCultureCode.Contains("vi")
                    ? "Đang tải...\nVui lòng đợi..."
                    : "Loading...\nPlease wait...";
            }

            loadingLabel.Text = message;

            // Center the label
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

        // ✅ THÊM: Show loading
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

        /// <summary>
        /// ✅ Đợi camera kết nối và nhận frame đầu tiên
        /// </summary>
        private async Task WaitForCameraConnectionAsync()
        {
            int timeout = 15000; // 15 seconds timeout
            int elapsed = 0;
            int checkInterval = 200;

            while (elapsed < timeout)
            {
                // ✅ Kiểm tra xem đã nhận frame đầu tiên chưa
                if (firstCameraFrameReceived)
                {
                    Debug.WriteLine("✅ Camera connected and first frame received!");
                    return;
                }

                await Task.Delay(checkInterval);
                elapsed += checkInterval;

                // ✅ Cập nhật progress
                if (elapsed % 2000 == 0) // Mỗi 2 giây
                {
                    string waitMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? $"🎥 Đang chờ camera... ({elapsed / 1000}s)"
                        : $"🎥 Waiting for camera... ({elapsed / 1000}s)";
                    UpdateLoadingText(waitMsg);
                }
            }

            // ✅ Timeout - Hiển thị warning nhưng vẫn tiếp tục
            string timeoutMsg = CultureManager.CurrentCultureCode.Contains("vi")
                ? "⚠️ Không thể kết nối camera sau 15 giây.\nVui lòng kiểm tra lại."
                : "⚠️ Camera connection timeout after 15 seconds.\nPlease check your camera.";

            Debug.WriteLine("⚠️ Camera connection timeout!");

            // Hiển thị warning trong UI
            this.Invoke(new Action(() =>
            {
                CustomMessageBox.ShowWarning(timeoutMsg, "Camera Warning");
            }));
        }
    }
}