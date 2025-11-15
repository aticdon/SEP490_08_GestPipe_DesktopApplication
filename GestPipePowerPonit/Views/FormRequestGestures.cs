//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.Windows.Forms;
//////using GestPipePowerPonit.Models;
//////using GestPipePowerPonit.Models.DTOs;
//////using GestPipePowerPonit.Services;
//////using GestPipePowerPonit.I18n;
//////using System.Threading.Tasks;

//////namespace GestPipePowerPonit.Views
//////{
//////    public partial class FormRequestGestures : Form
//////    {
//////        private string userId;
//////        // Danh sách các request Pending
//////        private List<UserGestureRequestDto> pendingRequests = new List<UserGestureRequestDto>();

//////        public FormRequestGestures(string userId)
//////        {
//////            InitializeComponent();
//////            this.userId = userId;
//////            this.Load += FormRequestGestures_Load;
//////        }

//////        private async void FormRequestGestures_Load(object sender, EventArgs e)
//////        {
//////            lvRequests.Items.Clear();
//////            pendingRequests.Clear();

//////            var configService = new UserGestureConfigService();
//////            var requestService = new UserGestureRequestService();
//////            var userService = new UserService();

//////            // Lấy thông tin User: ví dụ từ API (cần bổ sung UserRequestDto/Api trả về username + request_count_today)
//////            //var userDto = await userService.GetUserInfoAsync(userId); // Viết thêm hàm này nếu chưa có
//////            //var userDto = await userService.GetUserInfoAsync(userId);
//////            //string username = userDto?.UserName ?? userId;
//////            //int requestCountToday = userDto?.RequestCountToday ?? 0;
//////            string username = "DemoUser"; // Thay thế bằng dữ liệu thực tế
//////            int requestCountToday = 2; // Thay thế bằng dữ liệu thực tế
//////            var configs = await configService.GetUserGesturesAsync(userId);

//////            // Gom mô tả hiển thị (description)
//////            List<string> gestureNames = new List<string>();

//////            foreach (var config in configs)
//////            {
//////                var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
//////                if (request != null && request.Status != null &&
//////                   (request.Status.ContainsKey("en") && request.Status["en"] == "Pending" ||
//////                    request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đang xử")))
//////                {
//////                    // Gom tên gesture để tạo description
//////                    gestureNames.Add(I18nHelper.GetLocalized(config.Name));

//////                    var item = new ListViewItem();
//////                    item.Text = I18nHelper.GetLocalized(config.Name);

//////                    // Trạng thái
//////                    string status = request.Status.ContainsKey(GestPipePowerPonit.CultureManager.CurrentCultureCode)
//////                        ? request.Status[GestPipePowerPonit.CultureManager.CurrentCultureCode]
//////                        : (request.Status.ContainsKey("en") ? request.Status["en"] : "Pending");
//////                    item.SubItems.Add(status);

//////                    // Thời gian request
//////                    item.SubItems.Add(request.CreatedAt.ToString("dd-MM-yyyy HH:mm"));

//////                    // Thêm custom subitems: Username / Request Count Today / Description
//////                    item.SubItems.Add(username);

//////                    item.SubItems.Add(requestCountToday.ToString());

//////                    item.SubItems.Add($"This request updates {I18nHelper.GetLocalized(config.Name)}");

//////                    item.Tag = request;
//////                    lvRequests.Items.Add(item);

//////                    pendingRequests.Add(request);
//////                }
//////            }

//////            // Hiển thị description tổng hợp cho tất cả pending gestures
//////            if (gestureNames.Any())
//////            {
//////                string allDesc = "This request updates " + string.Join(", ", gestureNames);
//////                // Bạn có thể show lên label dưới nếu muốn
//////                this.Text = allDesc; // (hoặc set vào một label khác)
//////            }
//////        }

//////        // Xử lý khi bấm nút Request: duyệt toàn bộ các request pending
//////        private async void btnRequest_Click(object sender, EventArgs e)
//////        {
//////            if (pendingRequests.Count == 0)
//////            {
//////                MessageBox.Show("Không có gesture nào Pending để Request!", "Thông báo");
//////                return;
//////            }

//////            var userService = new UserService();
//////            var canRequest = await userService.CheckCanRequestAsync(userId);
//////            if (!canRequest)
//////            {
//////                MessageBox.Show("Bạn đã vượt quá số lần request hoặc không được phép request nữa!", "Thông báo");
//////                return;
//////            }

//////            var gestureRequestService = new UserGestureRequestService();
//////            bool overallSuccess = true;

