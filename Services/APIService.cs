using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using LynxUI_Main.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using JsonSerializer = System.Text.Json.JsonSerializer;


namespace LynxUI_Main.Services
{
    public class ApiService
    {
        private static readonly string BASE_URL = "http://203.162.54.169:2090";
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            var token = TokenStorage.LoadToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public static async Task RegisterAsync(RegisterModel model)
        {
            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync($"{BASE_URL}/api/auth/register", model);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Đăng ký thành công!");
            else
                MessageBox.Show("Đăng ký thất bại: " + message);
        }

        public static async Task<LoginResult> LoginAsync(LoginModel model)
        {
            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync($"{BASE_URL}/api/auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu.");
                return new LoginResult { Success = false };
            }

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("[Login] JSON: " + json);

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("token", out var tokenProp))
                {
                    MessageBox.Show("Phản hồi không hợp lệ (không có token).");
                    return new LoginResult { Success = false };
                }

                var token = tokenProp.GetString();
                if (string.IsNullOrEmpty(token))
                {
                    MessageBox.Show("Token rỗng.");
                    return new LoginResult { Success = false };
                }

                var userId = JwtHelper.ExtractUserId(token!);
                if (userId == null)
                {
                    MessageBox.Show("Không thể trích xuất userId từ token.");
                    return new LoginResult { Success = false };
                }

                TokenStorage.SaveToken(token!);
                MessageBox.Show("Đăng nhập thành công!");

