using GestPipePowerPonit.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class UserGestureConfigService
    {
        private readonly HttpClient _httpClient;

        public UserGestureConfigService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7219/");
        }

        // Lấy danh sách gesture config của user (đã đăng nhập)
        public async Task<List<UserGestureConfigDto>> GetUserGesturesAsync(string userId)
        {
            // Đảm bảo API backend có endpoint phù hợp, ví dụ:
            // GET /api/usergestureconfig/user/{userId}
            return await _httpClient.GetFromJsonAsync<List<UserGestureConfigDto>>($"/api/usergestureconfig/user/{userId}");
        }

        //// Lấy chi tiết động tác (nếu cần show chi tiết)
        //public async Task<GestureDetailsDto> GetGestureDetailAsync(string gestureId)
        //{
        //    // Đảm bảo API backend có endpoint phù hợp, ví dụ:
        //    // GET /api/usergestureconfig/{gestureId}
        //    return await _httpClient.GetFromJsonAsync<GestureDetailsDto>($"/api/usergestureconfig/{gestureId}");
        //}

        public async Task<GestureDetailsDto> GetGestureDetailAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<GestureDetailsDto>($"/api/usergestureconfig/{id}/detail");
        }

        public string GetGestureDescription(GestureDetailsDto gesture)
        {
            string leftHand = GetHandState(gesture.VectorData.Fingers, 0, "Left hand");
            string rightHand = GetHandState(gesture.VectorData.Fingers, 5, "Right hand");

            string direction = "";
            if (gesture.Type == "Static")
            {
                direction = "Keep right hand still for 1 second.";
            }
            else
            {
                if (gesture.VectorData.MainAxisX == 1)
                    direction = gesture.VectorData.DeltaX > 0 ? "Then wave hand from left to right." : "Then wave hand from right to left.";
                else if (gesture.VectorData.MainAxisY == 1)
                    direction = gesture.VectorData.DeltaY > 0 ? "Then wave hand from top to bottom." : "Then wave hand from bottom to top.";
            }
            string finish = "Left hand straighten fingers to finish the movement.";

            //return $"{leftHand}\n{rightHand}\n{direction}\n{finish}";
            return $"{leftHand}{Environment.NewLine}" +
           $"{rightHand}{Environment.NewLine}" +
           $"{direction}{Environment.NewLine}" +
           $"{finish}";
        }

        private string GetHandState(int[] fingers, int start, string handLabel)
        {
            string[] fingerNames = { "thumb", "index", "middle", "ring", "pinky" };
            List<string> open = new List<string>();
            List<string> closed = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if (fingers[start + i] == 1)
                    open.Add(fingerNames[i]);
                else
                    closed.Add(fingerNames[i]);
            }
            if (open.Count == 0)
                return $"{handLabel} clenched.";
            if (closed.Count == 0)
                return $"{handLabel} all fingers open.";
            return $"{handLabel} open {string.Join(" + ", open)}.";
        }

        // Hàm instruction dạng bảng
        public string GetInstructionTable(GestureDetailsDto gesture)
        {
            string[] fingerNames = {
            "Thumb", "Index Finger", "Middle Finger", "Ring Finger", "Little Finger"
            };

            string[] leftHandState = new string[5];
            string[] rightHandState = new string[5];
            for (int i = 0; i < 5; i++)
            {
                leftHandState[i] = gesture.VectorData.Fingers[i] == 1 ? "Open" : "Close";
                rightHandState[i] = gesture.VectorData.Fingers[i + 5] == 1 ? "Open" : "Close";
            }

            string direction = "";
            if( gesture.Type == "Static")
            {
                direction = "Stand Still";
            }
            else
            {
                if (gesture.VectorData.MainAxisX == 1)
                    direction = gesture.VectorData.DeltaX > 0 ? "Left to Right" : "Right to Left";
                else if (gesture.VectorData.MainAxisY == 1)
                    direction = gesture.VectorData.DeltaY > 0 ? "Top to Bottom" : "Bottom to Top";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{"",-16}{"Left Hands",-12}{"Right Hands",-12}");
            for (int i = 0; i < 5; i++)
                sb.AppendLine($"{fingerNames[i],-16}{leftHandState[i],-12}{rightHandState[i],-12}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"{"Direction:",-16}{direction}");
            return sb.ToString();
        }
    }
}