//////            foreach (var request in pendingRequests)
//////            {
//////                var trainingSuccess = await gestureRequestService.SetRequestStatusToTrainingAsync(request.UserGestureConfigId);
//////                if (!trainingSuccess)
//////                {
//////                    overallSuccess = false;
//////                }
//////            }

//////            var countSuccess = await userService.IncrementRequestCountAsync(userId);
//////            var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

//////            if (overallSuccess && countSuccess && statusSuccess)
//////            {
//////                MessageBox.Show("Đã chuyển trạng thái tất cả Pending thành Training/Huấn luyện và khóa chức năng request!", "Thành công");
//////                await ReloadRequests();
//////            }
//////            else
//////            {
//////                MessageBox.Show("Có lỗi khi cập nhật trạng thái, vui lòng thử lại!", "Lỗi");
//////            }
//////        }

//////        private async Task ReloadRequests()
//////        {
//////            FormRequestGestures_Load(this, EventArgs.Empty);
//////        }
//////    }
//////}

////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Windows.Forms;
////using GestPipePowerPonit.Models;
////using GestPipePowerPonit.Models.DTOs;
////using GestPipePowerPonit.Services;
////using GestPipePowerPonit.I18n;
////using System.Threading.Tasks;

////namespace GestPipePowerPonit.Views
////{
////    public partial class FormRequestGestures : Form
////    {
////        private string userId;
////        // Danh sách các request Pending
////        private List<UserGestureRequestDto> pendingRequests = new List<UserGestureRequestDto>();

////        // Tên của nút đã được đổi thành btnCancelRequest trong Designer

////        public FormRequestGestures(string userId)
////        {
////            InitializeComponent();
////            this.userId = userId;
////            this.Load += FormRequestGestures_Load;

////            // Gán sự kiện cho các nút Guna.UI2
////            this.btnCancelRequest.Click += new System.EventHandler(this.btnRequest_Click);
////            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
////        }

////        private async void FormRequestGestures_Load(object sender, EventArgs e)
////        {
////            // Bỏ lvRequests.Items.Clear()
////            pendingRequests.Clear();

////            var configService = new UserGestureConfigService();
////            var requestService = new UserGestureRequestService();
////            var userService = new UserService();

////            // Dữ liệu User (Lấy từ code cũ của bạn, sau này cần thay bằng API call thực)
////            string username = "DemoUser";

////            var configs = await configService.GetUserGesturesAsync(userId);
////            List<string> gestureNames = new List<string>();

////            // 1. Thu thập tất cả các request Pending
////            foreach (var config in configs)
////            {
////                var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
////                if (request != null && request.Status != null &&
////                   (request.Status.ContainsKey("en") && request.Status["en"] == "Pending" ||
////                    request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đang xử")))
////                {
////                    // Gom tên gesture để tạo description tổng hợp
////                    gestureNames.Add(I18nHelper.GetLocalized(config.Name));
////                    pendingRequests.Add(request);
////                }
////            }

////            // 2. Tổng hợp và cập nhật giao diện chi tiết (Guna.UI2 Labels)
////            if (pendingRequests.Count > 0)
////            {
////                var firstRequest = pendingRequests[0];

////                // Dòng mô tả tổng hợp, dựa trên ví dụ của bạn:
////                // "This request recreates the next slide, zoom in, zoom out and rotate left gestures for 3D models."
////                string descriptionList = string.Join(", ", gestureNames);
////                string allDescriptions = $"This request recreates the {descriptionList} gestures for 3D models.";

////                // Cập nhật các Label
////                this.lblUserValue.Text = username;
////                this.lblDescriptionValue.Text = allDescriptions;
////                this.lblRequestDateValue.Text = firstRequest.CreatedAt.ToString("dd-MM-yyyy");
////                this.lblRequestNumberValue.Text = pendingRequests.Count.ToString();

////                string status = firstRequest.Status.ContainsKey(GestPipePowerPonit.CultureManager.CurrentCultureCode)
////                     ? firstRequest.Status[GestPipePowerPonit.CultureManager.CurrentCultureCode]
////                     : (firstRequest.Status.ContainsKey("en") ? firstRequest.Status["en"] : "Pending");
////                this.lblStatusValue.Text = status;

