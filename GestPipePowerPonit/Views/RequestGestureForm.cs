using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit.Views
{
    public partial class RequestGestureForm : Form
    {
        private string userId;
        private List<UserGestureRequestDto> pendingRequests = new List<UserGestureRequestDto>();
        private bool isShowingUserGesture;
        public bool RequestSentSuccessfully { get; private set; } = false;

        public RequestGestureForm(string userId, bool isShowingUserGesture = false)
        {
            InitializeComponent();
            this.userId = userId;
            this.isShowingUserGesture = isShowingUserGesture;
            this.Load += RequestGestureForm_Load;
            ApplyLanguage();

            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.isShowingUserGesture = isShowingUserGesture;
        }

        private async void RequestGestureForm_Load(object sender, EventArgs e)
        {
            pendingRequests.Clear();

            var defaultGestureService = new DefaultGestureService();
            var userGestureService = new UserGestureConfigService();
            var requestService = new UserGestureRequestService();
            var userService = new UserService();
            var profileService = new ProfileService();

            try
            {
                string username = "Unknown User"; // Default fallback
                var profileResponse = await profileService.GetProfileAsync(userId);

                if (profileResponse.Success && profileResponse.Data?.Profile != null)
                {
                    username = profileResponse.Data.Profile.FullName;

                    // Nếu FullName rỗng, fallback sang email
                    if (string.IsNullOrWhiteSpace(username) && profileResponse.Data.User != null)
                    {
                        username = !string.IsNullOrWhiteSpace(profileResponse.Email)
                            ? profileResponse.Email
                            : profileResponse.UserId;
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ [FormRequestGestures] Failed to load profile: {profileResponse.Message}");
                    username = $"User {userId.Substring(0, Math.Min(8, userId.Length))}";
                }

                List<string> gestureNames = new List<string>();
                if (isShowingUserGesture)
                {
                    var userGesture = await userGestureService.GetUserGesturesAsync(userId);
                    foreach (var config in userGesture)
                    {
                        var request = await requestService.GetLatestRequestByConfigAsync(config.Id, userId);
                        if (request != null && request.Status != null &&
                           (request.Status.ContainsKey("en") && request.Status["en"] == "Customed" ||
                            request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đã chỉnh")))
                        {
                            gestureNames.Add(I18nHelper.GetLocalized(config.Name));
                            pendingRequests.Add(request);
                        }
                    }
                }
                else
                {
                    var configs = await defaultGestureService.GetDefaultGesturesAsync();
                    foreach (var config in configs)
                    {
                        var request = await requestService.GetLatestRequestByConfigAsync(config.Id, userId);
                        if (request != null && request.Status != null &&
                           (request.Status.ContainsKey("en") && request.Status["en"] == "Customed" ||
                            request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đã chỉnh")))
                        {
                            gestureNames.Add(I18nHelper.GetLocalized(config.Name));
                            pendingRequests.Add(request);
                        }
                    }
                }

                int actualRequestCount = pendingRequests.Count;

                if (pendingRequests.Count > 0)
                {
                    var firstRequest = pendingRequests[0];
                    string descriptionPrefix = Properties.Resources.lblDescriptionRequestPrefix;
                    var gestures = gestureNames; // List<string> các tên cử chỉ

                    int gesturesPerLine = 3;
                    int maxGesturesShow = 9; // tối đa 9 cử chỉ, dài hơn thì "..."

                    var toShow = gestures.Take(maxGesturesShow).ToList();
                    List<string> lines = new List<string>();
                    for (int i = 0; i < toShow.Count; i += gesturesPerLine)
                    {
                        var part = toShow.Skip(i).Take(gesturesPerLine);
                        lines.Add(string.Join(", ", part));
                    }

                    if (gestures.Count > maxGesturesShow)
                        lines.Add("...");

                    string descriptionBody = string.Join("<br>", lines);

                    lblUserValue.Text = username;
                    lblDescriptionValue.Text = descriptionPrefix + "<br>" + descriptionBody;
                    lblRequestDateValue.Text = firstRequest.CreatedAt.ToString("dd-MM-yyyy");

                    lblRequestNumberValue.Text = actualRequestCount.ToString();

                    string status = GetLocalizedStatus(firstRequest.Status);
                    lblStatusValue.Text = status;

                    btnStartRequest.Enabled = true;
                }
                else
                {
                    lblUserValue.Text = username;
                    lblDescriptionValue.Text = I18nHelper.GetString(
                        "No custom gesture request found.",
                        "Không tìm thấy cử chỉ tùy chỉnh"
                    );
                    lblRequestDateValue.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    lblRequestNumberValue.Text = actualRequestCount.ToString();
                    lblStatusValue.Text = "N/A";
                    btnStartRequest.Enabled = false;
                    btnStartRequest.FillColor = System.Drawing.Color.Gray;
                    btnStartRequest.Text = I18nHelper.GetString("Error request", "Lỗi gửi");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error loading data: {ex.Message}", "Loading Error");
                this.Close();
            }
        }

        private string GetLocalizedStatus(Dictionary<string, string> statusDict)
        {
            if (statusDict == null || statusDict.Count == 0)
                return "N/A";

            string currentLang = GestPipePowerPonit.CultureManager.CurrentCultureCode.Split('-')[0];

            if (statusDict.ContainsKey(currentLang))
                return statusDict[currentLang];

            if (statusDict.ContainsKey("en"))
                return statusDict["en"];

            return statusDict.Values.FirstOrDefault() ?? "N/A";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Start Training (không còn upload ở đây nữa)
        //private async void btnRequest_Click(object sender, EventArgs e)
        //{
        //    if (pendingRequests.Count == 0)
        //    {
        //        CustomMessageBox.ShowWarning(
        //            "No pending gestures available for training!",
        //            "No Training Available"
        //        );
        //        return;
        //    }

        //    try
        //    {
        //        var userService = new UserService();
        //        var gestureRequestService = new UserGestureRequestService();
        //        bool overallSuccess = true;

        //        btnStartRequest.Enabled = false;
        //        btnStartRequest.Text = "Processing...";
        //        btnStartRequest.FillColor = System.Drawing.Color.Orange;

        //        // Đổi trạng thái tất cả request sang Training
        //        foreach (var request in pendingRequests)
        //        {
        //            var trainingSuccess = await gestureRequestService
        //                .SetRequestStatusToTrainingAsync(request.UserGestureConfigId);

        //            if (!trainingSuccess)
        //            {
        //                overallSuccess = false;
        //            }
        //        }

        //        var countSuccess = await userService.IncrementRequestCountAsync(userId);
        //        var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

        //        if (overallSuccess && countSuccess && statusSuccess)
        //        {
        //            // Chỉ đánh dấu đã gửi request; upload do ListRequestGestureForm xử lý
        //            RequestSentSuccessfully = true;
        //            this.Close();
        //        }
        //        else
        //        {
        //            CustomMessageBox.ShowError(
        //                Properties.Resources.RequestErrorDetail,
        //                Properties.Resources.RequestErrorTitle
        //            );

        //            btnStartRequest.Enabled = true;
        //            btnStartRequest.Text = "🚀 Start Training";
        //            btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CustomMessageBox.ShowError(
        //            $"Unexpected error occurred:\n\n{ex.Message}\n\n" +
        //            "Please try again or contact technical support.",
        //            "System Error"
        //        );

        //        btnStartRequest.Enabled = true;
        //    }
        //}
        private async void btnRequest_Click(object sender, EventArgs e)
        {
            if (pendingRequests.Count == 0)
            {
                CustomMessageBox.ShowWarning(
                    "No pending gestures available for training! ",
                    "No Training Available"
                );
                return;
            }

            try
            {
                var userService = new UserService();
                var gestureRequestService = new UserGestureRequestService();
                bool overallSuccess = true;

                btnStartRequest.Enabled = false;
                btnStartRequest.Text = "Processing...";
                btnStartRequest.FillColor = System.Drawing.Color.Orange;

                // Đổi trạng thái tất cả request sang Training
                foreach (var request in pendingRequests)
                {
                    Console.WriteLine($"[btnRequest] Processing configId: {request.UserGestureConfigId}, userId: {userId}");

                    // ✅ THAY ĐỔI: Truyền thêm userId
                    var trainingSuccess = await gestureRequestService
                        .SetRequestStatusToTrainingAsync(request.UserGestureConfigId, userId); // ← THÊM userId

                    if (!trainingSuccess)
                    {
                        Console.WriteLine($"[btnRequest] ❌ Failed");
                        overallSuccess = false;
                    }
                    else
                    {
                        Console.WriteLine($"[btnRequest] ✅ Success");
                    }
                }

                var countSuccess = await userService.IncrementRequestCountAsync(userId);
                var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

                Console.WriteLine($"[btnRequest] countSuccess={countSuccess}, statusSuccess={statusSuccess}, overallSuccess={overallSuccess}");

                if (overallSuccess && countSuccess && statusSuccess)
                {
                    RequestSentSuccessfully = true;
                    Console.WriteLine($"[btnRequest] ✅ All success!  Closing form.. .");
                    this.Close();
                }
                else
                {
                    Console.WriteLine($"[btnRequest] ❌ Some operations failed");
                    CustomMessageBox.ShowError(
                        Properties.Resources.RequestErrorDetail,
                        Properties.Resources.RequestErrorTitle
                    );

                    btnStartRequest.Enabled = true;
                    btnStartRequest.Text = "🚀 Start Training";
                    btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[btnRequest] ❌ Exception: {ex.Message}");
                Console.WriteLine($"[btnRequest] StackTrace: {ex.StackTrace}");

                CustomMessageBox.ShowError(
                    $"Unexpected error occurred:\n\n{ex.Message}\n\n" +
                    "Please try again or contact technical support.",
                    "System Error"
                );

                btnStartRequest.Enabled = true;
                btnStartRequest.Text = "🚀 Start Training";
                btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
            }
        }

        private void ApplyLanguage()
        {
            ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);
            this.lblTitle.Text = Properties.Resources.lblRequest;
            this.lblUserTitle.Text = Properties.Resources.ProfileForm_FullName;
            this.lblDescriptionTitle.Text = Properties.Resources.lblDesRequest;
            this.lblRequestDateTitle.Text = Properties.Resources.lblRequestDate;
            this.lblStatusTitle.Text = Properties.Resources.lblStatus;
            this.btnBack.Text = Properties.Resources.Btn_Back;
            this.btnStartRequest.Text = Properties.Resources.BtnStartRequest;
            this.lblRequestNumberTitle.Text = Properties.Resources.lblRequestNumber;
        }
    }
}
