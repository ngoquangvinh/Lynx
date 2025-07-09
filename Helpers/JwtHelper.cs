using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LynxUI_Main.Helpers
{
    public static class JwtHelper
    {
        /// <summary>
        /// Giải mã JWT và lấy userId nếu tồn tại trong payload.
        /// </summary>
        /// <param name="jwtToken">Chuỗi token dạng JWT</param>
        /// <returns>userId (int) nếu tìm thấy, null nếu không</returns>
        public static int? ExtractUserId(string jwtToken)
        {
            try
            {
                var parts = jwtToken.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];

                // Padding nếu thiếu (base64url không có '=')
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                var bytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(bytes);

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("userId", out var userIdProp))
                {
                    var str = userIdProp.GetString();
                    return int.TryParse(str, out var id) ? id : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JwtHelper] Lỗi giải mã token: {ex.Message}");
                return null;
            }
        }
    }
}