////                this.btnCancelRequest.Enabled = true;
////                this.btnCancelRequest.FillColor = System.Drawing.Color.Red;
////            }
////            else
////            {
////                // Không có request pending
////                this.lblUserValue.Text = userId;
////                this.lblDescriptionValue.Text = "No pending gesture requests found.";
////                this.lblRequestDateValue.Text = DateTime.Now.ToString("dd-MM-yyyy");
////                this.lblRequestNumberValue.Text = "0";
////                this.lblStatusValue.Text = "N/A";
////                this.btnCancelRequest.Enabled = false;
////                this.btnCancelRequest.FillColor = System.Drawing.Color.Gray;
////            }
////        }

////        // Xử lý khi bấm nút Back (giả định đóng form)
////        private void btnBack_Click(object sender, EventArgs e)
////        {
////            this.Close();
////        }

////        // Xử lý khi bấm nút Request (đã đổi tên thành Cancel Request)
////        private async void btnRequest_Click(object sender, EventArgs e)
////        {
////            if (pendingRequests.Count == 0)
////            {
////                MessageBox.Show("Không có gesture nào Pending để xử lý!", "Thông báo");
////                return;
////            }

////            // Giữ nguyên logic Training/Request cũ
////            var userService = new UserService();
////            //var canRequest = await userService.CheckCanRequestAsync(userId);
////            //if (!canRequest)
////            //{
////            //    MessageBox.Show("Bạn đã vượt quá số lần request hoặc không được phép request nữa!", "Thông báo");
////            //    return;
////            //}

////            var gestureRequestService = new UserGestureRequestService();
////            bool overallSuccess = true;

////            // Đổi trạng thái tất cả các request có trạng thái pending sang Training
////            foreach (var request in pendingRequests)
////            {
////                var trainingSuccess = await gestureRequestService.SetRequestStatusToTrainingAsync(request.UserGestureConfigId);
////                if (!trainingSuccess)
////                {
////                    overallSuccess = false;
////                }
////            }

////            var countSuccess = await userService.IncrementRequestCountAsync(userId);
////            var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

////            if (overallSuccess && countSuccess && statusSuccess)
////            {
////                MessageBox.Show("Đã chuyển trạng thái tất cả Pending thành Training/Huấn luyện và khóa chức năng request!", "Thành công");
////                await ReloadRequests();
////            }
////            else
////            {
////                MessageBox.Show("Có lỗi khi cập nhật trạng thái, vui lòng thử lại!", "Lỗi");
////            }
////        }

////        private async Task ReloadRequests()
////        {
////            FormRequestGestures_Load(this, EventArgs.Empty);
////        }
////    }
////}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
//using GestPipePowerPonit.Models;
//using GestPipePowerPonit.Models.DTOs;
//using GestPipePowerPonit.Services;
//using GestPipePowerPonit.I18n;
//using System.Threading.Tasks;

//namespace GestPipePowerPonit.Views
//{
//    public partial class FormRequestGestures : Form
//    {
//        private string userId;
//        private List<UserGestureRequestDto> pendingRequests = new List<UserGestureRequestDto>();

//        public FormRequestGestures(string userId)
//        {
//            InitializeComponent();
//            this.userId = userId;
//            this.Load += FormRequestGestures_Load;

//            // Gán sự kiện cho nút Guna.UI2
//            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
//        }

//        private async void FormRequestGestures_Load(object sender, EventArgs e)
//        {
//            pendingRequests.Clear();

//            var configService = new UserGestureConfigService();
//            var requestService = new UserGestureRequestService();
//            var userService = new UserService();

//            // Lấy thông tin user, có thể đổi 'DemoUser' thành lấy từ API
//            string username = "DemoUser";
//            //int requestCountToday = ... // nếu có muốn lấy số lần request trong ngày

//            var configs = await configService.GetUserGesturesAsync(userId);
//            List<string> gestureNames = new List<string>();

//            foreach (var config in configs)
//            {
//                var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
//                if (request != null && request.Status != null &&
//                   (request.Status.ContainsKey("en") && request.Status["en"] == "Pending" ||
//                    request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đang xử")))
//                {
//                    gestureNames.Add(I18nHelper.GetLocalized(config.Name));
//                    pendingRequests.Add(request);
//                }
//            }

//            if (pendingRequests.Count > 0)
//            {
//                var firstRequest = pendingRequests[0];
//                string descriptionList = string.Join(", ", gestureNames);
//                string allDescriptions = $"This request to update {descriptionList}.";

//                lblUserValue.Text = username;
//                lblDescriptionValue.Text = allDescriptions;
//                lblRequestDateValue.Text = firstRequest.CreatedAt.ToString("dd-MM-yyyy");
//                lblRequestNumberValue.Text = pendingRequests.Count.ToString();

