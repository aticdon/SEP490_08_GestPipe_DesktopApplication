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
using System.Security.AccessControl;

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
        private bool isSlideShow = false;

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
        private bool _isInitializing = false;
        private WebView2 webView2_3D_External;
        private Dictionary<int, string> slide3DModelFiles = new Dictionary<int, string>();
        private System.Windows.Forms.Timer sessionTimer;


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

            sessionTimer = new System.Windows.Forms.Timer();
            sessionTimer.Interval = 1000; // Update mỗi 1 giây
            sessionTimer.Tick += SessionTimer_Tick;

            _homeForm = homeForm;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
            cmbTopic.SelectedIndexChanged += cmbTopic_SelectedIndexChanged;
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
        private void UpdateSlideLabel(PowerPoint.SlideShowWindow wn = null)
        {
            try
            {
                if (oPPT == null || oPres == null) return;

                PowerPoint.SlideShowView view;
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

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            if (_startTime.HasValue)
            {
                // ✅ Tính duration từ _startTime đã có
                TimeSpan duration = DateTime.UtcNow - _startTime.Value;

                // Format: HH:MM:SS
                lblTimeSession.Text = string.Format("{0:00}:{1:00}:{2:00}",
                    (int)duration.TotalHours,
                    duration.Minutes,
                    duration.Seconds);
            }
        }
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


        private async void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PowerPoint Files|*.pptx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 🧹 1. DỌN TOÀN BỘ SESSION CŨ
                    CleanupResources(keepPython: true);

                    // 🧠 2. Reset state logic
                    zoomSlideCount = 0;
                    gestureCounts.Clear();
                    firstCameraFrameReceived = false;

                    txtFile.Text = openFileDialog1.FileName;
                    string ext = Path.GetExtension(txtFile.Text).ToLower();

                    // 🧷 3. Đăng ký hotkey lại (nếu cần)
                    RegisterKeys();

                    // 🛰 4. Tạo SocketServer mới
                    server = new SocketServer(5006, this, HandleGestureCommand);
                    server.Start();
                    Debug.WriteLine("[SocketServer] Started on port 5006");

                    // 🎥 5. Start camera receiver
                    ShowLoading(
                        CultureManager.CurrentCultureCode.Contains("vi")
                            ? "🎥 Đang kết nối camera..." : "🎥 Connecting camera..."
                    );
                    StartCameraReceiver(6000);
                    pictureBoxCamera.Visible = true;

                    // 🐍 6. Start Python (sau khi server & camera đã sẵn sàng)
                    //await Task.Run(() => StartPythonProcess());
                    Task.Run(() => StartPythonProcess());

                    if (ext == ".pptx")
                    {
                        webView2_3D.Visible = false;
                        SetButtonsEnabled(true, false);

                        await Task.Run(() =>
                        {
                            Extract3DModels(txtFile.Text);
                            slidesWith3D = slide3DModelFiles.Keys.ToList();
                        });

                        try
                        {
                            oPPT = new PowerPoint.Application();
                            oPres = oPPT.Presentations.Open(
                                txtFile.Text,
                                MsoTriState.msoFalse,
                                MsoTriState.msoFalse,
                                MsoTriState.msoFalse
                            );

                            oPPT.SlideShowNextSlide += O_PPT_SlideShowNextSlide;
                            btnSlideShow.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                                ? $"Không thể mở PowerPoint: {ex.Message}"
                                : $"Cannot open PowerPoint: {ex.Message}";
                            Console.WriteLine(errorMsg);
                            oPPT = null;
                            oPres = null;
                            btnSlideShow.Enabled = false;
                        }
                    }

                    await WaitForCameraConnectionAsync();
                    HideLoading();

                    btnHome.Enabled = false;
                    btnGestureControl.Enabled = false;
                    btnInstruction.Enabled = false;
                    btnPresentation.Enabled = false;
                    btnCustomGesture.Enabled = false;
                    btnProfile.Enabled = false;
                }
                catch (Exception ex)
                {
                    string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? $"Lỗi: {ex.Message}"
                        : $"Error: {ex.Message}";
                    Console.WriteLine(errorMsg);
                    HideLoading();
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
        private void CheckAndShowGLBForCurrentSlide(PowerPoint.SlideShowWindow wn = null)
        {
            try
            {
                if (oPPT == null || oPres == null)
                    return;

                PowerPoint.SlideShowView view = null;

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


        private void btnSlideShow_Click(object sender, EventArgs e)
        {
            webView2_3D.Visible = false;
            pictureBoxCamera.Visible = false;
            isSlideShow = true;
            _startTime = DateTime.UtcNow;

            lblTimeSession.Text = "00:00:00";
            sessionTimer.Start();
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
                sessionTimer.Stop();
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
                isSlideShow =false;
                try
                {
                    if (oPres != null) { oPres.Close(); oPres = null; }
                    if (oPPT != null) { oPPT.Quit(); oPPT = null; }
                }
                catch { }
                CleanupResources(keepPython: false);
                lblTimeSession.Text = "00:00:00";
                _startTime = null;
            }
            else { return; }
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
            sessionTimer?.Stop();
            sessionTimer?.Dispose();
            CleanupResources(keepPython: false);
            base.OnFormClosing(e);
        }

        private void panelSlide_Paint(object sender, PaintEventArgs e) { }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _isInitializing = true;
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            pictureBoxCamera.Visible = false;
            SetButtonsEnabled(false, false);

            var categories = await categoryService.GetCategoriesAsync();

            cmbCategory.DisplayMember = "DisplayName";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = categories;

            _ = Task.Run(() => StartPythonProcess());
            // 🔁 Restore LastSelectedCategoryId
            var lastCategoryId = Properties.Settings.Default.LastSelectedCategoryId;
            if (!string.IsNullOrEmpty(lastCategoryId) &&
                categories.Any(c => c.Id == lastCategoryId))
            {
                cmbCategory.SelectedValue = lastCategoryId;
            }
            else if (categories.Count > 0)
            {
                cmbCategory.SelectedIndex = 0;
            }

            // Sau khi chọn được category -> load topic
            await LoadTopicsForSelectedCategoryAsync();
            _isInitializing = false;
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

        private void O_PPT_SlideShowNextSlide(PowerPoint.SlideShowWindow Wn)
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
                case "start_present": btnSlideShow_Click(null, null); Console.WriteLine("Slide Show"); break;
                case "end_present": btnClose_Click(null, null); Console.WriteLine("Close Slide"); break;
                case "rotate_down": btnViewTop_Click(null, null); Console.WriteLine("Rotate Down"); break;
                case "zoom_in_slide": btnZoomInSlide_Click(null, null); Console.WriteLine("Zoom In Slide"); break;
                case "zoom_out_slide": btnZoomOutSlide_Click(null, null); Console.WriteLine("Zoom Out Slide"); break;
                default: Console.WriteLine($"Nhận lệnh không xác định: {command}"); break;
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
        private void StartPythonProcess()
        {
            try
            {
                if (pythonProcess != null)
                {
                    try
                    {
                        if (!pythonProcess.HasExited)
                        {
                            Debug.WriteLine("Python process đã chạy rồi.");
                            return;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    pythonProcess.Dispose();
                    pythonProcess = null;
                }

                string pythonExePath = "python";
                string userFolder = $"user_{userId}";
                //string userFolder = "user_691d8197db57fd91994a04f3";
                string scriptFile = $@"D:\Semester9\codepython\hybrid_realtime_pipeline\code\{userFolder}\test_gesture_recognition.py";

                Debug.WriteLine("Python exe path: " + pythonExePath);
                Debug.WriteLine("Python script path: " + scriptFile);

                if (!File.Exists(scriptFile))
                {
                    Debug.WriteLine("Không tìm thấy script python: " + scriptFile);
                    return;
                }

                var proc = new Process();
                proc.StartInfo.FileName = pythonExePath;
                proc.StartInfo.Arguments = $"\"{scriptFile}\"";
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptFile);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;

                proc.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON OUT] " + e.Data);
                };
                proc.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON ERR] " + e.Data);
                };

                bool started = proc.Start();
                if (!started)
                {
                    Debug.WriteLine("Failed to start Python process.");
                    proc.Dispose();
                    return;
                }

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                pythonProcess = proc;  // 🔑 chỉ gán sau khi Start OK

                Debug.WriteLine($"Started Python process: {pythonExePath} {proc.StartInfo.Arguments}");
            }
            catch (Exception ex)
            {
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
            if (_isInitializing) return;               
            if (cmbCategory.SelectedValue == null) return;

            // 💾 Save Category vào Settings
            Properties.Settings.Default.LastSelectedCategoryId = cmbCategory.SelectedValue.ToString();
            Properties.Settings.Default.Save();

            await LoadTopicsForSelectedCategoryAsync();
        }
        private void cmbTopic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;               
            if (cmbTopic.SelectedValue == null) return;

            Properties.Settings.Default.LastSelectedTopicId = cmbTopic.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }
        private async void UpdateCultureAndApply(string cultureCode)
        {
            try
            {
                CultureManager.CurrentCultureCode = cultureCode;
                ResourceHelper.SetCulture(cultureCode, this);

                // 🔁 Lấy lại danh sách category theo ngôn ngữ mới
                var categories = await categoryService.GetCategoriesAsync();

                // Lưu lại category hiện tại (nếu có) để giữ nguyên lựa chọn
                var lastCategoryId = Properties.Settings.Default.LastSelectedCategoryId;

                cmbCategory.DataSource = null;
                cmbCategory.DisplayMember = "DisplayName";
                cmbCategory.ValueMember = "Id";
                cmbCategory.DataSource = categories;

                // Restore category đã chọn trước đó (nếu vẫn tồn tại)
                if (!string.IsNullOrEmpty(lastCategoryId) &&
                    categories.Any(c => c.Id == lastCategoryId))
                {
                    cmbCategory.SelectedValue = lastCategoryId;
                }
                else if (categories.Count > 0)
                {
                    cmbCategory.SelectedIndex = 0;
                }
                await LoadTopicsForSelectedCategoryAsync();

                await _apiClient.SetUserLanguageAsync(userId, cultureCode);
            }
            catch (Exception ex)
            {
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
            try { sessionTimer?.Stop(); } catch { }
            try { spinnerTimer?.Stop(); } catch { }
            CleanupResources(keepPython: false);
            ListDefaultGestureForm dGestureForm = new ListDefaultGestureForm(_homeForm);
            dGestureForm.Show();
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
                CleanupResources(keepPython: false);
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
                CleanupResources(keepPython: false);

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

        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                CleanupResources(keepPython: false);
                ProfileForm profileForm = new ProfileForm(userId, _homeForm);

                this.Hide();

                profileForm.Show();

                profileForm.FormClosed += (s, args) =>
                {
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
        private async Task LoadTopicsForSelectedCategoryAsync()
        {
            var categoryId = cmbCategory.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(categoryId))
            {
                cmbTopic.DataSource = null;
                return;
            }

            var allTopics = await topicService.GetTopicsAsync();
            var filteredTopics = allTopics
                .Where(t => t.CategoryId == categoryId)
                .ToList();

            cmbTopic.DisplayMember = "DisplayTitle";
            cmbTopic.ValueMember = "Id";
            cmbTopic.DataSource = filteredTopics;

            // 🔁 Restore LastSelectedTopicId nếu còn tồn tại
            var lastTopicId = Properties.Settings.Default.LastSelectedTopicId;
            if (!string.IsNullOrEmpty(lastTopicId) &&
                filteredTopics.Any(t => t.Id == lastTopicId))
            {
                cmbTopic.SelectedValue = lastTopicId;
            }
        }
        private void CleanupResources(bool keepPython)
        {
            try
            {
                try { spinnerTimer?.Stop(); } catch { }

                // Camera
                try { StopCameraReceiver(); }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Cleanup] StopCameraReceiver error: " + ex.Message);
                }

                // ✅ Python – chỉ kill nếu không keepPython
                if (!keepPython && pythonProcess != null)
                {
                    try
                    {
                        try
                        {
                            if (!pythonProcess.HasExited)
                                pythonProcess.Kill();
                        }
                        catch (InvalidOperationException)
                        {
                        }

                        pythonProcess.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[Cleanup] pythonProcess error: " + ex.Message);
                    }
                    finally
                    {
                        pythonProcess = null;
                    }
                }

                // Socket server
                if (server != null)
                {
                    try { server.Stop(); }
                    catch (Exception ex) { Debug.WriteLine("[Cleanup] SocketServer stop error: " + ex.Message); }
                    finally { server = null; }
                }

                // PowerPoint ...
                // (giữ nguyên phần oPPT, oPres như bạn đã làm)

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
            CleanupResources(keepPython: false);
            ListRequestGestureForm uGestureForm = new ListRequestGestureForm(_homeForm);
            uGestureForm.Show();
            this.Hide();

        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            // ✅ Dừng và reset timer
            sessionTimer.Stop();
            lblTimeSession.Text = "00:00:00";
            // 1. Dọn tài nguyên giống đóng form
            CleanupResources(keepPython: true);

            // 2. Reset trạng thái logic
            _startTime = null;
            firstCameraFrameReceived = false;
            gestureCounts.Clear();

            // 3. Reset UI
            txtFile.Text = string.Empty;
            lblSlide.Text = "Slide - / -";

            // Ẩn camera + 3D
            if (pictureBoxCamera.Image != null)
            {
                pictureBoxCamera.Image.Dispose();
                pictureBoxCamera.Image = null;
            }
            pictureBoxCamera.Visible = false;

            webView2_3D.Visible = false;

            // Không có file, không slideshow
            SetButtonsEnabled(false, false);

            // Bật lại các nút menu bên trái để user chọn lại
            btnHome.Enabled = true;
            btnGestureControl.Enabled = true;
            btnInstruction.Enabled = true;
            btnPresentation.Enabled = true;
            btnCustomGesture.Enabled = true;
            btnProfile.Enabled = true;
            btnOpen.Enabled = true;

            // Nếu đang hiện loading panel thì tắt luôn cho chắc
            HideLoading();

            Debug.WriteLine("[PresentationForm] Clear clicked – state reset without closing form");
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

        private void btnInstruction_Click(object sender, EventArgs e)
        {
            CleanupResources(keepPython: false);
            InstructionForm instructionForm = new InstructionForm(_homeForm);
            instructionForm.Show();
            this.Hide();
        }
    }
}