                return new LoginResult
                {
                    Success = true,
                    UserId = userId.Value,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Login JSON Error] " + ex.Message);
                MessageBox.Show("Lỗi xử lý phản hồi đăng nhập.");
                return new LoginResult { Success = false };
            }
        }


        public static async Task ForgotPasswordAsync(ForgotPasswordModel model)
        {
            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync($"{BASE_URL}/api/auth/forgot-password", model);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Đã gửi mail hướng dẫn đổi mật khẩu!");
            else
                MessageBox.Show("Yêu cầu thất bại: " + message);
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var token = TokenStorage.LoadToken();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{BASE_URL}/api/user/{userId}", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("[UpdateUserAsync] Lỗi: " + errorContent);
                return false;
            }
            return true;
        }

        public async Task<List<MessageItem>> GetMessagesAsync(int chatId, int currentUserId)
        {
            try
            {
                var token = TokenStorage.LoadToken();
                if (string.IsNullOrEmpty(token)) return null;
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{BASE_URL}/api/chat/{chatId}/messages");
                if (!response.IsSuccessStatusCode) return new();

                var content = await response.Content.ReadAsStringAsync();
                var apiMsgs = JsonSerializer.Deserialize<List<ApiMessageDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiMsgs?
                    .Select(m =>
                    {
                        Debug.WriteLine($"[API] MessageId={m.MessageId}, FileUrl={m.FileUrl}");
                        return MessageMapper.MapFromApiMessage(m, currentUserId);
                    })
                    .ToList() ?? new();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetMessagesAsync] Error: {ex.Message}");
                return new();
            }
        }

        public async Task<UserItem?> GetUserByIdAsync(int id)
        {
            try
            {
                var token = TokenStorage.LoadToken();
                if (string.IsNullOrEmpty(token)) return null;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{BASE_URL}/api/User/{id}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[API Response JSON] {json}");

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string? avatarRaw = null;
                if (root.TryGetProperty("avatarUrl", out var avatarProp))
                {
                    avatarRaw = avatarProp.GetString();

                    // Nếu không phải đường dẫn hợp lệ → reset về null để dùng fallback avatar
                    if (string.IsNullOrWhiteSpace(avatarRaw) ||
                        !Uri.IsWellFormedUriString(avatarRaw, UriKind.RelativeOrAbsolute))
                    {
                        avatarRaw = null;
                    }
                }

                return new UserItem
                {
                    Id = root.GetProperty("userId").GetInt32(),
                    DisplayName = root.GetProperty("userName").GetString(),
                    FullName = root.GetProperty("fullName").GetString(),
                    AvatarUrl = avatarRaw ?? "/Assets/avatar_default.png",
                    IsOnline = root.GetProperty("isOnline").GetBoolean(),

                    Email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : "",
                    PhoneNumber = root.TryGetProperty("phone", out var phoneProp) ? phoneProp.GetString() : "",
                    Birthday = root.TryGetProperty("birthday", out var birthdayProp)
                        && birthdayProp.ValueKind == JsonValueKind.String
                        && DateTime.TryParse(birthdayProp.GetString(), out var bday)
                            ? bday
                            : null
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetUserByIdAsync] Error: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> GetCurrentUserIdAsync()
        {
            try
            {
                var token = TokenStorage.LoadToken();
                if (string.IsNullOrEmpty(token)) return null;
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{BASE_URL}/api/auth/me");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("userId", out var idProp))
                {
                    return idProp.GetInt32();
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetCurrentUserIdAsync] Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ChatListItem>> GetChatListAsync(int userId)
        {
            try
            {
                var token = TokenStorage.LoadToken();
                if (string.IsNullOrEmpty(token)) return null;
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{BASE_URL}/api/chat/list/{userId}");
                if (!response.IsSuccessStatusCode) return new();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ChatListItem>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetChatListAsync] Error: {ex.Message}");
                return new();
            }
        }

        public async Task<ChatListItem?> GetChatByIdAsync(int chatId)
        {
            var res = await _httpClient.GetAsync($"{BASE_URL}/api/chat/{chatId}");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ChatListItem>(content);
            }
            return null;
        }

        public async Task<List<UserItem>> GetFriendListAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/api/friend/{userId}/friends");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("[API Response] " + json);
            return JsonSerializer.Deserialize<List<UserItem>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        public async Task<List<UserItem>> GetIncomingRequestsAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/api/friend/incoming/{userId}");
            var json = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("[IncomingRequests JSON] " + json);

            var dtos = JsonSerializer.Deserialize<List<UserItemDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return dtos?.Select(UserItemMapper.ToUserItem).ToList() ?? new();
        }

        public async Task AcceptFriendRequestAsync(int fromUserId, int toUserId)
        {
            var dto = new { FromUserId = fromUserId, ToUserId = toUserId };
            await _httpClient.PostAsJsonAsync($"{BASE_URL}/api/friend/accept", dto);
        }

        public async Task RejectFriendRequestAsync(int fromUserId, int toUserId)
        {
            var dto = new { FromUserId = fromUserId, ToUserId = toUserId };
            await _httpClient.PostAsJsonAsync($"{BASE_URL}/api/friend/reject", dto);
        }
        public async Task<bool> SendFriendRequestAsync(int fromUserId, int toUserId)
        {
            var dto = new
            {
                FromUserId = fromUserId,
                ToUserId = toUserId
            };

            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/api/friend/send", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<(int ChatId, bool IsNew)?> CreateChatAsync(int creatorId, int targetUserId)
        {
            var client = new HttpClient();
            var req = new
            {
                CreatorId = creatorId,
                TargetUserId = targetUserId
            };

            var json = JsonConvert.SerializeObject(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync($"{BASE_URL}/api/Chat/create_chat", content);
            if (!res.IsSuccessStatusCode) return null;

            var result = await res.Content.ReadAsStringAsync();
            Debug.WriteLine($"[CreateChatAsync] Raw response: {result}");
            var obj = JsonConvert.DeserializeObject<JObject>(result);

            if (obj == null || !obj.TryGetValue("chatId", StringComparison.OrdinalIgnoreCase, out var chatIdToken) ||
                !obj.TryGetValue("isNew", StringComparison.OrdinalIgnoreCase, out var isNewToken))
            {
                Debug.WriteLine("[CreateChatAsync] Invalid response format.");
                return null;
            }

            return (chatIdToken.Value<int>(), isNewToken.Value<bool>());
        }
        public async Task<string?> UploadFileAsync(string filePath, string type, int userId)
        {
            var token = TokenStorage.LoadToken();
            if (string.IsNullOrEmpty(token)) return null;

            var fileName = Path.GetFileName(filePath);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(File.OpenRead(filePath)), "file", fileName);

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"{BASE_URL}/api/chat/upload?senderId={userId}&type={type}", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("fileUrl").GetString();
        }
    }
}