//                string status = firstRequest.Status.ContainsKey(GestPipePowerPonit.CultureManager.CurrentCultureCode)
//                    ? firstRequest.Status[GestPipePowerPonit.CultureManager.CurrentCultureCode]
//                    : (firstRequest.Status.ContainsKey("en") ? firstRequest.Status["en"] : "Pending");
//                lblStatusValue.Text = status;

//                btnStartRequest.Enabled = true;
//                btnStartRequest.FillColor = System.Drawing.Color.Red;
//            }
//            else
//            {
//                lblUserValue.Text = userId;
//                lblDescriptionValue.Text = "No pending gesture requests found.";
//                lblRequestDateValue.Text = DateTime.Now.ToString("dd-MM-yyyy");
//                lblRequestNumberValue.Text = "0";
//                lblStatusValue.Text = "N/A";
//                btnStartRequest.Enabled = false;
//                btnStartRequest.FillColor = System.Drawing.Color.Gray;
//            }
//        }

//        private void btnBack_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        // Xử lý Cancel Request (đổi trạng thái tất cả request Pending sang Training)
//        private async void btnRequest_Click(object sender, EventArgs e)
//        {
//            if (pendingRequests.Count == 0)
//            {
//                MessageBox.Show("Không có gesture nào Pending để xử lý!", "Thông báo");
//                return;
//            }

//            var userService = new UserService();
//            var gestureRequestService = new UserGestureRequestService();
//            bool overallSuccess = true;

//            foreach (var request in pendingRequests)
//            {
//                var trainingSuccess = await gestureRequestService.SetRequestStatusToTrainingAsync(request.UserGestureConfigId);
//                if (!trainingSuccess)
//                {
//                    overallSuccess = false;
//                }
//            }

//            var countSuccess = await userService.IncrementRequestCountAsync(userId);
//            var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

//            if (overallSuccess && countSuccess && statusSuccess)
//            {
//                MessageBox.Show("Đã chuyển trạng thái tất cả Pending thành Training/Huấn luyện và khóa chức năng request!", "Thành công");
//                await ReloadRequests();
//            }
//            else
//            {
//                MessageBox.Show("Có lỗi khi cập nhật trạng thái, vui lòng thử lại!", "Lỗi");
//            }
//        }

//        private async Task ReloadRequests()
//        {
//            FormRequestGestures_Load(this, EventArgs.Empty);
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.I18n;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Views
{
    public partial class FormRequestGestures : Form
    {
        private string userId;
        private List<UserGestureRequestDto> pendingRequests = new List<UserGestureRequestDto>();

        public FormRequestGestures(string userId)
        {
            InitializeComponent();
            this.userId = userId;
            this.Load += FormRequestGestures_Load;

            // Gán sự kiện cho nút Guna.UI2
            //this.btnStartRequest.Click += new System.EventHandler(this.btnStartRequest_Click);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
        }

        private async void FormRequestGestures_Load(object sender, EventArgs e)
        {
            pendingRequests.Clear();

            var configService = new UserGestureConfigService();
            var requestService = new UserGestureRequestService();
            var userService = new UserService();

            try
            {
                string username = "DemoUser"; // Thay thế bằng dữ liệu thực tế
                int actualRequestCount = 2; // Thay thế bằng dữ liệu thực tế

                var configs = await configService.GetUserGesturesAsync(userId);
                List<string> gestureNames = new List<string>();

                foreach (var config in configs)
                {
                    var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
                    if (request != null && request.Status != null &&
                       (request.Status.ContainsKey("en") && request.Status["en"] == "Pending" ||
                        request.Status.ContainsKey("vi") && request.Status["vi"].Contains("Đang xử")))
                    {
                        gestureNames.Add(I18nHelper.GetLocalized(config.Name));
                        pendingRequests.Add(request);
                    }
                }

                if (pendingRequests.Count > 0)
                {
                    var firstRequest = pendingRequests[0];
                    string descriptionList = string.Join(", ", gestureNames);
                    string allDescriptions = $"This request to update {descriptionList}.";

                    lblUserValue.Text = username;
                    lblDescriptionValue.Text = allDescriptions;
                    lblRequestDateValue.Text = firstRequest.CreatedAt.ToString("dd-MM-yyyy");

                    // Hiển thị số request thực tế từ API, không phải số gesture pending
                    lblRequestNumberValue.Text = actualRequestCount.ToString();

                    string status = firstRequest.Status.ContainsKey(GestPipePowerPonit.CultureManager.CurrentCultureCode)
                        ? firstRequest.Status[GestPipePowerPonit.CultureManager.CurrentCultureCode]
                        : (firstRequest.Status.ContainsKey("en") ? firstRequest.Status["en"] : "Pending");
                    lblStatusValue.Text = status;

                    btnStartRequest.Enabled = true;
                    btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
                    btnStartRequest.Text = "🚀 Start Training";
                }
                else
                {
                    lblUserValue.Text = username;
                    lblDescriptionValue.Text = "No pending gesture requests found.";
                    lblRequestDateValue.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    lblRequestNumberValue.Text = actualRequestCount.ToString();
                    lblStatusValue.Text = "N/A";
                    btnStartRequest.Enabled = false;
                    btnStartRequest.FillColor = System.Drawing.Color.Gray;
                    btnStartRequest.Text = "No Training Available";
                }
            }
            catch (Exception ex)
            {
                // Sử dụng CustomMessageBox cho error
                CustomMessageBox.ShowError($"Error loading data: {ex.Message}", "Loading Error");
                this.Close();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Xử lý khi bấm nút Start Training - BỎ INSTRUCTION DIALOG
        private async void btnRequest_Click(object sender, EventArgs e)
        {
            if (pendingRequests.Count == 0)
            {
                // Sử dụng CustomMessageBox thay vì MessageBox thông thường
                CustomMessageBox.ShowWarning(
                    "No pending gestures available for training!",
                    "No Training Available"
                );
                return;
            }

            // ❌ ĐÃ BỎ ĐOẠN CODE HIỂN THỊ CustomGestureInstructionForm
            /*
            // Hiển thị hướng dẫn trước khi bắt đầu training
            string gestureNames = string.Join(", ", pendingRequests.Select(r => "Custom Gesture"));
            var instructionResult = CustomGestureInstructionForm.ShowInstructions(gestureNames);
            
            if (instructionResult != DialogResult.OK)
            {
                return; // User cancelled the instruction dialog
            }
            */

            try
            {
                var userService = new UserService();
                var gestureRequestService = new UserGestureRequestService();
                bool overallSuccess = true;

                // Disable button để tránh click nhiều lần
                btnStartRequest.Enabled = false;
                btnStartRequest.Text = "Processing...";
                btnStartRequest.FillColor = System.Drawing.Color.Orange;

                // Đổi trạng thái tất cả các request có trạng thái pending sang Training
                foreach (var request in pendingRequests)
                {
                    var trainingSuccess = await gestureRequestService.SetRequestStatusToTrainingAsync(request.UserGestureConfigId);
                    if (!trainingSuccess)
                    {
                        overallSuccess = false;
                    }
                }

                var countSuccess = await userService.IncrementRequestCountAsync(userId);
                var statusSuccess = await userService.UpdateGestureRequestStatusAsync(userId, "disable");

                if (overallSuccess && countSuccess && statusSuccess)
                {
                    // 🎉 SỬ DỤNG CustomMessageBox.ShowSuccess
                    CustomMessageBox.ShowSuccess(
                        "Training started successfully!\n\n" +
                        "✅ All pending gestures moved to Training status\n" +
                        "✅ Request count updated\n" +
                        "✅ Request function disabled\n\n" +
                        "The training process will begin shortly.",
                        "Training Started Successfully"
                    );

                    // Đóng form hiện tại
                    this.Close();
                }
                else
                {
                    // 🚨 SỬ DỤNG CustomMessageBox.ShowError
                    CustomMessageBox.ShowError(
                        "An error occurred while starting the training process!\n\n" +
                        "Please try again or contact technical support for assistance.",
                        "Training Start Failed"
                    );

                    // Reset button state
                    btnStartRequest.Enabled = true;
                    btnStartRequest.Text = "🚀 Start Training";
                    btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
                }
            }
            catch (Exception ex)
            {
                // 🚨 SỬ DỤNG CustomMessageBox.ShowError cho exception
                CustomMessageBox.ShowError(
                    $"Unexpected error occurred:\n\n{ex.Message}\n\n" +
                    "Please try again or contact technical support.",
                    "System Error"
                );

                // Reset button state
                btnStartRequest.Enabled = true;
                btnStartRequest.Text = "🚀 Start Training";
                btnStartRequest.FillColor = System.Drawing.Color.FromArgb(0, 188, 212);
            }
        }

        private async Task ReloadRequests()
        {
            FormRequestGestures_Load(this, EventArgs.Empty);
        }
    }
}